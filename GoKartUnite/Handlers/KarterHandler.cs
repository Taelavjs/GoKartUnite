﻿using GoKartUnite.Data;
using GoKartUnite.Models;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using GoKartUnite.Interfaces;
using GoKartUnite.DataFilterOptions;
using Microsoft.Extensions.Options;
using GoKartUnite.ViewModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using GoKartUnite.Models.Groups;
using X.PagedList;
using GoKartUnite.Projection.Admin;
using GoKartUnite.Projection;

namespace GoKartUnite.Handlers
{
    public class KarterHandler : IKarterHandler
    {
        private readonly GoKartUniteContext _context;
        public KarterHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task<Karter> GetUser(int id, KarterGetAllUsersFilter? options = null)
        {

            if (options == null)
            {
                options = new KarterGetAllUsersFilter();
            }
            var query = _context.Karter.AsQueryable();
            var newQuery = QueryFilterIncludeOptions(options, query);

            var karterInDb = await newQuery.SingleOrDefaultAsync(x => x.Id == id);
            return karterInDb;
        }

        public async Task<Karter> GetUser(string name, KarterGetAllUsersFilter? options = null)
        {
            if (options == null)
            {
                options = new KarterGetAllUsersFilter();
            }
            var query = _context.Karter.AsQueryable();
            var newQuery = QueryFilterIncludeOptions(options, query);

            var karterInDb = await newQuery.SingleOrDefaultAsync(k => k.Name.ToLower() == name.ToLower());
            return karterInDb;
        }

