import {Row, Col, Button, Rate, Typography, Space, Divider, Layout} from "antd";
import VoucherList from '../../components/ProductDetailComponent/VoucherList';
import Header from "../../layouts/Header";
import Footer from "../../layouts/Footer";
import ProductGallery from "../../components/ProductDetailComponent/ProductImages";

const { Title, Text } = Typography;
import img from '../../assets/img/SamSungS24 Ultra.jpg'
import ShopInfo from "../../components/ProductDetailComponent/ShopInfo";
import ProductInfo from "../../components/ProductDetailComponent/ProductInfo";
import RelatedProducts from "../../components/RelatedProducts";
import ProductReview from "../../components/ProductDetailComponent/ProductReview";
const productImages = [img,img,img,img,img];
const { Content } = Layout;
const ProductDetail: React.FC = () => {
    return (
        <>
            <Layout style={{ background: "#fff"  }}>
                {/* Header */}
                <Header />

                {/* Phần tiêu đề sản phẩm */}
                <div
                    style={{
                        padding: "20px 120px", // 👈 tăng khoảng cách hai bên
                        borderBottom: "1px solid #f0f0f0",
                        backgroundColor: "#fafafa",
                    }}
                >
                    <div style={{ padding: "30px 90px", background: "#fff" }}>
                        <Row gutter={32}>
                            {/* Cột hình ảnh */}
                            <Col span={10}>
                                <ProductGallery images={productImages} />
                            </Col>

                            {/* Cột thông tin sản phẩm */}
                            <Col span={14}>
                                <Title level={4} style={{ marginBottom: 8 }}>
                                    Son kem Tint Bóng Hàn Quốc Romand Juicy Lasting Tint 5.5g
                                </Title>

                                <Space align="center" size={8}>
                                    <Rate disabled allowHalf defaultValue={4.9} />
                                    <Text strong>4.9</Text>
                                    <Divider type="vertical" />
                                    <Text type="secondary">54k đánh giá</Text>
                                    <Divider type="vertical" />
                                    <Text type="secondary">Đã bán 200k+</Text>
                                </Space>

                                <Title
                                    level={3}
                                    style={{ color: "#ee4d2d", marginTop: 16, marginBottom: 16 }}
                                >
                                    135.834₫{" "}
                                    <Text delete type="secondary" style={{ fontSize: 16 }}>
                                        250.000₫
                                    </Text>
                                </Title>

                                <VoucherList />

                                <div style={{ marginTop: 24 }}>
                                    <Text strong>Số lượng:</Text>
                                    <Space style={{ marginLeft: 12 }}>
                                        <Button>-</Button>
                                        <Text>1</Text>
                                        <Button>+</Button>
                                    </Space>
                                </div>

                                <div style={{ marginTop: 32 }}>
                                    <Space size={16}>
                                        <Button type="default" size="large">
                                            Thêm vào giỏ hàng
                                        </Button>
                                        <Button
                                            type="primary"
                                            size="large"
                                            style={{ background: "#ee4d2d", borderColor: "#ee4d2d" }}
                                        >
                                            Mua với Voucher 135.834₫
                                        </Button>
                                    </Space>
                                </div>
                            </Col>
                        </Row>
                    </div>
                </div>

                {/* Nội dung chính */}
                <Content
                    style={{
                        padding: "30px 120px", // 👈 đẩy nội dung ra giữa nhiều hơn
                        background: "#fafafa",
                        minHeight: "50vh",
                    }}
                >
                    <ShopInfo />
                    <ProductInfo />
                    <ProductReview />
                    <RelatedProducts />

                </Content>

                {/* Footer */}
                <div style={{ marginTop: "60px" }}>
                    <Footer />
                </div>
            </Layout>
        </>
    );
};

export default ProductDetail;
