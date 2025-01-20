<script lang="ts" setup>
import {NIcon, NListItem, NSpace, useThemeVars} from "naive-ui";
import {ArrowForwardIosRound as ArrowRightIcon} from "@vicons/material";

const themeVars = useThemeVars();
</script>

<template>
  <n-list-item class="item-border">
    <n-space align="center" justify="space-between">
      <span class="info-list-item">
        <slot name="icon"></slot>
        &nbsp;
        <slot></slot>
      </span>
      <n-icon>
        <arrow-right-icon/>
      </n-icon>
    </n-space>
  </n-list-item>
</template>

<style scoped>
.info-list-item {
  padding: 5px 0;
  display: flex;
  align-items: center;
}

.item-border {
  border: 1px solid v-bind("themeVars.borderColor");
}
</style>
