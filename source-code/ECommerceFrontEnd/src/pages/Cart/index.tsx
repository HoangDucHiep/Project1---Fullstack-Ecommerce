import { Layout } from "antd";
import React, { useState } from "react";

import RefundedOrderPage from "../../components/seller/refund/RefundedOrderPage";
import RefundRequestPendingPage from "../../components/seller/refund/RefundRequestPendingPage";
import SellerDiscountVoucherPage from "../SellerDiscountVoucherPage";
import SidebarMenu from "../../components/seller/SidebarMenu";
import CanceledOrderPage from "../../components/seller/ListOrderSeller/CanceledOrderPage";
import CompletedOrderPage from "../../components/seller/ListOrderSeller/CompletedOrderPage";
import ConfirmedOrderPage from "../../components/seller/ListOrderSeller/ConfirmedOrderPage";
import OrderManagerPage from "../../components/seller/ListOrderSeller/OrderManagerPage";
import ShippingOrderPage from "../../components/seller/ListOrderSeller/ShippingOrderPage";
import UnresolvedOrderPage from "../../components/seller/ListOrderSeller/UnresolvedOrderPage";
import UnsuccessfulOrderPage from "../../components/seller/ListOrderSeller/UnsuccessfulOrderPage";
import SellerProductPage from "../SellerProductPage";
import SellerUnapprovedProductPage from "../SellerUnapprovedProductPage";
import SellerCanceledProductPage from "../SellerCanceledProductPage";
import SellerPendingProductPage from "../SellerPendingProductPage";
import SellerChatComponentPage from "../SellerChatComponentPage";
import SellerOrderReportPage from "../SellerOrderReportPage";
const { Content } = Layout;

const SellerPage: React.FC = () => {
    const [collapsed, setCollapsed] = useState(false);
    const [selectedKey, setSelectedKey] = useState("allOrders"); // ✅ trùng key menu
   
    const renderContent = () => {
        switch (selectedKey) {
            case "allOrders":
                return <OrderManagerPage />; // Tất cả đơn hàng
            case "unconfirmed":
                return <UnresolvedOrderPage />; // Chưa xác nhận
            case "confirmed":
                return <ConfirmedOrderPage />; // Đã xác nhận
            case "shipping":
                return <ShippingOrderPage />; // Đang giao hàng
            case "delivered":
                return <CompletedOrderPage />; // Đã giao hàng
            case "failed":
                return <UnsuccessfulOrderPage />; // Không giao được
            case "cancelled":
                return <CanceledOrderPage />; // Đơn hàng đã hủy
            case "completedRefund":
                return <RefundedOrderPage />; // Đơn hàng đã hoàn tiền
            case "pendingRefund":
                return <RefundRequestPendingPage />; // Đơn hàng chờ hoàn tiền
            case "sellerproductList":
                return <SellerProductPage /> // Danh sách sản phẩm của người bán
            case "sellerpendingApproval":
                return <SellerPendingProductPage /> // Sản phẩm chờ duyệt
            case "sellerunapprovedProduct":
                return <SellerUnapprovedProductPage /> // Sản phẩm không được duyệt
            case "sellercanceledProduct":
                return <SellerCanceledProductPage /> // Sản phẩm bị hủy
            case "selleraddProduct":
                return <SellerProductPage /> // Thêm sản phẩm
            case "DiscountVoucher":
                return <SellerDiscountVoucherPage /> // Chỉnh sửa sản phẩm
            case "inbox":
                return <SellerChatComponentPage /> // Thêm voucher giảm giá
            case "reportOrder":
                return <SellerOrderReportPage /> // Bao cao don hang
            default:
                return <div>Chọn một mục trong menu để xem nội dung.</div>;
        }
    };
    

    return (
        <Layout>
            <SidebarMenu
                collapsed={collapsed}
                onToggleCollapse={() => setCollapsed(!collapsed)}
                onMenuSelect={setSelectedKey}
                selectedKey={selectedKey}
            />

            <Layout
                style={{
                    marginLeft: collapsed ? 80 : 260,
                    transition: "margin-left 0.2s",
                }}
            >
                <Content
                    style={{
                        margin: "24px 16px",
                        padding: 24,
                        minHeight: "calc(100vh - 48px)",
                        background: "#fff",
                        borderRadius: 8,
                    }}
                >
                    {renderContent()}
                </Content>
            </Layout>
        </Layout>
    );
};

export default SellerPage;
