import Footer from "../../layouts/Footer";
import Header from "../../layouts/Header";
import CategoryManergerComponent from "../../components/CategoryManergerComponent/CategoryManergerComponent.tsx";

const HomePage= () => {
    return (
        <>
          <Header />
          <h1>Trang chủ</h1>
            <CategoryManergerComponent />
          <Footer/>

        </>
    )
}
export default HomePage;
