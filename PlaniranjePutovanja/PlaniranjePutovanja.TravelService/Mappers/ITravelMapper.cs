using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Mappers
{
    public interface ITravelMapper
    {
        Travel ToTravel(CreateTravelDto dto);

        TravelDto ToTravelDto(Travel travel);

        Destination ToDestination(CreateDestinationDto dto);

        DestinationDto ToDestinationDto(Destination destination);

        Activity ToActivity(CreateActivityDto dto);

        ActivityDto ToActivityDto(Activity activity);

        Checklist ToChecklist(CreateChecklistDto dto);

        ChecklistDto ToChecklistDto(Checklist checklist);
    }
}
