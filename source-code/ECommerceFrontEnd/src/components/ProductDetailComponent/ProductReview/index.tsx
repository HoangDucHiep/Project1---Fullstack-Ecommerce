import React, { useState } from "react";
import {
    Card,
    Rate,
    Typography,
    Button,
    Space,
    Avatar,
    Input,
    Upload,
    message,
    List,
} from "antd";
import { LikeOutlined, UploadOutlined, UserOutlined } from "@ant-design/icons";

const { Title, Text, Paragraph } = Typography;
const { TextArea } = Input;

interface Review {
    id: number;
    user: string;
    avatar?: string;
    rating: number;
    date: string;
    comment: string;
    likes: number;
}

const ProductReviews: React.FC = () => {
    const [reviews, setReviews] = useState<Review[]>([
        {
            id: 1,
            user: "L******0",
            avatar:
                "https://api.dicebear.com/9.x/adventurer/svg?seed=Linh",
            rating: 5,
            date: "2025-02-18 13:44",
            comment:
                "Sản phẩm dùng rất tốt, giao hàng nhanh, đóng gói cẩn thận. Mình sẽ ủng hộ shop thêm lần nữa 💕",
            likes: 12,
        },
        {
            id: 2,
            user: "N****T",
            avatar:
                "https://api.dicebear.com/9.x/adventurer/svg?seed=Tham",
            rating: 4,
            date: "2025-02-20 09:32",
            comment:
                "Son màu đẹp, mùi thơm nhẹ, chất lì vừa phải. Mình thấy ổn so với giá tiền.",
            likes: 8,
        },
    ]);

    const [newComment, setNewComment] = useState("");
    const [newRating, setNewRating] = useState(0);
    const [fileList, setFileList] = useState<unknown[]>([]);

    const handleAddComment = () => {
        if (!newComment.trim() || newRating === 0) {
            message.warning("Vui lòng nhập đánh giá và chọn số sao!");
            return;
        }

        const newReview: Review = {
            id: Date.now(),
            user: "Bạn mới",
            avatar: "https://api.dicebear.com/9.x/adventurer/svg?seed=NewUser",
            rating: newRating,
            date: new Date().toLocaleString(),
            comment: newComment,
            likes: 0,
        };

        setReviews([newReview, ...reviews]);
        setNewComment("");
        setNewRating(0);
        setFileList([]);
        message.success("Đã gửi đánh giá thành công!");
    };

    return (
        <div style={{ padding: 24, background: "#fff", borderRadius: 12 }}>
            <Title level={4} style={{ color: "#222" }}>
                ĐÁNH GIÁ SẢN PHẨM
            </Title>

            {/* Tổng điểm đánh giá */}
            <Card
                style={{
                    marginBottom: 24,
                    background: "#fff9f4",
                    borderRadius: 8,
                    textAlign: "center",
                }}
            >
                <Title level={2} style={{ color: "#ff4d4f", margin: 0 }}>
                    4.8
                </Title>
                <Rate disabled defaultValue={5} allowHalf />
                <Text type="secondary" style={{ display: "block", marginTop: 4 }}>
                    (Từ 2 đánh giá)
                </Text>
            </Card>

            {/* Ô nhập bình luận */}
            <Card style={{ marginBottom: 24, borderRadius: 8 }}>
                <Title level={5}>Viết đánh giá của bạn</Title>
                <Rate
                    value={newRating}
                    onChange={setNewRating}
                    style={{ marginBottom: 12 }}
                />
                <TextArea
                    rows={4}
                    placeholder="Nhập cảm nhận của bạn về sản phẩm..."
                    value={newComment}
                    onChange={(e) => setNewComment(e.target.value)}
                    style={{ marginBottom: 12 }}
                />
                <Upload
                    fileList={fileList}
                    beforeUpload={() => false}
                    onChange={({ fileList }) => setFileList(fileList)}
                >
                    <Button icon={<UploadOutlined />}>Tải ảnh lên</Button>
                </Upload>
                <div style={{ marginTop: 16, textAlign: "right" }}>
                    <Button type="primary" onClick={handleAddComment}>
                        Gửi đánh giá
                    </Button>
                </div>
            </Card>

            {/* Danh sách đánh giá */}
            <List
                itemLayout="horizontal"
                dataSource={reviews}
                renderItem={(item) => (
                    <Card
                        key={item.id}
                        style={{
                            marginBottom: 16,
                            background: "#fafafa",
                            borderRadius: 8,
                        }}
                        bodyStyle={{ padding: 16 }}
                    >
                        <Space align="start">
                            <Avatar
                                src={item.avatar}
                                icon={!item.avatar && <UserOutlined />}
                                size={48}
                            />
                            <div>
                                <Text strong>{item.user}</Text>
                                <div>
                                    <Rate
                                        disabled
                                        defaultValue={item.rating}
                                        style={{ fontSize: 14 }}
                                    />
                                </div>
                                <Text type="secondary" style={{ fontSize: 12 }}>
                                    {item.date}
                                </Text>
                                <Paragraph style={{ marginTop: 8 }}>{item.comment}</Paragraph>
                                <Space>
                                    <LikeOutlined />{" "}
                                    <Text type="secondary">{item.likes}</Text>
                                </Space>
                            </div>
                        </Space>
                    </Card>
                )}
            />
        </div>
    );
};

export default ProductReviews;

