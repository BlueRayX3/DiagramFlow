const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7070';
const AUTH_ENDPOINT = `${API_BASE_URL.replace(/\/$/, '')}/api/auth`;

const handleResponse = async (response) => {
  if (!response.ok) {
    const raw = await response.text();
    let message = raw;
    try {
      const parsed = JSON.parse(raw);
      message = parsed.detail || parsed.title || raw;
    } catch {
      message = raw;
    }
    throw new Error(message || `Request failed: ${response.status}`);
  }
  return response.json().catch(() => ({}));
};

export async function registerUser(payload) {
  const response = await fetch(`${AUTH_ENDPOINT}/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  return handleResponse(response);
}

export async function loginUser(payload) {
  const response = await fetch(`${AUTH_ENDPOINT}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  return handleResponse(response);
}
