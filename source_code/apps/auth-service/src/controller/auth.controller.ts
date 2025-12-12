import { NextFunction, Request, Response } from "express";
import {
  checkOtpRestrictions,
  handleForgotPassword,
  sendOtp,
  trackOtpRequests,
  validateRegistrationData,
  verifyForgotPasswordOtp,
  verifyOtp,
} from "../utils/auth.helper";
import prisma from "@packages/libs/prisma";
import { AuthError, ValidationError } from "@packages/error-handler";
import bcrypt from "bcryptjs";
import jwt, { JsonWebTokenError } from "jsonwebtoken";
import { setCookie } from "../utils/cookies/setCookie";
import exp from "constants";
import Stripe from "stripe";

const stripe = new Stripe(process.env.STRIPE_SECRET_KEY!, {
  apiVersion: "2025-11-17.clover",
});

// Register a new user
export const userRegistration = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    //validateRegistrationData(req.body, "user");
    const { name, email } = req.body;

    const existingUser = await prisma.users.findUnique({ where: { email } });

    if (existingUser) {
      return next(new ValidationError("Tài khoản với email này đã tồn tại."));
    }

    await checkOtpRestrictions(email, next);

    await trackOtpRequests(email, next);

    await sendOtp(name, email, "user-activation-mail");

    res.status(200).json({
      message:
        "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến và xác nhận.",
    });
  } catch (error) {
    next(error);
  }
};

// OTP verification and user creation would go here
export const verifyUser = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { email, otp, password, name } = req.body;
    if (!email || !otp || !password || !name) {
      return next(new ValidationError("Yêu cầu cung cấp đầy đủ thông tin."));
    }

    const existingUser = await prisma.users.findUnique({ where: { email } });

    if (existingUser) {
      return next(new ValidationError("Tài khoản với email này đã tồn tại."));
    }

    const isOtpValid = await verifyOtp(email, otp, next);
    if (!isOtpValid) {
      return; // Stop execution if OTP is invalid
    }

    const hashedPassword = await bcrypt.hash(password, 10);

    const user = await prisma.users.create({
      data: { name, email, password: hashedPassword },
    });

    res.status(201).json({
      success: true,
      message: "Tài khoản của bạn đã được tạo thành công.",
    });
  } catch (error) {
    return next(error);
  }
};

// Login
export const loginUser = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { email, password } = req.body;

    if (!email || !password)
      return next(new ValidationError("Vui lòng cung cấp email và mật khẩu."));

    const user = await prisma.users.findUnique({ where: { email } });

    if (!user) return next(new ValidationError("Tài khoản không tồn tại."));

    const isMatch = await bcrypt.compare(password, user.password!);

    if (!isMatch)
      return next(new ValidationError("Thông tin đăng nhập không hợp lệ."));

    // Generate access and refresh tokens
    const accessToken = jwt.sign(
      { id: user.id, role: "user" },
      process.env.ACCESS_TOKEN_SECRET as string,
      { expiresIn: "15m" }
    );

    const refreshToken = jwt.sign(
      { id: user.id, role: "user" },
      process.env.REFRESH_TOKEN_SECRET as string,
      { expiresIn: "7d" }
    );

    // Clear any existing seller cookies to prevent conflicts
    res.clearCookie("seller_access_token");
    res.clearCookie("seller_refresh_token");

    // store the refresh token and access token in
    setCookie(res, "refresh_token", refreshToken);
    setCookie(res, "access_token", accessToken);

    res.status(200).json({
      message: "Login successful!",
      user: {
        id: user.id,
        name: user.name,
        email: user.email,
      },
    });
  } catch (error) {
    return;
  }
};

