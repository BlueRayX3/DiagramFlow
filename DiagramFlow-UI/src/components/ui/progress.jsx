import { cn } from '../../lib/utils';

export default function Progress({ value = 0, className }) {
  const safeValue = Math.max(0, Math.min(100, value));
  return (
    <div className={cn('ui-progress', className)}>
      <div className="ui-progress__bar" style={{ width: `${safeValue}%` }} />
    </div>
  );
}
