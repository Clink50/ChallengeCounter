<script setup lang="ts">
import { computed, ref } from 'vue';
import { useProgressStore, useUserStore } from './store';

import HistoryTable from './components/HistoryTable.vue';
import Leaderboard from './components/Leaderboard.vue';
import LogSetForm from './components/LogSetForm.vue';
import MonthlyProgress from './components/MonthlyProgress.vue';
import TodayProgress from './components/TodayProgress.vue';
import UserLogin from './components/UserLogin.vue';

import { getUserId } from './api';

const showLogModal = ref(false);
const selectedYear = ref(new Date().getFullYear());
const selectedMonth = ref(new Date().getMonth() + 1);

const userStore = useUserStore();
const progressStore = useProgressStore();
const hasUsername = computed(() => !!userStore.username);

function handleMonthYearChanged({ year, month }: { year: number; month: number }) {
  selectedYear.value = year;
  selectedMonth.value = month;
}

function handleLog() {
  showLogModal.value = false;
  progressStore.fetchToday();
  progressStore.fetchMonthly(selectedYear.value, selectedMonth.value);
  progressStore.fetchLeaderboard(selectedYear.value, selectedMonth.value);
  progressStore.fetchHistory();
}
</script>

<template>
  <UserLogin />
  <div v-if="hasUsername" class="flex min-h-screen flex-col bg-gray-950 text-gray-100">
    <header
      class="sticky top-0 z-20 flex w-full items-center justify-between bg-gray-900 px-4 py-6 shadow"
    >
      <h1 class="text-2xl font-bold tracking-tight">Challenge Counter</h1>
      <h2>Welcome, {{ getUserId() }}!</h2>
      <!-- Optionally add a dark mode toggle or user info here -->
    </header>
    <main class="mx-auto flex w-full max-w-2xl flex-1 flex-col gap-6 px-2 py-6">
      <TodayProgress />
      <Leaderboard :year="selectedYear" :month="selectedMonth" />
      <MonthlyProgress @monthYearChanged="handleMonthYearChanged" />
      <HistoryTable />
    </main>
    <!-- Floating Action Button -->
    <button
      class="fixed right-6 bottom-6 z-40 flex h-16 w-16 items-center justify-center rounded-full bg-blue-600 shadow-lg hover:bg-blue-700 focus:ring-2 focus:ring-blue-400 focus:outline-none"
      @click="showLogModal = true"
      aria-label="Log Set"
    >
      <svg
        class="h-8 w-8 text-white"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        viewBox="0 0 24 24"
      >
        <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
      </svg>
    </button>
    <!-- Log Modal -->
    <div
      v-if="showLogModal"
      class="bg-opacity-70 fixed inset-0 z-50 flex items-center justify-center bg-black"
    >
      <div
        class="relative w-full max-w-md rounded-xl border border-gray-700 bg-gray-900 p-6 shadow-xl"
      >
        <button
          class="absolute top-2 right-2 text-gray-400 hover:text-white"
          @click="showLogModal = false"
          aria-label="Close"
        >
          <svg
            class="h-6 w-6"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            viewBox="0 0 24 24"
          >
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
        <LogSetForm @log="handleLog" />
      </div>
    </div>
  </div>
</template>

<style scoped>
/**** Hide the log form on small screens if needed ****/
@media (max-width: 640px) {
  .fixed.bottom-0.left-0 {
    position: static !important;
    width: 100% !important;
    box-shadow: none !important;
    background: none !important;
  }
}
</style>
