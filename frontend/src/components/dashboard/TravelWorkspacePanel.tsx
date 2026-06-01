import { useEffect, useMemo, useRef, useState } from "react";
import type { TravelDto } from "../../models/travel/travel/dtos";
import { calculateTripDays } from "./TravelDayStrip";
import AddDestinationModal from "../destination/AddDestinationModal";
import DestinationCard from "../destination/DestinationCard";
import AddActivityModal from "../activity/AddActivityModal";
import ActivityCard from "../activity/ActivityCard";
import { Modal } from "../shared/Modal";
import { destinationApi } from "../../api/travel/destinationApi";
import { activityApi } from "../../api/travel/activityApi";
import { getApiErrorMessage } from "../../helpers/apiError";
import {
  addDaysToDateOnly,
  formatDateOnly,
  formatDateOnlyForApi,
  parseDateOnly,
} from "../../helpers/date";
import type {
  CreateDestinationDto,
  DestinationDto,
} from "../../models/travel/destination/dtos";
import type {
  ActivityDto,
  CreateActivityDto,
} from "../../models/travel/activity/dtos";

type TravelWorkspacePanelProps = {
  travel: TravelDto;
  onRefresh?: () => Promise<void> | void;
};

type DestinationSchedule = DestinationDto & {
  startDay: number;
  endDay: number;
  startDate: string;
  endDate: string;
};

const formatDateRange = (startDate: string, endDate: string): string => {
  const start = formatDateOnly(startDate);
  const end = formatDateOnly(endDate);

  return `${start} - ${end}`;
};

