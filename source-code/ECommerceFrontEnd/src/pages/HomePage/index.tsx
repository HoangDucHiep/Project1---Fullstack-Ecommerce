import Footer from "../../layouts/Footer";
import Header from "../../layouts/Header";
import CategoryManergerComponent from "../../components/CategoryManergerComponent/CategoryManergerComponent.tsx";
import FeaturedProducts from "../../components/Featured products";
import CategoryMenu from "../../components/CategoryMenu";
import BrandMenu from "../../components/Brands";
import TopSeller from "../../components/TopSeller";

const HomePage= () => {
    return (
        <>
          <Header />
          <h1>Trang chủ</h1>
            <FeaturedProducts />
            <CategoryMenu />
            <BrandMenu />
            <TopSeller />
            <CategoryManergerComponent />
          <Footer/>

        </>
    )
}
export default HomePage;
