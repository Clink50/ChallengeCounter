import { defineStore } from 'pinia';
import { ref } from 'vue';
import { getGoal, getHistory, getLeaderboard, getMonthly, getToday, setGoal } from './api';

export const useUserStore = defineStore('user', () => {
  const username = ref(localStorage.getItem('username') || '');

  function setUsername(name: string) {
    username.value = name;
    localStorage.setItem('username', name);
  }

  function clearUsername() {
    username.value = '';
    localStorage.removeItem('username');
  }

  return { username, setUsername, clearUsername };
});

export const useGoalStore = defineStore('goal', () => {
  const goal = ref({ pushupsGoal: 100, squatsGoal: 100, absGoal: 100 });
  const loadingGoal = ref(false);

  async function fetchGoal(year?: number, month?: number) {
    loadingGoal.value = true;
    const data = await getGoal(year, month);
    if (data) {
      goal.value = {
        pushupsGoal: data.pushupsGoal,
        squatsGoal: data.squatsGoal,
        absGoal: data.absGoal,
      };
    }
    loadingGoal.value = false;
  }

  async function saveGoal({
    year,
    month,
    pushupsGoal,
    squatsGoal,
    absGoal,
  }: {
    year: number;
    month: number;
    pushupsGoal: number;
    squatsGoal: number;
    absGoal: number;
  }) {
    await setGoal({ year, month, pushupsGoal, squatsGoal, absGoal });
    await fetchGoal(year, month);
  }

  return { goal, loadingGoal, fetchGoal, saveGoal };
});

export const useProgressStore = defineStore('progress', () => {
  const today = ref({ pushups: 0, squats: 0, abs: 0, sets: 0 });
  const monthly = ref({ pushups: 0, squats: 0, abs: 0, sets: 0, year: 0, month: 0 });
  const leaderboard = ref([]);
  const history = ref([]);

  const loadingToday = ref(false);
  const loadingMonthly = ref(false);
  const loadingLeaderboard = ref(false);
  const loadingHistory = ref(false);

  async function fetchToday() {
    loadingToday.value = true;
    today.value = await getToday();
    loadingToday.value = false;
  }

  async function fetchMonthly(year: number, month: number) {
    loadingMonthly.value = true;
    monthly.value = await getMonthly(year, month);
    loadingMonthly.value = false;
  }

  async function fetchLeaderboard(year?: number, month?: number) {
    loadingLeaderboard.value = true;
    leaderboard.value = await getLeaderboard(year, month);
    loadingLeaderboard.value = false;
  }

  async function fetchHistory() {
    loadingHistory.value = true;
    history.value = await getHistory();
    loadingHistory.value = false;
  }

  return {
    today,
    monthly,
    leaderboard,
    history,
    fetchToday,
    fetchMonthly,
    fetchLeaderboard,
    fetchHistory,
    loadingToday,
    loadingMonthly,
    loadingLeaderboard,
    loadingHistory,
  };
});
