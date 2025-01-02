<template>
    <n-card style="height: 100vh; padding: 20vh">
      <n-result
        status="error"
        title="你没有权限访问该页面"
        description="由于权限限制，您无法访问该页面。"
        size="huge"
      >
        <template #footer>
          <n-button secondary type="primary" @click="goBack"> 返回主页 </n-button>
        </template>
      </n-result>
    </n-card>
  </template>
  
  <script setup lang="ts">
  import { NResult, NButton, NCard } from "naive-ui";
  import router from "@/routers";
  
  const goBack = () => {
    router.push({ name: "home" });
  };
  </script>