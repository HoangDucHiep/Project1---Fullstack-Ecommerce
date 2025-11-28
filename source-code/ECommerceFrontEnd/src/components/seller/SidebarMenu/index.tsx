import React, { useState } from "react";
import { Layout, Menu, Input, Image, Button } from "antd";
import {
    ShoppingOutlined,
    ReloadOutlined,
    TagsOutlined,
    StarOutlined,
    BarChartOutlined,
    FileTextOutlined,
    DollarOutlined,
    CheckCircleOutlined,
    ClockCircleOutlined,
    CarOutlined,
    CloseCircleOutlined,
    BankOutlined,
    InboxOutlined,
    TagOutlined,
    BarChartOutlined as ChartOutlined,
    CheckSquareOutlined,
    ExclamationCircleOutlined,
    PlusCircleOutlined,
    GiftOutlined,
    LineChartOutlined,
    FileDoneOutlined,
    FileProtectOutlined,
    MenuOutlined,
} from "@ant-design/icons";
import img1 from "../../../assets/img/logo.png";

const { Sider, Content } = Layout;
const { Search } = Input;

export interface SidebarMenuProps {
    collapsed: boolean; // ✅ dùng collapse từ cha
    onToggleCollapse: () => void; // ✅ callback từ cha
    onMenuSelect: (key: string) => void;
    selectedKey: string;
}

