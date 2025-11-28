import React, { useState, useRef } from "react";
import {
    Layout,
    Input,
    List,
    Avatar,
    Typography,
    Button,
    Upload,
    Tooltip,
    // Đã xóa Card
    Tabs,
    Badge,
} from "antd";
import {
    SearchOutlined,
    PaperClipOutlined,
    SmileOutlined,
    SendOutlined,
} from "@ant-design/icons";

const { Text, Paragraph, Title } = Typography;
const { TabPane } = Tabs;
const { Sider, Content } = Layout;

interface Message {
    id: string;
    fromMe: boolean;
    text: string;
    time?: string;
}

interface Customer {
    id: string;
    name: string;
    phone: string;
    lastSeen: string;
    avatar?: string;
    preview: string;
}

const SellerChatComponentPage: React.FC = () => {
    const [activeId, setActiveId] = useState<string>("1");
    const [query, setQuery] = useState("");
    const [input, setInput] = useState("");
    const listRef = useRef<HTMLDivElement | null>(null);

    const customers: Customer[] = [
        {
            id: "1",
            name: "Devid Jack",
            phone: "+1**********",
            lastSeen: "2 năm trước",
            preview: "Được, cảm ơn",
        },
        {
            id: "2",
            name: "Robert Downey",
            phone: "+1**********",
            lastSeen: "3 năm trước",
            avatar:
                "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=400&h=400&fit=crop",
            preview: "Bạn có thể đặt hàng sản phẩm từ trang web/ứng dụng...",
        },
    ];

    const [messages, setMessages] = useState<Record<string, Message[]>>({
        "1": [
            {
                id: "m1",
                fromMe: true,
                text: "Tôi có thể giúp gì cho bạn?",
                time: "10:12",
            },
            {
                id: "m2",
                fromMe: false,
                text: "Có rất nhiều phiên bản khác nhau của Lorem Ipsum...",
                time: "10:13",
            },
            { id: "m3", fromMe: false, text: "Được, cảm ơn", time: "10:14" },
        ],
        "2": [
            {
                id: "r1",
                fromMe: false,
                text: "Bạn có thể đặt hàng sản phẩm từ trang web/ứng dụng...",
                time: "09:05",
            },
        ],
    });

    const sendMessage = () => {
        if (!input.trim()) return;
        const newMsg: Message = {
            id: Date.now().toString(),
            fromMe: true,
            text: input,
            time: new Date().toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
            }),
        };
        setMessages((prev) => ({
            ...prev,
            [activeId]: [...(prev[activeId] || []), newMsg],
        }));
        setInput("");
        setTimeout(
            // Kéo xuống cuối danh sách tin nhắn
            () => listRef.current?.scrollTo({ top: 99999, behavior: "smooth" }),
            100
        );
    };

    const filteredCustomers = customers.filter((c) =>
        c.name.toLowerCase().includes(query.toLowerCase())
    );

    return (
        // THAY ĐỔI 1: Xóa Card bao ngoài và đặt style cho Layout là full màn hình
        <Layout style={{ height: "90vh", background: "#fff" }}>
            {/* Sidebar */}
            <Sider
                width={340}
                style={{
                    background: "#fff",
                    borderRight: "1px solid #f0f0f0",
                    padding: 16,
                }}
            >
                <Input
                    placeholder="Tìm kiếm khách hàng..."
                    prefix={<SearchOutlined />}
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    allowClear
                    style={{ marginBottom: 16 }}
                />

                <Tabs defaultActiveKey="customers" style={{ height: 'calc(100% - 48px)' }}>
                    <TabPane tab="Khách hàng" key="customers" style={{ height: '100%', overflowY: 'auto', paddingRight: 8 }}>
                        <List
                            itemLayout="horizontal"
                            dataSource={filteredCustomers}
                            renderItem={(item) => (
                                <List.Item
                                    style={{
                                        padding: "10px 12px",
                                        borderRadius: 8,
                                        cursor: "pointer",
                                        background:
                                            item.id === activeId ? "#e6f7ff" : "transparent",
                                        marginBottom: 4,
                                    }}
                                    onClick={() => setActiveId(item.id)}
                                >
                                    <List.Item.Meta
                                        avatar={
                                            <Badge dot={item.id === activeId}>
                                                <Avatar size={48} src={item.avatar}>
                                                    {item.name[0]}
                                                </Avatar>
                                            </Badge>
                                        }
                                        title={
                                            <div
                                                style={{
                                                    display: "flex",
                                                    justifyContent: "space-between",
                                                }}
                                            >
                                                <span>{item.name}</span>
                                                <Text type="secondary" style={{ fontSize: 12 }}>
                                                    {item.lastSeen}
                                                </Text>
                                            </div>
                                        }
                                        description={<Text ellipsis>{item.preview}</Text>}
                                    />
                                </List.Item>
                            )}
                        />
                    </TabPane>

                    <TabPane tab="Người giao hàng" key="riders">
                        <Text type="secondary">Danh sách người giao hàng...</Text>
                    </TabPane>
                </Tabs>
            </Sider>

            {/* Chat Content */}
            <Content
                style={{
                    padding: "0 0 12px 0",
                    display: "flex",
                    flexDirection: "column",
                }}
            >
                {/* Header */}
                <div
                    style={{
                        borderBottom: "1px solid #f0f0f0",
                        padding: "12px 16px",
                        display: "flex",
                        alignItems: "center",
                        gap: 12,
                    }}
                >
                    <Avatar
                        src={customers.find((c) => c.id === activeId)?.avatar}
                        size={40}
                    >
                        {customers.find((c) => c.id === activeId)?.name[0]}
                    </Avatar>
                    <div>
                        <Title level={5} style={{ margin: 0 }}>
                            {customers.find((c) => c.id === activeId)?.name}
                        </Title>
                        <Text type="secondary">
                            {customers.find((c) => c.id === activeId)?.phone}
                        </Text>
                    </div>
                </div>

                {/* Message Area */}
                <div
                    ref={listRef}
                    style={{
                        flex: 1,
                        overflowY: "auto",
                        padding: "20px",
                        background: "#fafafa",
                    }}
                >
                    {(messages[activeId] || []).map((m) => (
                        <div
                            key={m.id}
                            style={{
                                display: "flex",
                                justifyContent: m.fromMe ? "flex-end" : "flex-start",
                                marginBottom: 12,
                            }}
                        >
                            {!m.fromMe && (
                                <Avatar
                                    size={28}
                                    style={{ marginRight: 8 }}
                                    src={customers.find((c) => c.id === activeId)?.avatar}
                                >
                                    {customers.find((c) => c.id === activeId)?.name[0]}
                                </Avatar>
                            )}
                            <div style={{ maxWidth: "70%" }}>
                                <div
                                    style={{
                                        background: m.fromMe ? "#e6f7ff" : "#fff",
                                        padding: "10px 14px",
                                        borderRadius: 8,
                                        boxShadow: "0 1px 1px rgba(0,0,0,0.05)",
                                    }}
                                >
                                    <Paragraph style={{ margin: 0 }}>{m.text}</Paragraph>
                                </div>
                                <div
                                    style={{
                                        marginTop: 4,
                                        textAlign: m.fromMe ? "right" : "left",
                                    }}
                                >
                                    <Text type="secondary" style={{ fontSize: 12 }}>
                                        {m.time}
                                    </Text>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                {/* Input Composer */}
                <div
                    style={{
                        borderTop: "1px solid #f0f0f0",
                        background: "#fff",
                        padding: "10px 16px",
                    }}
                >
                    <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                        <Upload showUploadList={false} beforeUpload={() => false}>
                            <Tooltip title="Gửi file">
                                <Button icon={<PaperClipOutlined />} />
                            </Tooltip>
                        </Upload>

                        <Tooltip title="Emoji">
                            <Button icon={<SmileOutlined />} />
                        </Tooltip>

                        <Input.TextArea
                            value={input}
                            onChange={(e) => setInput(e.target.value)}
                            onPressEnter={(e) => {
                                e.preventDefault();
                                sendMessage();
                            }}
                            placeholder="Gửi tin nhắn..."
                            autoSize={{ minRows: 1, maxRows: 4 }}
                            style={{ flex: 1 }}
                        />
                        <Button
                            type="primary"
                            icon={<SendOutlined />}
                            onClick={sendMessage}
                        >
                            Gửi
                        </Button>
                    </div>
                </div>
            </Content>
        </Layout>
    );
};

export default SellerChatComponentPage;