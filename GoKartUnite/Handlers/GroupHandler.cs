using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.ViewModel;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace GoKartUnite.Handlers
{
    public class GroupHandler : IGroupHandler
    {
        private readonly GoKartUniteContext _context;

        public GroupHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task CreateNewGroup(ListedGroupView group, Karter k)
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

        public async Task<List<ListedGroupView>> GetAllGroups(Karter k)
        {
            List<ListedGroupView> groups = await _context.Groups
                .Include(g => g.MemberKarters)
                .Include(g => g.GroupMessages)
                .Select(g => new ListedGroupView
                {
                    Id = g.Id,
                    Name = g.Title,
                    Description = g.Description,
                    LeaderName = g.HostKarter.Name,
                    NumberMembers = g.MemberKarters.Count,
                    DateCreated = g.DateCreated,
                    isOwner = g.HostKarter == k,
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

        public async Task<List<Group>> GetMessagesForGroup(int groupId)
        {
            List<Group> groupsToReturn = _context.Groups.Where(x => x.Id == groupId)
                .Include(x => x.GroupMessages).ToList();
            return groupsToReturn;
        }

        public static async Task<GroupView> ToDTO(Group group)
        {
            return new GroupView
            {
                Id = group.Id,
                Description = group.Description,
                Comments = await MessagesToDTO(group.GroupMessages),
                CreatorName = group.HostKarter.Name,
                MemberCount = group.MemberKarters?.Count ?? 0,
                Name = group.Title,
            };
        }

        public static async Task<List<GroupView>> ToDTOList(List<Group> group)
        {
            List<GroupView> listToReturn = new List<GroupView>();
            foreach (Group groupView in group)
            {
                listToReturn.Add(await ToDTO(groupView));
            }
            return listToReturn;

        }


        public static async Task<List<GroupMessageView>> MessagesToDTO(List<GroupMessage> comments)
        {
            List<GroupMessageView> commentsToReturn = new List<GroupMessageView>();

            foreach (GroupMessage comment in comments)
            {
                commentsToReturn.Add(new GroupMessageView
                {
                    Id = comment.Id,
                    AuthorName = comment.Author?.Name ?? "",
                    MessageContent = comment.MessageContent,
                    TimeSent = comment.DateTimePosted,
                });
            }
            return commentsToReturn;
        }
    }
}