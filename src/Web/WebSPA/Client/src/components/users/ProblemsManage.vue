<template>
	<n-list clickable hoverable>
		<router-link class="text-inherit mb-2" to="/problems/create">
			<list-goto-item>
				<template #icon>
					<n-icon size="large">
						<create-new-problem-icon />
					</n-icon>
				</template>
				创建题目
			</list-goto-item>
		</router-link>
	</n-list>

	<n-alert title="题目管理" type="info" class="mt-2 mb-2" closable>
		使用鼠标右键点击题目以对题目进行操作。
	</n-alert>

	<problem-manage-list :user-id="userState.id!" />
</template>

<script lang="ts" setup>
import ListGotoItem from "../shared/ListGotoItem.vue";
import ProblemManageList from "../problems/ProblemManageList.vue";

import { NIcon, NList, NAlert } from "naive-ui";
import { BoxEdit20Regular as CreateNewProblemIcon } from "@vicons/fluent";
import { useUserState } from "@/stores/user";

const userState = useUserState();
</script>
