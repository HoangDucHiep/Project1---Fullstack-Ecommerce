import React from "react";
import { Row, Col, Select, Card } from "antd";
import { SortAscendingOutlined, FilterOutlined } from "@ant-design/icons";

const { Option } = Select;

const ProductHeader: React.FC = () => {
    return (
        <Card
            style={{
                borderRadius: 8,
                boxShadow: "0 2px 6px rgba(0,0,0,0.05)",
                border: "1px solid #f0f0f0",
            }}
            bodyStyle={{ padding: "16px 24px" }}
        >
            <Row align="middle" justify="space-between">
                {/* Bên trái */}
                <Col>
                    <div>
                        <h2 style={{ margin: 0, fontSize: 20, fontWeight: 600 }}>Products</h2>
                        <p style={{ margin: 0, color: "#666" }}>358 items found</p>
                    </div>
                </Col>

                {/* Bên phải */}
                <Col>
                    <div style={{ display: "flex", gap: 12 }}>
                        <Select
                            defaultValue="default"
                            suffixIcon={<SortAscendingOutlined />}
                            style={{ width: 160 }}
                        >
                            <Option value="default">Sort by Default</Option>
                            <Option value="price_low">Price: Low to High</Option>
                            <Option value="price_high">Price: High to Low</Option>
                            <Option value="latest">Latest</Option>
                        </Select>

                        <Select
                            defaultValue="default"
                            suffixIcon={<FilterOutlined />}
                            style={{ width: 160 }}
                        >
                            <Option value="default">Filter by Default</Option>
                            <Option value="available">Available</Option>
                            <Option value="sale">On Sale</Option>
                        </Select>
                    </div>
                </Col>
            </Row>
        </Card>
    );
};

export default ProductHeader;
