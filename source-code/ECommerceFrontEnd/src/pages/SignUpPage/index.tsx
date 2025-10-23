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
import Footer from "../../layouts/Footer";
import Header from "../../layouts/Header";
import { MessageComponent } from "../../components/MessageComponent";
import MessageProvider from "../../components/MessageProvider";
import { ApiClient } from "../../api/ApiClient.ts";

const { Title, Text, Link } = Typography;

const apiClient = new ApiClient({
    BASE: "https://localhost:5001",
});
const authService = apiClient.authentication;

const SignUpPage: React.FC = () => {
    const [loading, setLoading] = useState(false);
    const [otpVisible, setOtpVisible] = useState(false);
    const [otpCode, setOtpCode] = useState("");
    const [registeredPhone, setRegisteredPhone] = useState("");
    const [form] = Form.useForm();

    /** Xử lý khi submit form đăng ký */
    const onFinish = async (values: any) => {
        setLoading(true);
        const payload = {
            phoneNumber: values.phone?.trim(),
            password: values.password?.trim(),
        };

        try {
            const response = await authService.postApiAuthenticationRegisterPhoneOtp({
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

    /** Xác thực OTP */
    const handleVerifyOtp = async () => {
        if (!otpCode) {
            MessageComponent.error("Vui lòng nhập mã OTP!");
            return;
        }

        setLoading(true);
        try {
            const verifyPayload = {
                phoneNumber: registeredPhone,
                otp: otpCode.trim(),
            };

            const response = await authService.postApiAuthenticationRegisterPhoneOtpVerify({
                requestBody: verifyPayload,
            });

            if (response?.success !== false) {
                MessageComponent.success("Đăng ký thành công!");
                setOtpVisible(false);
                setTimeout(() => {
                    window.location.href = "/login";
                }, 4000);
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

    /** Gửi lại OTP */
    const handleResendOtp = async () => {
        try {
            await authService.postApiAuthenticationRegisterPhoneOtpResend({
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
                background: "#f5f7fa",
            }}
        >
            <MessageProvider />

            {/* ✅ Phần nội dung trung tâm */}
            <Row
                justify="center"
                align="middle"
                style={{
                    flex: "1 0 auto",
                    padding: "40px 0",
                }}
            >
                <Col xs={22} sm={20} md={14} lg={10} xl={8}>
                    <Card style={{ borderRadius: 12, padding: "24px 36px" }}>
                        <div style={{ textAlign: "center", marginBottom: 24 }}>
                            <UserOutlined
                                style={{ fontSize: 48, color: "#1a73e8", marginBottom: 8 }}
                            />
                            <Title level={3} style={{ margin: 0 }}>
                                Đăng ký tài khoản
                            </Title>
                        </div>

                        <Form layout="vertical" onFinish={onFinish} form={form}>
                            <Form.Item
                                label="Số điện thoại"
                                name="phone"
                                rules={[
                                    { required: true, message: "Vui lòng nhập số điện thoại!" },
                                    {
                                        pattern: /^[0-9]{9,11}$/,
                                        message: "Số điện thoại phải có từ 9–11 chữ số!",
                                    },
                                ]}
                            >
                                <Input placeholder="Nhập số điện thoại của bạn" />
                            </Form.Item>

                            <Form.Item
                                label="Mật khẩu"
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
                                />
                            </Form.Item>

                            <Form.Item
                                label="Xác nhận mật khẩu"
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

                            <Button
                                type="primary"
                                htmlType="submit"
                                block
                                loading={loading}
                            >
                                {loading ? "Đang xử lý..." : "Đăng ký"}
                            </Button>
                        </Form>

                        <Divider style={{ margin: "32px 0 16px" }} />

                        <div style={{ textAlign: "center" }}>
                            <Text>Hoặc tiếp tục với</Text>
                            <Space direction="vertical" style={{ width: "100%", marginTop: 16 }}>
                                <Button icon={<GoogleOutlined />} block>
                                    Google
                                </Button>
                                <Button icon={<FacebookFilled />} block>
                                    Facebook
                                </Button>
                            </Space>
                        </div>

                        <div style={{ textAlign: "center", marginTop: 20 }}>
                            <Text>Đã có tài khoản? </Text>
                            <Link href="#">Đăng nhập ngay</Link>
                        </div>
                    </Card>
                </Col>
            </Row>



            {/* ✅ Modal nhập OTP */}
            <Modal
                title="Nhập mã OTP"
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
                />
            </Modal>
        </div>
    );
};

export default SignUpPage;
