import React, { useState } from "react";
import { Card, Row, Col, Input, Button, Typography, Tag } from "antd";
import {
    GiftOutlined,
    ClockCircleOutlined,
    ShoppingOutlined,
} from "@ant-design/icons";

const { Text, Title } = Typography;

interface Voucher {
    id: number;
    title: string;
    discount: string;
    minOrder: string;
    expire: string;
    usedPercent?: string;
    timesLeft?: number;
    type: string;
    actionLabel: string;
}

const VoucherList: React.FC = () => {
    const [vouchers] = useState<Voucher[]>([
        {
            id: 1,
            title: "Giảm 12% Giảm tối đa 3trđ",
            discount: "Giảm 12%",
            minOrder: "Đơn tối thiểu 5trđ",
            expire: "Sắp hết hạn: Còn 23 giờ",
            type: "Shopee",
            timesLeft: 2,
            actionLabel: "Dùng ngay",
        },
        {
            id: 2,
            title: "Giảm 14% Giảm tối đa 3trđ",
            discount: "Giảm 14%",
            minOrder: "Đơn tối thiểu 5trđ",
            expire: "Đã dùng 60%, Sắp hết hạn: 23 giờ",
            type: "Shopee Xtra",
            timesLeft: 2,
            actionLabel: "Dùng ngay",
        },
        {
            id: 3,
            title: "Giảm 10% Giảm tối đa 35kđ",
            discount: "Giảm 10%",
            minOrder: "Đơn tối thiểu 99kđ",
            expire: "Còn 2 ngày",
            type: "Shopee",
            timesLeft: 2,
            actionLabel: "Dùng ngay",
        },
        {
            id: 4,
            title: "Giảm 12% Giảm tối đa 500kđ",
            discount: "Giảm 12%",
            minOrder: "Đơn tối thiểu 1.5trđ",
            expire: "Sắp hết hạn: Còn 23 giờ",
            type: "Shopee Xtra",
            timesLeft: 2,
            actionLabel: "Dùng ngay",
        },
    ]);

    return (
        <div style={{ padding: 24, background: "#fff", borderRadius: 8 }}>
            {/* Header */}
            <Row justify="space-between" align="middle" style={{ marginBottom: 20 }}>
                <Col>
                    <Title level={4} style={{ margin: 0 }}>
                        Kho Voucher
                    </Title>
                </Col>
                <Col>
                    <Button type="link" style={{ marginRight: 8 }}>
                        Tìm thêm voucher
                    </Button>
                    <Button type="link">Xem lịch sử voucher</Button>
                </Col>
            </Row>

            {/* Nhập mã voucher */}
            <Row gutter={8} style={{ marginBottom: 24 }}>
                <Col flex="auto">
                    <Input
                        placeholder="Nhập mã voucher tại đây"
                        prefix={<GiftOutlined />}
                    />
                </Col>
                <Col>
                    <Button type="primary">Lưu</Button>
                </Col>
            </Row>

            {/* Danh sách voucher */}
            <Row gutter={[16, 16]}>
                {vouchers.map((v) => (
                    <Col xs={24} md={12} key={v.id}>
                        <Card
                            style={{
                                display: "flex",
                                borderLeft: "6px solid #ff4d4f",
                                borderRadius: 10,
                                minHeight: 140,
                                position: "relative",
                            }}
                        >
                            {/* Icon Shopee */}
                            <div
                                style={{
                                    width: 80,
                                    height: 80,
                                    backgroundColor: "#ff4d4f",
                                    borderRadius: 8,
                                    display: "flex",
                                    alignItems: "center",
                                    justifyContent: "center",
                                    color: "#fff",
                                    fontSize: 30,
                                    fontWeight: "bold",
                                    flexShrink: 0,
                                }}
                            >
                                <ShoppingOutlined />
                            </div>

                            {/* Nội dung */}
                            <div style={{ marginLeft: 16, flex: 1 }}>
                                <Text strong>{v.title}</Text>
                                <br />
                                <Text type="secondary">{v.minOrder}</Text>
                                <br />
                                <Text type="secondary">
                                    <ClockCircleOutlined /> {v.expire}
                                </Text>
                                <br />
                                <Tag color="red" style={{ marginTop: 8 }}>
                                    {v.type.toUpperCase()}
                                </Tag>
                            </div>

                            {/* Góc phải */}
                            <div
                                style={{
                                    position: "absolute",
                                    top: 10,
                                    right: 10,
                                    textAlign: "center",
                                }}
                            >
                                {v.timesLeft && (
                                    <Tag color="red" style={{ fontSize: 12 }}>
                                        x{v.timesLeft}
                                    </Tag>
                                )}
                                <Button type="primary" size="small" style={{ marginTop: 4 }}>
                                    {v.actionLabel}
                                </Button>
                            </div>
                        </Card>
                    </Col>
                ))}
            </Row>
        </div>
    );
};


export default VoucherList;
