
export interface ApiResult<T> {
    data: T | undefined;
    success: boolean;
    message: string;
}

export interface ApiPaginationResult<T> {
    data: T[];
    totalCount: number;
    pageCount: number;
}