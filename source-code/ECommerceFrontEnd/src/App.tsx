import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { Fragment } from "react";
import { routes } from "./routes";
import DefaultComponent from "./components/DefaultComponent/DefaultComponent.tsx";
import { useAppSelector } from "./hooks/reduxHooks.ts";

function App() {
    const user = useAppSelector((state) => state.user);

    return (
        <Router>
            <Routes>
                {routes.map((route) => {
                    const Page = route.page;
                    const Layout = route.isShowHeader ? DefaultComponent : Fragment;

                    // ✅ kiểm tra private route
                    if (route.isPrivate && !user?.isAdmin) {
                        return (
                            <Route
                                key={route.id}
                                path={route.path}
                                element={<Navigate to="/sign-in" replace />}
                            />
                        );
                    } else {
                        return (
                            <Route
                                key={route.id}
                                path={route.path}
                                element={
                                    <Layout>
                                        <Page />
                                    </Layout>
                                }
                            />
                        );
                    }
                })}
            </Routes>
        </Router>
    );
}

export default App;
