import {authHandlers} from "@/mocks/handlers/auth";
import {problemHandlers} from "@/mocks/handlers/problems";
import {submissionHandlers} from "@/mocks/handlers/submissions";
import {userHandlers} from "@/mocks/handlers/users";

export const handlers = [
    ...authHandlers,
    ...problemHandlers,
    ...submissionHandlers,
    ...userHandlers,
];
