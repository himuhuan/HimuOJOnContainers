export interface ProblemListRequest {
    page: number;
    pageSize: number;

    // required for management api
    distributorId?: string;
}

export interface DefaultResourceLimit {
    maxMemoryLimitByte: number;
    maxRealTimeLimitMilliseconds: number;
}

export interface GuestAccessLimit {
    allowDownloadInput: boolean;
    allowDownloadOutput: boolean;
}

export interface ProblemListItem {
    id: number;
    title: string;
    totalSubmissionCount: number;
    acceptedSubmissionCount: number;
}

export interface ProblemList {
    total: number;
    pageCount: number;
    items: ProblemListItem[];
}

/**
 * Represents the detailed information about a problem.
 *
 * GET /problems/{problemId}/detail
 *
 * @property {string} title - The title of the problem.
 * @property {string} content - The content of the problem.
 * @property {string} createTime - The time when the problem was created.
 * @property {DefaultResourceLimit} defaultResourceLimit - The default resource limit for the problem.
 */
export interface ProblemDetail {
    title: string;
    content: string;
    createTime: string;
    defaultResourceLimit: DefaultResourceLimit;
}

export interface ProblemManageListItem {
    id: number;
    title: string;
    createTime: string;
    lastModifyTime: string;
    defaultResourceLimit: DefaultResourceLimit;
    guestAccessLimit: GuestAccessLimit;
}

// Get /problems/management
export interface ProblemManageList {
    total: number;
    pageCount: number;
    items: ProblemManageListItem[];
}

export interface TestPoint {

    /**
     * The unique identifier of the test point.
     * 
     * To create a new test point, set this field to 0.
     */
    id: number;
    
    problemId: number;
    input: string;
    expectedOutput: string;
    remarks: string;
}

export interface ProblemDto {
    title: string;
    content: string;
    maxMemoryLimitByte: number;
    maxRealTimeLimitMilliseconds: number;
    allowDownloadInput: boolean;
    allowDownloadAnswer: boolean;

    testPoints: TestPoint[];
}

export interface ProblemVo {
    id: number;
    title: string;
    content: string;
    maxMemoryLimitByte: number;
    maxRealTimeLimitMilliseconds: number;
    allowDownloadInput: boolean;
    allowDownloadAnswer: boolean;
    createTime: string;
    lastModifyTime: string;
    testPoints: TestPoint[];
}