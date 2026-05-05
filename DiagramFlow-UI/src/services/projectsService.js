import { mockProjects } from '../data/mockProjects';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7070';
const PROJECTS_ENDPOINT = `${API_BASE_URL.replace(/\/$/, '')}/api/projects`;

const statusCycle = ['Active', 'Review', 'Paused', 'Completed'];
const healthCycle = ['On Track', 'At Risk', 'Excellent'];

const toNumber = (value, fallback) => {
  const parsed = Number(value);
  return Number.isNaN(parsed) ? fallback : parsed;
};

const toString = (value, fallback) => {
  if (value === null || value === undefined) return fallback;
  return String(value);
};

const pickFallback = (index) => mockProjects[index % mockProjects.length];

const mapProject = (item, index) => {
  const fallback = pickFallback(index);
  const id =
    item.ProjectID ??
    item.ProjectId ??
    item.projectId ??
    item.id ??
    item.ID ??
    fallback.id;
  const name =
    item.ProjectName ??
    item.Name ??
    item.name ??
    fallback.name ??
    `Project ${id}`;
  const description =
    item.Description ?? item.description ?? fallback.description ?? 'No description';
  const owner = item.OwnerName ?? item.owner ?? item.Owner ?? fallback.owner;
  const status = item.Status ?? item.status ?? statusCycle[index % statusCycle.length];
  const progress = toNumber(
    item.Progress ?? item.progress,
    28 + (index * 13) % 68
  );
  const updatedAt =
    item.UpdatedAt ??
    item.updatedAt ??
    new Date(Date.now() - index * 86400000).toISOString();

  return {
    id: toString(id, fallback.id),
    name,
    description,
    owner,
    status,
    progress,
    updatedAt,
    tags: item.Tags ?? item.tags ?? fallback.tags,
    health: item.Health ?? item.health ?? healthCycle[index % healthCycle.length],
    members: item.Members ?? item.members ?? fallback.members
  };
};

const mapProjects = (data) => data.map(mapProject);

export async function getProjects() {
  try {
    const response = await fetch(PROJECTS_ENDPOINT, {
      headers: { 'Content-Type': 'application/json' }
    });
    if (!response.ok) {
      throw new Error(`Request failed: ${response.status}`);
    }
    const data = await response.json();
    if (!Array.isArray(data) || data.length === 0) {
      return mockProjects;
    }
    return mapProjects(data);
  } catch (error) {
    console.warn('Projects API unavailable, using mock data.', error);
    return mockProjects;
  }
}

export async function createProject(payload) {
  const response = await fetch(PROJECTS_ENDPOINT, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed: ${response.status}`);
  }

  return response.json().catch(() => ({}));
}
