import React from "react";
import { Layout, Row, Col, Typography, Input, Button, Space } from "antd";
import {
    MailOutlined,
    PhoneOutlined,
    UserOutlined,
    EnvironmentOutlined,
} from "@ant-design/icons";

const { Footer } = Layout;
const { Title, Text, Link } = Typography;

const AppFooter: React.FC = () => {
    return (
        <Footer style={{ background: "#154360", color: "#fff", padding: "50px 80px" }}>
            <Row gutter={[32, 32]}>
                {/* Logo + App Download */}
                <Col xs={24} sm={12} md={6}>
                    <div style={{ marginBottom: 20 }}>
                        <img
                            src="https://6valley.6amtech.com/public/assets/front-end/png/logo.png"
                            alt="6Valley"
                            style={{ height: 40 }}
                        />
                    </div>
                    <Title level={5} style={{ color: "#fff" }}>
                        DOWNLOAD OUR APP
                    </Title>
                    <Space direction="vertical" size="middle">
                        <img
                            src="https://developer.apple.com/assets/elements/badges/download-on-the-app-store.svg"
                            alt="App Store"
                            style={{ height: 40, cursor: "pointer" }}
                        />
                        <img
                            src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg"
                            alt="Google Play"
                            style={{ height: 40, cursor: "pointer" }}
                        />
                    </Space>
                </Col>

                {/* Quick Links */}
                <Col xs={24} sm={12} md={6}>
                    <Title level={5} style={{ color: "#fff" }}>
                        QUICK LINKS
                    </Title>
                    <Space direction="vertical">
                        <Link href="#" style={{ color: "#fff" }}>
                            Profile Info
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Flash Deal
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Featured Products
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Best Selling Product
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Latest Products
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Top Rated Product
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Track Order
                        </Link>
                    </Space>
                </Col>

                {/* Other */}
                <Col xs={24} sm={12} md={6}>
                    <Title level={5} style={{ color: "#fff" }}>
                        OTHER
                    </Title>
                    <Space direction="vertical">
                        <Link href="#" style={{ color: "#fff" }}>
                            About Us
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Terms And Conditions
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Privacy Policy
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Refund Policy
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Return Policy
                        </Link>
                        <Link href="#" style={{ color: "#fff" }}>
                            Cancellation Policy
                        </Link>
                    </Space>
                </Col>

                {/* Newsletter */}
                <Col xs={24} sm={12} md={6}>
                    <Title level={5} style={{ color: "#fff" }}>
                        NEWSLETTER
                    </Title>
                    <Text style={{ display: "block", marginBottom: 10 }}>
                        Subscribe to our new channel to get latest updates
                    </Text>
                    <Space.Compact style={{ width: "100%" }}>
                        <Input placeholder="Your Email Address" />
                        <Button type="primary">Subscribe</Button>
                    </Space.Compact>
                </Col>
            </Row>

            <Row gutter={[32, 32]} style={{ marginTop: 40, borderTop: "1px solid #2E4053", paddingTop: 20 }}>
                {/* Contact */}
                <Col xs={24} sm={12} md={6}>
                    <Title level={5} style={{ color: "#fff" }}>
                        Start A Conversation
                    </Title>
                    <p>
                        <PhoneOutlined /> +00xxxxxxxxxxxx
                    </p>
                    <p>
                        <MailOutlined /> copy@6amtech.com
                    </p>
                    <p>
                        <UserOutlined /> Support ticket
                    </p>
                </Col>

                {/* Address */}
                <Col xs={24} sm={12} md={6}>
                    <Title level={5} style={{ color: "#fff" }}>
                        Address
                    </Title>
                    <p>
                        <EnvironmentOutlined /> Kingston, New York 12401 United States
                    </p>
                </Col>
            </Row>

            <div style={{ textAlign: "center", marginTop: 30 }}>
                <Text style={{ color: "#fff" }}>Copyright 6amTech@2021</Text>
            </div>
        </Footer>
    );
};

export default AppFooter;
