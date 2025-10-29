/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CreateProductRequest } from '../models/CreateProductRequest';
import type { ProductSortBy } from '../models/ProductSortBy';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class ProductService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1Products({
        categoryId,
        minPrice,
        maxPrice,
        hasDiscount,
        shopId,
        sortBy,
        page = 1,
        pageSize = 20,
    }: {
        categoryId?: string,
        minPrice?: number,
        maxPrice?: number,
        hasDiscount?: boolean,
        shopId?: string,
        sortBy?: ProductSortBy,
        page?: number,
        pageSize?: number,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/products',
            query: {
                'categoryId': categoryId,
                'minPrice': minPrice,
                'maxPrice': maxPrice,
                'hasDiscount': hasDiscount,
                'shopId': shopId,
                'sortBy': sortBy,
                'page': page,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1ProductsCreate({
        requestBody,
    }: {
        requestBody?: CreateProductRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/products/create',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1ProductsPublic({
        productId,
    }: {
        productId: string,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/products/{productId}/public',
            path: {
                'productId': productId,
            },
        });
    }
}
