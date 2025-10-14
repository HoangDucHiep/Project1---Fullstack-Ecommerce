import React from "react";
import { Button, Typography, Row, Col, Divider, Space, Image } from "antd";
import img from '../../../assets/logohcm.png'
const { Text } = Typography;

const ShopInfo: React.FC = () => {
    return (
        <div
            style={{
                background: "#fff",
                borderRadius: 8,
                padding: "16px 24px",
                marginTop: 8,
                boxShadow: "0 1px 3px rgba(0,0,0,0.05)",
            }}
        >
            <Row align="middle" gutter={32}>
                {/* Bên trái: Thông tin shop */}
                <Col flex="300px">
                    <Space align="center" size={16}>
                        <Image
                            src={img}
                            alt="Shop logo"
                            width={70}
                            height={70}
                            style={{ borderRadius: "50%" }}
                            preview={false}
                        />
                        <div>
                            <Text strong style={{ fontSize: 16 }}>
                                Bovie Cosmetics
                            </Text>
                            <br />
                            <Text type="secondary">Online 3 Giờ Trước</Text>
                            <div style={{ marginTop: 8 }}>
                                <Space>
                                    <Button
                                        type="primary"
                                        size="small"
                                        style={{
                                            background: "#ee4d2d",
                                            borderColor: "#ee4d2d",
                                            fontWeight: 500,
                                        }}
                                    >
                                        Yêu Thích+
                                    </Button>
                                    <Button
                                        icon={<i className="ri-chat-3-line" />}
                                        type="default"
                                        size="small"
                                        style={{ borderColor: "#ee4d2d", color: "#ee4d2d" }}
                                    >
                                        Chat Ngay
                                    </Button>
                                    <Button size="small">Xem Shop</Button>
                                </Space>
                            </div>
                        </div>
                    </Space>
                </Col>

                {/* Đường chia */}
                <Col>
                    <Divider type="vertical" style={{ height: "80px" }} />
                </Col>

                {/* Bên phải: Thống kê */}
                <Col flex="auto">
                    <Row gutter={[48, 8]}>
                        <Col>
                            <Text type="secondary">Đánh Giá</Text>
                            <br />
                            <Text style={{ color: "#ee4d2d", fontWeight: 500 }}>606,7k</Text>
                        </Col>

                        <Col>
                            <Text type="secondary">Tỉ Lệ Phản Hồi</Text>
                            <br />
                            <Text style={{ color: "#ee4d2d", fontWeight: 500 }}>100%</Text>
                        </Col>

                        <Col>
                            <Text type="secondary">Sản Phẩm</Text>
                            <br />
                            <Text style={{ color: "#ee4d2d", fontWeight: 500 }}>1,6k</Text>
                        </Col>

                        <Col>
                            <Text type="secondary">Thời Gian Phản Hồi</Text>
                            <br />
                            <Text style={{ color: "#ee4d2d", fontWeight: 500 }}>
                                trong vài giờ
                            </Text>
                        </Col>

                        <Col>
                            <Text type="secondary">Tham Gia</Text>
                            <br />
                            <Text style={{ color: "#ee4d2d", fontWeight: 500 }}>
                                9 năm trước
                            </Text>
                        </Col>

                        <Col>
                            <Text type="secondary">Người Theo Dõi</Text>
                            <br />
                            <Text style={{ color: "#ee4d2d", fontWeight: 500 }}>794k</Text>
                        </Col>
                    </Row>
                </Col>
            </Row>
        </div>
    );
};

export default ShopInfo;
