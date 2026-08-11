import ProductCard from "../components/ProductCard";
import type { Product } from "../types/Product";

type ProductsPageProps = {
    products: Product[];
    getQuantity: (productId: number) => number;
    onQuantityChange: (product: Product, quantity: number) => void;
};

function ProductsPage({
    products,
    getQuantity,
    onQuantityChange
}: ProductsPageProps) {
    return (
        <main>
            <h2>Products</h2>

            <div>
                {products.map(product => (
                    <ProductCard
                        key={product.id}
                        product={product}
                        quantity={getQuantity(product.id)}
                        onQuantityChange={quantity =>
                            onQuantityChange(product, quantity)
                        }
                    />
                ))}
            </div>
        </main>
    );
}

export default ProductsPage;