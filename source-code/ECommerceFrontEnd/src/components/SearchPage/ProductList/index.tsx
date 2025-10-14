import React, { useState } from "react";
import { Card, Row, Col, Rate, Button, Pagination } from "antd";
import { ShoppingCartOutlined } from "@ant-design/icons";
import img from '../../../assets/img/SamSungS24 Ultra.jpg'
const ProductList: React.FC = () => {
    const [currentPage, setCurrentPage] = useState(1);

    // Giả lập danh sách sản phẩm
    const products = Array.from({ length: 24 }).map((_, i) => ({
        id: i + 1,
        name: `[Rom&nd] Son Tint lì cho môi căng mọng Hàn Quốc ${i + 1}`,
        price: 218000,
        sold: 19171,
        rating: 4.5,
        reviews: 145657,
        image:img, // bạn có thể thay ảnh khác
    }));

    const pageSize = 8;
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedProducts = products.slice(startIndex, endIndex);

    return (
        <div style={{ padding: "0 24px" }}>
            <Row gutter={[24, 24]}>
                {paginatedProducts.map((product) => (
                    <Col xs={24} sm={12} md={8} lg={6} key={product.id}>
                        <Card
                            hoverable
                            cover={
                                <img
                                    alt={product.name}
                                    src={product.image}
                                    style={{
                                        height: 200,
                                        objectFit: "cover",
                                        borderTopLeftRadius: 8,
                                        borderTopRightRadius: 8,
                                    }}
                                />
                            }
                            style={{
                                borderRadius: 12,
                                overflow: "hidden",
                                boxShadow: "0 2px 8px rgba(0, 0, 0, 0.1)",
                            }}
                            bodyStyle={{ padding: "12px 16px" }}
                        >
                            <div
                                style={{
                                    fontWeight: 600,
                                    marginBottom: 4,
                                    fontSize: 15,
                                    lineHeight: "20px",
                                    height: 40,
                                    overflow: "hidden",
                                }}
                            >
                                {product.name}
                            </div>
                            <div
                                style={{
                                    color: "#d0021b",
                                    fontWeight: 600,
                                    marginBottom: 4,
                                    fontSize: 16,
                                }}
                            >
                                {product.price.toLocaleString()}₫
                            </div>
                            <div
                                style={{
                                    display: "flex",
                                    alignItems: "center",
                                    justifyContent: "space-between",
                                    fontSize: 13,
                                    color: "#999",
                                    marginBottom: 8,
                                }}
                            >
                                <span>Đã bán {product.sold}</span>
                                <span>
                  <Rate
                      disabled
                      defaultValue={product.rating}
                      allowHalf
                      style={{ fontSize: 14 }}
                  />{" "}
                                    ({product.reviews})
                </span>
                            </div>
                            <Button
                                type="primary"
                                block
                                icon={<ShoppingCartOutlined />}
                                style={{
                                    background: "#ff4d4f",
                                    border: "none",
                                    borderRadius: 6,
                                }}
                            >
                                Thêm vào giỏ hàng
                            </Button>
                        </Card>
                    </Col>
                ))}
            </Row>

            {/* Phân trang */}
            <div
                style={{
                    marginTop: 40,
                    justifyContent: "center",
                }}
            >
                <Pagination
                    current={currentPage}
                    pageSize={pageSize}
                    total={products.length}
                    onChange={(page) => setCurrentPage(page)}
                    showSizeChanger={false}
                />
            </div>
        </div>
    );
};

export default ProductList;
