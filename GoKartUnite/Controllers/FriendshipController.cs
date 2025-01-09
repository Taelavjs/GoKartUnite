using GoKartUnite.Data;
using GoKartUnite.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GoKartUnite.Controllers
{
    public class FriendshipController : Controller
    {
        private readonly GoKartUniteContext _context;
        private readonly RelationshipHandler _relationship;
        public FriendshipController(RelationshipHandler relationships)
        {
            _relationship = relationships;
        }
    }
}
