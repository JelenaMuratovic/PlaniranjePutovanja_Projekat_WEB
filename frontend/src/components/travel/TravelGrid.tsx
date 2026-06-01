import type { TravelDto } from "../../models/travel/travel/dtos";
import { TravelCard } from "./TravelCard";

type TravelGridProps = {
  travels: TravelDto[];
  onViewPlan: (travel: TravelDto) => void;
  onEditPlan: (travel: TravelDto) => void;
  onDeletePlan: (travel: TravelDto) => void;
};

export const TravelGrid = ({
  travels,
  onViewPlan,
  onEditPlan,
  onDeletePlan,
}: TravelGridProps) => {
  if (travels.length === 0) {
    return (
      <div className="empty-state">
        <h3>No travel plans yet</h3>
        <p>
          Create your first trip to start building destinations, activities and
          checklists.
        </p>
      </div>
    );
  }

  // Prikazujemo putovanja obrnutim redosledom (najnovije prvo)
  const ordered = travels.slice().reverse();

  return (
    <div className="travel-grid">
      {ordered.map((travel) => (
        <TravelCard
          key={travel.id}
          travel={travel}
          onViewPlan={onViewPlan}
          onEdit={onEditPlan}
          onDelete={onDeletePlan}
        />
      ))}
    </div>
  );
};
