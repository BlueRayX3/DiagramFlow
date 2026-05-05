import { mockProfile } from '../data/mockProfile';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7070';
const USERS_ENDPOINT = `${API_BASE_URL.replace(/\/$/, '')}/api/users`;

const mapProfile = (user) => {
  if (!user) return mockProfile;

  const roleName =
    user.role ||
    user.Role ||
    user.roleName ||
    user.RoleName ||
    (user.roleId || user.RoleId ? `Role ${user.roleId || user.RoleId}` : null);

  return {
    name: `${user.firstName || user.FirstName || ''} ${
      user.lastName || user.LastName || ''
    }`.trim() || user.username || user.Username || mockProfile.name,
    role: roleName || mockProfile.role,
    handle: `@${user.username || user.Username || 'profile'}`,
    email: user.email || user.Email || mockProfile.email,
    location: user.location || mockProfile.location,
    summary: mockProfile.summary,
    stats: mockProfile.stats,
    focus: mockProfile.focus,
    skills: mockProfile.skills,
    highlights: mockProfile.highlights
  };
};

export async function getProfile(userId = 1) {
  try {
    const response = await fetch(`${USERS_ENDPOINT}/${userId}`, {
      headers: { 'Content-Type': 'application/json' }
    });
    if (!response.ok) {
      throw new Error(`Request failed: ${response.status}`);
    }
    const data = await response.json();
    return mapProfile(data);
  } catch (error) {
    console.warn('Profile API unavailable, using mock data.', error);
    return mockProfile;
  }
}
