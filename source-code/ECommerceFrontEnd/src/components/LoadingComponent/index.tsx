import React from "react";
import { Spin } from "antd";

interface LoadingComponentProps {
    tip?: string;
    fullscreen?: boolean;
}

const LoadingComponent: React.FC<LoadingComponentProps> = ({
                                                               tip = "Processing...",
                                                               fullscreen = false,
                                                           }) => {
    if (fullscreen) {
        return (
            <div
                style={{
                    position: "fixed",
                    top: 0,
                    left: 0,
                    width: "100vw",
                    height: "100vh",
                    backgroundColor: "rgba(255,255,255,0.7)",
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    zIndex: 9999,
                }}
            >
                <Spin size="large" tip={tip} />
            </div>
        );
    }

    return (
        <div style={{ textAlign: "center", padding: "40px 0" }}>
            <Spin size="large" tip={tip} />
        </div>
    );
};

export default LoadingComponent;
