import { cn } from "@/lib/utils";
import { Bell, Check } from "lucide-react";
import { UserNotification } from "@/types/auth";
import { Link } from "react-router-dom";

interface NotificationCardProps {
  notification: UserNotification;
  onMarkAsRead?: (uuid: string) => void;
}

export function NotificationCard({ notification, onMarkAsRead }: NotificationCardProps) {

  return (
    <div
      className={cn(
        "flex items-start gap-4 rounded-lg border p-4 transition-colors",
        notification.read ? "bg-background" : "bg-muted/50"
      )}
    >
      <div className="relative">
        <Bell className="h-5 w-5 text-muted-foreground" />
      </div>

      <div className="flex-1 space-y-1">
        <Link to={`/request/${notification.request_uuid}`} onClick={() => onMarkAsRead(notification.uuid)}>
          <p className="text-sm text-foreground">{notification.message}</p>
        </Link>
      </div>


      {!notification.read && onMarkAsRead && (
        <button
          onClick={() => onMarkAsRead(notification.uuid)}
          className="rounded-full p-1 hover:bg-muted"
          title="Mark as read">
          <Check className="h-4 w-4" />
        </button>
      )}
    </div>
  );
}