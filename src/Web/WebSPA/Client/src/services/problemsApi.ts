import type {
	ProblemDto,
	ProblemDetail,
	ProblemList,
	ProblemListRequest,
	ProblemManageList,
	ProblemVo,
	TestPoint,
} from "@/modules/problems-types.ts";

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
	const requestUrl = `/api/problems/${problemId}/detail`;
	const response = await client.get<ProblemDetail>(requestUrl);
	return response.data;
}

export async function getFullProblemAsync(problemId: number) {
	const requestUrl = `/api/problems/${problemId}`;
	const response = await client.get<ProblemVo>(requestUrl);
	return response.data;
}

export async function getProblemManageListAsync(
	request: ProblemListRequest
): Promise<ProblemManageList> {
	const requestUrl =
		`/api/problems/management_list?page=${request.page}&pageSize=${request.pageSize}` +
		`&distributorId=${request.distributorId}`;
	const response = await client.get<ProblemManageList>(requestUrl);
	return response.data;
}

export async function createProblemAsync(request: ProblemDto) {
	const response = await client.post(`/api/problems`, request);
	return response.data;
}

export async function deleteTestPointsAsync(
	problemId: number,
	testPoints: TestPoint[]
) {
	const ids = testPoints.map((testPoint) => testPoint.id);
	const response = await client.delete(
		`/api/problems/${problemId}/testpoints`,
		ids
	);
	return response.data;
}

export async function updateProblemAsync(
	problemId: number,
	request: ProblemDto,
	removedTestPoints: TestPoint[]
) {
	request.testPoints = request.testPoints.filter(
		(testPoint) => !removedTestPoints.includes(testPoint)
	);
	await client.put(`/api/problems/${problemId}`, request);
	return await deleteTestPointsAsync(problemId, removedTestPoints);
}

export async function deleteProblemAsync(problemId: number) {
	const response = await client.delete(`/api/problems/${problemId}`);
	return response.data;
}