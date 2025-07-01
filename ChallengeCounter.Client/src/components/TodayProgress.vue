<script setup lang="ts">
import { onMounted, watch } from 'vue';
import { useGoalStore, useProgressStore, useUserStore } from '../store';

const userStore = useUserStore();
const progressStore = useProgressStore();
const goalStore = useGoalStore();

onMounted(() => {
  progressStore.fetchToday();
  goalStore.fetchGoal();
});
watch(
  () => userStore.username,
  () => {
    progressStore.fetchToday();
    goalStore.fetchGoal();
  }
);
</script>

<template>
  <div class="mb-4 rounded-xl bg-gray-800/90 p-6 shadow-lg">
    <h2 class="mb-4 text-lg font-bold text-blue-400">Today's Progress</h2>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Pushups</span>
        <span>{{ progressStore.today.pushups }} / {{ goalStore.goal.pushupsGoal }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-blue-500 transition-all"
          :style="{
            width:
              Math.min(100, (progressStore.today.pushups / goalStore.goal.pushupsGoal) * 100) + '%',
          }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Squats</span>
        <span>{{ progressStore.today.squats }} / {{ goalStore.goal.squatsGoal }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-green-500 transition-all"
          :style="{
            width:
              Math.min(100, (progressStore.today.squats / goalStore.goal.squatsGoal) * 100) + '%',
          }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Abs</span>
        <span>{{ progressStore.today.abs }} / {{ goalStore.goal.absGoal }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-pink-500 transition-all"
          :style="{
            width: Math.min(100, (progressStore.today.abs / goalStore.goal.absGoal) * 100) + '%',
          }"
        ></div>
      </div>
    </div>
    <div class="mt-4 text-sm text-gray-400">
      Sets logged: <span class="font-semibold text-white">{{ progressStore.today.sets }}</span>
    </div>
  </div>
</template>