// refresh token
export const refreshToken = async (
  req: any,
  res: Response,
  next: NextFunction
) => {
  try {
    const refreshToken =
      req.cookies["seller_refresh_token"] ||
      req.cookies["refresh_token"] ||
      req.headers.authorization?.split(" ")[1];

    if (!refreshToken) {
      return next(
        new ValidationError(
          "Chưa xác thực đăng nhập. Không có refresh token. Vui lòng đăng nhập lại."
        )
      );
    }

    const decoded = jwt.verify(
      refreshToken,
      process.env.REFRESH_TOKEN_SECRET as string
    ) as { id: string; role: string };

    if (!decoded || !decoded.id || !decoded.role) {
      return next(new JsonWebTokenError("Refresh token không hợp lệ."));
    }

    let account;
    if (decoded.role === "user") {
      account = await prisma.users.findUnique({ where: { id: decoded.id } });
    } else if (decoded.role === "seller") {
      account = await prisma.sellers.findUnique({
        where: { id: decoded.id },
        include: { shop: true },
      });
    }

    if (!account) {
      return next(new AuthError("Tài khoản không tồn tại."));
    }

    const newAccessToken = jwt.sign(
      { id: decoded.id, role: decoded.role },
      process.env.ACCESS_TOKEN_SECRET as string,
      { expiresIn: "15m" }
    );

    if (decoded.role === "user") {
      setCookie(res, "access_token", newAccessToken);
    } else if (decoded.role === "seller") {
      setCookie(res, "seller_access_token", newAccessToken);
    }

    // also refresh the refresh token
    const newRefreshToken = jwt.sign(
      { id: decoded.id, role: decoded.role },
      process.env.REFRESH_TOKEN_SECRET as string,
      { expiresIn: "7d" }
    );

    if (decoded.role === "user") {
      setCookie(res, "refresh_token", newRefreshToken);
    } else if (decoded.role === "seller") {
      setCookie(res, "seller_refresh_token", newRefreshToken);
    }

    req.role = decoded.role;

    return res.status(201).json({
      success: true,
      message: "Refresh token successful!",
    });
  } catch (error) {
    return next(error);
  }
};

export const getUser = async (req: any, res: Response, next: NextFunction) => {
  try {
    const user = req.user;
    res.status(201).json({
      success: true,
      user,
    });
  } catch (error) {
    next(error);
  }
};

// user forgot password
export const userForgotPassword = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  await handleForgotPassword(req, res, next, "user");
};

// verify user otp for forgot password
export const verifyUserForgotPasswordOtp = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  await verifyForgotPasswordOtp(req, res, next);
};

// user reset password
export const resetUserPassword = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { email, newPassword } = req.body;

    if (!email || !newPassword) {
      return next(
        new ValidationError("Vui lòng cung cấp email và mật khẩu mới.")
      );
    }

    const user = await prisma.users.findUnique({ where: { email } });

    if (!user) {
      return next(
        new ValidationError("Tài khoản với email này không tồn tại.")
      );
    }

    // compare new password with old password
    const isSamePassword = await bcrypt.compare(newPassword, user.password!);

    if (isSamePassword) {
      return next(
        new ValidationError("Mật khẩu mới phải khác với mật khẩu cũ.")
      );
    }

    // hash the new password
    const hashedPassword = await bcrypt.hash(newPassword, 10);

    // update the password in the database
    await prisma.users.update({
      where: { email },
      data: { password: hashedPassword },
    });

    res.status(200).json({
      success: true,
      message:
        "Mật khẩu đã được đặt lại thành công. Bạn có thể đăng nhập với mật khẩu mới của mình.",
    });
  } catch (error) {
    next(error);
  }
};

// register a new seller
export const registerSeller = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    validateRegistrationData(req.body, "seller");

    const { name, email } = req.body;

    const existingSeller = await prisma.sellers.findUnique({
      where: { email },
    });

    if (existingSeller) {
      throw new ValidationError("Tài khoản với email này đã tồn tại.");
    }

    await checkOtpRestrictions(email, next);
    await trackOtpRequests(email, next);
    await sendOtp(name, email, "seller-activation");

    res.status(200).json({
      message:
        "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến và xác nhận.",
    });
  } catch (error) {
    next(error);
  }
};

