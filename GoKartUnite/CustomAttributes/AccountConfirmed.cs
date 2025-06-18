using Azure.Core;
using GoKartUnite.Data;
using GoKartUnite.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                var tables = dbContext.Database.GetPendingMigrations(); // This might give a hint
                var connection = dbContext.Database.GetDbConnection();
                connection.Open(); // Just in case it closed for some reason
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Karter';";
                    var result = command.ExecuteScalar();
                    // result should not be null if the table exists
                    System.Diagnostics.Debug.WriteLine($"Karter table exists? {result != null}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking table: {ex.Message}");
            }
            var userExists = dbContext.Karter
                .Any(u => u.NameIdentifier == NameIdentifier.Value);

            if (!userExists)
            {
                context.Result = new RedirectResult("/karterHome/Create");
            }
        }
    }


    public class ValidGroupMember : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext
                .RequestServices
                .GetService(typeof(GoKartUniteContext)) as GoKartUniteContext;

            var groupId = int.Parse(context.HttpContext.Request.Query["GroupId"]);

            var NameIdentifier = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            var user = dbContext.Karter
                    .Where(u => u.NameIdentifier == NameIdentifier.Value).Single();

            var isUserInGroup = dbContext.Groups
                .Any(g => g.Id == groupId && (g.MemberKarters.Any(k => k.KarterId == user.Id) || g.HostKarter.Id == user.Id));

            if (!isUserInGroup)
            {
                context.Result = new JsonResult(new { success = false, message = "You are not authorized to access this group" })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
