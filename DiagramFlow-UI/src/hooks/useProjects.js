import { useCallback, useEffect, useState } from 'react';
import { getProjects } from '../services/projectsService';

export function useProjects() {
  const [projects, setProjects] = useState([]);
  const [status, setStatus] = useState('idle');
  const [error, setError] = useState(null);

  const loadProjects = useCallback(async () => {
    setStatus('loading');
    setError(null);
    try {
      const data = await getProjects();
      setProjects(data);
      setStatus('success');
    } catch (err) {
      setError(err);
      setStatus('error');
    }
  }, []);

  useEffect(() => {
    loadProjects();
  }, [loadProjects]);

  return { projects, status, error, refresh: loadProjects };
}
