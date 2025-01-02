<template>
	<center-content-container width="80%">
		<!-- <n-card>
			<n-h2>筛选</n-h2>
		</n-card> -->
		<n-card class="mt-4">
			<n-h2>问题列表</n-h2>
			<problem-list />
		</n-card>
	</center-content-container>
</template>

<script setup lang="ts">
import { NCard, NH2 } from "naive-ui";
import ProblemList from "@/components/problems/ProblemList.vue";
import { onMounted } from "vue";
import CenterContentContainer from "@/components/shared/CenterContentContainer.vue";

onMounted(() => {
	
});


</script>
