import { useState } from "react";
import type { CartItem } from "../types/CartItem";
import { createPurchaseOrder } from "../api/purchaseOrderApi";

type CheckoutPageProps = {
    items: CartItem[];
};

function CheckoutPage({ items }: CheckoutPageProps) {

    const [customerId, setCustomerId] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    const total = items.reduce((sum, item) => sum + item.product.price * item.quantity, 0);

    const handleSubmit = async() => {
        setIsSubmitting(true);

       try {
            const response = await createPurchaseOrder({
                customerId: Number(customerId),
                items: items.map(item => ({
                    productId: item.product.id,
                    quantity: item.quantity
                }))
            });

            console.log("Order created:", response);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <main>
            <h2>Checkout</h2>

            <div>
                <label htmlFor="customerId">
                    Customer ID
                </label>

                <input
                    id="customerId"
                    type="number"
                    value={customerId}
                    onChange={event =>
                        setCustomerId(event.target.value)
                    }
                />
            </div>

            <h3>Order Summary</h3>

            {items.map(item => (
                <div key={item.product.id}>
                    <span>
                        {item.product.name} x{item.quantity}
                    </span>

                    <span>
                        £{(
                            item.product.price * item.quantity
                        ).toFixed(2)}
                    </span>
                </div>
            ))}

            <h3>
                Total: £{total.toFixed(2)}
            </h3>

            <button
                onClick={handleSubmit}
                disabled={isSubmitting || items.length === 0}
            >
                {isSubmitting ? "Placing Order..." : "Place Order"}
            </button>
        </main>
    );
}

export default CheckoutPage;