const SidebarMenu: React.FC<SidebarMenuProps> = ({
    collapsed,
    onToggleCollapse,
    onMenuSelect,
    selectedKey,
}) => {
    const [openKeys, setOpenKeys] = useState<string[]>(["order"]);

    const handleOpenChange = (keys: string[]) => {
        setOpenKeys(keys);
    };

    return (
        <Layout>
            <Sider
                collapsible
                collapsed={collapsed}
                onCollapse={onToggleCollapse} // ✅ dùng hàm từ cha
                trigger={null}
                width={260}
                collapsedWidth={80}
                style={{
                    background: "#001F3F",
                    minHeight: "100vh",
                    position: "fixed",
                    left: 0,
                    top: 0,
                    display: "flex",
                    flexDirection: "column",
                    overflow: "hidden",
                }}
            >
                {/* Header (logo + toggle) */}
                <div
                    style={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: collapsed ? "center" : "space-between",
                        height: 70,
                        borderBottom: "1px solid rgba(255,255,255,0.15)",
                        background: "#001A35",
                        padding: "0 20px",
                        transition: "all 0.2s",
                        flex: "0 0 70px",
                    }}
                >
                    {!collapsed && <Image src={img1} preview={false} width={75} />}
                    <Button
                        type="text"
                        icon={<MenuOutlined style={{ fontSize: 20, color: "white" }} />}
                        onClick={onToggleCollapse} // ✅ toggle từ cha
                    />
                </div>

                {!collapsed && (
                    <div style={{ padding: "10px", flex: "0 0 56px" }}>
                        <Search
                            placeholder="Tìm kiếm menu..."
                            allowClear
                            style={{
                                marginBottom: 8,
                                borderRadius: 6,
                            }}
                        />
                    </div>
                )}

                <div
                    className="menuScroll"
                    style={{
                        flex: 1,
                        minHeight: 0,
                        paddingRight: 6,
                        paddingBottom: 10,
                    }}
                >
                    <Menu
                        mode="inline"
                        theme="dark"
                        openKeys={openKeys}
                        onOpenChange={handleOpenChange}
                        selectedKeys={[selectedKey]}
                        onClick={({ key }) => onMenuSelect(key)}
                        style={{
                            background: "transparent",
                            borderRight: 0,
                            color: "white",
                            height: "100%",
                            minHeight: 0,
                            maxHeight: collapsed
                                ? "calc(100vh - 70px - 48px)"
                                : "calc(100vh - 70px - 56px - 48px)",
                            overflowY: "auto",
                            overflowX: "hidden",
                            paddingBottom: 8,
                        }}
                        items={[
                            {
                                key: "order",
                                icon: <ShoppingOutlined />,
                                label: "Quản lý đơn hàng",
                                children: [
                                    { key: "allOrders", icon: <CheckSquareOutlined />, label: "Tất cả đơn hàng" },
                                    { key: "unconfirmed", icon: <ClockCircleOutlined />, label: "Chưa giải quyết" },
                                    { key: "confirmed", icon: <CheckCircleOutlined />, label: "Đã xác nhận" },
                                    { key: "shipping", icon: <CarOutlined />, label: "Đang giao hàng" },
                                    { key: "delivered", icon: <FileDoneOutlined />, label: "Đã giao hàng" },
                                    { key: "failed", icon: <ExclamationCircleOutlined />, label: "Không giao được" },
                                    { key: "cancelled", icon: <CloseCircleOutlined />, label: "Đơn hàng đã hủy" },
                                ],
                            },
                            {
                                key: "refund",
                                icon: <ReloadOutlined />,
                                label: "Yêu cầu hoàn tiền",
                                children: [
                                    { key: "pendingRefund", icon: <ClockCircleOutlined />, label: "Chưa giải quyết" },
                                    { key: "completedRefund", icon: <CheckCircleOutlined />, label: "Đã hoàn tiền" },
                                    
                                ],
                            },
                            {
                                key: "product",
                                icon: <TagsOutlined />,
                                label: "Quản lý sản phẩm",
                                children: [
                                    { key: "sellerproductList", icon: <TagOutlined />, label: "Danh sách sản phẩm" },
                                    { key: "sellerpendingApproval", icon: <ClockCircleOutlined />, label: "Sản phẩm chờ duyệt" },
                                    { key: "sellerunapprovedProduct", icon: <ExclamationCircleOutlined />, label: "Sản phẩm không được duyệt" },
                                    { key: "sellercanceledProduct", icon: <CheckCircleOutlined />, label: "Sản phẩm bị hủy" },
                                    { key: "selleraddProduct", icon: <PlusCircleOutlined />, label: "Thêm sản phẩm" },
                                ],

                            },
                            {
                                key: "promotion",
                                icon: <StarOutlined />,
                                label: "Quản lý khuyến mãi",
                                children: [
                                    { key: "DiscountVoucher", icon: <GiftOutlined />, label: "Phiếu giảm giá" },
                                    
                                ],
                            },
                            {
                                key: "support",
                                icon: <FileTextOutlined />,
                                label: "Trợ giúp & hỗ trợ",
                                children: [{ key: "inbox", icon: <InboxOutlined />, label: "Hộp thư đến" }],
                            },
                            {
                                key: "report",
                                icon: <BarChartOutlined />,
                                label: "Báo cáo & phân tích",
                                children: [
                                    { key: "reportTransaction", icon: <LineChartOutlined />, label: "Báo cáo giao dịch" },
                                    { key: "reportProduct", icon: <ChartOutlined />, label: "Báo cáo sản phẩm" },
                                    { key: "reportOrder", icon: <FileProtectOutlined />, label: "Báo cáo đơn hàng" },
                                    { key: "reportVAT", icon: <FileDoneOutlined />, label: "Báo cáo VAT" },
                                ],
                            },
                            {
                                key: "business",
                                icon: <DollarOutlined />,
                                label: "Phần kinh doanh",
                                children: [
                                    { key: "withdraw", icon: <BankOutlined />, label: "Rút lui" },
                                    { key: "bankInfo", icon: <BankOutlined />, label: "Thông tin ngân hàng" },
                                ],
                            },
                        ]}
                    />
                </div>

                {!collapsed && (
                    <div
                        style={{
                            padding: "10px 15px",
                            height: 48,
                            flex: "0 0 48px",
                            fontSize: 12,
                            color: "rgba(255,255,255,0.7)",
                            textAlign: "center",
                            borderTop: "1px solid rgba(255,255,255,0.15)",
                            background: "#002B5B",
                        }}
                    >
                        © 2025 Seller Center
                    </div>
                )}
            </Sider>

        
            <style>{`
        ${/* 🎨 giữ nguyên toàn bộ CSS của bạn */""}
        .ant-menu-dark {
          background-color: transparent !important;
          color: #fff !important;
        }
        .ant-menu-submenu-title {
          font-weight: 600;
          color: #fff !important;
        }
        .ant-menu-item {
          color: rgba(255,255,255,0.9) !important;
          border-radius: 6px;
        }
        .ant-menu-item:hover {
          background-color: #1E63A3 !important;
        }
        .ant-menu-submenu-open > .ant-menu-sub {
          background-color: #0D3B66 !important;
          border-radius: 6px;
          padding: 4px 0;
        }
        .ant-menu-item-selected {
          background-color: #2A74B5 !important;
          color: #fff !important;
        }
          .ant-menu-submenu-title:hover {
        background-color: #1E63A3 !important;
      }

        .menuScroll .ant-menu {
          max-height: calc(100vh - 70px - 56px - 48px);
          overflow-y: auto;
        }
        .menuScroll .ant-menu::-webkit-scrollbar {
          width: 6px;
        }
        .menuScroll .ant-menu::-webkit-scrollbar-track {
          background: #001F3F;
        }
        .menuScroll .ant-menu::-webkit-scrollbar-thumb {
          background-color: #004B8D;
          border-radius: 4px;
        }
        .menuScroll .ant-menu::-webkit-scrollbar-thumb:hover {
          background-color: #0066CC;
        }

        .ant-menu-dark .ant-menu-item:hover,
        .ant-menu-dark .ant-menu-submenu-title:hover {
          color: #4CAF50 !important;
          background-color: transparent !important;
        }

        .ant-menu-item-selected {
          color: #4CAF50 !important;
          background-color: transparent !important;
          font-weight: 600;
        }
      `}</style>
        </Layout>
    );
};
export default SidebarMenu;
