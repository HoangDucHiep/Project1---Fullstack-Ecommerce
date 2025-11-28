// File: SellerProductList.tsx
import React, { useState } from "react";
import { Table, Space, Tag, Switch, Image, Pagination, Button } from "antd";
import { message } from "antd"; // Import message để ví dụ cho cột Hoạt động

interface ProductItem {
    key: number;
    name: string;
    category: string;
    price: string;
    image: string;
    verified: boolean;
    active: boolean;
}

// ĐÃ SỬA: Thay đổi tên prop trong interface thành 'hiddenColumns'
interface SellerProductListProps {
    hiddenColumns?: string[]; // Prop tùy chọn, chứa mảng các dataIndex cần ẩn
}

// ĐÃ SỬA: Component nhận prop 'hiddenColumns'
const SellerProductList: React.FC<SellerProductListProps> = ({ hiddenColumns = [] }) => {
    const [page, setPage] = useState(1);
    const pageSize = 5;

    const data: ProductItem[] = [
        { key: 1, name: "Tiện ích Norton Ultimate", category: "Phần mềm", price: "40,00 đô la", image: "https://via.placeholder.com/40/FF5733/FFFFFF?text=P1", verified: true, active: true },
        { key: 2, name: "Office 2021 Professional", category: "Phần mềm", price: "150,00 đô la", image: "https://via.placeholder.com/40/33FF57/FFFFFF?text=P2", verified: true, active: false },
        { key: 3, name: "125 Audio dành cho trẻ em", category: "Giáo dục", price: "50,00 đô la", image: "https://via.placeholder.com/40/3357FF/FFFFFF?text=P3", verified: true, active: true },
        { key: 4, name: "Tai nghe Bluetooth XM4", category: "Điện tử", price: "299,00 đô la", image: "https://via.placeholder.com/40/FF33A1/FFFFFF?text=P4", verified: false, active: true },
        { key: 5, name: "Chuột không dây Logitech", category: "Điện tử", price: "25,00 đô la", image: "https://via.placeholder.com/40/33FFF9/FFFFFF?text=P5", verified: true, active: true },
        { key: 6, name: "Bàn phím cơ K95 RGB", category: "Điện tử", price: "180,00 đô la", image: "https://via.placeholder.com/40/A1FF33/FFFFFF?text=P6", verified: true, active: true },
        { key: 7, name: "Sách: Lập trình React", category: "Sách", price: "35,00 đô la", image: "https://via.placeholder.com/40/33A1FF/FFFFFF?text=P7", verified: false, active: false },
        { key: 8, name: "Màn hình cong 27 inch", category: "Điện tử", price: "450,00 đô la", image: "https://via.placeholder.com/40/FF8833/FFFFFF?text=P8", verified: true, active: true },
        { key: 9, name: "Phần mềm kế toán Misa", category: "Phần mềm", price: "500,00 đô la", image: "https://via.placeholder.com/40/8833FF/FFFFFF?text=P9", verified: true, active: true },
        { key: 10, name: "Máy hút bụi Robot", category: "Gia dụng", price: "320,00 đô la", image: "https://via.placeholder.com/40/33FF88/FFFFFF?text=P10", verified: false, active: true },
        { key: 11, name: "Cáp sạc nhanh Type-C", category: "Điện tử", price: "12,00 đô la", image: "https://via.placeholder.com/40/88FF33/FFFFFF?text=P11", verified: true, active: true },
        { key: 12, name: "Máy pha cà phê tự động", category: "Gia dụng", price: "199,00 đô la", image: "https://via.placeholder.com/40/FF3388/FFFFFF?text=P12", verified: false, active: false },
        { key: 13, name: "Game thẻ bài Yugi", category: "Giải trí", price: "10,00 đô la", image: "https://via.placeholder.com/40/3388FF/FFFFFF?text=P13", verified: true, active: true },
        { key: 14, name: "Thẻ nhớ MicroSD 128GB", category: "Điện tử", price: "20,00 đô la", image: "https://via.placeholder.com/40/88FF33/FFFFFF?text=P14", verified: true, active: true },
        { key: 15, name: "Bộ dụng cụ sửa chữa", category: "Công cụ", price: "65,00 đô la", image: "https://via.placeholder.com/40/33FFD1/FFFFFF?text=P15", verified: false, active: true },
        { key: 16, name: "Phần mềm chỉnh sửa video", category: "Phần mềm", price: "99,00 đô la", image: "https://via.placeholder.com/40/D133FF/FFFFFF?text=P16", verified: true, active: true },
        { key: 17, name: "Đèn LED trang trí", category: "Gia dụng", price: "15,00 đô la", image: "https://via.placeholder.com/40/FFD133/FFFFFF?text=P17", verified: true, active: false },
        { key: 18, name: "Camera an ninh ngoài trời", category: "Điện tử", price: "85,00 đô la", image: "https://via.placeholder.com/40/33D1FF/FFFFFF?text=P18", verified: false, active: true },
        { key: 19, name: "Máy lọc không khí", category: "Gia dụng", price: "120,00 đô la", image: "https://via.placeholder.com/40/D1FF33/FFFFFF?text=P19", verified: true, active: true },
        { key: 20, name: "Đồng hồ thông minh Fitpro", category: "Điện tử", price: "75,00 đô la", image: "https://via.placeholder.com/40/33FFD1/FFFFFF?text=P20", verified: true, active: true },
    ];

    const displayedData = data.slice((page - 1) * pageSize, page * pageSize);

    // Hàm xử lý cơ bản (để minh họa nút Hoạt động)
    const handleAction = (action: string, product: ProductItem) => {
        message.info(`${action} sản phẩm: ${product.name}`);
    };

    // Định nghĩa cột gốc
    const allColumns = [
        { title: "SL", dataIndex: "key", width: 60, },
        {
            title: "Tên sản phẩm",
            dataIndex: "name",
            render: (_: any, record: ProductItem) => (
                <Space>
                    <Image width={40} height={40} src={record.image} preview={false} />
                    <span>{record.name}</span>
                </Space>
            ),
        },
        { title: "Loại sản phẩm", dataIndex: "category", },
        { title: "Đơn giá", dataIndex: "price", },
        {
            title: "Xác minh",
            dataIndex: "verified",
            render: (v: boolean) => <Tag color={v ? "green" : "red"}>{v ? "Đã xác minh" : "Chưa"}</Tag>,
        },
        {
            title: "Trạng thái",
            dataIndex: "active",
            render: (v: boolean) => <Switch defaultChecked={v} />,
        },
        {
            title: "Hoạt động",
            render: (_: any, record: ProductItem) => ( // Thêm record để dùng trong onClick
                <Space>
                    <Button type="link" onClick={() => handleAction("Xem", record)}>👁</Button>
                    <Button type="link" onClick={() => handleAction("Chỉnh sửa", record)}>✏️</Button>
                    <Button type="link" danger onClick={() => handleAction("Xóa", record)}>🗑</Button>
                </Space>
            ),
        },
    ];

    // ĐÃ SỬA: Logic lọc cột sử dụng prop 'hiddenColumns'
    const visibleColumns = allColumns.filter((column: any) => {
        const dataIndex = column.dataIndex as string | undefined;

        // Nếu cột không có dataIndex (như cột Hoạt động) HOẶC 
        // dataIndex KHÔNG nằm trong mảng hiddenColumns, thì hiển thị.
        if (!dataIndex || !hiddenColumns.includes(dataIndex)) {
            return true;
        }

        // Ẩn cột nếu dataIndex nằm trong hiddenColumns
        return false;
    });
    // -------------------------------------------------------------------

    return (
        <div className="p-4 bg-white rounded-xl shadow-md">
            <Table
                columns={visibleColumns} // SỬ DỤNG MẢNG CỘT ĐÃ LỌC
                dataSource={displayedData}
                pagination={false}
                rowKey="key"
            />

            <div className="flex justify-end mt-4">
                <Pagination
                    current={page}
                    pageSize={pageSize}
                    total={data.length}
                    onChange={(p) => setPage(p)}
                />
            </div>
        </div>
    );
};

export default SellerProductList;