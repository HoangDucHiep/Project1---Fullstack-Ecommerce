import React, { useState, useMemo } from 'react';
import { Table, Tag, Button, Input, Space, Typography, Badge, Flex } from 'antd';
// DownloadOutlined không dùng nữa, thay bằng biểu tượng Excel
import { SearchOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';

// Import Font Awesome (nếu bạn đã cài đặt)
// import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
// import { faFileExcel } from '@fortawesome/free-solid-svg-icons';


// --- 1. DATA TYPES (TYPESCRIPT) ---
interface Order {
    key: string;
    stt: number;
    orderCode: string;
    totalAmount: number;
    productDiscount: number;
    voucherDiscount: number;
    referralDiscount: number;
    shippingFee: number;
    vatTax: number;
    shipperIncentive: number;
    status: 'Đã xác nhận' | 'Đã giao hàng' | string;
}

// --- 2. UTILITY FUNCTIONS ---
const formatCurrency = (amount: number): string => {
    return `${amount.toLocaleString('vi-VN', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    })}VND`;
};

const handleExport = () => {
    alert('Thực hiện xuất file Excel/CSV...');
};

// --- 3. MOCK DATA (Dữ liệu cố định) ---
const rawMockData: Omit<Order, 'key'>[] = [
    { stt: 1, orderCode: '100130', totalAmount: 5000000, productDiscount: 250000, voucherDiscount: 0, referralDiscount: 0, shippingFee: 100000, vatTax: 150000, shipperIncentive: 0, status: 'Đã xác nhận' },
    { stt: 2, orderCode: '100188', totalAmount: 535000, productDiscount: 0, voucherDiscount: 0, referralDiscount: 0, shippingFee: 10000, vatTax: 25000, shipperIncentive: 0, status: 'Đã giao hàng' },
    { stt: 3, orderCode: '100181', totalAmount: 380000, productDiscount: 40000, voucherDiscount: 0, referralDiscount: 0, shippingFee: 0, vatTax: 20000, shipperIncentive: 0, status: 'Đã giao hàng' },
    { stt: 4, orderCode: '100152', totalAmount: 1059000, productDiscount: 60000, voucherDiscount: 0, referralDiscount: 0, shippingFee: 30000, vatTax: 99000, shipperIncentive: 40000, status: 'Đã giao hàng' },
    { stt: 5, orderCode: '100144', totalAmount: 5400000, productDiscount: 100000, voucherDiscount: 0, referralDiscount: 0, shippingFee: 0, vatTax: 500000, shipperIncentive: 0, status: 'Đã giao hàng' },
    { stt: 6, orderCode: '100143', totalAmount: 4808000, productDiscount: 500000, voucherDiscount: 0, referralDiscount: 0, shippingFee: 58000, vatTax: 250000, shipperIncentive: 0, status: 'Đã xác nhận' },
    { stt: 7, orderCode: '100140', totalAmount: 583000, productDiscount: 0, voucherDiscount: 0, referralDiscount: 0, shippingFee: 58000, vatTax: 25000, shipperIncentive: 0, status: 'Đã giao hàng' },
    { stt: 8, orderCode: '100138', totalAmount: 23040000, productDiscount: 2500000, voucherDiscount: 1000000, referralDiscount: 0, shippingFee: 290000, vatTax: 1250000, shipperIncentive: 0, status: 'Đã xác nhận' },
];

const initialData: Order[] = rawMockData.map((item) => ({
    ...item,
    key: item.orderCode,
}));

// --- 4. COLUMN DEFINITION (Định nghĩa cột cố định) ---
const orderColumns: ColumnsType<Order> = [
    { title: 'SL', dataIndex: 'stt', key: 'stt', width: 50, fixed: 'left' as const },
    { title: 'Mã Đơn Hàng', dataIndex: 'orderCode', key: 'orderCode', width: 120, fixed: 'left' as const },
    {
        title: 'Tổng Số Tiền',
        dataIndex: 'totalAmount',
        key: 'totalAmount',
        render: (amount: number) => <strong>{formatCurrency(amount)}</strong>,
        sorter: (a, b) => a.totalAmount - b.totalAmount,
    },
    {
        title: 'Giảm Giá Sản Phẩm',
        dataIndex: 'productDiscount',
        key: 'productDiscount',
        render: (amount: number) => <span>{formatCurrency(amount)}</span>,
    },
    {
        title: 'Phiếu Giảm Giá',
        dataIndex: 'voucherDiscount',
        key: 'voucherDiscount',
        render: (amount: number) => <span>{formatCurrency(amount)}</span>,
    },
    {
        title: 'Giảm Giá Giới Thiệu',
        dataIndex: 'referralDiscount',
        key: 'referralDiscount',
        render: (amount: number) => <span>{formatCurrency(amount)}</span>,
    },
    {
        title: 'Phí Vận Chuyển',
        dataIndex: 'shippingFee',
        key: 'shippingFee',
        render: (amount: number) => <span>{formatCurrency(amount)}</span>,
    },
    {
        title: 'VAT/THUẾ',
        dataIndex: 'vatTax',
        key: 'vatTax',
        render: (amount: number) => <span>{formatCurrency(amount)}</span>,
    },
    {
        title: 'Khuyến Khích Người Giao Hàng',
        dataIndex: 'shipperIncentive',
        key: 'shipperIncentive',
        render: (amount: number) => <span>{formatCurrency(amount)}</span>,
    },
    {
        title: 'Trạng Thái',
        dataIndex: 'status',
        key: 'status',
        width: 120,
        fixed: 'right' as const,
        render: (status: string) => {
            const color = status === 'Đã xác nhận' ? 'blue' : 'green';
            return (
                <Tag color={color} key={status}>
                    {status}
                </Tag>
            );
        },
        filters: [
            { text: 'Đã xác nhận', value: 'Đã xác nhận' },
            { text: 'Đã giao hàng', value: 'Đã giao hàng' },
        ],
        onFilter: (value, record) => record.status.indexOf(value as string) === 0,
    },
];


// --- 5. MAIN COMPONENT ---
const SellerOrderListTable: React.FC = () => {
    const [searchText, setSearchText] = useState('');
    const totalOrders = initialData.length;

    const filteredData = useMemo(() => {
        if (!searchText) {
            return initialData;
        }
        const lowerCaseSearchText = searchText.toLowerCase();
        return initialData.filter(item =>
            item.orderCode.toLowerCase().includes(lowerCaseSearchText) ||
            item.status.toLowerCase().includes(lowerCaseSearchText)
        );
    }, [searchText]);

    return (
        <div style={{ padding: 24, backgroundColor: '#fff', borderRadius: 8 }}>
            <Flex justify="space-between" align="center" style={{ marginBottom: 16 }}>
                <Typography.Title level={4} style={{ margin: 0, display: 'flex', alignItems: 'center' }}>
                    Tổng số đơn hàng{' '}
                    <Badge
                        count={totalOrders}
                        style={{ backgroundColor: '#1890ff', marginLeft: 8 }}
                    />
                </Typography.Title>

                <Space>
                    <Input
                        placeholder="Tìm kiếm theo mã đh, trạng thái"
                        prefix={<SearchOutlined />}
                        style={{ width: 280 }}
                        value={searchText}
                        onChange={(e) => setSearchText(e.target.value)}
                    />

                    <Button
                        onClick={handleExport}
                        // 💡 CẬP NHẬT ICON VÀ CSS CHO NÚT XUẤT KHẨU 💡
                        // Sử dụng một span để chứa biểu tượng Excel (có thể là Font Awesome, hoặc img)
                        icon={
                            // Nếu bạn dùng Font Awesome:
                            // <FontAwesomeIcon icon={faFileExcel} style={{ color: '#107c41' }} />
                            // Hoặc dùng thẻ <img> nếu bạn có sẵn icon:
                            <img src="https://img.icons8.com/color/24/000000/ms-excel.png" alt="Excel icon" style={{ marginRight: 8, height: 16 }} />
                        }
                        style={{
                            backgroundColor: '#e6f7ff', // Màu nền nhạt, giống trong ảnh
                            borderColor: '#91d5ff',    // Border màu xanh nhạt
                            color: '#1890ff',           // Chữ màu xanh Ant Design Primary
                            fontWeight: 'bold',
                            borderRadius: '6px',        // Bo tròn góc
                            display: 'flex',            // Để icon và text căn giữa
                            alignItems: 'center',       // Căn dọc
                            padding: '4px 15px',        // Đệm nút
                            height: '32px',             // Chiều cao nút
                        }}
                    >
                        Xuất khẩu
                    </Button>
                </Space>
            </Flex>

            <Table<Order>
                columns={orderColumns}
                dataSource={filteredData}
                pagination={{
                    pageSize: 8,
                    showSizeChanger: true,
                    pageSizeOptions: ['8', '15', '30'],
                }}
                scroll={{ x: 1600 }}
                bordered
            />
        </div>
    );
};

export default SellerOrderListTable;