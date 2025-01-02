<template>
	<div class="background"></div>
	<div
		v-if="
			userState.isLogin &&
			userState.perferTheme === 'dark'
		"
		class="background-dark"
	></div>
	<!--  <img v-else class="bgbox" :src="userState.backgroundUri" /> -->
</template>

<script setup lang="ts">
import { defineProps } from "vue";
import { useUserState } from "@/stores/user";
import { useThemeVars } from "naive-ui";

const props = defineProps({
	backgroundThemeColor: {
		type: String,
		default: "rgb(255, 255, 255)",
	},
});

const userState = useUserState();
const themeVars = useThemeVars();
</script>

<style scoped>
.background {
	z-index: -100;
	position: fixed;
	top: 0;
	left: 0;
	opacity: 1;
	width: 100%;
	height: 100%;
	object-fit: cover;
	backface-visibility: hidden;
	mix-blend-mode: luminosity;
	background: linear-gradient(
		360deg,
		v-bind("props.backgroundThemeColor") 20%,
		v-bind("themeVars.bodyColor") 100%
	);
}

.background-dark {
	z-index: -100;
	position: fixed;
	top: 0;
	left: 0;
	opacity: 0.95;
	width: 100%;
	height: 100%;
	object-fit: cover;
	backface-visibility: hidden;
	mix-blend-mode: luminosity;
	background-color: v-bind("themeVars.bodyColor");
}

.bgbox {
	z-index: -100;
	position: fixed;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	filter: blur(10px);
	transform: scale(1.2);
}
</style>
