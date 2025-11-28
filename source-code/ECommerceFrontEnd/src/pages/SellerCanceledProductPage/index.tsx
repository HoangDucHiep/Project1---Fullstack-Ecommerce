import React from "react";
import { Typography, Card, Tag, Space } from "antd";
import SellerProductActionBar from "../../components/seller/ProductSeller/SellerProductActionBar";
import SellerProductFilter from "../../components/seller/ProductSeller/SellerProductFilter";
import SellerProductList from "../../components/seller/ProductSeller/SellerProductList";

const { Title } = Typography;

const SellerCanceledProductPage: React.FC = () => {
    const canceledCount = 5;

    return (
        <div style={{ padding: 12 }}>
            {/* 1️⃣ Tiêu đề */}
            <Card
                style={{
                    marginBottom: 8,
                    borderRadius: 12,
                    boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
                }}
                // 👇 GHI ĐÈ PADDING:
                // Cài đặt padding top/bottom = 8px
                // Cài đặt padding left/right = 12px
                bodyStyle={{ padding: "8px 12px" }}
            >
                <Title
                    level={4}
                    style={{
                        margin: 0,
                        display: "flex",
                        alignItems: "center",
                    }}
                >
                    Sản phẩm bị hủy
                    <Tag
                        color="volcano"
                        style={{
                            marginLeft: 8,
                            borderRadius: 6,
                        }}
                    >
                        {canceledCount}
                    </Tag>
                </Title>
            </Card>

            {/* 2️⃣ Khối Điều khiển (Filter + Action Bar) */}
            <Card
                hoverable
                style={{
                    marginBottom: 8,
                    borderRadius: 12,
                }}
                // (Giữ nguyên padding 12px cho khối này)
                bodyStyle={{ padding: 12 }}
            >
                <Space direction="vertical" style={{ width: "100%" }} size="small">
                    <SellerProductFilter />
                    <SellerProductActionBar />
                </Space>
            </Card>

            {/* 3️⃣ Danh sách sản phẩm */}
            <Card
                hoverable
                style={{
                    borderRadius: 12,
                }}
                // (Giữ nguyên padding 0 cho khối list)
                bodyStyle={{ padding: 0 }}
            >
                <SellerProductList hiddenColumns={["active"]} />
            </Card>
        </div>
    );
};

export default SellerCanceledProductPage;