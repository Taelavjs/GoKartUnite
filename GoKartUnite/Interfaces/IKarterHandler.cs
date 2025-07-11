﻿using GoKartUnite.DataFilterOptions;
using GoKartUnite.Models;
using GoKartUnite.Models.Groups;
using GoKartUnite.Projection;
using GoKartUnite.Projection.Admin;
using GoKartUnite.ViewModel;
using System.Security.Claims;

namespace GoKartUnite.Interfaces
{
    public interface IKarterHandler
    {
        Task<Karter> GetUser(int id, KarterGetAllUsersFilter? options = null);
        Task<Karter> GetUser(string name, KarterGetAllUsersFilter? options = null);
        Task DeleteUser(Karter karter);
        Task DeleteUser(int id);
        Task<bool> SendFriendRequest(Karter sentBy, Karter requestedTo);
        Task<List<Karter>> GetAllUsers(KarterGetAllUsersFilter? options = null);
        Task<int> GetNumberOfUserPages(string track, int usersPerPage = 3);
        Task<List<Karter>> GetAllUsersByTrackId(int id, KarterGetAllUsersFilter? options = null);
        Task CreateUser(Karter karter, string email);
        Task<Karter> GetUserByGoogleId(string GoogleId, bool withTrack = false);
        Task<List<KarterView>> KarterModelToView(List<Karter> karters, FriendshipStatus status);
        Task<KarterView> KarterModelToView(Karter karter, FriendshipStatus status);
        Task UpdateUser(string nameIdentifier, KarterView kv);
        Task<string> GetCurrentUserNameIdentifier(ClaimsPrincipal User);
        Task<KarterProfilePreview> GetUserProfileCard(string username);
        Task<List<KarterAdminView>> GetAllUsersAdmin();

        Task<List<BlogPost>> GetAllUserPostsAdmin(int id);

        Task<List<Comment>> GetAllUsersCommentsAdmin(int id);

        Task<List<Models.Groups.Group>> GetAllUsersGroupsAdmin(int id);
        Task<List<AdminGroupMessage>> GetUsersMessagesByGroup(int id, int groupId);
        Task<Dictionary<int, string>> GetUserGroupsList(int id);
        Task<bool> DeleteUserAdmin(int id);
        Task<List<FriendStatusNotification>> GetUsersFriendNotifications(int userId, int Skip = 0, int Take = 5);

    }
}
