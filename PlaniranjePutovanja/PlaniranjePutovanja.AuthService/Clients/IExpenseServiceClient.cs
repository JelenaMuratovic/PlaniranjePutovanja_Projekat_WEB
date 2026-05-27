using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Clients
{
    public interface IExpenseServiceClient
    {
        Task<bool> DeleteExpensesByTravelIdAsync(string travelId);
    }
}
