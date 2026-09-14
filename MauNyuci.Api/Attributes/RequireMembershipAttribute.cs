using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MauNyuci.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireMembershipAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _requiredTiers;

        public RequireMembershipAttribute(params string[] requiredTiers)
        {
            _requiredTiers = requiredTiers;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var membershipClaim = user.FindFirst("MembershipTier")?.Value;

            if (string.IsNullOrEmpty(membershipClaim) || !_requiredTiers.Contains(membershipClaim, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ObjectResult(new { message = "Fitur ini khusus untuk pengguna Membership Premium." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
