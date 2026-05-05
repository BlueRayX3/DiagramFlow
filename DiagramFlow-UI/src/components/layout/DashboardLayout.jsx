import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Topbar from './Topbar';
import { IconFolder, IconGrid, IconUser } from '../icons';

const navItems = [
  { to: '/dashboard', label: 'Overview', icon: <IconGrid className="icon" /> },
  {
    to: '/dashboard/projects',
    label: 'Projelerim',
    icon: <IconFolder className="icon" />
  },
  { to: '/dashboard/profile', label: 'Profile', icon: <IconUser className="icon" /> }
];

export default function DashboardLayout() {
  return (
    <div className="app-shell">
      <Sidebar items={navItems} />
      <div className="app-shell__main">
        <Topbar />
        <main className="app-shell__content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
