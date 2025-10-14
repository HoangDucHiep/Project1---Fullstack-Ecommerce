import React from "react";
import { Layout, Row, Col, Typography, Space } from "antd";
import {
    MailOutlined,
    PhoneOutlined,
    UserOutlined,
    EnvironmentOutlined,
} from "@ant-design/icons";
import img from "../../assets/logohcm.png";

const { Footer } = Layout;
const { Title, Text, Link } = Typography;

const AppFooter: React.FC = () => {
    return (
        <Footer
            style={{
                background: "#154360",
                color: "#fff",
                padding: "40px 100px 20px 100px",
            }}
        >
            {/* --- 5 cột ngang hàng --- */}
            <Row
                gutter={[32, 32]}
                justify="center"
                style={{ maxWidth: 1400, margin: "0 auto" }}
            >
                {/* Cột 1: Logo + App Download */}
                <Col xs={24} sm={12} md={6} lg={4}>
                    <div style={{ marginBottom: 16 }}>
                        <img src={img} alt="Logo" style={{ height: 40 }} />
                    </div>
                    <Title level={5} style={{ color: "#fff" }}>
                        DOWNLOAD OUR APP
                    </Title>
                    <Space direction="vertical" size="middle">
                        <img
                            src="https://developer.apple.com/assets/elements/badges/download-on-the-app-store.svg"
                            alt="App Store"
                            style={{ height: 36, cursor: "pointer" }}
                        />
                        <img
                            src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg"
                            alt="Google Play"
                            style={{ height: 36, cursor: "pointer" }}
                        />
                    </Space>
                </Col>

                {/* Cột 2: Quick Links */}
                <Col xs={12} sm={12} md={6} lg={4}>
                    <Title level={5} style={{ color: "#fff" }}>
                        QUICK LINKS
                    </Title>
                    <Space direction="vertical" size={6}>
                        {[
                            "Profile Info",
                            "Flash Deal",
                            "Featured Products",
                            "Best Selling Product",
                            "Latest Products",
                            "Top Rated Product",
                            "Track Order",
                        ].map((item) => (
                            <Link key={item} href="#" style={{ color: "#fff" }}>
                                {item}
                            </Link>
                        ))}
                    </Space>
                </Col>

                {/* Cột 3: Other */}
                <Col xs={12} sm={12} md={6} lg={4}>
                    <Title level={5} style={{ color: "#fff" }}>
                        OTHER
                    </Title>
                    <Space direction="vertical" size={6}>
                        {[
                            "About Us",
                            "Terms And Conditions",
                            "Privacy Policy",
                            "Refund Policy",
                            "Return Policy",
                            "Cancellation Policy",
                        ].map((item) => (
                            <Link key={item} href="#" style={{ color: "#fff" }}>
                                {item}
                            </Link>
                        ))}
                    </Space>
                </Col>


                {/* Cột 5: Start a Conversation + Address */}
                <Col xs={24} sm={12} md={6} lg={8}>
                    <Title level={5} style={{ color: "#fff" }}>
                        Start A Conversation
                    </Title>
                    <p style={{ marginBottom: 6 }}>
                        <PhoneOutlined /> +00xxxxxxxxxxxx
                    </p>
                    <p style={{ marginBottom: 6 }}>
                        <MailOutlined /> copy@6amtech.com
                    </p>
                    <p style={{ marginBottom: 12 }}>
                        <UserOutlined /> Support ticket
                    </p>

                    <Title level={5} style={{ color: "#fff", marginTop: 16 }}>
                        Address
                    </Title>
                    <p style={{ marginBottom: 0 }}>
                        <EnvironmentOutlined /> Kingston, New York 12401 United States
                    </p>
                </Col>
            </Row>

            {/* --- Dòng bản quyền --- */}
            <div
                style={{
                    textAlign: "center",
                    marginTop: 30,
                    borderTop: "1px solid #2E4053",
                    paddingTop: 20,
                }}
            >
                <Text style={{ color: "#ccc", fontSize: 13 }}>
                    © 2025 HCMUT Store. All rights reserved.
                </Text>
            </div>
        </Footer>
    );
};

export default AppFooter;
