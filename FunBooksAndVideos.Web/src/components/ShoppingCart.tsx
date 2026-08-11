import type { CartItem } from "../types/CartItem";


type ShoppingCartProps = {
    items: CartItem[];
}

function ShoppingCart({items}:ShoppingCartProps){

    const total = items.reduce((sum, item) => sum + item.product.price * item.quantity, 0);

    return (
        <section>
            <h2>Shopping Cart</h2>
            {items.length === 0 ? (
                <p>Your cart is empty</p>
            ) : (
                <>
                    {items.map(item => (
                        <div key={item.product.id}>
                            <span>{item.product.name} x{item.quantity}</span>
                            <span>${(item.product.price * item.quantity).toFixed(2)}</span>
                        </div>
                    ))}
                    <h3>
                        Total: ${total.toFixed(2)}
                    </h3>
                </>
            )}
        </section>
    );
}

export default ShoppingCart;