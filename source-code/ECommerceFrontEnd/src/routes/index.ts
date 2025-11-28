import HomePage from "../pages/HomePage";
import LoginPage from "../pages/LoginPage";
import ProductPage from "../pages/ProductPage";
import Cart from "../pages/Cart";
import SearchPage from "../pages/SearchPage";
import ProductDetail from "../pages/ProductDetail";
import SignUpPage from "../pages/SignUpPage";
import TestPage from "../pages/TestPage";
import BuyPage from "../pages/BuyPage";
import AccountPage from "../pages/AccountPage";
import SellerProductPage from "../pages/SellerProductPage";
import SellerPendingProductPage from "../pages/SellerPendingProductPage";
import SellerUnapprovedProductPage from "../pages/SellerUnapprovedProductPage";
import SellerDiscountVoucherPage from "../pages/SellerDiscountVoucherPage";
import SellerChatComponentPage from "../pages/SellerChatComponentPage";
import SellerCanceledProductPage from "../pages/SellerCanceledProductPage";
import SellerOrderReportPage from "../pages/SellerOrderReportPage";
export const routes = [
    {
        id: "home",
        path: "/",
        page: HomePage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "Login",
        path: "/Login",
        page: LoginPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SignUp",
        path: "/SignUp",
        page: SignUpPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "ProductPage",
        path: "/ProductPage",
        page: ProductPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "CartPage",
        path: "/CartPage",
        page: Cart,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SearchPage",
        path: "/SearchPage",
        page: SearchPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "ProductDetail",
        path: "/ProductDetail",
        page: ProductDetail,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "TestPage",
        path: "/TestPage",
        page: TestPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "BuyPage",
        path: "/BuyPage",
        page: BuyPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "AccountPage",
        path: "/AccountPage",
        page: AccountPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {   
        id: "SellerProductPage",
        path: "/SellerProductPage",
        page:SellerProductPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SellerPendingProductPage", 
        path: "/SellerPendingProductPage",
        page: SellerPendingProductPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SellerUnapprovedProductPage", 
        path: "/SellerUnapprovedProductPage",
        page: SellerUnapprovedProductPage,
        isShowHeader: false,
        isFinite: false,
    },
    {
        id: "SellerCanceledProductPage",
        path: "/SellerCanceledProductPage",
        page: SellerCanceledProductPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SellerDiscountVoucherPage",
        path: "/SellerDiscountVoucherPage",
        page: SellerDiscountVoucherPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SellerChatComponentPage",
        path: "/SellerChatComponentPage",
        page: SellerChatComponentPage,
        isShowHeader: false,
        isPrivate: false,
    },
    {
        id: "SellerOrderReportPage",
        path: "/SellerOrderReportPage",
        page: SellerOrderReportPage,
        isShowHeader: false,
        isPrivate: false,
    },



];