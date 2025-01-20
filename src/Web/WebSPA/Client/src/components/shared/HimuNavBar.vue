<template>
  <n-grid :cols="16" id="himu-navbar" item-responsive responsive="screen">
    <n-grid-item class="n-grid-item" span="8 m:2">
      <NSpace align="center">
        <n-avatar :src="src" size="large"/>
        <n-a class="himu-nav-title hvr-underline-from-center">
          <router-link to="/"> {{ props.title }}</router-link>
        </n-a>
      </NSpace>
    </n-grid-item>
    <n-grid-item class="n-grid-item" span="0 m:6">
      <n-menu
          :options="mainNavMenuOptions"
          mode="horizontal"
          style="justify-content: space-around"
      >
      </n-menu>
    </n-grid-item>
    <n-grid-item
        class="n-grid-item"
        style="justify-content: flex-end; margin-left: auto; margin-right: 20px"
        span="8"
    >
      <n-menu
          v-if="!userState.isLogin"
          :options="userInfoMenuOptions"
          mode="horizontal"
          style="justify-content: flex-end"
      />
      <n-dropdown
          v-else
          animated
          :options="userProfileDropdownOptions"
          @select="handleSelect"
      >
        <avatar-with-text
            :src="userState.userAvatar!"
            :message="userState.userName ?? '未知用户'"
            :href="`/users/${userState.id}`"
        />
      </n-dropdown>
    </n-grid-item>
  </n-grid>
</template>

<script lang="ts" setup>
import {
  DropdownOption,
  MenuOption,
  NA,
  NAvatar,
  NDropdown,
  NGrid,
  NGridItem,
  NIcon,
  NMenu,
  NSpace,
  useMessage,
  useThemeVars,
} from "naive-ui";
import HimuLogo from "@/assets/images/himu-logo.svg";
import AvatarWithText from "./AvatarWithText.vue";
import {useUserState} from "@/stores/user";
import {RouterLink} from "vue-router";
import {h} from "vue";

// icons
import {
  PlaylistAddCheckRound as ContestIcon,
  QueryStatsRound as SubmissionIcon,
  QuestionAnswerOutlined as DisscussionIcon,
} from "@vicons/material";

import {ColorWandOutline as ThemeIcon} from "@vicons/ionicons5";

import {BoxMultiple20Regular as ProblemIcon, PersonArrowRight20Regular as LogoutIcon,} from "@vicons/fluent";

const props = defineProps({
  title: {
    type: String,
    default: "Himu OJ",
  },
  src: {
    type: String,
    default: HimuLogo,
  },
  height: {
    type: Number,
    default: 48,
  },
  userId: {
    type: String,
    default: null,
  },
});

const userState = useUserState();
const themeVars = useThemeVars();
window.$message = useMessage();

const mainNavMenuOptions: MenuOption[] = [
  {
    label: () => h(RouterLink, {to: "/problems"}, {default: () => "题库"}),
    key: "problems",
    icon: () => h(NIcon, null, {default: () => h(ProblemIcon)}),
  },
  {
    label: () =>
        h(RouterLink, {to: "/submissions"}, {default: () => "提交"}),
    key: "submissions",
    icon: () => h(NIcon, null, {default: () => h(SubmissionIcon)}),
  },
  {
    label: () => h(RouterLink, {to: "/contests"}, {default: () => "比赛"}),
    key: "contests",
    icon: () => h(NIcon, null, {default: () => h(ContestIcon)}),
  },
  {
    label: () =>
        h(RouterLink, {to: "/discussions"}, {default: () => "讨论"}),
    key: "discussions",
    icon: () => h(NIcon, null, {default: () => h(DisscussionIcon)}),
  },
];

const userInfoMenuOptions: MenuOption[] = [
  {
    label: () => {
      if (!userState.isLogin) {
        return h(
            "a",
            {href: `/bff/login`},
            {
              default: () => "注册/登录",
            }
        );
      }
    },
    key: "home",
  },
  {
    label: () => {
      return h(
          "a",
          {href: `/users/register`},
          {
            default: () => "注册",
          }
      );
    }
  }
];

const userProfileDropdownOptions: DropdownOption[] = [
  {
    label: () => "切换主题",
    key: "theme-switch",
    icon: () => h(NIcon, null, {default: () => h(ThemeIcon)}),
  },
  {
    label: () => "登出",
    key: "logout",
    icon: () => h(NIcon, null, {default: () => h(LogoutIcon)}),
  },
];

const handleSelect = (key: string | number) => {
  if (key === "theme-switch") {
    userState.triggerThemeChange();
    window.$message.info("已切换主题");
  } else if (key === "logout") {
    window.location.href = userState.userLogoutUrl!;
  }
};
</script>

<style scoped>
#himu-navbar {
  z-index: 1000;
  position: sticky;
  backdrop-filter: blur(10px);
  top: 0;
  width: 100%;
  height: v-bind(height+ "px");
  background-color: v-bind("themeVars.bodyColor");
  border-bottom: 1px solid v-bind("themeVars.borderColor");
}

.n-grid-item {
  display: flex;
  align-items: center;
  padding: 0 16px;
  justify-content: space-around;
}

.himu-nav-title {
  text-decoration: none;
  color: v-bind("themeVars.textColor1");
  font-size: v-bind(height / 3 + "px");
  font-weight: bold;
  font-style: italic;
  font-family: "Victor Mono", "Consolas", "Microsoft Yahei", "PingFang SC",
  "Helvetica Neue", "Helvetica", "Arial", sans-serif;
}

.hvr-underline-from-center {
  display: inline-block;
  vertical-align: middle;
  -webkit-transform: perspective(1px) translateZ(0);
  transform: perspective(1px) translateZ(0);
  box-shadow: 0 0 1px rgba(0, 0, 0, 0);
  position: relative;
  overflow: hidden;
}

.hvr-underline-from-center:before {
  content: "";
  position: absolute;
  z-index: -1;
  left: 51%;
  right: 51%;
  bottom: 0;
  background: v-bind("themeVars.primaryColor");
  height: 3px;
  -webkit-transition-property: left, right;
  transition-property: left, right;
  -webkit-transition-duration: 0.3s;
  transition-duration: 0.3s;
  -webkit-transition-timing-function: ease-out;
  transition-timing-function: ease-out;
}

.hvr-underline-from-center:hover:before,
.hvr-underline-from-center:focus:before,
.hvr-underline-from-center:active:before {
  left: 0;
  right: 0;
}

a {
  text-decoration: none;
}

a:visited {
  color: v-bind("themeVars.textColor1");
}
</style>
