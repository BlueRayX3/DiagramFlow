import { useCallback, useEffect, useState } from 'react';
import { getProfile } from '../services/usersService';

export function useProfile(userId) {
  const [profile, setProfile] = useState(null);
  const [status, setStatus] = useState('idle');
  const [error, setError] = useState(null);

  const loadProfile = useCallback(async () => {
    if (!userId) {
      setProfile(null);
      setStatus('idle');
      setError(null);
      return;
    }
    setStatus('loading');
    setError(null);
    try {
      const data = await getProfile(userId);
      setProfile(data);
      setStatus('success');
    } catch (err) {
      setError(err);
      setStatus('error');
    }
  }, [userId]);

  useEffect(() => {
    loadProfile();
  }, [loadProfile]);

  return { profile, status, error, refresh: loadProfile };
}
