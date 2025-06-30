<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { getToday } from '../api';

const today = ref({ pushups: 0, squats: 0, abs: 0, sets: 0 });

const fetchToday = async () => {
  today.value = await getToday();
};

onMounted(fetchToday);
</script>

<template>
  <div class="mb-4 rounded-xl bg-gray-800/90 p-6 shadow-lg">
    <h2 class="mb-4 text-lg font-bold text-blue-400">Today's Progress</h2>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Pushups</span>
        <span>{{ today.pushups }} / 100</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-blue-500 transition-all"
          :style="{ width: Math.min(100, (today.pushups / 100) * 100) + '%' }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Squats</span>
        <span>{{ today.squats }} / 100</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-green-500 transition-all"
          :style="{ width: Math.min(100, (today.squats / 100) * 100) + '%' }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Abs</span>
        <span>{{ today.abs }} / 100</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-pink-500 transition-all"
          :style="{ width: Math.min(100, (today.abs / 100) * 100) + '%' }"
        ></div>
      </div>
    </div>
    <div class="mt-4 text-sm text-gray-400">
      Sets logged: <span class="font-semibold text-white">{{ today.sets }}</span>
    </div>
  </div>
</template>
