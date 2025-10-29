import React from "react";
import { Card, Typography, Space } from "antd";
import { EnvironmentOutlined } from "@ant-design/icons";

const { Text } = Typography;

const DeliveryAddress: React.FC = () => {
    return (
        <Card
            style={{
                border: "1px solid #f0f0f0",
                borderRadius: 8,
                padding: 0,
            }}
            bodyStyle={{ padding: 0 }}
        >
            {/* Dải viền trang trí phía trên */}
            <div
                style={{
                    borderTop: "4px dashed transparent",
                    borderImage:
                        "repeating-linear-gradient(90deg, #ff6b6b 0, #ff6b6b 10px, #5f9eff 10px, #5f9eff 20px) 8",
                    marginBottom: 16,
                }}
            />

            <div style={{ padding: "0 16px 16px 16px" }}>
                <Space direction="vertical" style={{ width: "100%" }}>
                    {/* Tiêu đề */}
                    <Space align="start">
                        <EnvironmentOutlined style={{ color: "#ff4d4f", fontSize: 18 }} />
                        <Text strong style={{ color: "#ff4d4f" }}>
                            Địa Chỉ Nhận Hàng
                        </Text>
                    </Space>

                    {/* Nội dung */}
                    <div style={{ marginTop: 8, marginLeft: 28 }}>
                        <Text strong>Đỗ Duy Tiến</Text>
                        <Text> &nbsp; (+84) 969 902 132</Text>
                        <br />
                        <Text>
                            Số 48, Ngách 158/21a, Phường Ngọc Hà, Quận Ba Đình, Hà Nội
                        </Text>

                        <Space style={{ marginLeft: 8 }}>
                            {/* Chữ “Mặc định” */}
                            <span
                                style={{
                                    border: "1px solid #ff4d4f",
                                    color: "#ff4d4f",
                                    fontSize: 12,
                                    padding: "0 6px",
                                    borderRadius: 3,
                                    display: "inline-block",
                                    lineHeight: "20px",
                                }}
                            >
                                Mặc định
                            </span>

                            {/* “Thay đổi” là link */}
                            <a
                                href="#"
                                style={{
                                    color: "#1677ff",
                                    fontSize: 13,
                                    textDecoration: "none",
                                    marginLeft: 4,
                                }}
                            >
                                Thay đổi
                            </a>
                        </Space>
                    </div>
                </Space>
            </div>
        </Card>
    );
};

export default DeliveryAddress;
