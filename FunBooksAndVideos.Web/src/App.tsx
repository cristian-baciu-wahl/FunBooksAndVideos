import { useState } from "react";
import {
    BrowserRouter,
    Routes,
    Route,
    Navigate
} from "react-router-dom";

import AppLayout from "./components/AppLayout";
import ProductsPage from "./pages/ProductsPage";
import CartPage from "./pages/CartPage";
import CheckoutPage from "./pages/CheckoutPage";

import type { Product } from "./types/Product";
import type { CartItem } from "./types/CartItem";

function App() {

    // We hard-coded the same 2 products like we have in our database seed 
    // Preferably an e-commerce site should have a product catalog that is retrieved from storage
    const products: Product[] = [
        {
            id: 1,
            name: "The Girl on the train",
            price: 15.99,            
            type: "Book"
        },
        {
            id: 2,
            name: "Comprehensive First Aid Training",
            price: 33.51,
            type: "Video"
        },
    ];

    const [cart, setCart] = useState<CartItem[]>([]);

    const handleQuantityChange = (
        product: Product,
        quantity: number
    ) => {
        setCart(currentCart => {
            if (quantity === 0) {
                return currentCart.filter(
                    item => item.product.id !== product.id
                );
            }

            const existingItem = currentCart.find(
                item => item.product.id === product.id
            );

            if (existingItem) {
                return currentCart.map(item =>
                    item.product.id === product.id
                        ? { ...item, quantity }
                        : item
                );
            }

            return [
                ...currentCart,
                {
                    product,
                    quantity
                }
            ];
        });
    };

    const getQuantity = (productId: number) =>
        cart.find(item => item.product.id === productId)?.quantity ?? 0;

    return (
        <BrowserRouter>
            <Routes>
                <Route element={<AppLayout />}>
                  <Route
                      path="/products"
                      element={
                          <ProductsPage
                              products={products}
                              getQuantity={getQuantity}
                              onQuantityChange={handleQuantityChange}
                          />
                      }
                  />

                  <Route
                      path="/cart"
                      element={<CartPage items={cart} />}
                  />

                  <Route
                      path="/checkout"
                      element={<CheckoutPage items={cart}/>}
                  />
                </Route>
                <Route
                    path="*"
                    element={<Navigate to="/products" replace />}
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App;