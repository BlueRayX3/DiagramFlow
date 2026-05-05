import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Badge from '../components/ui/badge';
import Button from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import Input from '../components/ui/input';
import { registerUser } from '../services/authService';

export default function Register() {
  const navigate = useNavigate();
  const [formState, setFormState] = useState({
    firstName: '',
    lastName: '',
    username: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [status, setStatus] = useState('idle');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setSuccess('');

    if (formState.password !== formState.confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setStatus('loading');

    try {
      await registerUser({
        username: formState.username.trim(),
        email: formState.email.trim(),
        password: formState.password,
        firstName: formState.firstName.trim(),
        lastName: formState.lastName.trim()
      });
      setSuccess('Account created. Redirecting to login...');
      setStatus('success');
      setTimeout(() => navigate('/login'), 600);
    } catch (err) {
      setError(err.message || 'Unable to register.');
      setStatus('error');
    }
  };

  return (
    <div className="auth">
      <div className="auth__grid">
        <div className="auth__intro">
          <Badge variant="primary">New workspace</Badge>
          <h1 className="auth__title">Join DiagramFlow</h1>
          <p className="auth__subtitle">
            Set up a lightweight account to start mapping flows instantly.
          </p>
          <div className="auth__hint">
            Already have access?{' '}
            <Link to="/login" className="auth__link">
              Sign in
            </Link>
          </div>
        </div>

        <Card className="auth__card">
          <CardHeader>
            <CardTitle>Create account</CardTitle>
          </CardHeader>
          <CardContent>
            <form className="auth__form" onSubmit={handleSubmit}>
              <div className="auth__row">
                <div>
                  <label className="auth__label" htmlFor="register-firstname">
                    First name
                  </label>
                  <Input
                    id="register-firstname"
                    placeholder="First name"
                    value={formState.firstName}
                    onChange={(event) =>
                      setFormState((prev) => ({
                        ...prev,
                        firstName: event.target.value
                      }))
                    }
                  />
                </div>
                <div>
                  <label className="auth__label" htmlFor="register-lastname">
                    Last name
                  </label>
                  <Input
                    id="register-lastname"
                    placeholder="Last name"
                    value={formState.lastName}
                    onChange={(event) =>
                      setFormState((prev) => ({
                        ...prev,
                        lastName: event.target.value
                      }))
                    }
                  />
                </div>
              </div>

              <label className="auth__label" htmlFor="register-username">
                Username
              </label>
              <Input
                id="register-username"
                placeholder="username"
                value={formState.username}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    username: event.target.value
                  }))
                }
              />

              <label className="auth__label" htmlFor="register-email">
                Email
              </label>
              <Input
                id="register-email"
                type="email"
                placeholder="email@diagramflow.com"
                value={formState.email}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    email: event.target.value
                  }))
                }
              />

              <label className="auth__label" htmlFor="register-password">
                Password
              </label>
              <Input
                id="register-password"
                type="password"
                placeholder="Create a password"
                value={formState.password}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    password: event.target.value
                  }))
                }
              />

              <label className="auth__label" htmlFor="register-confirm">
                Confirm password
              </label>
              <Input
                id="register-confirm"
                type="password"
                placeholder="Repeat password"
                value={formState.confirmPassword}
                onChange={(event) =>
                  setFormState((prev) => ({
                    ...prev,
                    confirmPassword: event.target.value
                  }))
                }
              />

              {error && <p className="auth__error">{error}</p>}
              {success && <p className="auth__success">{success}</p>}

              <Button type="submit" size="lg" disabled={status === 'loading'}>
                {status === 'loading' ? 'Creating...' : 'Create account'}
              </Button>
            </form>
            <p className="auth__switch">
              Already have an account?{' '}
              <Link to="/login" className="auth__link">
                Login
              </Link>
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
