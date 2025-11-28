import React from "react";
import {
    Form,
    Input,
    Select,
    Button,
    DatePicker,
    Row,
    Col,
    Typography,
    Card,
    Space,
} from "antd";
import { TagsFilled, SendOutlined, ReloadOutlined } from "@ant-design/icons";

const { Title } = Typography;
const { Option } = Select; // Vẫn giữ lại Option nhưng sẽ không dùng cho loaiGiamGia

// Định nghĩa kiểu CSS tùy chỉnh (giữ nguyên để giữ giao diện đẹp)
const styles = {
    card: {
        borderRadius: 16,
        boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
        maxWidth: 1200,
        margin: "30px auto",
    },
    title: {
        marginBottom: "24px",
        borderBottom: "1px solid #f0f0f0",
        paddingBottom: "12px",
        color: "#1890ff",
    },
};

const SellerDiscountForm: React.FC = () => {
    const [form] = Form.useForm();

    const handleSubmit = (values: any) => {
        console.log("Dữ liệu gửi đi:", values);
    };

    const handleReset = () => {
        form.resetFields();
    };

    const generateCode = () => {
        const random = Math.random().toString(36).substring(2, 10).toUpperCase();
        form.setFieldValue("maGiamGia", random);
    };

    return (
        <Card
            style={styles.card}
            bodyStyle={{ padding: "40px" }}
        >
            <Title level={3} style={styles.title}>
                <TagsFilled style={{ marginRight: 10, color: "#faad14" }} /> Thiết Lập Phiếu Giảm Giá{" "}
            </Title>

            <Form
                form={form}
                layout="vertical"
                onFinish={handleSubmit}
                initialValues={{
                    loaiGiamGia: "VND", // Đổi giá trị khởi tạo thành đơn vị mặc định
                }}
            >
                <Row gutter={[32, 24]}>
                    {/* Cột 1: Tập trung các trường chính (Loại phiếu, Giá trị, Ngày/Giờ) */}
                    <Col xs={24} md={8}>
                        <Form.Item
                            name="loaiPhieu"
                            label="Loại Phiếu Giảm Giá"
                            tooltip="Chọn phạm vi áp dụng của phiếu"
                            rules={[{ required: true, message: "Vui lòng chọn loại phiếu" }]}
                        >
                            <Select placeholder="Chọn loại phiếu giảm giá" size="large">
                                <Option value="toanBo">Toàn bộ sản phẩm</Option>
                                <Option value="sanPham">Theo sản phẩm</Option>
                            </Select>
                        </Form.Item>

                        {/* GOM 2 TRƯỜNG NGÀY VÀO MỘT HÀNG CON */}
                        <Row gutter={16}>
                            {/* Ngày Bắt Đầu (Đã di chuyển) */}
                            <Col span={12}>
                                <Form.Item
                                    name="ngayBatDau"
                                    label="Ngày Bắt Đầu"
                                    rules={[{ required: true, message: "Chọn ngày bắt đầu" }]}
                                >
                                    <DatePicker
                                        format="DD/MM/YYYY"
                                        style={{ width: "100%" }}
                                        size="large"
                                        placeholder="Chọn ngày"
                                    />
                                </Form.Item>
                            </Col>
                            {/* Ngày Hết Hạn (Đã di chuyển) */}
                            <Col span={12}>
                                <Form.Item
                                    name="ngayHetHan"
                                    label="Ngày Hết Hạn"
                                    rules={[{ required: true, message: "Chọn ngày hết hạn" }]}
                                >
                                    <DatePicker
                                        format="DD/MM/YYYY"
                                        style={{ width: "100%" }}
                                        size="large"
                                        placeholder="Chọn ngày"
                                    />
                                </Form.Item>
                            </Col>
                        </Row>

                        <Form.Item
                            name="soTien"
                            label="Giá Trị Giảm Giá"
                            rules={[{ required: true, message: "Vui lòng nhập giá trị" }]}
                        >
                            {/* Đã xóa suffix="VND" để phù hợp với việc nhập loại giá trị tự do */}
                            <Input
                                placeholder="Ví dụ: 5000"
                                size="large"
                                type="number"
                            />
                        </Form.Item>
                    </Col>

                    {/* Cột 2: Thông tin chi tiết (Tiêu đề, Mua tối thiểu) */}
                    <Col xs={24} md={8}>
                        <Form.Item
                            name="tieuDe"
                            label="Tiêu Đề Phiếu Giảm Giá"
                            rules={[{ required: true, message: "Vui lòng nhập tiêu đề" }]}
                        >
                            <Input placeholder="Ví dụ: Mừng khai trương" size="large" />
                        </Form.Item>

                        <Form.Item
                            name="muaToiThieu"
                            label="Giá Trị Đơn Hàng Tối Thiểu"
                            tooltip="Đơn hàng phải đạt mức tối thiểu này để áp dụng mã"
                        >
                            <Input
                                placeholder="Ví dụ: 100000"
                                size="large"
                                suffix="VND"
                                type="number"
                            />
                        </Form.Item>
                    </Col>

                    {/* Cột 3: Mã và Loại giảm giá */}
                    <Col xs={24} md={8}>
                        <Form.Item label="Mã Giảm Giá" required>
                            <Input.Group compact size="large">
                                <Form.Item
                                    name="maGiamGia"
                                    noStyle
                                    rules={[{ required: true, message: "Vui lòng nhập mã" }]}
                                >
                                    <Input style={{ width: "calc(100% - 100px)" }} placeholder="Tạo hoặc nhập mã" />
                                </Form.Item>
                                <Button
                                    type="default"
                                    onClick={generateCode}
                                    style={{ width: "100px" }}
                                    icon={<TagsFilled />}
                                >
                                    Tạo mã
                                </Button>
                            </Input.Group>
                        </Form.Item>

                        {/* ĐIỂM THAY ĐỔI: Thay thế Select bằng Input cho trường loaiGiamGia */}
                        <Form.Item
                            name="loaiGiamGia"
                            label="Loại Giá Trị Giảm Giá"
                            tooltip="Nhập đơn vị hoặc loại giảm giá (ví dụ: VND, %, chiếc)"
                            rules={[{ required: true, message: "Vui lòng nhập loại giá trị" }]}
                        >
                            <Input
                                placeholder="Ví dụ: VND hoặc %"
                                size="large"
                            />
                        </Form.Item>
                        {/* END ĐIỂM THAY ĐỔI */}

                    </Col>
                </Row>

                <Row justify="end" style={{ marginTop: 40 }}>
                    <Col>
                        <Space size="middle">
                            <Button onClick={handleReset} icon={<ReloadOutlined />}>
                                Cài lại
                            </Button>
                            <Button type="primary" htmlType="submit" icon={<SendOutlined />}>
                                Tạo Phiếu
                            </Button>
                        </Space>
                    </Col>
                </Row>
            </Form>
        </Card>
    );
};

export default SellerDiscountForm;