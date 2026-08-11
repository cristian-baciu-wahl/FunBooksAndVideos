import { Link } from "react-router-dom";

type HeaderProps = {
    title:string;
    subtitle:string;
}

function Header({title,subtitle}: HeaderProps) {
    return (
        <header>
            <h1>{title}</h1>
            <h3>{subtitle}</h3>

            <nav>
                <Link to="/products">Products</Link>
                {" | "}
                <Link to="/cart">Cart</Link>
                {" | "}
                <Link to="/checkout">Checkout</Link>
            </nav>
        </header>
    );
}

export default Header;