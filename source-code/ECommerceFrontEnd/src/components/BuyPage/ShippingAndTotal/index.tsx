import React, { useState } from "react";
import { Card, Typography, Row, Col, Input, Space, Modal, Radio } from "antd";

const { Text } = Typography;

interface ShippingAndTotalProps {
    shippingFee: number;
    itemTotal: number;
    voucherDiscount: number;
    shippingMethod: "Nhanh" | "Hỏa tốc";
    onChangeShipping: (method: "Nhanh" | "Hỏa tốc", fee: number) => void;
}

const formatVND = (num: number) =>
    num.toLocaleString("vi-VN", { style: "currency", currency: "VND" }).replace(",00", "");

const ShippingAndTotal: React.FC<ShippingAndTotalProps> = ({
    shippingFee,
    itemTotal,
    voucherDiscount,
    shippingMethod,
    onChangeShipping,
}) => {
    const [isModalOpen, setIsModalOpen] = useState(false);

    const handleSelectShipping = (value: "Nhanh" | "Hỏa tốc") => {
        const newFee = value === "Nhanh" ? 25000 : 50000;
        onChangeShipping(value, newFee);
        setIsModalOpen(false);
    };

    const total = Math.max(0, itemTotal + shippingFee );

    return (
        <>
            <Card
                style={{
                    border: "1px solid #f0f0f0",
                    borderRadius: 8,
                    marginTop: 24,
                }}
                bodyStyle={{ padding: "16px 24px" }}
            >
                {/* 📝 Lời nhắn và phương thức vận chuyển */}
                <Row gutter={16}>
                    <Col span={8}>
                        <Space align="start">
                            <Text style={{ fontWeight: 500 }}>Lời nhắn:</Text>
                            <Input placeholder="Lưu ý cho người bán..." />
                        </Space>
                    </Col>

                    <Col span={16}>
                        <Row justify="space-between" align="top">
                            <Col>
                                <Text style={{ fontWeight: 500 }}>Phương thức vận chuyển:</Text>{" "}
                                <Text strong>{shippingMethod}</Text>
                                <a
                                    onClick={() => setIsModalOpen(true)}
                                    style={{
                                        marginLeft: 8,
                                        color: "#1677ff",
                                        fontWeight: 500,
                                        cursor: "pointer",
                                    }}
                                >
                                    Thay đổi
                                </a>
                                <br />
                                <Text type="secondary">Nhận từ <b>12 Th10 - 15 Th10</b></Text>
                            </Col>
                            <Col>
                                <Text strong>{formatVND(shippingFee)}</Text>
                            </Col>
                        </Row>
                    </Col>
                </Row>

                {/* 💵 Tổng tiền */}
                <Row
                    justify="end"
                    align="middle"
                    style={{
                        marginTop: 24,
                        borderTop: "1px solid #f0f0f0",
                        paddingTop: 12,
                    }}
                >
                    <Col>
                        <Text style={{ color: "#666", marginRight: 8 }}>
                            Tổng số tiền ({1} sản phẩm):
                        </Text>
                        <Text strong style={{ color: "#ff4d4f", fontSize: 18 }}>
                            {formatVND(total)}
                        </Text>
                    </Col>
                </Row>
            </Card>

            {/* 📦 Modal chọn phương thức vận chuyển */}
            <Modal
                title="Chọn phương thức vận chuyển"
                open={isModalOpen}
                onCancel={() => setIsModalOpen(false)}
                footer={null}
            >
                <Radio.Group
                    value={shippingMethod}
                    onChange={(e) => handleSelectShipping(e.target.value as "Nhanh" | "Hỏa tốc")}
                    style={{ display: "flex", flexDirection: "column", gap: 8 }}
                >
                    <Radio value="Nhanh">Nhanh — {formatVND(25000)}</Radio>
                    <Radio value="Hỏa tốc">Hỏa tốc — {formatVND(50000)}</Radio>
                </Radio.Group>
            </Modal>
        </>
    );
};

export default ShippingAndTotal;
