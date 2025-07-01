<script setup lang="ts">
import { ref } from 'vue';
import { useUserStore } from '../store';

const input = ref('');
const userStore = useUserStore();

const save = () => {
  if (input.value.trim()) {
    userStore.setUsername(input.value.trim());
    window.location.reload();
  }
};
</script>

<template>
  <div
    v-if="!userStore.username"
    class="bg-opacity-70 fixed inset-0 z-50 flex items-center justify-center bg-black"
  >
    <div class="w-full max-w-xs rounded-xl border border-gray-700 bg-gray-900 p-6 shadow-xl">
      <h2 class="mb-2 font-bold text-gray-100">Enter your username</h2>
      <input
        v-model="input"
        @keydown.enter="save"
        class="input input-bordered mb-2 w-full border border-gray-700 bg-gray-800 text-gray-100 focus:ring-2 focus:ring-blue-500"
        placeholder="Username"
      />
      <button
        @click="save"
        @keydown.enter="save"
        class="btn w-full rounded bg-blue-600 py-2 font-semibold text-white transition hover:bg-blue-700"
      >
        Continue
      </button>
    </div>
  </div>
</template>
