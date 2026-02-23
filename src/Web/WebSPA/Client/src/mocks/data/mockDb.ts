import type {
    ProblemDetail,
    ProblemDto,
    ProblemList,
    ProblemListItem,
    ProblemManageList,
    ProblemManageListItem,
    ProblemVo,
    TestPoint,
} from "@/modules/problems-types";
import type {
    CreateSubmissionRequest,
    ResourceUsage,
    SubmissionDetail,
    SubmissionList,
    SubmissionListItem,
    TestPointResult,
} from "@/modules/submits-type";
import type {UserClaim, UserDetail, UserRegisterRequest} from "@/modules/user-types";
import type {MockAuthRole} from "./constants";

interface MockProblemRecord extends ProblemVo {
    distributorId: string;
}

interface MockSubmissionRecord {
    id: number;
    problemId: number;
    submitterId: string;
    submitTime: string;
    compilerName: string;
    status: string;
    usage: ResourceUsage | null;
    statusMessage: string;
    sourceCode: string;
    testPointResults: TestPointResult[];
}

function paginate<T>(items: T[], page: number, pageSize: number) {
    const normalizedPage = Number.isFinite(page) && page > 0 ? page : 1;
    const normalizedSize = Number.isFinite(pageSize) && pageSize > 0 ? pageSize : 10;

    const total = items.length;
    const pageCount = Math.max(1, Math.ceil(total / normalizedSize));
    const start = (normalizedPage - 1) * normalizedSize;
    const end = start + normalizedSize;

    return {
        total,
        pageCount,
        items: items.slice(start, end),
    };
}

function createSeedUsers() {
    return new Map<string, UserDetail>([
        [
            "u1001",
            {
                userId: "u1001",
                userName: "alice",
                email: "alice@example.com",
                avatar: "https://api.dicebear.com/9.x/shapes/svg?seed=alice",
                registerDate: "2025-01-10",
                lastLoginDate: "2026-02-20",
                totalSubmissionCount: 0,
                acceptedSubmissionCount: 0,
                totalProblemTriedCount: 0,
                acceptedProblemCount: 0,
            },
        ],
        [
            "u2001",
            {
                userId: "u2001",
                userName: "distributor_tom",
                email: "tom@example.com",
                avatar: "https://api.dicebear.com/9.x/shapes/svg?seed=tom",
                registerDate: "2024-11-02",
                lastLoginDate: "2026-02-19",
                totalSubmissionCount: 0,
                acceptedSubmissionCount: 0,
                totalProblemTriedCount: 0,
                acceptedProblemCount: 0,
            },
        ],
        [
            "u0001",
            {
                userId: "u0001",
                userName: "admin",
                email: "admin@example.com",
                avatar: "https://api.dicebear.com/9.x/shapes/svg?seed=admin",
                registerDate: "2024-01-01",
                lastLoginDate: "2026-02-21",
                totalSubmissionCount: 0,
                acceptedSubmissionCount: 0,
                totalProblemTriedCount: 0,
                acceptedProblemCount: 0,
            },
        ],
    ]);
}

function createSeedProblems() {
    const now = Date.now();
    const problems: MockProblemRecord[] = [];

    for (let i = 1; i <= 24; i++) {
        const createTime = new Date(now - i * 86400000).toISOString();
        const testPoints: TestPoint[] = [
            {
                id: i * 100 + 1,
                problemId: i,
                resourceType: "Text",
                input: `${i} ${i + 1}`,
                expectedOutput: `${i + i + 1}`,
                remarks: "基础样例",
            },
            {
                id: i * 100 + 2,
                problemId: i,
                resourceType: "Text",
                input: `${i * 10} ${i * 10 + 2}`,
                expectedOutput: `${i * 20 + 2}`,
                remarks: "边界样例",
            },
        ];

        problems.push({
            id: i,
            title: `Mock Problem #${i}`,
            content:
                `# 题目 ${i}\n\n` +
                "给定两个整数 a 和 b，输出它们的和。\n\n" +
                "输入：一行两个整数。\n\n" +
                "输出：一个整数。",
            maxMemoryLimitByte: 134217728,
            maxRealTimeLimitMilliseconds: 1000 + (i % 3) * 500,
            allowDownloadInput: i % 2 === 0,
            allowDownloadAnswer: i % 3 === 0,
            createTime,
            lastModifyTime: createTime,
            testPoints,
            distributorId: i % 2 === 0 ? "u2001" : "u0001",
        });
    }

    return problems;
}

