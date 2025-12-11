import { NextFunction, Request, Response } from "express";
import {
  checkOtpRestrictions,
  handleForgotPassword,
  sendOtp,
  trackOtpRequests,
  verifyForgotPasswordOtp,
  verifyOtp,
} from "../utils/auth.helper";
import prisma from "@packages/libs/prisma";
import { AuthError, ValidationError } from "@packages/error-handler";
import bcrypt from "bcryptjs";
import jwt, { JsonWebTokenError } from "jsonwebtoken";
import { setCookie } from "../utils/cookies/setCookie";
import exp from "constants";

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
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    const refreshToken = req.cookies.refresh_token;

    if (!refreshToken) {
      return new ValidationError(
        "Chưa xác thực đăng nhập. Không có refresh token. Vui lòng đăng nhập lại."
      );
    }

    const decoded = jwt.verify(
      refreshToken,
      process.env.REFRESH_TOKEN_SECRET as string
    ) as { id: string; role: string };

    if (!decoded || !decoded.id || !decoded.role) {
      return new JsonWebTokenError("Refresh token không hợp lệ.");
    }

    //let account;
    // if (decoded.role === "user")
    const user = await prisma.users.findUnique({ where: { id: decoded.id } });

    if (!user) {
      return new AuthError("Tài khoản không tồn tại.");
    }

    const newAccessToken = jwt.sign(
      { id: decoded.id, role: decoded.role },
      process.env.ACCESS_TOKEN_SECRET as string,
      { expiresIn: "15m" }
    );

    setCookie(res, "access_token", newAccessToken);
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
}


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

    if (!user)
      return next(
        new ValidationError("Tài khoản với email này không tồn tại.")
      );

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
