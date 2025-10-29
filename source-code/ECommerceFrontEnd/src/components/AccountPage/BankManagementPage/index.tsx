import React, { useState } from "react";
import { Card, Row, Col, Typography, Modal, Dropdown, message } from "antd";
import {
    PlusOutlined,
    MoreOutlined,
    ExclamationCircleOutlined,
} from "@ant-design/icons";
import BankLinkForm from "../BankLinkForm";

const { Title, Text } = Typography;
const { confirm } = Modal;

interface BankCard {
    bank: string;
    cardNumber: string;
    cardName: string;
    expiry: string;
    color: string;
}

const BankManagementPage: React.FC = () => {
    const [cards, setCards] = useState<BankCard[]>([
        {
            bank: "Vietcombank",
            cardNumber: "**** **** **** 1234",
            cardName: "NGUYEN VAN A",
            expiry: "12/26",
            color: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
        },
        {
            bank: "Techcombank",
            cardNumber: "**** **** **** 5678",
            cardName: "NGUYEN VAN A",
            expiry: "08/25",
            color: "linear-gradient(135deg, #ff758c 0%, #ff7eb3 100%)",
        },
        {
            bank: "BIDV",
            cardNumber: "**** **** **** 9012",
            cardName: "NGUYEN VAN A",
            expiry: "03/27",
            color: "linear-gradient(135deg, #43cea2 0%, #185a9d 100%)",
        },
    ]);

    const [isModalOpen, setIsModalOpen] = useState(false);

    // ➕ Thêm thẻ mới
    const handleAddCard = (newCard: BankCard) => {
        setCards((prev) => [...prev, newCard]);
        setIsModalOpen(false);
        message.success("Thêm thẻ mới thành công!");
    };

    // 🧹 Xác nhận & xóa thẻ
    const handleDelete = (index: number) => {
        confirm({
            title: "Xác nhận xóa thẻ ngân hàng?",
            icon: <ExclamationCircleOutlined />,
            content: "Bạn có chắc chắn muốn xóa thẻ này không?",
            okText: "Xóa",
            okType: "danger",
            cancelText: "Hủy",
            centered: true,
            onOk() {
                setCards((prev) => prev.filter((_, i) => i !== index));
                message.success("Đã xóa thẻ thành công!");
            },
        });
    };

    return (
        <div style={{ padding: 32 }}>
            <Title level={3}>Quản lý Thẻ Ngân Hàng</Title>

            <div style={{ marginTop: 24 }}>
                <Title level={5}>Thẻ đã liên kết</Title>

                <Row gutter={[16, 16]} style={{ marginTop: 12 }}>
                    {cards.map((card, index) => (
                        <Col xs={24} sm={12} md={8} key={index}>
                            <Card
                                style={{
                                    borderRadius: 16,
                                    background: card.color,
                                    color: "#fff",
                                }}
                                bodyStyle={{ padding: 20 }}
                            >
                                <div
                                    style={{
                                        display: "flex",
                                        justifyContent: "space-between",
                                        alignItems: "center",
                                    }}
                                >
                                    <Text style={{ color: "#fff", fontWeight: 600 }}>
                                        {card.bank}
                                    </Text>

                                    <Dropdown
                                        trigger={["click"]}
                                        destroyPopupOnHide
                                        menu={{
                                            items: [
                                                {
                                                    key: "delete",
                                                    label: (
                                                        <span style={{ color: "red" }}>Xóa ngân hàng</span>
                                                    ),
                                                },
                                            ],
                                            onClick: ({ key }) => {
                                                if (key === "delete") handleDelete(index);
                                            },
                                        }}
                                    >
                                        <MoreOutlined
                                            style={{
                                                color: "#fff",
                                                fontSize: 18,
                                                cursor: "pointer",
                                                transition: "0.2s",
                                            }}
                                        />
                                    </Dropdown>
                                </div>

                                <Title
                                    level={4}
                                    style={{
                                        color: "#fff",
                                        marginTop: 20,
                                        marginBottom: 10,
                                    }}
                                >
                                    {card.cardNumber}
                                </Title>

                                <div
                                    style={{
                                        display: "flex",
                                        justifyContent: "space-between",
                                        fontSize: 13,
                                        color: "#f5f5f5",
                                    }}
                                >
                                    <div>
                                        <Text>Chủ thẻ</Text>
                                        <br />
                                        <b>{card.cardName}</b>
                                    </div>
                                    <div>
                                        <Text>Hết hạn</Text>
                                        <br />
                                        <b>{card.expiry}</b>
                                    </div>
                                </div>
                            </Card>
                        </Col>
                    ))}

                    {/* ➕ Nút thêm thẻ */}
                    <Col xs={24} sm={12} md={8}>
                        <Card
                            style={{
                                borderRadius: 16,
                                border: "2px dashed #d9d9d9",
                                textAlign: "center",
                                height: 200,
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                flexDirection: "column",
                            }}
                            onClick={() => setIsModalOpen(true)}
                            hoverable
                        >
                            <PlusOutlined style={{ fontSize: 32, color: "#1677ff" }} />
                            <Text style={{ marginTop: 8, color: "#1677ff" }}>
                                Thêm thẻ ngân hàng mới
                            </Text>
                            <Text type="secondary" style={{ fontSize: 12 }}>
                                Liên kết thẻ tín dụng hoặc ghi nợ
                            </Text>
                        </Card>
                    </Col>
                </Row>
            </div>

            {/* 🪟 Modal thêm thẻ */}
            <Modal
                open={isModalOpen}
                footer={null}
                onCancel={() => setIsModalOpen(false)}
                centered
                destroyOnClose
            >
                <BankLinkForm
                    onSuccess={(values) =>
                        handleAddCard({
                            bank: values.bank,
                            cardNumber: "**** **** **** " + values.cardNumber.slice(-4),
                            cardName: values.cardName,
                            expiry: values.expiry,
                            color: "linear-gradient(135deg, #36d1dc 0%, #5b86e5 100%)",
                        })
                    }
                />
            </Modal>
        </div>
    );
};

export default BankManagementPage;
