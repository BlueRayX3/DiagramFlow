import { Link } from 'react-router-dom';
import Badge from '../components/ui/badge';
import Button from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import ProjectCard from '../components/projects/ProjectCard';
import { useProjects } from '../hooks/useProjects';
import { IconArrowRight, IconBolt, IconLayers } from '../components/icons';

const heroMetrics = [
  { label: 'Active flows', value: '24' },
  { label: 'Teams online', value: '11' },
  { label: 'Review speed', value: '2.3 days' }
];

const featureCards = [
  {
    title: 'Adaptive flowboards',
    description: 'Flexible lanes that reshape as your work evolves.',
    icon: <IconLayers className="icon" />
  },
  {
    title: 'Signal focused views',
    description: 'Highlight the highest impact decisions in seconds.',
    icon: <IconBolt className="icon" />
  },
  {
    title: 'Narrative ready exports',
    description: 'Turn flows into decks, briefs, and reports.',
    icon: <IconArrowRight className="icon" />
  }
];

const flowPreview = [
  { title: 'Brand Atlas', meta: 'Review lane · 78% aligned' },
  { title: 'Carbon Sync', meta: 'Stakeholder pass · Today' },
  { title: 'Route Weaver', meta: 'Next checkpoint · Fri' }
];

export default function Home() {
  const { projects } = useProjects();
  const featuredProjects = projects.slice(0, 3);

  return (
    <div className="home">
      <section className="hero">
        <div>
          <Badge variant="primary">New release</Badge>
          <h1 className="hero__title">Flow design for teams that move fast.</h1>
          <p className="hero__subtitle">
            DiagramFlow keeps your projects aligned with a dark crimson inspired
            command center. Track every decision, every flow, every outcome.
          </p>
          <div className="hero__actions">
            <Link className="ui-button ui-button--primary ui-button--lg" to="/dashboard">
              Open dashboard
              <IconArrowRight className="icon" />
            </Link>
            <Link className="ui-button ui-button--outline ui-button--lg" to="/dashboard/projects">
              View projects
            </Link>
            <Button variant="ghost" size="lg">Request walkthrough</Button>
          </div>
          <div className="hero__auth">
            <Link to="/login" className="auth__link">Login</Link>
            <span className="hero__auth-divider">or</span>
            <Link to="/register" className="auth__link">Register</Link>
          </div>
          <div className="hero__metrics">
            {heroMetrics.map((metric) => (
              <div key={metric.label} className="hero__metric">
                <div className="hero__metric-value">{metric.value}</div>
                <div className="hero__metric-label">{metric.label}</div>
              </div>
            ))}
          </div>
        </div>

        <div className="hero__panel">
          <div className="hero__panel-grid">
            {flowPreview.map((item) => (
              <div key={item.title} className="hero__panel-item">
                <div>
                  <div className="hero__panel-title">{item.title}</div>
                  <div className="hero__panel-meta">{item.meta}</div>
                </div>
                <Badge variant="outline">Live</Badge>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section>
        <div className="section-heading">
          <div>
            <h2 className="section-title">Designed for clarity</h2>
            <p className="section-subtitle">
              Bold structure, soft edges, and a light red pulse.
            </p>
          </div>
          <Link className="ui-button ui-button--ghost ui-button--sm" to="/dashboard">
            Explore overview
            <IconArrowRight className="icon" />
          </Link>
        </div>
        <div className="feature-grid">
          {featureCards.map((feature) => (
            <Card key={feature.title} className="feature-card">
              <CardHeader>
                {feature.icon}
                <CardTitle>{feature.title}</CardTitle>
              </CardHeader>
              <CardContent>{feature.description}</CardContent>
            </Card>
          ))}
        </div>
      </section>

      <section>
        <div className="section-heading">
          <div>
            <h2 className="section-title">Latest projects</h2>
            <p className="section-subtitle">Always synced with your API.</p>
          </div>
          <Link className="ui-button ui-button--outline ui-button--sm" to="/dashboard/projects">
            All projects
          </Link>
        </div>
        <div className="project-grid">
          {featuredProjects.map((project) => (
            <ProjectCard key={project.id} project={project} />
          ))}
        </div>
      </section>
    </div>
  );
}
