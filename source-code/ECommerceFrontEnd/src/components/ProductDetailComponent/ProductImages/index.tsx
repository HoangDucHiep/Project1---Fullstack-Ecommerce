import React, { useRef, useState } from "react";
import Slider from "react-slick";
import { Image } from "antd";
import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

interface ProductGalleryProps {
    images: string[];
}

const ProductGallery: React.FC<ProductGalleryProps> = ({ images }) => {
    const [current, setCurrent] = useState<number>(0);
    const sliderRef = useRef<Slider | null>(null);

    const settings = {
        dots: false,
        infinite: false,
        speed: 300,
        slidesToShow: 4, // ✅ Hiển thị 4 ảnh nhỏ
        slidesToScroll: 1,
        arrows: false,
    };

    return (
        <div style={{ textAlign: "center" }}>
            {/* ẢNH CHÍNH */}
            <div
                style={{
                    width: "100%",
                    borderRadius: 10,
                    overflow: "hidden",
                }}
            >
                <Image
                    src={images[current]}
                    alt={`Ảnh ${current + 1}`}
                    width="100%"
                    height={400}
                    style={{
                        objectFit: "cover",
                        borderRadius: 10,
                        userSelect: "none",
                    }}
                    preview={false}
                />
            </div>

            {/* CAROUSEL ẢNH NHỎ */}
            <div
                style={{
                    marginTop: 12,
                    position: "relative",
                    width: "100%",
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                }}
            >
                {/* Nút điều hướng trái */}
                <LeftOutlined
                    onClick={() => sliderRef.current?.slickPrev()}
                    style={{
                        position: "absolute",
                        left: 8,
                        zIndex: 10,
                        color: "#666",
                        fontSize: 18,
                        background: "#fff",
                        borderRadius: "50%",
                        padding: 6,
                        cursor: "pointer",
                        boxShadow: "0 0 4px rgba(0,0,0,0.2)",
                    }}
                />

                <div
                    style={{
                        width: "60%",
                        overflow: "hidden",
                    }}
                >
                    <Slider ref={sliderRef} {...settings}>
                        {images.map((thumb, index) => (
                            <div key={index}>
                                <div
                                    onClick={() => setCurrent(index)}
                                    style={{
                                        width: 75,
                                        height: 75,
                                        border:
                                            current === index
                                                ? "2px solid #1677ff"
                                                : "2px solid transparent",
                                        borderRadius: 8,
                                        overflow: "hidden",
                                        cursor: "pointer",
                                        transition: "all 0.2s ease",
                                        marginRight: 10, // ✅ khoảng cách giữa các ảnh
                                    }}
                                >
                                    <img
                                        src={thumb}
                                        alt={`Thumb ${index + 1}`}
                                        style={{
                                            width: "100%",
                                            height: "100%",
                                            objectFit: "cover",
                                            display: "block",
                                        }}
                                    />
                                </div>
                            </div>
                        ))}
                    </Slider>
                </div>

                {/* Nút điều hướng phải */}
                <RightOutlined
                    onClick={() => sliderRef.current?.slickNext()}
                    style={{
                        position: "absolute",
                        right: 8,
                        zIndex: 10,
                        color: "#666",
                        fontSize: 18,
                        background: "#fff",
                        borderRadius: "50%",
                        padding: 6,
                        cursor: "pointer",
                        boxShadow: "0 0 4px rgba(0,0,0,0.2)",
                    }}
                />
            </div>
        </div>
    );
};

export default ProductGallery;

