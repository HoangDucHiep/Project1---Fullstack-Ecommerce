import React, { useState } from "react";
import {
    Button,
    Checkbox,
    Col,
    Form,
    Input,
    Row,
    Typography,
    Card,
    Divider,
    Space,
    Modal,
} from "antd";
import {
    EyeInvisibleOutlined,
    EyeTwoTone,
    GoogleOutlined,
    FacebookFilled,
    UserOutlined,
} from "@ant-design/icons";
import { MessageComponent } from "../../components/MessageComponent";
import MessageProvider from "../../components/MessageProvider";
import { ApiClient } from "../../api/ApiClient.ts";

const { Title, Text, Link } = Typography;
const apiClient = new ApiClient({ BASE: "https://localhost:5001" });
const authService = apiClient.authentication;

const SignUpPage: React.FC = () => {
    const [loading, setLoading] = useState(false);
    const [otpVisible, setOtpVisible] = useState(false);
    const [otpCode, setOtpCode] = useState("");
    const [registeredPhone, setRegisteredPhone] = useState("");
    const [form] = Form.useForm();

    /** ✅ Giữ nguyên logic đăng ký */
    const onFinish = async (values: any) => {
        setLoading(true);
        const payload = {
            phoneNumber: values.phone?.trim(),
            password: values.password?.trim(),
        };
        try {
            const response = await authService.postApiV1AuthRegisterPhoneOtp({
                requestBody: payload,
            });
            if (response?.success !== false) {
                MessageComponent.success("Mã OTP đã được gửi đến số điện thoại của bạn!");
                setRegisteredPhone(payload.phoneNumber);
                setOtpVisible(true);
            } else {
                MessageComponent.error(response?.message || "Đăng ký thất bại!");
            }
        } catch (error: any) {
            const status = error.response?.status;
            console.error("Registration error:", error);
            if (status === 409 || status === 400) {
                MessageComponent.error("Tài khoản đã tồn tại!");
            } else if (status === 500) {
                MessageComponent.error("Lỗi máy chủ, vui lòng thử lại sau!");
            } else if (error.code === "ERR_NETWORK") {
                MessageComponent.error("Không thể kết nối đến máy chủ!");
            } else {
                MessageComponent.error("Đăng ký thất bại, vui lòng thử lại!");
            }
        } finally {
            setLoading(false);
        }
    };

    /** ✅ Giữ nguyên logic xác thực OTP */
    const handleVerifyOtp = async () => {
        if (!otpCode) {
            MessageComponent.error("Vui lòng nhập mã OTP!");
            return;
        }
        setLoading(true);
        try {
            const verifyPayload = { phoneNumber: registeredPhone, otp: otpCode.trim() };
            const response = await authService.postApiV1AuthRegisterPhoneOtpVerify({
                requestBody: verifyPayload,
            });
            if (response?.success !== false) {
                MessageComponent.success("Đăng ký thành công!");
                setOtpVisible(false);
                setTimeout(() => (window.location.href = "/login"), 4000);
            } else {
                MessageComponent.error(response?.message || "Mã OTP không hợp lệ!");
            }
        } catch (error: any) {
            const status = error.response?.status;
            console.error("OTP verify error:", error);
            if (status === 400) {
                MessageComponent.error("Mã OTP không hợp lệ hoặc đã hết hạn!");
            } else if (status === 500) {
                MessageComponent.error("Lỗi máy chủ, vui lòng thử lại sau!");
            } else if (error.code === "ERR_NETWORK") {
                MessageComponent.error("Không thể kết nối đến máy chủ!");
            } else {
                MessageComponent.error("Xác thực OTP thất bại!");
            }
        } finally {
            setLoading(false);
        }
    };

    const handleResendOtp = async () => {
        try {
            await authService.postApiV1AuthRegisterPhoneOtpResend({
                requestBody: { phoneNumber: registeredPhone },
            });
            MessageComponent.success("Đã gửi lại mã OTP!");
        } catch {
            MessageComponent.error("Không thể gửi lại mã OTP!");
        }
    };

    return (
        <div
            style={{
                display: "flex",
                flexDirection: "column",
                minHeight: "100vh",
                /** 🎨 Thay đổi màu nền — gradient sang trọng */
                background: "linear-gradient(135deg, #e3f2fd 0%, #bbdefb 50%, #90caf9 100%)",
            }}
        >
            <MessageProvider />

            {/* 🎨 Card trung tâm với hiệu ứng đổ bóng, bo góc lớn */}
            <Row justify="center" align="middle" style={{ flex: "1 0 auto", padding: "40px 0" }}>
                <Col xs={22} sm={20} md={14} lg={10} xl={8}>
                    <Card
                        style={{
                            borderRadius: 20,
                            padding: "36px 40px",
                            boxShadow: "0 8px 20px rgba(0,0,0,0.15)",
                            background: "white",
                        }}
                    >
                        {/* 🎨 Header gọn gàng và hiện đại */}
                        <div style={{ textAlign: "center", marginBottom: 24 }}>
                            <UserOutlined
                                style={{
                                    fontSize: 54,
                                    color: "#1976d2",
                                    marginBottom: 12,
                                    background: "#e3f2fd",
                                    padding: 16,
                                    borderRadius: "50%",
                                }}
                            />
                            <Title level={3} style={{ margin: 0, color: "#0d47a1" }}>
                                Đăng ký tài khoản
                            </Title>
                        </div>

                        <Form layout="vertical" onFinish={onFinish} form={form}>
                            {/* 🎨 Ô nhập số điện thoại */}
                            <Form.Item
                                label={<strong>Số điện thoại</strong>}
                                name="phone"
                                rules={[
                                    { required: true, message: "Vui lòng nhập số điện thoại!" },
                                    {
                                        pattern: /^[0-9]{9,11}$/,
                                        message: "Số điện thoại phải có từ 9–11 chữ số!",
                                    },
                                ]}
                            >
                                <Input
                                    placeholder="Nhập số điện thoại của bạn"
                                    style={{ borderRadius: 8, padding: "10px 12px" }}
                                />
                            </Form.Item>

                            {/* 🎨 Ô nhập mật khẩu */}
                            <Form.Item
                                label={<strong>Mật khẩu</strong>}
                                name="password"
                                rules={[
                                    { required: true, message: "Vui lòng nhập mật khẩu!" },
                                    { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
                                ]}
                            >
                                <Input.Password
                                    placeholder="Nhập mật khẩu (ít nhất 6 ký tự)"
                                    iconRender={(visible) =>
                                        visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                                    }
                                    style={{ borderRadius: 8, padding: "10px 12px" }}
                                />
                            </Form.Item>

                            {/* 🎨 Ô nhập lại mật khẩu */}
                            <Form.Item
                                label={<strong>Xác nhận mật khẩu</strong>}
                                name="confirmPassword"
                                dependencies={["password"]}
                                rules={[
                                    { required: true, message: "Vui lòng nhập lại mật khẩu!" },
                                    ({ getFieldValue }) => ({
                                        validator(_, value) {
                                            if (!value || getFieldValue("password") === value) {
                                                return Promise.resolve();
                                            }
                                            return Promise.reject(
                                                new Error("Mật khẩu xác nhận không khớp!")
                                            );
                                        },
                                    }),
                                ]}
                            >
                                <Input.Password
                                    placeholder="Nhập lại mật khẩu"
                                    iconRender={(visible) =>
                                        visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                                    }
                                    style={{ borderRadius: 8, padding: "10px 12px" }}
                                />
                            </Form.Item>

                            <Form.Item
                                name="agree"
                                valuePropName="checked"
                                rules={[
                                    {
                                        validator: (_, value) =>
                                            value
                                                ? Promise.resolve()
                                                : Promise.reject(
                                                    new Error("Vui lòng đồng ý với điều khoản sử dụng!")
                                                ),
                                    },
                                ]}
                            >
                                <Checkbox>
                                    Tôi đồng ý với{" "}
                                    <Link href="#" target="_blank">
                                        Điều khoản và chính sách
                                    </Link>
                                </Checkbox>
                            </Form.Item>

                            {/* 🎨 Nút đăng ký có hiệu ứng hover */}
                            <Button
                                type="primary"
                                htmlType="submit"
                                block
                                loading={loading}
                                style={{
                                    background: "#1976d2",
                                    borderRadius: 8,
                                    padding: "10px 0",
                                    fontWeight: 600,
                                    transition: "0.3s",
                                }}
                            >
                                {loading ? "Đang xử lý..." : "Đăng ký"}
                            </Button>
                        </Form>

                        <Divider style={{ margin: "32px 0 16px" }}>Hoặc</Divider>

                        {/* 🎨 Nút Google & Facebook giống thật */}
                        <Space direction="vertical" style={{ width: "100%" }}>
                            <Button
                                icon={<GoogleOutlined style={{ color: "#db4437" }} />}
                                block
                                style={{
                                    borderRadius: 8,
                                    fontWeight: 500,
                                    height: 44,
                                    borderColor: "#dadce0",
                                    background: "white",
                                }}
                            >
                                Đăng ký với Google
                            </Button>
                            <Button
                                icon={<FacebookFilled style={{ color: "#1877f2" }} />}
                                block
                                style={{
                                    borderRadius: 8,
                                    fontWeight: 500,
                                    height: 44,
                                    background: "#e8f0fe",
                                    border: "1px solid #c5d6f8",
                                }}
                            >
                                Đăng ký với Facebook
                            </Button>
                        </Space>

                        <div style={{ textAlign: "center", marginTop: 24 }}>
                            <Text>Đã có tài khoản? </Text>
                            <Link href="/login">Đăng nhập ngay</Link>
                        </div>
                    </Card>
                </Col>
            </Row>

            {/* ✅ Modal nhập OTP giữ nguyên logic, thêm chút UI mềm mại */}
            <Modal
                title={<span style={{ fontWeight: 600 }}>Nhập mã OTP</span>}
                open={otpVisible}
                onOk={handleVerifyOtp}
                onCancel={() => setOtpVisible(false)}
                okText="Xác nhận"
                confirmLoading={loading}
                cancelText="Hủy"
                footer={[
                    <Button key="resend" onClick={handleResendOtp}>
                        Gửi lại mã OTP
                    </Button>,
                    <Button key="cancel" onClick={() => setOtpVisible(false)}>
                        Hủy
                    </Button>,
                    <Button
                        key="verify"
                        type="primary"
                        loading={loading}
                        onClick={handleVerifyOtp}
                        style={{ background: "#1976d2" }}
                    >
                        Xác nhận OTP
                    </Button>,
                ]}
            >
                <Input
                    placeholder="Nhập mã OTP gồm 6 chữ số"
                    value={otpCode}
                    onChange={(e) => setOtpCode(e.target.value)}
                    maxLength={6}
                    style={{
                        borderRadius: 8,
                        padding: "10px 12px",
                        fontSize: 16,
                        textAlign: "center",
                        letterSpacing: 4,
                    }}
                />
            </Modal>
        </div>
    );
};

export default SignUpPage;
