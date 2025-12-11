import React from "react";

const ProfileIcon = ({ className = "w-6 h-6" }: { className?: string }) => {
  return (
    <svg
      className={className}
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      viewBox="0 0 24 24"
      xmlns="http://www.w3.org/2000/svg"
    >
      <circle cx="12" cy="7" r="3.5" />
      <path d="M6 18c0-3 2.5-5.5 6-5.5s6 2.5 6 5.5c0 1.5-1 3-2.5 3.5H8.5C7 21 6 19.5 6 18z" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  );
};

export default ProfileIcon;
