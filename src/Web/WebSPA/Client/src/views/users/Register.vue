<template>
	<center-content-container width="40%">
		<n-card title="注册账户" bordered>
			<n-form :rules="rules" ref="modelRef" :model="formState">
				<n-form-item-row label="邮箱" path="register.email">
					<n-input placeholder="邮箱" v-model:value="formState.register.email">
						<template #prefix>
							<n-icon>
								<mail-outline />
							</n-icon>
						</template>
					</n-input>
				</n-form-item-row>
				<n-form-item-row label="用户名" path="register.userName">
					<n-input
						placeholder="用户名"
						v-model:value="formState.register.userName"
					/>
				</n-form-item-row>
				<n-form-item-row label="密码" path="register.password">
					<n-input
						type="password"
						show-password-on="click"
						placeholder="密码"
						v-model:value="formState.register.password"
					/>
				</n-form-item-row>
				<n-form-item-row label="重复密码" path="register.repeatedPassword">
					<n-input
						type="password"
						placeholder="重复密码"
						show-password-on="click"
						v-model:value="formState.register.repeatedPassword"
					/>
				</n-form-item-row>
				<n-form-item-row label="联系电话" path="register.phone">
					<n-input
						placeholder="联系电话"
						v-model:value="formState.register.phone"
					/>
				</n-form-item-row>
				<n-alert type="info" title="注意">
					注册账户，即表明您已同意我们的<n-a href="#">隐私政策和服务条款。</n-a>
					<br />
					在点击注册后, Himu酱将会向您的邮箱发送一封包含验证令牌的邮件,
					请注意查收。
				</n-alert>
				<n-form-item-row>
					<n-button
						type="primary"
						block
						secondary
						strong
						:disabled="formState.loading"
						@click="registerButtonClick"
					>
						注册
					</n-button>
				</n-form-item-row>
			</n-form>
		</n-card>
	</center-content-container>
</template>

<script setup lang="ts">
import CenterContentContainer from "@/components/shared/CenterContentContainer.vue";
import { ref, reactive } from "vue";
import { MailOutline } from "@vicons/ionicons5";
import type { FormRules } from "naive-ui";
import { FormInst, useLoadingBar, useNotification } from "naive-ui";
import { UserRegisterRequest } from "@/modules/user-types";
import { registerUser } from "@/services/usersApi";

const modelRef = ref<FormInst | null>(null);
const notification = useNotification();
const loadingBar = useLoadingBar();

const formState = reactive({
	register: {
		email: "",
		userName: "",
		password: "",
		repeatedPassword: "",
		phone: "",
	} as UserRegisterRequest,
	loading: false,
});

const rules: FormRules = {
	"register.email": [
		{ required: true, message: "请输入邮箱", trigger: "blur" },
		{ type: "email", message: "请输入有效的邮箱地址", trigger: "blur" },
	],
	"register.userName": [
		{ required: true, message: "请输入用户名", trigger: "blur" },
		{
			min: 3,
			max: 20,
			message: "用户名长度应在3-20个字符之间",
			trigger: ["blur", "change"],
		},
	],
	"register.password": [
		{ required: true, message: "请输入密码", trigger: "blur" },
		{ min: 6, message: "密码长度至少6个字符", trigger: "blur" },
	],
	"register.repeatedPassword": [
		{ required: true, message: "请重复输入密码", trigger: "blur" },
		{
			validator: (_, value) => value === formState.register.password,
			message: "两次输入的密码不一致",
			trigger: ["blur", "change"],
		},
	],
	"register.phone": [
		{
			pattern: /^1[3-9]\d{9}$/,
			message: "请输入有效的手机号码",
			trigger: ["blur", "change"],
		},
	],
};

function registerButtonClick() {
	modelRef.value?.validate((errors) => {
		if (errors) {
			notification.error({
				title: "表单填写错误",
				content: errors[0][0].message,
			});
		} else {
			formState.loading = true;
			registerUser(formState.register)
				.then(() => {
					notification.success({
						title: "注册成功",
						content: "请查收邮件并验证您的邮箱",
						duration: 3000,
					});
				})
				.catch((e) => {
					notification.error({
						title: "注册失败",
						content: e.message,
						duration: 3000,
					});
				}).finally(() => {
					formState.loading = false;
				});
		}
	});
}
</script>
