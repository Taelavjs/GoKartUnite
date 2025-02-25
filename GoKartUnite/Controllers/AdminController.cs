using GoKartUnite.Handlers;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoKartUnite.Controllers
{
    public class AdminController : Controller
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
            List<Karter> karters = await _karter.GetAllUsers(true);
            return View(karters);
        }
    }
}
