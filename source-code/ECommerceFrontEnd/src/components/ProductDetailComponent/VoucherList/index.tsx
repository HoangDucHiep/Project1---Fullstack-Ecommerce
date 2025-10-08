import React from "react";
import { Space, Tag, Typography } from "antd";

const { Text } = Typography;

const VoucherList: React.FC = () => {
    // 🔹 Sau này có thể thay mảng này bằng data lấy từ API hoặc DB
    const vouchers = [
        { id: 1, title: "Giảm 20%", minOrder: "Đơn tối thiểu 150k", expired: "30.11.2025" },
        { id: 2, title: "Giảm 10%", minOrder: "Đơn tối thiểu 100k", expired: "15.12.2025" },
    ];

    return (
        <div style={{ marginTop: 12 }}>
            <Text strong style={{ display: "block", marginBottom: 6 }}>
                Voucher Của Shop
            </Text>
            <Space wrap>
                {vouchers.map((voucher) => (
                    <Tag
                        key={voucher.id}
                        color="#ffecec"
                        style={{
                            color: "#ee4d2d",
                            border: "1px solid #ee4d2d",
                            borderRadius: 4,
                            fontWeight: 500,
                            padding: "4px 8px",
                            fontSize: 13,
                        }}
                    >
                        {voucher.title}
                    </Tag>
                ))}
            </Space>
        </div>
    );
};

export default VoucherList;
