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

function getTimezone() {
  return Intl.DateTimeFormat().resolvedOptions().timeZone;
}

export async function logSet(data: Omit<WorkoutLogDto, 'userId'>) {
  const userId = getUserId();
  const timezone = getTimezone();
  const response = await fetch(`${API_BASE}/log?timezone=${encodeURIComponent(timezone)}`, {
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
  const timezone = getTimezone();
  const response = await fetch(
    `${API_BASE}/today?userId=${encodeURIComponent(userId)}&timezone=${encodeURIComponent(timezone)}`
  );

  if (!response.ok) {
    throw new Error('Something went wrong.');
  }

  return await response.json();
}

export async function getHistory() {
  const userId = getUserId();
  const timezone = getTimezone();
  const response = await fetch(
    `${API_BASE}/history?userId=${encodeURIComponent(userId)}&timezone=${encodeURIComponent(timezone)}`
  );

  if (!response.ok) {
    throw new Error('Something went wrong.');
  }

  return await response.json();
}

export async function getMonthly(year?: number, month?: number) {
  const userId = getUserId();
  const timezone = getTimezone();
  let url = `${API_BASE}/monthly?userId=${encodeURIComponent(userId)}&timezone=${encodeURIComponent(timezone)}`;
  if (year) url += `&year=${year}`;
  if (month) url += `&month=${month}`;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error('Something went wrong.');
  }
  return await response.json();
}

export async function getLeaderboard(year?: number, month?: number) {
  const timezone = getTimezone();
  let url = `${API_BASE}/leaderboard?timezone=${encodeURIComponent(timezone)}`;
  const params = [];
  if (year) params.push(`year=${year}`);
  if (month) params.push(`month=${month}`);
  if (params.length) url += `&${params.join('&')}`;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error('Something went wrong.');
  }
  return await response.json();
}

export async function getGoal(year?: number, month?: number) {
  const userId = getUserId();
  let url = `${API_BASE}/goal?userId=${encodeURIComponent(userId)}`;
  if (year) url += `&year=${year}`;
  if (month) url += `&month=${month}`;
  const response = await fetch(url);
  if (!response.ok) {
    if (response.status === 404) {
      return { pushupsGoal: 0, squatsGoal: 0, absGoal: 0 }; // No goal set
    }
    throw new Error('Failed to fetch goal');
  }
  return await response.json();
}

export async function setGoal({
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
  const userId = getUserId();
  const response = await fetch(`${API_BASE}/goal`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, year, month, pushupsGoal, squatsGoal, absGoal }),
  });
  if (!response.ok) {
    throw new Error('Failed to set goal');
  }

  // No data to return, just confirmation of success
  return;
}
