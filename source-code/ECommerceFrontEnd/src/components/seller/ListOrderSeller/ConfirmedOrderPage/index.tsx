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

const ConfirmedOrderPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "CF-1010",
            date: "30 tháng 10 năm 2025, 08:45 sáng",
            customer: "Tony Stark",
            phone: "+1*********",
            total: "980,00 đô la",
            status: "Đã xác nhận",
        },
        {
            key: "2",
            id: "CF-1009",
            date: "29 tháng 10 năm 2025, 12:22 trưa",
            customer: "Steve Rogers",
            phone: "+1*********",
            total: "1.250,00 đô la",
            status: "Đang đóng gói",
        },
        {
            key: "3",
            id: "CF-1008",
            date: "28 tháng 10 năm 2025, 09:10 sáng",
            customer: "Natasha Romanoff",
            phone: "+1*********",
            total: "690,00 đô la",
            status: "Sẵn sàng giao",
        },
        {
            key: "4",
            id: "CF-1007",
            date: "27 tháng 10 năm 2025, 16:40 chiều",
            customer: "Bruce Banner",
            phone: "+1*********",
            total: "2.000,00 đô la",
            status: "Đang chờ xác nhận",
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
                    case "Đã xác nhận":
                        return <Tag color="blue">{status}</Tag>;
                    case "Đang đóng gói":
                        return <Tag color="processing">{status}</Tag>;
                    case "Sẵn sàng giao":
                        return <Tag color="geekblue">{status}</Tag>;
                    case "Đang chờ xác nhận":
                        return <Tag color="orange">{status}</Tag>;
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
                    🧾 Xác nhận đơn hàng{" "}
                    <Tag color="blue" style={{ fontSize: 14, padding: "2px 8px" }}>
                        10
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
                            <Option value="Tony Stark">Tony Stark</Option>
                            <Option value="Steve Rogers">Steve Rogers</Option>
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
                        Danh Sách Đơn Hàng Cần Xác Nhận <Tag color="blue">10</Tag>
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
                        total: 10,
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
                    color: #096dd9 !important;
                    border-color: #91d5ff !important;
                    transition: all 0.3s ease;
                }
                .export-btn:hover {
                    background-color: #bae7ff !important;
                    color: #0050b3 !important;
                }
            `}</style>
        </div>
    );
};

export default ConfirmedOrderPage;
