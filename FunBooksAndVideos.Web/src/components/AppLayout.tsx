import { Outlet } from "react-router-dom";
import Header from "./Header";

function AppLayout() {
    return (
        <>
            <Header 
                title="Fun Books & Videos"
                subtitle="Your online shop for books and videos"
            />

            <Outlet />
        </>
    )
}

export default AppLayout;