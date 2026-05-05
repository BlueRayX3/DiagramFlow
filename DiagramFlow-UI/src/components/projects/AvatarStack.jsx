import Avatar from '../ui/avatar';

export default function AvatarStack({ members = [] }) {
  const visibleMembers = members.slice(0, 3);
  const extraCount = members.length - visibleMembers.length;

  return (
    <div className="avatar-stack">
      {visibleMembers.map((member) => (
        <Avatar
          key={member.id || member.initials}
          alt={member.name}
          fallback={member.initials}
          src={member.avatar}
        />
      ))}
      {extraCount > 0 && (
        <span className="avatar-stack__extra">+{extraCount}</span>
      )}
    </div>
  );
}
