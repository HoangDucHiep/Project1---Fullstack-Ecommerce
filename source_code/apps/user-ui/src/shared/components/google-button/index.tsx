import GoogleIcon from "apps/user-ui/src/assets/svgs/google-icon";
import React from "react";

const GoogleButton = () => {
  return (
    <div className="w-full flex justify-center">
      <div className="h-[46px] cursor-pointer border border-blue-100 flex items-center gap-2 px-3 rounded my-2 bg-[rgba(210, 227, 252, 0.3)]">
        <span className="inline-flex w-5 h-5 sm:w-6 sm:h-6 shrink-0 [&>svg]:w-full [&>svg]:h-full">
          <GoogleIcon />
        </span>
        <span className="text-[16px] opacity-[.8] font-Poppins">
          Sign In with Google
        </span>
      </div>
    </div>
  );
};

export default GoogleButton;
