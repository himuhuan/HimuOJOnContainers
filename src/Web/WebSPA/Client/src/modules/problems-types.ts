
export interface ProblemListRequest {
    page: number;
    pageSize: number;    
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
 * GET /problems/{problemId}
 * 
 * @property {string} title - The title of the problem.
 * @property {string} content - The content of the problem.
 * @property {string} createTime - The time when the problem was created.
 * @property {DefaultResourceLimit} defaultResourceLimit - The default resource limit for the problem.
 */
export interface ProblemDetail {
    title: string
    content: string
    createTime: string
    defaultResourceLimit: DefaultResourceLimit
}

