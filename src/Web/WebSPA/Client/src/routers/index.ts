import {createRouter, createWebHistory} from "vue-router";
import {useUiServices} from "@/stores/ui-services";
import {useUserState} from "@/stores/user";

const routes = [
    {
        path: "/",
        name: "home",
        redirect: "/problems",
    },
    {
        path: "/authentication",
        name: "authentication",
        redirect: "/bff/login",
    },
    {
        path: "/problems",
        name: "problems",
        component: () => import("@/views/problems/Problems.vue"),
    },
    {
        path: "/submissions",
        name: "submissions",
        component: () => import("@/views/submissions/Submissions.vue"),
    },
    {
        path: "/problems/:id",
        name: "problem-detail",
        props: true,
        meta: {
            requireAuthentication: true,
        },
        component: () => import("@/views/problems/ProblemDetail.vue"),
    },
    {
        path: "/problems/create",
        name: "problem-create",
        meta: {
            requireAuthentication: true,
        },
        component: () => import("@/views/problems/ProblemEditor.vue"),
    },
    {
        path: "/problems/:id/edit",
        name: "problem-edit",
        props: true,
        meta: {
            requireAuthentication: true,
        },
        component: () => import("@/views/problems/ProblemEditor.vue"),
    },
    {
        path: "/submissions/:id",
        name: "submission-detail",
        props: true,
        component: () => import("@/views/submissions/SubmissionDetail.vue"),
    },
    {
        path: "/users/:id",
        name: "user-profile",
        props: true,
        meta: {
            requireAuthentication: true,
        },
        component: () => import("@/views/users/UserProfile.vue"),
    },
    {
        path: "/error/not-found",
        name: "not-found",
        component: () => import("@/views/error/NotFound.vue"),
    },
    {
        path: "/error/permission-denied",
        name: "permission-denied",
        component: () => import("@/views/error/PermissionDenied.vue"),
    },
];

const router = createRouter({history: createWebHistory(), routes});

router.beforeEach((to, from, next) => {
    const userState = useUserState();
    useUiServices().loadingBar?.start();
    if (to.matched.length === 0) {
        next({name: "not-found"});
    } else if (to.matched.some((record) => record.meta.requireAuthentication)) {
        if (userState.isLogin) {
            next();
        } else {
            next({name: "permission-denied"});
        }
    } else {
        next();
    }
});

router.afterEach(() => {
    useUiServices().loadingBar?.finish();
});

export default router;
