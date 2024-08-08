using TimeTrackerBot.Services;

namespace TimeTracker
{
    public class HelpHandler
    {
        public HelpHandler()
        {
        }

        public async Task<bool> TryHandle(string message, MessageSender messageSender)
        {
            return false;
        }
    }
}