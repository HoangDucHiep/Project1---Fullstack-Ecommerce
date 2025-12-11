import { Response } from "express";

export const setCookie = (res: Response, name: string, value: string) => {
  console.log(`Setting cookie: ${name} = ${value.substring(0, 20)}...`);
  res.cookie(name, value, {
    httpOnly: true,
    secure: false,
    sameSite: 'lax', // Use 'lax' for development with HTTP
    maxAge: 7 * 24 * 60 * 60 * 1000, // 7 days
  });
};
