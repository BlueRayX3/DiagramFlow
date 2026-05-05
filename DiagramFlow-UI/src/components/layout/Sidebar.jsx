import { Link, NavLink } from 'react-router-dom';
import { cn } from '../../lib/utils';
import { LogoMark } from '../icons';

export default function Sidebar({ items }) {
  return (
    <aside className="sidebar">
      <Link to="/" className="sidebar__brand">
        <LogoMark className="sidebar__logo" />
        <div>
          <span className="sidebar__title">DiagramFlow</span>
          <span className="sidebar__subtitle">Dark Crimson</span>
        </div>
      </Link>

      <div className="sidebar__section">
        <p className="sidebar__label">Workspace</p>
        <nav className="sidebar__nav">
          {items.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn('sidebar__link', isActive && 'is-active')
              }
            >
              {item.icon}
              <span>{item.label}</span>
            </NavLink>
          ))}
        </nav>
      </div>

      <div className="sidebar__footer">
        <div className="sidebar__pulse" />
        <div>
          <p className="sidebar__footer-title">Focus mode</p>
          <p className="sidebar__footer-text">Muted distractions, higher signal.</p>
        </div>
      </div>
    </aside>
  );
}
