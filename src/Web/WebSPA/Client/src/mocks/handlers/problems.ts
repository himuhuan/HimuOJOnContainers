import {delay, http, HttpResponse} from "msw";
import {
    createProblem,
    deleteProblem,
    deleteProblemTestPoints,
    getProblemDetail,
    getProblemList,
    getProblemManageList,
    getProblemResourceContent,
    getProblemTestPointResourceContent,
    getProblemVo,
    updateProblem,
    uploadProblemResource,
} from "@/mocks/data/mockDb";
import type {ProblemDto} from "@/modules/problems-types";

function parsePositiveInt(value: string | undefined) {
    const num = Number(value);
    if (!Number.isInteger(num) || num <= 0) {
        return undefined;
    }
    return num;
}

export const problemHandlers = [
    http.get("/api/problems/list", async ({request}) => {
        await delay(150);
        const url = new URL(request.url);
        const page = Number(url.searchParams.get("page") ?? "1");
        const pageSize = Number(url.searchParams.get("pageSize") ?? "10");
        return HttpResponse.json(getProblemList(page, pageSize));
    }),

    http.get("/api/problems/management_list", async ({request}) => {
        await delay(150);
        const url = new URL(request.url);
        const page = Number(url.searchParams.get("page") ?? "1");
        const pageSize = Number(url.searchParams.get("pageSize") ?? "10");
        const distributorId = url.searchParams.get("distributorId") ?? undefined;
        return HttpResponse.json(getProblemManageList(page, pageSize, distributorId));
    }),

    http.get("/api/problems/:id/detail", async ({params}) => {
        await delay(120);
        const problemId = parsePositiveInt(params.id as string | undefined);
        if (!problemId) {
            return HttpResponse.text("Invalid problem id", {status: 400});
        }

        const detail = getProblemDetail(problemId);
        if (!detail) {
            return HttpResponse.text("Problem not found", {status: 404});
        }

        return HttpResponse.json(detail);
    }),

    http.get("/api/problems/:id", async ({params}) => {
        await delay(120);
        const problemId = parsePositiveInt(params.id as string | undefined);
        if (!problemId) {
            return HttpResponse.text("Invalid problem id", {status: 400});
        }

        const problem = getProblemVo(problemId);
        if (!problem) {
            return HttpResponse.text("Problem not found", {status: 404});
        }

        return HttpResponse.json(problem);
    }),

    http.post("/api/problems", async ({request}) => {
        await delay(200);
        const payload = (await request.json()) as ProblemDto;
        const id = createProblem(payload);
        return HttpResponse.json({id}, {status: 201});
    }),

    http.put("/api/problems/:id", async ({request, params}) => {
        await delay(200);
        const problemId = parsePositiveInt(params.id as string | undefined);
        if (!problemId) {
            return HttpResponse.text("Invalid problem id", {status: 400});
        }

        const payload = (await request.json()) as ProblemDto;
        const updated = updateProblem(problemId, payload);
        if (!updated) {
            return HttpResponse.text("Problem not found", {status: 404});
        }

        return HttpResponse.json({ok: true});
    }),

    http.delete("/api/problems/:id", async ({params}) => {
        await delay(100);
        const problemId = parsePositiveInt(params.id as string | undefined);
        if (!problemId) {
            return HttpResponse.text("Invalid problem id", {status: 400});
        }

        const deleted = deleteProblem(problemId);
        if (!deleted) {
            return HttpResponse.text("Problem not found", {status: 404});
        }

        return HttpResponse.json({ok: true});
    }),

    http.delete("/api/problems/:id/testpoints", async ({request, params}) => {
        await delay(100);
        const problemId = parsePositiveInt(params.id as string | undefined);
        if (!problemId) {
            return HttpResponse.text("Invalid problem id", {status: 400});
        }

        const testPointIds = (await request.json()) as number[];
        deleteProblemTestPoints(problemId, testPointIds);
        return HttpResponse.json({ok: true});
    }),

    http.post("/api/problems/:id/resources/:resourceType", async ({request, params}) => {
        await delay(220);
        const problemId = parsePositiveInt(params.id as string | undefined);
        const resourceType = params.resourceType as "input" | "answer" | undefined;
        if (!problemId || (resourceType !== "input" && resourceType !== "answer")) {
            return HttpResponse.text("Invalid upload request", {status: 400});
        }

        const formData = await request.formData();
        const file = formData.get("file");
        if (!(file instanceof File)) {
            return HttpResponse.text("Missing file", {status: 400});
        }

        const fileName = uploadProblemResource(problemId, resourceType);
        if (!fileName) {
            return HttpResponse.text("Problem not found", {status: 404});
        }

        return HttpResponse.text(fileName);
    }),

    http.get("/api/problems/:id/resources/:resourceName", async ({params}) => {
        await delay(80);
        const problemId = parsePositiveInt(params.id as string | undefined);
        const resourceName = params.resourceName as string | undefined;
        if (!problemId || !resourceName) {
            return HttpResponse.text("Invalid request", {status: 400});
        }

        const content = getProblemResourceContent(problemId, resourceName);
        if (!content) {
            return HttpResponse.text("Resource not found", {status: 404});
        }

        return new HttpResponse(content, {
            headers: {
                "Content-Type": "application/octet-stream",
            },
        });
    }),

    http.get("/api/problems/:id/testpoints/:testPointId/:type", async ({params}) => {
        await delay(80);
        const problemId = parsePositiveInt(params.id as string | undefined);
        const testPointId = parsePositiveInt(params.testPointId as string | undefined);
        const type = params.type as "input" | "answer" | undefined;
        if (!problemId || !testPointId || (type !== "input" && type !== "answer")) {
            return HttpResponse.text("Invalid request", {status: 400});
        }

        const content = getProblemTestPointResourceContent(problemId, testPointId, type);
        if (!content) {
            return HttpResponse.text("Resource not found", {status: 404});
        }

        return new HttpResponse(content, {
            headers: {
                "Content-Type": "application/octet-stream",
            },
        });
    }),
];
