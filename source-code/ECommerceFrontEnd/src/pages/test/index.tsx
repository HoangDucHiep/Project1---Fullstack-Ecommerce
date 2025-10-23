import React, { useState } from "react";
import api from '../../api/axios.ts'

const RegisterPage: React.FC = () => {
    const [phoneNumber, setPhoneNumber] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleRegister = async () => {
        try {
            const response = await api.post("/api/Authentication/register/phone", {
                phoneNumber,
                password,
            });

            console.log("Kết quả:", response.data);
            setMessage(response.data.message || "Đăng ký thành công!");
        } catch (error: any) {
            // ⚠️ Xử lý lỗi ở đây
            if (error.response) {
                // Lỗi trả về từ server (.NET)
                console.error("Lỗi server:", error.response.data);
                setMessage(error.response.data.message || "Đăng ký thất bại!");
            } else if (error.request) {
                // Không kết nối được tới server
                console.error("Không nhận được phản hồi từ server:", error.request);
                setMessage("Không thể kết nối đến server!");
            } else {
                // Lỗi khác
                console.error("Lỗi không xác định:", error.message);
                setMessage("Đã xảy ra lỗi không xác định!");
            }
        }
    };

    return (
        <div style={{ padding: "20px" }}>
            <h2>Đăng ký bằng số điện thoại</h2>
            <input
                type="text"
                placeholder="Số điện thoại"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
            />
            <br />
            <input
                type="password"
                placeholder="Mật khẩu"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />
            <br />
            <button onClick={handleRegister}>Đăng ký</button>
            <p>{message}</p>
        </div>
    );
};

export default RegisterPage;
