import React, { useState } from "react";
import {
    Card,
    Select,
    Button,
    Row,
    Col,
    Typography,
    Divider,
    Tooltip,
    Avatar,
    Space,
} from "antd";
import {
    ShoppingCartOutlined,
    DollarOutlined,
    InfoCircleOutlined,
    BarChartOutlined, // Đã thay đổi icon
    PieChartOutlined
} from "@ant-design/icons";
import {
    BarChart, // Đã thay đổi từ LineChart
    Bar, // Thêm component Bar
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip as ReTooltip,
    ResponsiveContainer,
    PieChart,
    Pie,
    Cell,
} from "recharts";

const { Title, Text } = Typography;
const { Option } = Select;

// Hàm định dạng tiền tệ VND tiêu chuẩn (ví dụ: 1.234.567.890 VND)
const formatVND = (value) =>
    `${value.toLocaleString('vi-VN')} VND`;

// Custom Tooltip cho Recharts (hiển thị giá trị VND đầy đủ)
const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
        // Lấy dữ liệu từ payload[0]
        const dataValue = payload[0].value;
        const color = payload[0].color || payload[0].fill || '#1890ff';

        return (
            <div className="custom-tooltip" style={{
                backgroundColor: '#fff',
                padding: '10px',
                border: '1px solid #ccc',
                borderRadius: '8px',
                boxShadow: '0 2px 8px rgba(0, 0, 0, 0.15)'
            }}>
                <p className="label" style={{ fontWeight: 'bold', margin: 0 }}>{`Tháng ${label}`}</p>
                <p className="intro" style={{ margin: '4px 0 0 0', color: color }}>
                    {`Doanh thu: ${formatVND(dataValue)}`}
                </p>
            </div>
        );
    }
    return null;
};

// Dữ liệu mẫu (ĐÃ CẬP NHẬT: Thêm dữ liệu cho các tháng)
const orderData = [
    { month: "Jan", value: 350000000 },
    { month: "Feb", value: 420000000 },
    { month: "Mar", value: 550000000 },
    { month: "Apr", value: 780000000 },
    { month: "May", value: 890000000 },
    { month: "Jun", value: 920000000 },
    { month: "Jul", value: 1100000000 },
    { month: "Aug", value: 1350000000 },
    { month: "Sep", value: 2760000000 }, // Đỉnh điểm (Peak)
    { month: "Oct", value: 1850000000 },
    { month: "Nov", value: 1500000000 },
    { month: "Dec", value: 2100000000 },
];

const paymentData = [
    { name: "Tiền mặt", value: 807379200, color: "#1677ff" },
    { name: "Kỹ thuật số", value: 309120000, color: "#52c41a" },
    { name: "Ví điện tử", value: 1426836000, color: "#faad14" },
];

const totalPaymentResolved = paymentData.reduce((sum, item) => item.value > 0 ? sum + item.value : sum, 0);

const totalOrders = 250; // Tăng số lượng đơn hàng cho phù hợp với dữ liệu lớn
const canceledOrders = 40;
const ongoingOrders = 110;
const completedOrders = 100;
const totalAmount = orderData.reduce((sum, item) => sum + item.value, 0); // Tính tổng từ dữ liệu mới

