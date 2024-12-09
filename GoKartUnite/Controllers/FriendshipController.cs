using GoKartUnite.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Controllers
{
    public class FriendshipController : Controller
    {
        private readonly GoKartUniteContext _context;
        public FriendshipController(GoKartUniteContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetNumberFriends(int id)
        {
            int numFriends = await _context.Friendships.CountAsync(k => k.KarterFirstId == id || k.KarterSecondId == id);
            return Json(numFriends);
        }
    }
}
