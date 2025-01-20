<template>
  <div class="text-gray-500 flex items-center space-x-2">
    <n-icon :size="16">
      <memory-icon/>
    </n-icon>
    {{ toMB(props.usage?.usedMemoryByte) }} MB /
    <n-icon :size="16">
      <time-icon/>
    </n-icon>
    {{ props.usage?.usedTimeMs ?? "NaN" }} ms /
    <n-icon :size="16">
      <compiler-icon/>
    </n-icon>
    {{ props.compilerName }}
  </div>
</template>

<script lang="ts" setup>
import {ResourceUsage} from "@/modules/submits-type";
import {PropType} from "vue";
import {NIcon} from "naive-ui";
import {Code20Regular as CompilerIcon, Flash20Regular as MemoryIcon, Timer20Regular as TimeIcon,} from "@vicons/fluent";

const props = defineProps({
  usage: {
    type: Object as PropType<ResourceUsage | null>,
    required: false,
    default: null,
  },
  compilerName: {
    type: String,
    required: true,
  },
});

function toMB(bytes: number | undefined): string {
  if (bytes === undefined) return "NaN";
  return (bytes / 1024 / 1024).toFixed(0).toString();
}
</script>
