using TaskFy.Application.Common.Interfaces;

namespace TaskFy.Api.Common;

public sealed class DefaultUserContext : IUserContext
{
    public static readonly Guid DefaultUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public Guid CurrentUserId => DefaultUserId;
}
