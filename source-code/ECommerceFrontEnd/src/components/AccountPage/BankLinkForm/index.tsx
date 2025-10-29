import React from "react";
import {
    Form,
    Input,
    Button,
    Select,
    Row,
    Col,
    Typography,
    Card,
    Alert,
} from "antd";
import { CreditCardOutlined, LockOutlined } from "@ant-design/icons";

const { Title, Text } = Typography;
const { Option } = Select;

interface BankFormValues {
    cardNumber: string;
    cardName: string;
    expiry: string;
    cvv: string;
    bank: string;
}

interface BankLinkFormProps {
    onSuccess?: (values: BankFormValues) => void;
}

const BankLinkForm: React.FC<BankLinkFormProps> = ({ onSuccess }) => {
    const [form] = Form.useForm();

    const handleFinish = (values: BankFormValues) => {
        console.log("Dữ liệu liên kết:", values);
        if (onSuccess) onSuccess(values);
        form.resetFields();
    };

    return (
        <Card
            style={{
                borderRadius: 10,
                boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
            }}
        >
            <Title level={4} style={{ textAlign: "center", marginBottom: 24 }}>
                Liên Kết Thẻ Ngân Hàng
            </Title>

            <Form
                form={form}
                layout="vertical"
                onFinish={handleFinish}
                autoComplete="off"
            >
                <Form.Item
                    label="Số thẻ"
                    name="cardNumber"
                    rules={[
                        { required: true, message: "Vui lòng nhập số thẻ!" },
                        { len: 16, message: "Số thẻ phải gồm 16 chữ số!" },
                    ]}
                >
                    <Input
                        placeholder="1234 5678 9012 3456"
                        maxLength={16}
                        prefix={<CreditCardOutlined />}
                    />
                </Form.Item>

                <Form.Item
                    label="Tên chủ thẻ"
                    name="cardName"
                    rules={[{ required: true, message: "Vui lòng nhập tên chủ thẻ!" }]}
                >
                    <Input placeholder="NGUYEN VAN A" />
                </Form.Item>

                <Row gutter={12}>
                    <Col span={12}>
                        <Form.Item
                            label="Ngày hết hạn"
                            name="expiry"
                            rules={[{ required: true, message: "Vui lòng nhập ngày hết hạn!" }]}
                        >
                            <Input placeholder="MM/YY" maxLength={5} />
                        </Form.Item>
                    </Col>
                    <Col span={12}>
                        <Form.Item
                            label="CVV"
                            name="cvv"
                            rules={[
                                { required: true, message: "Vui lòng nhập CVV!" },
                                { len: 3, message: "CVV gồm 3 chữ số!" },
                            ]}
                        >
                            <Input.Password placeholder="123" maxLength={3} />
                        </Form.Item>
                    </Col>
                </Row>

                <Form.Item
                    label="Ngân hàng"
                    name="bank"
                    rules={[{ required: true, message: "Vui lòng chọn ngân hàng!" }]}
                >
                    <Select placeholder="Chọn ngân hàng">
                        <Option value="Vietcombank">Vietcombank</Option>
                        <Option value="Techcombank">Techcombank</Option>
                        <Option value="VPBank">VPBank</Option>
                        <Option value="ACB">ACB</Option>
                        <Option value="BIDV">BIDV</Option>
                    </Select>
                </Form.Item>

                <Alert
                    type="info"
                    showIcon
                    icon={<LockOutlined />}
                    message={
                        <span>
                            <b>Bảo mật:</b> Thông tin thẻ của bạn được mã hóa và bảo vệ an toàn.
                        </span>
                    }
                    style={{
                        backgroundColor: "#f7f9ff",
                        borderColor: "#d6e0ff",
                        marginBottom: 20,
                    }}
                />

                <Form.Item>
                    <Button type="primary" htmlType="submit" block>
                        Liên Kết Thẻ
                    </Button>
                </Form.Item>

                <Text type="secondary" style={{ fontSize: 12 }}>
                    Bằng cách liên kết thẻ, bạn đồng ý với{" "}
                    <a href="#">Điều khoản sử dụng</a> và{" "}
                    <a href="#">Chính sách bảo mật</a>.
                </Text>
            </Form>
        </Card>
    );
};

export default BankLinkForm;
