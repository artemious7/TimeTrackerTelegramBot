using TimeTrackerBot.Services;

namespace TimeTracker;

public class HelpHandler
{
    public async Task<bool> TryHandle(string message, MessageSender SendMessage)
    {
        if (IsCommand(StartCommand) || IsCommand(HelpCommand))
        {
            await Help();
            return true;
        }
        return false;

        bool IsCommand(string command) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);

        async Task Help() => await SendMessage($"Send me the time, I will sum it up for you, e.g. `1:35` or `15:45 - 16:20` to add, or `-0:20` to subtract.{LineBreak}Commands:{CommandListString}");
    }

    private const string StartCommand = "/start";
    private const string HelpCommand = "/help";
    private const string LineBreak = "  \r\n";
    private static string CommandListString => string.Join("", CommandList.Select(command => $"{LineBreak} {command}"));
    private const string ResetCommand = "/reset";
    private const string ShowTotalCommand = "/showTotal";
    private const string UndoCommand = "/undo";
    private static readonly string[] CommandList = [ShowTotalCommand, UndoCommand, ResetCommand, HelpCommand];
}