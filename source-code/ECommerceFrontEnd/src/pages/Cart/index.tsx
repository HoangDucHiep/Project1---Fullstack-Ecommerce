import { useState } from "react";
import { Card, Row, Col, Checkbox, Image, InputNumber, Button, Select, Input, Divider } from "antd";
import { DeleteOutlined, TruckOutlined, CreditCardOutlined, ReloadOutlined, SafetyCertificateOutlined } from "@ant-design/icons";

const { Option } = Select;

export default function ShoppingCart() {
    const [quantity, setQuantity] = useState(1);

    const unitPrice = 1149.0;
    const subtotal = unitPrice * quantity;
    const shipping = 0;
    const discount = 0;
    const tax = subtotal * 0.15; // demo tax 15%
    const total = subtotal + shipping - discount + tax;

    return (
        <Row gutter={24} style={{ padding: 24, background: "#f9f9f9" }}>
            {/* Left Section */}
            <Col span={16}>
                <h2 style={{ fontWeight: "bold", marginBottom: 16 }}>Shopping cart</h2>

                <Card>
                    <Checkbox defaultChecked>
                        <span style={{ fontWeight: 500 }}>Bicycle Shop</span>
                    </Checkbox>

                    <Divider style={{ margin: "12px 0" }} />

                    <Row align="middle" gutter={16}>
                        <Col>
                            <Checkbox defaultChecked />
                        </Col>
                        <Col>
                            <Image
                                src="https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-14-pro-max-purple"
                                alt="iPhone 14 Pro Max"
                                width={80}
                                preview={false}
                            />
                        </Col>
                        <Col flex="auto">
                            <div style={{ fontWeight: 500 }}>iPhone 14 Pro Max</div>
                            <div style={{ color: "#666" }}>Variant : Purple</div>
                            <div style={{ marginTop: 8, color: "#1677ff" }}>
                                <TruckOutlined /> You Get Free Delivery Bonus
                            </div>
                        </Col>
                        <Col>
                            <div style={{ fontWeight: 500 }}>${unitPrice.toFixed(2)}</div>
                        </Col>
                        <Col>
                            <Row align="middle" gutter={8}>
                                <Col>
                                    <Button
                                        type="text"
                                        icon={<DeleteOutlined style={{ color: "red" }} />}
                                    />
                                </Col>
                                <Col>
                                    <InputNumber
                                        min={1}
                                        value={quantity}
                                        onChange={(val) => setQuantity(val || 1)}
                                    />
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <div style={{ fontWeight: 500 }}>${subtotal.toFixed(2)}</div>
                        </Col>
                    </Row>
                </Card>

                <Card style={{ marginTop: 16 }}>
                    <Select defaultValue="Choose shipping method" style={{ width: "100%" }}>
                        <Option value="standard">Standard Shipping</Option>
                        <Option value="express">Express Shipping</Option>
                    </Select>
                    <Input.TextArea
                        placeholder="Order Note (Optional)"
                        rows={3}
                        style={{ marginTop: 12 }}
                    />
                </Card>
            </Col>

            {/* Right Section */}
            <Col span={8}>
                <Card>
                    <Row justify="space-between" style={{ marginBottom: 8 }}>
                        <Col>Sub total</Col>
                        <Col>${subtotal.toFixed(2)}</Col>
                    </Row>
                    <Row justify="space-between" style={{ marginBottom: 8 }}>
                        <Col>Shipping</Col>
                        <Col>${shipping.toFixed(2)}</Col>
                    </Row>
                    <Row justify="space-between" style={{ marginBottom: 8 }}>
                        <Col>Discount on product</Col>
                        <Col>- ${discount.toFixed(2)}</Col>
                    </Row>
                    <Row justify="space-between" style={{ marginBottom: 12 }}>
                        <Col>Tax</Col>
                        <Col>${tax.toFixed(2)}</Col>
                    </Row>

                    <Divider style={{ margin: "12px 0" }} />

                    <Row justify="space-between" style={{ fontWeight: "bold", fontSize: 16, marginBottom: 16 }}>
                        <Col>Total</Col>
                        <Col>${total.toFixed(2)}</Col>
                    </Row>

                    <Row justify="space-around" style={{ marginBottom: 16 }}>
                        <Col style={{ textAlign: "center" }}>
                            <TruckOutlined style={{ fontSize: 20 }} />
                            <div style={{ fontSize: 12 }}>Fast Delivery</div>
                        </Col>
                        <Col style={{ textAlign: "center" }}>
                            <CreditCardOutlined style={{ fontSize: 20 }} />
                            <div style={{ fontSize: 12 }}>Safe Payment</div>
                        </Col>
                        <Col style={{ textAlign: "center" }}>
                            <ReloadOutlined style={{ fontSize: 20 }} />
                            <div style={{ fontSize: 12 }}>7 Days Return</div>
                        </Col>
                        <Col style={{ textAlign: "center" }}>
                            <SafetyCertificateOutlined style={{ fontSize: 20 }} />
                            <div style={{ fontSize: 12 }}>100% Authentic</div>
                        </Col>
                    </Row>

                    <Button type="primary" block style={{ marginBottom: 12 }}>
                        Proceed to Checkout
                    </Button>
                    <Button type="link" block>
                        &lt; Continue Shopping
                    </Button>
                </Card>
            </Col>
        </Row>
    );
}

