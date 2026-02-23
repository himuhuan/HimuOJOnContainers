import type {UserProfile} from "@/modules/user-types.ts";
import client from "@/modules/HttpClient";
import {defineStore} from "pinia";
import {type UserClaim} from "../modules/user-types.ts";

interface UserState {
    profile: UserProfile | undefined;
    localSettings: {
        perferTheme: "light" | "dark";
    };
}

const mappedClaimKeys = {
    id: ["sub", "nameidentifier"],
    userName: ["unique_name", "preferred_username", "name"],
    email: ["email"],
    avatar: ["avatar", "picture"],
    logoutUrl: ["bff:logout_url", "logout_url"],
    roles: ["role", "roles"],
} as const;

function getUserClaimValue(user: UserProfile | undefined, names: readonly string[]) {
    if (!user || !user.isLogin) return undefined;
    for (const name of names) {
        const value = user.claims.find((x) => x.type === name)?.value;
        if (value) {
            return value;
        }
    }
    return undefined;
}

function getUserClaimValues(user: UserProfile | undefined, names: readonly string[]) {
    if (!user || !user.isLogin) return [];
    return user.claims.filter((x) => names.includes(x.type)).map((x) => x.value);
}

function parseRoleValues(rawClaimValue: string): string[] {
    const normalized = rawClaimValue.trim();
    if (!normalized) {
        return [];
    }

    if (normalized.startsWith("[") && normalized.endsWith("]")) {
        try {
            const parsed = JSON.parse(normalized);
            if (Array.isArray(parsed)) {
                return parsed
                    .map((x) => String(x).trim())
                    .filter((x) => x.length > 0);
            }
        } catch {
            // Fallback to split mode below when claim is not valid JSON.
        }
    }

    return normalized.split(/[\s,]+/).filter((x) => x.length > 0);
}

function getUserRoles(user: UserProfile | undefined) {
    const rawRoleValues = getUserClaimValues(user, mappedClaimKeys.roles);
    const mappedRoles = rawRoleValues.flatMap((x) => parseRoleValues(x));
    return [...new Set(mappedRoles)];
}

export const useUserState = defineStore("user", {
    state: (): UserState => ({
        profile: undefined,
        localSettings: {
            perferTheme: "light",
        },
    }),
    actions: {
        async fetchProfile() {
            try {
                const resp = await client.get<UserClaim[]>("/bff/user");
                this.profile = {
                    isLogin: true,
                    claims: resp.data,
                };
                console.log("Welcome to HimuOJ!", this.profile);
                console.log("role", this.roles);
            } catch (error: any) {
                if (error?.status === 401) {
                    this.profile = {
                        isLogin: false,
                        claims: [],
                    };
                    console.log("user not login");
                    return;
                }

                throw new Error("fetch user profile failed");
            }
        },

        triggerThemeChange() {
            this.localSettings.perferTheme =
                this.localSettings.perferTheme === "light" ? "dark" : "light";
        },
    },
    getters: {
        isLogin: (state) => state.profile?.isLogin ?? false,
        claims: (state) => state.profile?.claims ?? [],
        id: (state) => getUserClaimValue(state.profile, mappedClaimKeys.id),
        userName: (state) => getUserClaimValue(state.profile, mappedClaimKeys.userName),
        userEmail: (state) => getUserClaimValue(state.profile, mappedClaimKeys.email),
        userAvatar: (state) => getUserClaimValue(state.profile, mappedClaimKeys.avatar),
        userLogoutUrl: (state) =>
            getUserClaimValue(state.profile, mappedClaimKeys.logoutUrl) ?? "/bff/logout",
        perferTheme: (state) => state.localSettings.perferTheme,
        roles: (state) => getUserRoles(state.profile),
        isDistributor: (state) => {
            const roles = getUserRoles(state.profile);
            return roles.includes("Distributor") || roles.includes("Administrator");
        },
    },
});
