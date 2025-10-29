import React, { useState } from "react";
import {
    Form,
    Input,
    Button,
    Radio,
    Upload,
    Row,
    Col,
    Typography,
    message,
    DatePicker,
    Avatar, // 👈 Import Avatar
} from "antd";
import { UploadOutlined, UserOutlined } from "@ant-design/icons";
import dayjs, { Dayjs } from "dayjs";

const { Title, Text } = Typography;

interface ProfileFormValues {
    username: string;
    fullName: string;
    email: string;
    phone: string;
    address: string;
    gender: string;
    birthday: Dayjs | null;
}
interface ProfileFormProps {
    onAvatarChange?: (newUrl: string) => void;
}

const ProfileForm: React.FC<ProfileFormProps>= () => {
    const [form] = Form.useForm();
    const [gender, setGender] = useState("Nam");
    // 1. STATE MỚI: Lưu URL của ảnh đại diện
    const [avatarUrl, setAvatarUrl] = useState<string | null>(null);

    // URL ảnh mặc định (hoặc ảnh placeholder nếu chưa có)
    const DEFAULT_AVATAR = "https://i.pravatar.cc/150?img=60";

    const handleFinish = (values: ProfileFormValues) => {
        const dateOfBirth = values.birthday ? values.birthday.format("YYYY-MM-DD") : null;

        console.log("Dữ liệu gửi:", { ...values, birthday: dateOfBirth, avatarUrl });
        message.success("Lưu thông tin hồ sơ thành công!");
    };

    const uploadProps = {
        // 2. CẬP NHẬT beforeUpload: Tạo URL từ file
        beforeUpload: (file: File) => {
            const isJpgOrPng = file.type === "image/jpeg" || file.type === "image/png";
            if (!isJpgOrPng) {
                message.error("Chỉ chấp nhận file JPG hoặc PNG!");
                return Upload.LIST_IGNORE;
            }
            const isLt1M = file.size / 1024 / 1024 < 1;
            if (!isLt1M) {
                message.error("Dung lượng file phải nhỏ hơn 1MB!");
                return Upload.LIST_IGNORE;
            }

            // Tạo URL tạm thời cho ảnh được chọn để hiển thị ngay lập tức
            const reader = new FileReader();
            reader.onload = (e) => {
                if (e.target?.result) {
                    setAvatarUrl(e.target.result as string);
                }
            };
            reader.readAsDataURL(file);

            return false; // Ngăn chặn upload tự động của Ant Design
        },
    };

    return (
        <div style={{ background: "#fff", padding: 24, borderRadius: 8 }}>
            <Title level={4}>Hồ Sơ Của Tôi</Title>
            <Text type="secondary">
                Quản lý thông tin hồ sơ để bảo mật tài khoản
            </Text>

            <Form
                form={form}
                layout="vertical"
                style={{ marginTop: 24 }}
                onFinish={handleFinish}
                initialValues={{
                    username: "doaz123tt",
                    fullName: "Nguyễn Văn A",
                    email: "doaz123tt@gmail.com",
                    phone: "",
                    address: "",
                    gender: "Nam",
                    birthday: dayjs("2004-01-01"),
                } as ProfileFormValues}
            >
                <Row gutter={40}>
                    {/* Cột trái: Form fields */}
                    <Col span={16}>
                        {/* ... (Các trường nhập giữ nguyên) */}
                        <Form.Item label="Tên đăng nhập" name="username">
                            <Input disabled />
                        </Form.Item>

                        <Form.Item label="Họ và Tên" name="fullName" rules={[{ required: true, message: 'Vui lòng nhập Họ và Tên của bạn!' }]}>
                            <Input placeholder="Nhập Họ và Tên" />
                        </Form.Item>

                        <Form.Item
                            label="Email"
                            name="email"
                            rules={[
                                { required: true, message: 'Vui lòng nhập Email!' },
                                { type: 'email', message: 'Email không hợp lệ!' }
                            ]}
                        >
                            <Input placeholder="Nhập Email của bạn" />
                        </Form.Item>

                        <Form.Item label="Số điện thoại" name="phone">
                            <Input placeholder="Thêm số điện thoại" addonAfter={<a href="#">Thêm</a>} />
                        </Form.Item>

                        <Form.Item label="Địa chỉ" name="address">
                            <Input placeholder="Nhập địa chỉ của bạn" addonAfter={<a href="#">Thay Đổi</a>} />
                        </Form.Item>

                        <Form.Item label="Giới tính" name="gender">
                            <Radio.Group
                                onChange={(e) => setGender(e.target.value)}
                                value={gender}
                            >
                                <Radio value="Nam">Nam</Radio>
                                <Radio value="Nữ">Nữ</Radio>
                                <Radio value="Khác">Khác</Radio>
                            </Radio.Group>
                        </Form.Item>

                        <Form.Item label="Ngày sinh" name="birthday">
                            <DatePicker
                                format="DD/MM/YYYY"
                                style={{ width: "100%" }}
                                placeholder="Chọn ngày sinh"
                            />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit">
                                Lưu
                            </Button>
                        </Form.Item>
                    </Col>

                    {/* Cột phải: Ảnh đại diện & Upload */}
                    <Col
                        span={8}
                        style={{
                            display: "flex",
                            flexDirection: "column",
                            alignItems: "center",
                            justifyContent: "center",
                            borderLeft: "1px solid #f0f0f0",
                        }}
                    >
                        {/* THAY THẾ Icon bằng Avatar có ảnh */}
                        <Avatar
                            size={80} // Kích thước bằng với icon cũ (80px)
                            icon={!avatarUrl ? <UserOutlined /> : undefined} // Icon mặc định nếu chưa có ảnh
                            src={avatarUrl || DEFAULT_AVATAR} // Sử dụng ảnh đã upload hoặc ảnh mặc định
                            style={{ marginBottom: 10 }}
                        />

                        <Upload {...uploadProps} showUploadList={false}>
                            <Button
                                icon={<UploadOutlined />}
                                style={{ marginTop: 12 }}
                                size="small"
                            >
                                Chọn Ảnh
                            </Button>
                        </Upload>
                        <Text type="secondary" style={{ fontSize: 12, marginTop: 6 }}>
                            Dung lượng tối đa 1MB<br />Định dạng: JPEG, PNG
                        </Text>
                    </Col>
                </Row>
            </Form>
        </div>
    );
};

export default ProfileForm;