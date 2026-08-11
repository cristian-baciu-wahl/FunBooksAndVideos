import ShoppingCart from "../components/ShoppingCart";
import type { CartItem } from "../types/CartItem";

type CartPageProps = {
    items: CartItem[];
};

function CartPage({ items }: CartPageProps) {
    return (
        <main>
            <ShoppingCart items={items} />
        </main>
    );
}

export default CartPage;