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
    DownloadOutlined,
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

const RefundedOrderPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "400123",
            date: "25 tháng 9 năm 2025, 08:40 sáng",
            customer: "Tom Hiddleston",
            phone: "+1*********",
            total: "350,00 đô la",
            status: "Đã hoàn tiền",
        },
        {
            key: "2",
            id: "400088",
            date: "14 tháng 8 năm 2025, 11:22 trưa",
            customer: "Elizabeth Olsen",
            phone: "+1*********",
            total: "920,00 đô la",
            status: "Đã hoàn tiền",
        },
        {
            key: "3",
            id: "400077",
            date: "10 tháng 4 năm 2024, 15:20 chiều",
            customer: "Paul Bettany",
            phone: "+1*********",
            total: "470,00 đô la",
            status: "Đã hoàn tiền",
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
            render: (status: string) => (
                <Tag color="blue">{status}</Tag>
            ),
        },
        {
            title: "Hoạt Động",
            render: () => (
                <Space>
                    <Button type="link" icon={<DownloadOutlined />} />
                </Space>
            ),
        },
    ];

    return (
        <div className="order-page">
            {/* --- Header --- */}
            <div className="page-header">
                <Title level={3}>
                    💸 Đơn hàng đã hoàn tiền{" "}
                    <Tag color="blue" style={{ fontSize: 14, padding: "2px 8px" }}>
                        3
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
                            <Option value="Tom Hiddleston">Tom Hiddleston</Option>
                            <Option value="Elizabeth Olsen">Elizabeth Olsen</Option>
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
                        Danh Sách Đơn Hàng Đã Hoàn Tiền <Tag color="blue">3</Tag>
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
                        total: 3,
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
                    color: #0050b3 !important;
                }
            `}</style>
        </div>
    );
};

export default RefundedOrderPage;
