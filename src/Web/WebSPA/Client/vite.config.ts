import {defineConfig} from "vite";
import AutoImport from 'unplugin-auto-import/vite'
import {NaiveUiResolver} from 'unplugin-vue-components/resolvers'
import Components from 'unplugin-vue-components/vite'
import vue from "@vitejs/plugin-vue";
import path, {resolve} from "path";
import fs from "fs";
import child_process from "child_process";
import {env} from "process";

// For Asp.Net development environment

const useHttps = false; // Disable https if running in docker-compose

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ""
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "webspa.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (
        0 !==
        child_process.spawnSync(
            "dotnet",
            [
                "dev-certs",
                "https",
                "--export-path",
                certFilePath,
                "--format",
                "Pem",
                "--no-password",
            ],
            {stdio: "inherit"}
        ).status
    ) {
        throw new Error("Could not create certificate.");
    }
}

const target = useHttps ? "https://localhost:6001" : "http://localhost:6000";

export default defineConfig({
    plugins: [
        vue(),
        AutoImport({
            imports: [
                'vue',
                {
                    'naive-ui': [
                        'useDialog',
                        'useMessage',
                        'useNotification',
                        'useLoadingBar'
                    ]
                }
            ]
        }),
        Components({
            resolvers: [NaiveUiResolver()]
        })
    ],
    resolve: {
        alias: {
            "@": resolve(__dirname, "src"),
            "#": resolve(__dirname, "src/components"),
        },
    },
    optimizeDeps: {
        include: [
            `monaco-editor/esm/vs/language/json/json.worker`,
            `monaco-editor/esm/vs/language/css/css.worker`,
            `monaco-editor/esm/vs/language/html/html.worker`,
            `monaco-editor/esm/vs/language/typescript/ts.worker`,
            `monaco-editor/esm/vs/editor/editor.worker`,
        ],
    },
    server: {
        proxy: {
            "^/bff": {
                target,
                secure: false,
            },
            "^/signin-oidc": {
                target,
                secure: false,
            },
            "^/signout-callback-oidc": {
                target,
                secure: false,
            },
            "^/api": {
                target,
                secure: false,
            },
            "^/submitshub": {
                target,
                secure: false,
                ws: true,
            },
            // static files
            "^/static": {
                target,
                secure: false,
            },
        },
        port: 6100,
        // https: useHttps && {
        //   key: fs.readFileSync(keyFilePath),
        //   cert: fs.readFileSync(certFilePath),
        // },
    },
});
