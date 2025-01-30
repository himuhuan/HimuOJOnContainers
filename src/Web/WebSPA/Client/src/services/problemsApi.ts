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

export async function downloadProblemResourceAsync(
	problemId: number,
	resourceName: string
) {
	try {
		const response = await client.get(
			`/api/problems/${problemId}/resources/${resourceName}`,
			{
				responseType: "blob",
			}
		);
		const url = window.URL.createObjectURL(
			new Blob([response.data as BlobPart])
		);
		const link = document.createElement("a");
		link.href = url;
		link.setAttribute("download", resourceName);
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
		window.URL.revokeObjectURL(url);
	} catch (error) {
		console.error(error);
		window.$message.error("下载失败", error);
	}
}

/**
 * Uploads a resource file for a specific problem.
 * @param problemId - The ID of the problem to upload the resource for
 * @param file - The file to be uploaded
 * @param resourceType - The type of resource being uploaded, either "input" or "answer"
 * @returns Promise that resolves to the resource name as a string
 * @throws Will throw an error if the upload request fails
 */
export async function uploadProblemResourceAsync(
	problemId: number,
	file: File,
	resourceType: "input" | "answer"
) {
	const formData = new FormData();
	formData.append("file", file);
	const url = `/api/problems/${problemId}/resources/${resourceType}`;
	const response = await client.post(url, formData);
	return response.data as string; // return the resource name
}

/**
 * Downloads a test point resource (input or answer file) for a specific problem.
 * Creates a temporary download link and automatically triggers the download.
 * 
 * @param problemId - The unique identifier of the problem
 * @param testPointId - The unique identifier of the test point
 * @param type - The type of resource to download ("input" or "answer")
 * 
 * @throws Will throw an error if the download fails
 * 
 * @example
 * ```typescript
 * Download an input file
 * await downloadProblemTestPointResource(1, 1, "input");
 * 
 * Download an answer file
 * await downloadProblemTestPointResource(1, 1, "answer");
 * ```
 */
export async function downloadProblemTestPointResource(
	problemId: number,
	testPointId: number,
	type: "input" | "answer"
) {
	try {
		const response = await client.get(
			`/api/problems/${problemId}/testpoints/${testPointId}/${type}`,
			{
				responseType: "blob",
			}
		);
		const url = window.URL.createObjectURL(
			new Blob([response.data as BlobPart])
		);
		const link = document.createElement("a");
		link.href = url;
		link.setAttribute("download", `${problemId}_${testPointId}.${type}`);
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
		window.URL.revokeObjectURL(url);
	} catch (error) {
		console.error(error);
		window.$message.error("下载失败", error);
	}
}
