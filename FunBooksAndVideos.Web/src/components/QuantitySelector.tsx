
type QuantitySelectorProps = {
    quantity: number;
    onQuantityChange: (quantity:number) => void;
}

function QuantitySelector({
    quantity,
    onQuantityChange
}: QuantitySelectorProps) {

    return (
        <div>
           <button  onClick={() => onQuantityChange(quantity - 1)} disabled={quantity === 0}>
                -
            </button>

            <span>{quantity}</span>

            <button onClick={() => onQuantityChange(quantity + 1)}>
                +
            </button>
        </div>
    );
}

export default QuantitySelector;