using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.Enums
{
    public enum ActivityStatus
    {
        Planned = 0,      // Planirano
        Reserved = 1,     // Rezervisano
        Completed = 2,    // Zavrseno
        Cancelled = 3     // Otkazano
    }
}
