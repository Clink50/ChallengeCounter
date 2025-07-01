<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { getMonthly, getGoal, setGoal } from '../api';
import GoalDialog from './GoalDialog.vue';

const now = new Date();
const currentYear = now.getFullYear();
const currentMonth = now.getMonth() + 1;
const currentDay = now.getDate();

const selectedYear = ref(currentYear);
const selectedMonth = ref(currentMonth);
const monthly = ref({
  pushups: 0,
  squats: 0,
  abs: 0,
  sets: 0,
  year: currentYear,
  month: currentMonth,
});
const daysInMonth = ref(new Date(currentYear, currentMonth, 0).getDate());

const goal = ref({ pushupsGoal: 100, squatsGoal: 100, absGoal: 100 }); // daily goals
const monthlyGoal = ref({ pushups: 0, squats: 0, abs: 0 });
const dailyGoal = ref({ pushups: 0, squats: 0, abs: 0 });
const showGoalDialog = ref(false);

const emit = defineEmits(['monthYearChanged']);

const fetchMonthly = async () => {
  const data = await getMonthly(selectedYear.value, selectedMonth.value);
  monthly.value = {
    ...data,
    year: data.year || selectedYear.value,
    month: data.month || selectedMonth.value,
  };
  daysInMonth.value = new Date(monthly.value.year, monthly.value.month, 0).getDate();
  emit('monthYearChanged', { year: monthly.value.year, month: monthly.value.month });
};

const fetchGoal = async () => {
  const data = await getGoal(selectedYear.value, selectedMonth.value);
  debugger;
  if (data) {
    goal.value = {
      pushupsGoal: data.pushupsGoal,
      squatsGoal: data.squatsGoal,
      absGoal: data.absGoal,
    };
  } else {
    goal.value = { pushupsGoal: 100, squatsGoal: 100, absGoal: 100 };
  }
  calcMonthlyGoal();
};

const calcMonthlyGoal = () => {
  monthlyGoal.value = {
    pushups: goal.value.pushupsGoal * daysInMonth.value,
    squats: goal.value.squatsGoal * daysInMonth.value,
    abs: goal.value.absGoal * daysInMonth.value,
  };
};

const saveGoal = async () => {
  await setGoal({
    year: selectedYear.value,
    month: selectedMonth.value,
    pushupsGoal: goal.value.pushupsGoal,
    squatsGoal: goal.value.squatsGoal,
    absGoal: goal.value.absGoal,
  });
  fetchGoal();
};

onMounted(() => {
  fetchMonthly();
  fetchGoal().then(() => {
    // If no daily goal set, show dialog
    if (!goal.value.pushupsGoal && !goal.value.squatsGoal && !goal.value.absGoal) {
      goal.value = { pushupsGoal: 100, squatsGoal: 100, absGoal: 100 };
      showGoalDialog.value = true;
    }
  });
});
watch([selectedYear, selectedMonth], () => {
  fetchMonthly();
  fetchGoal();
});

const years = Array.from({ length: 5 }, (_, i) => currentYear - 2 + i);
const months = Array.from({ length: 12 }, (_, i) => i + 1);

function handleGoalSave(newGoal: { pushupsGoal: number; squatsGoal: number; absGoal: number }) {
  goal.value = newGoal;
  saveGoal();
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
        <span>{{ monthly.pushups }} / {{ daysInMonth * 100 }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-blue-500 transition-all"
          :style="{ width: Math.min(100, (monthly.pushups / (daysInMonth * 100)) * 100) + '%' }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Squats</span>
        <span>{{ monthly.squats }} / {{ daysInMonth * 100 }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-green-500 transition-all"
          :style="{ width: Math.min(100, (monthly.squats / (daysInMonth * 100)) * 100) + '%' }"
        ></div>
      </div>
    </div>
    <div class="mb-2">
      <div class="mb-1 flex justify-between text-sm">
        <span>Abs</span>
        <span>{{ monthly.abs }} / {{ daysInMonth * 100 }}</span>
      </div>
      <div class="h-3 w-full rounded bg-gray-700">
        <div
          class="h-3 rounded bg-pink-500 transition-all"
          :style="{ width: Math.min(100, (monthly.abs / (daysInMonth * 100)) * 100) + '%' }"
        ></div>
      </div>
    </div>
    <div class="mt-4 text-sm text-gray-400">
      Sets logged: <span class="font-semibold text-white">{{ monthly.sets }}</span>
    </div>
  </div>
  <GoalDialog
    :show="showGoalDialog"
    :initialGoal="goal"
    @close="showGoalDialog = false"
    @save="handleGoalSave"
  />
</template>
