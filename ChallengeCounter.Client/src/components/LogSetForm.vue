<script setup lang="ts">
import { ref } from 'vue';
import { logSet } from '../api';

const emit = defineEmits(['log']);

const pushups = ref(0);
const squats = ref(0);
const abs = ref(0);

const submit = async () => {
  await logSet({
    exerciseDate: new Date().toISOString().slice(0, 10),
    pushups: pushups.value,
    squats: squats.value,
    abs: abs.value,
  });
  pushups.value = squats.value = abs.value = 0;
  emit('log');
};
</script>

<template>
  <form @submit.prevent="submit" class="flex flex-col gap-3">
    <div class="flex gap-2">
      <div class="flex-1">
        <label class="mb-1 block text-xs text-gray-300">Pushups</label>
        <input
          v-model.number="pushups"
          type="number"
          min="0"
          placeholder="Pushups"
          class="w-full rounded border border-gray-700 bg-gray-900 px-3 py-2 text-gray-100 focus:ring-2 focus:ring-blue-500 focus:outline-none"
          required
        />
      </div>
      <div class="flex-1">
        <label class="mb-1 block text-xs text-gray-300">Squats</label>
        <input
          v-model.number="squats"
          type="number"
          min="0"
          placeholder="Squats"
          class="w-full rounded border border-gray-700 bg-gray-900 px-3 py-2 text-gray-100 focus:ring-2 focus:ring-blue-500 focus:outline-none"
          required
        />
      </div>
      <div class="flex-1">
        <label class="mb-1 block text-xs text-gray-300">Abs</label>
        <input
          v-model.number="abs"
          type="number"
          min="0"
          placeholder="Abs"
          class="w-full rounded border border-gray-700 bg-gray-900 px-3 py-2 text-gray-100 focus:ring-2 focus:ring-blue-500 focus:outline-none"
          required
        />
      </div>
    </div>
    <button
      type="submit"
      class="mt-2 w-full rounded bg-blue-600 py-2 font-semibold text-white transition hover:bg-blue-700"
    >
      Log Set
    </button>
  </form>
</template>
