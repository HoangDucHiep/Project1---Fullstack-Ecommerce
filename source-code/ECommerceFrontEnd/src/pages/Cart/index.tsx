import React, { useState } from "react";
import { Button, Card } from "antd";
import ChatBox from "../../components/BuyPage/ChatBox";

const BuyPage: React.FC = () => {
    const [chatOpen, setChatOpen] = useState(false);

    return (
        <div style={{ padding: 20 }}>
            <Card title="Sản phẩm: Tai nghe Bluetooth" style={{ width: 400 }}>
                <p>Giá: 299.000đ</p>
                <p>Kho: Còn hàng</p>
                <Button type="primary" onClick={() => setChatOpen(true)}>
                    💬 Chat ngay
                </Button>
            </Card>

            <ChatBox open={chatOpen} onClose={() => setChatOpen(false)} />
        </div>
    );
};

export default BuyPage;