const SalesChart = () => {
    const [filter, setFilter] = useState("year");

    const CardMetric = ({ title, value, icon, color, subText }) => (
        <Card
            style={{
                marginBottom: 24,
                borderRadius: 12,
                boxShadow: '0 4px 12px rgba(0, 0, 0, 0.05)', // Bóng đổ nhẹ nhàng
                border: 'none',
                transition: 'all 0.3s'
            }}
            hoverable
        >
            <div style={{ display: "flex", alignItems: "center", justifyContent: 'space-between' }}>
                <Space direction="vertical" size={4}>
                    <Text
                        type="secondary"
                        style={{ display: 'block', textTransform: 'uppercase', fontSize: 13, fontWeight: 500 }} // Tinh chỉnh font
                    >
                        {title}
                    </Text>
                    <Title
                        level={2}
                        style={{ margin: 0, fontSize: '24px', fontWeight: 800 }} // Số liệu cực kỳ nổi bật
                    >
                        {value}
                    </Title>
                </Space>
                <Avatar
                    size={50}
                    icon={icon}
                    style={{ background: color + '22', color: color, fontSize: 24 }} // Màu nền nhạt, icon màu đậm
                />
            </div>

            <Divider style={{ margin: '16px 0' }} />

            <div style={{ display: "flex", justifyContent: "space-between", fontSize: 13, color: '#595959' }}>
                {subText}
                <Tooltip title="Thông tin chi tiết">
                    <InfoCircleOutlined style={{ color: '#aaa', cursor: 'pointer' }} />
                </Tooltip>
            </div>
        </Card>
    );

    return (
        <div style={{
            padding: 32,
            minHeight: '100vh',
            background: '#f8f9fb',
            fontFamily: 'Inter, sans-serif'
        }}>
            <Title level={3} style={{ marginTop: 0, marginBottom: 24, color: '#262626', fontWeight: 700 }}>
                Bảng điều khiển & Phân tích bán hàng
            </Title>

            {/* Thanh Filter (đặt ở trên) */}
            <div style={{
                marginBottom: 32,
                padding: '16px 20px',
                backgroundColor: '#ffffff',
                borderRadius: 12,
                boxShadow: '0 1px 3px rgba(0,0,0,0.08)'
            }}>
                <Text strong style={{ display: 'block', marginBottom: 8, color: '#595959' }}>Khoảng thời gian</Text>
                <Space size="middle">
                    <Select
                        defaultValue="year"
                        style={{ width: 160 }}
                        onChange={setFilter}
                    >
                        <Option value="today">Hôm nay</Option>
                        <Option value="month">Tháng này</Option>
                        <Option value="year">Năm nay</Option>
                    </Select>
                    <Button type="primary" style={{ borderRadius: 8 }}>Áp dụng</Button>
                </Space>
            </div>

            <Row gutter={[32, 32]}>
                {/* Cột Thống kê chính (Tổng Đơn & Tổng Tiền) */}
                <Col xs={24} lg={8}>

                    {/* Card 1: Tổng đơn hàng */}
                    <CardMetric
                        title="Tổng đơn hàng"
                        value={totalOrders}
                        icon={<ShoppingCartOutlined />}
                        color="#1890ff"
                        subText={
                            <div style={{ display: "flex", justifyContent: "space-between", width: '100%' }}>
                                <Tooltip title="Đã hủy">
                                    <Text type="danger" style={{ cursor: 'pointer' }}>
                                        <span style={{ fontWeight: 'bold' }}>{canceledOrders}</span> Đã hủy
                                    </Text>
                                </Tooltip>
                                <Tooltip title="Đang diễn ra">
                                    <Text type="warning" style={{ cursor: 'pointer' }}>
                                        <span style={{ fontWeight: 'bold' }}>{ongoingOrders}</span> Đang diễn ra
                                    </Text>
                                </Tooltip>
                                <Tooltip title="Hoàn thành">
                                    <Text type="success" style={{ cursor: 'pointer' }}>
                                        <span style={{ fontWeight: 'bold' }}>{completedOrders}</span> Hoàn thành
                                    </Text>
                                </Tooltip>
                            </div>
                        }
                    />

                    {/* Card 2: Tổng số tiền đặt hàng */}
                    <CardMetric
                        title="Tổng doanh thu"
                        // Sử dụng hàm formatVND cho số tiền lớn
                        value={formatVND(totalAmount)}
                        icon={<DollarOutlined />}
                        color="#faad14"
                        subText={
                            <Text type="secondary">Doanh thu đã chốt thành công trong kỳ</Text>
                        }
                    />
                </Col>

                {/* Cột Biểu đồ (Chiếm 2/3) */}
                <Col xs={24} lg={16}>
                    <Row gutter={[32, 32]}>

                        {/* Biểu đồ Doanh thu theo tháng (Bar Chart) - Đã cập nhật */}
                        <Col xs={24} md={16}>
                            <Card
                                bordered={false}
                                style={{
                                    borderRadius: 12,
                                    height: '100%',
                                    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.05)'
                                }}
                            >
                                <Space align="center" style={{ marginBottom: 16 }}>
                                    <BarChartOutlined style={{ fontSize: 18, color: '#595959' }} />
                                    <Title level={5} style={{ margin: 0, color: '#262626' }}>Thống Kê Doanh Thu Theo Tháng</Title>
                                </Space>
                                <ResponsiveContainer width="100%" height={300}>
                                    <BarChart data={orderData} margin={{ top: 5, right: 0, left: 10, bottom: 5 }}>
                                        <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#e0e0e0" />
                                        <XAxis dataKey="month" stroke="#888" />
                                        {/* Format trục Y: Chia cho 1.000.000.000 để hiển thị "Tỷ VND" */}
                                        <YAxis
                                            tickFormatter={(value) => `${(value / 1000000000).toFixed(1)} Tỷ VND`}
                                            stroke="#888"
                                            domain={[0, 'auto']}
                                        />
                                        <ReTooltip content={<CustomTooltip />} />
                                        <Bar
                                            dataKey="value"
                                            fill="#1890ff"
                                            barSize={30} // Điều chỉnh độ rộng cột
                                            radius={[4, 4, 0, 0]} // Bo tròn góc trên
                                        />
                                    </BarChart>
                                </ResponsiveContainer>
                            </Card>
                        </Col>

                        {/* Biểu đồ Thống Kê Thanh Toán (Pie Chart) */}
                        <Col xs={24} md={8}>
                            <Card
                                bordered={false}
                                style={{
                                    borderRadius: 12,
                                    height: '100%',
                                    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.05)'
                                }}
                            >
                                <Space align="center" style={{ marginBottom: 16 }}>
                                    <PieChartOutlined style={{ fontSize: 18, color: '#595959' }} />
                                    <Title level={5} style={{ margin: 0, color: '#262626' }}>Phân Tích Thanh Toán</Title>
                                </Space>
                                <div style={{ position: 'relative', height: 200, display: 'flex', justifyContent: 'center' }}>
                                    <ResponsiveContainer width="100%" height="100%">
                                        <PieChart>
                                            <Pie
                                                data={paymentData}
                                                dataKey="value"
                                                nameKey="name"
                                                innerRadius={65}
                                                outerRadius={95}
                                                paddingAngle={3}
                                                cornerRadius={5}
                                                startAngle={90}
                                                endAngle={-270}
                                            >
                                                {paymentData.map((entry, index) => (
                                                    <Cell
                                                        key={`cell-${index}`}
                                                        fill={entry.color}
                                                        stroke={entry.color}
                                                        strokeWidth={1}
                                                    />
                                                ))}
                                            </Pie>
                                        </PieChart>
                                    </ResponsiveContainer>

                                    {/* Text ở giữa Pie Chart */}
                                    <div style={{
                                        position: 'absolute',
                                        top: '50%',
                                        left: '50%',
                                        transform: 'translate(-50%, -50%)',
                                        textAlign: 'center',
                                    }}>
                                        <Text strong style={{ fontSize: 18, color: '#262626', lineHeight: 1.2 }}>
                                            {/* HIỂN THỊ VND (Tỷ VND) */}
                                            {`${(totalPaymentResolved / 1000000000).toLocaleString('vi-VN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} tỷ`}
                                        </Text>
                                        <Text type="secondary" style={{ display: 'block', fontSize: 11 }}>
                                            Đã thanh toán
                                        </Text>
                                    </div>
                                </div>

                                {/* Legend */}
                                <div style={{ marginTop: 24, maxHeight: 150, overflowY: 'auto' }}>
                                    {paymentData.map((p, idx) => (
                                        <div key={idx} style={{ display: 'flex', alignItems: 'center', marginBottom: 8 }}>
                                            <span
                                                style={{
                                                    display: "inline-block",
                                                    width: 10,
                                                    height: 10,
                                                    borderRadius: "50%",
                                                    background: p.color,
                                                    marginRight: 10,
                                                    flexShrink: 0
                                                }}
                                            ></span>
                                            <Text style={{ flex: 1, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis', fontSize: 13 }}>
                                                {p.name}: <span style={{ fontWeight: 'bold' }}>{formatVND(p.value)}</span>
                                            </Text>
                                        </div>
                                    ))}
                                </div>
                            </Card>
                        </Col>
                    </Row>
                </Col>
            </Row>
        </div>
    );
};

export default SalesChart;