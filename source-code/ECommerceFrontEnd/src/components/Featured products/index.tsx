import React, { useRef } from "react";
import { Carousel, Card, Typography, Button, Row } from "antd";
import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import type { CarouselRef } from "antd/es/carousel";

const { Title, Text } = Typography;
import img from '../../assets/img/SamSungS24 Ultra.jpg'
interface Product {
    id: number;
    name: string;
    price: string;
    image: string;
}

const products: Product[] = [
    {
        id: 1,
        name: "Samsung S24 Ultra",
        price: "$1,150.00",
        image: img,
    },
    {
        id: 2,
        name: "Electric Table Blender",
        price: "$250.00",
        image: img,
    },
    {
        id: 3,
        name: "2020 MacBook Air M1",
        price: "$1,158.00",
        image: img,
    },
    {
        id: 4,
        name: "Fireplaces Cook Stoves",
        price: "$1,200.00",
        image: img,
    },
    {
        id: 5,
        name: "Honor X14 Plus Laptop",
        price: "$2,200.00",
        image: img,
    },
    {
        id: 6,
        name: "Quercetinol Cleansing Gel",
        price: "$10.00",
        image: img,
    },
];

const FeaturedProducts: React.FC = () => {
    const carouselRef = useRef<CarouselRef>(null);

    return (
        <div style={{ padding: "40px 80px", background: "#fff" }}>
            {/* Header */}
            <Row justify="center" style={{ marginBottom: 10 }}>
                <Title level={4} style={{ color: "#0d6efd", margin: 0 }}>
                    Featured products
                </Title>
            </Row>
            <Row justify="end" style={{ marginBottom: 20 }}>
                <Button type="link" style={{ fontWeight: 500 }}>
                    View All
                </Button>
            </Row>


            {/* Carousel */}
            <div style={{ position: "relative" }}>
                <Carousel
                    ref={carouselRef}
                    dots={false}
                    infinite
                    slidesToShow={4}
                    slidesToScroll={1}
                    responsive={[
                        { breakpoint: 1200, settings: { slidesToShow: 3 } },
                        { breakpoint: 768, settings: { slidesToShow: 2 } },
                        { breakpoint: 480, settings: { slidesToShow: 1 } },
                    ]}
                >
                    {products.map((p) => (
                        <div key={p.id} style={{ padding: "0 10px" }}>
                            <Card
                                hoverable
                                cover={
                                    <img
                                        src={p.image}
                                        alt={p.name}
                                        style={{ height: 200, objectFit: "contain", padding: 20 }}
                                    />
                                }
                                style={{ borderRadius: 8 }}
                            >
                                <Text strong>{p.name}</Text>
                                <br />
                                <Text type="secondary">{p.price}</Text>
                            </Card>
                        </div>
                    ))}
                </Carousel>

                {/* Custom arrows */}
                <Button
                    shape="circle"
                    icon={<LeftOutlined />}
                    style={{
                        position: "absolute",
                        top: "40%",
                        left: -20,
                        zIndex: 1,
                        background: "#0d6efd",
                        color: "#fff",
                    }}
                    onClick={() => carouselRef.current?.prev()}
                />
                <Button
                    shape="circle"
                    icon={<RightOutlined />}
                    style={{
                        position: "absolute",
                        top: "40%",
                        right: -20,
                        zIndex: 1,
                        background: "#0d6efd",
                        color: "#fff",
                    }}
                    onClick={() => carouselRef.current?.next()}
                />
            </div>
        </div>
    );
};

export default FeaturedProducts;
