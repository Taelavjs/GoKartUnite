using GoKartUnite.CustomAttributes;
using GoKartUnite.DataFilterOptions;
using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

namespace GoKartUnite.Controllers
{
    [Authorize(Roles = "Admin")]
    [AccountConfirmed]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IKarterHandler _karter;

        public AdminController(KarterHandler karter)
        {
            _karter = karter;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            KarterGetAllUsersFilter filter = new KarterGetAllUsersFilter
            {
                IncludeTrack = true
            };
            List<Karter> karters = await _karter.GetAllUsers(filter);
            return View(karters);
        }
    }
}
