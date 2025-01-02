<script setup lang="ts">
import {
	ResourceUsage,
	SubmissionList,
	SubmissionListItem,
} from "@/modules/submits-type";
import { h, onMounted, ref, defineProps, onUnmounted } from "vue";
import { DataTableColumn, NButton, NDataTable, useThemeVars } from "naive-ui";
import { getSubmissionList } from "@/services/submissionsApi";
import AvatarWithText from "@/components/shared/AvatarWithText.vue";
import SubmissionStatusTag from "./SubmissionStatusTag.vue";
import SubmissionRemarks from "./SubmissionRemarks.vue";
import router from "@/routers";
import { createSubmitsHubConnection } from "@/services/hubConnection";

////////////////////////// props //////////////////////////

const props = defineProps({
	showUser: {
		type: Boolean,
		default: false,
	},
	showSubmissionId: {
		type: Boolean,
		default: true,
	},
	filter: {
		type: Object,
		default: () => ({
			problemId: undefined,
			submitterId: undefined,
		}),
	},
});

////////////////////////// variable //////////////////////////

const loadingRef = ref<boolean>(false);

const dataRef = ref<SubmissionList>({
	total: 0,
	items: [],
	pageCount: 0,
});

const paginationRef = ref({
	page: 1,
	pageCount: 1,
	pageSize: 10,
	itemCount: 0,
	prefix({ itemCount }: { itemCount: number | undefined }) {
		return `共 ${itemCount ?? 0}.`;
	},
});

const tableColumns = ref<DataTableColumn[]>([
	{
		key: "problemTitle",
		title: "题目",
		resizable: true,
		minWidth: "200px",
		render(row: any) {
			return h(
				NButton,
				{
					text: true,
					tag: "a",
					href: `/problems/${row.problemId}`,
				},
				{ default: () => row.problemTitle }
			);
		},
	},
	{
		key: "status",
		title: "状态",
		render(row: any) {
			return h(SubmissionStatusTag, {
				status: row.status,
			});
		},
	},
	{
		key: "usage",
		title: "详细信息",
		render(row: any) {
			return h(SubmissionRemarks, {
				usage: row.usage,
				compilerName: row.compilerName,
			});
		},
	},
	{
		key: "submitTime",
		title: "提交时间",
		render(row: any) {
			return h(
				"div",
				{
					class: "text-gray-500",
				},
				{
					default: () => new Date(row.submitTime).toLocaleString(),
				}
			);
		},
	},
]);

////////////////////////// function //////////////////////////

function handlePageChange(currentPage: number) {
	fetchSubmissionList(currentPage, paginationRef.value.pageSize);
}

function handleRowProps(rowData: SubmissionListItem, _: number) {
	return {
		style: {
			cursor: "pointer",
		},
		onclick: () => {
			if (rowData.status === "Running") {
				window.$message.info("由于 HimuOJ 正在运行您的提交，暂时无法查看详情");
				return;
			}
			router.push(`/submissions/${rowData.id}`);
		},
	};
}

let connection: signalR.HubConnection | null = null;
async function fetchSubmissionList(page: number, pageSize: number = 30) {
	loadingRef.value = true;
	getSubmissionList({
		page,
		pageSize,
		...props.filter,
	}).then((data) => {
		dataRef.value = data;
		paginationRef.value = {
			...paginationRef.value,
			page: page,
			pageCount: data.pageCount,
			pageSize: pageSize,
			itemCount: data.total,
		};
		// console.log("dataRef: ", dataRef.value);
		connection = createSubmitsHubConnection();
		connection.on("ReceiveSubmissionStatus", updateSubmissionStatusCallback);
		connection
			.start()
			.then(() => {
				console.log("Connection started");
			})
			.catch((err: any) => {
				console.error(err);
			});
		loadingRef.value = false;
	});
}

onUnmounted(() => {
	console.log("Unsubscribing from hub connection");
	if (connection) {
		connection.off("ReceiveSubmissionStatus", updateSubmissionStatusCallback);
		connection.stop();
	}
});

function updateSubmissionStatusCallback(
	submissionId: number,
	status: string,
	usage?: ResourceUsage
) {
	// console.log("Received: ", submissionId, status);
	for (let i = 0; i < dataRef.value.items.length; i++) {
		if (dataRef.value.items[i].id === submissionId) {
			dataRef.value.items[i].status = status;
			if (usage !== null) dataRef.value.items[i].usage = usage!;
			break;
		}
	}
	// dataRef.value.items = [...dataRef.value.items];
}

////////////////////////// lifecycle //////////////////////////

onMounted(async () => {
	if (props.showUser) {
		tableColumns.value.unshift({
			key: "submitterName",
			title: "用户",
			render(row: any) {
				return h(AvatarWithText, {
					src: row.submitterAvatar,
					message: row.submitterName,
					href: `users/${row.submitterId}`,
				});
			},
		});
	}

	if (props.showSubmissionId) {
		tableColumns.value.unshift({
			key: "id",
			title: "提交 ID",
		});
	}

	fetchSubmissionList(1);
});
</script>

<template>
	<n-data-table
		remote
		striped
		size="small"
		:data="dataRef.items"
		:pagination="paginationRef"
		:columns="tableColumns"
		:loading="loadingRef"
		:row-key="(row) => row.problemCode"
		:row-props="handleRowProps"
		:on-update:page="handlePageChange"
	></n-data-table>

	<!-- <pre>
		Javascript: {{ JSON.stringify(dataRef, null, 2) }}
	</pre
	> -->
</template>

<style scoped></style>
