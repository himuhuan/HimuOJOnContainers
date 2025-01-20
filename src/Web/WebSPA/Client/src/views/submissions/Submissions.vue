<template>
  <center-content-container width="80%">
    <n-card>
      <n-h2>筛选</n-h2>
    </n-card>
    <n-card class="mt-4">
      <n-h2>提交列表</n-h2>
      <submission-list show-user show-submission-id/>
    </n-card>
  </center-content-container>
</template>

<script setup lang="ts">
import {NCard, NH2} from "naive-ui";
import SubmissionList from "@/components/submissions/SubmissionList.vue";
import {onMounted} from "vue";
import CenterContentContainer from "@/components/shared/CenterContentContainer.vue";

onMounted(() => {

});


</script>
