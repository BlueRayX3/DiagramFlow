import Badge from '../ui/badge';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '../ui/card';
import Progress from '../ui/progress';
import AvatarStack from './AvatarStack';
import { formatShortDate } from '../../utils/date';

const statusVariant = {
  Active: 'success',
  Review: 'warning',
  Paused: 'muted',
  Completed: 'primary'
};

export default function ProjectCard({ project }) {
  const variant = statusVariant[project.status] || 'default';

  return (
    <Card className="project-card">
      <CardHeader className="project-card__header">
        <div>
          <CardTitle>{project.name}</CardTitle>
          <CardDescription>{project.description}</CardDescription>
        </div>
        <Badge variant={variant}>{project.status}</Badge>
      </CardHeader>
      <CardContent className="project-card__content">
        <div className="project-card__meta">
          <span className="project-card__owner">@{project.owner}</span>
          <span className="project-card__date">
            Updated {formatShortDate(project.updatedAt)}
          </span>
        </div>
        <Progress value={project.progress} />
        <div className="project-card__tags">
          {project.tags?.map((tag) => (
            <span key={tag} className="ui-tag">
              {tag}
            </span>
          ))}
        </div>
      </CardContent>
      <CardFooter className="project-card__footer">
        <div className="project-card__health">
          <span className="project-card__health-dot" data-tone={variant} />
          <span>{project.health}</span>
        </div>
        <AvatarStack members={project.members} />
      </CardFooter>
    </Card>
  );
}
