<template>
	<transition name="fade">
		<div class="spining-container" v-if="state.loading">
			<n-spin></n-spin>
			<n-text> Loading...</n-text>
		</div>
	</transition>
	<div v-if="!state.loading" class="problem-view-container">
		<transition name="slide-fade">
			<n-alert
				v-if="isEdit"
				size="small"
				type="error"
				:show-icon="false"
				closable
			>
				这是一个已提交的问题，你可以任意修改题目或者测试点，但是你所做的修改将不会影响已提交的记录。
			</n-alert>
		</transition>
		<n-split
			:default-size="0.5"
			:resize-trigger-size="12"
			direction="horizontal"
			class="problem-view-container"
		>
			<template #1>
				<n-scrollbar style="height: calc(100vh - 45px - 60px)">
					<n-form
						:rules="formRules"
						:model="state.detail"
						label-placement="top"
						class="p-5"
					>
						<n-form-item label="题目标题" path="title">
							<n-input
								placeholder="标题将会显示到题目列表中"
								v-model:value="state.detail.title"
							/>
						</n-form-item>
						<n-form-item label="内存限制" path="maxMemoryLimitByte">
							<n-input-number
								placeholder="内存限制（字节）"
								style="width: 100%"
								v-model:value="state.detail.maxMemoryLimitByte"
							>
								<template #suffix> BYTE</template>
							</n-input-number>
						</n-form-item>
						<n-form-item label="时间限制" path="maxRealTimeLimitMilliseconds">
							<n-input-number
								placeholder="时间限制（毫秒）"
								style="width: 100%"
								v-model:value="state.detail.maxRealTimeLimitMilliseconds"
							>
								<template #suffix> MS</template>
							</n-input-number>
						</n-form-item>

						<div style="margin: 10px">
							<md-editor
								style="height: 80vh"
								:preview="false"
								v-model="state.detail.content"
							/>
						</div>

						<n-form-item
							label="允许下载测试点的答案"
							label-placement="left"
							inline
						>
							<n-switch v-model:value="state.detail.allowDownloadAnswer" />
						</n-form-item>
						<n-form-item
							label="允许下载测试点的输入"
							label-placement="left"
							inline
						>
							<n-switch v-model:value="state.detail.allowDownloadInput" />
						</n-form-item>
					</n-form>
				</n-scrollbar>
			</template>
			<template #2>
				<n-scrollbar style="height: calc(100vh - 45px - 60px)" class="p-5">
					<n-tabs animated justify-content="space-evenly" type="line">
						<n-tab-pane name="preview" tab="预览">
							<problem-detail-preview :detail="detail" />
						</n-tab-pane>
						<n-tab-pane name="testpoints" tab="测试点" class="pb-10 p-2">
							<n-button
								style="width: 100%"
								type="primary"
								secondary
								@click="handleAddTestPoint"
								>添加测试点</n-button
							>
							<transition-group name="slide-fade">
								<n-collapse
									v-for="(testPoint, idx) in state.detail.testPoints"
									:key="testPoint.id"
								>
									<template #header-extra>
										<n-button
											secondary
											size="small"
											v-if="!state.removedTestPoints.includes(testPoint)"
											@click="(e) => handleRemoveTestPoint(e, testPoint)"
										>
											<template #icon>
												<n-icon>
													<delete-icon />
												</n-icon>
											</template>
											删除
										</n-button>
										<n-space v-else>
											<n-tag type="error">该项将在提交后被删除</n-tag>
											<n-button
												secondary
												size="small"
												@click="
													(e) => handleRemoveTestPoint(e, testPoint, true)
												"
											>
												<template #icon>
													<n-icon>
														<restore-icon />
													</n-icon>
												</template>
												撤销删除
											</n-button>
										</n-space>
									</template>

									<n-card
										:key="testPoint.id"
										bordered
										size="small"
										class="mt-1"
										:class="{
											'removed-test-point':
												state.removedTestPoints.includes(testPoint),
											'new-test-point': testPoint.id === 0,
										}"
									>
										<n-collapse-item
											:key="testPoint.id"
											:title="testPoint.remarks"
										>
											<n-form
												:disabled="state.apiLoading"
												:model="testPoint"
												label-placement="top"
											>
												<n-form-item label="备注" path="testPointRemarks">
													<n-input v-model:value="testPoint.remarks" />
												</n-form-item>
												<n-form-item label="类型" path="testPointType">
													<n-radio-group
														:disabled="testPoint.id !== 0"
														v-model:value="testPoint.resourceType"
														:default-value="typeOptions[0].value"
													>
														<n-radio-button
															v-for="type in typeOptions"
															:key="type.value"
															:value="type.value"
															:label="type.label"
														>
														</n-radio-button>
													</n-radio-group>
												</n-form-item>
												<n-form-item label="输入" path="testPointInput">
													<n-input
														v-if="testPoint.resourceType === 'Text'"
														type="textarea"
														placeholder="测试点的输入"
														:autosize="{ minRows: 2, maxRows: 6 }"
														v-model:value="testPoint.input"
													/>
													<n-upload
														v-else
														:custom-request="handleUploadProblemResource"
														:show-file-list="false"
														:data="{
															idx: idx.toString(),
															type: 'input',
														}"
														:show-remove-button="false"
													>
														<n-upload-dragger>
															<div style="margin-bottom: 12px">
																<n-icon size="48">
																	<submit-icon />
																</n-icon>
															</div>
															<n-text style="font-size: 16px">
																点击或者拖动文件到该区域来上传.
															</n-text>
															<n-p depth="3" style="margin: 8px 0 0 0">
																文件大小不得超过
																10MB。如果该测试点已有数据，上传将会覆盖原有数据。
															</n-p>
														</n-upload-dragger>
													</n-upload>
												</n-form-item>
												<n-form-item
													:show-label="false"
													v-if="
														testPoint.resourceType === 'File' &&
														testPoint.input.length > 0
													"
												>
													<n-alert type="success" show-icon class="w-full">
														<template #header>
															已存在于
															{{ getTimeStringFromFileName(testPoint.input) }}
															上传的数据。
														</template>
														<n-button
															size="small"
															class="w-full"
															secondary
															@click.stop="
																downloadProblemResourceAsync(
																	testPoint.problemId,
																	testPoint.input
																)
															"
														>
															下载
														</n-button>
													</n-alert>
												</n-form-item>
												<n-form-item
													label="输出"
													path="testPointExpectedOutput"
												>
													<n-input
														v-if="testPoint.resourceType === 'Text'"
														type="textarea"
														placeholder="测试点的期望输出"
														:autosize="{ minRows: 2, maxRows: 6 }"
														v-model:value="testPoint.expectedOutput"
													/>
													<n-upload
														v-else
														:custom-request="handleUploadProblemResource"
														:data="{
															idx: idx.toString(),
															type: 'answer',
														}"
														:show-remove-button="false"
														:show-file-list="false"
													>
														<n-upload-dragger>
															<div style="margin-bottom: 12px">
																<n-icon size="48">
																	<submit-icon />
																</n-icon>
															</div>
															<n-text style="font-size: 16px">
																点击或者拖动文件到该区域来上传.
															</n-text>
															<n-p depth="3" style="margin: 8px 0 0 0">
																文件大小不得超过
																10MB。如果该测试点已有数据，上传将会覆盖原有数据。
															</n-p>
														</n-upload-dragger>
													</n-upload>
												</n-form-item>
												<n-form-item
													:show-label="false"
													v-if="
														testPoint.resourceType === 'File' &&
														testPoint.input.length > 0
													"
												>
													<n-alert type="success" show-icon class="w-full mt-2">
														<template #header>
															已存在于
															{{
																getTimeStringFromFileName(
																	testPoint.expectedOutput
																)
															}}
															上传的数据。
														</template>
														<n-button
															size="small"
															class="w-full"
															secondary
															@click.stop="
																downloadProblemResourceAsync(
																	testPoint.problemId,
																	testPoint.expectedOutput
																)
															"
														>
															下载
														</n-button>
													</n-alert>
												</n-form-item>
											</n-form>
										</n-collapse-item>
									</n-card>
								</n-collapse>
							</transition-group>
						</n-tab-pane>
						<n-tab-pane name="submits" tab="提交记录" v-if="isEdit">
							<submission-list
								:filter="{ problemId: Number(props.id!) }"
								:show-problem-title="false"
							/>
						</n-tab-pane>
					</n-tabs>
				</n-scrollbar>
			</template>
			<template #resize-trigger>
				<div class="resize-trigger">
					<n-icon :color="themeVars.textColor1" :size="12">
						<swap-horizontal-icon />
					</n-icon>
				</div>
			</template>
		</n-split>
	</div>
	<div class="problem-submit-banners p-3">
		<n-space>
			<!-- <n-button type="primary" secondary size="large">保存到本地</n-button> -->
			<n-button
				:disabled="state.apiLoading"
				type="primary"
				size="large"
				@click="handleSubmitProblem"
			>
				提交
			</n-button>
		</n-space>
	</div>
