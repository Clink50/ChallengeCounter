<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { getLeaderboard } from '../api';

const props = defineProps<{ year: number; month: number }>();
const leaderboard = ref<any[]>([]);
const loading = ref(false);

const fetchLeaderboard = async () => {
  loading.value = true;
  leaderboard.value = await getLeaderboard(props.year, props.month);
  loading.value = false;
};

onMounted(fetchLeaderboard);
watch(() => [props.year, props.month], fetchLeaderboard);
</script>

<template>
  <div class="mb-4 rounded-xl bg-gray-800/90 p-6 shadow-lg">
    <h2 class="mb-4 text-lg font-bold text-green-400">Leaderboard</h2>
    <div v-if="loading" class="py-4 text-center text-gray-400">Loading...</div>
    <div v-else class="overflow-x-auto">
      <table class="min-w-full text-sm">
        <thead>
          <tr class="bg-gray-900 text-green-300">
            <th class="px-3 py-2 font-semibold">User</th>
            <th class="px-3 py-2 font-semibold">Pushups</th>
            <th class="px-3 py-2 font-semibold">Squats</th>
            <th class="px-3 py-2 font-semibold">Abs</th>
            <th class="px-3 py-2 font-semibold">Sets</th>
            <th class="px-3 py-2 font-semibold">Total</th>
          </tr>
        </thead>
        <tbody class="text-center">
          <tr
            v-for="row in leaderboard"
            :key="row.userId"
            class="transition-colors hover:bg-gray-700"
          >
            <td class="px-3 py-2 font-semibold text-blue-200">{{ row.userId }}</td>
            <td class="px-3 py-2">{{ row.pushups }}</td>
            <td class="px-3 py-2">{{ row.squats }}</td>
            <td class="px-3 py-2">{{ row.abs }}</td>
            <td class="px-3 py-2">{{ row.sets }}</td>
            <td class="px-3 py-2 font-bold">{{ row.pushups + row.squats + row.abs }}</td>
          </tr>
        </tbody>
      </table>
      <div v-if="leaderboard.length === 0" class="py-4 text-center text-gray-400">
        No data for this month.
      </div>
    </div>
  </div>
</template>
