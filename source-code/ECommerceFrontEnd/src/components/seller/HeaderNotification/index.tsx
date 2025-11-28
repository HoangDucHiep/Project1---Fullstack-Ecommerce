import React from "react";
import { Badge, Tooltip } from "antd";
import { BellOutlined, MessageOutlined } from "@ant-design/icons";

const HeaderNotification: React.FC = () => {
    return (
        <div
            style={{
                display: "flex",
                alignItems: "center",
                gap: "20px",
            }}
        >
            {/* 🔔 Thông báo */}
            <Tooltip title="Thông báo">
                <Badge count={1} size="small" offset={[-2, 5]}>
                    <BellOutlined
                        style={{
                            fontSize: 22,
                            color: "#002B5B",
                            cursor: "pointer",
                            transition: "color 0.2s",
                        }}
                        onMouseEnter={(e) => (e.currentTarget.style.color = "#4CAF50")}
                        onMouseLeave={(e) => (e.currentTarget.style.color = "#002B5B")}
                    />
                </Badge>
            </Tooltip>

            {/* 💬 Tin nhắn */}
            <Tooltip title="Tin nhắn">
                <Badge count={3} size="small" offset={[-2, 5]}>
                    <MessageOutlined
                        style={{
                            fontSize: 22,
                            color: "#002B5B",
                            cursor: "pointer",
                            transition: "color 0.2s",
                        }}
                        onMouseEnter={(e) => (e.currentTarget.style.color = "#4CAF50")}
                        onMouseLeave={(e) => (e.currentTarget.style.color = "#002B5B")}
                    />
                </Badge>
            </Tooltip>
        </div>
    );
};

export default HeaderNotification;