function createTestPointResults(problem: MockProblemRecord, status: string): TestPointResult[] {
    return problem.testPoints.map((testPoint, idx) => {
        const accepted = status === "Accepted";
        return {
            id: idx + 1,
            submissionId: 0,
            testPointId: testPoint.id,
            status: accepted ? "Accepted" : idx === 0 ? status : "PendingOrSkipped",
            usage: accepted
                ? {
                    usedMemoryByte: 5_000_000 + idx * 500_000,
                    usedTimeMs: 20 + idx * 5,
                }
                : idx === 0
                    ? {
                        usedMemoryByte: 6_000_000,
                        usedTimeMs: 40,
                    }
                    : undefined,
            difference:
                accepted || idx !== 0
                    ? {position: 0}
                    : {
                        position: 2,
                        expectedOutput: testPoint.expectedOutput,
                        actualOutput: `${Number(testPoint.expectedOutput || 0) + 1}`,
                    },
        };
    });
}

function createSeedSubmissions(problems: MockProblemRecord[]) {
    const statuses = [
        "Accepted",
        "WrongAnswer",
        "TimeLimitExceeded",
        "CompilationError",
        "Running",
        "Pending",
    ];

    const submitters = ["u1001", "u2001", "u0001"];
    const submissions: MockSubmissionRecord[] = [];

    for (let i = 1; i <= 60; i++) {
        const problem = problems[(i - 1) % problems.length];
        const status = statuses[i % statuses.length];
        const submitTime = new Date(Date.now() - i * 3600000).toISOString();
        const usage =
            status === "Running" || status === "Pending"
                ? null
                : {
                    usedMemoryByte: 4_000_000 + (i % 7) * 800_000,
                    usedTimeMs: 10 + (i % 9) * 12,
                };

        const result = createTestPointResults(problem, status === "WrongAnswer" ? "WrongAnswer" : "Accepted");
        submissions.push({
            id: i,
            problemId: problem.id,
            submitterId: submitters[i % submitters.length],
            submitTime,
            compilerName: i % 4 === 0 ? "python" : i % 3 === 0 ? "java" : "g++",
            status,
            usage,
            statusMessage:
                status === "WrongAnswer"
                    ? "Wrong answer on test point 1"
                    : status === "TimeLimitExceeded"
                        ? "Time limit exceeded"
                        : status === "CompilationError"
                            ? "main.cpp:3: error: expected ';'"
                            : status === "Running"
                                ? "Judging..."
                                : status === "Pending"
                                    ? "Queued..."
                                    : "Accepted",
            sourceCode:
                "#include <bits/stdc++.h>\nusing namespace std;\nint main(){ long long a,b; cin>>a>>b; cout<<a+b; }",
            testPointResults: result,
        });
    }

    return submissions;
}

const users = createSeedUsers();
const problems = createSeedProblems();
const submissions = createSeedSubmissions(problems);

let submissionAutoId = submissions.length + 1;
let problemAutoId = problems.length + 1;

function refreshUserStats() {
    for (const user of users.values()) {
        const userSubmissions = submissions.filter((x) => x.submitterId === user.userId);
        const triedProblems = new Set(userSubmissions.map((x) => x.problemId));
        const acceptedProblems = new Set(
            userSubmissions.filter((x) => x.status === "Accepted").map((x) => x.problemId)
        );

        user.totalSubmissionCount = userSubmissions.length;
        user.acceptedSubmissionCount = userSubmissions.filter((x) => x.status === "Accepted").length;
        user.totalProblemTriedCount = triedProblems.size;
        user.acceptedProblemCount = acceptedProblems.size;
    }
}

