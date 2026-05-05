import Badge from '../ui/badge';
import Button from '../ui/button';
import Input from '../ui/input';
import { IconBell, IconPlus, IconSearch } from '../icons';

export default function Topbar() {
  return (
    <header className="topbar">
      <div className="topbar__search">
        <IconSearch className="icon" />
        <Input placeholder="Search flows, teams, or tags" />
      </div>
      <div className="topbar__actions">
        <Badge variant="outline">v2.1</Badge>
        <Button variant="ghost" size="sm" className="icon-button" aria-label="Notifications">
          <IconBell className="icon" />
        </Button>
        <Button size="sm">
          <IconPlus className="icon" />
          New flow
        </Button>
      </div>
    </header>
  );
}
