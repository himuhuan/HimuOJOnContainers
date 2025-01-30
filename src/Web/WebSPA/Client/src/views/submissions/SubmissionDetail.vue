<template>
	<transition name="fade">
		<div v-if="stateRef.loading" class="spining-container">
			<n-spin></n-spin>
			<n-text> Loading...</n-text>
		</div>
	</transition>
	<n-alert v-if="stateRef.hasError" closable title="加载失败" type="error">
		在加载提交详情时发生错误，请稍后再试。<br />
		<n-hr />
		<pre class="p-3 text-wrap">{{ stateRef.errorMessage }}</pre>
	</n-alert>
	<center-content-container
		v-if="!stateRef.hasError && !stateRef.loading"
		width="90%"
	>
		<n-alert
			v-if="stateRef.data!.statusMessage === 'Illegal operation'"
			title="非法操作 - 安全警告!"
			type="error"
			class="mb-2"
		>
			HimuOJ 检测到该提交的代码可能试图执行非法代码，已终止评测。
			<br />
			请不要尝试进行非法操作。过多的非法操作将导致您的账号被封禁。
		</n-alert>
		<n-card :title="`提交 #${stateRef.data!.id}`" size="large">
			<template #header-extra>
				<submission-status-tag :status="stateRef.data!.status" />
			</template>
			<n-descriptions :column="1" bordered label-placement="left">
				<n-descriptions-item label="题目">
					<router-link :to="`/problems/${stateRef.data!.problemId}`"
						>{{ stateRef.data!.problemTitle }}
					</router-link>
				</n-descriptions-item>
				<n-descriptions-item label="提交者">
					<AvatarWithText
						:href="`/users/${stateRef.data!.submitterId}`"
						:message="stateRef.data!.submitterName || '未知用户'"
						:src="stateRef.data!.submitterAvatar || ''"
					/>
				</n-descriptions-item>
				<n-descriptions-item label="提交时间">
					{{ new Date(stateRef.data!.submitTime).toLocaleString() }}
				</n-descriptions-item>
				<n-descriptions-item label="状态">
					<submission-status-tag :status="stateRef.data!.status" />
				</n-descriptions-item>
				<n-descriptions-item label="运行信息">
					<submission-remarks
						:compiler-name="stateRef.data!.compilerName"
						:usage="stateRef.data!.usage"
					/>
				</n-descriptions-item>
			</n-descriptions>
		</n-card>

		<n-hr />
		<n-card>
			<n-collapse :default-expanded-names="['source-code', 'run-result']">
				<n-collapse-item name="source-code" title="源代码">
					<n-card embedded>
						<n-code
							:code="stateRef.data!.sourceCode"
							:language="mapCompilerNameToLanguage(stateRef.data!.compilerName)"
							class="pre-wrapper"
							show-line-numbers
						/>
					</n-card>
				</n-collapse-item>
				<n-collapse-item
					v-if="!isAcceptedStatus(stateRef.data!.status)"
					name="output"
					title="输出"
				>
					<n-card embedded>
						<n-code
							:code="stateRef.data!.statusMessage"
							word-wrap
							language="plaintext"
						/>
					</n-card>
				</n-collapse-item>

				<n-collapse-item name="run-result" title="运行结果">
					<n-alert
						v-if="
							!stateRef.data?.problemAllowDownloadAnswer &&
							!stateRef.data?.problemAllowDownloadAnswer
						"
						title="注意：无法下载测试数据"
						type="info"
					>
						HimuOJ 官方测试不会限制用户自由下载测试的相关数据,
						由于创建者的策略设置，你无法下载这些测试数据。
					</n-alert>

					<n-collapse
						v-for="result in stateRef.data?.testPointResults"
						:key="result.testPointId"
					>
						<n-card
							:class="{
								'other-result': isPendingOrSkippedStatus(result.status),
								'success-result': isAcceptedStatus(result.status),
								'error-result': isRejectedStatus(result.status),
							}"
							size="small"
							style="margin-top: 5px"
						>
							<n-collapse-item :title="'测试 #' + result.id">
								<template #header-extra>
									<n-space justify="space-between">
										<n-tag size="large" type="info">
											{{ Math.ceil(result.usage?.usedMemoryByte! / 1000) }} KB
										</n-tag>
										<n-tag size="large" type="info">
											{{ result.usage?.usedTimeMs || "N/A" }} ms
										</n-tag>
										<submission-status-tag :status="result.status" />
									</n-space>
								</template>
								<test-point-result :data="result" />
								<n-space
									class="mt-2"
									justify="end"
									v-if="
										stateRef.data?.problemAllowDownloadAnswer ||
										stateRef.data?.problemAllowDownloadAnswer
									"
								>
									<n-button
										v-if="stateRef.data?.problemAllowDownloadInput"
										@click="
											downloadProblemTestPointResource(
												stateRef.data?.problemId!,
												result.testPointId,
												'input'
											)
										"
										tertiary
									>
										下载输入数据
									</n-button>
									<n-button
										v-if="stateRef.data?.problemAllowDownloadAnswer"
										@click="
											downloadProblemTestPointResource(
												stateRef.data?.problemId!,
												result.testPointId,
												'answer'
											)
										"
										tertiary
									>
										下载答案数据
									</n-button>
								</n-space>
							</n-collapse-item>
						</n-card>
					</n-collapse>
				</n-collapse-item>
			</n-collapse>
		</n-card>
	</center-content-container>
