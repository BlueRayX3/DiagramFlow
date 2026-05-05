import { cn } from '../../lib/utils';

export default function Avatar({ src, alt, fallback = 'DF', className }) {
  return (
    <div className={cn('ui-avatar', className)} aria-label={alt || 'Avatar'}>
      {src ? <img src={src} alt={alt || 'Avatar'} /> : <span>{fallback}</span>}
    </div>
  );
}
