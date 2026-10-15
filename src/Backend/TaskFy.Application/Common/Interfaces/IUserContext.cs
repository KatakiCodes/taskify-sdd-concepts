namespace TaskFy.Application.Common.Interfaces;

public interface IUserContext
{
    Guid CurrentUserId { get; }
}
