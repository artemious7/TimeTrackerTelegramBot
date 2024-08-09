using System.Diagnostics.CodeAnalysis;
using TimeTracker.Services;
using static TimeTracker.Data.Commands;

namespace TimeTracker.Handlers;

public class HelpHandler : IHandler, IHelpResponder
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (IsCommand(StartCommand) || IsCommand(HelpCommand))
        {
            await Help(SendMessage);
            return new(true, data);
        }
        return new(false, data);

        bool IsCommand(string command) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task Help(MessageSender SendMessage) => await SendMessage($"Send me the time, I will sum it up for you, e.g. `1:35` or `15:45 - 16:20` to add, or `-0:20` to subtract.{LineBreak}Commands:{CommandListString}");
}