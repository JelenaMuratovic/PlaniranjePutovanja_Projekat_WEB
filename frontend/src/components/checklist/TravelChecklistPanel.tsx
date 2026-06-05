import { useEffect, useRef, useState, type FormEvent } from "react";
import { Modal } from "../shared/Modal";
import { checklistApi } from "../../api/travel/checklistApi";
import { getApiErrorMessage } from "../../helpers/apiError";
import { formatDateOnly } from "../../helpers/date";
import type { ChecklistDto } from "../../models/travel/checklist/dtos";

type TravelChecklistPanelProps = {
  travelId: string;
  onRefresh?: () => Promise<void> | void;
  isReadOnly?: boolean;
};

export const TravelChecklistPanel = ({
  travelId,
  onRefresh,
  isReadOnly = false,
}: TravelChecklistPanelProps) => {
  const [items, setItems] = useState<ChecklistDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [itemText, setItemText] = useState("");
  const [itemPendingDelete, setItemPendingDelete] =
    useState<ChecklistDto | null>(null);
  const [togglingItemId, setTogglingItemId] = useState<string | null>(null);

  const noticeTimer = useRef<number | null>(null);
  const errorTimer = useRef<number | null>(null);

  const clearNotice = () => {
    if (noticeTimer.current) {
      window.clearTimeout(noticeTimer.current);
      noticeTimer.current = null;
    }
    setNotice(null);
  };

  const clearError = () => {
    if (errorTimer.current) {
      window.clearTimeout(errorTimer.current);
      errorTimer.current = null;
    }
    setError(null);
  };

  const showNotice = (message: string) => {
    if (noticeTimer.current) {
      window.clearTimeout(noticeTimer.current);
    }

    setNotice(message);
    noticeTimer.current = window.setTimeout(() => {
      setNotice(null);
      noticeTimer.current = null;
    }, 2600);
  };

  const showError = (message: string) => {
    if (errorTimer.current) {
      window.clearTimeout(errorTimer.current);
    }

    setError(message);
    errorTimer.current = window.setTimeout(() => {
      setError(null);
      errorTimer.current = null;
    }, 3200);
  };

  useEffect(
    () => () => {
      if (noticeTimer.current) {
        window.clearTimeout(noticeTimer.current);
      }
      if (errorTimer.current) {
        window.clearTimeout(errorTimer.current);
      }
    },
    [],
  );

  const loadChecklists = async () => {
    setIsLoading(true);
    clearError();

    try {
      const data = await checklistApi.getChecklistsByTravelId(travelId);
      setItems(data);
    } catch (listError) {
      console.error("Failed to load checklists", listError);
      showError("Checklist could not be loaded right now.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    setItems([]);
    setItemText("");
    setItemPendingDelete(null);
    setTogglingItemId(null);
    clearNotice();
    clearError();

    void loadChecklists();
  }, [travelId]);

  const handleAddChecklist = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const trimmedItem = itemText.trim();
    if (!trimmedItem) {
      return;
    }

    try {
      setIsSubmitting(true);
      await checklistApi.addChecklist(travelId, { item: trimmedItem });
      setItemText("");
      showNotice("Item added to checklist.");
      await loadChecklists();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (addError) {
      console.error("Failed to add checklist item", addError);
      showError(
        getApiErrorMessage(addError) ||
          "Checklist item could not be saved. Please try again.",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const toggleChecklist = async (item: ChecklistDto) => {
    try {
      setTogglingItemId(item.id);
      await checklistApi.toggleChecklist(travelId, item.id, !item.isCompleted);
      showNotice(
        item.isCompleted
          ? "Item moved back to pending."
          : "Item marked as completed.",
      );
      await loadChecklists();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (toggleError) {
      console.error("Failed to toggle checklist item", toggleError);
      showError(
        getApiErrorMessage(toggleError) ||
          "Checklist item could not be updated. Please try again.",
      );
    } finally {
      setTogglingItemId(null);
    }
  };

  const confirmDeleteChecklist = async () => {
    if (!itemPendingDelete) {
      return;
    }

    try {
      await checklistApi.deleteChecklist(travelId, itemPendingDelete.id);
      setItemPendingDelete(null);
      showNotice("Checklist item deleted successfully.");
      await loadChecklists();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (deleteError) {
      console.error("Failed to delete checklist item", deleteError);
      showError(
        getApiErrorMessage(deleteError) ||
          "Checklist item could not be deleted. Please try again.",
      );
    }
  };

  return (
    <section className="checklist-panel">
      <div className="section-heading section-heading--inline checklist-panel__header">
        <div className="checklist-panel__counter">
          <strong>{items.filter((item) => item.isCompleted).length}</strong>
          <span>completed</span>
        </div>
      </div>

      {!isReadOnly && (
        <form className="checklist-panel__form" onSubmit={handleAddChecklist}>
          <div className="field">
            <input
              id="checklist-item"
              value={itemText}
              onChange={(event) => setItemText(event.target.value)}
              placeholder="Passport, charger, insurance, clothes..."
            />
          </div>

          <button
            type="submit"
            className="button button--primary button--sm"
            disabled={isSubmitting || itemText.trim().length === 0}
          >
            {isSubmitting ? "Adding..." : "Add item"}
          </button>
        </form>
      )}

      {notice ? <div className="message message--success">{notice}</div> : null}

      {error ? <div className="message message--error">{error}</div> : null}

      <div className="checklist-panel__list">
        {isLoading ? (
          <div className="empty-state">
            <h5>Loading checklist</h5>
            <p>Fetching items for this travel plan.</p>
          </div>
        ) : items.length > 0 ? (
          items.map((item) => (
            <article
              key={item.id}
              className={`checklist-item ${item.isCompleted ? "checklist-item--completed" : ""}`}
            >
              <button
                type="button"
                className={`checklist-item__check ${item.isCompleted ? "checklist-item__check--active" : ""}`}
                aria-pressed={item.isCompleted}
                aria-label={
                  item.isCompleted
                    ? `Mark ${item.item} as incomplete`
                    : `Mark ${item.item} as completed`
                }
                disabled={isReadOnly || togglingItemId === item.id}
                onClick={() => toggleChecklist(item)}
              >
                <span aria-hidden="true">
                  {togglingItemId === item.id
                    ? "…"
                    : item.isCompleted
                      ? "✓"
                      : ""}
                </span>
              </button>

              <button
                type="button"
                className="checklist-item__content"
                onClick={() => toggleChecklist(item)}
                aria-label={
                  item.isCompleted
                    ? `Mark ${item.item} as incomplete`
                    : `Mark ${item.item} as completed`
                }
                disabled={isReadOnly || togglingItemId === item.id}
              >
                <h5>{item.item}</h5>
                <span className="checklist-item__status">
                  {item.isCompleted ? "Completed" : "Pending"}
                </span>
                <p>
                  {formatDateOnly(item.createdAt, {
                    day: "2-digit",
                    month: "short",
                    year: "numeric",
                  })}
                </p>
              </button>

              {!isReadOnly && (
                <button
                  type="button"
                  className="button button--secondary button--sm checklist-item__delete"
                  onClick={() => setItemPendingDelete(item)}
                  disabled={togglingItemId === item.id}
                >
                  Delete
                </button>
              )}
            </article>
          ))
        ) : (
          <div className="empty-state empty-state--soft">
            <h5>No checklist items yet</h5>
            <p>Add essentials, documents, and reminders before the trip.</p>
          </div>
        )}
      </div>

      {itemPendingDelete ? (
        <Modal
          title="Delete checklist item"
          description={`Are you sure you want to delete ${itemPendingDelete.item}?`}
          onClose={() => setItemPendingDelete(null)}
        >
          <div className="stack">
            <p>This action cannot be undone.</p>
            <div className="form-grid form-grid--two">
              <button
                type="button"
                className="button button--secondary"
                onClick={() => setItemPendingDelete(null)}
              >
                Cancel
              </button>
              <button
                type="button"
                className="button button--primary"
                onClick={confirmDeleteChecklist}
              >
                Delete item
              </button>
            </div>
          </div>
        </Modal>
      ) : null}
    </section>
  );
};

export default TravelChecklistPanel;
