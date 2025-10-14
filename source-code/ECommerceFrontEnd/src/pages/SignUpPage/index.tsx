import React from "react";
import {
    Button,
    Checkbox,
    Col,
    Form,
    Input,
    Row,
    Typography,
    Card,
    Select,
    Divider,
    Space,
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

const { Title, Text, Link } = Typography;
const { Option } = Select;
interface SignUpFormValues {
    username: string;
    email: string;
    password: string;
}
const SignUpPage: React.FC = () => {
    const onFinish = (values: SignUpFormValues) => {
        console.log("Form values:", values);
    };

    return (
        <>
        <Header />
        <Row
            justify="center"
            align="middle"
            style={{ height: "100vh", background: "#f5f7fa", margin: 20 }}
        >
            <Col xs={22} sm={20} md={16} lg={12} xl={10}>
                <Card style={{ borderRadius: 12, padding: "24px 36px" }}>
                    <div style={{ textAlign: "center", marginBottom: 24 }}>
                        <UserOutlined
                            style={{ fontSize: 48, color: "#1a73e8", marginBottom: 8 }}
                        />
                        <Title level={3} style={{ margin: 0 }}>
                            Sign Up
                        </Title>
                    </div>

                    <Form layout="vertical" onFinish={onFinish}>
                        <Row gutter={16}>
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="First Name"
                                    name="firstName"
                                    rules={[{ required: true, message: "Please enter first name" }]}
                                >
                                    <Input placeholder="Ex: John" />
                                </Form.Item>
                            </Col>

                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Last Name"
                                    name="lastName"
                                    rules={[{ required: true, message: "Please enter last name" }]}
                                >
                                    <Input placeholder="Ex: Doe" />
                                </Form.Item>
                            </Col>
                        </Row>

                        <Row gutter={16}>
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Email Address"
                                    name="email"
                                    rules={[
                                        { required: true, message: "Please enter your email" },
                                        { type: "email", message: "Invalid email format" },
                                    ]}
                                >
                                    <Input placeholder="Enter email address" />
                                </Form.Item>
                            </Col>

                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Phone Number"
                                    name="phone"
                                    rules={[
                                        { required: true, message: "Please enter your phone number" },
                                    ]}
                                >
                                    <Input
                                        addonBefore={
                                            <Select defaultValue="+1" style={{ width: 80 }}>
                                                <Option value="+1">+1</Option>
                                                <Option value="+84">+84</Option>
                                                <Option value="+44">+44</Option>
                                            </Select>
                                        }
                                        placeholder="Enter phone number"
                                    />
                                </Form.Item>
                            </Col>
                        </Row>

                        <Row gutter={16}>
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Password"
                                    name="password"
                                    rules={[
                                        { required: true, message: "Please enter your password" },
                                        { min: 8, message: "Minimum 8 characters long" },
                                    ]}
                                >
                                    <Input.Password
                                        placeholder="Minimum 8 characters long"
                                        iconRender={(visible) =>
                                            visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                                        }
                                    />
                                </Form.Item>
                            </Col>

                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Confirm Password"
                                    name="confirmPassword"
                                    dependencies={["password"]}
                                    rules={[
                                        { required: true, message: "Please confirm your password" },
                                        ({ getFieldValue }) => ({
                                            validator(_, value) {
                                                if (!value || getFieldValue("password") === value) {
                                                    return Promise.resolve();
                                                }
                                                return Promise.reject(
                                                    new Error("Passwords do not match!")
                                                );
                                            },
                                        }),
                                    ]}
                                >
                                    <Input.Password
                                        placeholder="Minimum 8 characters long"
                                        iconRender={(visible) =>
                                            visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                                        }
                                    />
                                </Form.Item>
                            </Col>
                        </Row>

                        <Form.Item
                            name="agree"
                            valuePropName="checked"
                            rules={[
                                {
                                    validator: (_, value) =>
                                        value
                                            ? Promise.resolve()
                                            : Promise.reject(
                                                new Error("You must agree to terms and conditions")
                                            ),
                                },
                            ]}
                        >
                            <Checkbox>
                                I agree to Your{" "}
                                <Link href="#" target="_blank">
                                    Terms and Condition
                                </Link>
                            </Checkbox>
                        </Form.Item>

                        <Button type="primary" htmlType="submit" block>
                            Sign up
                        </Button>
                    </Form>

                    <Divider style={{ margin: "32px 0 16px" }} />

                    <div style={{ textAlign: "center" }}>
                        <Text>Or continue with</Text>
                        <Space direction="vertical" style={{ width: "100%", marginTop: 16 }}>
                            <Button icon={<GoogleOutlined />} block>
                                Google
                            </Button>
                            <Button icon={<FacebookFilled />} block>
                                Facebook
                            </Button>
                        </Space>
                    </div>
                    <div style={{ textAlign: "center", marginTop:20 }}>
                        <Text>Have a account? </Text>
                        <Link href="#">Sign In</Link>
                    </div>
                </Card>
            </Col>
        </Row>
        <Footer />
        </>
    );
};

export default SignUpPage;
