import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Badge from '../components/ui/badge';
import Button from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import Input from '../components/ui/input';
import { loginUser } from '../services/authService';
import { setStoredUser } from '../utils/auth';

export default function Login() {
  const navigate = useNavigate();
  const [formState, setFormState] = useState({ usernameOrEmail: '', password: '' });
  const [status, setStatus] = useState('idle');
  const [error, setError] = useState('');

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setStatus('loading');

    try {
      const user = await loginUser({
        usernameOrEmail: formState.usernameOrEmail.trim(),
        password: formState.password
      });
      setStoredUser(user);
      setStatus('success');
      navigate('/dashboard');
    } catch (err) {
      setError(err.message || 'Unable to sign in.');
      setStatus('error');
    }
  };

  return (
    <div className="auth">
      <div className="auth__grid">
        <div className="auth__intro">
          <Badge variant="primary">Welcome back</Badge>
          <h1 className="auth__title">Sign in to your flowboard</h1>
          <p className="auth__subtitle">
            Continue shaping your diagrams with a calm, focused workspace.
          </p>
          <div className="auth__hint">
            Need a new workspace?{' '}
            <Link to="/register" className="auth__link">
              Create an account
            </Link>
          </div>
        </div>

        <Card className="auth__card">
          <CardHeader>
            <CardTitle>Login</CardTitle>
          </CardHeader>
          <CardContent>
            <form className="auth__form" onSubmit={handleSubmit}>
              <label className="auth__label" htmlFor="login-identity">
                Username or email
              </label>
              <Input
                id="login-identity"
                placeholder="username or email"
                value={formState.usernameOrEmail}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    usernameOrEmail: event.target.value
                  }))
                }
              />

              <label className="auth__label" htmlFor="login-password">
                Password
              </label>
              <Input
                id="login-password"
                type="password"
                placeholder="••••••••"
                value={formState.password}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    password: event.target.value
                  }))
                }
              />

              {error && <p className="auth__error">{error}</p>}

              <Button type="submit" size="lg" disabled={status === 'loading'}>
                {status === 'loading' ? 'Signing in...' : 'Sign in'}
              </Button>
            </form>
            <p className="auth__switch">
              Don't have an account?{' '}
              <Link to="/register" className="auth__link">
                Register
              </Link>
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
