const API_BASE = '/api';

export interface WorkoutLogDto {
  userId?: string;
  exerciseDate: string; // ISO date string
  pushups: number;
  squats: number;
  abs: number;
}

function getUserId() {
  return localStorage.getItem('username') || '';
}

export async function logSet(data: Omit<WorkoutLogDto, 'userId'>) {
  const userId = getUserId();
  const response = await fetch(`${API_BASE}/log`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ ...data, userId }),
  });

  if (!response.ok) {
    throw new Error('Something went wrong.');
  }

  return await response.json();
}

export async function getToday() {
  const userId = getUserId();
  const response = await fetch(`${API_BASE}/today?userId=${encodeURIComponent(userId)}`);

  if (!response.ok) {
    throw new Error('Something went wrong.');
  }

  return await response.json();
}

export async function getHistory() {
  const userId = getUserId();
  const response = await fetch(`${API_BASE}/history?userId=${encodeURIComponent(userId)}`);

  if (!response.ok) {
    throw new Error('Something went wrong.');
  }

  return await response.json();
}

export async function getMonthly(year?: number, month?: number) {
  const userId = getUserId();
  let url = `${API_BASE}/monthly?userId=${encodeURIComponent(userId)}`;
  if (year) url += `&year=${year}`;
  if (month) url += `&month=${month}`;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error('Something went wrong.');
  }
  return await response.json();
}

export async function getLeaderboard(year?: number, month?: number) {
  let url = `${API_BASE}/leaderboard`;
  const params = [];
  if (year) params.push(`year=${year}`);
  if (month) params.push(`month=${month}`);
  if (params.length) url += `?${params.join('&')}`;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error('Something went wrong.');
  }
  return await response.json();
}
