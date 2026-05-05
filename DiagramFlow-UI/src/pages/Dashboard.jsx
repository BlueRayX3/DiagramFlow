import Badge from '../components/ui/badge';
import Button from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import ProjectCard from '../components/projects/ProjectCard';
import { useProjects } from '../hooks/useProjects';
import { mockActivity, quickActions } from '../data/mockDashboard';
import { IconArrowRight, IconBolt } from '../components/icons';

const getStats = (projects) => {
  const counts = { total: 0, active: 0, review: 0, completed: 0 };
  projects.forEach((project) => {
    counts.total += 1;
    if (project.status === 'Active') counts.active += 1;
    if (project.status === 'Review') counts.review += 1;
    if (project.status === 'Completed') counts.completed += 1;
  });
  return [
    { label: 'Total flows', value: counts.total, note: 'All tracked lanes' },
    { label: 'Active', value: counts.active, note: 'In motion now' },
    { label: 'Review', value: counts.review, note: 'Awaiting feedback' },
    { label: 'Completed', value: counts.completed, note: 'Delivered flows' }
  ];
};

export default function Dashboard() {
  const { projects } = useProjects();
  const stats = getStats(projects);
  const highlights = projects.slice(0, 2);

  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Overview</h1>
          <p className="page__subtitle">Your command center for active flows.</p>
        </div>
        <Button size="sm">
          <IconBolt className="icon" />
          Create flow
        </Button>
      </div>

      <div className="stats-grid">
        {stats.map((stat) => (
          <Card key={stat.label}>
            <CardHeader>
              <CardTitle>{stat.label}</CardTitle>
              <Badge variant="outline">Live</Badge>
            </CardHeader>
            <CardContent>
              <div className="hero__metric-value">{stat.value}</div>
              <div className="hero__metric-label">{stat.note}</div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="panel-grid">
        <Card>
          <CardHeader>
            <CardTitle>Momentum</CardTitle>
            <Badge variant="primary">Today</Badge>
          </CardHeader>
          <CardContent className="activity-list">
            {mockActivity.map((activity) => (
              <div key={activity.id} className="activity-item">
                <div>
                  <div className="activity-item__title">{activity.title}</div>
                  <div className="hero__panel-meta">{activity.detail}</div>
                </div>
                <Badge variant={activity.tone}>{activity.time}</Badge>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Quick actions</CardTitle>
            <Badge variant="outline">Suggested</Badge>
          </CardHeader>
          <CardContent className="activity-list">
            {quickActions.map((action) => (
              <div key={action.id} className="activity-item">
                <div>
                  <div className="activity-item__title">{action.title}</div>
                  <div className="hero__panel-meta">{action.description}</div>
                </div>
                <Button variant="ghost" size="sm">
                  {action.cta}
                  <IconArrowRight className="icon" />
                </Button>
              </div>
            ))}
          </CardContent>
        </Card>
      </div>

      <div>
        <div className="section-heading">
          <div>
            <h2 className="section-title">Project highlights</h2>
            <p className="section-subtitle">Fast look at your most active flows.</p>
          </div>
        </div>
        <div className="project-grid">
          {highlights.map((project) => (
            <ProjectCard key={project.id} project={project} />
          ))}
        </div>
      </div>
    </div>
  );
}
