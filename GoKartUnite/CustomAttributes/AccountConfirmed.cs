using GoKartUnite.Data;
using GoKartUnite.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Security.Claims;

namespace GoKartUnite.CustomAttributes
{
    public class AccountConfirmed : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Retrieve the database context
            var dbContext = context.HttpContext
                .RequestServices
                .GetService(typeof(GoKartUniteContext)) as GoKartUniteContext;

            var userEmailClaim = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (userEmailClaim == null || userEmailClaim.Value == String.Empty)
            {
                // Fail if the user ID claim is missing or invalid
                context.Result = new RedirectResult("/login");
                return;
            }

            // Check if the user exists in the database and is confirmed
            var userExists = dbContext.Karter
                .Any(u => u.Email == userEmailClaim.Value);

            if (!userExists)
            {
                // Deny access if the user doesn't exist or the account is not confirmed
                context.Result = new RedirectResult("/karterHome/Create");
            }
        }
    }
}
