import React, { useState } from "react";
import {
    Tabs,
    Card,
    Row,
    Col,
    Button,
    Divider,
    Tag,
    Typography,
    Image,
    Tooltip,
} from "antd";
import {
    MessageOutlined,
    ShopOutlined,
    QuestionCircleOutlined,
} from "@ant-design/icons";
// Import ảnh local (giả sử dùng chung ảnh này cho tất cả sản phẩm)
import img from '../../../assets/img/SamSungS24 Ultra.jpg';

const { Text } = Typography;

interface Product {
    id: number;
    name: string;
    type: string;
    quantity: number;
    price: number;
    oldPrice?: number;
    image: string;
}

interface Order {
    id: number;
    shopName: string;
    status: string;
    products: Product[];
    total: number;
    // Thêm trường phụ trợ để hiển thị thông tin đặc biệt
    specialInfo?: string;
    isMall?: boolean;
    ratingDeadline?: string;
    bonusInfo?: string; // Thông tin thưởng (Đánh giá ngay và nhận 200 Xu)
}

const OrderList: React.FC = () => {
    const [activeTab, setActiveTab] = useState("all");

    // --- DỮ LIỆU MẪU ĐƯỢC CẬP NHẬT THEO HÌNH ẢNH MỚI ---
    const orders: Order[] = [
        {
            id: 1,
            shopName: "BeautyToday.vn",
            status: "CHỜ GIAO HÀNG",
            products: [
                {
                    id: 1,
                    name: "2 Cái / bộ Kẹp Tóc Sóng Hình Học Hợp Kim Thời Trang Cho Bé Gái",
                    type: "Phân loại: 2",
                    quantity: 1,
                    price: 5465,
                    oldPrice: 5508,
                    image: img,
                },
                {
                    id: 2,
                    name: "Bông tai hình xương cá Acrylic thời trang dành cho nữ",
                    type: "Phân loại: 1",
                    quantity: 1,
                    price: 12540,
                    oldPrice: 20990,
                    image: img,
                },
            ],
            total: 18005,
            specialInfo: "Đơn hàng đã được đăng cùng CHIMA",
        },
        {
            id: 2,
            shopName: "ThinkPlus Store.vn",
            status: "HOÀN THÀNH",
            products: [
                {
                    id: 3,
                    name: "Tai nghe Lenovo LE209 Tai nghe Bluetooth Độ trễ thấp chống nước HIFI chất lượng âm thanh Bluetooth V6.0",
                    type: "Phân loại: Trắng",
                    quantity: 1,
                    price: 189000,
                    oldPrice: 249000,
                    image: img,
                },
            ],
            total: 160650,
            isMall: true,
            ratingDeadline: "08-11-2025",
            bonusInfo: "Đánh giá ngay và nhận 200 Xu",
        },
    ];

    const filteredOrders =
        activeTab === "all"
            ? orders
            : orders.filter((o) => o.status === activeTab);

    // Cập nhật tên tab để khớp với số lượng hiển thị (3)
    const items = [
        { key: "all", label: "Tất cả" },
        { key: "Chờ xác nhận", label: "Chờ xác nhận" },
        { key: "Vận chuyển", label: "Vận chuyển" },
        { key: "CHỜ GIAO HÀNG", label: `Chờ giao hàng (${orders.filter(o => o.status === 'CHỜ GIAO HÀNG').length})` },
        { key: "HOÀN THÀNH", label: "Hoàn thành" },
        { key: "Đã hủy", label: "Đã hủy" },
        { key: "Trả hàng", label: "Trả hàng/Hoàn tiền" },
    ];

    // --- RENDER COMPONENT ---
    return (
        <div style={{ background: "#fff", padding: 16, borderRadius: 8 }}>
            <Tabs
                items={items}
                activeKey={activeTab}
                onChange={setActiveTab}
                tabBarGutter={16}
            />
            {filteredOrders.map((order) => (
                <Card
                    key={order.id}
                    style={{
                        marginBottom: 16,
                        borderRadius: 10,
                        boxShadow: "0 1px 4px rgba(0,0,0,0.1)",
                    }}
                    bodyStyle={{ padding: 16 }}
                >
                    {/* Hàng 1: Header - Shop Name & Status */}
                    <Row justify="space-between" align="middle">
                        <Col>
                            {order.isMall && (
                                <Tag color="red" style={{ marginRight: 8, fontWeight: 'bold' }}>Mall</Tag>
                            )}
                            <ShopOutlined style={{ color: "#ff4d4f", marginRight: 8 }} />
                            <Text strong>{order.shopName}</Text>
                            <Button
                                type="primary"
                                danger
                                size="small"
                                icon={<MessageOutlined />}
                                style={{ marginLeft: 16, marginRight: 8 }}
                            >
                                Chat
                            </Button>
                            <Button size="small">Xem Shop</Button>
                        </Col>
                        <Col>
                            {order.status === "HOÀN THÀNH" && (
                                <Text type="success" style={{ marginRight: 8 }}>
                                    Giao hàng thành công
                                </Text>
                            )}
                            {order.specialInfo && (
                                <Text type="secondary" style={{ marginRight: 8, fontSize: 13 }}>
                                    {order.specialInfo}
                                    <Tooltip title="Chi tiết thông tin vận chuyển">
                                        <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                                    </Tooltip>
                                </Text>
                            )}
                            <Tag
                                color={
                                    order.status === "HOÀN THÀNH"
                                        ? "red" // Sử dụng màu đỏ để nổi bật trạng thái HOÀN THÀNH
                                        : order.status === "CHỜ GIAO HÀNG"
                                            ? "orange"
                                            : "blue"
                                }
                                style={{ fontWeight: 'bold' }}
                            >
                                {order.status}
                            </Tag>
                        </Col>
                    </Row>

                    <Divider style={{ margin: "12px 0" }} />

                    {/* Hàng 2: Chi tiết Sản phẩm */}
                    {order.products.map((product, index) => (
                        <Row
                            key={product.id}
                            gutter={12}
                            align="top" // Căn trên cùng
                            style={{ marginBottom: order.products.length > 1 && index < order.products.length - 1 ? 12 : 0 }}
                        >
                            <Col span={4}>
                                <Image
                                    src={product.image}
                                    alt={product.name}
                                    width={80}
                                    height={80}
                                    style={{ borderRadius: 6, objectFit: 'cover' }}
                                    preview={false}
                                />
                            </Col>
                            <Col span={15}>
                                <Text>{product.name}</Text>
                                <br />
                                <Text type="secondary" style={{ fontSize: 12 }}>{product.type}</Text>
                                <br />
                                <Text type="secondary" style={{ fontSize: 12 }}>x{product.quantity}</Text>
                            </Col>
                            {/* Giá sản phẩm */}
                            <Col span={5} style={{ textAlign: 'right' }}>
                                <Text delete type="secondary" style={{ marginRight: 4, fontSize: 13 }}>
                                    {product.oldPrice?.toLocaleString()}₫
                                </Text>
                                <Text strong type="danger">
                                    {product.price.toLocaleString()}₫
                                </Text>
                            </Col>
                        </Row>
                    ))}

                    <Divider style={{ margin: "12px 0" }} />

                    {/* Hàng 3: Footer - Total & Buttons */}
                    <Row justify="space-between" align="middle" style={{ marginTop: 10 }}>
                        <Col>


                        </Col>

                        {/* Tổng tiền và nút hành động */}
                        <Col>
                            <Row align="middle" gutter={8} style={{ marginBottom: 10, justifyContent: 'flex-end' }}>
                                <Col>
                                    <Text style={{ fontSize: 14 }}>
                                        Thành tiền:{" "}
                                        <Text type="danger" style={{ fontSize: 18, fontWeight: 'bold' }}>
                                            {order.total.toLocaleString()}₫
                                        </Text>
                                    </Text>
                                </Col>
                            </Row>

                            <Row gutter={8} justify="end">
                                {order.status === "HOÀN THÀNH" ? (
                                    <>
                                        <Col>
                                            <Button type="primary" danger style={{ height: 35 }}>
                                                Đánh Giá
                                            </Button>
                                        </Col>
                                        <Col>
                                            <Button type="default" style={{ height: 35 }}>
                                                Yêu Cầu Trả Hàng/Hoàn Tiền
                                            </Button>
                                        </Col>
                                        <Col>
                                            <Button type="default" style={{ height: 35 }}>
                                                Thêm
                                            </Button>
                                        </Col>
                                    </>
                                ) : (
                                    <>
                                        <Col>
                                            <Button type="default" disabled style={{ height: 35 }}>
                                                Đã Nhận Hàng
                                            </Button>
                                        </Col>
                                        <Col>
                                            <Button type="default" style={{ height: 35 }}>
                                                Liên Hệ Người Bán
                                            </Button>
                                        </Col>
                                    </>
                                )}
                            </Row>
                        </Col>
                    </Row>
                </Card>
            ))}
        </div>
    );
};

export default OrderList;