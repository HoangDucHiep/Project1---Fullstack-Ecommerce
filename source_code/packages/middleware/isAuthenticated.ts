import prisma from "@packages/libs/prisma";
import { NextFunction, Request, Response } from "express";
import jwt from "jsonwebtoken";

const isAuthenticated = async (
  req: any,
  res: Response,
  next: NextFunction
) => {
  try {
    const token =
      req.cookies["access_token"] ||
      req.cookies["seller_access_token"] ||
      req.headers.authorization?.split(" ")[1];

    if (!token) {
      return res.status(401).json({
        success: false,
        message: "Chưa xác thực đăng nhập. Vui lòng đăng nhập lại.",
      });
    }

    // verify token
    const decoded = jwt.verify(
      token,
      process.env.ACCESS_TOKEN_SECRET as string
    ) as { id: string; role: string };

    if (!decoded) {
      return res.status(401).json({
        success: false,
        message: "Token không hợp lệ. Vui lòng đăng nhập lại.",
      });
    }

    let account;

    if (decoded.role === "user") {
      account = await prisma.users.findUnique({ where: { id: decoded.id } });
      req.user = account;
    } else if (decoded.role === "seller") {
      account = await prisma.sellers.findUnique({
        where: { id: decoded.id },
        include: { shop: true },
      });
      req.seller = account;
    }

    if (!account) {
      return res.status(401).json({
        success: false,
        message: "Tài khoản không tồn tại. Vui lòng đăng nhập lại.",
      });
    }

    req.role = decoded.role;

    return next();
  } catch (error) {
    return res.status(401).json({
      success: false,
      message: "Lỗi xác thực đăng nhập. Vui lòng đăng nhập lại.",
    });
  }
};

export default isAuthenticated;
