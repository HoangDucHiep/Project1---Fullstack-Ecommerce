/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddressCreateRequest } from '../models/AddressCreateRequest';
import type { AddressUpdateRequest } from '../models/AddressUpdateRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AddressService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1MeAddresses({
        requestBody,
    }: {
        requestBody?: AddressCreateRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/me/addresses',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1MeAddresses({
        page,
        pageSize,
    }: {
        page?: number,
        pageSize?: number,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/me/addresses',
            query: {
                'Page': page,
                'PageSize': pageSize,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1MeAddresses1({
        addressId,
    }: {
        addressId: string,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/me/addresses/{addressId}',
            path: {
                'addressId': addressId,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public putApiV1MeAddresses({
        addressId,
        requestBody,
    }: {
        addressId: string,
        requestBody?: AddressUpdateRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/me/addresses/{addressId}',
            path: {
                'addressId': addressId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public deleteApiV1MeAddresses({
        addressId,
    }: {
        addressId: string,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/me/addresses/{addressId}',
            path: {
                'addressId': addressId,
            },
        });
    }
}
