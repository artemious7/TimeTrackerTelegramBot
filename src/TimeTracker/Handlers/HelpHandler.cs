using System.Diagnostics.CodeAnalysis;
using TimeTracker.Services;

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

    private const string StartCommand = "/start";
    private const string HelpCommand = "/help";
    private const string LineBreak = "  \r\n";
    private const string ResetCommand = "/reset";
    private const string ShowTotalCommand = "/showTotal";
    private const string UndoCommand = "/undo";
    private static string CommandListString => string.Join("", CommandList.Select(command => $"{LineBreak} {command}"));
    private static readonly string[] CommandList = [ShowTotalCommand, UndoCommand, ResetCommand, HelpCommand];
}