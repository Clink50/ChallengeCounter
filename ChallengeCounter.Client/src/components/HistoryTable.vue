<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { getHistory } from '../api';

const history = ref<any[]>([]);

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  });
};

const fetchHistory = async () => {
  history.value = await getHistory();
};

onMounted(fetchHistory);
</script>

<template>
  <div class="rounded-xl bg-gray-800/90 p-6 shadow-lg">
    <h2 class="mb-4 text-lg font-bold text-blue-400">History</h2>
    <div class="overflow-x-auto">
      <table class="min-w-full text-sm">
        <thead>
          <tr class="bg-gray-900 text-blue-300">
            <th class="px-3 py-2 font-semibold">Date</th>
            <th class="px-3 py-2 font-semibold">Pushups</th>
            <th class="px-3 py-2 font-semibold">Squats</th>
            <th class="px-3 py-2 font-semibold">Abs</th>
            <th class="px-3 py-2 font-semibold">Sets</th>
          </tr>
        </thead>
        <tbody class="text-center">
          <tr v-for="row in history" :key="row.date" class="transition-colors hover:bg-gray-700">
            <td class="px-3 py-2">{{ formatDate(row.date) }}</td>
            <td class="px-3 py-2">{{ row.pushups }}</td>
            <td class="px-3 py-2">{{ row.squats }}</td>
            <td class="px-3 py-2">{{ row.abs }}</td>
            <td class="px-3 py-2">{{ row.sets }}</td>
          </tr>
        </tbody>
      </table>
      <div v-if="history.length === 0" class="py-4 text-center text-gray-400">
        No data for this month.
      </div>
    </div>
  </div>
</template>
