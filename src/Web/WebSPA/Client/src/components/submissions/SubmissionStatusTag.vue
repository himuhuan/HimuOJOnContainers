<template>
  <n-tag
      v-if="props.status === 'WrongAnswer'"
      size="large"
      type="error"
  >
    结果错误
  </n-tag>
  <n-tag
      v-else-if="props.status === 'Accepted'"
      size="large"
      type="success"
  >
    已通过
  </n-tag>
  <n-tag
      v-else-if="props.status === 'Running'"
      size="large"
      type="warning"
  >
    正在运行
  </n-tag>
  <n-tag
      v-else-if="props.status === 'Pending'"
      size="large"
      type="default"
  >
    等待运行
  </n-tag>
  <n-tag
      v-else-if="props.status === 'CompilationError'"
      size="large"
      type="error"
  >
    编译错误
  </n-tag>
  <n-tag
      v-else-if="props.status === 'SystemError'"
      size="large"
      type="error"
  >
    系统错误
  </n-tag>
  <n-tag
      v-else-if="props.status === 'TimeLimitExceeded'"
      size="large"
      type="error"
  >
    时间超限
  </n-tag>
  <n-tag
      v-else-if="props.status === 'MemoryLimitExceeded'"
      size="large"
      type="error"
  >
    内存超限
  </n-tag>
  <n-tag v-else-if="props.status === 'PendingOrSkipped'" size="large">
    已跳过
  </n-tag>
  <n-tag v-else-if="props.status === 'RuntimeError'" size="large" type="error">
    运行错误
  </n-tag>
  <n-tag v-else size="large" type="error">
    {{ props.status }}
  </n-tag>
</template>

<script lang="ts" setup>
import {NTag} from "naive-ui";
import {defineProps} from "vue";

const props = defineProps({
  status: {
    type: String,
    default: "default",
  },
});
</script>
  