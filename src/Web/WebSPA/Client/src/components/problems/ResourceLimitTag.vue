<template>
	<n-tag type="primary">
		<n-icon size="small">
			<memory-icon />
		</n-icon>
		{{ memoryLimit }}
	</n-tag>
	<n-divider vertical />

	<n-tag type="primary">
		<n-icon size="small">
			<timer-icon />
		</n-icon>
		{{ timeLimit }}
	</n-tag>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { NTag, NIcon, NDivider } from "naive-ui";
import {
	Timer28Regular as TimerIcon,
	Database20Regular as MemoryIcon,
} from "@vicons/fluent";

const props = defineProps({
	maxMemoryByte: {
		type: Number,
		required: true,
	},
	maxTimeMs: {
		type: Number,
		required: true,
	},
});

const memoryLimit = computed(() => {
	return `${(props.maxMemoryByte / 1000 / 1000).toFixed(0)} MB`;
});

const timeLimit = computed(() => {
	return `${props.maxTimeMs / 1000} s`;
});
</script>
