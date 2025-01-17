<template>
	<n-tag :type="props.allowDownloadInput ? 'success' : 'error'">
		<n-icon>
			<yes-icon v-if="props.allowDownloadInput" />
			<no-icon v-else />
		</n-icon>
		输入
	</n-tag>
	<n-divider vertical />
	<n-tag :type="props.allowDownloadAnswer ? 'success' : 'error'">
		<n-icon >
			<yes-icon v-if="props.allowDownloadAnswer" />
			<no-icon v-else />
		</n-icon>
		答案
	</n-tag>
</template>

<script lang="ts" setup>
const props = defineProps({
	allowDownloadInput: {
		type: Boolean,
		required: true,
	},
	allowDownloadAnswer: {
		type: Boolean,
		required: true,
	},
});

import { NIcon, NTag, NDivider } from "naive-ui";

import {
	CheckmarkCircle20Filled as YesIcon,
	DismissCircle12Filled as NoIcon,
} from "@vicons/fluent";
</script>
