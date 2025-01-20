import client from "@/modules/HttpClient";
import type {
    CreateSubmissionRequest,
    SubmissionDetail,
    SubmissionList,
    SubmissionListRequest,
} from "@/modules/submits-type";

// const exampleProblemList: SubmissionList = {
// 	total: 1000,
// 	pageCount: 100,
// 	items: Array.from({ length: 1000 })
// 		.fill(null)
// 		.map((_, index) => {
// 			return {
// 				id: index,
// 				submitterId: index.toString(),
// 				submitterName: "User",
// 				submitterAvatar: "/static/users/default/default_avatar.png",
// 				problemId: index,
// 				problemTitle: `Problem ${index}`,
// 				submitTime: "2021-10-01T00:00:00Z",
// 				compilerName: "cpp",
// 				usage: {
// 					usedMemoryByte: 1024,
// 					usedTimeMs: 1000,
// 				},
// 				status: "Accepted",
// 			} as SubmissionListItem;
// 		}),
// };

export async function getSubmissionList(
    request: SubmissionListRequest
): Promise<SubmissionList> {
    let requestUrl = `/api/submissions?page=${request.page}&pageSize=${request.pageSize}`;
    if (request.problemId) requestUrl += `&problemId=${request.problemId}`;
    if (request.submitterId) requestUrl += `&submitterId=${request.submitterId}`;
    const response = await client.get<SubmissionList>(requestUrl);
    return response.data;
}

export async function postSubmission(request: CreateSubmissionRequest) {
    const response = await client.post<number>("/api/submissions", request);
    return response.data;
}

export async function getSubmissionDetail(submissionId: number) {
    try {
        const response = await client.get<SubmissionDetail>(
            `/api/submissions/${submissionId}/detail`
        );
        return response.data;
    } catch (error) {
        console.error("Error occurred while fetching submission detail:", error);
        throw error;
    }
}
