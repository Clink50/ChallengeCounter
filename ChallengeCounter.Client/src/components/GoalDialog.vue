<script setup lang="ts">
import { ref, watch, defineProps, defineEmits } from 'vue';

const props = defineProps<{
  show: boolean;
  initialGoal: { pushupsGoal: number; squatsGoal: number; absGoal: number };
}>();
const emit = defineEmits(['close', 'save']);
const goal = ref({ ...props.initialGoal });

watch(
  () => props.show,
  (val) => {
    if (val) goal.value = { ...props.initialGoal };
  }
);

function submit() {
  emit('save', { ...goal.value });
  emit('close');
}
</script>

<template>
  <div
    v-if="show"
    class="bg-opacity-60 fixed inset-0 z-50 flex items-center justify-center bg-black"
  >
    <div class="w-full max-w-md rounded-xl bg-gray-900 p-6 shadow-xl">
      <h2 class="mb-4 text-lg font-bold text-yellow-400">Set Your Monthly Goal</h2>
      <p class="mb-4 text-gray-300">
        Set your monthly goal for each exercise. The default is 100 reps per day per exercise. You
        can change it now or later.
      </p>
      <form @submit.prevent="submit">
        <div class="mb-2">
          <label class="block text-xs text-gray-400">Pushups Goal</label>
          <input
            v-model.number="goal.pushupsGoal"
            type="number"
            min="0"
            class="w-full rounded border border-gray-700 bg-gray-800 px-2 py-1 text-gray-100"
          />
        </div>
        <div class="mb-2">
          <label class="block text-xs text-gray-400">Squats Goal</label>
          <input
            v-model.number="goal.squatsGoal"
            type="number"
            min="0"
            class="w-full rounded border border-gray-700 bg-gray-800 px-2 py-1 text-gray-100"
          />
        </div>
        <div class="mb-4">
          <label class="block text-xs text-gray-400">Abs Goal</label>
          <input
            v-model.number="goal.absGoal"
            type="number"
            min="0"
            class="w-full rounded border border-gray-700 bg-gray-800 px-2 py-1 text-gray-100"
          />
        </div>
        <button
          type="submit"
          class="w-full rounded bg-blue-600 px-3 py-2 text-white hover:bg-blue-700"
        >
          Save Goal
        </button>
      </form>
    </div>
  </div>
</template>
