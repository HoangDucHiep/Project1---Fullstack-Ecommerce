import React from "react";
import { Card, Descriptions, Tag, Typography, Divider, List } from "antd";

const { Title, Paragraph } = Typography;

const ProductInfo: React.FC = () => {
    return (
        <div style={{ padding: 24, background: "#fff", marginTop:20 }}>
            <Title level={4} style={{ color: "#222", marginBottom: 16 }}>
                CHI TIẾT SẢN PHẨM
            </Title>

            <Card bordered={false} style={{ background: "#fafafa", marginBottom: 32 }}>
                <Descriptions column={2} bordered size="small">
                    <Descriptions.Item label="Danh Mục">
                        Shopee &gt; Thiết Bị Điện Gia Dụng &gt; Quạt & Máy nóng lạnh &gt; Quạt
                    </Descriptions.Item>
                    <Descriptions.Item label="Kho">CÒN HÀNG</Descriptions.Item>
                    <Descriptions.Item label="Loại bảo hành">
                        Bảo hành nhà cung cấp
                    </Descriptions.Item>
                    <Descriptions.Item label="Xuất xứ">Trung Quốc</Descriptions.Item>
                    <Descriptions.Item label="Hạn bảo hành">15 ngày</Descriptions.Item>
                    <Descriptions.Item label="Tên tổ chức chịu trách nhiệm sản xuất">
                        Đại lý Dung Lượng
                    </Descriptions.Item>
                    <Descriptions.Item label="Địa chỉ tổ chức chịu trách nhiệm sản xuất" span={2}>
                        3D Tiền Thịnh - Mê Linh - Hà Nội
                    </Descriptions.Item>
                    <Descriptions.Item label="Gửi từ">Hà Nội</Descriptions.Item>
                </Descriptions>
            </Card>

            <Title level={4} style={{ color: "#222", marginBottom: 16 }}>
                MÔ TẢ SẢN PHẨM
            </Title>

            <Card bordered={false} style={{ background: "#fafafa" }}>
                <Paragraph>
                    Quạt để bàn văn phòng, để giường cho trẻ em hiện thu cảm điểm. Quạt mini bền đẹp.
                </Paragraph>
                <Paragraph>
                    <Tag color="green">
                        ✅ Quạt để bàn tiện lợi, đặc biệt phù hợp khi dùng kết hợp với máy lạnh trong văn phòng hoặc phòng ngủ.
                    </Tag>
                    Tiết kiệm tới 30% lượng điện máy lạnh tiêu thụ.
                </Paragraph>
                <Paragraph>
                    <Tag color="blue">
                        💨 Ngoài ra phù hợp khi dùng cá nhân trong văn phòng hoặc cho trẻ em vì công suất vừa phải, kích thước nhỏ gọn.
                    </Tag>
                </Paragraph>

                <Divider />

                <Title level={5}>👉👉👉 Thông số Quạt để bàn</Title>
                <List
                    size="small"
                    dataSource={[
                        "Chuyên dụng: Để bàn / Để trong giường ngủ / trẻ em",
                        "Quạt gió: 2 tốc độ; Cánh quạt: 15 cm; Tốc độ gió: 40km/h; Khoảng cách: 4m",
                        "Nguồn điện: 220V / 50-60HZ; Công suất 20W",
                        "Kích thước: 28 x 28 x 13cm; Đặt quạt: Đường kính 15cm",
                        "Màu: hồng, xanh, trắng",
                        "Chất liệu: Nhựa ABS khó vỡ, độ bền cao, thiết kế mạnh mẽ",
                        "Xuất xứ: Trung Quốc",
                    ]}
                    renderItem={(item) => <List.Item>- {item}</List.Item>}
                />

                <Paragraph>
                    <Tag color="red">📦 Bảo hành:</Tag> 15 ngày lỗi. Trả hàng hoàn tiền 15 ngày miễn phí.
                </Paragraph>

                <Divider />

                <Title level={5}>📘 Sử dụng Quạt để bàn</Title>
                <List
                    size="small"
                    dataSource={[
                        "Không để quạt ở các nơi ẩm ướt.",
                        "Tránh chạm cánh quạt khi đang quay.",
                        "Cách sử dụng: Cắm điện, bật công tắc, vặn núm điều chỉnh hướng gió.",
                    ]}
                    renderItem={(item) => <List.Item>- {item}</List.Item>}
                />
            </Card>
        </div>
    );
};

export default ProductInfo;
