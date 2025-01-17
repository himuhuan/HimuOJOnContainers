<script lang="ts" setup>
import {
	ProblemManageList,
	ProblemManageListItem,
} from "@/modules/problems-types.ts";
import { h, nextTick, onMounted, ref } from "vue";
import {
	DataTableColumn,
	NButton,
	NDataTable,
	useThemeVars,
	DropdownOption,
	NDropdown,
	NIcon,
	useDialog,
	useNotification,
} from "naive-ui";
import { deleteProblemAsync, getProblemManageListAsync } from "@/services/problemsApi.ts";

import ResourceLimitTag from "./ResourceLimitTag.vue";
import GuestAccessLimitTag from "./GuestAccessLimitTag.vue";
import router from "@/routers";
import { BoxDismiss20Regular, BoxEdit20Regular } from "@vicons/fluent";

////////////////////////// variable //////////////////////////

const themeVars = useThemeVars();
const dialog = useDialog();
const notification = useNotification();

const props = defineProps({
	userId: {
		type: String,
		required: true,
	},
});

const loadingRef = ref<boolean>(false);

const dataRef = ref<ProblemManageList>({
	total: 0,
	items: [],
	pageCount: 0,
});

const paginationRef = ref({
	page: 1,
	pageCount: 1,
	pageSize: 10,
	itemCount: 0,
	prefix({ itemCount }: any) {
		return `共 ${itemCount} 项`;
	},
});

const tableColumns = ref<DataTableColumn[]>([
	{
		key: "id",
		title: "题目 ID",
	},
	{
		key: "title",
		title: "题目",
		resizable: true,
		minWidth: "200px",
		render(row: any) {
			return h(
				NButton,
				{
					text: true,
					tag: "a",
					href: `/problems/${row.id}`,
				},
				{ default: () => row.title }
			);
		},
	},
	{
		key: "createTime",
		title: "创建时间",
		render(row: any) {
			return new Date(row.createTime).toLocaleString();
		},
	},
	{
		key: "lastModifyTime",
		title: "更新时间",
		render(row: any) {
			return new Date(row.lastModifyTime).toLocaleString();
		},
	},
	{
		key: "defaultResourceLimit",
		title: "资源限制",
		render(row: any) {
			return h(ResourceLimitTag, {
				maxMemoryByte: row.defaultResourceLimit.maxMemoryLimitByte,
				maxTimeMs: row.defaultResourceLimit.maxRealTimeLimitMilliseconds,
			});
		},
	},
	{
		key: "guestAccessLimit",
		title: "访客限制",
		render(row: ProblemManageListItem) {
			return h(GuestAccessLimitTag, {
				allowDownloadInput: row.guestAccessLimit.allowDownloadInput,
				allowDownloadAnswer: row.guestAccessLimit.allowDownloadOutput,
			});
		},
	},
]);

const dropdownOptions: DropdownOption[] = [
	{
		label: "编辑",
		icon: () => h(NIcon, null, { default: () => h(BoxEdit20Regular) }),
		key: "edit",
	},
	{
		label: () =>
			h("span", { style: { color: themeVars.value.errorColor } }, "删除"),
		icon: () =>
			h(
				NIcon,
				{
					color: themeVars.value.errorColor,
				},
				{ default: () => h(BoxDismiss20Regular) }
			),
		key: "delete",
	},
];

const dropdownState = ref({
	visible: false,
	currentRow: null as ProblemManageListItem | null,
	x: 0,
	y: 0,
});

////////////////////////// function //////////////////////////

async function fetchProblemList(page: number, pageSize: number = 30) {
	loadingRef.value = true;
	getProblemManageListAsync({
		page,
		pageSize,
		distributorId: props.userId,
	}).then((data) => {
		console.log(data);
		dataRef.value = data;
		paginationRef.value = {
			...paginationRef.value,
			page: page,
			pageCount: data.pageCount,
			pageSize: pageSize,
			itemCount: data.total,
		};
		loadingRef.value = false;
	});
}

function handlePageChange(currentPage: number) {
	fetchProblemList(currentPage, paginationRef.value.pageSize);
}

function onClickoutside() {
	dropdownState.value.visible = false;
}

function handleSelect(option: any) {
	dropdownState.value.visible = false;
	if (option === "edit" && dropdownState.value.currentRow) {
		router.push(`/problems/${dropdownState.value.currentRow!.id}/edit`);
	} else if (option === "delete") {
		dialog.error({
			title: "删除题目",
			content: "确定删除该题目吗？ 删除后不可恢复",
			positiveText: "确定",
			negativeText: "取消",
			autoFocus: false,
			icon: () =>
				h(
					NIcon,
					{ color: themeVars.value.errorColor },
					{ default: () => h(BoxDismiss20Regular) }
				),
			onPositiveClick() {
				deleteProblemAsync(dropdownState.value.currentRow!.id).then(() => {
					notification.success({
						title: "删除成功",
						content: "题目已成功删除",
						duration: 3000,
					});
					fetchProblemList(1);
				}).catch((err) => {
					notification.error({
						title: "删除失败",
						content: err.message,
						duration: 3000,
					});
				});
			},
		});
	}
}

function rowProps(row: ProblemManageListItem) {
	return {
		onContextmenu(e: MouseEvent) {
			e.preventDefault();
			dropdownState.value.visible = false;
			nextTick().then(() => {
				dropdownState.value = {
					currentRow: row,
					visible: true,
					x: e.clientX,
					y: e.clientY,
				};
			});
		},
	};
}

////////////////////////// lifecycle //////////////////////////

onMounted(async () => {
	await fetchProblemList(1);
});
</script>

<template>
	<n-data-table
		:bordered="false"
		:columns="tableColumns"
		:data="dataRef.items"
		:loading="loadingRef"
		:on-update:page="handlePageChange"
		:pagination="paginationRef"
		:row-key="(row) => row.problemCode"
		:row-props="rowProps"
		remote
	></n-data-table>
	<n-dropdown
		placement="bottom-start"
		trigger="manual"
		:x="dropdownState.x"
		:y="dropdownState.y"
		:options="dropdownOptions"
		:show="dropdownState.visible"
		:on-clickoutside="onClickoutside"
		@select="handleSelect"
	/>
</template>

<style scoped></style>
