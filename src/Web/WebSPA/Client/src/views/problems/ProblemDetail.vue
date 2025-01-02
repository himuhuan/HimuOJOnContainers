<template>
	<transition name="fade">
		<div class="spining-container" v-if="state.loading">
			<n-spin></n-spin>
			<n-text> Loading...</n-text>
		</div>
	</transition>
	<div v-if="!state.loading" class="problem-view-container">
		<n-split
			:default-size="0.4"
			:resize-trigger-size="12"
			direction="horizontal"
			class="problem-view-container"
		>
			<template #1>
				<n-scrollbar style="height: calc(100vh - 45px)">
					<problem-detail-preview :detail="state.detail!" />
				</n-scrollbar>
			</template>
			<template #2>
				<n-scrollbar style="height: calc(100vh - 45px)">
					<problem-commit-tabs :problem-id="state.problemId" />
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
</template>

<script setup lang="ts">
import { ProblemDetail } from "@/modules/problems-types";
import router from "@/routers";
import { getProblemDetailAsync } from "@/services/problemsApi";
import { useThemeVars } from "naive-ui";
import { onMounted, reactive } from "vue";
import { NSpin, NText, NSplit, NScrollbar, NIcon } from "naive-ui";
import { SwapHorizontal as SwapHorizontalIcon } from "@vicons/ionicons5";
import ProblemCommitTabs from "@/components/problems/ProblemCommitTabs.vue";
import ProblemDetailPreview from "@/components/problems/ProblemDetailPreview.vue";

const props = defineProps({
	id: {
		type: String,
		required: true,
	},
});

const state = reactive({
	loading: true,
	detail: null as ProblemDetail | null,
	problemId: "",
});

const themeVars = useThemeVars();

onMounted(() => {
	state.problemId = props.id;
	getProblemDetailAsync(Number(state.problemId))
		.then((res) => {
			state.detail = res;
			state.loading = false;
			document.title = `题目详情 - ${res.title}`;
		})
		.catch((err) => {
			console.error(err);
			window.$message.error("获取题目详情失败");
			router.push({ name: "not-found" });
		})
		.finally(() => {
			state.loading = false;
		});
});
</script>

<style scoped>
.problem-view-container {
	background-color: v-bind("themeVars.bodyColor");
	height: calc(100vh - 45px);
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
</style>
