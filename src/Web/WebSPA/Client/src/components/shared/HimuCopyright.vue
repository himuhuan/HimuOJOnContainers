<template>
  <div class="container" id="copyright">
    <n-grid x-gap="32" cols="4">
      <n-grid-item>
        <n-space vertical>
          <span class="copyright-header"> 资源 </span>
          <ul>
            <li><a href="#"> 本地客户端 </a></li>
            <li><a href="#"> 数据生成 </a></li>
            <li><a href="#"> API 参考 </a></li>
          </ul>
        </n-space>
      </n-grid-item>
      <n-grid-item>
        <n-space vertical>
          <span class="copyright-header"> 帮助 </span>
          <ul>
            <li><a href="#"> Bug反馈 </a></li>
            <li><a href="#"> 常见问题 </a></li>
            <li><a href="#"> 更新日志 </a></li>
          </ul>
        </n-space>
      </n-grid-item>
      <n-grid-item>
        <n-space vertical>
          <span class="copyright-header"> 联系我们 </span>
          <ul>
            <li><a href="#"> QQ </a></li>
            <li><a href="#"> 电子邮箱 </a></li>
            <li><a href="#"> Github </a></li>
          </ul>
        </n-space>
      </n-grid-item>
      <n-grid-item>
        <n-space vertical>
          <span class="copyright-header"> 开源社区 </span>
          <p>本项目的诞生离不开以下开源项目与社区的帮助：</p>
          <n-space>
            <img :src="DotNetBrand" alt="DotNetBrand" width="32" height="32" />
            <img :src="NaiveLogo" alt="NaiveLogo" width="32" height="32" />
            <n-icon size="32" :color="themeVars.primaryColor">
              <logo-docker />
            </n-icon>
            <n-icon size="32" :color="themeVars.primaryColor">
              <logo-vue />
            </n-icon>
          </n-space>
        </n-space>
      </n-grid-item>
    </n-grid>
    <n-hr />
    <n-space vertical>
      <n-text> Copyright &copy; 2023-2024 Himu. </n-text>
      <n-text>
        本项目是开源项目,
        请参看源代码的版权声明。本软件没有任何担保；包括没有适销性和某一专用目的下的适用性担保
      </n-text>
    </n-space>
  </div>
</template>

<script lang="ts" setup>
import {
  NSpace,
  NGrid,
  NGridItem,
  NIcon,
  NHr,
  NText,
  useThemeVars,
} from "naive-ui";
import { defineProps } from "vue";
import DotNetBrand from "@/assets/images/brand-dotnet.png";
import NaiveLogo from "@/assets/images/naive-logo.svg";
import { LogoDocker, LogoVue } from "@vicons/ionicons5";

const props = defineProps({
  backgroundColor: {
    type: String,
    default: null,
  },
});

const themeVars = useThemeVars();
</script>

<style scoped>
#copyright {
  padding: 1rem 3rem;
  background-color: v-bind("themeVars.bodyColor");
  backdrop-filter: blur(10px);
	top: 0;
	width: 100%;
}

#copyright a {
  text-decoration: none;
  color: v-bind("themeVars.textColor2");
  transition: 0.3s ease;
}

#copyright a:hover {
  text-decoration: underline;
  color: v-bind("themeVars.primaryColor");
}

#copyright ul {
  list-style: none;
  padding: 2px 0;
  margin: 3px 0;
}

#copyright li {
  padding: 2px 0;
}

.copyright-header {
  color: v-bind("themeVars.textColor1");
  font-size: 1rem;
  font-weight: 800;
  border-bottom: 3px v-bind("themeVars.primaryColor") solid;
}
</style>
