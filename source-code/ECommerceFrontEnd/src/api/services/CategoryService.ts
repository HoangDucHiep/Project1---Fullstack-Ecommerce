/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CreateCategoryRequest } from '../models/CreateCategoryRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CategoryService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CategoriesCreate({
        requestBody,
    }: {
        requestBody?: CreateCategoryRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/categories/create',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1CategoriesSearch({
        nameOfCategory,
    }: {
        nameOfCategory?: string,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/categories/Search',
            query: {
                'NameOfCategory': nameOfCategory,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1Categories(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/categories',
        });
    }
}