        public async Task DeleteUser(Karter karter)
        {
            if (await GetUser(karter.Id) != karter) return;

            var friendshipsToRemove = await _context.Friendships
                .Where(f => f.KarterFirstId == karter.Id || f.KarterSecondId == karter.Id).ToListAsync();
            _context.Friendships.RemoveRange(friendshipsToRemove);

            _context.Karter.Remove(karter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            await DeleteUser(await GetUser(id));
        }

        public async Task<bool> SendFriendRequest(Karter sentBy, Karter requestedTo)
        {
            int sentById = sentBy.Id;
            if (sentBy == null || requestedTo == null)
            {
                return false;
            }

            if (sentBy.Id > requestedTo.Id)
            {
                (sentBy, requestedTo) = (requestedTo, sentBy);
            }

            var ifExists = await _context.Friendships.FindAsync(sentBy.Id, requestedTo.Id); // uupdate when all handlers created
            if (ifExists != null || sentBy.Id == requestedTo.Id)
            {
                return false;
            }

            var friendship = new Friendships(sentBy.Id, requestedTo.Id);
            _context.Attach(sentBy);
            _context.Attach(requestedTo);
            friendship.requestedByInt = sentById;

            await _context.Friendships.AddAsync(friendship);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Karter>> GetAllUsers(KarterGetAllUsersFilter? options = null)
        {

            if (options == null)
            {
                options = new KarterGetAllUsersFilter();
            }
            var query = _context.Karter.AsQueryable();
            var newQuery = QueryFilterIncludeOptions(options, query);

            switch (options.sort)
            {
                case SortKartersBy.Alphabetically:
                    newQuery = newQuery.OrderBy(k => k.Name);
                    break;
                case SortKartersBy.ReverseAlphabetically:
                    newQuery = newQuery.OrderByDescending(k => k.Name);
                    break;
                case SortKartersBy.YearsExperience:
                    newQuery = newQuery.OrderBy(k => k.YearsExperience);
                    break;
                case SortKartersBy.ReverseYearsExperience:
                    newQuery = newQuery.OrderByDescending(k => k.YearsExperience);
                    break;
            };
            return await newQuery.ToListAsync();

        }

        public async Task<int> GetNumberOfUserPages(string track, int usersPerPage = 3)
        {
            return _context.Karter.Include(k => k.Track).Count() / usersPerPage;
        }

        public async Task<List<Karter>> GetAllUsersByTrackId(int id, KarterGetAllUsersFilter? options = null)
        {
            var query = _context.Karter.AsQueryable();
            query = QueryFilterIncludeOptions(options, query);


            var karters = await query
                .Where(k => k.TrackId == id)
                .ToListAsync();

            return karters;
        }

        public async Task CreateUser(Karter karter, string email)
        {
            karter.Track = await _context.Track.SingleOrDefaultAsync(t => t.Id == karter.TrackId);
            karter.Email = email;
            var prevKarterRecord = await GetUser(karter.Id);

            if (prevKarterRecord == null)
            {
                if (await _context.Karter.FirstOrDefaultAsync(k => k.NameIdentifier == karter.NameIdentifier) != null)
                {
                    return;
                }
                _context.Karter.Add(karter);
            }
            else
            {
                prevKarterRecord.Name = karter.Name;
                prevKarterRecord.Track = karter.Track;
                prevKarterRecord.TrackId = karter.TrackId;
                prevKarterRecord.YearsExperience = karter.YearsExperience;
            }

            await _context.SaveChangesAsync();

        }

        public async Task<Karter> GetUserByGoogleId(string GoogleId, bool withTrack = false)
        {

            if (withTrack)
            {
                var karterInDbD = await _context.Karter.Include(k => k.Track).FirstOrDefaultAsync(k => k.NameIdentifier == GoogleId);
                return karterInDbD;
            }
            var karterInDb = await _context.Karter.FirstOrDefaultAsync(k => k.NameIdentifier == GoogleId);
            return karterInDb;
        }

        public async Task<List<FriendStatusNotification>> GetUsersFriendNotifications(int userId, int Skip = 0, int Take = 5)
        {
            var AllFriendNotifs = await _context.FriendStatusNotifications.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateCreated)
                .Skip(Skip)
                .Take(Take)
                .ToListAsync();

            return (await Task.WhenAll(AllFriendNotifs.Select(x => x.ModelToView()))).ToList();
        }

        public async Task<List<KarterView>> KarterModelToView(List<Karter> karters, FriendshipStatus status)
        {
            List<KarterView> kvs = new List<KarterView>();
            foreach (var karter in karters)
            {

                KarterView kv = new KarterView();
                kv.Id = karter.Id;
                kv.YearsExperience = karter.YearsExperience;
                kv.Name = karter.Name;
                kv.LocalTrack = karter.Track;
                kv.FriendStatus = status;

                kvs.Add(kv);
            }

            return kvs;
        }

        public async Task<KarterView> KarterModelToView(Karter karter, FriendshipStatus status)
        {

            KarterView kv = new KarterView();
            kv.Id = karter.Id;
            kv.YearsExperience = karter.YearsExperience;
            kv.Name = karter.Name;
            kv.LocalTrack = karter.Track;
            kv.FriendStatus = status;

            return kv;
        }

        public async Task UpdateUser(string nameIdentifier, KarterView kv)
        {
            Karter k = await GetUserByGoogleId(nameIdentifier);
            k.Name = kv.Name;
            k.TrackId = kv.TrackId;
            k.YearsExperience = kv.YearsExperience;

            _context.Karter.Update(k);
            _context.SaveChanges();
        }

        private IQueryable<Karter> QueryFilterIncludeOptions(KarterGetAllUsersFilter options, IQueryable<Karter> query)
        {
            if (options == null)
            {
                options = new KarterGetAllUsersFilter();
            }

            if (options.IncludeTrack)
            {
                query = query.Include(k => k.Track);
            }

            if (options.IncludeNotification)
            {
                query = query.Include(k => k.Notification);
            }

            if (options.IncludeBlogPosts)
            {
                query = query.Include(k => k.BlogPosts);
            }

            if (options.IncludeFriendships)
            {
                query = query.Include(k => k.Friendships);
            }

            if (options.IncludeUserRoles)
            {
                query = query.Include(k => k.UserRoles);
            }
            if (options.IncludeComments)
            {
                query = query.Include(k => k.Comments);
            }

            if (!string.IsNullOrEmpty(options.TrackToFetchFor))
            {
                query = query.Where(k => k.Track != null && k.Track.Title == options.TrackToFetchFor);
            }

            return query;
        }


        public async Task<string> GetCurrentUserNameIdentifier(ClaimsPrincipal User)
        {
            var retStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (retStr == null) return String.Empty;
            return retStr.Value;
        }

        public async Task<KarterProfilePreview> GetUserProfileCard(string username)
        {
            if (username == "") return new KarterProfilePreview();

            var res = _context.Karter.Where(x => x.Name == username)
                .Select(x => new KarterProfilePreview
                {
                    Id = x.Id,
                    Name = x.Name,
                    YearsExperience = "" + x.YearsExperience,
                    LocalTrack = x.Track.Title ?? "Freelancer",
                }).FirstOrDefault();

            if (res == null) return new KarterProfilePreview();
            return res;

        }

        public async Task<List<KarterAdminView>> GetAllUsersAdmin()
        {
            return await _context.Karter
                .Include(k => k.Track)
                .Include(k => k.UserRoles)
                .Include(k => k.Friendships)
                .Include(k => k.BlogPosts)
                .Include(k => k.Notification)
                .Include(k => k.Stats)
                .Select(k => new KarterAdminView
                {
                    Id = k.Id,
                    Email = k.Email,
                    NameIdentifier = k.NameIdentifier,
                    Name = k.Name,
                    YearsExperience = k.YearsExperience,
                    TrackId = k.TrackId,
                    Track = k.Track,
                    UserRoles = k.UserRoles.Count,
                    Friendships = k.Friendships.Count,
                    BlogPosts = k.BlogPosts.Count,
                    Notification = k.Notification.Count,
                    Stats = k.Stats.Count,
                })
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetAllUserPostsAdmin(int id)
        {
            return await _context.BlogPosts.Include(k => k.Karter).Where(k => k.KarterId == id).ToListAsync();
        }

        public async Task<List<Comment>> GetAllUsersCommentsAdmin(int id)
        {
            return await _context.Comments.Include(k => k.BlogPost).Include(k => k.Author).Where(k => k.AuthorId == id).ToListAsync();
        }

        public async Task<List<Models.Groups.Group>> GetAllUsersGroupsAdmin(int id)
        {
            return await _context.Groups.Where(k => k.HostKarter.Id == id).ToListAsync();
        }

        public async Task<List<AdminGroupMessage>> GetUsersMessagesByGroup(int id, int groupId)
        {
            var messages = await _context.GroupMessages
                .Where(x => x.GroupCommentOnId == groupId && x.AuthorId == id)
                .Select(x => new AdminGroupMessage
                {
                    Id = x.Id,
                    UserId = x.AuthorId,
                    Username = x.Author.Name,
                    DateSent = x.DateTimePosted,
                    MessageContent = x.MessageContent
                })
                .ToListAsync();

            return messages;


        }

        public async Task<Dictionary<int, string>> GetUserGroupsList(int id)
        {
            return await _context.Groups
                .Where(x => x.MemberKarters.Any(mk => mk.KarterId == id) || x.HostId == id)
                .Select(x => new { x.Id, x.Title })
                .ToDictionaryAsync(x => x.Id, x => x.Title);
        }

        public async Task<bool> DeleteUserAdmin(int id)
        {
            await _context.Friendships.Where(x => x.KarterSecondId == id || x.KarterSecondId == id).ExecuteDeleteAsync();
            await _context.Groups.Where(x => x.HostId == id).ExecuteDeleteAsync();
            await _context.Memberships.Where(x => x.KarterId == id).ExecuteDeleteAsync();
            await _context.GroupMessages.Where(x => x.AuthorId == id).ExecuteDeleteAsync();
            await _context.FollowTracks.Where(x => x.KarterId == id).ExecuteDeleteAsync();

            await _context.Karter.Where(x => x.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
