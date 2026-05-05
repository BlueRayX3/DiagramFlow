import Avatar from '../components/ui/avatar';
import Badge from '../components/ui/badge';
import Button from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { useProfile } from '../hooks/useProfile';
import { IconArrowRight } from '../components/icons';

export default function Profile() {
  const { profile } = useProfile(1);
  const activeProfile = profile || {
    name: 'Loading',
    role: 'Profile',
    handle: '@loading',
    summary: 'Fetching profile details.',
    stats: [],
    focus: [],
    highlights: [],
    skills: []
  };

  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Profile</h1>
          <p className="page__subtitle">Keep your identity and focus aligned.</p>
        </div>
        <Button variant="outline" size="sm">
          Edit profile
        </Button>
      </div>

      <div className="profile-grid">
        <Card>
          <CardHeader className="profile-card__header">
            <Avatar fallback={activeProfile.name.slice(0, 2).toUpperCase()} />
            <div>
              <div className="profile-card__name">{activeProfile.name}</div>
              <div className="profile-card__meta">{activeProfile.role}</div>
              <div className="profile-card__meta">{activeProfile.handle}</div>
            </div>
          </CardHeader>
          <CardContent>
            <p className="page__subtitle">{activeProfile.summary}</p>
            <div className="hero__metrics">
              {activeProfile.stats.map((stat) => (
                <div key={stat.label} className="hero__metric">
                  <div className="hero__metric-value">{stat.value}</div>
                  <div className="hero__metric-label">{stat.label}</div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        <div className="panel-grid">
          <Card>
            <CardHeader>
              <CardTitle>Focus</CardTitle>
              <Badge variant="primary">Now</Badge>
            </CardHeader>
            <CardContent className="skill-list">
              {activeProfile.focus.map((item) => (
                <Badge key={item} variant="outline">
                  {item}
                </Badge>
              ))}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Highlights</CardTitle>
            </CardHeader>
            <CardContent className="activity-list">
              {activeProfile.highlights.map((item) => (
                <div key={item.title} className="activity-item">
                  <div>
                    <div className="activity-item__title">{item.title}</div>
                    <div className="hero__panel-meta">{item.detail}</div>
                  </div>
                  <Badge variant="outline">{item.time}</Badge>
                </div>
              ))}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Skills</CardTitle>
              <Button variant="ghost" size="sm">
                View all
                <IconArrowRight className="icon" />
              </Button>
            </CardHeader>
            <CardContent className="skill-list">
              {activeProfile.skills.map((skill) => (
                <span key={skill} className="ui-tag">
                  {skill}
                </span>
              ))}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