function findProblemById(problemId: number) {
    return problems.find((x) => x.id === problemId);
}

function findSubmissionById(submissionId: number) {
    return submissions.find((x) => x.id === submissionId);
}

function refreshRunningSubmissions() {
    for (const submission of submissions) {
        if (submission.status !== "Running" && submission.status !== "Pending") {
            continue;
        }

        const elapsed = Date.now() - new Date(submission.submitTime).getTime();
        if (elapsed < 15000) {
            continue;
        }

        submission.status = elapsed % 2 === 0 ? "Accepted" : "WrongAnswer";
        submission.usage = {
            usedMemoryByte: 7_000_000,
            usedTimeMs: 55,
        };
        submission.statusMessage = submission.status === "Accepted" ? "Accepted" : "Wrong answer on test point 1";
    }
}

export function getProblemList(page: number, pageSize: number): ProblemList {
    refreshRunningSubmissions();

    const problemItems: ProblemListItem[] = problems.map((problem) => {
        const problemSubmissions = submissions.filter((x) => x.problemId === problem.id);
        const acceptedCount = problemSubmissions.filter((x) => x.status === "Accepted").length;
        return {
            id: problem.id,
            title: problem.title,
            totalSubmissionCount: problemSubmissions.length,
            acceptedSubmissionCount: acceptedCount,
        };
    });

    return paginate(problemItems, page, pageSize);
}

export function getProblemDetail(problemId: number): ProblemDetail | undefined {
    const problem = findProblemById(problemId);
    if (!problem) {
        return undefined;
    }

    return {
        title: problem.title,
        content: problem.content,
        createTime: problem.createTime,
        defaultResourceLimit: {
            maxMemoryLimitByte: problem.maxMemoryLimitByte,
            maxRealTimeLimitMilliseconds: problem.maxRealTimeLimitMilliseconds,
        },
    };
}

export function getProblemVo(problemId: number): ProblemVo | undefined {
    const problem = findProblemById(problemId);
    if (!problem) {
        return undefined;
    }

    return structuredClone(problem);
}

export function getProblemManageList(
    page: number,
    pageSize: number,
    distributorId?: string
): ProblemManageList {
    const filtered = distributorId
        ? problems.filter((x) => x.distributorId === distributorId)
        : problems;

    const items: ProblemManageListItem[] = filtered.map((problem) => ({
        id: problem.id,
        title: problem.title,
        createTime: problem.createTime,
        lastModifyTime: problem.lastModifyTime,
        defaultResourceLimit: {
            maxMemoryLimitByte: problem.maxMemoryLimitByte,
            maxRealTimeLimitMilliseconds: problem.maxRealTimeLimitMilliseconds,
        },
        guestAccessLimit: {
            allowDownloadInput: problem.allowDownloadInput,
            allowDownloadOutput: problem.allowDownloadAnswer,
        },
    }));

    return paginate(items, page, pageSize);
}

export function createProblem(request: ProblemDto) {
    const now = new Date().toISOString();
    const id = problemAutoId++;

    const mappedTestPoints = request.testPoints.map((x, idx) => ({
        ...x,
        id: idx + 1,
        problemId: id,
    }));

    const problem: MockProblemRecord = {
        id,
        title: request.title,
        content: request.content,
        maxMemoryLimitByte: request.maxMemoryLimitByte,
        maxRealTimeLimitMilliseconds: request.maxRealTimeLimitMilliseconds,
        allowDownloadInput: request.allowDownloadInput,
        allowDownloadAnswer: request.allowDownloadAnswer,
        createTime: now,
        lastModifyTime: now,
        testPoints: mappedTestPoints,
        distributorId: "u2001",
    };

    problems.unshift(problem);
    return id;
}

