import React, { useState } from "react";
import {
    Button,
    Checkbox,
    Col,
    Divider,
    Form,
    Input,
    Row,
    Typography,
    Space,
    Card,
} from "antd";
import {
    GoogleOutlined,
    FacebookFilled,
    UserOutlined,
    EyeInvisibleOutlined,
    EyeTwoTone,
} from "@ant-design/icons";
import Header from "../../layouts/Header";
import Footer from "../../layouts/Footer";

const { Title, Text, Link } = Typography;

const LoginPage: React.FC = () => {
    const [loading, setLoading] = useState(false);

    const onFinish = (values: { email: string; password: string; remember: boolean }) => {
        console.log("Giá trị form:", values);
        setLoading(true);
        setTimeout(() => {
            setLoading(false);
        }, 1500);
    };

    return (
        <div
            style={{
                display: "flex",
                flexDirection: "column",
                minHeight: "100vh",
                background: "#f5f7fa",
            }}
        >
            <Header />

            <Row
                justify="center"
                align="middle"
                style={{
                    flex: "1 0 auto",
                    background: "#f5f7fa",
                    padding: "40px 0",
                }}
            >
                <Col xs={22} sm={20} md={16} lg={12} xl={10}>
                    <Card style={{ borderRadius: 12, padding: "24px 36px" }}>
                        <div style={{ textAlign: "center", marginBottom: 24 }}>
                            <UserOutlined
                                style={{ fontSize: 48, color: "#1a73e8", marginBottom: 8 }}
                            />
                            <Title level={3} style={{ margin: 0 }}>
                                Đăng nhập tài khoản
                            </Title>
                        </div>

                        <Row gutter={32}>
                            {/* Form đăng nhập */}
                            <Col xs={24} md={12}>
                                <Form layout="vertical" onFinish={onFinish}>
                                    <Form.Item
                                        label="Email hoặc Số điện thoại"
                                        name="email"
                                        rules={[
                                            { required: true, message: "Vui lòng nhập email hoặc số điện thoại!" },
                                        ]}
                                    >
                                        <Input placeholder="Nhập email hoặc số điện thoại" />
                                    </Form.Item>

                                    <Form.Item
                                        label="Mật khẩu"
                                        name="password"
                                        rules={[{ required: true, message: "Vui lòng nhập mật khẩu!" }]}
                                    >
                                        <Input.Password
                                            placeholder="Nhập mật khẩu"
                                            iconRender={(visible) =>
                                                visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                                            }
                                        />
                                    </Form.Item>

                                    <div
                                        style={{
                                            display: "flex",
                                            justifyContent: "space-between",
                                            alignItems: "center",
                                            marginBottom: 16,
                                        }}
                                    >
                                        <Checkbox>Ghi nhớ đăng nhập</Checkbox>
                                        <Link href="#">Quên mật khẩu?</Link>
                                    </div>

                                    <Button type="primary" htmlType="submit" block loading={loading}>
                                        Đăng nhập
                                    </Button>
                                </Form>
                            </Col>

                            {/* Đăng nhập mạng xã hội */}
                            <Col xs={24} md={12} style={{ borderLeft: "1px solid #f0f0f0" }}>
                                <div
                                    style={{
                                        paddingLeft: 24,
                                        display: "flex",
                                        flexDirection: "column",
                                        justifyContent: "center",
                                        height: "100%",
                                    }}
                                >
                                    <Text type="secondary">Hoặc đăng nhập bằng</Text>
                                    <Space
                                        direction="vertical"
                                        style={{ width: "100%", marginTop: 16 }}
                                    >
                                        {/* 🔴 Google Button */}
                                        <Button
                                            icon={<GoogleOutlined style={{ color: "#DB4437" }} />}
                                            block
                                            style={{
                                                background: "#fff",
                                                border: "1px solid #ddd",
                                                fontWeight: 500,
                                                height: 40,
                                                display: "flex",
                                                alignItems: "center",
                                                justifyContent: "center",
                                            }}
                                            onMouseEnter={(e) =>
                                                (e.currentTarget.style.background = "#f8f8f8")
                                            }
                                            onMouseLeave={(e) =>
                                                (e.currentTarget.style.background = "#fff")
                                            }
                                        >
                                            <span style={{ color: "#444" }}>Đăng nhập với Google</span>
                                        </Button>

                                        {/* 🔵 Facebook Button */}
                                        <Button
                                            icon={
                                                <FacebookFilled
                                                    style={{
                                                        color: "#fff",
                                                        background: "transparent",
                                                    }}
                                                />
                                            }
                                            block
                                            style={{
                                                background: "#1877F2",
                                                border: "none",
                                                color: "#fff",
                                                fontWeight: 500,
                                                height: 40,
                                                display: "flex",
                                                alignItems: "center",
                                                justifyContent: "center",
                                            }}
                                            onMouseEnter={(e) =>
                                                (e.currentTarget.style.background = "#166FE5")
                                            }
                                            onMouseLeave={(e) =>
                                                (e.currentTarget.style.background = "#1877F2")
                                            }
                                        >
                                            Đăng nhập với Facebook
                                        </Button>
                                    </Space>
                                </div>
                            </Col>
                        </Row>

                        <Divider style={{ margin: "32px 0 16px" }} />

                        <div style={{ textAlign: "center" }}>
                            <Text>Bạn chưa có tài khoản? </Text>
                            <Link href="#">Đăng ký ngay</Link>
                        </div>
                    </Card>
                </Col>
            </Row>

            <div style={{ flexShrink: 0 }}>
                <Footer />
            </div>
        </div>
    );
};

export default LoginPage;
