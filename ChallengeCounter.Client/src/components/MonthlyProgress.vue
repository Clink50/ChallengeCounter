<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useGoalStore, useProgressStore, useUserStore } from '../store';
import GoalDialog from './GoalDialog.vue';

const userStore = useUserStore();
const progressStore = useProgressStore();
const goalStore = useGoalStore();

const emit = defineEmits(['monthYearChanged']);

const now = new Date();
const currentYear = now.getFullYear();
const currentMonth = now.getMonth() + 1;
const currentDay = now.getDate();

const selectedYear = ref(currentYear);
const selectedMonth = ref(currentMonth);
const daysInMonth = ref(new Date(currentYear, currentMonth, 0).getDate());

const monthlyGoal = ref({ pushups: 0, squats: 0, abs: 0 });
const showGoalDialog = ref(false);

const calcMonthlyGoal = () => {
  monthlyGoal.value = {
    pushups: goalStore.goal.pushupsGoal * daysInMonth.value,
    squats: goalStore.goal.squatsGoal * daysInMonth.value,
    abs: goalStore.goal.absGoal * daysInMonth.value,
  };
};

onMounted(async () => {
  await progressStore.fetchMonthly(selectedYear.value, selectedMonth.value);
  await goalStore.fetchGoal(selectedYear.value, selectedMonth.value);
  calcMonthlyGoal();

  if (!goalStore.goal.pushupsGoal && !goalStore.goal.squatsGoal && !goalStore.goal.absGoal) {
    goalStore.saveGoal({
      year: selectedYear.value,
      month: selectedMonth.value,
      pushupsGoal: 100,
      squatsGoal: 100,
      absGoal: 100,
    });
    showGoalDialog.value = true;
  }
});

watch([selectedYear, selectedMonth, () => userStore.username], () => {
  progressStore.fetchMonthly(selectedYear.value, selectedMonth.value);
  goalStore.fetchGoal(selectedYear.value, selectedMonth.value).then(calcMonthlyGoal);
});

watch(() => goalStore.goal, calcMonthlyGoal, { deep: true });

const years = Array.from({ length: 5 }, (_, i) => currentYear - 2 + i);
const months = Array.from({ length: 12 }, (_, i) => i + 1);

async function handleGoalSave(newGoal: {
  pushupsGoal: number;
  squatsGoal: number;
  absGoal: number;
}) {
  await goalStore.saveGoal({
    year: selectedYear.value,
    month: selectedMonth.value,
    pushupsGoal: newGoal.pushupsGoal,
    squatsGoal: newGoal.squatsGoal,
    absGoal: newGoal.absGoal,
  });
  calcMonthlyGoal();
  showGoalDialog.value = false;
}
</script>

<template>
  <div class="mb-4 rounded-xl bg-gray-800/90 p-6 shadow-lg">
    <div class="mb-4 flex flex-wrap items-center gap-2">
      <h2 class="flex-1 text-lg font-bold text-yellow-400">Monthly Progress</h2>
      <select
        v-model="selectedYear"
        class="rounded border border-gray-700 bg-gray-900 px-2 py-1 text-gray-100"
      >
        <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
      </select>
      <select
        v-model="selectedMonth"
        class="rounded border border-gray-700 bg-gray-900 px-2 py-1 text-gray-100"
      >
        <option v-for="m in months" :key="m" :value="m">{{ m.toString().padStart(2, '0') }}</option>
      </select>
      <span class="text-xs text-gray-400">Day {{ currentDay }} / {{ daysInMonth }}</span>
    </div>
    <!-- <form class="mb-4 flex flex-wrap items-end gap-2" @submit.prevent="saveGoal">
      <div>
        <label class="block text-xs text-gray-400">Pushups Daily Goal</label>
        <input
          v-model.number="goal.pushupsGoal"
          type="number"
          min="0"
          class="w-20 rounded border border-gray-700 bg-gray-900 px-2 py-1 text-gray-100"
        />
      </div>
      <div>
        <label class="block text-xs text-gray-400">Squats Daily Goal</label>
        <input
          v-model.number="goal.squatsGoal"
          type="number"
          min="0"
          class="w-20 rounded border border-gray-700 bg-gray-900 px-2 py-1 text-gray-100"
        />
      </div>
      <div>
        <label class="block text-xs text-gray-400">Abs Daily Goal</label>
        <input
          v-model.number="goal.absGoal"
          type="number"
          min="0"
          class="w-20 rounded border border-gray-700 bg-gray-900 px-2 py-1 text-gray-100"
        />
      </div>
      <button type="submit" class="rounded bg-blue-600 px-3 py-1 text-white hover:bg-blue-700">
        Save Goal
      </button>
      <span
        v-if="goal.pushupsGoal || goal.squatsGoal || goal.absGoal"
        class="ml-4 text-xs text-gray-400"
      >
        Monthly: {{ monthlyGoal.pushups }} pushups, {{ monthlyGoal.squats }} squats,
        {{ monthlyGoal.abs }} abs
      </span>
    </form> -->
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Pushups</span>
        <span
          >{{ progressStore.monthly.pushups }} /
          {{ daysInMonth * goalStore.goal.pushupsGoal }}</span
        >
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-blue-500 transition-all"
          :style="{
            width:
              Math.min(
                100,
                (progressStore.monthly.pushups / (daysInMonth * goalStore.goal.pushupsGoal)) * 100
              ) + '%',
          }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Squats</span>
        <span
          >{{ progressStore.monthly.squats }} / {{ daysInMonth * goalStore.goal.squatsGoal }}</span
        >
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-green-500 transition-all"
          :style="{
            width:
              Math.min(
                100,
                (progressStore.monthly.squats / (daysInMonth * goalStore.goal.squatsGoal)) * 100
              ) + '%',
          }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Abs</span>
        <span>{{ progressStore.monthly.abs }} / {{ daysInMonth * goalStore.goal.absGoal }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-pink-500 transition-all"
          :style="{
            width:
              Math.min(
                100,
                (progressStore.monthly.abs / (daysInMonth * goalStore.goal.absGoal)) * 100
              ) + '%',
          }"
        ></div>
      </div>
    </div>
    <div class="mt-4 text-sm text-gray-400">
      Sets logged: <span class="font-semibold text-white">{{ progressStore.monthly.sets }}</span>
    </div>
  </div>
  <GoalDialog
    :show="showGoalDialog"
    :initialGoal="goalStore.goal"
    @close="showGoalDialog = false"
    @save="handleGoalSave"
  />
</template>
