// If you see a linter error for 'pinia', run: npm install pinia
import { createPinia } from 'pinia';
import './assets/style.css';

import { createApp } from 'vue';
import App from './App.vue';

const app = createApp(App);
app.use(createPinia());
app.mount('#app');
