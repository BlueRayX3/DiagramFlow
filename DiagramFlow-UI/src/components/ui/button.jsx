import { forwardRef } from 'react';
import { cn } from '../../lib/utils';

const Button = forwardRef(function Button(
  { variant = 'primary', size = 'md', className, ...props },
  ref
) {
  return (
    <button
      ref={ref}
      className={cn(
        'ui-button',
        `ui-button--${variant}`,
        `ui-button--${size}`,
        className
      )}
      {...props}
    />
  );
});

export default Button;
