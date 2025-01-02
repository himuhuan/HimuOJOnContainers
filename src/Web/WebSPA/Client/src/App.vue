<template>
  <n-config-provider
    :theme="userState.perferTheme === 'dark' ? darkTheme : null"
    :theme-overrides="
      userState.perferTheme !== 'dark'
        ? lightThemeOverrides
        : darkThemeOverrides
    "
    :hljs="hljs"
  >
    <n-message-provider placement="top-right">
      <n-notification-provider>
        <n-dialog-provider>
          <n-scrollbar style="max-height: 100vh" trigger="hover">
            <himu-nav-bar></himu-nav-bar>
            <himu-background background-theme-color="#f85a6625" />
            <n-loading-bar-provider>
              <n-modal-provider>
                <ViewComponent />
              </n-modal-provider>
            </n-loading-bar-provider>
          </n-scrollbar>
        </n-dialog-provider>
      </n-notification-provider>
    </n-message-provider>
  </n-config-provider>
</template>

<script lang="ts" setup>
import HimuNavBar from "@/components/shared/HimuNavBar.vue";
import HimuBackground from "./components/shared/HimuBackground.vue";
import "@/style.css";
import { defineComponent, h } from "vue";
import { useLoadingBar } from "naive-ui"; 
import hljs from "highlight.js";
import cpp from "highlight.js/lib/languages/cpp";
import java from "highlight.js/lib/languages/java";
import python from "highlight.js/lib/languages/python";

hljs.registerLanguage("cpp", cpp);
hljs.registerLanguage("java", java);
hljs.registerLanguage("python", python);

import {
  NLoadingBarProvider,
  NMessageProvider,
  NNotificationProvider,
  NDialogProvider,
  NConfigProvider,
  NModalProvider,
  NScrollbar,
  GlobalThemeOverrides,
  darkTheme,
  useThemeVars,
} from "naive-ui";
import { RouterView } from "vue-router";
import router from "./routers";

import { useUiServices } from "@/stores/ui-services";
import { useUserState } from "./stores/user";

const userState = useUserState();

const lightThemeOverrides: GlobalThemeOverrides = {
  common: {
    bodyColor: "#ffffffdf",
    primaryColor: "#f85a66",
    primaryColorHover: "#dd0000",
    warningColor: "#cb95f8",
  },
  Card: {
    color: "#ffffffdf",
  },
  Dialog: {
    iconColorInfo: "#f85a66",
  },
  Button: {
    colorInfo: "#f85a66",
    colorPressedInfo: "#dd0000",
    colorHoverInfo: "#dd0000",
    colorFocusInfo: "#dd0000",
  },
};

const darkThemeOverrides: GlobalThemeOverrides = {
  common: {
    bodyColor: "#18181cdf",
    primaryColor: "#f85a66",
    primaryColorHover: "#dd0000",
  },
  Card: {
    color: "#18181cdf",
  },
  Dialog: {
    iconColorInfo: "#f85a66",
  },
  Button: {
    colorInfo: "#f85a66",
    colorPressedInfo: "#dd0000",
    colorHoverInfo: "#dd0000",
    colorFocusInfo: "#dd0000",
  },
};

const ViewComponent = defineComponent({
  render: () => h(RouterView, { key: router.currentRoute.value.fullPath }),
  setup: () => {
    const uiServices = useUiServices();
    uiServices.setLoadingBarInstance(useLoadingBar());
    uiServices.setThemeVars(useThemeVars());
    uiServices.setThemeVars(useThemeVars());
  },
});

</script>