// verify seller otp for forgot password
export const verifySeller = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { email, otp, password, name, phone_number, country } = req.body;

    if (!email || !otp || !password || !name || !phone_number || !country) {
      return next(new ValidationError("Vui lòng cung cấp đầy đủ thông tin."));
    }

    const existingSeller = await prisma.sellers.findUnique({
      where: { email },
    });

    if (existingSeller) {
      throw new ValidationError("Tài khoản với email này đã tồn tại.");
    }

    await verifyOtp(email, otp, next);

    const hashedPassword = await bcrypt.hash(password, 10);

    const seller = await prisma.sellers.create({
      data: {
        name,
        email,
        password: hashedPassword,
        phone_number,
        country,
      },
    });

    res.status(201).json({
      success: true,
      seller,
      message: "Tài khoản người bán của bạn đã được tạo thành công.",
    });
  } catch (error) {
    next(error);
  }
};

// create a new shop
export const createShop = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { name, bio, address, opening_hours, website, category, sellerId } =
      req.body;

    if (!name || !address || !category || !sellerId) {
      return next(new ValidationError("Vui lòng cung cấp đầy đủ thông tin."));
    }

    const shopData: any = {
      name,
      bio,
      address,
      opening_hours,
      category,
      sellerId,
    };

    if (website && website.trim() !== "") {
      shopData.website = website;
    }

    const shop = await prisma.shops.create({
      data: shopData,
    });

    res.status(201).json({
      succecss: true,
      shop,
      message: "Shop đã được tạo thành công.",
    });
  } catch (error) {
    next(error);
  }
};

// create stripe connect account link
export const createStripeConnectLink = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { sellerId } = req.body;

    if (!sellerId) return next(new ValidationError("Seller ID là bắt buộc."));

    const seller = await prisma.sellers.findUnique({ where: { id: sellerId } });

    if (!seller) return next(new ValidationError("Người bán không tồn tại."));

    const account = await stripe.accounts.create({
      type: "express",
      email: seller.email!,
      country: "GB",
      capabilities: {
        card_payments: { requested: true },
        transfers: { requested: true },
      },
    });

    await prisma.sellers.update({
      where: { id: sellerId },
      data: { stripeId: account.id },
    });

    const accountLink = await stripe.accountLinks.create({
      account: account.id,
      refresh_url: "http://localhost:3000/success",
      return_url: "http://localhost:3000/success",
      type: "account_onboarding",
    });

    res.json({
      success: true,
      url: accountLink.url,
    });
  } catch (error) {
    return next(error);
  }
};

export const loginSeller = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const { email, password } = req.body;

    if (!email || !password)
      return next(new ValidationError("Vui lòng cung cấp email và mật khẩu."));

    const seller = await prisma.sellers.findUnique({ where: { email } });

    if (!seller) return next(new ValidationError("Tài khoản không tồn tại."));

    const isMatch = await bcrypt.compare(password, seller.password!);

    if (!isMatch)
      return next(new ValidationError("Thông tin đăng nhập không hợp lệ."));

    // Generate access and refresh tokens
    const accessToken = jwt.sign(
      { id: seller.id, role: "seller" },
      process.env.ACCESS_TOKEN_SECRET as string,
      { expiresIn: "15m" }
    );

    const refreshToken = jwt.sign(
      { id: seller.id, role: "seller" },
      process.env.REFRESH_TOKEN_SECRET as string,
      { expiresIn: "7d" }
    );

    // Clear any existing user cookies to prevent conflicts
    res.clearCookie("access_token");
    res.clearCookie("refresh_token");

    // store the refresh token and access token in
    setCookie(res, "seller_refresh_token", refreshToken);
    setCookie(res, "seller_access_token", accessToken);

    res.status(200).json({
      message: "Login successful!",
      user: {
        id: seller.id,
        name: seller.name,
        email: seller.email,
      },
    });
  } catch (error) {
    return;
  }
};

export const getSeller = async (
  req: any,
  res: Response,
  next: NextFunction
) => {
  try {
    const seller = req.seller;
    res.status(201).json({
      success: true,
      seller,
    });
  } catch (error) {
    next(error);
  }
};
