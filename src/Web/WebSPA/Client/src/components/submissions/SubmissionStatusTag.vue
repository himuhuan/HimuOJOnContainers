<template>
    <n-tag
      size="large"
      type="error"
      v-if="props.status === 'WrongAnswer'"
    >
      结果错误
    </n-tag>
    <n-tag
      size="large"
      type="success"
      v-else-if="props.status === 'Accepted'"
    >
      已通过
    </n-tag>
    <n-tag
      size="large"
      type="warning"
      v-else-if="props.status === 'Running'"
    >
      正在运行
    </n-tag>
    <n-tag
      size="large"
      type="default"
      v-else-if="props.status === 'Pending'"
    >
      等待运行
    </n-tag>
    <n-tag
      size="large"
      type="error"
      v-else-if="props.status === 'CompilationError'"
    >
      编译错误
    </n-tag>
    <n-tag
      size="large"
      type="error"
      v-else-if="props.status === 'SystemError'"
    >
      系统错误
    </n-tag>
    <n-tag
      size="large"
      type="error"
      v-else-if="props.status === 'TimeLimitExceeded'"
    >
      时间超限
    </n-tag>
    <n-tag
      size="large"
      type="error"
      v-else-if="props.status === 'MemoryLimitExceeded'"
    >
      内存超限
    </n-tag>
    <n-tag size="large" v-else-if="props.status === 'PendingOrSkipped'">
      已跳过
    </n-tag>
    <n-tag size="large" type="error" v-else-if="props.status === 'RuntimeError'">
      运行错误
    </n-tag>
    <n-tag size="large" type="error" v-else>
      {{ props.status }}
    </n-tag>
  </template>
  
  <script setup lang="ts">
  import { NTag } from "naive-ui";
  import { defineProps } from "vue";
  
  const props = defineProps({
    status: {
      type: String,
      default: "default",
    },
  });
  </script>
  