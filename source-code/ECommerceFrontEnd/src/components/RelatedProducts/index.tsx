import React from "react";
import { Card, Row, Col, Typography, Tag } from "antd";
import img from '../../assets/img/SamSungS24 Ultra.jpg'
const { Text, Title } = Typography;

interface Product {
    id: number;
    name: string;
    price: string;
    sold: string;
    image: string;
    discount?: string;
    tag?: string;
}

const RelatedProducts: React.FC = () => {
    const otherProducts: Product[] = [
        {
            id: 1,
            name: "Nồi Cơm Điện mini 1L-1.2L-1.8L...",
            price: "209.000đ",
            sold: "Đã bán 100+",
            image: img,
            discount: "-25%",
            tag: "10-10",
        },
        {
            id: 2,
            name: "(Mẫu 2025) Quạt mini hình thú dễ thương...",
            price: "117.000đ",
            sold: "Đã bán 1k+",
            image: img,
            discount: "-10%",
            tag: "10-10",
        },
        {
            id: 3,
            name: "Vali kéo du lịch size 20-24...",
            price: "200.000đ",
            sold: "Đã bán 10k+",
            image: img,
            discount: "-10%",
            tag: "10-10",
        },
        {
            id: 4,
            name: "Bộ 6 nhựa kèm chậu 6 món cao cấp",
            price: "72.000đ",
            sold: "Đã bán 3k+",
            image: img,
            discount: "-19%",
            tag: "10-10",
        },
        {
            id: 11,
            name: "Quạt mini để bàn đa năng có đèn LED",
            price: "108.000đ",
            sold: "Đã bán 6k+",
            image: img,
            discount: "-24%",
            tag: "10-10",
        },
        {
            id: 12,
            name: "Quạt mini để bàn đa năng có đèn LED",
            price: "108.000đ",
            sold: "Đã bán 6k+",
            image: img,
            discount: "-24%",
            tag: "10-10",
        },

    ];

    const suggestProducts: Product[] = [
        {
            id: 5,
            name: "Quạt mini để bàn hình thú dễ thương",
            price: "119.000đ",
            sold: "Đã bán 1k+",
            image: img,
            discount: "-23%",
            tag: "10-10",
        },
        {
            id: 6,
            name: "Quạt mini cầm tay hình thú đáng yêu",
            price: "93.000đ",
            sold: "Đã bán 3k+",
            image: img,
            discount: "-32%",
            tag: "10-10",
        },
        {
            id: 7,
            name: "Quạt mini để bàn đa năng có đèn LED",
            price: "108.000đ",
            sold: "Đã bán 6k+",
            image: img,
            discount: "-24%",
            tag: "10-10",
        },
        {
            id: 8,
            name: "Quạt mini để bàn đa năng có đèn LED",
            price: "108.000đ",
            sold: "Đã bán 6k+",
            image: img,
            discount: "-24%",
            tag: "10-10",
        },
        {
            id: 9,
            name: "Quạt mini để bàn đa năng có đèn LED",
            price: "108.000đ",
            sold: "Đã bán 6k+",
            image: img,
            discount: "-24%",
            tag: "10-10",
        },
        {
            id: 10,
            name: "Quạt mini để bàn đa năng có đèn LED",
            price: "108.000đ",
            sold: "Đã bán 6k+",
            image: img,
            discount: "-24%",
            tag: "10-10",
        },
    ];

    const renderProduct = (item: Product) => (
        <Col key={item.id} xs={12} sm={8} md={6} lg={4} style={{ marginBottom: 16 }}>
            <Card
                hoverable
                cover={<img alt={item.name} src={item.image} style={{ height: 150, objectFit: "cover" }} />}
                bodyStyle={{ padding: "10px" }}
            >
                <div style={{ display: "flex", justifyContent: "space-between" }}>
                    {item.tag && (
                        <Tag color="orange" style={{ fontWeight: 600 }}>
                            {item.tag}
                        </Tag>
                    )}
                    {item.discount && <Tag color="red">{item.discount}</Tag>}
                </div>
                <Text strong style={{ fontSize: 13 }}>
                    {item.name}
                </Text>
                <div style={{ marginTop: 6 }}>
                    <Text type="danger" strong>
                        {item.price}
                    </Text>
                    <br />
                    <Text type="secondary" style={{ fontSize: 12 }}>
                        {item.sold}
                    </Text>
                </div>
            </Card>
        </Col>
    );

    return (
        <div style={{ padding: 24 , background: "#fff", marginTop:20  }}>
            <Title level={5}>CÁC SẢN PHẨM KHÁC CỦA SHOP</Title>
            <Row gutter={[16, 16]}>{otherProducts.map(renderProduct)}</Row>

            <div style={{ marginTop: 40 }}>
                <Title level={5}>CÓ THỂ BẠN CŨNG THÍCH</Title>
                <Row gutter={[16, 16]}>{suggestProducts.map(renderProduct)}</Row>
            </div>
        </div>
    );
};

export default RelatedProducts;
