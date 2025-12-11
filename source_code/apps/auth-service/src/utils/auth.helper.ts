import { NextFunction } from "express";
import crypto from "crypto";
import { sendEmail } from "./sendMail";
import { ValidationError } from "@packages/error-handler";
import redis from "@packages/libs/redis";

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export const validateRegistrationData = (
  data: any,
  userType: "user" | "seller"
) => {
  const { name, email, password, phone_number, country } = data;
  if (
    !name ||
    !email ||
    !password ||
    (userType === "seller" && (!phone_number || !country))
  ) {
    return new ValidationError("Missing required fields");
  }

  if (!emailRegex.test(email)) {
    return new ValidationError("Invalid email format");
  }

  // NOT DONE
  return true;
};

export const checkOtpRestrictions = async (email: string, next: NextFunction) => {
  if (await redis.get(`otp_lock:${email}`)) {
    return next(
      new ValidationError(
        "Bạn đã vượt quá số lần thử mã OTP. Vui lòng thử lại sau 30 phút."
      )
    );
  }

  if (await redis.get(`otp_spam_lock:${email}`)) {
    return next(
      new ValidationError(
        "Bạn đã gửi quá nhiều yêu cầu mã OTP. Vui lòng thử lại sau 1 giờ."
      )
    );
  }

  if (await redis.get(`otp_cooldown:${email}`)) {
    return next(
      new ValidationError(
        "Vui lòng chờ 1 phút trước khi yêu cầu mã OTP mới."
      )
    )
  }
};

export const trackOtpRequests = async (email: string, next: NextFunction) => {
  const otpRequestKey = `otp_request_count:${email}`;

  let otpRequests = parseInt((await redis.get(otpRequestKey)) || "0");

  if (otpRequests >= 2) {
    await redis.set(`otp_spam_lock:${email}`, "1", "EX", 3600); // 1 hour lock
    return next(
      new ValidationError(
        "Too many OTP requests. Please wait 1 hour before trying again."
      )
    );
  }

  await redis.set(otpRequestKey, (otpRequests + 1).toString(), "EX", 3600); // Count resets after 1 hour
};

export const sendOtp = async (
  name: string,
  email: string,
  template: string
) => {
  const otp = crypto.randomInt(1000, 9999).toString();

  await sendEmail(email, "Xác minh Email - Mã OTP", template, { name, otp });

  await redis.set(`otp:${email}`, otp, "EX", 5 * 60); // OTP valid for 5 minutes
  await redis.set(`otp_cooldown:${email}`, "true", "EX", 60); // 1 minutes cooldown
};


export const verifyOtp = async (email: string, otp: string, next: NextFunction) => {
  const storedOtp = await redis.get(`otp:${email}`);

  if (!storedOtp) {
    throw new ValidationError("Mã OTP đã hết hạn hoặc không hợp lệ. Vui lòng yêu cầu mã mới.");
  }

  const failedAttemptsKey = `otp_attempts:${email}`;
  const failedAttempts = parseInt((await redis.get(failedAttemptsKey)) || "0");

  if (storedOtp !== otp) {
    if (failedAttempts >= 2) {
      await redis.set(`otp_lock:${email}`, "locked", "EX", 1800);
      await redis.del(`otp:${email}`, failedAttemptsKey);

      throw new ValidationError("Bạn đã vượt quá số lần thử mã OTP. Vui lòng thử lại sau 30 phút.");
    }

    await redis.set(failedAttemptsKey, failedAttempts + 1, "EX", 300);
    throw new ValidationError(`Mã OTP không hợp lệ. Còn ${2 - failedAttempts} lần thử.`);
  }

  await redis.del(`otp:${email}`, failedAttemptsKey);
  return true;
}
