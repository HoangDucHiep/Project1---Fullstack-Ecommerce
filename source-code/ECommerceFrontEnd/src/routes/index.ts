// src/routes/index.ts
import HomePage from "../pages/HomePage";
import LoginPage from "../pages/LoginPage";
import ProductPage from "../pages/ProductPage";

export interface AppRoute {
    id: string;
    path: string;
    page: React.FC;
    isShowHeader?: boolean;
    isPrivate?: boolean;
}

export const routes = [
    {
        id: "home",
        path: "/",
        page: HomePage,
        isShowHeader: false,   // có header
        isPrivate: false,
    },
    {
        id: "Login",
        path: "/Login",
        page: LoginPage,
        isShowHeader: false,  // không có header
        isPrivate: false,
    },
    {
        id: "ProductPage",
        path: "/ProductPage",
        page: ProductPage,
        isShowHeader: false,  // không có header
        isPrivate: false,
    },

];