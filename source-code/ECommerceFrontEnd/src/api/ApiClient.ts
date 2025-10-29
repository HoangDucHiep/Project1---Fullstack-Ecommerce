/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { AxiosHttpRequest } from './core/AxiosHttpRequest';
import { AddressService } from './services/AddressService';
import { AuthenticationService } from './services/AuthenticationService';
import { CategoryService } from './services/CategoryService';
import { ECommerceBackendApiService } from './services/ECommerceBackendApiService';
import { MediaService } from './services/MediaService';
import { ProductService } from './services/ProductService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class ApiClient {
    public readonly address: AddressService;
    public readonly authentication: AuthenticationService;
    public readonly category: CategoryService;
    public readonly eCommerceBackendApi: ECommerceBackendApiService;
    public readonly media: MediaService;
    public readonly product: ProductService;
    public readonly request: BaseHttpRequest;
    constructor(config?: Partial<OpenAPIConfig>, HttpRequest: HttpRequestConstructor = AxiosHttpRequest) {
        this.request = new HttpRequest({
            BASE: config?.BASE ?? '',
            VERSION: config?.VERSION ?? '1',
            WITH_CREDENTIALS: config?.WITH_CREDENTIALS ?? false,
            CREDENTIALS: config?.CREDENTIALS ?? 'include',
            TOKEN: config?.TOKEN,
            USERNAME: config?.USERNAME,
            PASSWORD: config?.PASSWORD,
            HEADERS: config?.HEADERS,
            ENCODE_PATH: config?.ENCODE_PATH,
        });
        this.address = new AddressService(this.request);
        this.authentication = new AuthenticationService(this.request);
        this.category = new CategoryService(this.request);
        this.eCommerceBackendApi = new ECommerceBackendApiService(this.request);
        this.media = new MediaService(this.request);
        this.product = new ProductService(this.request);
    }
}

