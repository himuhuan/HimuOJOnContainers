<template>
	<n-list :show-divider="false" clickable hoverable>

		<list-goto-item class="mb-2">
			<template #icon>
				<n-icon size="large">
					<password-icon />
				</n-icon>
			</template>
			修改密码
		</list-goto-item>

		<list-goto-item class="mb-2" @click="handleLogout">
			<template #icon>
				<n-icon size="large">
					<logout-icon />
				</n-icon>
			</template>
			注销
		</list-goto-item>
	</n-list>
</template>

<script lang="ts" setup>
import ListGotoItem from "../shared/ListGotoItem.vue";
import { NIcon, NList } from "naive-ui";
import {
	ArrowForwardDownPerson20Filled as LogoutIcon,
	Password20Regular as PasswordIcon,
} from "@vicons/fluent";
import { useUserState } from "@/stores/user";

const userState = useUserState();

function handleLogout() {
	window.location.href = userState.userLogoutUrl!;
}

</script>
