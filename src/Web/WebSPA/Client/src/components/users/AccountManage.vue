<template>
  <n-list :show-divider="false" clickable hoverable>
    <list-goto-item class="mb-2">
      <template #icon>
        <n-icon size="large">
          <AvatarIcon/>
        </n-icon>
      </template>
      修改头像
    </list-goto-item>

    <list-goto-item class="mb-2">
      <template #icon>
        <n-icon size="large">
          <account-settings-icon/>
        </n-icon>
      </template>
      高级设置
    </list-goto-item>

    <list-goto-item class="mb-2">
      <template #icon>
        <n-icon size="large">
          <logout-icon/>
        </n-icon>
      </template>
      注销
    </list-goto-item>
  </n-list>
</template>

<script lang="ts" setup>
import ListGotoItem from "../shared/ListGotoItem.vue";
import {NIcon, NList} from "naive-ui";
import {
  ArrowForwardDownPerson20Filled as LogoutIcon,
  Password20Regular as PasswordIcon,
  Person24Regular as AvatarIcon,
  PersonSettings20Regular as AccountSettingsIcon,
} from "@vicons/fluent";
</script>
