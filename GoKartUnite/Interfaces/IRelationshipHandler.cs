﻿using GoKartUnite.Models;
using GoKartUnite.ViewModel;

namespace GoKartUnite.Interfaces
{
    public interface IRelationshipHandler
    {
        Task<int> GetFriendsCount(int id);
        Task<List<Karter>> GetAllFriends(int id);
        Task<List<Karter>> GetAllFriendRequests(int id);
        Task<List<Karter>> GetAllSentRequests(int id);
        Task<bool> AcceptFriendRequest(int acceptorsId, int senderId);
        Task<bool> RemoveFriendShip(int sentbyId, int friendId);
        Task<Friendships> GetFriendshipByIds(int id1, int id2);
        Task<List<KarterView>> AddStatusToKarters(List<KarterView> karters, int userId);
        Task<FriendshipStatus> GetFriendshipStatus(int id1, int id2);
    }
}
