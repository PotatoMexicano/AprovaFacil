// src/components/timeline.tsx
import { RequestResponse } from "@/types/request";
import { cn } from "@/lib/utils";
import { CheckCircle2, XCircle, Clock } from "lucide-react";
import { Badge } from "@/app/components/ui/badge";
import { Separator } from "@/app/components/ui/separator";

interface TimelineProps {
  item: RequestResponse;
  className?: string;
}

function formatDate(dateString: string | null): string {
  return dateString ? new Date(dateString).toLocaleDateString() : '';
}

function getApprovalIcon(approved: number | null) {
  if (approved === null) return <Clock className="h-4 w-4 text-orange-500" />;
  return approved === 1 ? (
    <CheckCircle2 className="h-4 w-4 text-green-500" />
  ) : (
    <XCircle className="h-4 w-4 text-red-500" />
  );
}

export function Timeline({ item, className }: TimelineProps) {
  // Check for rejections at each level
  const isRejectedByManager = item.first_level_at && item.approved_first_level === 0;
  const isRejectedByDirector = item.second_level_at && item.approved_second_level === 0;

  return (
    <div className={cn("relative space-y-8 pb-4", className)}>
      {/* Vertical line */}
      <div className="absolute left-2.5 top-3 bottom-4 w-px bg-primary/20" />

      <div key={item.uuid} className="space-y-8">
        {/* Registrado */}
        <TimelinePoint
          title="Registrado"
          date={item.create_at}
          actor={item.requester.full_name}
          approved={1}
          showIcon={true}
          iconText="Registrado"
          isActive={true}
        />

        {!isRejectedByManager && (
          <>
            <Separator className="ml-[11px] bg-primary/20" />
            {/* Gerente */}
            <TimelinePoint
              title="Gerente"
              date={item.first_level_at}
              actor={item.first_level_at ? item.managers[0]?.full_name : undefined}
              approved={item.first_level_at ? item.approved_first_level : null}
              showIcon={true}
              isActive={!!item.first_level_at}
            />
          </>
        )}

        {!isRejectedByManager && !isRejectedByDirector && item.directors.length > 0 && (
          <>
            <Separator className="ml-[11px] bg-primary/20" />
            {/* Diretor */}
            <TimelinePoint
              title="Diretor"
              date={item.second_level_at}
              actor={item.second_level_at ? item.directors[0]?.full_name : undefined}
              approved={item.second_level_at ? item.approved_second_level : null}
              showIcon={true}
              isActive={!!item.second_level_at}
            />
          </>
        )}

        {!isRejectedByManager && !isRejectedByDirector && item.approved === 1 && (
          <>
            <Separator className="ml-[11px] bg-primary/20" />
            {/* Faturado */}
            <TimelinePoint
              title="Faturado"
              date={item.received_at}
              actor={item.received_at ? item.finisher?.full_name : undefined}
              approved={item.received_at ? item.approved : null}
              showIcon={true}
              isActive={!!item.received_at}
            />
          </>
        )}
      </div>
    </div>
  );
}

interface TimelinePointProps {
  title: string;
  date: string | null;
  actor?: string;
  approved: number | null;
  showIcon: boolean;
  iconText?: string;
  isActive: boolean;
}

function TimelinePoint({ title, date, actor, approved, showIcon, iconText, isActive }: TimelinePointProps) {
  return (
    <div className="relative pl-10">
      {/* Timeline dot */}
      <div className="absolute left-[1px] top-1.5 flex h-5 w-5 items-center justify-center">
        <div className={cn(
          "h-3 w-3 rounded-full",
          isActive ? (approved === 0 ? "bg-red-500" 
            : approved === 1 ? "bg-green-500"
            : approved === -1 ? "bg-red-500"
             : "bg-green-500") : "bg-gray-300"
        )} />
      </div>

      {/* Content */}
      <div className="flex flex-col gap-2">
        <div className="flex items-center justify-between">
          <span className="text-sm font-medium text-foreground">{title}</span>
          <span className="text-sm text-muted-foreground">
            {date ? formatDate(date) : "Aguardando"}
          </span>
        </div>

        <div className="flex w-full justify-between">
          {showIcon && (
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                {getApprovalIcon(approved)}
                <span className="text-sm">
                  {!isActive ? (
                      <p>Aguardando</p>
                  ) : (
                    iconText || (approved === 1 ? "Aprovado" : "Rejeitado")
                  )}
                </span>
              </div>
            </div>
          )}

          {actor && (
            <div className="flex justify-end">
              <Badge variant="secondary" className="text-xs">
                {actor}
              </Badge>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
