import {delay, http, HttpResponse} from "msw";
import {normalizeMockAuthRole} from "@/mocks/data/constants";
import {getClaimsByRole} from "@/mocks/data/mockDb";

export const authHandlers = [
    http.get("/bff/user", async ({request}) => {
        await delay(120);
        const role = normalizeMockAuthRole(request.headers.get("X-Mock-Role"));
        if (role === "guest") {
            return new HttpResponse(null, {status: 401});
        }

        return HttpResponse.json(getClaimsByRole(role));
    }),
];
