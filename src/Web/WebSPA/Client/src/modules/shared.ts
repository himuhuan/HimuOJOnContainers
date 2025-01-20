export interface ApiResult<T> {
    data: T | undefined;
    success: boolean;
    message: string;
}
