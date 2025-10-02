import React from "react";
import { Row, Col, Typography, Button, Card } from "antd";

const { Title } = Typography;

interface Category {
    id: number;
    name: string;
    image: string;
    bgColor: string;
}

const categories: Category[] = [
    { id: 1, name: "Men's Fashion", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#cceeff" },
    { id: 2, name: "Women's Fashion", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#ffe0cc" },
    { id: 3, name: "Kid's Fashion", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#ffccdd" },
    { id: 4, name: "Health & Beauty", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#e2f7d3" },
    { id: 5, name: "Pet Supplies", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#c8f4f9" },
    { id: 6, name: "Home & Kitchen", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#dfffe2" },
    { id: 7, name: "Baby & Toddler", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#ffe6e6" },
    { id: 8, name: "Sports & Outdoor", image: "https://6valley.6amtech.com/storage/app/public/brand/2024-09-17-66e954f96bb4c.webp", bgColor: "#cce6ff" },
];

const BrandMenu: React.FC = () => {
    return (
        <Card style={{ borderRadius: 12 }}>
            <Row justify="center" style={{ marginBottom: 10 }}>
                <Title level={4} style={{ color: "#0d6efd", margin: 0 }}>
                    Brand
                </Title>
            </Row>
            <Row justify="end" style={{ marginBottom: 20 }}>
                <Button type="link" style={{ fontWeight: 500 }}>
                    View All
                </Button>
            </Row>

            <Row gutter={[32, 24]} justify="center">
                {categories.map((cat) => (
                    <Col key={cat.id} xs={8} sm={6} md={4} lg={3} style={{ textAlign: "center" }}>
                        <div
                            style={{
                                width: 100,
                                height: 100,
                                borderRadius: "50%",
                                backgroundColor: cat.bgColor,
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                margin: "0 auto 10px",
                            }}
                        >
                            <img src={cat.image} alt={cat.name} style={{ width: 60, height: 60, objectFit: "contain" }} />
                        </div>
                        <span style={{ fontSize: 14, fontWeight: 500 }}>{cat.name}</span>
                    </Col>
                ))}
            </Row>
        </Card>
    );
};

export default BrandMenu;
