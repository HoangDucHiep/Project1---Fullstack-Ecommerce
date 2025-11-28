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

const OrderManagerPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "100193",
            date: "22 tháng 9 năm 2025, 19:29 tối",
            customer: "Robert Downey",
            phone: "+1*********",
            total: "165,50 đô la",
            status: "Đã giao hàng",
        },
        {
            key: "2",
            id: "100188",
            date: "10 tháng 1 năm 2024, 18:22 chiều",
            customer: "Devid Jack",
            phone: "+1*********",
            total: "535,00 đô la",
            status: "Đã giao hàng",
        },
        {
            key: "3",
            id: "100181",
            date: "10 tháng 1 năm 2023, 02:56 sáng",
            customer: "Tom Holland",
            phone: "+1*********",
            total: "380,00 đô la",
            status: "Đã giao hàng",
        },
        {
            key: "4",
            id: "100178",
            date: "10 tháng 1 năm 2023, 02:55 sáng",
            customer: "Anthony Mackie",
            phone: "+1*********",
            total: "4.300,00 đô la",
            status: "Chưa thanh toán",
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
            render: (status: string) => {
                switch (status) {
                    case "Đã giao hàng":
                        return <Tag color="green">{status}</Tag>;
                    case "Chưa thanh toán":
                        return <Tag color="red">{status}</Tag>;
                    default:
                        return <Tag>{status}</Tag>;
                }
            },
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
                    📋 Tất cả các đơn hàng{" "}
                    <Tag color="blue" style={{ fontSize: 14, padding: "2px 8px" }}>
                        25
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
                            <Option value="Robert Downey">Robert Downey</Option>
                            <Option value="Devid Jack">Devid Jack</Option>
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
                        Danh Sách Đơn Hàng <Tag color="blue">25</Tag>
                    </Title>
                    <Space>
                        <Input
                            prefix={<SearchOutlined />}
                            placeholder="Tìm kiếm đơn hàng"
                            style={{ width: 250 }}
                        />
                        <Button type="primary">Tìm kiếm</Button>
                        <Button icon={<FileExcelOutlined />} className="export-btn">
                            Xuất file Excel
                        </Button>
                    </Space>
                </div>

                <Table
                    columns={columns}
                    dataSource={orders}
                    pagination={{
                        total: 25,
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
                    box-shadow: 0 4px 16px rgba(76, 175, 80, 0.25);
                    transform: translateY(-2px);
                }
                .table-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 15px;
                }
                .export-btn {
                    background-color: #f6ffed !important;
                    color: #389e0d !important;
                    border-color: #b7eb8f !important;
                    transition: all 0.3s ease;
                }
                .export-btn:hover {
                    background-color: #d9f7be !important;
                    color: #237804 !important;
                }
            `}</style>
        </div>
    );
};

export default OrderManagerPage;