</template>

<script setup lang="ts">
import { ProblemDetail, TestPoint } from "@/modules/problems-types";
import {
	NIcon,
	NSpin,
	NSplit,
	NText,
	NP,
	useThemeVars,
	NSpace,
	NButton,
	NScrollbar,
	NForm,
	NFormItem,
	NInputNumber,
	NInput,
	NTabs,
	NTag,
	NTabPane,
	NSwitch,
	FormRules,
	NAlert,
	NUpload,
	NUploadDragger,
	useLoadingBar,
	NRadioGroup,
	NRadioButton,
	NCollapse,
	NCollapseItem,
	NCard,
	UploadCustomRequestOptions,
	useDialog,
} from "naive-ui";
import { computed, onMounted, ref } from "vue";
import { SwapHorizontal as SwapHorizontalIcon } from "@vicons/ionicons5";
import {
	Delete20Regular as DeleteIcon,
	ArrowForward20Regular as RestoreIcon,
	DocumentArrowUp20Regular as SubmitIcon,
} from "@vicons/fluent";
import { MdEditor } from "md-editor-v3";
import "md-editor-v3/lib/style.css";

import ProblemDetailPreview from "@/components/problems/ProblemDetailPreview.vue";
import SubmissionList from "@/components/submissions/SubmissionList.vue";
import {
	createProblemAsync,
	getFullProblemAsync,
	downloadProblemResourceAsync,
	updateProblemAsync,
	uploadProblemResourceAsync,
} from "@/services/problemsApi";
import router from "@/routers";

