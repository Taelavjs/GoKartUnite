﻿using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.Projection;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IGroupHandler
    {
        Task CreateNewGroup(ListedGroupView group, Karter k);
        Task<List<ListedGroupView>> GetAllGroups(Karter k, Filters? filter, string groupName);
        Task<bool> JoinGroup(int groupId, Karter karter);
        Task<bool> LeaveGroup(int groupId, Karter karter);
        Task<List<GroupMessageView>> MessagesToDTO(List<GroupMessage> comments);
        Task<List<GroupView>> ToDTOList(List<Group> group);
        Task<GroupView> ToDTO(Group group);
        Task<Group> GetGroupById(int groupId);
        Task<List<GroupMember>> GetAllMembersProjection(int groupId);
        Task<List<GroupStatDisplay>> GetStatsForGroupGraph(int groupId, string trackTitle);
        Task<bool> CreateUserMessageInGroup(int groupId, string messageContent, Karter user);

        Task<List<int>> GetAllUserGroupIds(int kId);

    }

    public enum Filters
    {
        NONE,
        DATE,
        NAME,
        MEMBERCOUNT
    }
}
