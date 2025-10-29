import React from "react";
import { Menu } from "antd";
import {
    MobileOutlined,
    ShoppingOutlined,
    GiftOutlined,
    LaptopOutlined,
    CoffeeOutlined,
    SkinOutlined,
    ClockCircleOutlined,
    HeartOutlined,
    SoundOutlined,
    RocketOutlined,
    UsbOutlined,
    CameraOutlined,
} from "@ant-design/icons";

const { SubMenu } = Menu;

const CategoryMenu: React.FC = () => {
    return (
        <div
            style={{
                width: 260,
                borderRadius: 12,
                overflow: "hidden",
                boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                backgroundColor: "#fff",
            }}
        >
            <Menu
                mode="vertical"
                style={{
                    width: "100%",
                    borderInlineEnd: "none",
                }}
            >
                <SubMenu
                    key="dien-thoai"
                    icon={<MobileOutlined />}
                    title="Điện thoại & Phụ kiện"
                    popupOffset={[10, 0]}
                    popupClassName="custom-submenu"
                >
                    <Menu.Item key="the-sim" icon={<UsbOutlined />}>
                        Thẻ sim
                    </Menu.Item>
                    <Menu.Item key="may-tinh-bang" icon={<LaptopOutlined />}>
                        Máy tính bảng
                    </Menu.Item>
                    <Menu.Item key="dien-thoai" icon={<MobileOutlined />}>
                        Điện thoại
                    </Menu.Item>

                    <SubMenu
                        key="phu-kien"
                        title="Phụ kiện"
                        icon={<ShoppingOutlined />}
                        popupOffset={[10, 0]}
                    >
                        <Menu.Item key="bo-dam" icon={<SoundOutlined />}>
                            Bộ đàm
                        </Menu.Item>
                        <Menu.Item key="khac-phukien" icon={<GiftOutlined />}>
                            Khác
                        </Menu.Item>

                        <SubMenu
                            key="phu-kien-selfie"
                            title="Phụ kiện Selfie"
                            icon={<CameraOutlined />}
                            popupOffset={[10, 0]}
                        >
                            <Menu.Item key="gay-selfie">Gậy selfie</Menu.Item>
                            <Menu.Item key="gia-do">Giá đỡ</Menu.Item>
                            <Menu.Item key="den-flash">Đèn flash</Menu.Item>
                            <Menu.Item key="but-cam-ung">Bút cảm ứng</Menu.Item>
                            <Menu.Item key="day-deo">Dây đeo & móc khóa</Menu.Item>
                        </SubMenu>
                    </SubMenu>
                </SubMenu>

                <SubMenu
                    key="thoi-trang-nam"
                    icon={<SkinOutlined />}
                    title="Thời trang nam"
                    popupOffset={[10, 0]}
                >
                    <Menu.Item key="giay-dep-nam">Giày dép nam</Menu.Item>
                    <Menu.Item key="ao-nam">Áo nam</Menu.Item>
                    <Menu.Item key="quan-nam">Quần nam</Menu.Item>
                </SubMenu>

                <SubMenu
                    key="thoi-trang-nu"
                    icon={<HeartOutlined />}
                    title="Thời trang nữ"
                    popupOffset={[10, 0]}
                >
                    <Menu.Item key="tui-vi-nu">Túi & ví nữ</Menu.Item>
                    <Menu.Item key="giay-dep-nu">Giày dép nữ</Menu.Item>
                </SubMenu>

                <SubMenu
                    key="dong-ho"
                    icon={<ClockCircleOutlined />}
                    title="Đồng hồ"
                    popupOffset={[10, 0]}
                >
                    <Menu.Item key="dong-ho-nam">Đồng hồ nam</Menu.Item>
                    <Menu.Item key="dong-ho-nu">Đồng hồ nữ</Menu.Item>
                </SubMenu>

                <SubMenu
                    key="du-lich"
                    icon={<RocketOutlined />}
                    title="Du lịch & Hành lý"
                    popupOffset={[10, 0]}
                />

                <SubMenu
                    key="do-uong"
                    icon={<CoffeeOutlined />}
                    title="Thực phẩm & Đồ uống"
                    popupOffset={[10, 0]}
                />
            </Menu>

            <style>
                {`
                    /* ✅ CSS tùy chỉnh submenu bung ngang */
                    .ant-menu-submenu-popup {
                        margin-left: 4px;
                    }

                    .ant-menu-submenu-popup .ant-menu {
                        border-radius: 8px;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                        padding: 4px 0;
                    }

                    .ant-menu-item:hover {
                        background-color: #f5f5f5 !important;
                    }

                    .ant-menu-submenu-title:hover {
                        background-color: #f0f0f0 !important;
                        color: #1677ff !important;
                    }

                    .ant-menu-submenu-arrow {
                        color: #aaa;
                    }

                    .ant-menu-submenu-selected > .ant-menu-submenu-title {
                        color: #1677ff;
                    }
                `}
            </style>
        </div>
    );
};

export default CategoryMenu;