export const TravelWorkspacePanel = ({
  travel,
  onRefresh,
}: TravelWorkspacePanelProps) => {
  const [activeTab, setActiveTab] = useState<
    "details" | "destinations" | "activities" | "map" | "checklist"
  >("details");
  const [showAddDestination, setShowAddDestination] = useState(false);
  const [destinationBeingEdited, setDestinationBeingEdited] =
    useState<DestinationDto | null>(null);
  const [destinationPendingDelete, setDestinationPendingDelete] =
    useState<DestinationDto | null>(null);
  const [destinations, setDestinations] = useState<DestinationDto[]>([]);
  const [isLoadingDestinations, setIsLoadingDestinations] = useState(false);
  const [destinationNotice, setDestinationNotice] = useState<string | null>(
    null,
  );
  const [destinationError, setDestinationError] = useState<string | null>(null);
  const [activityDestination, setActivityDestination] =
    useState<DestinationSchedule | null>(null);
  const [activityBeingEdited, setActivityBeingEdited] = useState<{
    destination: DestinationSchedule;
    activity: ActivityDto;
  } | null>(null);
  const [activityPendingDelete, setActivityPendingDelete] = useState<{
    destination: DestinationSchedule;
    activity: ActivityDto;
  } | null>(null);
  const [activitiesByDestination, setActivitiesByDestination] = useState<
    Record<string, ActivityDto[]>
  >({});
  const [isLoadingActivities, setIsLoadingActivities] = useState(false);
  const [activityError, setActivityError] = useState<string | null>(null);
  const totalDays = calculateTripDays(travel.startDate, travel.endDate);
  const destinationNoticeTimer = useRef<number | null>(null);
  const destinationErrorTimer = useRef<number | null>(null);
  const activityErrorTimer = useRef<number | null>(null);

  const clearDestinationError = () => {
    if (destinationErrorTimer.current) {
      window.clearTimeout(destinationErrorTimer.current);
      destinationErrorTimer.current = null;
    }
    setDestinationError(null);
  };

  const showDestinationNotice = (message: string) => {
    if (destinationNoticeTimer.current) {
      window.clearTimeout(destinationNoticeTimer.current);
    }

    setDestinationNotice(message);
    destinationNoticeTimer.current = window.setTimeout(() => {
      setDestinationNotice(null);
      destinationNoticeTimer.current = null;
    }, 2800);
  };

  const showDestinationError = (message: string) => {
    if (destinationErrorTimer.current) {
      window.clearTimeout(destinationErrorTimer.current);
    }

    setDestinationError(message);
    destinationErrorTimer.current = window.setTimeout(() => {
      setDestinationError(null);
      destinationErrorTimer.current = null;
    }, 3200);
  };

  const showActivityError = (message: string) => {
    if (activityErrorTimer.current) {
      window.clearTimeout(activityErrorTimer.current);
    }

    setActivityError(message);
    activityErrorTimer.current = window.setTimeout(() => {
      setActivityError(null);
      activityErrorTimer.current = null;
    }, 3200);
  };

  useEffect(
    () => () => {
      if (destinationNoticeTimer.current) {
        window.clearTimeout(destinationNoticeTimer.current);
      }
      if (destinationErrorTimer.current) {
        window.clearTimeout(destinationErrorTimer.current);
      }
      if (activityErrorTimer.current) {
        window.clearTimeout(activityErrorTimer.current);
      }
    },
    [],
  );

  const destinationSchedules = useMemo<DestinationSchedule[]>(() => {
    const schedules: DestinationSchedule[] = [];
    let dayCursor = 1;
    let currentDate = parseDateOnly(travel.startDate);

    for (const destination of destinations) {
      const startDay = dayCursor;
      const endDay = dayCursor + destination.daysSpent - 1;
      const startDate = new Date(currentDate);
      const endDate = addDaysToDateOnly(currentDate, destination.daysSpent - 1);

      schedules.push({
        ...destination,
        startDay,
        endDay,
        startDate: formatDateOnlyForApi(startDate),
        endDate: formatDateOnlyForApi(endDate),
      });

      dayCursor += destination.daysSpent;
      currentDate = addDaysToDateOnly(endDate, 1);
    }

    return schedules;
  }, [destinations, travel.startDate, travel.endDate]);

  const destinationDayMap = useMemo(() => {
    const map = new Map<number, DestinationDto>();
    for (const schedule of destinationSchedules) {
      for (
        let dayNumber = schedule.startDay;
        dayNumber <= schedule.endDay;
        dayNumber += 1
      ) {
        map.set(dayNumber, schedule);
      }
    }

    return map;
  }, [destinationSchedules]);

  const loadDestinations = async () => {
    setIsLoadingDestinations(true);
    clearDestinationError();

    try {
      const data = await destinationApi.getDestinationsByTravelId(travel.id);
      setDestinations(data);
    } catch (error) {
      // Debugging
      console.error("Failed to load destinations", error);
      setDestinationError("Destinations could not be loaded right now.");
    } finally {
      setIsLoadingDestinations(false);
    }
  };

  useEffect(() => {
    setDestinations([]);
    setActivitiesByDestination({});
    setDestinationBeingEdited(null);
    setDestinationPendingDelete(null);
    setActivityDestination(null);
    setActivityBeingEdited(null);
    setActivityPendingDelete(null);

    void loadDestinations();
  }, [travel.id]);

  const loadActivities = async () => {
    setIsLoadingActivities(true);
    if (activityErrorTimer.current) {
      window.clearTimeout(activityErrorTimer.current);
      activityErrorTimer.current = null;
    }
    setActivityError(null);

    try {
      const entries = await Promise.all(
        destinations.map(async (destination) => {
          const data = await activityApi.getActivitiesByDestinationId(
            travel.id,
            destination.id,
          );

          return [destination.id, data] as const;
        }),
      );

      setActivitiesByDestination(Object.fromEntries(entries));
    } catch (error) {
      // Debugging
      console.error("Failed to load activities", error);
      showActivityError("Activities could not be loaded right now.");
    } finally {
      setIsLoadingActivities(false);
    }
  };

  useEffect(() => {
    if (destinations.length === 0) {
      setActivitiesByDestination({});
      return;
    }

    void loadActivities();
  }, [travel.id, destinations]);

  const handleAddDestination = async (values: CreateDestinationDto) => {
    try {
      setDestinationError(null);
      setDestinationNotice(null);

      if (destinationBeingEdited) {
        await destinationApi.updateDestination(
          travel.id,
          destinationBeingEdited.id,
          values,
        );
      } else {
        await destinationApi.addDestination(travel.id, values);
      }

      setShowAddDestination(false);
      setDestinationBeingEdited(null);
      showDestinationNotice(
        destinationBeingEdited
          ? "Destination updated successfully."
          : "Destination added successfully.",
      );
      await loadDestinations();
      if (onRefresh) {
        void onRefresh();
      }
      setActiveTab("destinations");
    } catch (error) {
      // Debugging
      console.error("Failed to add destination", error);
      showDestinationError(
        getApiErrorMessage(error) ||
          "Destination could not be saved. Please try again.",
      );
    }
  };

  const handleAddActivity = async (values: CreateActivityDto) => {
    if (!activityDestination) {
      return;
    }

    try {
      if (activityBeingEdited) {
        await activityApi.updateActivity(
          travel.id,
          activityDestination.id,
          activityBeingEdited.activity.id,
          values,
        );
      } else {
        await activityApi.addActivity(
          travel.id,
          activityDestination.id,
          values,
        );
      }

      setActivityDestination(null);
      setActivityBeingEdited(null);
      showDestinationNotice(
        activityBeingEdited
          ? "Activity updated successfully."
          : "Activity added successfully.",
      );
      await loadDestinations();
      await loadActivities();
      if (onRefresh) {
        void onRefresh();
      }
      setActiveTab("activities");
    } catch (error) {
      // Debugging
      console.error("Failed to add activity", error);
      showActivityError(
        getApiErrorMessage(error) ||
          "Activity could not be saved. Please try again.",
      );
    }
  };

  const allActivities = useMemo(
    () =>
      destinations.flatMap((destination) => {
        const destinationSchedule = destinationSchedules.find(
          (schedule) => schedule.id === destination.id,
        ) ?? {
          ...destination,
          startDay: 1,
          endDay: destination.daysSpent,
          startDate: travel.startDate,
          endDate: travel.endDate,
        };

        const destinationActivities =
          activitiesByDestination[destination.id] ?? [];

        return destinationActivities.map((activity) => ({
          activity,
          destination: destinationSchedule,
        }));
      }),
    [
      activitiesByDestination,
      destinations,
      destinationSchedules,
      travel.startDate,
      travel.endDate,
    ],
  );

  const orderedDestinations = useMemo(
    () => [...destinations].reverse(),
    [destinations],
  );

  const openDestinationCreateModal = () => {
    setDestinationBeingEdited(null);
    setShowAddDestination(true);
  };

  const openDestinationEditModal = (destination: DestinationDto) => {
    setDestinationBeingEdited(destination);
    setShowAddDestination(true);
  };

  const openDestinationDeleteConfirm = (destination: DestinationDto) => {
    setDestinationPendingDelete(destination);
  };

  const confirmDeleteDestination = async () => {
    if (!destinationPendingDelete) {
      return;
    }

    try {
      await destinationApi.deleteDestination(
        travel.id,
        destinationPendingDelete.id,
      );
      setDestinationPendingDelete(null);
      if (activityDestination?.id === destinationPendingDelete.id) {
        setActivityDestination(null);
      }
      showDestinationNotice("Destination deleted successfully.");
      await loadDestinations();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (error) {
      // Debugging
      console.error("Failed to delete destination", error);
      showDestinationError(
        getApiErrorMessage(error) ||
          "Destination could not be deleted. Please try again.",
      );
    }
  };

  const openActivityCreateModal = (destination: DestinationSchedule) => {
    setActivityBeingEdited(null);
    setActivityDestination(destination);
  };

  const openActivityEditModal = (
    activity: ActivityDto,
    destination: DestinationSchedule,
  ) => {
    setActivityBeingEdited({ activity, destination });
    setActivityDestination(destination);
  };

  const openActivityDeleteConfirm = (
    activity: ActivityDto,
    destination: DestinationSchedule,
  ) => {
    setActivityPendingDelete({ activity, destination });
  };

  const confirmDeleteActivity = async () => {
    if (!activityPendingDelete) {
      return;
    }

    try {
      await activityApi.deleteActivity(
        travel.id,
        activityPendingDelete.destination.id,
        activityPendingDelete.activity.id,
      );
      setActivityPendingDelete(null);
      showDestinationNotice("Activity deleted successfully.");
      await loadDestinations();
      await loadActivities();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (error) {
      // Debugging
      console.error("Failed to delete activity", error);
      showActivityError(
        getApiErrorMessage(error) ||
          "Activity could not be deleted. Please try again.",
      );
    }
  };

  const selectedActivity = activityBeingEdited?.activity ?? null;
  const selectedActivityDestination =
    activityBeingEdited?.destination ?? activityDestination;

  return (
    <div className="travel-workspace">
      <div className="travel-workspace__header">
        <div>
          <p className="travel-workspace__eyebrow">Selected travel</p>
          <h3 className="travel-workspace__title">{travel.name}</h3>
          <p className="travel-workspace__description">{travel.description}</p>
        </div>

        <div className="workspace-tabs">
          <button
            className={`workspace-tabs__item ${activeTab === "details" ? "workspace-tabs__item--active" : ""}`}
            onClick={() => setActiveTab("details")}
          >
            Details
          </button>
          <button
            className={`workspace-tabs__item ${activeTab === "destinations" ? "workspace-tabs__item--active" : ""}`}
            onClick={() => setActiveTab("destinations")}
          >
            Destinations
          </button>
          <button
            className={`workspace-tabs__item ${activeTab === "activities" ? "workspace-tabs__item--active" : ""}`}
            onClick={() => setActiveTab("activities")}
          >
            Activities
          </button>
          <button
            className={`workspace-tabs__item ${activeTab === "map" ? "workspace-tabs__item--active" : ""}`}
            onClick={() => setActiveTab("map")}
          >
            Map
          </button>
          <button
            className={`workspace-tabs__item ${activeTab === "checklist" ? "workspace-tabs__item--active" : ""}`}
            onClick={() => setActiveTab("checklist")}
          >
            Checklist
          </button>
        </div>
      </div>

      <div className="travel-workspace__content">
        {activeTab === "details" && (
          <section className="app-card travel-workspace__overview">
            <div className="travel-workspace__meta-grid">
              <article className="travel-workspace__meta-card">
                <span>Date range</span>
                <strong>
                  {formatDateRange(travel.startDate, travel.endDate)}
                </strong>
              </article>
              <article className="travel-workspace__meta-card">
                <span>Duration</span>
                <strong>{totalDays} days</strong>
              </article>
              <article className="travel-workspace__meta-card">
                <span>Planned budget</span>
                <strong>
                  {travel.budget.toLocaleString("en-GB", {
                    style: "currency",
                    currency: "EUR",
                  })}
                </strong>
              </article>
              <article className="travel-workspace__meta-card">
                <span>Destinations</span>
                <strong>{travel.destinationCount}</strong>
              </article>
            </div>

            {travel.notes ? (
              <div className="travel-workspace__notes">
                <span>Notes</span>
                <p>{travel.notes}</p>
              </div>
            ) : null}

            <div
              style={{ marginTop: 12 }}
              className="travel-workspace__actions"
            >
              <button type="button" className="button button--primary">
                Export PDF
              </button>
              <button type="button" className="button button--secondary">
                Generate QR
              </button>
            </div>

            <div style={{ marginTop: 12 }}>
              <div
                className="travel-day-strip"
                aria-label="Travel days overview"
              >
                {Array.from({ length: totalDays }, (_, i) => i + 1).map(
                  (dayNumber) => {
                    const d = addDaysToDateOnly(travel.startDate, dayNumber - 1);
                    const dateLabel = formatDateOnly(d);
                    const destinationOnDay = destinationDayMap.get(dayNumber);
                    const tooltip = destinationOnDay
                      ? `${dateLabel} • ${destinationOnDay.city}, ${destinationOnDay.country}`
                      : dateLabel;

                    return (
                      <div
                        key={dayNumber}
                        className={`travel-day-strip__day ${destinationOnDay ? "travel-day-strip__day--active" : ""}`}
                      >
                        <span className="travel-day-strip__label">Day</span>
                        <strong>{dayNumber}</strong>
                        <div className="travel-day-tooltip" role="tooltip">
                          {tooltip}
                        </div>
                      </div>
                    );
                  },
                )}
              </div>
            </div>
          </section>
        )}

        {activeTab === "destinations" && (
          <section className="app-card travel-workspace__section">
            <div className="section-heading section-heading--inline">
              <button
                type="button"
                className="button button--primary button--sm"
                onClick={openDestinationCreateModal}
              >
                + Add destination
              </button>
            </div>

            {destinationNotice ? (
              <div className="message message--success">
                {destinationNotice}
              </div>
            ) : null}

            {destinationError ? (
              <div className="message message--error">{destinationError}</div>
            ) : null}

            <div className="travel-workspace__list">
              {isLoadingDestinations ? (
                <div className="empty-state">
                  <h5>Loading destinations</h5>
                  <p>Fetching saved stops for this travel plan.</p>
                </div>
              ) : orderedDestinations.length > 0 ? (
                orderedDestinations.map((destination) => (
                  <DestinationCard
                    key={destination.id}
                    destination={destination}
                    schedule={destinationSchedules.find(
                      (schedule) => schedule.id === destination.id,
                    )}
                    onAddActivity={() => {
                      const destinationSchedule = destinationSchedules.find(
                        (schedule) => schedule.id === destination.id,
                      ) ?? {
                        ...destination,
                        startDay: 1,
                        endDay: destination.daysSpent,
                        startDate: travel.startDate,
                        endDate: travel.endDate,
                      };

                      openActivityCreateModal(destinationSchedule);
                    }}
                    onEdit={openDestinationEditModal}
                    onDelete={openDestinationDeleteConfirm}
                  />
                ))
              ) : (
                <div className="empty-state empty-state--soft">
                  <h5>No destinations added yet</h5>
                  <p>
                    Add the first stop to start building the route and daily
                    activities.
                  </p>
                </div>
              )}
            </div>
          </section>
        )}

        {activeTab === "activities" && (
          <section className="app-card travel-workspace__section">
            <div className="section-heading section-heading--inline">
              <div></div>
            </div>

            {activityError ? (
              <div className="message message--error">{activityError}</div>
            ) : null}

            <div className="travel-workspace__list">
              {isLoadingActivities ? (
                <div className="empty-state">
                  <h5>Loading activities</h5>
                  <p>Fetching saved activities for this trip.</p>
                </div>
              ) : allActivities.length > 0 ? (
                allActivities.map(({ destination, activity }) => (
                  <ActivityCard
                    key={activity.id}
                    activity={activity}
                    destination={destination}
                    onEdit={openActivityEditModal}
                    onDelete={openActivityDeleteConfirm}
                  />
                ))
              ) : (
                <div className="empty-state empty-state--soft">
                  <h5>No activities yet</h5>
                  <p>
                    Create activities from a destination card to see them here.
                  </p>
                </div>
              )}
            </div>
          </section>
        )}

        {activeTab === "map" && (
          <section className="app-card travel-workspace__section travel-workspace__section--map">
            <div className="section-heading"></div>
            <div className="travel-workspace__map-frame" />
          </section>
        )}

        {activeTab === "checklist" && (
          <section className="app-card travel-workspace__section">
            <div className="travel-workspace__planner-frame">
              <div className="empty-state">
                <h5>No checklist items yet</h5>
                <p>Add essentials, documents, and reminders when ready.</p>
              </div>
            </div>
          </section>
        )}
      </div>
      {showAddDestination && (
        <AddDestinationModal
          open={showAddDestination}
          onClose={() => {
            setShowAddDestination(false);
            setDestinationBeingEdited(null);
          }}
          onSave={handleAddDestination}
          title={
            destinationBeingEdited ? "Edit destination" : "Add destination"
          }
          description={
            destinationBeingEdited
              ? "Update the destination details and save changes."
              : "Add a stop for this trip."
          }
          submitLabel={
            destinationBeingEdited ? "Save changes" : "Add destination"
          }
          initialValues={destinationBeingEdited}
        />
      )}

      {selectedActivityDestination && (
        <AddActivityModal
          open={Boolean(selectedActivityDestination)}
          destination={selectedActivityDestination}
          travelStartDate={selectedActivityDestination.startDate}
          onClose={() => {
            setActivityDestination(null);
            setActivityBeingEdited(null);
          }}
          onSave={handleAddActivity}
          title={
            activityBeingEdited
              ? `Edit activity for ${selectedActivityDestination.name}`
              : `Add activity for ${selectedActivityDestination.name}`
          }
          modalDescription={
            activityBeingEdited
              ? "Update the activity details and save changes."
              : "Choose a day for this destination, then enter the activity details."
          }
          submitLabel={activityBeingEdited ? "Save changes" : "Add activity"}
          initialValues={selectedActivity}
        />
      )}

      {destinationPendingDelete ? (
        <Modal
          title="Delete destination"
          description={`Are you sure you want to delete ${destinationPendingDelete.name}? This will also remove related activities.`}
          onClose={() => setDestinationPendingDelete(null)}
        >
          <div className="stack">
            <p>This action cannot be undone.</p>
            <div className="form-grid form-grid--two">
              <button
                type="button"
                className="button button--secondary"
                onClick={() => setDestinationPendingDelete(null)}
              >
                Cancel
              </button>
              <button
                type="button"
                className="button button--primary"
                onClick={confirmDeleteDestination}
              >
                Delete destination
              </button>
            </div>
          </div>
        </Modal>
      ) : null}

      {activityPendingDelete ? (
        <Modal
          title="Delete activity"
          description={`Are you sure you want to delete ${activityPendingDelete.activity.name}?`}
          onClose={() => setActivityPendingDelete(null)}
        >
          <div className="stack">
            <p>This action cannot be undone.</p>
            <div className="form-grid form-grid--two">
              <button
                type="button"
                className="button button--secondary"
                onClick={() => setActivityPendingDelete(null)}
              >
                Cancel
              </button>
              <button
                type="button"
                className="button button--primary"
                onClick={confirmDeleteActivity}
              >
                Delete activity
              </button>
            </div>
          </div>
        </Modal>
      ) : null}
    </div>
  );
};
