import React, { useState } from "react";
import { Layout } from "antd";
import Header from "../../layouts/Header";
import Footer from "../../layouts/Footer";
import { Content } from "antd/es/layout/layout";
import AccountSidebar from "../../components/AccountPage/AccountSidebar";
import NotificationList from "../../components/AccountPage/NotificationList";
import ProfileForm from "../../components/AccountPage/ProfileForm";
import OrderList from "../../components/AccountPage/OrderList";
import VoucherList from "../../components/AccountPage/VoucherList"; // ✅ sửa đường dẫn
import ChangePasswordForm from "../../components/AccountPage/ChangePasswordForm";
import BankManagementPage from "../../components/AccountPage/BankManagementPage";
const AccountPage: React.FC = () => {
    const [selectedKey, setSelectedKey] = useState("profile");
    const [avatarUrl, setAvatarUrl] = useState<string>("");

    const handleMenuSelect = (key: string) => setSelectedKey(key);
    const handleAvatarChange = (newUrl: string) => setAvatarUrl(newUrl);

    const renderContent = () => {
        switch (selectedKey) {
            case "notifications":
                return <NotificationList />;
            case "profile":
                return <ProfileForm onAvatarChange={handleAvatarChange} />;
            case "bank":
                return <BankManagementPage />;
            case "changePassword":
                return <ChangePasswordForm />;
            case "orders":
                return <OrderList />;
            case "voucher":
                return <VoucherList />;
            default:
                return <div>Chọn danh mục ở sidebar</div>;
        }
    };

    return (
        <Layout style={{ minHeight: "100vh", background: "#f5f5f5" }}>
            <Header />
            <Content style={{ padding: "24px 100px" }}>
                <div
                    style={{
                        display: "flex",
                        background: "#fff",
                        borderRadius: 8,
                        minHeight: "80vh",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                    }}
                >
                    <AccountSidebar
                        selectedKey={selectedKey}
                        onMenuSelect={handleMenuSelect}
                        avatarUrl={avatarUrl}
                    />
                    <div style={{ flex: 1, padding: 24 }}>{renderContent()}</div>
                </div>
            </Content>
            <Footer />
        </Layout>
    );
};

export default AccountPage;
