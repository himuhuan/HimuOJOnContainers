export interface ResourceUsage {
    usedMemoryByte: number;
    usedTimeMs: number;
}

export interface SubmissionListRequest {
    page: number;
    pageSize: number;

    problemId?: number;
    submitterId?: string;
}

export interface SubmissionListItem {
    id: number;
    submitterId: string;
    submitterName: string;
    submitterAvatar: string;

    problemId: number | null;
    problemTitle: string;

    submitTime: string;
    compilerName: string;
    usage: ResourceUsage | null;
    status: string;
}

export interface SubmissionList {
    total: number;
    pageCount: number;
    items: SubmissionListItem[];
}

export interface CreateSubmissionRequest {
    problemId: number;
    sourceCode: string;
    compilerName: string;
}

export interface CreateSubmissionResponse {
    id: number;
}

export interface OutputDifference {
    expectedOutput?: string;
    actualOutput?: string;
    position: number;
}

export interface TestPointResult {
    submissionId: number;
    testPointId: number;
    status: string;
    usage?: ResourceUsage;
    difference: OutputDifference;
    id: number;
}

export interface SubmissionDetail {
    submitterName?: string;
    submitterAvatar?: string;
    problemTitle?: string;
    id: number;
    problemId?: number;
    usage?: ResourceUsage;
    status: string;
    submitterId?: string;
    submitTime: string;
    compilerName: string;
    statusMessage?: string;
    sourceCode: string;
    testPointResults: TestPointResult[];
}