const state = ref({
	loading: false,
	apiLoading: false,
	detail: {
		title: "新创建问题",
		content: "> 请在这里输入题目描述",
		createTime: new Date().toDateString(),
		maxMemoryLimitByte: 125 * 1000 * 1000,
		maxRealTimeLimitMilliseconds: 1000,
		allowDownloadInput: false,
		allowDownloadAnswer: false,
		testPoints: [] as TestPoint[],
	},
	removedTestPoints: [] as TestPoint[],
});

const props = defineProps({
	id: {
		type: String,
		required: false,
	},
});

const isEdit = computed(() => !!props.id);

const themeVars = useThemeVars();
const loadingBar = useLoadingBar();
const dialog = useDialog();
const customThemeVars = {
	removedTestPointColor: themeVars.value.errorColor + "30",
	newTestPointColor: themeVars.value.successColor + "10",
};

const formRules: FormRules = {
	title: {
		required: true,
		message: "请输入题目标题",
		trigger: "blur",
	},
	maxMemoryLimitByte: {
		type: "number",
		required: true,
		trigger: ["blur", "change"],
		validator: (_, value: number | undefined) => {
			if (!value) return new Error("请输入内存限制");
			if (value < 50 * 1000 * 1000) return new Error("内存限制不得少于 50MiB");
			if (value >= 1 * 1000 * 1000 * 1000)
				return new Error("内存不得高于 1GiB");
			return true;
		},
	},
	maxRealTimeLimitMilliseconds: {
		type: "number",
		required: true,
		trigger: ["blur", "change"],
		validator: (_, value: number | undefined) => {
			if (!value) return new Error("请输入时间限制");
			if (value < 0) return new Error("时间限制必须大于 0");
			if (value >= 10 * 1000) return new Error("时间不得高于 10s");
			return true;
		},
	},
};

const typeOptions = [
	{ label: "文本", value: "Text" },
	{ label: "文件 (建议测试数据大于 10 KB 使用)", value: "File" },
];

const detail = computed(() => {
	return {
		...state.value.detail,
		defaultResourceLimit: {
			maxMemoryLimitByte: state.value.detail.maxMemoryLimitByte,
			maxRealTimeLimitMilliseconds:
				state.value.detail.maxRealTimeLimitMilliseconds,
		},
	} as ProblemDetail;
});

//////////////////////////////////// function ////////////////////////////////////

