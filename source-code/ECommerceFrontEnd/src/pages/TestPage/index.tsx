import React from "react";
import { Layout, Menu, Dropdown, Input, Badge, Button } from "antd";
import type { MenuProps } from "antd/es/menu";
import {
    DownOutlined,
    SearchOutlined,
    HeartOutlined,
    ShoppingCartOutlined,
    UserOutlined,
    AppstoreOutlined,
} from "@ant-design/icons";
import CategoryManergerComponent from "../../components/CategoryManergerComponent/CategoryManergerComponent.tsx";
import img1 from "../../assets/img/logo.png";

const { Header } = Layout;
const { Search } = Input;

const CategoryManagerComponent: React.FC = () => {
    const navMenu: MenuProps["items"] = [
        { key: "home", label: "Trang chủ" },
        { key: "brand", label: "Thương hiệu" },
        {
            key: "offers",
            label: (
                <span>
                    Ưu đãi <DownOutlined style={{ fontSize: 12 }} />
                </span>
            ),
            children: [
                { key: "offer1", label: "Khuyến mãi hot" },
                { key: "offer2", label: "Giảm giá" },
            ],
        },
        {
            key: "vendor-zone",
            label: (
                <span>
                    Kênh người bán <DownOutlined style={{ fontSize: 12 }} />
                </span>
            ),
            children: [
                { key: "v1", label: "Trở thành nhà bán" },
                { key: "v2", label: "Đăng nhập nhà bán" },
            ],
        },
    ];

    const categoryMenu: MenuProps = {
        items: [
            {
                key: "categories",
                label: (
                    <div
                        style={{
                            background: "#fff",
                            boxShadow: "0 2px 8px rgba(0,0,0,0.15)",
                            borderRadius: 8,
                            padding: 10,
                        }}
                    >
                        <CategoryManergerComponent />
                    </div>
                ),
            },
        ],
    };

    const handleClick = (label: string) => {
        console.log(`Đã nhấn vào: ${label}`);
    };

    return (
        <Layout
            style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                width: "100%",
                background: "#fff",
            }}
        >
            {/* ---------------- Header ---------------- */}
            <Header
                style={{
                    background: "#fff",
                    padding: "10px 20px",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    width: "100%",
                    borderBottom: "1px solid #f0f0f0",
                    height: 80,
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        width: "100%",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "space-between",
                        gap: 20,
                    }}
                >
                    {/* ✅ Logo ảnh tĩnh, không click, không hover */}
                    <div
                        style={{
                            width: 150,
                            height: 150,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            flexShrink: 0,
                        }}
                    >
                        <img
                            src={img1}
                            alt="Logo"
                            style={{
                                width: "100%",
                                height: "100%",
                                objectFit: "contain",
                                borderRadius: 12,
                                pointerEvents: "none", // không click
                            }}
                        />
                    </div>

                    {/* 🔍 Thanh tìm kiếm */}
                    <Search
                        placeholder="Tìm kiếm sản phẩm yêu thích..."
                        enterButton={
                            <Button
                                type="primary"
                                icon={<SearchOutlined />}
                                style={{
                                    background: "#7e22ce",
                                    borderColor: "#7e22ce",
                                    height: 40,
                                }}
                            />
                        }
                        size="large"
                        style={{
                            flex: 1,
                            minWidth: 200,
                            maxWidth: 600,
                        }}
                        onSearch={(value) => console.log(value)}
                    />

                    {/* ❤️ 👤 🛒 Icon */}
                    <div
                        style={{
                            display: "flex",
                            alignItems: "center",
                            gap: 20,
                            flexShrink: 0,
                        }}
                    >
                        <Button
                            type="text"
                            icon={<HeartOutlined style={{ fontSize: 22 }} />}
                            onClick={() => handleClick("Yêu thích")}
                            style={{
                                color: "#4B5563",
                            }}
                        />
                        <Button
                            type="text"
                            icon={<UserOutlined style={{ fontSize: 22 }} />}
                            onClick={() => handleClick("Tài khoản")}
                            style={{
                                color: "#4B5563",
                            }}
                        />
                        <Button
                            type="text"
                            onClick={() => handleClick("Giỏ hàng")}
                            style={{
                                display: "flex",
                                alignItems: "center",
                                color: "#4B5563",
                                padding: 0,
                            }}
                        >
                            <Badge count={0} size="small" offset={[-5, 5]}>
                                <ShoppingCartOutlined style={{ fontSize: 22 }} />
                            </Badge>
                            <span
                                style={{
                                    fontSize: 14,
                                    color: "#333",
                                    marginLeft: 8,
                                    fontWeight: 500,
                                }}
                            >
                                Giỏ hàng ₫0
                            </span>
                        </Button>
                    </div>
                </div>
            </Header>

            {/* ---------------- Navigation Bar ---------------- */}
            <div
                style={{
                    background: "#1890ff",
                    display: "flex",
                    justifyContent: "center",
                    width: "100%",
                }}
            >
                <div
                    style={{
                        maxWidth: 1200,
                        width: "100%",
                        display: "flex",
                        alignItems: "stretch",
                    }}
                >
                    {/* ⚡ Menu Danh mục */}
                    <Dropdown menu={categoryMenu} trigger={["click"]}>
                        <div
                            style={{
                                background: "#1565c0",
                                padding: "0 24px",
                                display: "flex",
                                alignItems: "center",
                                cursor: "pointer",
                                fontWeight: "500",
                                fontSize: 16,
                                color: "#fff",
                                borderRight: "1px solid rgba(255,255,255,0.3)",
                                transition: "transform 0.2s ease",
                            }}
                            onMouseEnter={(e) =>
                                (e.currentTarget.style.transform = "scale(1.05)")
                            }
                            onMouseLeave={(e) =>
                                (e.currentTarget.style.transform = "scale(1)")
                            }
                        >
                            <AppstoreOutlined style={{ fontSize: 18, marginRight: 8 }} />
                            Danh mục
                            <DownOutlined style={{ fontSize: 12, marginLeft: 8 }} />
                        </div>
                    </Dropdown>

                    {/* ⚡ Menu chính – bỏ hover màu, thêm scale nhẹ */}
                    <Menu
                        mode="horizontal"
                        items={navMenu}
                        style={{
                            background: "transparent",
                            borderBottom: "none",
                            flex: 1,
                            fontSize: 16,
                            justifyContent: "flex-start",
                            lineHeight: "46px",
                            color: "#fff",
                        }}
                        theme="dark"
                        selectable={false}
                    />
                </div>
            </div>

            <style>
                {`
                    .ant-menu-dark .ant-menu-item:hover,
                    .ant-menu-dark .ant-menu-submenu-title:hover {
                        background-color: transparent !important;
                        transform: scale(1.08);
                        transition: transform 0.2s ease;
                    }
                `}
            </style>
        </Layout>
    );
};

export default CategoryManagerComponent;
