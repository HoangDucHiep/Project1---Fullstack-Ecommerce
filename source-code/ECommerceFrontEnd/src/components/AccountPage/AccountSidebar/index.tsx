import React from "react";
import { Menu, Avatar, Typography } from "antd";
import {
    BellOutlined,
    UserOutlined,
    CreditCardOutlined,
    LockOutlined,
    ShoppingOutlined,
    GiftOutlined,
} from "@ant-design/icons";

import defaultAvatar from "../../../assets/img/SamSungS24 Ultra.jpg";

const { Text } = Typography;

// 🧠 Nhận props để dùng avatar và xử lý sự kiện
interface AccountSidebarProps {
    selectedKey: string;          // ✅ thêm dòng này
    avatarUrl?: string;           // ✅ thêm dòng này
    onMenuSelect?: (key: string) => void;
}

const AccountSidebar: React.FC<AccountSidebarProps> = ({
    avatarUrl,
    onMenuSelect,
}) => {
    return (
        <div
            style={{
                width: 250,
                background: "#fff",
                padding: "20px 0",
                borderRight: "1px solid #f0f0f0",
                minHeight: "100vh",
            }}
        >
            {/* Hồ sơ người dùng */}
            <div
                style={{
                    textAlign: "center",
                    marginBottom: 20,
                }}
            >
                {/* ✅ Hiển thị avatar nhận từ props, fallback về ảnh mặc định */}
                <Avatar size={64} src={avatarUrl || defaultAvatar} />

                <div style={{ marginTop: 8 }}>
                    <Text strong>doaz123tt</Text>
                </div>
                <div>
                    <a href="#" style={{ fontSize: 12 }}>
                        Sửa Hồ Sơ
                    </a>
                </div>
            </div>

            {/* Menu danh mục */}
            <Menu
                mode="inline"
                defaultSelectedKeys={["profile"]}
                onClick={({ key }) => onMenuSelect?.(key)} // ✅ Khi click menu, gửi key lên trên
                items={[
                    {
                        key: "notifications",
                        icon: <BellOutlined />,
                        label: "Thông Báo",
                    },
                    {
                        key: "account",
                        icon: <UserOutlined />,
                        label: "Tài Khoản Của Tôi",
                        children: [
                            { key: "profile", label: "Hồ Sơ" },
                            { key: "bank", label: "Ngân Hàng", icon: <CreditCardOutlined /> },
                            { key: "changePassword", label: "Đổi Mật Khẩu", icon: <LockOutlined /> },
                        ],
                    },
                    {
                        key: "orders",
                        icon: <ShoppingOutlined />,
                        label: "Đơn Mua",
                    },
                    {
                        key: "voucher",
                        icon: <GiftOutlined />,
                        label: "Kho Voucher",
                    },
                ]}
            />
        </div>
    );
};

export default AccountSidebar;
