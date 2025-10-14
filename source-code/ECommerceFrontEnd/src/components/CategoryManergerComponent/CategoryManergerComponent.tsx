
import React from "react";
import { Menu } from "antd";

const { SubMenu } = Menu;

const CategoryMenu: React.FC = () => {
    return (
        <Menu mode="vertical" style={{ width: 250 }}>
            <SubMenu
                key="giay-dep-nam"
                title="Giày Dép Nam"
            >
                <Menu.Item key="thesim">Thẻ sim</Menu.Item>
                <Menu.Item key="maytinh-bang">Máy tính bảng</Menu.Item>
                <Menu.Item key="dienthoai">Điện thoại</Menu.Item>
                <SubMenu key="thietbi-deo" title="Thiết bị đeo thông minh" popupOffset={[0, 0]} />

                <SubMenu
                    key="phukien"
                    title={<span style={{ color: "red" }}>Phụ kiện</span>}
                    popupOffset={[0, 0]}
                >
                    <Menu.Item key="bodam">Bộ đàm</Menu.Item>
                    <Menu.Item key="khacphukien">Khác</Menu.Item>

                    <SubMenu
                        key="phukien-selfie"
                        title={<span style={{ color: "red" }}>Phụ kiện selfie</span>}
                        popupOffset={[0, 0]}
                    >
                        <Menu.Item key="ongkinh">Ống kính điện thoại</Menu.Item>
                        <Menu.Item key="denflash">Đèn flash điện thoại & Đèn selfie</Menu.Item>
                        <Menu.Item key="quatusb">Quạt USB & Quạt điện thoại</Menu.Item>
                        <Menu.Item key="butcam-ung">Bút cảm ứng</Menu.Item>
                        <Menu.Item key="kepdt">Kẹp điện thoại</Menu.Item>
                        <Menu.Item key="daydeo">Dây đeo điện thoại & Móc khóa</Menu.Item>
                        <Menu.Item key="thenho">Thẻ nhớ</Menu.Item>
                        <Menu.Item key="thietbi-trinh-chieu">Thiết bị trình chiếu</Menu.Item>
                        <Menu.Item key="tuidung-dt">Túi đựng điện thoại</Menu.Item>

                        <SubMenu key="phukien-selfie-more" title="Phụ kiện khác" popupOffset={[0, 0]}>
                            <Menu.Item key="gayselfie">Gậy selfie</Menu.Item>
                            <Menu.Item key="giado">Giá đỡ</Menu.Item>
                            <Menu.Item key="dieukhien">Điều khiển chụp hình</Menu.Item>
                            <Menu.Item key="khacselfie">Khác</Menu.Item>
                        </SubMenu>
                    </SubMenu>
                </SubMenu>
            </SubMenu>
            <SubMenu
                key="dien-thoai"
                title={<span style={{ color: "red" }}>Điện Thoại & Phụ Kiện</span>}
                popupOffset={[0, 0]} // 👈 fix ngang hàng
            >
                <Menu.Item key="the-sim">Thẻ sim</Menu.Item>
                <Menu.Item key="may-tinh-bang">Máy tính bảng</Menu.Item>
                <Menu.Item key="dien-thoai">Điện thoại</Menu.Item>
                <SubMenu key="thiet-bi-deo" title="Thiết bị đeo thông minh" popupOffset={[0, 0]} />

                <SubMenu
                    key="phu-kien"
                    title={<span style={{ color: "red" }}>Phụ kiện</span>}
                    popupOffset={[0, 0]}
                >
                    <Menu.Item key="bo-dam">Bộ đàm</Menu.Item>
                    <Menu.Item key="khac-phukien">Khác</Menu.Item>

                    <SubMenu
                        key="phu-kien-selfie"
                        title={<span style={{ color: "red" }}>Phụ kiện selfie</span>}
                        popupOffset={[0, 0]}
                    >
                        <Menu.Item key="ong-kinh">Ống kính điện thoại</Menu.Item>
                        <Menu.Item key="den-flash">Đèn flash điện thoại & Đèn selfie</Menu.Item>
                        <Menu.Item key="quat-usb">Quạt USB & Quạt điện thoại</Menu.Item>
                        <Menu.Item key="but-cam-ung">Bút cảm ứng</Menu.Item>
                        <Menu.Item key="kep-dt">Kẹp điện thoại</Menu.Item>
                        <Menu.Item key="day-deo">Dây đeo điện thoại & Móc khóa</Menu.Item>
                        <Menu.Item key="the-nho">Thẻ nhớ</Menu.Item>
                        <Menu.Item key="thiet-bi-trinh-chieu">Thiết bị trình chiếu</Menu.Item>
                        <Menu.Item key="tui-dung-dt">Túi đựng điện thoại</Menu.Item>

                        <SubMenu key="phu-kien-selfie-more" title="Phụ kiện khác" popupOffset={[0, 0]}>
                            <Menu.Item key="gay-selfie">Gậy selfie</Menu.Item>
                            <Menu.Item key="gia-do">Giá đỡ</Menu.Item>
                            <Menu.Item key="dieu-khien">Điều khiển chụp hình</Menu.Item>
                            <Menu.Item key="khac-selfie">Khác</Menu.Item>
                        </SubMenu>
                    </SubMenu>
                </SubMenu>
            </SubMenu>

            <SubMenu key="du-lich" title="Du lịch & Hành lý" />
            <SubMenu key="tui-vi-nu" title="Túi Ví Nữ" />
            <SubMenu key="giay-dep-nu" title="Giày Dép Nữ" />
            <SubMenu key="tui-vi-nam" title="Túi Ví Nam" />
            <SubMenu key="dong-ho" title="Đồng Hồ" />
            <SubMenu key="thiet-bi-am-thanh" title="Thiết Bị Âm Thanh" />
            <SubMenu key="thuc-pham" title="Thực phẩm và đồ uống" />
            <SubMenu key="cham-soc-thu-cung" title="Chăm Sóc Thú Cưng" />
        </Menu>
    );
};

export default CategoryMenu;
