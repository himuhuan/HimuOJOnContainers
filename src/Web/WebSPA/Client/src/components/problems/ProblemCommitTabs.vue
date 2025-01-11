<script setup lang="ts">
import {onMounted, reactive, toRaw} from "vue";
import * as monaco from "monaco-editor";
import {
  NButton,
  NCard,
  NIcon,
  NSelect,
  NSpace,
  NTabPane,
  NTabs,
  SelectOption,
  useDialog,
  useThemeVars,
} from "naive-ui";

import {CodeTemplateInstance} from "@/utils/CodeTemplate.ts";
import {SaveOutlined as SaveCodeIcon, UploadFileOutlined as SubmitIcon,} from "@vicons/material";
import {ArrowReset20Regular as ResetIcon} from "@vicons/fluent";
import SubmissionList from "../submissions/SubmissionList.vue";
import {useUserState} from "@/stores/user";
import {postSubmission} from "@/services/submissionsApi";
import {mapCompilerNameToLanguage} from "@/utils/HimuTools";

const themeVars = useThemeVars();
const userState = useUserState();
const dialog = useDialog();

const languageOptions: SelectOption[] = [
  {
    label: "C++ (GCC C++ 17)",
    value: "g++",
  },
  {
    label: "C",
    value: "c",
  },
  {
    label: "Java",
    value: "java",
  },
  {
    label: "Python",
    value: "python",
  },
];

const props = defineProps({
  problemId: {
    type: String,
    required: true,
  },
});

const state = reactive({
  loading: false,
  currentTab: "code-edit",
  editor: null as monaco.editor.IStandaloneCodeEditor | null,
  fileToUpload: null as File | null,
  selectedLanguage: languageOptions[0],
  hasLocalCache: false,
  showResetModal: false,
});

const submissionListFilter = {
  problemId: props.problemId,
  submitterId: userState.id,
};

////////////////////////// function //////////////////////////


function changeTabs(tab: string | number) {
  state.currentTab = tab as string;
}

function handleHandleLanguageChange(value: string, option: SelectOption) {
  const editorRaw = toRaw(state.editor)!;
  const model = editorRaw.getModel();
  state.selectedLanguage = option;
  const language = mapCompilerNameToLanguage(value);
  monaco.editor.setModelLanguage(model!, language);
  editorRaw.setValue(CodeTemplateInstance.getTemplate(language));
}

// async function uploadSourceFile(options: UploadCustomRequestOptions) {
// 	if (options.file.file) {
// 		state.fileToUpload = options.file.file;
// 	}
// }

// function removeUploadSourceFile() {
// 	state.fileToUpload = null;
// }

async function handleSubmitCode() {
  if (state.currentTab === "code-edit") {
    const editorRaw = toRaw(state.editor)!;
    const model = editorRaw.getModel();
    const code = model!.getValue();
    state.loading = true;
    await postSubmission({
      problemId: Number(props.problemId),
      sourceCode: code,
      compilerName: state.selectedLanguage.value! as string,
    });
    state.loading = false;
  }
  state.currentTab = "commits";
}

function handleSaveCode() {
  const editorRaw = toRaw(state.editor)!;
  const model = editorRaw.getModel();
  const code = model!.getValue();
  localStorage.setItem("himuoj_local_submits_code", code);
  localStorage.setItem(
      "himuoj_local_submits_language",
      state.selectedLanguage.value as string
  );
  state.hasLocalCache = true;
  window.$message.success("代码已保存至本地");
}

function handleResetCode() {
  dialog.info({
    title: "还原代码",
    content:
        "你确定要还原代码吗？这将会清空你的当前编辑器内容，并清空本地缓存。",
    positiveText: "确定",
    negativeText: "取消",
    autoFocus: false,
    onPositiveClick: () => {
      const editorRaw = toRaw(state.editor)!;
      const model = editorRaw.getModel();
      const language = mapCompilerNameToLanguage(
          state.selectedLanguage.value as string
      );
      const code = CodeTemplateInstance.getTemplate(language);
      model!.setValue(code);
      localStorage.removeItem("himuoj_local_submits_code");
      localStorage.removeItem("himuoj_local_submits_language");
      state.hasLocalCache = false;
      window.$message.success("代码已还原");
    },
  });
}

