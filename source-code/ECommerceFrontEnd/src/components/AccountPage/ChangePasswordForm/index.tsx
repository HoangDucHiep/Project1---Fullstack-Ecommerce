import React, { useState } from "react";
import {
    Card,
    Form,
    Input,
    Button,
    Typography,
    Progress,
    Space,
    message,
} from "antd";
import {
    EyeInvisibleOutlined,
    EyeTwoTone,
    LockOutlined,
    ReloadOutlined,
} from "@ant-design/icons";

const { Title, Text } = Typography;

// 1. ĐỊNH NGHĨA INTERFACE RÕ RÀNG CHO DỮ LIỆU FORM
interface PasswordFormValues {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
    captcha: string;
}

const ChangePasswordForm: React.FC = () => {
    const [form] = Form.useForm();
    const [captcha, setCaptcha] = useState(generateCaptcha());
    const [passwordStrength, setPasswordStrength] = useState(0);

    function generateCaptcha(): string {
        const chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        let text = "";
        for (let i = 0; i < 6; i++) text += chars[Math.floor(Math.random() * chars.length)];
        return text;
    }

    const refreshCaptcha = () => {
        setCaptcha(generateCaptcha());
    };

    const checkPasswordStrength = (value: string) => {
        if (!value) return setPasswordStrength(0);
        let strength = 0;
        if (value.length >= 8) strength += 30;
        if (/[A-Z]/.test(value)) strength += 20;
        if (/[0-9]/.test(value)) strength += 20;
        if (/[^A-Za-z0-9]/.test(value)) strength += 30;
        setPasswordStrength(strength);
    };

    // 2. SỬ DỤNG INTERFACE THAY CHO `any`
    const handleFinish = (values: PasswordFormValues) => {
        if (values.captcha.toLowerCase() !== captcha.toLowerCase()) { // Thêm .toLowerCase() để Captcha không phân biệt chữ hoa/thường (tùy chọn)
            message.error("Mã xác thực không đúng!");
            refreshCaptcha(); // Tải lại Captcha khi nhập sai
            return;
        }

        // Logic gửi API cập nhật mật khẩu ở đây
        // console.log("Gửi dữ liệu đổi mật khẩu:", values.newPassword);

        message.success("Cập nhật mật khẩu thành công!");
        form.resetFields();
        setCaptcha(generateCaptcha());
        setPasswordStrength(0);
    };

    return (
        <Card
            style={{
                width: 400,
                margin: "0 auto",
                borderRadius: 10,
                boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
                padding: 24,
            }}
        >
            <Title level={3} style={{ textAlign: "center" }}>
                Đổi Mật Khẩu
            </Title>
            <Text type="secondary" style={{ display: "block", textAlign: "center", marginBottom: 24 }}>
                Cập nhật mật khẩu bảo mật của bạn
            </Text>

            <Form
                layout="vertical"
                form={form}
                onFinish={handleFinish}
                autoComplete="off"
            >
                {/* Mật khẩu hiện tại */}
                <Form.Item
                    label="Mật khẩu hiện tại *"
                    name="currentPassword"
                    rules={[{ required: true, message: "Vui lòng nhập mật khẩu hiện tại!" }]}
                >
                    <Input.Password
                        placeholder="Nhập mật khẩu hiện tại"
                        iconRender={(visible) =>
                            visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                        }
                    />
                </Form.Item>

                {/* Mật khẩu mới */}
                <Form.Item
                    label="Mật khẩu mới *"
                    name="newPassword"
                    rules={[
                        { required: true, message: "Vui lòng nhập mật khẩu mới!" },
                        { min: 8, message: "Mật khẩu phải có ít nhất 8 ký tự!" },
                    ]}
                >
                    <Input.Password
                        placeholder="Nhập mật khẩu mới"
                        onChange={(e) => checkPasswordStrength(e.target.value)}
                        iconRender={(visible) =>
                            visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                        }
                    />
                </Form.Item>

                {/* Độ mạnh mật khẩu */}
                {passwordStrength > 0 && (
                    <div style={{ marginBottom: 16 }}>
                        <Progress
                            percent={passwordStrength}
                            showInfo={false}
                            strokeColor={
                                passwordStrength < 40
                                    ? "red"
                                    : passwordStrength < 70
                                        ? "orange"
                                        : "green"
                            }
                        />
                        <Text type="secondary" style={{ fontSize: 12 }}>
                            Độ mạnh mật khẩu
                        </Text>
                    </div>
                )}

                {/* Xác nhận mật khẩu mới */}
                <Form.Item
                    label="Xác nhận mật khẩu mới *"
                    name="confirmPassword"
                    dependencies={["newPassword"]}
                    rules={[
                        { required: true, message: "Vui lòng nhập lại mật khẩu mới!" },
                        ({ getFieldValue }) => ({
                            validator(_, value) {
                                if (!value || getFieldValue("newPassword") === value) {
                                    return Promise.resolve();
                                }
                                return Promise.reject("Mật khẩu xác nhận không khớp!");
                            },
                        }),
                    ]}
                >
                    <Input.Password
                        placeholder="Nhập lại mật khẩu mới"
                        iconRender={(visible) =>
                            visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                        }
                    />
                </Form.Item>

                {/* Captcha */}
                <Form.Item
                    label="Mã xác thực *"
                    // 🚨 ĐẶT TÊN `name` CHO INPUT BÊN TRONG!
                    // Nếu không đặt name ở đây, form không nhận giá trị input
                    // Tên name của Form.Item là 'captcha', nhưng INPUT bên trong cần có name riêng
                    // 💡 LƯU Ý: Ant Design Form sẽ tự động liên kết Input trong Form.Item (nếu là con duy nhất).
                    // Tuy nhiên, vì bạn dùng Space, Input không còn là con trực tiếp duy nhất,
                    // nên cần đặt Input bên trong Form.Item riêng, hoặc dùng `Input.Group`.
                    // Cách đơn giản nhất: Đặt `name="captcha"` cho Form.Item bao quanh Input
                    name="captcha"
                    rules={[{ required: true, message: "Vui lòng nhập mã xác thực!" }]}
                >
                    <Space direction="vertical" style={{ width: '100%' }}>
                        <Space>
                            <div
                                style={{
                                    background: `repeating-linear-gradient(
                                        45deg,
                                        #f0f2ff,
                                        #f0f2ff 10px,
                                        #e0e5ff 10px,
                                        #e0e5ff 20px
                                    )`,
                                    border: "1px solid #ccc",
                                    borderRadius: 6,
                                    padding: "6px 12px",
                                    letterSpacing: 4,
                                    fontWeight: "bold",
                                    color: "#555",
                                    userSelect: "none",
                                    fontFamily: "monospace",
                                }}
                            >
                                {captcha.split("").map((c, i) => (
                                    <span
                                        key={i}
                                        style={{
                                            color: `hsl(${Math.random() * 360}, 70%, 40%)`,
                                            transform: `rotate(${Math.random() * 20 - 10}deg)`,
                                            display: "inline-block",
                                        }}
                                    >
                                        {c}
                                    </span>
                                ))}
                            </div>
                            <Button
                                icon={<ReloadOutlined />}
                                onClick={refreshCaptcha}
                                style={{ borderRadius: 6 }}
                            />
                        </Space>
                        {/* 👈 INPUT cần thiết để form lấy giá trị */}
                        <Input placeholder="Nhập mã xác thực" />
                    </Space>
                </Form.Item>

                {/* Nút cập nhật */}
                <Form.Item>
                    <Button
                        type="primary"
                        htmlType="submit"
                        block
                        icon={<LockOutlined />}
                        style={{
                            background: "linear-gradient(90deg, #5B86E5, #36D1DC)",
                            border: "none",
                            height: 40,
                            borderRadius: 8,
                        }}
                    >
                        Cập nhật mật khẩu
                    </Button>
                </Form.Item>
            </Form>
        </Card>
    );
};

export default ChangePasswordForm;