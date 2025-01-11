/**
 * To avoid annoying warnings for some ides
 */
export function usedVariables(v: any) {
    return v;
}

export function utc2LoaclDate(utc: any): Date {
    let date = (typeof utc === "string") ? new Date(utc + "Z") : new Date(utc);
    return new Date(date.toLocaleString());
}

export function dateDiffLocal(lhs: Date, rhs: Date): [number, string] {
    const countdown = lhs.getTime() - rhs.getTime();
    const countdownAbs = Math.abs(countdown);
    const seconds = Math.floor(countdownAbs / 1000);
    const days = Math.floor(seconds / (24 * 3600));
    const hours = Math.floor((seconds % (24 * 3600)) / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    let result = '';
    if (days > 0) result += `${days}天`;
    if (hours > 0) result += `${hours}小时`;
    if (minutes > 0) result += `${minutes}分钟`;
    if (secs > 0 || result === '') result += `${secs}秒`;
    return [countdown, result];
}

/**
 * Combine paths into a single path
 * @param paths the paths to combine
 */
export function combinePath(...paths: string[]): string {
    return paths.join('/').replace(/\/+/g, '/');
}

/**
 * Maps a given compiler name to its corresponding programming language.
 *
 * @param compilerName - The name of the compiler to map.
 * @returns The corresponding programming language for the given compiler name.
 *          If the compiler name is "g++" or "clang++", it returns "cpp".
 *          Otherwise, it returns the original compiler name.
 */
export function mapCompilerNameToLanguage(compilerName: string) {
    switch (compilerName) {
        case "g++":
        case "clang++":
            return "cpp";
        default:
            return compilerName;
    }
}

export function isAcceptedStatus(status: string) {
    return status === "Accepted";
}

export function isRejectedStatus(status: string) {
    return status !== "PendingOrSkipped" && status !== "Accepted";
}

export function isPendingOrSkippedStatus(status: string) {
    return status === "PendingOrSkipped";
}

export function isWrongAnswerStatus(status: string) {
    return status === "WrongAnswer";
}

export function isTimeLimitExceededStatus(status: string) {
    return status === "TimeLimitExceeded";
}

export function isMemoryLimitExceededStatus(status: string) {
    return status === "MemoryLimitExceeded";
}