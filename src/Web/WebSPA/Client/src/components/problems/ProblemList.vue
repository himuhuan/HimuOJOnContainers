<script lang="ts" setup>
import {ProblemList} from "@/modules/problems-types.ts";
import {h, onMounted, ref} from "vue";
import {DataTableColumn, NButton, NDataTable, NProgress, useThemeVars,} from "naive-ui";
import {getProblemListAsync} from "@/services/problemsApi.ts";
import {changeColor} from "seemly";

////////////////////////// variable //////////////////////////

const themeVars = useThemeVars();

const loadingRef = ref<boolean>(false);

const dataRef = ref<ProblemList>({
  total: 0,
  items: [],
  pageCount: 0,
});

const paginationRef = ref({
  page: 1,
  pageCount: 1,
  pageSize: 10,
  itemCount: 0,
  prefix({itemCount}: any) {
    return `共 ${itemCount}.`;
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
            href: `/problems/${row.id}`
          },
          {default: () => row.title}
      );
    },
  },
  {
    key: "acceptedRate",
    title: "通过率",
    render(row: any) {
      return h(
          NProgress,
          {
            type: "line",
            percentage: row.totalSubmissionCount
                ? (row.acceptedSubmissionCount / row.totalSubmissionCount) * 100
                : 0,
            indicatorPlacement: "outside",
            color: themeVars.value.primaryColor,
            railColor: changeColor(themeVars.value.primaryColor, {alpha: 0.1}),
          },
          {
            default: () =>
                `${row.acceptedSubmissionCount}/${row.totalSubmissionCount}`,
          }
      );
    },
  },
]);

////////////////////////// function //////////////////////////

async function fetchProblemList(page: number, pageSize: number = 30) {
  loadingRef.value = true;
  getProblemListAsync({
    page,
    pageSize,
  }).then((data) => {
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
      remote
  ></n-data-table>
</template>

<style scoped></style>
