"use client";

import { useMutation } from "@tanstack/react-query";
import axios, { AxiosError } from "axios";
import Link from "next/link";
import { useRouter } from "next/navigation";
import React, { useRef, useState } from "react";
import { useForm } from "react-hook-form";
import toast from "react-hot-toast";

type FormData = {
  email: string;
  password: string;
};

const ForgotPassword = () => {
  const [step, setStep] = useState<"email" | "otp" | "reset">("email");
  const [otp, setOtp] = useState(["", "", "", ""]);
  const [userEmail, setUserEmail] = useState<string | null>(null);
  const [canResend, setCanResend] = useState(true);
  const [timer, setTimer] = useState(60);
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);
  const [serverError, setServerError] = useState<string | null>(null);
  const router = useRouter();

  const startResendTimer = () => {
    const interval = setInterval(() => {
      setTimer((prev) => {
        if (prev === 1) {
          clearInterval(interval);
          setCanResend(true);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>();

  const requestOtpMutation = useMutation({
    mutationFn: async ({ email }: { email: string }) => {
      const response = await axios.post(
        `${process.env.NEXT_PUBLIC_SERVER_URI}/api/forgot-password-user`,
        { email }
      );
      return response.data;
    },
    onSuccess: (_, { email }) => {
      setUserEmail(email);
      setStep("otp");
      setServerError(null);
      setCanResend(false);
      setTimer(60);
      startResendTimer();
    },
    onError: (error: AxiosError) => {
      const errorMessage =
        (error.response?.data as { message: string })?.message ||
        "An error occurred. Please try again.";
      setServerError(errorMessage);
    },
  });

  const verifyOtpMutation = useMutation({
    mutationFn: async () => {
      if (!userEmail) return;
      const response = await axios.post(
        `${process.env.NEXT_PUBLIC_SERVER_URI}/api/verify-forgot-password-user`,
        { email: userEmail, otp: otp.join("") }
      );
      return response.data;
    },
    onSuccess: () => {
      setStep("reset");
      setServerError(null);
    },
    onError: (error: AxiosError) => {
      const errorMessage =
        (error.response?.data as { message: string })?.message ||
        "An error occurred. Please try again.";
      setServerError(errorMessage || "Invalid OTP. Please try again.");
    },
  });

  const resetPasswordMutation = useMutation({
    mutationFn: async ({ password }: { password: string }) => {
      if (!password) return;
      const response = await axios.post(
        `${process.env.NEXT_PUBLIC_SERVER_URI}/api/reset-password-user`,
        { email: userEmail, newPassword: password }
      );
      return response.data;
    },
    onSuccess: () => {
      setStep("email");
      toast.success(
        "Password reset successful. Please login with your new password.",
      );
      setServerError(null);
      router.push("/login");
    },
    onError: (error: AxiosError) => {
      const errorMessage = (error.response?.data as { message: string })
        ?.message;
      setServerError(
        errorMessage || "Failed to reset password. Please try again."
      );
    },
  });

  const handleOtpChange = (index: number, value: string) => {
    if (!/^[0-9]$/.test(value) && value !== "") {
      return;
    }

    const newOtp = [...otp];
    newOtp[index] = value;
    setOtp(newOtp);

    if (value && index < inputRefs.current.length - 1) {
      inputRefs.current[index + 1]?.focus();
    }
  };

  const handleOtpKeyDown = (
    index: number,
    e: React.KeyboardEvent<HTMLInputElement>
  ) => {
    if (e.key === "Backspace" && !otp[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  const onSubmitEmail = ({ email }: { email: string }) => {
    requestOtpMutation.mutate({ email });
  };

  const onSubmitPassword = (data: FormData) => {
    resetPasswordMutation.mutate({ password: data.password });
  };

  return (
    <div className="w-full py-10 min-h-[85vh] bg-[#f1f1f1]">
      <h1 className="text-4xl font-Poppins font-semibold text-black text-center">
        Forgot Password
      </h1>
      <p className="text-center text-lg font-medium py-3 text-[#00000099]">
        Home . Forgot-Password
      </p>

      <div className="w-full flex justify-center">
        <div className="md:w-[480px] p-8 bg-white shadow rounded-lg">
          {step === "email" && (
            <>
              <h3 className="text-3xl font-semibold text-center mb-2">
                Enter your email to reset password
              </h3>
              <p className="text-center text-gray-500 mb-4">
                Go back to?{" "}
                <Link href={"/login"} className="text-blue-500">
                  Login
                </Link>
              </p>

              <form onSubmit={handleSubmit(onSubmitEmail)}>
                <label className="block text-gray-700 mb-1">Email</label>
                <input
                  type="email"
                  placeholder="support@shopdev.com"
                  className="w-full p-2 border border-gray-300 outline-0 !rounded mb-1"
                  {...register("email", {
                    required: "Email is required",
                    pattern: {
                      value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                      message: "Invalid email address",
                    },
                  })}
                />
                {errors.email && (
                  <p className="text-red-500 text-sm">
                    {String(errors.email.message)}
                  </p>
                )}

                <button
                  type="submit"
                  disabled={requestOtpMutation.isPending}
                  className="w-full text-lg cursor-pointer mt-4 bg-black text-white py-2 rounded-lg"
                >
                  {requestOtpMutation.isPending
                    ? "Submitting..."
                    : "Request OTP"}
                </button>

                {serverError && (
                  <p className="text-red-500 text-sm mt-2">{serverError}</p>
                )}
              </form>
            </>
          )}
          {step === "otp" && (
            <>
              <h3 className="text-xl font-semibold text-center mb-4">
                Enter OTP
              </h3>
              <div className="flex justify-center gap-6">
                {otp?.map((digit, index) => (
                  <input
                    type="text"
                    key={index}
                    ref={(el) => {
                      if (el) inputRefs.current[index] = el;
                    }}
                    maxLength={1}
                    className="w-12 h-12 text-center border border-gray-300 outline-none !rounded"
                    value={digit}
                    onChange={(e) => handleOtpChange(index, e.target.value)}
                    onKeyDown={(e) => handleOtpKeyDown(index, e)}
                  />
                ))}
              </div>
              <button
                className="w-full mt-4 text-lg cursor-pointer bg-blue-500 text-white py-2 rounded-lg"
                disabled={verifyOtpMutation.isPending}
                onClick={() => verifyOtpMutation.mutate()}
              >
                {verifyOtpMutation.isPending ? "Verifying..." : "Verify OTP"}
              </button>

              {canResend ? (
                <button
                  className="text-blue-600 underline mt-2"
                  onClick={() => {
                    if (!userEmail) return;
                    setServerError(null);
                    setCanResend(false);
                    setTimer(60);
                    startResendTimer();
                    requestOtpMutation.mutate({ email: userEmail });
                  }}
                >
                  Resend OTP
                </button>
              ) : (
                `Resend OTP in ${timer} sec`
              )}
              {serverError && (
                <p className="text-red-500 text-sm mt-2">{serverError}</p>
              )}
            </>
          )}
          {step === "reset" && (
            <>
              <h3 className="text-xl font-semibold text-center mb-4">
                Reset Password
              </h3>
              <form onSubmit={handleSubmit(onSubmitPassword)}>
                <label className="block text-gray-700 mb-1">New Password</label>
                <input
                  type="password"
                  placeholder="Enter new password"
                  className="w-full p-2 border border-gray-300 outline-0 !rounded mb-3"
                  {...register("password", {
                    required: "Password is required",
                    minLength: {
                      value: 6,
                      message: "Password must be at least 6 characters long",
                    },
                  })}
                />
                {errors.password && (
                  <p className="text-red-500 text-sm">
                    {String(errors.password.message)}
                  </p>
                )}

                <button
                  type="submit"
                  className="w-full text-lg cursor-pointer mt-4 bg-black text-white py-2 rounded-lg"
                  disabled={resetPasswordMutation.isPending}
                >
                  {resetPasswordMutation.isPending
                    ? "Resetting..."
                    : "Reset Password"}
                </button>

                {serverError && (
                  <p className="text-red-500 text-sm mt-2">{serverError}</p>
                )}
              </form>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;

