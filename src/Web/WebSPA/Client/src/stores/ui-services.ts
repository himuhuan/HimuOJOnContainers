import { defineStore } from "pinia";
import { LoadingBarApi, ThemeCommonVars, CustomThemeCommonVars } from "naive-ui";
import { ref, type ComputedRef } from "vue";

/**
 * This store is used to provide UI services to the application.
 * It is used to provide global UI services such as notifications, modals, etc.
 */
export const useUiServices = defineStore("ui-services", () => {
	const loadingBar = ref<LoadingBarApi>();
	const theme = ref<ThemeCommonVars & CustomThemeCommonVars>();

	function setLoadingBarInstance(instance: LoadingBarApi) {
		loadingBar.value = instance;
	}

	function setThemeVars(overrides: ComputedRef<ThemeCommonVars & CustomThemeCommonVars>) {
		theme.value = overrides.value;
	}

	return {
		loadingBar,
		theme,
		setLoadingBarInstance,
		setThemeVars,
	};
});