export function updateProblem(problemId: number, request: ProblemDto) {
    const problem = findProblemById(problemId);
    if (!problem) {
        return false;
    }

    const maxId = problem.testPoints.reduce((max, current) => Math.max(max, current.id), 0);
    let nextTestPointId = maxId + 1;

    problem.title = request.title;
    problem.content = request.content;
    problem.maxMemoryLimitByte = request.maxMemoryLimitByte;
    problem.maxRealTimeLimitMilliseconds = request.maxRealTimeLimitMilliseconds;
    problem.allowDownloadInput = request.allowDownloadInput;
    problem.allowDownloadAnswer = request.allowDownloadAnswer;
    problem.lastModifyTime = new Date().toISOString();
    problem.testPoints = request.testPoints.map((x) => ({
        ...x,
        id: x.id === 0 ? nextTestPointId++ : x.id,
        problemId,
    }));

    return true;
}

export function deleteProblem(problemId: number) {
    const idx = problems.findIndex((x) => x.id === problemId);
    if (idx < 0) {
        return false;
    }

    problems.splice(idx, 1);
    return true;
}

export function deleteProblemTestPoints(problemId: number, testPointIds: number[]) {
    const problem = findProblemById(problemId);
    if (!problem) {
        return false;
    }

    problem.testPoints = problem.testPoints.filter((x) => !testPointIds.includes(x.id));
    problem.lastModifyTime = new Date().toISOString();
    return true;
}

export function uploadProblemResource(problemId: number, resourceType: "input" | "answer") {
    const problem = findProblemById(problemId);
    if (!problem) {
        return undefined;
    }

    const fileName = `${Math.floor(Date.now() / 1000)}.${resourceType}`;
    return fileName;
}

export function getProblemResourceContent(problemId: number, resourceName: string) {
    const problem = findProblemById(problemId);
    if (!problem) {
        return undefined;
    }

    return `Mock resource for problem ${problemId}: ${resourceName}`;
}

export function getProblemTestPointResourceContent(
    problemId: number,
    testPointId: number,
    type: "input" | "answer"
) {
    const problem = findProblemById(problemId);
    if (!problem) {
        return undefined;
    }

    const testPoint = problem.testPoints.find((x) => x.id === testPointId);
    if (!testPoint) {
        return undefined;
    }

    if (testPoint.resourceType === "Text") {
        return type === "input" ? testPoint.input : testPoint.expectedOutput;
    }

    const fileName = type === "input" ? testPoint.input : testPoint.expectedOutput;
    return `Mock file content: ${fileName}`;
}

function toSubmissionListItem(submission: MockSubmissionRecord): SubmissionListItem {
    const problem = findProblemById(submission.problemId);
    const user = users.get(submission.submitterId);
    return {
        id: submission.id,
        submitterId: submission.submitterId,
        submitterName: user?.userName ?? "未知用户",
        submitterAvatar: user?.avatar ?? "",
        problemId: submission.problemId,
        problemTitle: problem?.title ?? "未知题目",
        submitTime: submission.submitTime,
        compilerName: submission.compilerName,
        usage: submission.usage,
        status: submission.status,
    };
}

export function getSubmissionList(
    page: number,
    pageSize: number,
    problemId?: number,
    submitterId?: string
): SubmissionList {
    refreshRunningSubmissions();

    const filtered = submissions
        .filter((submission) => {
            if (problemId && submission.problemId !== problemId) {
                return false;
            }
            if (submitterId && submission.submitterId !== submitterId) {
                return false;
            }
            return true;
        })
        .sort((lhs, rhs) => rhs.id - lhs.id)
        .map((submission) => toSubmissionListItem(submission));

    return paginate(filtered, page, pageSize);
}

