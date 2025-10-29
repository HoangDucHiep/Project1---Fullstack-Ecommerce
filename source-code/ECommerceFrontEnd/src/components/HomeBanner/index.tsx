import React, { useState } from "react";
import { Button, Row, Col, Typography } from "antd";
import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import img1 from "../../assets/img/SamSungS24 Ultra.jpg";
import img2 from "../../assets/img/logo.png";

const { Title, Paragraph } = Typography;

interface Slide {
    title: string;
    subtitle: string;
    image: string;
}

const slides: Slide[] = [
    {
        title: "Ngôi nhà thông minh – Tiết kiệm thông minh!",
        subtitle: "Những ưu đãi không thể bỏ lỡ cho mọi nhu cầu thiết bị gia dụng của bạn.",
        image: img2,
    },
    {
        title: "Nâng tầm phong cách sống của bạn!",
        subtitle: "Khám phá các thiết bị thông minh mới nhất với mức giá siêu hấp dẫn.",
        image: img1,
    },
    {
        title: "Lựa chọn tiết kiệm năng lượng!",
        subtitle: "Tiết kiệm chi phí và bảo vệ hành tinh với các sản phẩm thân thiện môi trường.",
        image: img1,
    },

];

const Banner: React.FC = () => {
    const [current, setCurrent] = useState(0);

    const nextSlide = () => {
        setCurrent((prev) => (prev + 1) % slides.length);
    };

    const prevSlide = () => {
        setCurrent((prev) => (prev - 1 + slides.length) % slides.length);
    };

    const slide = slides[current];

    return (
        <div
            style={{
                position: "relative",
                overflow: "hidden",
                backgroundColor: "#f9f9f9",
                borderRadius: 16,
                margin: "20px auto",
                width: "95%",
                height: 400,
            }}
        >
            {/* Nút trái */}
            <Button
                type="primary"
                shape="circle"
                icon={<LeftOutlined />}
                onClick={prevSlide}
                style={{
                    position: "absolute",
                    top: "50%",
                    left: 20,
                    transform: "translateY(-50%)",
                    zIndex: 10,
                }}
            />

            {/* Nội dung chính */}
            <Row
                justify="center"
                align="middle"
                style={{
                    height: "100%",
                    padding: "0 80px",
                    transition: "all 0.5s ease",
                }}
            >
                <Col xs={24} md={10}>
                    <Title level={2} style={{ color: "#0050b3" }}>
                        {slide.title}
                    </Title>
                    <Paragraph style={{ fontSize: 16, color: "#555" }}>
                        {slide.subtitle}
                    </Paragraph>
                    <Button type="primary" size="large" style={{ marginTop: 20 }}>
                        Get Yours
                    </Button>
                </Col>

                <Col xs={24} md={10} style={{ textAlign: "center" }}>
                    <img
                        src={slide.image}
                        alt="banner"
                        style={{
                            width: "100%",
                            maxWidth: 500,
                            borderRadius: 12,
                            objectFit: "contain",
                            transition: "all 0.6s ease",
                        }}
                    />
                </Col>
            </Row>

            {/* Nút phải */}
            <Button
                type="primary"
                shape="circle"
                icon={<RightOutlined />}
                onClick={nextSlide}
                style={{
                    position: "absolute",
                    top: "50%",
                    right: 20,
                    transform: "translateY(-50%)",
                    zIndex: 10,
                }}
            />

            {/* Dấu chấm chỉ slide */}
            <div
                style={{
                    position: "absolute",
                    bottom: 20,
                    left: "50%",
                    transform: "translateX(-50%)",
                    display: "flex",
                    gap: 8,
                }}
            >
                {slides.map((_, index) => (
                    <div
                        key={index}
                        style={{
                            width: 10,
                            height: 10,
                            borderRadius: "50%",
                            backgroundColor: current === index ? "#1677ff" : "#d9d9d9",
                            transition: "0.3s",
                            cursor: "pointer",
                        }}
                        onClick={() => setCurrent(index)}
                    />
                ))}
            </div>
        </div>
    );
};

export default Banner;
