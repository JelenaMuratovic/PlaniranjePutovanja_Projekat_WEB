import { useState, useMemo, useEffect } from "react";
import {
  MapContainer,
  TileLayer,
  Marker,
  Popup,
  Polyline,
  useMap,
} from "react-leaflet";
import L from "leaflet";
import type { DestinationDto } from "../../models/travel/destination/dtos";
import type { ActivityDto } from "../../models/travel/activity/dtos";
import "../../styles/mapStyle.css";

// Sredjivanje podrazumevane Leaflet ikonice da se ne zabaguje
const DefaultIcon = L.icon({
  iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
});
L.Marker.prototype.options.icon = DefaultIcon;

// Komponenta koja menja fokus (centar) mape kada korisnik klikne sa strane
const ChangeMapFocus = ({ center }: { center: [number, number] }) => {
  const map = useMap();
  useEffect(() => {
    map.setView(center, map.getZoom());
  }, [center, map]);
  return null;
};

type TravelMapPanelProps = {
  destinations: DestinationDto[];
  activitiesByDestination: Record<string, ActivityDto[]>;
};

export const TravelMapPanel = ({
  destinations,
  activitiesByDestination,
}: TravelMapPanelProps) => {
  // Filtriramo samo destinacije koje imaju unete koordinate
  const validDestinations = useMemo(() => {
    return destinations.filter(
      (d) => d.latitude !== null && d.longitude !== null,
    ) as Array<DestinationDto & { latitude: number; longitude: number }>;
  }, [destinations]);

  // Stanje za selektovanu destinaciju (podrazumevano prva vazeca ako postoji)
  const [selectedDestination, setSelectedDestination] =
    useState<DestinationDto | null>(validDestinations[0] || null);

  // Stanje za aktivni marker (da bismo znali sta da naglasimo na mapi ili u listi)
  const [activeActivityId, setActiveActivityId] = useState<string | null>(null);

  // Ako se spisak destinacija promeni, azuriraj selektovanu
  useEffect(() => {
    if (validDestinations.length > 0 && !selectedDestination) {
      setSelectedDestination(validDestinations[0]);
    }
  }, [validDestinations, selectedDestination]);

  // Generisanje koordinata za liniju rute (Polyline)
  const routePositions = useMemo(() => {
    return validDestinations.map(
      (d) => [d.latitude, d.longitude] as [number, number],
    );
  }, [validDestinations]);

  // Izvlacenje i sortiranje aktivnosti za selektovanu destinaciju
  const sortedActivities = useMemo(() => {
    if (!selectedDestination) return [];
    const activities = activitiesByDestination[selectedDestination.id] || [];

    // Sortiramo aktivnosti po pocetnom vremenu (hronoloski)
    return [...activities].sort((a, b) => {
      const timeA = a.startTime || "00:00";
      const timeB = b.startTime || "00:00";
      return timeA.localeCompare(timeB);
    });
  }, [selectedDestination, activitiesByDestination]);

  // Odredjivanje podrazumevanog centra mape (centar prve destinacije ili centar Balkana/Evrope)
  const defaultCenter: [number, number] =
    validDestinations.length > 0
      ? [validDestinations[0].latitude, validDestinations[0].longitude]
      : [44.7866, 20.4489]; // Beograd kao fallback

  if (validDestinations.length === 0) {
    return (
      <div className="empty-state empty-state--soft">
        <h5>No coordinates available</h5>
        <p>
          Please edit your destinations and enter Latitude and Longitude to
          generate the travel route map.
        </p>
      </div>
    );
  }

  return (
    <div className="travel-map-container">
      {/* LEVA STRANA: Leaflet Interaktivna Mapa */}
      <div className="travel-map__leaflet-wrapper">
        <MapContainer
          center={defaultCenter}
          zoom={6}
          className="travel-map__leaflet"
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />

          {/* Crtanje rute izmedju destinacija */}
          {routePositions.length > 1 && (
            <Polyline
              positions={routePositions}
              color="var(--primary)"
              weight={4}
              dashArray="8, 8"
              smoothFactor={1}
            />
          )}

          {/* Markeri za destinacije */}
          {validDestinations.map((destination, index) => (
            <Marker
              key={destination.id}
              position={[destination.latitude, destination.longitude]}
              eventHandlers={{
                click: () => {
                  setSelectedDestination(destination);
                },
              }}
            >
              <Popup>
                <div style={{ padding: "2px" }}>
                  <strong style={{ fontSize: "1rem", color: "var(--heading)" }}>
                    {index + 1}. {destination.city}
                  </strong>
                  <p
                    style={{
                      margin: "4px 0 0 0",
                      fontSize: "0.85rem",
                      color: "var(--text-soft)",
                    }}
                  >
                    {destination.name} ({destination.daysSpent} days)
                  </p>
                </div>
              </Popup>
            </Marker>
          ))}

          {/* Okidac za promenu centra mape */}
          {selectedDestination && (
            <ChangeMapFocus
              center={[
                selectedDestination.latitude!,
                selectedDestination.longitude!,
              ]}
            />
          )}
        </MapContainer>
      </div>

      {/* DESNA STRANA: Prelepi Timeline panel sa aktivnostima */}
      <div className="travel-map__timeline-panel">
        <div className="travel-map__timeline-header">
          <h3 className="travel-map__timeline-title">
            {selectedDestination
              ? `${selectedDestination.city} Itinerary`
              : "Select a Destination"}
          </h3>
          <p className="travel-map__timeline-subtitle">
            {selectedDestination?.country} &bull;{" "}
            {selectedDestination?.daysSpent} days scheduled
          </p>
        </div>

        <div className="travel-map__timeline-scroll">
          {sortedActivities.length > 0 ? (
            <div className="travel-timeline">
              {sortedActivities.map((activity) => (
                <div
                  key={activity.id}
                  className={`travel-timeline__item ${
                    activeActivityId === activity.id
                      ? "travel-timeline__item--active"
                      : ""
                  }`}
                >
                  <div className="travel-timeline__badge" />
                  <div
                    className={`travel-timeline__card ${
                      activeActivityId === activity.id
                        ? "travel-timeline__card--active"
                        : ""
                    }`}
                    onClick={() => setActiveActivityId(activity.id)}
                  >
                    <span className="travel-timeline__time">
                      🕒{" "}
                      {activity.startTime
                        ? activity.startTime.slice(0, 5)
                        : "Not set"}
                    </span>
                    <h4 className="travel-timeline__name">{activity.name}</h4>
                    {activity.description && (
                      <p className="travel-timeline__desc">
                        {activity.description}
                      </p>
                    )}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="travel-map__empty-timeline">
              <h5>No activities found</h5>
              <p>
                There are no activities added for {selectedDestination?.city}{" "}
                yet.
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TravelMapPanel;
