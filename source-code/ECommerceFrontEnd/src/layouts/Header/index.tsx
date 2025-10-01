import React from "react";
import { Layout, Menu, Dropdown, Input, Badge } from "antd";
import {
    DownOutlined,
    SearchOutlined,
    HeartOutlined,
    ShoppingCartOutlined,
    UserOutlined,
    AppstoreOutlined,
} from "@ant-design/icons";
import Logo from "../../assets/logohcm.png"; // Thay bằng logo 6Valley

const { Header } = Layout;

const CategoryManergerComponent: React.FC = () => {
    const currencyMenu = (
        <Menu
            items={[
                { key: "usd", label: "USD $" },
                { key: "eur", label: "EUR €" },
            ]}
        />
    );

    const languageMenu = (
        <Menu
            items={[
                { key: "en", label: "English" },
                { key: "vi", label: "Tiếng Việt" },
            ]}
        />
    );

    const navMenu = [
        { key: "home", label: "Home" },
        { key: "brand", label: "Brand" },
        {
            key: "offers",
            label: (
                <span>
          Offers <DownOutlined style={{ fontSize: 12 }} />
        </span>
            ),
            children: [
                { key: "offer1", label: "Hot Deals" },
                { key: "offer2", label: "Discounts" },
            ],
        },
        { key: "pub", label: "Publication House" },
        { key: "vendors", label: "All Vendors" },
        {
            key: "vendor-zone",
            label: (
                <span>
          Vendor Zone <DownOutlined style={{ fontSize: 12 }} />
        </span>
            ),
            children: [
                { key: "v1", label: "Become a Vendor" },
                { key: "v2", label: "Vendor Login" },
            ],
        },
    ];

    return (
        <Layout style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', width: '100%', background: '#e9f0f4' }}>
            {/* Top Bar */}
            <div
                style={{
                    background: "#e9f0f4",
                    padding: "8px 20px",
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    fontSize: 16,
                    width: '100%',
                }}
            >
                <div style={{ maxWidth: 1200, width: '100%',marginLeft:170}}>📞 +00xxxxxxxxxxxx</div>
                <div style={{ maxWidth: 1200, width: '100%', display: "flex", gap: 10, alignItems: "center", justifyContent: "center" }}>
                    <Dropdown overlay={currencyMenu} placement="bottomRight">
                        <a style={{ fontSize: 16 }}>
                            USD $ <DownOutlined />
                        </a>
                    </Dropdown>
                    <Dropdown overlay={languageMenu} placement="bottomRight">
                        <a style={{ fontSize: 16 }}>
                            English <DownOutlined />
                        </a>
                    </Dropdown>
                </div>
            </div>

            {/* Main Header */}
            <Header
                style={{
                    background: "#fff",
                    padding: "10px 20px",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    width: '100%',
                }}
            >
                <div style={{ maxWidth: 1200, width: '100%', display: "flex", alignItems: "center", justifyContent: "center", gap: 20 }}>
                    <img src={Logo} alt="6Valley logo" style={{ height: 50,marginRight:70 }} />
                    <Input
                        placeholder="Tìm kiếm sản phẩm..."
                        style={{ width: 700, height: 40, fontSize: 16, textAlign: "center" }}
                        suffix={<SearchOutlined style={{ fontSize: 18 }} />}
                    />
                    <div style={{ display: "flex", alignItems: "center", gap: 15, marginLeft:40 }}>
                        <Badge count={0} size="small">
                            <HeartOutlined style={{ fontSize: 30 }} />
                        </Badge>
                        <UserOutlined style={{ fontSize: 30 }} />
                        <Badge count={0} size="small">
                            <ShoppingCartOutlined style={{ fontSize: 30 }} />
                        </Badge>
                        <div style={{ fontWeight: "bold", fontSize: 16 }}>
                            My cart $0.00
                        </div>
                    </div>
                </div>
            </Header>

            {/* Navigation Menu */}
            <div
                style={{
                    background: "#0054a6",
                    display: "flex",
                    justifyContent: "center",
                    width: '100%',
                    height:'150%',
                }}
            >
                <Menu
                    mode="horizontal"
                    theme="dark"
                    items={[
                        {
                            key: "categories",
                            icon: <AppstoreOutlined style={{ fontSize:18 }} />,
                            label: (
                                <span>
                  Categories <DownOutlined style={{ fontSize: 12 }} />
                </span>
                            ),
                        },
                        ...navMenu,
                    ]}
                    style={{
                        background: "#0054a6",
                        borderBottom: "none",
                        width: '100%',
                        maxWidth: 1200,
                        fontSize: 18,
                        justifyContent: "space-around",
                    }}
                />
            </div>
        </Layout>
    );
};

export default CategoryManergerComponent;