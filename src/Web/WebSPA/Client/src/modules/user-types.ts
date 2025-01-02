export interface UserClaim {
	type: string;
	value: string;
}

export interface UserProfile {
	isLogin: boolean;
	claims: UserClaim[];
}


