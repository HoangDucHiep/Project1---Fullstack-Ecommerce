import React, { useState } from "react";
import { Modal, Input, Button, List, Avatar, Typography, Space } from "antd";
import { SendOutlined, UserOutlined, ShopOutlined } from "@ant-design/icons";

const { Text } = Typography;

interface Message {
    id: number;
    from: "user" | "shop";
    content: string;
}

interface ChatBoxProps {
    open: boolean;
    onClose: () => void;
}

const ChatBox: React.FC<ChatBoxProps> = ({ open, onClose }) => {
    const [messages, setMessages] = useState<Message[]>([
        {
            id: 1,
            from: "shop",
            content:
                "Welcome to our shop! Check out our products, and feel free to ask any questions.",
        },
        { id: 2, from: "shop", content: "Bạn có thể muốn hỏi:" },
        { id: 3, from: "shop", content: "• Sản phẩm này có sẵn không?" },
        { id: 4, from: "shop", content: "• Có thể thanh toán bằng COD được không?" },
        { id: 5, from: "shop", content: "• Tôi có thể được giảm giá không?" },
    ]);

    const [input, setInput] = useState("");

    const handleSend = () => {
        if (!input.trim()) return;
        const newMessage: Message = {
            id: Date.now(),
            from: "user",
            content: input,
        };
        setMessages([...messages, newMessage]);
        setInput("");
    };

    return (
        <Modal
            open={open}
            title="Chat với người bán"
            footer={null}
            onCancel={onClose}
            centered
            width={420}
        >
            <div
                style={{
                    height: 400,
                    overflowY: "auto",
                    border: "1px solid #f0f0f0",
                    borderRadius: 8,
                    padding: 10,
                    marginBottom: 10,
                }}
            >
                <List
                    dataSource={messages}
                    renderItem={(msg) => (
                        <List.Item
                            style={{
                                justifyContent:
                                    msg.from === "user" ? "flex-end" : "flex-start",
                            }}
                        >
                            <Space align="start">
                                {msg.from === "shop" && (
                                    <Avatar icon={<ShopOutlined />} size="small" />
                                )}
                                <div
                                    style={{
                                        background:
                                            msg.from === "user" ? "#1890ff" : "#f5f5f5",
                                        color: msg.from === "user" ? "white" : "black",
                                        padding: "8px 12px",
                                        borderRadius: 12,
                                        maxWidth: 250,
                                        wordWrap: "break-word",
                                    }}
                                >
                                    <Text>{msg.content}</Text>
                                </div>
                                {msg.from === "user" && (
                                    <Avatar icon={<UserOutlined />} size="small" />
                                )}
                            </Space>
                        </List.Item>
                    )}
                />
            </div>

            <Space.Compact style={{ width: "100%" }}>
                <Input
                    placeholder="Nhập tin nhắn..."
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onPressEnter={handleSend}
                />
                <Button type="primary" icon={<SendOutlined />} onClick={handleSend}>
                    Gửi
                </Button>
            </Space.Compact>
        </Modal>
    );
};

export default ChatBox;
