import type { Product } from "../types/Product";
import QuantitySelector from "./QuantitySelector";

type ProductCardProps = {
    product: Product;
    quantity: number; 
    onQuantityChange: (quantity:number) => void;
}

function ProductCard({product, quantity, onQuantityChange}: ProductCardProps) {
    return (
        <article>
            <h3>{product.name}</h3>
            <p>Type: {product.type}</p>
            <p>Price: ${product.price.toFixed(2)}</p>
            <QuantitySelector quantity={quantity} onQuantityChange={onQuantityChange}/>
        </article>
    )
}

export default ProductCard;