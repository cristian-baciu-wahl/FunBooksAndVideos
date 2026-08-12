import { useState } from "react";
import type { CartItem } from "../types/CartItem";
import { createPurchaseOrder } from "../api/purchaseOrderApi";

type CheckoutPageProps = {
    items: CartItem[];
};

function CheckoutPage({ items }: CheckoutPageProps) {
    const [customerId, setCustomerId] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [orderId, setOrderId] = useState<number | null>(null);
    const [error, setError] = useState<string | null>(null);

    const total = items.reduce(
        (sum, item) => sum + item.product.price * item.quantity,
        0
    );

    const handleSubmit = async () => {
        setError(null);

        const parsedCustomerId = Number(customerId);

        if (!Number.isInteger(parsedCustomerId) || parsedCustomerId <= 0) {
            setError("Please enter a valid customer ID.");
            return;
        }

        if (items.length === 0) {
            setError("Your cart is empty.");
            return;
        }

        setIsSubmitting(true);

        try {
            const response = await createPurchaseOrder({
                customerId: parsedCustomerId,
                items: items.map(item => ({
                    productId: item.product.id,
                    quantity: item.quantity
                }))
            });

            setOrderId(response.orderId);
        } catch {
            setError(
                "We couldn't place your order. Please try again."
            );
        } finally {
            setIsSubmitting(false);
        }
    };

    if (orderId !== null) {
        return (
            <main>
                <h2>Order Confirmed</h2>

                <p>
                    Your order has been placed successfully.
                </p>

                <p>
                    <strong>Order ID:</strong> {orderId}
                </p>

                <p>
                    <strong>Items:</strong> {items.length}
                </p>

                <p>
                    <strong>Total:</strong> £{total.toFixed(2)}
                </p>
            </main>
        );
    }

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
                    min="1"
                    value={customerId}
                    onChange={event => setCustomerId(event.target.value)}
                    disabled={isSubmitting}
                />
            </div>

            {error && (
                <p role="alert">
                    {error}
                </p>
            )}

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
                type="button"
                onClick={handleSubmit}
                disabled={isSubmitting || items.length === 0}
            >
                {isSubmitting
                    ? "Placing Order..."
                    : "Place Order"}
            </button>
        </main>
    );
}

export default CheckoutPage;
