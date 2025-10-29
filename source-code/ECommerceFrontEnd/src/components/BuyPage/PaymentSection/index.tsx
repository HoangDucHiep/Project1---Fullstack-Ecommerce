import React, { useState, useMemo } from "react";
import { Row, Col, Typography, Divider, Button, Modal, Radio } from "antd";
import BankManagementPage from "../../AccountPage/BankManagementPage" // 👉 import trang quản lý thẻ

const { Text } = Typography;

export type PaymentMethod = "cod" | "bank" | "wallet";

export interface PaymentProps {
    itemTotal: number;
    shippingFee: number;
    voucherDiscount: number;
    onOrder?: (total: number, method: PaymentMethod) => void;
}

const formatVND = (value?: number) =>
    (value ?? 0)
        .toLocaleString("vi-VN", { style: "currency", currency: "VND" })
        .replace(",00", "");

const PaymentMethodSection: React.FC<PaymentProps> = ({
    itemTotal = 0,
    shippingFee = 0,
    voucherDiscount = 0,
    onOrder,
}) => {
    const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("cod");
    const [openModal, setOpenModal] = useState(false);
    const [showBankModal, setShowBankModal] = useState(false); // 👉 thêm modal quản lý thẻ

    const totalPayment = useMemo(
        () => Math.max(0, itemTotal + shippingFee - voucherDiscount),
        [itemTotal, shippingFee, voucherDiscount]
    );

    const handleConfirmOrder = () => {
        onOrder?.(totalPayment, paymentMethod);
    };

    // 👉 Khi bấm xác nhận trong modal chọn phương thức
    const handleSelectPayment = () => {
        setOpenModal(false);
        if (paymentMethod === "bank") {
            // Nếu chọn "Chuyển khoản ngân hàng" thì mở form quản lý thẻ
            setShowBankModal(true);
        }
    };

    return (
        <div
            style={{
                background: "#fff",
                border: "1px solid #f0f0f0",
                borderRadius: 8,
                padding: 20,
                marginTop: 24,
            }}
        >
            {/* ======= HEADER ======= */}
            <Row justify="space-between" align="middle">
                <Col>
                    <Text style={{ fontWeight: 500, fontSize: 16 }}>
                        Phương thức thanh toán
                    </Text>
                </Col>
                <Col>
                    <Text style={{ color: "#333" }}>
                        {paymentMethod === "cod"
                            ? "Thanh toán khi nhận hàng"
                            : paymentMethod === "bank"
                                ? "Chuyển khoản ngân hàng"
                                : "Ví điện tử"}
                    </Text>
                    <a
                        style={{
                            marginLeft: 10,
                            color: "#1677ff",
                            fontWeight: 500,
                            cursor: "pointer",
                        }}
                        onClick={() => setOpenModal(true)}
                    >
                        THAY ĐỔI
                    </a>
                </Col>
            </Row>

            <Divider style={{ margin: "16px 0" }} />

            {/* ======= PAYMENT DETAIL ======= */}
            <div style={{ float: "right", width: 300 }}>
                <Row justify="space-between" style={{ marginBottom: 8 }}>
                    <Text>Tổng tiền hàng</Text>
                    <Text>{formatVND(itemTotal)}</Text>
                </Row>

                <Row justify="space-between" style={{ marginBottom: 8 }}>
                    <Text>Phí vận chuyển</Text>
                    <Text>{formatVND(shippingFee)}</Text>
                </Row>

                <Row justify="space-between" style={{ marginBottom: 8 }}>
                    <Text>Giảm giá voucher</Text>
                    <Text style={{ color: "#cf1322" }}>
                        -{formatVND(voucherDiscount)}
                    </Text>
                </Row>

                <Divider style={{ margin: "8px 0" }} />

                <Row justify="space-between" align="middle">
                    <Text strong>Tổng thanh toán</Text>
                    <Text strong style={{ color: "#ff4d4f", fontSize: 20 }}>
                        {formatVND(totalPayment)}
                    </Text>
                </Row>

                <div style={{ marginTop: 20 }}>
                    <Text type="secondary" style={{ fontSize: 12 }}>
                        Nhấn "Đặt hàng" đồng nghĩa với việc bạn đồng ý tuân theo{" "}
                        <a href="#" style={{ color: "#1677ff" }}>
                            Điều khoản Shopee
                        </a>
                    </Text>

                    <div style={{ textAlign: "right", marginTop: 12 }}>
                        <Button
                            type="primary"
                            size="large"
                            style={{
                                backgroundColor: "#1677ff",
                                borderColor: "#1677ff",
                                minWidth: 120,
                                height: 44,
                                fontWeight: 500,
                            }}
                            onClick={handleConfirmOrder}
                        >
                            Đặt hàng
                        </Button>
                    </div>
                </div>
            </div>

            {/* ======= MODAL CHỌN PHƯƠNG THỨC ======= */}
            <Modal
                open={openModal}
                title="Chọn phương thức thanh toán"
                onCancel={() => setOpenModal(false)}
                onOk={handleSelectPayment}
                okText="Xác nhận"
                cancelText="Hủy"
            >
                <Radio.Group
                    value={paymentMethod}
                    onChange={(e) => setPaymentMethod(e.target.value)}
                >
                    <Radio value="cod" style={{ display: "block", margin: "8px 0" }}>
                        Thanh toán khi nhận hàng (COD)
                    </Radio>
                    <Radio value="bank" style={{ display: "block", margin: "8px 0" }}>
                        Chuyển khoản ngân hàng
                    </Radio>
                </Radio.Group>
            </Modal>

            {/* ======= MODAL QUẢN LÝ THẺ ======= */}
            <Modal
                open={showBankModal}
                footer={null}
                onCancel={() => setShowBankModal(false)}
                width={900}
                centered
                destroyOnClose
            >
                <BankManagementPage />
            </Modal>
        </div>
    );
};

export default PaymentMethodSection;
