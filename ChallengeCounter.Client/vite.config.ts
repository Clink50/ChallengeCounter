import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import tailwindcss from '@tailwindcss/vite';

const port = process.env.PORT ? parseInt(process.env.PORT) : undefined;

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), tailwindcss()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    host: true,
    port,
    proxy: {
      '/api': {
        target: process.env.services__api__https__0 || process.env.services__api__http__0,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
