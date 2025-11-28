import React, { useState } from "react";
import {
    Table,
    Button,
    Space,
    Input,
    Switch,
    Tooltip,
    Typography,
    Card,
    Modal,
    Descriptions,
} from "antd";
import {
    EyeOutlined,
    EditOutlined,
    DeleteOutlined,
    SearchOutlined,
    FileExcelOutlined,
} from "@ant-design/icons";

const { Title, Text } = Typography;

interface Discount {
    key: number;
    title: string;
    code: string;
    type: string;
    timeRange: string;
    user: string;
    active: boolean;
}

const SellerDiscountList: React.FC = () => {
    const [searchText, setSearchText] = useState("");
    const [data, setData] = useState<Discount[]>([
        {
            key: 1,
            title: "Giảm giá 75% khi mua",
            code: "rFhfx7XiCm",
            type: "Giảm Giá Khi Mua Hàng",
            timeRange: "10 tháng 1, 23 – 31 tháng 1, 29",
            user: "Người bán",
            active: true,
        },
        {
            key: 2,
            title: "Giao hàng miễn phí",
            code: "l2oDTjKF3z",
            type: "Giao Hàng Miễn Phí",
            timeRange: "10 tháng 1, 23 – 31 tháng 1, 29",

            user: "Người bán",
            active: true,
        },
        {
            key: 3,
            title: "Giảm giá khi mua hàng",
            code: "856gmef66p",
            type: "Giảm Giá Khi Mua Hàng",
            timeRange: "10 tháng 1, 23 – 10 tháng 6, 26",

            user: "Người bán",
            active: false,
        },
        {
            key: 4,
            title: "Giảm 50 đô la khi mua",
            code: "2ul59rwkw2",
            type: "Giảm Giá Khi Mua Hàng",
            timeRange: "10 tháng 1, 23 – 31 tháng 12, 26",

            user: "Người bán",
            active: true,
        },
        {
            key: 5,
            title: "Giao hàng miễn phí",
            code: "pcuw655ytg",
            type: "Giao Hàng Miễn Phí",
            timeRange: "10 tháng 1, 24 – 31 tháng 12, 27",

            user: "Người bán",
            active: true,
        },
    ]);

    const [openModal, setOpenModal] = useState(false);
    const [selectedDiscount, setSelectedDiscount] = useState<Discount | null>(
        null
    );

    const columns = [
        {
            title: "SL",
            dataIndex: "key",
            width: 60,
        },
        {
            title: "Phiếu Giảm Giá",
            dataIndex: "title",
            render: (text: string, record: Discount) => (
                <div>
                    <Text strong>{text}</Text>
                    <br />
                    <Text type="secondary">Mã: {record.code}</Text>
                </div>
            ),
        },
        {
            title: "Loại Phiếu Giảm Giá",
            dataIndex: "type",
        },
        {
            title: "Khoảng Thời Gian",
            dataIndex: "timeRange",
        },

        {
            title: "Người Tạo Phiếu Giảm Giá",
            dataIndex: "user",
        },
        {
            title: "Trạng Thái",
            dataIndex: "active",
            render: (active: boolean, record: Discount) => (
                <Switch
                    checked={active}
                    onChange={(checked) => {
                        const newData = data.map((item) =>
                            item.key === record.key ? { ...item, active: checked } : item
                        );
                        setData(newData);
                    }}
                />
            ),
        },
        {
            title: "Hoạt Động",
            key: "action",
            render: (_: any, record: Discount) => (
                <Space>
                    <Tooltip title="Xem chi tiết">
                        <Button
                            icon={<EyeOutlined />}
                            shape="circle"
                            onClick={() => {
                                setSelectedDiscount(record);
                                setOpenModal(true);
                            }}
                        />
                    </Tooltip>
                    <Tooltip title="Chỉnh sửa">
                        <Button icon={<EditOutlined />} type="primary" shape="circle" />
                    </Tooltip>
                    <Tooltip title="Xóa">
                        <Button
                            icon={<DeleteOutlined />}
                            type="primary"
                            danger
                            shape="circle"
                        />
                    </Tooltip>
                </Space>
            ),
        },
    ];

    const filteredData = data.filter((item) =>
        item.title.toLowerCase().includes(searchText.toLowerCase())
    );

    return (
        <>
            <Card
                style={{ borderRadius: 12, boxShadow: "0 2px 8px rgba(0,0,0,0.05)" }}
                bodyStyle={{ padding: "24px" }}
            >
                <div
                    style={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                        marginBottom: 16,
                    }}
                >
                    <Title level={5}>
                        Danh Sách Phiếu Giảm Giá{" "}
                        <Text type="secondary">({data.length})</Text>
                    </Title>

                    <Space>
                        <Input
                            placeholder="Tìm kiếm theo Tiêu đề"
                            prefix={<SearchOutlined />}
                            value={searchText}
                            onChange={(e) => setSearchText(e.target.value)}
                            style={{ width: 260 }}
                        />
                        <Button type="primary">Tìm kiếm</Button>
                        <Button
                            icon={<FileExcelOutlined />}
                            style={{ background: "#f6ffed", color: "#389e0d" }}
                        >
                            Xuất khẩu
                        </Button>
                    </Space>
                </div>

                <Table
                    columns={columns}
                    dataSource={filteredData}
                    pagination={{ pageSize: 5 }}
                    bordered
                    rowKey="key"
                />
            </Card>

            {/* Modal hiển thị chi tiết phiếu giảm giá */}
            <Modal
                title="Chi Tiết Phiếu Giảm Giá"
                open={openModal}
                footer={[
                    <Button key="close" onClick={() => setOpenModal(false)}>
                        Đóng
                    </Button>,
                ]}
                onCancel={() => setOpenModal(false)}
                centered
            >
                {selectedDiscount ? (
                    <Descriptions
                        bordered
                        column={1}
                        size="middle"
                        labelStyle={{ width: 160, fontWeight: 500 }}
                    >
                        <Descriptions.Item label="Tên phiếu giảm giá">
                            {selectedDiscount.title}
                        </Descriptions.Item>
                        <Descriptions.Item label="Mã giảm giá">
                            {selectedDiscount.code}
                        </Descriptions.Item>
                        <Descriptions.Item label="Loại phiếu giảm giá">
                            {selectedDiscount.type}
                        </Descriptions.Item>
                        <Descriptions.Item label="Khoảng thời gian">
                            {selectedDiscount.timeRange}


                        </Descriptions.Item>
                        <Descriptions.Item label="Người tạo mang phiếu">
                            {selectedDiscount.user}
                        </Descriptions.Item>
                        <Descriptions.Item label="Trạng thái">
                            {selectedDiscount.active ? "Đang hoạt động" : "Ngừng hoạt động"}
                        </Descriptions.Item>
                    </Descriptions>
                ) : (
                    <p>Không có dữ liệu để hiển thị</p>
                )}
            </Modal>
        </>
    );
};

export default SellerDiscountList;
