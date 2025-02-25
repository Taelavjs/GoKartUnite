namespace GoKartUnite.Interfaces
{
    public interface IRoleHandler
    {
        Task AddRoleToUser(int userId, string roleName);
        Task RemoveUserRole(int userId, string roleToRemove);
        Task<int> GetTrackUserTrackId(int userId);
        Task<bool> IsAdminAtTrack(string track, int userId);
    }
}
