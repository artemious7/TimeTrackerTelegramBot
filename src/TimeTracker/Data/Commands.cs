namespace TimeTracker.Data;

internal static class Commands
{
    public const string StartCommand = "/start";
    public const string HelpCommand = "/help";
    public const string LineBreak = "  \r\n";
    public const string ResetCommand = "/reset";
    public const string ShowTotalCommand = "/showTotal";
    public const string UndoCommand = "/undo";
    public static string CommandListString => string.Join("", CommandList.Select(command => $"{LineBreak} {command}"));
    public static readonly string[] CommandList = [ShowTotalCommand, UndoCommand, ResetCommand, HelpCommand];
}
