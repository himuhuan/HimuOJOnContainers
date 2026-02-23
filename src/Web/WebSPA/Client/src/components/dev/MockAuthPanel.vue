<template>
	<n-card v-if="isMockMode" size="small" class="mock-auth-panel" embedded>
		<n-space vertical size="small">
			<n-text depth="3">Mock 身份</n-text>
			<n-space>
				<n-button
					v-for="option in roleOptions"
					:key="option.value"
					size="tiny"
					:type="selectedRole === option.value ? 'primary' : 'default'"
					secondary
					@click="handleChangeRole(option.value)"
				>
					{{ option.label }}
				</n-button>
			</n-space>
		</n-space>
	</n-card>
</template>

<script setup lang="ts">
import {onMounted, ref} from "vue";
import {NButton, NCard, NSpace, NText} from "naive-ui";
import {useUserState} from "@/stores/user";
import {getMockAuthRole, MockAuthRole, setMockAuthRole} from "@/mocks/data/constants";

const isMockMode = Boolean(import.meta.env.VITE_USE_MOCK);
const userState = useUserState();

const roleOptions: Array<{label: string; value: MockAuthRole}> = [
	{label: "游客", value: "guest"},
	{label: "用户", value: "user"},
	{label: "出题人", value: "distributor"},
	{label: "管理员", value: "admin"},
];

const selectedRole = ref<MockAuthRole>("guest");

function handleChangeRole(role: MockAuthRole) {
	setMockAuthRole(role);
	selectedRole.value = role;
	userState.fetchProfile();
}

onMounted(() => {
	if (!isMockMode) {
		return;
	}

	selectedRole.value = getMockAuthRole();
});
</script>

<style scoped>
.mock-auth-panel {
	position: fixed;
	z-index: 1500;
	right: 16px;
	bottom: 16px;
	max-width: 320px;
	box-shadow: 0 8px 20px rgb(0 0 0 / 0.08);
}
</style>
