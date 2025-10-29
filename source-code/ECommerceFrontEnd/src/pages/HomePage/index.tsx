import React from "react";
import { Row, Col } from "antd";
import Footer from "../../layouts/Footer";
import Header from "../../layouts/Header";
import CategoryManergerComponent from "../../components/CategoryManergerComponent/CategoryManergerComponent.tsx";
import FeaturedProducts from "../../components/Featured products";
import CategoryMenu from "../../components/CategoryMenu";
import BrandMenu from "../../components/Brands";
import TopSeller from "../../components/TopSeller";
import Banner from "../../components/HomeBanner";

const HomePage = () => {
    return (
        <div style={{ backgroundColor: "#fafafa" }}>
            {/* 🌟 Header */}
            <Header />

            {/* 🌟 Danh mục + Banner */}
            <section
                style={{
                    backgroundColor: "#fff",
                    padding: "40px 0",
                    boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
                    borderBottom: "1px solid #f0f0f0",
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        margin: "0 auto",
                        padding: "0 20px",
                    }}
                >
                    <Row gutter={[24, 24]}>
                        <Col xs={24} md={8} lg={6}>
                            <div
                                style={{
                                    border: "1px solid #eee",
                                    borderRadius: 8,
                                    background: "#fafafa",
                                    padding: 12,
                                }}
                            >
                                <h3
                                    style={{
                                        fontSize: 18,
                                        fontWeight: 600,
                                        marginBottom: 12,
                                        color: "#333",
                                    }}
                                >
                                    Danh mục sản phẩm
                                </h3>
                                <CategoryManergerComponent />
                            </div>
                        </Col>

                        <Col xs={24} md={16} lg={18}>
                            <div
                                style={{
                                    borderRadius: 12,
                                    overflow: "hidden",
                                    boxShadow: "0 4px 12px rgba(0,0,0,0.08)",
                                }}
                            >
                                <Banner />
                            </div>
                        </Col>
                    </Row>
                </div>
            </section>

            {/* 🌟 Featured Products */}
            <section
                style={{
                    backgroundColor: "#f9f9f9",
                    padding: "50px 0",
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        margin: "0 auto",
                        padding: "0 20px",
                    }}
                >
                    <h2
                        style={{
                            fontSize: 24,
                            fontWeight: 700,
                            marginBottom: 24,
                            color: "#333",
                            borderLeft: "5px solid #7e22ce",
                            paddingLeft: 12,
                        }}
                    >
                        🌟 Sản phẩm nổi bật
                    </h2>
                    <FeaturedProducts />
                </div>
            </section>

            {/* 🌟 Category Menu */}
            <section
                style={{
                    backgroundColor: "#fff",
                    padding: "50px 0",
                    borderTop: "1px solid #eee",
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        margin: "0 auto",
                        padding: "0 20px",
                    }}
                >
                    <h2
                        style={{
                            fontSize: 24,
                            fontWeight: 700,
                            marginBottom: 24,
                            color: "#333",
                            borderLeft: "5px solid #7e22ce",
                            paddingLeft: 12,
                        }}
                    >
                        🛍️ Danh mục nổi bật
                    </h2>
                    <CategoryMenu />
                </div>
            </section>

            {/* 🌟 Brand Menu */}
            <section
                style={{
                    backgroundColor: "#f9f9f9",
                    padding: "50px 0",
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        margin: "0 auto",
                        padding: "0 20px",
                    }}
                >
                    <h2
                        style={{
                            fontSize: 24,
                            fontWeight: 700,
                            marginBottom: 24,
                            color: "#333",
                            borderLeft: "5px solid #7e22ce",
                            paddingLeft: 12,
                        }}
                    >
                        💎 Thương hiệu nổi bật
                    </h2>
                    <BrandMenu />
                </div>
            </section>

            {/* 🌟 Top Seller */}
            <section
                style={{
                    backgroundColor: "#fff",
                    padding: "50px 0",
                    borderTop: "1px solid #eee",
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        margin: "0 auto",
                        padding: "0 20px",
                    }}
                >
                    <h2
                        style={{
                            fontSize: 24,
                            fontWeight: 700,
                            marginBottom: 24,
                            color: "#333",
                            borderLeft: "5px solid #7e22ce",
                            paddingLeft: 12,
                        }}
                    >
                        🔝 Top người bán
                    </h2>
                    <TopSeller />
                </div>
            </section>

            {/* 🌟 Footer */}
            <Footer />
        </div>
    );
};

export default HomePage;
