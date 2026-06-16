import { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAuth } from "../hooks/useAuth";
import { travelApi } from "../api/travel/travelApi";
import { getApiErrorMessage } from "../helpers/apiError";
import { AddTravelForm } from "../components/travel/AddTravelForm";
import { TravelGrid } from "../components/travel/TravelGrid";
import { Modal } from "../components/shared/Modal";
import { TravelWorkspacePanel } from "../components/dashboard/TravelWorkspacePanel";
import type { TravelDto } from "../models/travel/travel/dtos";
import {
  createTravelSchema,
  type CreateTravelFormValues,
} from "../helpers/validation/travel";

export const DashboardPage = () => {
  const { user } = useAuth();
  const [serverMessage, setServerMessage] = useState<string | null>(null);
  const [travels, setTravels] = useState<TravelDto[]>([]);
  const [selectedTravel, setSelectedTravel] = useState<TravelDto | null>(null);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [travelBeingEdited, setTravelBeingEdited] = useState<TravelDto | null>(
    null,
  );
  const [travelPendingDelete, setTravelPendingDelete] =
    useState<TravelDto | null>(null);
  const serverMessageTimer = useRef<number | null>(null);

  const showServerMessage = (message: string) => {
    if (serverMessageTimer.current) {
      window.clearTimeout(serverMessageTimer.current);
    }

    setServerMessage(message);
    serverMessageTimer.current = window.setTimeout(() => {
      setServerMessage(null);
      serverMessageTimer.current = null;
    }, 3000);
  };

  useEffect(
    () => () => {
      if (serverMessageTimer.current) {
        window.clearTimeout(serverMessageTimer.current);
      }
    },
    [],
  );

  const formDefaultValues = {
    name: "",
    description: "",
    startDate: "",
    endDate: "",
    budget: 0,
    notes: "",
  };

  const mapTravelToFormValues = (
    travel: TravelDto,
  ): CreateTravelFormValues => ({
    name: travel.name,
    description: travel.description,
    startDate: travel.startDate.split("T")[0] ?? travel.startDate,
    endDate: travel.endDate.split("T")[0] ?? travel.endDate,
    budget: travel.budget,
    notes: travel.notes ?? "",
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateTravelFormValues>({
    resolver: zodResolver(createTravelSchema),
    defaultValues: {
      name: "",
      description: "",
      startDate: "",
      endDate: "",
      budget: 0,
      notes: "",
    },
  });

  const loadTravels = async () => {
    if (!user) {
      return;
    }
    try {
      const data =
        user.role === "Admin"
          ? await travelApi.getAllTravels()
          : await travelApi.getTravelsByUserId(user.id);

      setTravels(data ?? []);
      setSelectedTravel((currentTravel) => {
        if (!currentTravel) {
          return data[0] ?? null;
        }

        return (
          data.find((travel) => travel.id === currentTravel.id) ??
          data[0] ??
          null
        );
      });
    } catch (error) {
      console.error("Failed to load travels", error);
      setServerMessage(getApiErrorMessage(error));
    }
  };

  useEffect(() => {
    void loadTravels();
  }, [user]);

  const onSubmit = async (values: CreateTravelFormValues) => {
    try {
      setServerMessage(null);
      const payload = {
        ...values,
        notes: values.notes ?? "",
      };

      if (travelBeingEdited) {
        await travelApi.updateTravel(travelBeingEdited.id, payload);
        showServerMessage("Travel plan updated successfully.");
      } else {
        await travelApi.createTravel(payload);
        showServerMessage("Travel plan created successfully.");
      }

      reset();
      setIsCreateModalOpen(false);
      setTravelBeingEdited(null);
      await loadTravels();
    } catch (error) {
      showServerMessage(getApiErrorMessage(error));
    }
  };

  const openCreateModal = () => {
    setTravelBeingEdited(null);
    setServerMessage(null);
    reset(formDefaultValues);
    setIsCreateModalOpen(true);
  };

  const openEditModal = (travel: TravelDto) => {
    setTravelBeingEdited(travel);
    setServerMessage(null);
    reset(mapTravelToFormValues(travel));
    setIsCreateModalOpen(true);
  };

  const openDeleteConfirmation = (travel: TravelDto) => {
    setTravelPendingDelete(travel);
  };

  const confirmDeleteTravel = async () => {
    if (!travelPendingDelete) {
      return;
    }

    try {
      setServerMessage(null);
      await travelApi.deleteTravel(travelPendingDelete.id);
      showServerMessage("Travel plan deleted successfully.");
      setTravelPendingDelete(null);
      if (selectedTravel?.id === travelPendingDelete.id) {
        setSelectedTravel(null);
      }
      await loadTravels();
    } catch (error) {
      showServerMessage(getApiErrorMessage(error));
    }
  };

  return (
    <section className="dashboard-shell dashboard-shell--clean">
      {serverMessage ? (
        <div className="app-card dashboard-error" role="alert">
          <strong></strong>&nbsp;{serverMessage}
        </div>
      ) : null}

      <div className="dashboard-layout dashboard-layout--split">
        {/* Leva kolona: lista putovanja */}
        <aside className="dashboard-panel dashboard-panel--list">
          <div className="dashboard-panel__header">
            <div>
              <h2>Dashboard</h2>
              <p className="muted">Plan and manage your trips</p>
            </div>
            <button
              type="button"
              className="button button--primary button--sm"
              onClick={openCreateModal}
            >
              + Create trip
            </button>
          </div>

          <TravelGrid
            travels={travels}
            onViewPlan={(travel) => {
              setSelectedTravel(travel);
            }}
            onEditPlan={openEditModal}
            onDeletePlan={openDeleteConfirmation}
          />
        </aside>

        {/* Desna kolona: Detaljni prikaz */}
        <main className="dashboard-panel dashboard-panel--detail">
          {selectedTravel ? (
            <TravelWorkspacePanel
              travel={selectedTravel}
              onRefresh={() => void loadTravels()}
            />
          ) : (
            <div className="empty-state empty-state--detail">
              <h4>No travel selected</h4>
              <p>
                Select a travel card on the left to open the detailed planning
                view.
              </p>
            </div>
          )}
        </main>
      </div>

      {isCreateModalOpen ? (
        <Modal
          title={travelBeingEdited ? "Edit travel plan" : "Create travel plan"}
          description={
            travelBeingEdited
              ? "Update the trip details and save the changes."
              : "Enter the basics for your new journey, then add destinations and activities later."
          }
          onClose={() => {
            setIsCreateModalOpen(false);
            setServerMessage(null);
            setTravelBeingEdited(null);
          }}
        >
          <AddTravelForm
            register={register}
            errors={errors}
            isSubmitting={isSubmitting}
            serverMessage={serverMessage}
            onSubmit={handleSubmit(onSubmit)}
            submitLabel={
              travelBeingEdited ? "Save changes" : "Create travel plan"
            }
          />
        </Modal>
      ) : null}

      {travelPendingDelete ? (
        <Modal
          title="Delete travel plan"
          description={`Are you sure you want to delete ${travelPendingDelete.name}? This action cannot be undone.`}
          onClose={() => setTravelPendingDelete(null)}
        >
          <div className="stack">
            <p>
              This will permanently remove the travel plan and its current
              dashboard view.
            </p>
            <div className="form-grid form-grid--two">
              <button
                type="button"
                className="button button--secondary"
                onClick={() => setTravelPendingDelete(null)}
              >
                Cancel
              </button>
              <button
                type="button"
                className="button button--primary"
                onClick={confirmDeleteTravel}
              >
                Delete travel plan
              </button>
            </div>
          </div>
        </Modal>
      ) : null}
    </section>
  );
};
