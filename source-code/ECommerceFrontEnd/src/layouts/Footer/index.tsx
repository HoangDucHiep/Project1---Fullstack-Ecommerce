import React from "react";
import { Layout, Row, Col, Typography, Space } from "antd";
import {
    MailOutlined,
    PhoneOutlined,
    UserOutlined,
    EnvironmentOutlined,
} from "@ant-design/icons";
import img from "../../assets/img/logo.png";

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
                {/* Cột 1: Logo + Tải ứng dụng */}
                <Col xs={24} sm={12} md={6} lg={4}>
                    <div style={{ marginBottom: 16 }}>
                        <img src={img} alt="Logo" style={{ height: 40 }} />
                    </div>
                    <Title level={5} style={{ color: "#fff" }}>
                        TẢI ỨNG DỤNG CỦA CHÚNG TÔI
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

                {/* Cột 2: Liên kết nhanh */}
                <Col xs={12} sm={12} md={6} lg={4}>
                    <Title level={5} style={{ color: "#fff" }}>
                        LIÊN KẾT NHANH
                    </Title>
                    <Space direction="vertical" size={6}>
                        {[
                            "Thông tin cá nhân",
                            "Ưu đãi chớp nhoáng",
                            "Sản phẩm nổi bật",
                            "Sản phẩm bán chạy",
                            "Sản phẩm mới nhất",
                            "Sản phẩm được đánh giá cao",
                            "Theo dõi đơn hàng",
                        ].map((item) => (
                            <Link key={item} href="#" style={{ color: "#fff" }}>
                                {item}
                            </Link>
                        ))}
                    </Space>
                </Col>

                {/* Cột 3: Khác */}
                <Col xs={12} sm={12} md={6} lg={4}>
                    <Title level={5} style={{ color: "#fff" }}>
                        KHÁC
                    </Title>
                    <Space direction="vertical" size={6}>
                        {[
                            "Về chúng tôi",
                            "Điều khoản và điều kiện",
                            "Chính sách bảo mật",
                            "Chính sách hoàn tiền",
                            "Chính sách đổi trả",
                            "Chính sách hủy đơn",
                        ].map((item) => (
                            <Link key={item} href="#" style={{ color: "#fff" }}>
                                {item}
                            </Link>
                        ))}
                    </Space>
                </Col>

                {/* Cột 4: Liên hệ & Địa chỉ */}
                <Col xs={24} sm={12} md={6} lg={8}>
                    <Title level={5} style={{ color: "#fff" }}>
                        BẮT ĐẦU TRAO ĐỔI
                    </Title>
                    <p style={{ marginBottom: 6 }}>
                        <PhoneOutlined /> +00xxxxxxxxxxxx
                    </p>
                    <p style={{ marginBottom: 6 }}>
                        <MailOutlined /> lienhe@hcmutstore.com
                    </p>
                    <p style={{ marginBottom: 12 }}>
                        <UserOutlined /> Gửi yêu cầu hỗ trợ
                    </p>

                    <Title level={5} style={{ color: "#fff", marginTop: 16 }}>
                        ĐỊA CHỈ
                    </Title>
                    <p style={{ marginBottom: 0 }}>
                        <EnvironmentOutlined /> Quận 10, TP. Hồ Chí Minh, Việt Nam
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
                    © 2025 HCMUT Store. Bản quyền thuộc về tất cả các bên.
                </Text>
            </div>
        </Footer>
    );
};

export default AppFooter;
