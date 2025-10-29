import React, { useState } from "react";
import { Card, Typography, Space, Row, Col, Image } from "antd";
import { MessageOutlined, ShopOutlined } from "@ant-design/icons";
import ChatBox from "../ChatBox";
import img from "../../../assets/img/SamSungS24 Ultra.jpg";

const { Text, Title } = Typography;

export interface ProductItem {
    id: number;
    name: string;
    category: string;
    image: string;
    price: number;
    quantity: number;
}

const ProductListBuyPage: React.FC = () => {
    const [isChatOpen, setIsChatOpen] = useState(false);

    const products: ProductItem[] = [
        {
            id: 1,
            name: "Giá Đỡ Điện Thoại Máy Tính Bảng Để Bàn Kim Loại nặng",
            category: "Giá Đỡ Màu Xám",
            image: img,
            price: 50000,
            quantity: 3,
        },
    ];

    const formatPrice = (num: number) =>
        num.toLocaleString("vi-VN", { style: "currency", currency: "VND" });

    return (
        <>
            <Card
                style={{
                    border: "1px solid #f0f0f0",
                    borderRadius: 12,
                    marginTop: 24,
                }}
                bodyStyle={{ padding: "24px 32px" }}
            >
                <Title level={4}>Sản phẩm</Title>

                {/* --- Shop Info + Chat ngay --- */}
                <Space align="center" style={{ marginBottom: 20 }}>
                    <ShopOutlined style={{ fontSize: 18, marginRight: 6 }} />
                    <Text strong style={{ fontSize: 16 }}>Hang3c.shop</Text>
                    <span style={{ color: "#d9d9d9" }}>|</span>
                    <Space
                        align="center"
                        style={{
                            cursor: "pointer",
                            color: "#00b96b",
                            fontWeight: 500,
                            fontSize: 15,
                        }}
                        onClick={() => setIsChatOpen(true)}
                    >
                        <MessageOutlined />
                        Chat ngay
                    </Space>
                </Space>

                {/* --- Header row --- */}
                <Row
                    style={{
                        fontWeight: 600,
                        color: "#888",
                        borderBottom: "1px solid #f0f0f0",
                        paddingBottom: 10,
                        marginBottom: 16,
                        fontSize: 15,
                    }}
                >
                    <Col span={12}>Sản phẩm</Col>
                    <Col span={4} style={{ textAlign: "center" }}>Đơn giá</Col>
                    <Col span={4} style={{ textAlign: "center" }}>Số lượng</Col>
                    <Col span={4} style={{ textAlign: "right" }}>Thành tiền</Col>
                </Row>

                {/* --- Product rows --- */}
                {products.map((item) => (
                    <Row key={item.id} align="middle" style={{ marginBottom: 20 }}>
                        <Col span={12}>
                            <Space align="start" size={16}>
                                <Image
                                    src={item.image}
                                    alt={item.name}
                                    width={90}
                                    height={90}
                                    style={{ borderRadius: 6, objectFit: "cover" }}
                                    preview={false}
                                />
                                <div>
                                    <Text style={{ fontWeight: 600, fontSize: 16 }}>
                                        {item.name}
                                    </Text>
                                    <br />
                                    <Text type="secondary" style={{ fontSize: 14 }}>
                                        Phân loại: {item.category}
                                    </Text>
                                </div>
                            </Space>
                        </Col>
                        <Col span={4} style={{ textAlign: "center" }}>
                            <Text strong style={{ fontSize: 15 }}>
                                {formatPrice(item.price)}
                            </Text>
                        </Col>
                        <Col span={4} style={{ textAlign: "center" }}>
                            <Text style={{ fontSize: 15 }}>{item.quantity}</Text>
                        </Col>
                        <Col span={4} style={{ textAlign: "right" }}>
                            <Text strong style={{ fontSize: 15 }}>
                                {formatPrice(item.price * item.quantity)}
                            </Text>
                        </Col>
                    </Row>
                ))}
            </Card>

            <ChatBox open={isChatOpen} onClose={() => setIsChatOpen(false)} />
        </>
    );
};

export default ProductListBuyPage;
