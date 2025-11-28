import React from 'react';
import { Layout, Typography } from 'antd';
import { SyncOutlined } from '@ant-design/icons';
import SalesChart from '../../components/seller/SellerOrderReport/SalesChart';
import SellerOrderListTable from '../../components/seller/SellerOrderReport/SellerOrderListTable';

// --- PHẦN 1: IMPORT CÁC COMPONENTS CỦA BẠN ---
// Giả định đường dẫn dựa trên cấu trúc thư mục bạn cung cấp.
// Lưu ý: Mình bỏ '.tsx' và 'index' ở cuối để tuân thủ quy ước import thông thường

// --- PHẦN 2: TRANG PAGE CHÍNH (GỌI 2 COMPONENTS) ---

const { Content } = Layout;
const { Title } = Typography;

const SellerOrderReportPage: React.FC = () => {
    // 1. STATE QUẢN LÝ ĐỔ BÓNG KHI DI CHUỘT
    // Sử dụng useState để kiểm soát độ sâu đổ bóng (shadow depth)
    const [chartShadowDepth, setChartShadowDepth] = React.useState(6);
    const [tableShadowDepth, setTableShadowDepth] = React.useState(6);



    // Style cơ bản cho khối (giả lập Card Ant Design)
    const cardBaseStyle: React.CSSProperties = {
        padding: 24,
        backgroundColor: '#fff',
        borderRadius: 12,
        transition: 'box-shadow 0.3s cubic-bezier(0.4, 0, 0.2, 1)', // Hiệu ứng chuyển động mượt mà
        border: '1px solid #f0f0f0',
    };

    return (
        <Layout style={{ minHeight: '100vh', backgroundColor: '#f0f2f5' }}>
            <Content style={{ padding: 24 }}>

                {/* HEADER CỦA TRANG BÁO CÁO */}
                <div style={{ marginBottom: 24, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Title level={2} style={{ margin: 0, fontWeight: 700 }}>
                        <SyncOutlined style={{ marginRight: 10, color: '#faad14' }} />
                        Báo Cáo Tổng Quan Đơn Hàng
                    </Title>
                   
                </div>

                {/* --- 1. SELLER SALES CHART (Ở TRÊN) --- */}
                <div
                    style={{
                        ...cardBaseStyle,
                        boxShadow: `0 4px ${chartShadowDepth}px rgba(0, 0, 0, 0.1)`,
                        // Thêm hiệu ứng nâng lên một chút khi hover
                        transform: chartShadowDepth > 10 ? 'translateY(-2px)' : 'translateY(0)',
                        transition: 'box-shadow 0.3s cubic-bezier(0.4, 0, 0.2, 1), transform 0.3s ease-in-out',
                    }}
                    onMouseEnter={() => setChartShadowDepth(25)} // Shadow sâu hơn khi di chuột vào
                    onMouseLeave={() => setChartShadowDepth(6)} // Trở về shadow mặc định
                >
                    <SalesChart />
                </div>

                {/* --- 2. SELLER ORDER LIST TABLE (Ở DƯỚI) --- */}
                <div
                    style={{
                        ...cardBaseStyle,
                        marginTop: 24,
                        boxShadow: `0 4px ${tableShadowDepth}px rgba(0, 0, 0, 0.1)`,
                        transform: tableShadowDepth > 10 ? 'translateY(-2px)' : 'translateY(0)',
                        transition: 'box-shadow 0.3s cubic-bezier(0.4, 0, 0.2, 1), transform 0.3s ease-in-out',
                    }}
                    onMouseEnter={() => setTableShadowDepth(25)}
                    onMouseLeave={() => setTableShadowDepth(6)}
                >
                    <SellerOrderListTable />
                </div>

            </Content>
        </Layout>
    );
};

export default SellerOrderReportPage;