using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.ViewModel;
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

        public async Task JoinGroup(int groupId, Karter karter)
        {
            Group? group = await _context.Groups
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null) return;

            if (!group.MemberKarters.Contains(karter) || group.HostKarter == karter) return;

            group.MemberKarters.Add(karter);

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task LeaveGroup(int groupId, Karter karter)
        {
            Group? group = await _context.Groups
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null) return;

            if (!group.MemberKarters.Contains(karter)) return;

            group.MemberKarters.Remove(karter);

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }


    }
}