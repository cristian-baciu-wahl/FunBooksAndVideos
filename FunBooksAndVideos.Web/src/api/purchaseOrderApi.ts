import apiClient from "./apiClient";
import type { PurchaseOrderRequest } from "./models/PurchaseOrderRequest";

export type PurchaseOrderResponse = {
    orderId: number;
    message: string;
    items: number;
    totalPrice: number;
};

export async function createPurchaseOrder(request: PurchaseOrderRequest): Promise<PurchaseOrderResponse> 
{
    const response = await apiClient.post<PurchaseOrderResponse>(
        "/PurchaseOrder",
        request
    );

    return response.data;
}