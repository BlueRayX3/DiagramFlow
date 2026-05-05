import { cn } from '../../lib/utils';

export default function Badge({ variant = 'default', className, ...props }) {
  return (
    <span
      className={cn('ui-badge', `ui-badge--${variant}`, className)}
      {...props}
    />
  );
}
