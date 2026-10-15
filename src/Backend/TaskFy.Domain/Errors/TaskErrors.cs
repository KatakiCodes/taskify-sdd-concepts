using TaskFy.Domain.Common;

namespace TaskFy.Domain.Errors;

public static class TaskErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Task.NotFound",
        "The requested task was not found.");

    public static readonly Error InvalidName = Error.Validation(
        "Task.InvalidName",
        "Task name is required and must be between 3 and 150 characters.");

    public static readonly Error InvalidDates = Error.Validation(
        "Task.InvalidDates",
        "Task end date cannot be earlier than start date.");
}
