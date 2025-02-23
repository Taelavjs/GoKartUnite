using GoKartUnite.Data;
using GoKartUnite.Models;
using System.Security.Claims;

namespace GoKartUnite.Handlers
{
    public class RoleHandler
    {
        private readonly GoKartUniteContext _context;
        public RoleHandler(GoKartUniteContext context)
        {
            _context = context;
        }

        public async Task AddRoleToUser(int userId, string roleName)
        {
            int roleId = _context.Role.FirstOrDefault(r => r.Name == roleName)?.Id ?? 0;

            UserRoles role = new UserRoles
            {
                KarterId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(role);
        }

        public async Task RemoveUserRole(int userId, string roleToRemove)
        {
            int roleId = _context.Role.FirstOrDefault(r => r.Name == roleToRemove)?.Id ?? 0;

            UserRoles deleteRole = _context.UserRoles.FirstOrDefault(r => r.KarterId == userId && roleId == r.RoleId);
            _context.UserRoles.Remove(deleteRole);
        }

        public async Task<int> getTrackUserTrackId(int userId)
        {
            return _context.TrackAdmin
                .Single(t => t.KarterId == userId).TrackId;
        }

        public async Task<bool> isAdminAtTrack(string track, int userId)
        {
            TrackAdmins admin = _context.TrackAdmin.SingleOrDefault(t => t.KarterId == userId);
            if (admin == null || admin.ManagedTrack == null) return false;

            return admin.ManagedTrack.Title == track;
        }
    }
}
