<template>
  <center-content-container width="80%">
    <n-card>
      <n-layout embedded has-sider>
        <n-layout-sider content-style="padding: 5px">
          <n-avatar
              :size="128"
              round
              src="/static/users/default/default_avatar.png"
          />
        </n-layout-sider>
        <n-layout>
          <n-layout-header>
            <n-h2 class="mb-0">
              {{ userRef.userName }}
              <n-tag type="primary" class="m-2 float-end">
                Administrator
              </n-tag>
            </n-h2>
            <div>
              {{ userRef.email }}
              <n-divider vertical/>
              注册于
              {{ userRef.registerDate }}
              <n-divider vertical/>
              最近登录于
              {{ userRef.lastLoginDate }}
            </div>
          </n-layout-header>
          <n-layout-content class="mt-3 p-1">
            <n-row>
              <n-col :span="6">
                <n-statistic label="提交数">
                  <n-number-animation
                      :from="0"
                      :to="userRef.totalSubmissionCount"
                  />
                </n-statistic>
              </n-col>
              <n-col :span="6">
                <n-statistic label="通过数">
                  <n-number-animation
                      :from="0"
                      :to="userRef.acceptedSubmissionCount"
                  />
                </n-statistic>
              </n-col>
              <n-col :span="6">
                <n-statistic label="尝试题目数">
                  <n-number-animation
                      :from="0"
                      :to="userRef.totalProblemTriedCount"
                  />
                </n-statistic>
              </n-col>
              <n-col :span="6">
                <n-statistic label="通过题目数">
                  <n-number-animation
                      :from="0"
                      :to="userRef.acceptedProblemCount"
                  />
                </n-statistic>
              </n-col>
            </n-row>
          </n-layout-content>
        </n-layout>
      </n-layout>
    </n-card>

    <n-card class="mt-4">
      <n-tabs type="line" animated>
        <n-tab-pane name="submissions" tab="提交分析" display-directive="show">
          <submission-list
              show-submission-id
              :page-size="10"
              :filter="{
							submitterId: userRef.userId,
						}"
          />
        </n-tab-pane>
        <n-tab-pane
            v-if="isCurrentUser"
            name="account"
            tab="账号管理"
            display-directive="show:lazy"
        >
          <account-manage/>
        </n-tab-pane>

        <n-tab-pane
            v-if="isCurrentUser"
            name="problems-manage"
            tab="创建/管理题目"
        >
          <problems-manage v-if="userState.isDistributor"/>
          <n-alert
              v-else
              type="error"
              show-icon
              title="你不是题目管理员"
          >
            你没有在 HimuOJ 创建题目的权限，请联系管理员以获取权限。
        </n-alert>
        </n-tab-pane>
      </n-tabs>
    </n-card>
  </center-content-container>
</template>

<script setup lang="ts">
import CenterContentContainer from "@/components/shared/CenterContentContainer.vue";

import {
  NAvatar,
  NCard,
  NCol,
  NDivider,
  NH2,
  NLayout,
  NLayoutContent,
  NLayoutHeader,
  NLayoutSider,
  NNumberAnimation,
  NRow,
  NStatistic,
  NTabPane,
  NTabs,
  NTag,
} from "naive-ui";

import {onMounted, ref} from "vue";

import AccountManage from "@/components/users/AccountManage.vue";
import SubmissionList from "@/components/submissions/SubmissionList.vue";
import ProblemsManage from "@/components/users/ProblemsManage.vue";

import {getUserDetail} from "@/services/usersApi";
import {UserDetail} from "@/modules/user-types";
import {useUserState} from "@/stores/user";

const props = defineProps({
  id: {
    type: String,
    required: true,
  },
});

const userState = useUserState();

const userRef = ref<UserDetail>({
  userId: props.id,
  userName: "未知用户",
  email: "",
  registerDate: "9999-07-21",
  lastLoginDate: "9999-07-21",
  totalSubmissionCount: 0,
  acceptedSubmissionCount: 0,
  totalProblemTriedCount: 0,
  acceptedProblemCount: 0,
});

const isCurrentUser = ref(false);

onMounted(() => {
  getUserDetail(props.id)
      .then((res) => {
        userRef.value = res;
        isCurrentUser.value = userState.isLogin && res.userId === userState.id;
      })
      .catch((err) => {
        console.error(err);
      });
});
</script>
