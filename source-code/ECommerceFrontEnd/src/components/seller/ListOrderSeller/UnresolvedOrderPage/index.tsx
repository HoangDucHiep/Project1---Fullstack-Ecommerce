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

const UnresolvedOrderPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "DH-1005",
            date: "30 tháng 10 năm 2025, 09:15 sáng",
            customer: "Chris Evans",
            phone: "+1*********",
            total: "1.250,00 đô la",
            status: "Chờ xử lý",
        },
        {
            key: "2",
            id: "DH-1003",
            date: "28 tháng 10 năm 2025, 14:25 chiều",
            customer: "Scarlett Johansson",
            phone: "+1*********",
            total: "875,00 đô la",
            status: "Đang giao hàng",
        },
        {
            key: "3",
            id: "DH-1002",
            date: "26 tháng 10 năm 2025, 10:42 sáng",
            customer: "Mark Ruffalo",
            phone: "+1*********",
            total: "560,00 đô la",
            status: "Chưa thanh toán",
        },
        {
            key: "4",
            id: "DH-1001",
            date: "25 tháng 10 năm 2025, 16:05 chiều",
            customer: "Jeremy Renner",
            phone: "+1*********",
            total: "1.020,00 đô la",
            status: "Chưa xác nhận",
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
                    case "Chưa thanh toán":
                        return <Tag color="red">{status}</Tag>;
                    case "Chưa xác nhận":
                        return <Tag color="orange">{status}</Tag>;
                    case "Đang giao hàng":
                        return <Tag color="blue">{status}</Tag>;
                    case "Chờ xử lý":
                        return <Tag color="gold">{status}</Tag>;
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
                    ⚠️ Đơn hàng chưa giải quyết{" "}
                    <Tag color="red" style={{ fontSize: 14, padding: "2px 8px" }}>
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
                            <Option value="Chris Evans">Chris Evans</Option>
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
                        Danh Sách Đơn Hàng Chưa Giải Quyết <Tag color="red">12</Tag>
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
                    box-shadow: 0 4px 16px rgba(255, 99, 71, 0.25);
                    transform: translateY(-2px);
                }
                .table-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 15px;
                }
                .export-btn {
                    background-color: #fff1f0 !important;
                    color: #cf1322 !important;
                    border-color: #ffa39e !important;
                    transition: all 0.3s ease;
                }
                .export-btn:hover {
                    background-color: #ffccc7 !important;
                    color: #a8071a !important;
                }
            `}</style>
        </div>
    );
};

export default UnresolvedOrderPage;
