/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { LoginOtpResendRequest } from '../models/LoginOtpResendRequest';
import type { LoginUserRequest } from '../models/LoginUserRequest';
import type { RefreshTokenRequest } from '../models/RefreshTokenRequest';
import type { RegisterUserCommandRequest } from '../models/RegisterUserCommandRequest';
import type { RegisterUserRequest } from '../models/RegisterUserRequest';
import type { VerifyLoginOtpRequest } from '../models/VerifyLoginOtpRequest';
import type { VerifyOtpRegisterRequest } from '../models/VerifyOtpRegisterRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AuthenticationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRegisterPhone({
        requestBody,
    }: {
        requestBody?: RegisterUserRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/register/phone',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRegisterPhoneOtp({
        requestBody,
    }: {
        requestBody?: RegisterUserRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/register/phone/otp',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRegisterPhoneOtpVerify({
        requestBody,
    }: {
        requestBody?: VerifyOtpRegisterRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/register/phone/otp/verify',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRegisterPhoneOtpResend({
        requestBody,
    }: {
        requestBody?: RegisterUserCommandRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/register/phone/otp/resend',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLogin({
        requestBody,
    }: {
        requestBody?: LoginUserRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/login',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLoginOtp({
        requestBody,
    }: {
        requestBody?: LoginUserRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/login/otp',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLoginOtpVerify({
        requestBody,
    }: {
        requestBody?: VerifyLoginOtpRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/login/otp/verify',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLoginOtpResend({
        requestBody,
    }: {
        requestBody?: LoginOtpResendRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/login/otp/resend',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1AuthMe(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/auth/me',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRefreshToken({
        requestBody,
    }: {
        requestBody?: RefreshTokenRequest,
    }): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/refresh-token',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLogout(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/auth/logout',
        });
    }
}
