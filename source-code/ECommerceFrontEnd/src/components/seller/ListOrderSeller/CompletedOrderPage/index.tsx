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

const CompletedOrderPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "DH-2001",
            date: "28 tháng 10 năm 2025, 09:45 sáng",
            customer: "Robert Downey Jr.",
            phone: "+1*********",
            total: "1.250,00 đô la",
            status: "Đã giao hàng",
        },
        {
            key: "2",
            id: "DH-1999",
            date: "25 tháng 10 năm 2025, 17:10 chiều",
            customer: "Tom Holland",
            phone: "+1*********",
            total: "820,00 đô la",
            status: "Đã giao hàng",
        },
        {
            key: "3",
            id: "DH-1995",
            date: "20 tháng 10 năm 2025, 11:30 sáng",
            customer: "Chris Hemsworth",
            phone: "+1*********",
            total: "1.600,00 đô la",
            status: "Đã giao hàng",
        },
        {
            key: "4",
            id: "DH-1990",
            date: "15 tháng 10 năm 2025, 20:12 tối",
            customer: "Elizabeth Olsen",
            phone: "+1*********",
            total: "970,00 đô la",
            status: "Đã giao hàng",
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
            render: (status: string) => <Tag color="green">{status}</Tag>,
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
                    ✅ Đơn hàng đã hoàn thành{" "}
                    <Tag color="green" style={{ fontSize: 14, padding: "2px 8px" }}>
                        18
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
                            <Option value="Robert Downey Jr.">Robert Downey Jr.</Option>
                            <Option value="Tom Holland">Tom Holland</Option>
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
                        Danh Sách Đơn Hàng Đã Hoàn Thành <Tag color="green">18</Tag>
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
                        total: 18,
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

export default CompletedOrderPage;
