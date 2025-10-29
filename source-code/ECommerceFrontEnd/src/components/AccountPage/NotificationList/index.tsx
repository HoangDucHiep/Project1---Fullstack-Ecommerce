import React, { useState } from "react";
import {
    Card,
    Tag,
    Button,
    Typography,
    Space,
    Row,
    Col,
    message,
} from "antd";
import {
    DeleteOutlined,
    ExclamationCircleOutlined,
    GiftOutlined,
    ShoppingOutlined,
    SettingOutlined,
    InfoCircleOutlined,
} from "@ant-design/icons";

const { Text, Title } = Typography;

type NotificationType = "Hệ thống" | "Khuyến mãi" | "Đơn hàng" | "Chính sách";
type Priority = "Cao" | "Trung bình" | "Thấp";

interface Notification {
    id: number;
    title: string;
    content: string;
    type: NotificationType;
    priority: Priority;
    time: string;
}

const NotificationList: React.FC = () => {
    const [activeFilter, setActiveFilter] = useState<NotificationType | "Tất cả">("Tất cả");
    const [notifications, setNotifications] = useState<Notification[]>([
        {
            id: 1,
            title: "Flash Sale Cuối Tuần - Giảm đến 70%",
            content:
                "Chương trình Flash Sale đặc biệt với hàng ngàn sản phẩm giảm giá sốc. Thời gian có hạn từ 00:00 - 23:59 ngày 15/12. Nhanh tay đặt hàng để không bỏ lỡ cơ hội!",
            type: "Khuyến mãi",
            priority: "Cao",
            time: "2 phút trước",
        },
        {
            id: 2,
            title: "⚠️ Bảo trì hệ thống định kỳ",
            content:
                "Hệ thống sẽ được bảo trì từ 02:00 - 04:00 ngày 16/12/2024. Một số tính năng có thể bị gián đoạn. Xin lỗi vì sự bất tiện này.",
            type: "Hệ thống",
            priority: "Trung bình",
            time: "1 giờ trước",
        },
        {
            id: 3,
            title: "Đơn hàng #DH123456 đã được giao thành công",
            content:
                "Đơn hàng của bạn đã được giao thành công đến địa chỉ: 123 Nguyễn Văn A, Quận 1, TP.HCM. Cảm ơn bạn đã mua sắm tại cửa hàng!",
            type: "Đơn hàng",
            priority: "Thấp",
            time: "3 giờ trước",
        },
    ]);

    const handleDelete = (id: number) => {
        setNotifications((prev) => prev.filter((n) => n.id !== id));
        message.success("Đã xóa thông báo!");
    };

    const handleDeleteAll = () => {
        setNotifications([]);
        message.success("Đã xóa tất cả thông báo!");
    };

    const filteredNotifications =
        activeFilter === "Tất cả"
            ? notifications
            : notifications.filter((n) => n.type === activeFilter);

    const typeColor: Record<NotificationType, string> = {
        "Hệ thống": "#faad14",
        "Khuyến mãi": "#ff4d4f",
        "Đơn hàng": "#52c41a",
        "Chính sách": "#1890ff",
    };

    const priorityColor: Record<Priority, string> = {
        "Cao": "error",
        "Trung bình": "warning",
        "Thấp": "success",
    };

    const typeIcon: Record<NotificationType, React.ReactNode> = {
        "Hệ thống": <SettingOutlined />,
        "Khuyến mãi": <GiftOutlined />,
        "Đơn hàng": <ShoppingOutlined />,
        "Chính sách": <InfoCircleOutlined />,
    };

    return (
        <div style={{ padding: "24px", maxWidth: 900, margin: "0 auto" }}>
            {/* Bộ lọc */}
            <Space
                wrap
                size="middle"
                style={{
                    marginBottom: 24,
                    justifyContent: "space-between",
                    width: "100%",
                }}
            >
                <Space>
                    {["Tất cả", "Hệ thống", "Khuyến mãi", "Đơn hàng", "Chính sách"].map(
                        (type) => (
                            <Button
                                key={type}
                                type={activeFilter === type ? "primary" : "default"}
                                onClick={() =>
                                    setActiveFilter(type as NotificationType | "Tất cả")
                                }
                            >
                                {type}
                            </Button>
                        )
                    )}
                </Space>
                <Button danger icon={<DeleteOutlined />} onClick={handleDeleteAll}>
                    Xóa tất cả
                </Button>
            </Space>

            {/* Danh sách thông báo */}
            <Space direction="vertical" style={{ width: "100%" }} size="large">
                {filteredNotifications.length > 0 ? (
                    filteredNotifications.map((n) => (
                        <Card
                            key={n.id}
                            bordered={false}
                            style={{
                                borderLeft: `5px solid ${typeColor[n.type]}`,
                                boxShadow: "0 2px 10px rgba(0,0,0,0.05)",
                                borderRadius: 10,
                            }}
                            bodyStyle={{ padding: 20 }}
                        >
                            <Row justify="space-between" align="middle">
                                <Col flex="auto">
                                    <Space align="start">
                                        <div
                                            style={{
                                                width: 40,
                                                height: 40,
                                                borderRadius: "50%",
                                                background: "linear-gradient(45deg,#6a11cb,#2575fc)",
                                                display: "flex",
                                                alignItems: "center",
                                                justifyContent: "center",
                                                color: "#fff",
                                                fontSize: 18,
                                            }}
                                        >
                                            {typeIcon[n.type]}
                                        </div>
                                        <div>
                                            <Title level={5} style={{ margin: 0 }}>
                                                {n.title}
                                            </Title>
                                            <Text>{n.content}</Text>
                                            <div style={{ marginTop: 8 }}>
                                                <Tag color={typeColor[n.type]}>{n.type}</Tag>
                                                <Tag color={priorityColor[n.priority]}>
                                                    {n.priority}
                                                </Tag>
                                            </div>
                                        </div>
                                    </Space>
                                </Col>
                                <Col>
                                    <Space direction="vertical" align="end">
                                        <Text type="secondary" style={{ fontSize: 12 }}>
                                            {n.time}
                                        </Text>
                                        <Button
                                            type="text"
                                            danger
                                            icon={<DeleteOutlined />}
                                            onClick={() => handleDelete(n.id)}
                                        />
                                    </Space>
                                </Col>
                            </Row>
                        </Card>
                    ))
                ) : (
                    <Card
                        style={{
                            textAlign: "center",
                            borderRadius: 10,
                            color: "#999",
                        }}
                    >
                        <ExclamationCircleOutlined style={{ fontSize: 24, marginBottom: 8 }} />
                        <div>Không có thông báo nào.</div>
                    </Card>
                )}
            </Space>
        </div>
    );
};

export default NotificationList;
