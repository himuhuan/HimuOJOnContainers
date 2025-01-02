<template>
	<n-card style="height: 100vh; padding: 20vh">
		<n-result
			status="403"
			title="您访问的页面似乎不存在"
			description="该结束这场闹剧了。"
			size="huge"
		>
			<template #footer>
				<n-button secondary type="primary" @click="goBack"> 返回主页 </n-button>
			</template>
		</n-result>
	</n-card>
</template>

<script setup lang="ts">
import { NResult, NButton, NCard } from "naive-ui";
import router from "@/routers";

const goBack = () => {
	router.push({ name: "home" });
};
</script>
