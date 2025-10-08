import Footer from "../../layouts/Footer";
import Header from "../../layouts/Header";
import ProductHeader from "../../components/SearchPage/ProductHeader";
import FilterSidebar from "../../components/SearchPage/FilterSidebar";
import {Col, Layout, Row} from "antd";
import ProductList from "../../components/SearchPage/ProductList";

const { Content } = Layout;

const SearchPage = () => {
    return (
        <Layout style={{ background: "#fff" }}>
            {/* Header */}
            <Header />

            {/* Phần tiêu đề sản phẩm */}
            <div
                style={{
                    padding: "20px 120px", // 👈 tăng khoảng cách hai bên
                    borderBottom: "1px solid #f0f0f0",
                    backgroundColor: "#fff",
                }}
            >
                <ProductHeader />
            </div>

            {/* Nội dung chính */}
            <Content
                style={{
                    padding: "30px 120px", // 👈 đẩy nội dung ra giữa nhiều hơn
                    background: "#fafafa",
                    minHeight: "50vh",
                }}
            >
                <Row gutter={[32, 32]}>
                    <Col xs={24} md={6} lg={5}>
                        <FilterSidebar />
                    </Col>
                    <Col xs={24} md={18} lg={19}>
                        <ProductList />
                    </Col>
                </Row>
            </Content>

            {/* Footer */}
            <div style={{ marginTop: "60px" }}>
                <Footer />
            </div>
        </Layout>
    );
};

export default SearchPage;