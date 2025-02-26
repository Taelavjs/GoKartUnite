using GoKartUnite.Data;
using GoKartUnite.Interfaces;
using GoKartUnite.Models;
using System.Security.Claims;

namespace GoKartUnite.Handlers
{
    public class RoleHandler : IRoleHandler
    {
        private readonly GoKartUniteContext _context;
        public RoleHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task AddRoleToUser(int userId, string roleName)
        {
            if (!_context.Karter.Any(x => x.Id == userId)) return;

            int roleId = _context.Role.FirstOrDefault(r => r.Name == roleName)?.Id ?? -1;
            if (roleId == -1) return;

            UserRoles role = new UserRoles
            {
                KarterId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(role);
            _context.SaveChanges();
        }

        public async Task RemoveUserRole(int userId, string roleToRemove)
        {
            int roleId = _context.Role.FirstOrDefault(r => r.Name == roleToRemove)?.Id ?? 0;

            UserRoles deleteRole = _context.UserRoles.FirstOrDefault(r => r.KarterId == userId && roleId == r.RoleId) ?? null;
            if (deleteRole == null) return;
            _context.UserRoles.Remove(deleteRole);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTrackUserTrackId(int userId)
        {
            return _context.TrackAdmin
                .Single(t => t.KarterId == userId).TrackId;
        }

        public async Task<bool> IsAdminAtTrack(string track, int userId)
        {
            TrackAdmins admin = _context.TrackAdmin.SingleOrDefault(t => t.KarterId == userId);
            if (admin == null || admin.ManagedTrack == null) return false;

            return admin.ManagedTrack.Title == track;
        }
    }
}
