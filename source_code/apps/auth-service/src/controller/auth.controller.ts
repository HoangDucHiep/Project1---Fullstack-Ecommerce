import { NextFunction, Request, Response } from "express";
import prisma from "../../../../packages/libs/prisma";

// Register a new user
export const userRegistration = async (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  try {
    //validateRegistrationData(req.body, "user");
    const { name, email } = req.body;

    const existingUser = await prisma.users
  }
};
