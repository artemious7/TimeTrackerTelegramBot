namespace TimeTracker.Services;

public interface IResponder
{
    Task<UserData?> Process();
}
