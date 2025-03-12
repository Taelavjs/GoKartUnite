using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoKartUnite.Handlers
{
    public class GroupHandler : IGroupHandler
    {
        private readonly GoKartUniteContext _context;

        public GroupHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task CreateNewGroup(GroupView group, Karter k)
        {
            Group newGroup = new Group
            {
                Title = group.Name,
                HostKarter = k,
                Description = group.Description,
                HostId = k.Id,
            };

            _context.Groups.Add(newGroup);
            _context.SaveChanges();
        }

        public async Task<List<GroupView>> GetAllGroups()
        {
            List<GroupView> groups = await _context.Groups
                .Include(g => g.HostKarter)
                .Include(g => g.MemberKarters)
                .Include(g => g.GroupPosts)
                .Select(g => new GroupView
                {
                    Id = g.Id,
                    Name = g.Title,
                    Description = g.Description,
                    LeaderName = g.HostKarter.Name,
                    NumberMembers = g.MemberKarters.Count,
                    DateCreated = g.DateCreated,
                })
                .ToListAsync();

            return groups;
        }
    }
}
