import React from "react";
import { Row, Col, Typography, Button, Card, Rate } from "antd";

const { Title } = Typography;

interface Seller {
    id: number;
    name: string;
    logo: string;
    banner: string;
    reviews: number;
    products: number;
    rating?: number;
}

const sellers: Seller[] = [
    {
        id: 1,
        name: "Kids Corner",
        logo: "https://6valley.6amtech.com/storage/app/public/shop/2024-09-19-66ebf28839ba7.webp",
        banner: "https://6valley.6amtech.com/storage/app/public/shop/banner/2024-09-23-66f13c3b3fa6b.webp",
        reviews: 0,
        products: 11,
    },
    {
        id: 2,
        name: "Hanover Electronics",
        logo: "https://6valley.6amtech.com/storage/app/public/shop/2024-09-19-66ebf28839ba7.webp",
        banner: "https://6valley.6amtech.com/storage/app/public/shop/banner/2024-09-23-66f13c3b3fa6b.webp",
        reviews: 0,
        products: 20,
    },
    {
        id: 3,
        name: "6valley CMS",
        logo: "https://6valley.6amtech.com/storage/app/public/shop/2024-09-19-66ebf28839ba7.webp",
        banner: "https://6valley.6amtech.com/storage/app/public/shop/banner/2024-09-23-66f13c3b3fa6b.webp",
        reviews: 4,
        products: 194,
        rating: 4.8,
    },
    {
        id: 4,
        name: "Bicycle Shop",
        logo: "https://6valley.6amtech.com/storage/app/public/shop/2024-09-19-66ebf28839ba7.webp",
        banner: "https://6valley.6amtech.com/storage/app/public/shop/banner/2024-09-23-66f13c3b3fa6b.webp",
        reviews: 0,
        products: 14,
    },
];

const TopSellers: React.FC = () => {
    return (
        <Card style={{ borderRadius: 12 }}>
            <Row justify="center" style={{ marginBottom: 10 }}>
                <Title level={4} style={{ color: "#0d6efd", margin: 0 }}>
                    Top Seller
                </Title>
            </Row>
            <Row justify="end" style={{ marginBottom: 20 }}>
                <Button type="link" style={{ fontWeight: 500 }}>
                    View All
                </Button>
            </Row>

            <Row gutter={[24, 24]}>
                {sellers.map((seller) => (
                    <Col xs={24} sm={12} md={8} lg={6} key={seller.id}>
                        <Card
                            style={{
                                borderRadius: 12,
                                overflow: "hidden",
                                padding: 0,
                            }}
                            bodyStyle={{ padding: 0 }}
                        >
                            {/* Banner */}
                            <div
                                style={{
                                    backgroundImage: `url(${seller.banner})`,
                                    backgroundSize: "cover",
                                    backgroundPosition: "center",
                                    height: 120,
                                    position: "relative",
                                }}
                            >
                                {/* Logo tròn ở giữa */}
                                <div
                                    style={{
                                        width: 70,
                                        height: 70,
                                        borderRadius: "50%",
                                        background: "#fff",
                                        display: "flex",
                                        alignItems: "center",
                                        justifyContent: "center",
                                        position: "absolute",
                                        bottom: -35,
                                        left: "50%",
                                        transform: "translateX(-50%)",
                                        boxShadow: "0 4px 6px rgba(0,0,0,0.1)",
                                    }}
                                >
                                    <img
                                        src={seller.logo}
                                        alt={seller.name}
                                        style={{ width: 40, height: 40, objectFit: "contain" }}
                                    />
                                </div>
                            </div>

                            {/* Nội dung */}
                            <div style={{ padding: "50px 16px 16px", textAlign: "center" }}>
                                <Title level={5} style={{ marginBottom: 8 }}>
                                    {seller.name}
                                </Title>
                                {seller.rating && (
                                    <div style={{ marginBottom: 8 }}>
                                        <Rate disabled defaultValue={seller.rating} allowHalf style={{ fontSize: 14 }} />
                                        <span style={{ marginLeft: 6, fontSize: 12 }}>
                      {seller.rating} ★ Rating
                    </span>
                                    </div>
                                )}
                                <Row justify="space-around" style={{ marginTop: 12 }}>
                                    <Col style={{ textAlign: "center" }}>
                                        <Title level={5} style={{ margin: 0 }}>
                                            {seller.reviews}
                                        </Title>
                                        <span style={{ fontSize: 12 }}>Reviews</span>
                                    </Col>
                                    <Col style={{ textAlign: "center" }}>
                                        <Title level={5} style={{ margin: 0 }}>
                                            {seller.products}
                                        </Title>
                                        <span style={{ fontSize: 12 }}>Products</span>
                                    </Col>
                                </Row>
                            </div>
                        </Card>
                    </Col>
                ))}
            </Row>
        </Card>
    );
};

export default TopSellers;
