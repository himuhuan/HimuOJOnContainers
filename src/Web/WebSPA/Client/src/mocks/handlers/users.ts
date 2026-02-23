import {delay, http, HttpResponse} from "msw";
import {getUserDetail, registerUser, uploadUserAvatar} from "@/mocks/data/mockDb";
import type {UserRegisterRequest} from "@/modules/user-types";

export const userHandlers = [
    http.get("/api/users/:id/detail", async ({params}) => {
        await delay(100);
        const userId = params.id as string | undefined;
        if (!userId) {
            return HttpResponse.text("Invalid user id", {status: 400});
        }

        const user = getUserDetail(userId);
        if (!user) {
            return HttpResponse.text("User not found", {status: 404});
        }

        return HttpResponse.json(user);
    }),

    http.post("/api/users", async ({request}) => {
        await delay(180);
        const payload = (await request.json()) as UserRegisterRequest;
        const user = registerUser(payload);
        return HttpResponse.json(user, {status: 201});
    }),

    http.put("/api/users/:id/avatar", async ({params, request}) => {
        await delay(180);
        const userId = params.id as string | undefined;
        if (!userId) {
            return HttpResponse.text("Invalid user id", {status: 400});
        }

        const formData = await request.formData();
        const file = formData.get("file");
        if (!(file instanceof File)) {
            return HttpResponse.text("Missing file", {status: 400});
        }

        const avatarUrl = uploadUserAvatar(userId);
        if (!avatarUrl) {
            return HttpResponse.text("User not found", {status: 404});
        }

        return HttpResponse.json({avatar: avatarUrl});
    }),
];
