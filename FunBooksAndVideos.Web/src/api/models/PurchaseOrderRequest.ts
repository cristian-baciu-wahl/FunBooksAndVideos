export type PurchaseOrderItemRequest = {
    productId?: number;
    membershipType?: string;
    quantity: number;
};

export type PurchaseOrderRequest = {
    customerId: number;
    items: PurchaseOrderItemRequest[];
};