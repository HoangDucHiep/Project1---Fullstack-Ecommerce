// 👇 1. Import 'Space'
import React from "react";
import { Typography, Card, Tag, Space } from "antd";
import SellerProductActionBar from "../../components/seller/ProductSeller/SellerProductActionBar";
import SellerProductFilter from "../../components/seller/ProductSeller/SellerProductFilter";
import SellerProductList from "../../components/seller/ProductSeller/SellerProductList";

const { Title } = Typography;

const SellerUnapprovedProductPage: React.FC = () => {
    const unapprovedCount = 8; // Giả lập số lượng

    return (
        // 👇 2. Giảm padding tổng
        <div style={{ padding: 12 }}>
            {/* 1️⃣ Tiêu đề */}
            <Card
                style={{
                    marginBottom: 8, // 👈 3. Giảm margin
                    borderRadius: 12,
                    boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
                }}
                // 👇 4. Giảm padding body, sát lề trên
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
                    Sản phẩm không được duyệt
                    <Tag
                        color="red" // Màu đỏ
                        style={{
                            marginLeft: 8,
                            borderRadius: 6,
                        }}
                    >
                        {unapprovedCount}
                    </Tag>
                </Title>
            </Card>

            {/* 2️⃣ Khối Điều khiển (GỘP Filter + Action Bar) */}
            <Card
                hoverable
                style={{
                    marginBottom: 8, // 👈 3. Giảm margin
                    borderRadius: 12,
                }}
                // 👇 5. Giảm padding body khối điều khiển
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
                // 👇 6. Xóa padding body để list dính sát viền
                bodyStyle={{ padding: 0 }}
            >
                <SellerProductList hiddenColumns={["active"]} />
            </Card>
        </div>
    );
};

export default SellerUnapprovedProductPage;