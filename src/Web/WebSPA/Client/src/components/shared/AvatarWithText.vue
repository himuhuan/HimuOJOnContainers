<template>
  <a class="flex items-center text-body" :href="props.href">
    <img :src="props.src" alt="avatar" class="w-8 h-8 rounded-full"/>
    <span class="ml-2 text-sm font-medium">{{
        props.message
      }}</span>
  </a>
</template>

<script setup lang="ts">
import {useThemeVars} from 'naive-ui';

const themeVars = useThemeVars();

const props = defineProps({
  src: {
    type: String,
    required: true,
  },
  href: {
    type: String,
    required: true,
  },
  message: {
    type: String,
    required: true,
  },
});
</script>

<style scoped>
.text-body {
  color: v-bind('themeVars.textColor1');
}
</style>