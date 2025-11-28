import React from "react";
import { Layout, Typography, Card } from "antd";
import SellerDiscountForm from "../../components/seller/discount voucher/SellerDiscountForm";
import SellerDiscountList from "../../components/seller/discount voucher/SellerDiscountList"; 

const { Content } = Layout;
const { Title } = Typography;

const DiscountVoucherPage: React.FC = () => {
    return (
        <Layout
            style={{
                minHeight: "100vh",
                background: "#f5f7fa",
                display: "flex",
                justifyContent: "center",
                padding: "40px 20px",
            }}
        >
            <Content
                style={{
                    width: "100%",
                    maxWidth: 1200,
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    gap: 8, // khoảng cách rất nhỏ giữa 2 khối
                }}
            >
                {/* --- KHỐI 1: FORM --- */}
                <Card
                    className="block-card"
                    title={
                        <Title
                            level={4}
                            style={{
                                margin: 0,
                                color: "#1890ff",
                                fontWeight: 600,
                                textTransform: "uppercase",
                            }}
                        >
                            🎟️ Tạo Phiếu Giảm Giá
                        </Title>
                    }
                    bodyStyle={{ padding: 24 }}
                >
                    <SellerDiscountForm />
                </Card>

                {/* --- KHỐI 2: LIST --- */}
                <Card
                    className="block-card"
                    title={
                        <Title
                            level={4}
                            style={{
                                margin: 0,
                                color: "#1890ff",
                                fontWeight: 600,
                                textTransform: "uppercase",
                            }}
                        >
                            📋 Danh Sách Phiếu Giảm Giá
                        </Title>
                    }
                    bodyStyle={{ padding: 24 }}
                >
                    <SellerDiscountList />
                </Card>

                {/* ✅ CSS nội bộ */}
                <style>{`
                    /* Mỗi khối card */
                    .block-card {
                        width: 100%;
                        background: #fff;
                        border-radius: 16px;
                        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
                        transition: all 0.3s ease;
                    }

                    /* Hover: nổi nhẹ lên */
                    .block-card:hover {
                        transform: translateY(-4px);
                        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
                    }

                    /* Loại bỏ khoảng cách giữa tiêu đề và card */
                    .ant-card-head {
                        border-bottom: none;
                        padding: 16px 24px 0;
                    }

                    .ant-card-body {
                        padding-top: 12px !important;
                    }

                    @media (max-width: 768px) {
                        .block-card {
                            border-radius: 12px;
                            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
                        }
                    }
                `}</style>
            </Content>
        </Layout>
    );
};

export default DiscountVoucherPage;
