import React from "react";
import { Input, Button, Space, Tooltip } from "antd";
import { SearchOutlined, PlusOutlined } from "@ant-design/icons";

const SellerProductActionBar: React.FC = () => {
    return (
        <div
            style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                flexWrap: "wrap",
                gap: 16,
                marginBottom: 20,
            }}
        >
            {/* Ô tìm kiếm */}
            <Input.Search
                placeholder="Tìm kiếm theo Tên sản phẩm"
                allowClear
                enterButton="Tìm kiếm"
                prefix={<SearchOutlined />}
                style={{
                    width: "100%",
                    maxWidth: 400,
                    borderRadius: 8,
                    overflow: "hidden",
                }}
            />

            {/* Nhóm nút hành động */}
            <Space wrap>
                {/* Nút xuất file có icon Excel thật */}
                <Tooltip >
                    <Button
                        style={{
                            borderColor: "#52c41a",
                            color: "#52c41a",
                            fontWeight: 500,
                            borderRadius: 8,
                            display: "flex",
                            alignItems: "center",
                            gap: 6,
                            transition: "all 0.2s ease",
                        }}
                        onMouseEnter={(e) =>
                            (e.currentTarget.style.backgroundColor = "#f6ffed")
                        }
                        onMouseLeave={(e) =>
                            (e.currentTarget.style.backgroundColor = "transparent")
                        }
                    >
                        <img
                            src="https://cdn-icons-png.flaticon.com/512/732/732220.png"
                            alt="Excel Icon"
                            style={{ width: 18, height: 18 }}
                        />
                        Xuất file
                    </Button>
                </Tooltip>

                {/* Nút thêm sản phẩm */}
                <Button
                    type="primary"
                    icon={<PlusOutlined />}
                    style={{
                        backgroundColor: "#0056b3",
                        borderRadius: 8,
                        fontWeight: 500,
                    }}
                >
                    Thêm sản phẩm mới
                </Button>
            </Space>
        </div>
    );
};

export default SellerProductActionBar;
