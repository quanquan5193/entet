namespace mrs.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string RoleLevel { get; }
    }
}
