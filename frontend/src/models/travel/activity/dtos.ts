export type ActivityStatus = "Planned" | "Reserved" | "Completed" | "Cancelled";

export type ActivityDto = {
  id: string;
  destinationId: string;
  name: string;
  description: string;
  activityDate: string;
  startTime: string | null;
  price: number;
  status: ActivityStatus;
  createdAt: string;
};

export type CreateActivityDto = {
  name: string;
  description: string;
  activityDate: string;
  startTime: string | null;
  price: number;
  status: ActivityStatus;
};