import React, { useState } from "react";
import ProductList, { type ProductItem } from "../../components/BuyPage/ProductListBuyPage";
import ShippingAndTotal from "../../components/BuyPage/ShippingAndTotal";
import PaymentMethodSection from "../../components/BuyPage/PaymentSection";

import Header from "../../layouts/Header";
import Footer from "../../layouts/Footer";
import FeaturedProducts from "../../components/Featured products";
import img from "../../assets/img/SamSungS24 Ultra.jpg";
import DeliveryAddress from "../../components/BuyPage/DeliveryAddress";

const BuyPage: React.FC = () => {

    // 🛍️ Danh sách sản phẩm
    const products: ProductItem[] = [
        {
            id: 1,
            name: "Giá Đỡ Điện Thoại Máy Tính Bảng Để Bàn Kim Loại Nặng lắm ",
            category: "Giá Đỡ Màu Xám",
            image: img,
            price: 50000,
            quantity: 3,
        },
    ];

    // 💰 Tổng tiền hàng
    const itemTotal = products.reduce((sum, p) => sum + p.price * p.quantity, 0);

    // 🚚 Trạng thái vận chuyển
    const [shippingFee, setShippingFee] = useState(25000);
    const [shippingMethod, setShippingMethod] = useState<"Nhanh" | "Hỏa tốc">("Nhanh");

    // 🎟️ Giảm giá từ voucher
    const [voucherDiscount] = useState(15000);

    // 📦 Nhận dữ liệu thay đổi từ ShippingAndTotal
    const handleChangeShipping = (method: "Nhanh" | "Hỏa tốc", fee: number) => {
        setShippingMethod(method);
        setShippingFee(fee);
    };

    return (
        <>
            <Header />
            <div
                style={{
                    maxWidth: 960,
                    margin: "100px auto 0",
                    padding: "24px 20px 320px",
                    background: "#fff",
                    borderRadius: 8,
                    boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                    minHeight: "70vh",
                    position: "relative",
                    zIndex: 1,
                }}
            >   
                <DeliveryAddress/>
                {/* 🛒 Danh sách sản phẩm */}
                <ProductList />

                {/* 🚚 Thông tin vận chuyển & tổng tiền */}
                <ShippingAndTotal
                    shippingFee={shippingFee}
                    itemTotal={itemTotal}
                    voucherDiscount={voucherDiscount}
                    onChangeShipping={handleChangeShipping}
                    shippingMethod={shippingMethod}
                />

                {/* 💳 Chọn phương thức thanh toán */}
                <PaymentMethodSection
                    itemTotal={itemTotal}
                    shippingFee={shippingFee}
                    voucherDiscount={voucherDiscount}
                    onOrder={(total, method) => {
                        console.log("Đặt hàng:", total, method);
                    }}
                />
            </div>

            <FeaturedProducts />
            <Footer />
        </>
    );
};

export default BuyPage;
