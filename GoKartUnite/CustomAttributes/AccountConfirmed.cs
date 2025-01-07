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
            var dbContext = context.HttpContext
                .RequestServices
                .GetService(typeof(GoKartUniteContext)) as GoKartUniteContext;

            var NameIdentifier = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (NameIdentifier == null || NameIdentifier.Value == String.Empty)
            {
                context.Result = new RedirectResult("/login");
                return;
            }

            var userExists = dbContext.Karter
                .Any(u => u.NameIdentifier == NameIdentifier.Value);

            if (!userExists)
            {
                context.Result = new RedirectResult("/karterHome/Create");
            }
        }
    }
}
