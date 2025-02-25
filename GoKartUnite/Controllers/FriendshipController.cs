using GoKartUnite.Data;
using GoKartUnite.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GoKartUnite.Interfaces;

namespace GoKartUnite.Controllers
{
    public class FriendshipController : Controller
    {
        private readonly GoKartUniteContext _context;
        private readonly IRelationshipHandler _relationship;
        public FriendshipController(IRelationshipHandler relationships)
        {
            _relationship = relationships;
        }
    }
}
