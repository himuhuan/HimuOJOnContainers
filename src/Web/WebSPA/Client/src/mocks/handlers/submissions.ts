import {delay, http, HttpResponse} from "msw";
import {
    createSubmission,
    getSubmissionDetail,
    getSubmissionList,
} from "@/mocks/data/mockDb";
import type {CreateSubmissionRequest} from "@/modules/submits-type";

function parsePositiveInt(value: string | undefined) {
    const num = Number(value);
    if (!Number.isInteger(num) || num <= 0) {
        return undefined;
    }
    return num;
}

export const submissionHandlers = [
    http.get("/api/submissions", async ({request}) => {
        await delay(120);
        const url = new URL(request.url);
        const page = Number(url.searchParams.get("page") ?? "1");
        const pageSize = Number(url.searchParams.get("pageSize") ?? "20");

        const problemIdParam = url.searchParams.get("problemId") ?? undefined;
        const problemId = problemIdParam ? parsePositiveInt(problemIdParam) : undefined;
        const submitterId = url.searchParams.get("submitterId") ?? undefined;

        return HttpResponse.json(getSubmissionList(page, pageSize, problemId, submitterId));
    }),

    http.post("/api/submissions", async ({request}) => {
        await delay(180);
        const payload = (await request.json()) as CreateSubmissionRequest;
        const submissionId = createSubmission(payload);
        if (!submissionId) {
            return HttpResponse.text("Problem not found", {status: 404});
        }

        return HttpResponse.json(submissionId, {status: 201});
    }),

    http.get("/api/submissions/:id/detail", async ({params}) => {
        await delay(100);
        const submissionId = parsePositiveInt(params.id as string | undefined);
        if (!submissionId) {
            return HttpResponse.text("Invalid submission id", {status: 400});
        }

        const detail = getSubmissionDetail(submissionId);
        if (!detail) {
            return HttpResponse.text("Submission not found", {status: 404});
        }

        return HttpResponse.json(detail);
    }),
];
