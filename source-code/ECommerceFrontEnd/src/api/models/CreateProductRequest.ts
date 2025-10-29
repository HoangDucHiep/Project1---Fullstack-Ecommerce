/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ProductMediaDto } from './ProductMediaDto';
import type { ProductOptionDto } from './ProductOptionDto';
import type { ProductVariantDto } from './ProductVariantDto';
export type CreateProductRequest = {
    shopId?: string;
    categoryId?: string;
    name?: string | null;
    description?: string | null;
    media?: Array<ProductMediaDto> | null;
    options?: Array<ProductOptionDto> | null;
    variants?: Array<ProductVariantDto> | null;
};

