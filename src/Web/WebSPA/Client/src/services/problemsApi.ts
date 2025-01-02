import type {ProblemDetail, ProblemList, ProblemListRequest,} from "@/modules/problems-types.ts";

import client from "@/modules/HttpClient.ts";

// const exampleProblemList: ProblemList = {
// 	total: 1000,
// 	pageCount: 100,
// 	items: Array.from({ length: 1000 })
// 		.fill(null)
// 		.map((_, index) => {
// 			return {
// 				id: index,
// 				title: `Problem ${index}`,
// 				totalSubmissionCount: 100,
// 				acceptedSubmissionCount: 50,
// 			} as ProblemListItem;
// 		}),
// };

export async function getProblemListAsync(
    request: ProblemListRequest
): Promise<ProblemList> {
    const requestUrl = `/api/problems/list?page=${request.page}&pageSize=${request.pageSize}`;
    const response = await client.get<ProblemList>(requestUrl);
    return response.data;
}

export async function getProblemDetailAsync(problemId: number) {
    const requestUrl = `/api/problems/${problemId}`;
    const response = await client.get<ProblemDetail>(requestUrl);
    return response.data;
}