<template>
	<div v-if="isAcceptedStatus(props.data!.status)">
		<n-text strong> 该测试点已通过测试。</n-text>
	</div>
	<div v-else-if="isPendingOrSkippedStatus(props.data!.status)">
		<n-text strong>
			由于之前的测试点没能通过测试，因此该测试点已被跳过。
		</n-text>
	</div>
	<div v-else-if="isWrongAnswerStatus(props.data!.status)">
		<n-space vertical>
			<n-text strong> 该测试点得到的结果与期望不符。</n-text>
			<n-descriptions
				:columns="1"
				bordered
				size="small"
				label-placement="left"
				:label-style="{ width: '100px' }"
			>
				<n-descriptions-item label="位于">
					{{ props.data.difference?.position }}
				</n-descriptions-item>
				<n-descriptions-item label="期望结果">
					{{ truncateStringWith(props.data.difference?.expectedOutput, 100) }}
				</n-descriptions-item>
				<n-descriptions-item label="实际结果">
					<div style="color: red">
						{{ truncateStringWith(props.data.difference?.actualOutput, 100) }}
					</div>
				</n-descriptions-item>
			</n-descriptions>
		</n-space>
	</div>
	<div v-else-if="isTimeLimitExceededStatus(props.data!.status)">
		<n-text strong> 该测试点运行超时。</n-text>
	</div>
    <div v-else-if="isMemoryLimitExceededStatus(props.data!.status)">
		<n-text strong> 该测试点内存超限。</n-text>
	</div>
	<div v-else>
		<n-text strong> 该测试点运行时发生错误。详情请查看输出。 </n-text>
	</div>
</template>

<script setup lang="ts">
import { TestPointResult } from "@/modules/submits-type";
import {
	isAcceptedStatus,
	isMemoryLimitExceededStatus,
	isPendingOrSkippedStatus,
	isTimeLimitExceededStatus,
	isWrongAnswerStatus,
} from "@/utils/HimuTools";
import { NText, NSpace, NDescriptions, NDescriptionsItem } from "naive-ui";
import { PropType, defineProps } from "vue";
import { truncateStringWith } from "@/utils/StringUtils";

const props = defineProps({
	data: {
		type: Object as PropType<TestPointResult>,
		required: true,
	},
});
</script>
