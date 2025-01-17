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
						<n-tab-pane name="testpoints" tab="测试点">
							<n-button
								style="width: 100%"
								type="primary"
								secondary
								@click="handleAddTestPoint"
								>添加测试点</n-button
							>
							<transition-group name="slide-fade">
								<n-collapse
									v-for="testPoint in state.detail.testPoints"
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
											name="ss"
										>
											<n-form
												:model="testPoint"
												label-placement="top"
											>
												<n-form-item label="备注" path="testPointRemarks">
													<n-input v-model:value="testPoint.remarks" />
												</n-form-item>
												<n-form-item label="输入" path="testPointInput">
													<n-input
														type="textarea"
														placeholder="测试点的输入"
														:autosize="{ minRows: 2, maxRows: 6 }"
														v-model:value="testPoint.input"
													/>
												</n-form-item>
												<n-form-item
													label="输出"
													path="testPointExpectedOutput"
												>
													<n-input
														type="textarea"
														placeholder="测试点的期望输出"
														:autosize="{ minRows: 2, maxRows: 6 }"
														v-model:value="testPoint.expectedOutput"
													/>
												</n-form-item>
											</n-form>
										</n-collapse-item>
									</n-card>
								</n-collapse>
							</transition-group>
						</n-tab-pane>
						<n-tab-pane name="submits" tab="提交记录" v-if="isEdit">
							<submission-list :filter="{ problemId: Number(props.id!) }" :show-problem-title="false"/>
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
	useLoadingBar,
	NCollapse,
	NCollapseItem,
	NCard,
} from "naive-ui";
import { computed, onMounted, ref } from "vue";
import { SwapHorizontal as SwapHorizontalIcon } from "@vicons/ionicons5";
import {
	Delete20Regular as DeleteIcon,
	ArrowForward20Regular as RestoreIcon,
} from "@vicons/fluent";
import { MdEditor } from "md-editor-v3";
import "md-editor-v3/lib/style.css";

import ProblemDetailPreview from "@/components/problems/ProblemDetailPreview.vue";
import SubmissionList from "@/components/submissions/SubmissionList.vue";
import {
	createProblemAsync,
	getFullProblemAsync,
	updateProblemAsync,
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
	}
};

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

function handleSubmitProblem() {
	state.value.apiLoading = true;
	loadingBar.start();
	if (isEdit.value) {
		// update problem
		updateProblemAsync(Number(props.id!), state.value.detail, state.value.removedTestPoints)
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
