import { useGetNotificationsQuery, useMarkAsReadMutation } from "@/app/api/notificationApiSlice";
import { ScrollArea } from "../ui/scroll-area";
import { NotificationCard } from "./notification-card";
import { useEffect, useState } from "react";
import { UserNotification } from "@/types/auth";

export function NotificationsList() {
  const { data, isLoading, isError, refetch } = useGetNotificationsQuery();
  const [markAsRead] = useMarkAsReadMutation();

  useEffect(() => {
    // refetch toda vez que o componente for montado
    refetch();
  }, [refetch]);

  const [notifications, setNotifications] = useState<UserNotification[]>([]);

  useEffect(() => {
    if (data) {
      setNotifications(data);
    }
  }, [data]);

  const onMarkAsRead = (notificationIdentifier: string) => {
    markAsRead(notificationIdentifier).unwrap();

    setNotifications((prev) =>
      prev.filter((n) => n.uuid !== notificationIdentifier) // <- remove da lista
    );

  }

  if (isLoading) {
    return (
      <div className="p-4 text-center text-sm text-muted-foreground">
        Carregando notificações...
      </div>
    );
  }

  if (isError) {
    return (
      <div className="p-4 text-center text-sm text-destructive">
        Ocorreu um erro ao carregar as notificações.
      </div>
    );
  }

  if (!data || data.length === 0) {
    return (
      <div className="p-4 text-center text-sm text-muted-foreground">
        Nenhuma notificação encontrada.
      </div>
    );
  }

  return (
    <ScrollArea className="w-full rounded-md py-4">
      <div className="space-y-4">
        {notifications.length === 0 && (<p>Nenhuma notificação encontrada.</p>)}
          {notifications.map(notification => {
            return <>
              <NotificationCard
                key={notification.uuid}
                notification={notification}
                onMarkAsRead={() => onMarkAsRead(notification.uuid)} />
            </>
          })}
      </div>
    </ScrollArea>
  );
}