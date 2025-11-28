import React, { useState } from "react";
import {
    Card,
    Row,
    Col,
    Button,
    Typography,
    Table,
    Tag,
    Input,
    DatePicker,
    Space,
    Select,
} from "antd";
import {
    SearchOutlined,
    FileExcelOutlined,
    EyeOutlined,
    ReloadOutlined,
} from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";

const { Title, Text } = Typography;
const { Option } = Select;
const { RangePicker } = DatePicker;

interface Order {
    key: string;
    id: string;
    date: string;
    customer: string;
    phone: string;
    total: string;
    status: string;
}

const ShippingOrderPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "DH-2050",
            date: "03 tháng 11 năm 2025, 10:30 sáng",
            customer: "Benedict Cumberbatch",
            phone: "+1*********",
            total: "1.200,00 đô la",
            status: "Đang giao hàng",
        },
        {
            key: "2",
            id: "DH-2048",
            date: "02 tháng 11 năm 2025, 15:45 chiều",
            customer: "Scarlett Johansson",
            phone: "+1*********",
            total: "890,00 đô la",
            status: "Đang giao hàng",
        },
        {
            key: "3",
            id: "DH-2045",
            date: "01 tháng 11 năm 2025, 08:20 sáng",
            customer: "Mark Ruffalo",
            phone: "+1*********",
            total: "1.450,00 đô la",
            status: "Đang giao hàng",
        },
        {
            key: "4",
            id: "DH-2042",
            date: "31 tháng 10 năm 2025, 21:10 tối",
            customer: "Chris Evans",
            phone: "+1*********",
            total: "1.050,00 đô la",
            status: "Đang giao hàng",
        },
    ]);

    const columns: ColumnsType<Order> = [
        { title: "SL", dataIndex: "key", width: 60 },
        { title: "Mã Đơn Hàng", dataIndex: "id", width: 120 },
        { title: "Ngày Đặt Hàng", dataIndex: "date", width: 250 },
        {
            title: "Thông Tin Khách Hàng",
            render: (_, record) => (
                <>
                    <Text strong>{record.customer}</Text>
                    <br />
                    <Text type="secondary">{record.phone}</Text>
                </>
            ),
            width: 220,
        },
        { title: "Tổng Số Tiền", dataIndex: "total", width: 160 },
        {
            title: "Trạng Thái Đơn Hàng",
            dataIndex: "status",
            render: (status: string) => <Tag color="blue">{status}</Tag>,
        },
        {
            title: "Hoạt Động",
            render: () => (
                <Space>
                    <Button type="link" icon={<EyeOutlined />} />
                    <Button type="link" icon={<ReloadOutlined />} />
                </Space>
            ),
        },
    ];

    return (
        <div className="order-page">
            {/* --- Header --- */}
            <div className="page-header">
                <Title level={3}>
                    🚚 Đơn hàng đang giao hàng{" "}
                    <Tag color="blue" style={{ fontSize: 14, padding: "2px 8px" }}>
                        12
                    </Tag>
                </Title>
            </div>

            {/* --- Filter Section --- */}
            <Card className="hover-card">
                <Title level={5}>Bộ Lọc</Title>
                <Row gutter={[16, 16]}>
                    <Col xs={24} sm={12} md={8} lg={6}>
                        <Text>Nhập Mã Đơn</Text>
                        <Input placeholder="Nhập mã đơn hàng..." allowClear />
                    </Col>
                    <Col xs={24} sm={12} md={8} lg={6}>
                        <Text>Khách Hàng</Text>
                        <Select defaultValue="Tất cả khách hàng" style={{ width: "100%" }}>
                            <Option value="Tất cả khách hàng">Tất cả khách hàng</Option>
                            <Option value="Benedict Cumberbatch">Benedict Cumberbatch</Option>
                            <Option value="Scarlett Johansson">Scarlett Johansson</Option>
                        </Select>
                    </Col>
                    <Col xs={24} sm={12} md={8} lg={6}>
                        <Text>Khoảng Ngày</Text>
                        <RangePicker style={{ width: "100%" }} />
                    </Col>
                    <Col
                        xs={24}
                        sm={12}
                        md={8}
                        lg={6}
                        style={{ display: "flex", alignItems: "flex-end", gap: 10 }}
                    >
                        <Button icon={<ReloadOutlined />}>Cài lại</Button>
                        <Button type="primary" icon={<SearchOutlined />}>
                            Hiển thị dữ liệu
                        </Button>
                    </Col>
                </Row>
            </Card>

            {/* --- Table Section --- */}
            <Card className="hover-card">
                <div className="table-header">
                    <Title level={5} style={{ margin: 0 }}>
                        Danh Sách Đơn Hàng Đang Giao Hàng <Tag color="blue">12</Tag>
                    </Title>
                    <Space>
                        <Input
                            prefix={<SearchOutlined />}
                            placeholder="Tìm kiếm đơn hàng"
                            style={{ width: 250 }}
                        />
                        <Button type="primary">Tìm kiếm</Button>
                        <Button icon={<FileExcelOutlined />} className="export-btn">
                            Xuất khẩu
                        </Button>
                    </Space>
                </div>

                <Table
                    columns={columns}
                    dataSource={orders}
                    pagination={{
                        total: 12,
                        pageSize: 7,
                        showSizeChanger: false,
                        position: ["bottomRight"],
                    }}
                    bordered
                />
            </Card>

            {/* --- CSS nội tuyến --- */}
            <style>{`
                .order-page {
                    padding: 20px;
                    background-color: #f9fafc;
                    min-height: 100vh;
                }
                .page-header {
                    margin-bottom: 20px;
                }
                .hover-card {
                    transition: all 0.3s ease;
                    border-radius: 10px !important;
                    margin-bottom: 20px;
                }
                .hover-card:hover {
                    box-shadow: 0 4px 16px rgba(24, 144, 255, 0.25);
                    transform: translateY(-2px);
                }
                .table-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 15px;
                }
                .export-btn {
                    background-color: #e6f7ff !important;
                    color: #1890ff !important;
                    border-color: #91d5ff !important;
                    transition: all 0.3s ease;
                }
                .export-btn:hover {
                    background-color: #bae7ff !important;
                    color: #096dd9 !important;
                }
            `}</style>
        </div>
    );
};

export default ShippingOrderPage;
