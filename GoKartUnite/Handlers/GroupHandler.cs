using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
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
                Description = group.Description,
                HostId = k.Id,
            };

            _context.Groups.Add(newGroup);
            _context.SaveChanges();


            Membership newMembership = new Membership
            {
                KarterId = k.Id,
                User = k,
                GroupId = newGroup.Id,
                Group = newGroup,
                MemberRole = GroupMemberStatus.OWNER
            };

            newGroup.MemberKarters = new List<Membership> { newMembership };

            _context.Memberships.Add(newMembership);
            _context.SaveChanges();
        }

        public async Task<List<GroupView>> GetAllGroups(Karter k)
        {
            List<GroupView> groups = await _context.Groups
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
                    isMember = g.MemberKarters.Any(x => x.KarterId == k.Id) || g.HostKarter == k,
                })
                .ToListAsync();

            return groups;
        }

        public async Task JoinGroup(int groupId, Karter karter)
        {
            var group = await _context.Groups.Include(x => x.MemberKarters)
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null) return;

            if (group.MemberKarters.Any(x => x.User == karter && x.Group == group) || group.HostKarter == karter) return;

            group.MemberKarters.Add(new Membership
            {
                User = karter,
                KarterId = karter.Id,
                Group = group,
                GroupId = group.Id,
                MemberRole = GroupMemberStatus.MEMBER,
            });

            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task LeaveGroup(int groupId, Karter karter)
        {
            Group? group = await _context.Groups
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null) return;

            if (!group.MemberKarters.Any(x => x.KarterId == karter.Id)) return;
            var membershipToRemove = group.MemberKarters
                .FirstOrDefault(x => x.GroupId == group.Id && x.KarterId == karter.Id);
            if (membershipToRemove != null)
            {
                group.MemberKarters.Remove(membershipToRemove);
                _context.Memberships.Remove(membershipToRemove);
                await _context.SaveChangesAsync();
            }
        }


    }
}