import * as React from "react";

const StripeLogo = (props: any) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 100 100"
    width="24"
    height="24"
    fill="none"
    {...props}
  >
    {/* Background rounded rectangle */}
    <rect width="100" height="100" rx="12" fill="#6366F1" />

    {/* Letter S */}
    <text
      x="50"
      y="60"
      textAnchor="middle"
      dominantBaseline="middle"
      fontSize="70"
      fontWeight="900"
      fill="white"
      fontFamily="system-ui, -apple-system, sans-serif"
      letterSpacing="-2"
    >
      S
    </text>
  </svg>
);

export default StripeLogo;