export function getSubmissionDetail(submissionId: number): SubmissionDetail | undefined {
    refreshRunningSubmissions();
    const submission = findSubmissionById(submissionId);
    if (!submission) {
        return undefined;
    }

    const user = users.get(submission.submitterId);
    const problem = findProblemById(submission.problemId);

    return {
        id: submission.id,
        problemId: submission.problemId,
        problemTitle: problem?.title,
        problemAllowDownloadInput: problem?.allowDownloadInput,
        problemAllowDownloadAnswer: problem?.allowDownloadAnswer,
        submitterId: submission.submitterId,
        submitterName: user?.userName,
        submitterAvatar: user?.avatar,
        submitTime: submission.submitTime,
        compilerName: submission.compilerName,
        status: submission.status,
        statusMessage: submission.statusMessage,
        usage: submission.usage ?? undefined,
        sourceCode: submission.sourceCode,
        testPointResults: submission.testPointResults.map((x) => ({
            ...x,
            submissionId: submission.id,
        })),
    };
}

export function createSubmission(request: CreateSubmissionRequest) {
    const problem = findProblemById(request.problemId);
    if (!problem) {
        return undefined;
    }

    const id = submissionAutoId++;
    const submitTime = new Date().toISOString();

    const submission: MockSubmissionRecord = {
        id,
        problemId: problem.id,
        submitterId: "u1001",
        submitTime,
        compilerName: request.compilerName,
        status: "Running",
        usage: null,
        statusMessage: "Judging...",
        sourceCode: request.sourceCode,
        testPointResults: problem.testPoints.map((x, idx) => ({
            id: idx + 1,
            submissionId: id,
            testPointId: x.id,
            status: "PendingOrSkipped",
            difference: {
                position: 0,
            },
        })),
    };

    submissions.unshift(submission);
    refreshUserStats();
    return id;
}

export function getUserDetail(userId: string) {
    refreshUserStats();
    const user = users.get(userId);
    if (!user) {
        return undefined;
    }

    return structuredClone(user);
}

export function registerUser(request: UserRegisterRequest) {
    const userId = `u${Math.floor(Math.random() * 9000 + 1000)}`;
    const now = new Date().toISOString().split("T")[0];
    const user: UserDetail = {
        userId,
        userName: request.userName,
        email: request.email,
        avatar: `https://api.dicebear.com/9.x/shapes/svg?seed=${encodeURIComponent(request.userName)}`,
        registerDate: now,
        lastLoginDate: now,
        totalSubmissionCount: 0,
        acceptedSubmissionCount: 0,
        totalProblemTriedCount: 0,
        acceptedProblemCount: 0,
    };

    users.set(userId, user);
    return structuredClone(user);
}

export function uploadUserAvatar(userId: string) {
    const user = users.get(userId);
    if (!user) {
        return undefined;
    }

    user.avatar = `https://api.dicebear.com/9.x/shapes/svg?seed=${encodeURIComponent(
        `${userId}-${Date.now()}`
    )}`;
    return user.avatar;
}

export function getClaimsByRole(role: MockAuthRole): UserClaim[] {
    if (role === "guest") {
        return [];
    }

    if (role === "user") {
        const user = users.get("u1001")!;
        return [
            {type: "sub", value: user.userId},
            {type: "preferred_username", value: user.userName},
            {type: "email", value: user.email},
            {type: "picture", value: user.avatar},
            {type: "roles", value: '["User"]'},
            {type: "bff:logout_url", value: "/bff/logout"},
        ];
    }

    if (role === "distributor") {
        const user = users.get("u2001")!;
        return [
            {type: "sub", value: user.userId},
            {type: "unique_name", value: user.userName},
            {type: "email", value: user.email},
            {type: "avatar", value: user.avatar},
            {type: "role", value: "Distributor"},
            {type: "bff:logout_url", value: "/bff/logout"},
        ];
    }

    const user = users.get("u0001")!;
    return [
        {type: "sub", value: user.userId},
        {type: "name", value: user.userName},
        {type: "email", value: user.email},
        {type: "picture", value: user.avatar},
        {type: "role", value: "Administrator"},
        {type: "role", value: "Distributor"},
        {type: "bff:logout_url", value: "/bff/logout"},
    ];
}

refreshUserStats();
