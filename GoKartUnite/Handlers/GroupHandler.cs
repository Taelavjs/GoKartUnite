﻿using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.Projection;
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

        public async Task<GroupView> ToDTO(Group group)
        {
            return new GroupView
            {
                Id = group.Id,
                Description = group.Description,
                Comments = await MessagesToDTO(group.GroupMessages),
                CreatorName = group.HostKarter?.Name ?? "",
                MemberCount = group.MemberKarters?.Count ?? 0,
                Name = group.Title,
            };
        }

        public async Task<List<GroupView>> ToDTOList(List<Group> group)
        {
            List<GroupView> listToReturn = new List<GroupView>();
            foreach (Group groupView in group)
            {
                listToReturn.Add(await ToDTO(groupView));
            }
            return listToReturn;

        }


        public async Task<List<GroupMessageView>> MessagesToDTO(List<GroupMessage> comments)
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

        public async Task<Group> GetGroupById(int groupId)
        {
            return await _context.Groups
                .Include(X => X.GroupMessages)
                .Include(x => x.MemberKarters)
                .SingleOrDefaultAsync(x => x.Id == groupId);
        }

        public async Task<List<GroupMember>> GetAllMembersProjection(int groupId)
        {
            return await _context.Groups
                .Where(x => x.Id == groupId)
                .SelectMany(p => p.MemberKarters.Select(m => new GroupMember
                {
                    Id = m.KarterId,
                    Name = m.User.Name,
                }))
                .ToListAsync();
        }

        public async Task<List<GroupStatDisplay>> GetStatsForGroupGraph(int groupId, string trackTitle)
        {
            List<int> karterIds = await _context.Groups.Where(x => x.Id == groupId)
                .SelectMany(x => x.MemberKarters.Select(z => z.KarterId)).ToListAsync();

            karterIds.Add(await _context.Groups.Where(x => x.Id == groupId).Select(x => x.HostId).FirstOrDefaultAsync());

            var t = _context.KarterTrackStats
                .Include(x => x.ForKarter)
                .Where(x => karterIds.Contains(x.KarterId) && x.RecordedTrack.Title == trackTitle)
                .GroupBy(x => x.KarterId);

            List<GroupStatDisplay> stats = new List<GroupStatDisplay>();
            foreach (var group in t)
            {
                var model = group.Select(x => new
                {
                    ForKarterName = x.ForKarter.Name,
                    BestLapTime = x.BestLapTime.TotalMilliseconds
                }).FirstOrDefault();

                if (model == null) continue;

                stats.Add(new GroupStatDisplay
                {
                    KarterName = model.ForKarterName,
                    BestLapTime = model.BestLapTime
                });
            };



            return stats;
        }
    }
}