
import React, { useState } from "react";
import { Button, Checkbox, Col, Divider, Form, Input, Row, Typography, Space, Card } from "antd";
import { GoogleOutlined, FacebookFilled, UserOutlined, EyeInvisibleOutlined, EyeTwoTone } from "@ant-design/icons";
import Header from "../../layouts/Header";
import Footer from "../../layouts/Footer";

const { Title, Text, Link } = Typography;

const LoginPage: React.FC = () => {
    const [loading, setLoading] = useState(false);

    const onFinish = (values: { email: string; password: string; remember: boolean }) => {
        console.log("Form values:", values);
        setLoading(true);
        setTimeout(() => {
            setLoading(false);
        }, 1500);
    };

    return (
        <>
        <Header />
        <Row justify="center" align="middle" style={{ height: "100vh", background: "#f5f7fa" }}>
            <Col xs={22} sm={20} md={16} lg={12} xl={10}>
                <Card style={{ borderRadius: 12, padding: "24px 36px" }}>
                    <div style={{ textAlign: "center", marginBottom: 24 }}>
                        <UserOutlined style={{ fontSize: 48, color: "#1a73e8", marginBottom: 8 }} />
                        <Title level={3} style={{ margin: 0 }}>
                            Sign In
                        </Title>
                    </div>

                    <Row gutter={32}>
                        {/* Form đăng nhập */}
                        <Col xs={24} md={12}>
                            <Form layout="vertical" onFinish={onFinish}>
                                <Form.Item
                                    label="Email / Phone"
                                    name="email"
                                    rules={[{ required: true, message: "Please enter your email or phone!" }]}
                                >
                                    <Input placeholder="Enter email or phone" />
                                </Form.Item>

                                <Form.Item
                                    label="Password"
                                    name="password"
                                    rules={[{ required: true, message: "Please enter your password!" }]}
                                >
                                    <Input.Password
                                        placeholder="Enter password"
                                        iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)}
                                    />
                                </Form.Item>

                                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
                                    <Checkbox>Remember me</Checkbox>
                                    <Link href="#">Forgot password?</Link>
                                </div>

                                <Button type="primary" htmlType="submit" block loading={loading}>
                                    Sign in
                                </Button>
                            </Form>
                        </Col>

                        {/* Đăng nhập mạng xã hội */}
                        <Col xs={24} md={12} style={{ borderLeft: "1px solid #f0f0f0" }}>
                            <div style={{ paddingLeft: 24, display: "flex", flexDirection: "column", justifyContent: "center", height: "100%" }}>
                                <Text type="secondary">Or sign in with</Text>
                                <Space direction="vertical" style={{ width: "100%", marginTop: 16 }}>
                                    <Button icon={<GoogleOutlined />} block>
                                        Google
                                    </Button>
                                    <Button icon={<FacebookFilled />} block>
                                        Facebook
                                    </Button>
                                </Space>
                            </div>
                        </Col>
                    </Row>

                    <Divider style={{ margin: "32px 0 16px" }} />

                    <div style={{ textAlign: "center" }}>
                        <Text>Enjoy New experience </Text>
                        <Link href="#">Sign up</Link>
                    </div>
                </Card>
            </Col>
        </Row>
        <Footer />
        </>
    );
};

export default LoginPage;