onMounted(async () => {
  const code = localStorage.getItem("himuoj_local_submits_code");
  const language = localStorage.getItem("himuoj_local_submits_language");

  if (code) {
    state.hasLocalCache = true;
  }

  state.editor = monaco.editor.create(document.getElementById("user-editor")!, {
    value: code ? code : CodeTemplateInstance.getTemplate("cpp"),
    language: language ? mapCompilerNameToLanguage(language) : "cpp",
    fontSize: 14,
    fontLigatures: true,
    automaticLayout: true,
    lineHeight: 1.5,
    fontFamily: themeVars.value.fontFamilyMono,
    lineNumbers: "on",
    roundedSelection: false,
    scrollBeyondLastLine: false,
    readOnly: false,
    theme: userState.perferTheme === "dark" ? "vs-dark" : "vs",
  });
  if (language) {
    state.selectedLanguage = languageOptions.find((x) => x.value === language)!;
    console.log(state.selectedLanguage);
  }
});
</script>

<template>
  <div class="problem-commit-tabs-container">
    <n-tabs
        default-value="code-edit"
        :on-update:value="changeTabs"
        :value="state.currentTab"
        animated
    >
      <template #suffix>
        <transition name="slide-fade">
          <n-space v-if="state.currentTab === 'code-edit'">
            <n-select
                :value="state.selectedLanguage.value"
                filterable
                placeholder="选择语言..."
                :options="languageOptions"
                style="width: 200px"
                :on-update:value="handleHandleLanguageChange"
            ></n-select>
            <n-button
                type="primary"
                :loading="state.loading"
                @click="handleSubmitCode"
            >
              <n-icon size="medium" :component="SubmitIcon"/> &nbsp; 提交
            </n-button>
            <n-button type="primary" secondary @click="handleSaveCode">
              <n-icon size="medium" :component="SaveCodeIcon"/> &nbsp; 保存
            </n-button>
            <n-button
                @click="handleResetCode"
                type="primary"
                tertiary
                v-if="state.hasLocalCache"
            >
              <n-icon size="medium" :component="ResetIcon"/> &nbsp; 还原
            </n-button>
          </n-space>
        </transition>
      </template>
      <n-tab-pane
          name="code-edit"
          tab="代码"
          display-directive="show"
          style="height: 90vh; width: 100%"
      >
        <n-card bordered class="monaco-editor" id="user-editor"></n-card>
      </n-tab-pane>
      <!--- Removed for now
      <n-tab-pane name="source" tab="源文件">
        <n-space vertical>
          <n-alert title="安全警告" type="error" closable>
            <template #icon>
              <n-icon size="larger" :component="WarningIcon" />
            </template>
            请不要试图上传一切可能危害服务器的代码或者文件。<br />
            否则，一旦被发现，你的账号将会被永久封禁。
          </n-alert>
          <n-alert title="注意" type="info" closable>
            请注意，你上传的文件必须与你选择的语言(
            {{ state.selectedLanguage?.label }} )相匹配，否则将会导致编译错误。
          </n-alert>
          <n-upload
            :custom-request="uploadSourceFile"
            :on-remove="removeUploadSourceFile"
            :max="1"
          >
            <n-upload-dragger>
              <div style="margin-bottom: 12px">
                <n-icon size="large" :depth="3">
                  <submit-icon />
                </n-icon>
                <n-text style="font-size: 16px">
                  点击或者拖动文件到该区域来上传
                </n-text>
                <n-p depth="3" style="margin: 8px 0 0 0">
                  请仔细阅读上方的安全警告和注意事项。
                </n-p>
              </div>
            </n-upload-dragger>
          </n-upload>
          <transition name="fade">
            <n-alert
              v-if="state.fileToUpload != null"
              title="就绪"
              type="success"
              closable
            >
              你已经准备好了上传文件。请点击上方的上传按钮来上传你的文件。
            </n-alert>
          </transition>
        </n-space>
      </n-tab-pane>
      -->
      <n-tab-pane name="commits" tab="提交">
        <submission-list
            :show-submission-id="false"
            :filter="submissionListFilter"
        />
      </n-tab-pane>
    </n-tabs>
  </div>
</template>

<style scoped>
.problem-commit-tabs-container {
  padding: 24px;
  height: 100vh;
}

.monaco-editor {
  height: 100%;
  width: 100%;
  resize: both;
  overflow: auto;
  align-items: center;
}
</style>
