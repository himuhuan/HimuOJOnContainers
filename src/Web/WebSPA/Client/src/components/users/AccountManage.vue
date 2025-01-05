<template>
	<n-list :show-divider="false" hoverable clickable>
		<list-goto-item class="mb-2">
			<template #icon>
				<n-icon size="large"> <AvatarIcon /></n-icon>
			</template>
			修改头像
		</list-goto-item>

		<list-goto-item class="mb-2">
			<template #icon>
				<n-icon size="large"> <password-icon /></n-icon>
			</template>
			修改密码
		</list-goto-item>

		<list-goto-item class="mb-2">
			<template #icon>
				<n-icon size="large"> <account-settings-icon /></n-icon>
			</template>
			高级设置
		</list-goto-item>

		<list-goto-item class="mb-2">
			<template #icon>
				<n-icon size="large"> <logout-icon /></n-icon>
			</template>
			注销
		</list-goto-item>
	</n-list>
</template>

<script setup lang="ts">
import ListGotoItem from "../shared/ListGotoItem.vue";
import { NList, NIcon } from "naive-ui";
import {
	Person24Regular as AvatarIcon,
	PersonSettings20Regular as AccountSettingsIcon,
	ArrowForwardDownPerson20Filled as LogoutIcon,
    Password20Regular as PasswordIcon,
} from "@vicons/fluent";

</script>
