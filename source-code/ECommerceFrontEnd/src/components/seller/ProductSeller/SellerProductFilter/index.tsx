import React from "react";
import { Row, Col, Select, Typography, Button, Card, Space } from "antd";

const { Title, Text } = Typography;

/**
 * Component: Bộ lọc sản phẩm dành cho Seller
 * (Lọc theo thương hiệu, loại, danh mục phụ, tiểu mục phụ)
 */
const SellerProductFilter: React.FC = () => {
    const handleReset = () => {
        console.log("Đã cài lại bộ lọc");
    };

    const handleShowData = () => {
        console.log("Hiển thị dữ liệu");
    };

    return (
        <Card style={{ borderRadius: "12px" }}>
            {/* Tiêu đề */}
            <Title level={5} style={{ marginBottom: 16 }}>
                Lọc sản phẩm
            </Title>

            {/* Các trường lọc */}
            <Row gutter={[16, 16]} align="middle">
                {/* Thương Hiệu */}
                <Col xs={24} sm={12} md={6}>
                    <Text>Thương Hiệu</Text>
                    <Select
                        placeholder="Tất cả các thương hiệu"
                        style={{ width: "100%", marginTop: 4 }}
                        options={[
                            { value: "all", label: "Tất cả các thương hiệu" },
                            { value: "apple", label: "Apple" },
                            { value: "samsung", label: "Samsung" },
                        ]}
                    />
                </Col>

                {/* Loại */}
                <Col xs={24} sm={12} md={6}>
                    <Text>Loại</Text>
                    <Select
                        placeholder="Chọn danh mục"
                        style={{ width: "100%", marginTop: 4 }}
                        options={[
                            { value: "phone", label: "Điện thoại" },
                            { value: "laptop", label: "Laptop" },
                        ]}
                    />
                </Col>

                {/* Danh Mục Phụ */}
                <Col xs={24} sm={12} md={6}>
                    <Text>Danh Mục Phụ</Text>
                    <Select
                        placeholder="Chọn danh mục phụ"
                        style={{ width: "100%", marginTop: 4 }}
                        options={[
                            { value: "case", label: "Ốp lưng" },
                            { value: "charger", label: "Sạc" },
                        ]}
                    />
                </Col>

                {/* Tiểu Mục Phụ */}
                <Col xs={24} sm={12} md={6}>
                    <Text>Tiểu Mục Phụ</Text>
                    <Select
                        placeholder="Chọn danh mục phụ"
                        style={{ width: "100%", marginTop: 4 }}
                        options={[
                            { value: "type1", label: "Loại 1" },
                            { value: "type2", label: "Loại 2" },
                        ]}
                    />
                </Col>
            </Row>

            {/* Nút hành động */}
            <Row justify="end" style={{ marginTop: 24 }}>
                <Space>
                    <Button onClick={handleReset}>Cài lại</Button>
                    <Button type="primary" onClick={handleShowData}>
                        Hiển thị dữ liệu
                    </Button>
                </Space>
            </Row>
        </Card>
    );
};

export default SellerProductFilter;