function handleSubmitProblem() {
	state.value.apiLoading = true;
	loadingBar.start();

	if (isEdit.value) {
		// update problem
		updateProblemAsync(
			Number(props.id!),
			state.value.detail,
			state.value.removedTestPoints
		)
			.then((_) => {
				window.$message.success("更新问题成功! ");
				router.go(0);
			})
			.catch((err) => {
				console.error("Problem update failed", err);
				window.$message.error("更新问题失败! ");
			})
			.finally(() => {
				state.value.apiLoading = false;
				loadingBar.finish();
			});
	} else {
		// create problem
		createProblemAsync(state.value.detail)
			.then((_) => {
				router.push(`/problems`);
			})
			.catch((err) => {
				console.error("Problem create failed", err);
				window.$message.error("创建问题失败! ");
			})
			.finally(() => {
				state.value.apiLoading = false;
				loadingBar.finish();
			});
	}
}

function handleAddTestPoint() {
	state.value.detail.testPoints.push({
		id: 0,
		problemId: 0,
		remarks: "新测试点 #" + state.value.detail.testPoints.length,
		input: "",
		expectedOutput: "",
		resourceType: "Text",
	});
}

function handleRemoveTestPoint(
	e: MouseEvent,
	testPoint: TestPoint,
	undo: boolean = false
) {
	e.stopPropagation();
	e.preventDefault();
	if (undo) {
		const removedIndex = state.value.removedTestPoints.indexOf(testPoint);
		if (removedIndex >= 0) {
			state.value.removedTestPoints.splice(removedIndex, 1);
		}
		return;
	}

	const index = state.value.detail.testPoints.indexOf(testPoint);
	if (index >= 0 && testPoint.id === 0) {
		state.value.detail.testPoints.splice(index, 1);
	} else {
		state.value.removedTestPoints.push(testPoint);
	}
}

function handleUploadProblemResource(option: UploadCustomRequestOptions) {
	if (!option.file.file) return;
	dialog.warning({
		title: "警告",
		content: "上传文件将会覆盖原有数据，此操作不可逆！是否继续？",
		positiveText: "继续",
		onPositiveClick: async () => {
			await dohandleUploadProblemResource(option);
		},
		negativeText: "取消",
	});
}

async function dohandleUploadProblemResource(
	option: UploadCustomRequestOptions
) {
	const file = option.file.file as File;
	const data = option.data as { idx: string; type: "input" | "answer" };
	const testPoint = state.value.detail.testPoints[Number(data.idx)];
	const type = data.type;
	state.value.apiLoading = true;
	loadingBar.start();
	uploadProblemResourceAsync(Number(props.id!), file, type)
		.then((newFileName) => {
			if (type === "input") {
				testPoint.input = newFileName;
			} else {
				testPoint.expectedOutput = newFileName;
			}
		})
		.catch((err) => {
			console.error("Problem resource upload failed", err);
			window.$message.error("上传资源失败! ");
		})
		.finally(() => {
			state.value.apiLoading = false;
			loadingBar.finish();
		});
}

function getTimeStringFromFileName(fileName: string) {
	const timestamp = Number(fileName.split(".")[0]);
	return new Date(timestamp * 1000).toLocaleString();
}

onMounted(async () => {
	if (isEdit.value) {
		state.value.loading = true;
		getFullProblemAsync(Number(props.id!))
			.then((problem) => {
				state.value.detail = problem;
			})
			.catch((err) => {
				console.error("Problem fetch failed", err);
				window.$message.error("获取问题失败! ");
			})
			.finally(() => {
				state.value.loading = false;
			});
	}
});
</script>

<style scoped>
.problem-view-container {
	background-color: v-bind("themeVars.bodyColor");
	height: calc(100vh - 45px - 64px);
}

.problem-submit-banners {
	display: flex;
	justify-content: end;
	align-items: center;
	position: fixed;
	bottom: 0;
	left: 0;
	padding: 5px 10px;
	width: 100%;
	height: 64px;
	border-top: 1px solid v-bind("themeVars.borderColor");
	background-color: v-bind("themeVars.bodyColor");
}

.resize-trigger {
	border: 1px solid v-bind("themeVars.borderColor");
	height: 100%;
	background-color: v-bind("themeVars.tableColorHover");
	display: flex;
	justify-content: center;
	align-items: center;
}

.resize-trigger:hover {
	background-color: v-bind("themeVars.tableColor");
	transition: background-color 0.3s;
}

.removed-test-point {
	background-color: v-bind("customThemeVars.removedTestPointColor");
	border: 1px solid v-bind("themeVars.borderColor");
}

.new-test-point {
	background-color: v-bind("customThemeVars.newTestPointColor");
	border: 1px solid v-bind("themeVars.borderColor");
}
</style>