</template>

<script lang="ts" setup>
import { SubmissionDetail } from "@/modules/submits-type";
import { getSubmissionDetail } from "@/services/submissionsApi";

import {
	NAlert,
	NCard,
	NCode,
	NCollapse,
	NCollapseItem,
	NDescriptions,
	NDescriptionsItem,
	NHr,
	NSpace,
	NSpin,
	NTag,
	NText,
	NButton,
	useThemeVars,
} from "naive-ui";

import SubmissionStatusTag from "@/components/submissions/SubmissionStatusTag.vue";
import SubmissionRemarks from "@/components/submissions/SubmissionRemarks.vue";
import CenterContentContainer from "@/components/shared/CenterContentContainer.vue";
import TestPointResult from "@/components/submissions/TestPointResult.vue";

import { onMounted, ref } from "vue";
import AvatarWithText from "@/components/shared/AvatarWithText.vue";
import {
	isAcceptedStatus,
	isPendingOrSkippedStatus,
	isRejectedStatus,
	mapCompilerNameToLanguage,
} from "@/utils/HimuTools";
import { downloadProblemTestPointResource } from "@/services/problemsApi";

const props = defineProps({
	id: {
		type: String,
		required: true,
	},
});

const stateRef = ref({
	loading: true,
	hasError: false,
	errorMessage: "",
	data: null as SubmissionDetail | null,
});

const themeVars = useThemeVars();
const customThemeVars = {
	errorCardColor: themeVars.value.errorColor + "30",
	successCardColor: themeVars.value.successColor + "10",
	otherCardColor: themeVars.value.warningColor + "10",
};

onMounted(() => {
	getSubmissionDetail(Number(props.id))
		.then((data) => {
			console.log(data);
			stateRef.value.data = data;
		})
		.catch((e) => {
			stateRef.value.hasError = true;
			console.error(e);
			stateRef.value.errorMessage = e.data;
		})
		.finally(() => {
			stateRef.value.loading = false;
		});
});
</script>

<style scoped>
.error-result {
	background-color: v-bind("customThemeVars.errorCardColor");
	border: 1px solid v-bind("themeVars.borderColor");
}

.success-result {
	background-color: v-bind("customThemeVars.successCardColor");
	border: 1px solid v-bind("themeVars.borderColor");
}

.other-result {
	border: 1px solid v-bind("themeVars.borderColor");
}
</style>
