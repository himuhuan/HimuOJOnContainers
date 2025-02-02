export interface UserClaim {
    type: string;
    value: string;
}

export interface UserProfile {
    isLogin: boolean;
    claims: UserClaim[];
}

export interface UserDetail {
    userId: string;
    userName: string;
    email: string;
    avatar: string;
    registerDate: string;
    lastLoginDate: string;
    totalSubmissionCount: number;
    acceptedSubmissionCount: number;
    totalProblemTriedCount: number;
    acceptedProblemCount: number;
}

export interface UserRegisterRequest {
    email: string,
    userName: string,
    password: string,
    repeatedPassword: string,
    phone?: string,
}

