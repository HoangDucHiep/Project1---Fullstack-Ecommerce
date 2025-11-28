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
    reason: string;
    status: string;
}

const RefundRequestPendingPage: React.FC = () => {
    const [orders] = useState<Order[]>([
        {
            key: "1",
            id: "HT-3021",
            date: "03 tháng 11 năm 2025, 09:15 sáng",
            customer: "Tom Hiddleston",
            phone: "+1*********",
            total: "950,00 đô la",
            reason: "Sản phẩm lỗi khi nhận",
            status: "Chờ xử lý hoàn tiền",
        },
        {
            key: "2",
            id: "HT-3018",
            date: "02 tháng 11 năm 2025, 19:20 tối",
            customer: "Zendaya",
            phone: "+1*********",
            total: "1.150,00 đô la",
            reason: "Khách yêu cầu đổi trả",
            status: "Chờ xử lý hoàn tiền",
        },
        {
            key: "3",
            id: "HT-3014",
            date: "01 tháng 11 năm 2025, 11:00 sáng",
            customer: "Chris Pratt",
            phone: "+1*********",
            total: "870,00 đô la",
            reason: "Không đúng mô tả",
            status: "Chờ xử lý hoàn tiền",
        },
        {
            key: "4",
            id: "HT-3010",
            date: "31 tháng 10 năm 2025, 22:40 tối",
            customer: "Brie Larson",
            phone: "+1*********",
            total: "1.300,00 đô la",
            reason: "Đã hủy đơn trước khi giao",
            status: "Chờ xử lý hoàn tiền",
        },
    ]);

    const columns: ColumnsType<Order> = [
        { title: "SL", dataIndex: "key", width: 60 },
        { title: "Mã Đơn Hoàn Tiền", dataIndex: "id", width: 150 },
        { title: "Ngày Yêu Cầu", dataIndex: "date", width: 250 },
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
        { title: "Lý Do Hoàn Tiền", dataIndex: "reason", width: 250 },
        {
            title: "Trạng Thái",
            dataIndex: "status",
            render: (status: string) => <Tag color="orange">{status}</Tag>,
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
                    ⚠️ Đơn hàng chưa giải quyết (Hoàn tiền){" "}
                    <Tag color="orange" style={{ fontSize: 14, padding: "2px 8px" }}>
                        9
                    </Tag>
                </Title>
            </div>

            {/* --- Filter Section --- */}
            <Card className="hover-card">
                <Title level={5}>Bộ Lọc</Title>
                <Row gutter={[16, 16]}>
                    <Col xs={24} sm={12} md={8} lg={6}>
                        <Text>Nhập Mã Hoàn Tiền</Text>
                        <Input placeholder="Nhập mã hoàn tiền..." allowClear />
                    </Col>
                    <Col xs={24} sm={12} md={8} lg={6}>
                        <Text>Khách Hàng</Text>
                        <Select defaultValue="Tất cả khách hàng" style={{ width: "100%" }}>
                            <Option value="Tất cả khách hàng">Tất cả khách hàng</Option>
                            <Option value="Tom Hiddleston">Tom Hiddleston</Option>
                            <Option value="Zendaya">Zendaya</Option>
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
                        Danh Sách Yêu Cầu Hoàn Tiền Chưa Giải Quyết <Tag color="orange">9</Tag>
                    </Title>
                    <Space>
                        <Input
                            prefix={<SearchOutlined />}
                            placeholder="Tìm kiếm yêu cầu"
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
                        total: 9,
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
                    box-shadow: 0 4px 16px rgba(250, 173, 20, 0.25);
                    transform: translateY(-2px);
                }
                .table-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 15px;
                }
                .export-btn {
                    background-color: #fffbe6 !important;
                    color: #faad14 !important;
                    border-color: #ffe58f !important;
                    transition: all 0.3s ease;
                }
                .export-btn:hover {
                    background-color: #fff1b8 !important;
                    color: #d48806 !important;
                }
            `}</style>
        </div>
    );
};

export default RefundRequestPendingPage;
