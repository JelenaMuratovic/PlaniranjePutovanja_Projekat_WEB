export type ChecklistDto = {
  id: string;
  travelId: string;
  item: string;
  isCompleted: boolean;
  createdAt: string;
};

export type CreateChecklistDto = {
  item: string;
};