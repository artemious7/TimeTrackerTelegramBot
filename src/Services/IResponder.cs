using ReportCounterBot.ReportCounter;

namespace ReportCounterBot.Services;

public interface IResponder
{
    Task<UserData?> Process();
}
