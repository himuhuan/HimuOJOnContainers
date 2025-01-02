import type { UserProfile } from "@/modules/user-types.ts";
import client from "@/modules/HttpClient";
import { defineStore } from "pinia";
import { type UserClaim } from "../modules/user-types.ts";

interface UserState {
	profile: UserProfile | undefined;
	localSettings: {
		perferTheme: "light" | "dark";
	};
}

function getUserClaimValue(user: UserProfile | undefined, name: string) {
	if (!user || !user.isLogin) return undefined;
	return user.claims.find((x) => x.type === name)?.value ?? undefined;
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
			const resp = await client.get<UserClaim[]>("/bff/user");
			if (resp.status !== 200) {
                if (resp.status === 401) {
                    this.profile = {
                        isLogin: false,
                        claims: [],
                    };
                    console.log("user not login");
                    return;
                }
                throw new Error("fetch user profile failed");
            }
			this.profile = {
				isLogin: true,
				claims: resp.data,
			};
			console.log("user profile fetched", this.profile);
		},

		triggerThemeChange() {
			this.localSettings.perferTheme =
				this.localSettings.perferTheme === "light" ? "dark" : "light";
		},
	},
	getters: {
		isLogin: (state) => state.profile?.isLogin ?? false,
		claims: (state) => state.profile?.claims ?? [],
		id: (state) => getUserClaimValue(state.profile, "sub"),
		userName: (state) => getUserClaimValue(state.profile, "unique_name"),
		userEmail: (state) => getUserClaimValue(state.profile, "email"),
		userAvatar: (state) => getUserClaimValue(state.profile, "avatar"),
		userLogoutUrl: (state) =>
			getUserClaimValue(state.profile, "bff:logout_url"),
		perferTheme: (state) => state.localSettings.perferTheme,
	},
});
