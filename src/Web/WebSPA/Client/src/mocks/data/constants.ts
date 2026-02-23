export type MockAuthRole = "guest" | "user" | "distributor" | "admin";

export const MOCK_AUTH_ROLE_STORAGE_KEY = "himuoj_mock_auth_role";

export function normalizeMockAuthRole(role: string | null | undefined): MockAuthRole {
    if (role === "user" || role === "distributor" || role === "admin") {
        return role;
    }

    return "guest";
}

export function getMockAuthRole(): MockAuthRole {
    if (typeof window === "undefined") {
        return "guest";
    }

    const role = localStorage.getItem(MOCK_AUTH_ROLE_STORAGE_KEY);
    return normalizeMockAuthRole(role);
}

export function setMockAuthRole(role: MockAuthRole) {
    if (typeof window === "undefined") {
        return;
    }

    localStorage.setItem(MOCK_AUTH_ROLE_STORAGE_KEY, role);
}
