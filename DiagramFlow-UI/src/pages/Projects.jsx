import { useMemo, useState } from 'react';
import Button from '../components/ui/button';
import Input from '../components/ui/input';
import ProjectCard from '../components/projects/ProjectCard';
import { useProjects } from '../hooks/useProjects';
import { cn } from '../lib/utils';
import { createProject } from '../services/projectsService';

const statuses = ['All', 'Active', 'Review', 'Paused', 'Completed'];

export default function Projects() {
  const { projects, refresh } = useProjects();
  const [query, setQuery] = useState('');
  const [status, setStatus] = useState('All');
  const [isPopoverOpen, setIsPopoverOpen] = useState(false);
  const [formState, setFormState] = useState({ name: '', description: '' });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState('');

  const filteredProjects = useMemo(() => {
    return projects.filter((project) => {
      const matchesStatus = status === 'All' || project.status === status;
      const matchesQuery = project.name
        .toLowerCase()
        .includes(query.toLowerCase());
      return matchesStatus && matchesQuery;
    });
  }, [projects, query, status]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    const trimmedName = formState.name.trim();

    if (!trimmedName) {
      setFormError('Project name is required.');
      return;
    }

    setFormError('');
    setIsSubmitting(true);

    try {
      await createProject({
        name: trimmedName,
        description: formState.description.trim(),
        isPublic: true
      });
      await refresh();
      setFormState({ name: '', description: '' });
      setIsPopoverOpen(false);
    } catch (error) {
      setFormError(error.message || 'Unable to save project. Please try again.');
      console.error(error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Projelerim</h1>
          <p className="page__subtitle">Track every flow and its live status.</p>
        </div>
        <div className="popover">
          <Button size="sm" onClick={() => setIsPopoverOpen(true)}>
            New project
          </Button>
          {isPopoverOpen && (
            <>
              <button
                type="button"
                className="popover__backdrop"
                aria-label="Close new project"
                onClick={() => setIsPopoverOpen(false)}
              />
              <div className="popover__panel" role="dialog" aria-modal="true">
                <div className="popover__header">
                  <div>
                    <h3 className="popover__title">Create project</h3>
                    <p className="popover__subtitle">Add a new flow to the workspace.</p>
                  </div>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => setIsPopoverOpen(false)}
                    aria-label="Close"
                  >
                    Close
                  </Button>
                </div>
                <form className="popover__form" onSubmit={handleSubmit}>
                  <label className="popover__label" htmlFor="project-name">
                    Project name
                  </label>
                  <Input
                    id="project-name"
                    placeholder="New project name"
                    value={formState.name}
                    onChange={(event) =>
                      setFormState((prev) => ({
                        ...prev,
                        name: event.target.value
                      }))
                    }
                  />

                  <label className="popover__label" htmlFor="project-description">
                    Description
                  </label>
                  <Input
                    id="project-description"
                    placeholder="Describe the flow in one line"
                    value={formState.description}
                    onChange={(event) =>
                      setFormState((prev) => ({
                        ...prev,
                        description: event.target.value
                      }))
                    }
                  />

                  {formError && <p className="popover__error">{formError}</p>}

                  <div className="popover__actions">
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      onClick={() => setIsPopoverOpen(false)}
                    >
                      Cancel
                    </Button>
                    <Button size="sm" type="submit" disabled={isSubmitting}>
                      {isSubmitting ? 'Saving...' : 'Create'}
                    </Button>
                  </div>
                </form>
              </div>
            </>
          )}
        </div>
      </div>

      <div className="panel-grid">
        <div className="filters">
          {statuses.map((item) => (
            <Button
              key={item}
              variant="ghost"
              size="sm"
              className={cn('filter-button', item === status && 'is-active')}
              onClick={() => setStatus(item)}
            >
              {item}
            </Button>
          ))}
        </div>
        <Input
          placeholder="Search by project name"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
      </div>

      <div className="project-grid">
        {filteredProjects.map((project) => (
          <ProjectCard key={project.id} project={project} />
        ))}
      </div>
    </div>
  );
}
