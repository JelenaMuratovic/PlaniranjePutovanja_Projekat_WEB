using Microsoft.AspNetCore.Authorization;

namespace PlaniranjePutovanja.APIGateway.Authorization
{
    public enum AccessRequirement
    {
        View,
        Edit
    }

    public sealed class TravelAccessRequirement : IAuthorizationRequirement
    {
        public AccessRequirement RequiredAccess { get; }

        public TravelAccessRequirement(AccessRequirement requiredAccess)
        {
            RequiredAccess = requiredAccess;
        }
    }
}
