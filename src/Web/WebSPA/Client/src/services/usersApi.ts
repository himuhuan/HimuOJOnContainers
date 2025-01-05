import client from "@/modules/HttpClient";
import { UserDetail } from "@/modules/user-types";

export async function getUserDetail(id: string) {
    const response = await client.get<UserDetail>(`/api/users/${id}/detail`);
    return response.data;
}