import client from "@/modules/HttpClient";
import {UserDetail, UserRegisterRequest} from "@/modules/user-types";

export async function getUserDetail(id: string) {
    const response = await client.get<UserDetail>(`/api/users/${id}/detail`);
    return response.data;
}

export async function registerUser(user: UserRegisterRequest) {
    const response = await client.post<UserDetail>("/api/users", user);
    return response.data;
}