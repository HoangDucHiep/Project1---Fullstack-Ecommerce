import React from "react";

const Payment = ({ fill = "#000000", width = 25, height = 25 }: { fill?: string; width?: number; height?: number }) => {
  return (
    <svg width={width} height={height} viewBox="0 0 25 25" fill="none" xmlns="http://www.w3.org/2000/svg">
      <g id="SVGRepo_bgCarrier" strokeWidth="0"></g>
      <g
        id="SVGRepo_tracerCarrier"
        strokeLinecap="round"
        strokeLinejoin="round"
      ></g>
      <g id="SVGRepo_iconCarrier">
        {" "}
        <rect
          x="3"
          y="6"
          width="18"
          height="13"
          rx="2"
          stroke={fill}
          strokeWidth="2"
          strokeLinecap="round"
          strokeLinejoin="round"
        ></rect>{" "}
        <path
          d="M3 10H20.5"
          stroke={fill}
          strokeWidth="2"
          strokeLinecap="round"
          strokeLinejoin="round"
        ></path>{" "}
        <path
          d="M7 15H9"
          stroke={fill}
          strokeWidth="2"
          strokeLinecap="round"
          strokeLinejoin="round"
        ></path>{" "}
      </g>
    </svg>
  );
};

export default Payment;
