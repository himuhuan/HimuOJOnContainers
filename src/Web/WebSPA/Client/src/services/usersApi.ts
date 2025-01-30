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

export async function uploadUserAvatar(id: string, file: File) {
    const formData = new FormData();
    formData.append("file", file);
    const response = await client.put(`/api/users/${id}/avatar`, formData, {
        headers: {
            "Content-Type": "multipart/form-data",
        },
    });

    return response.data;
}