namespace TimeTracker.Data;

internal static class Commands
{
    public static readonly string[] CommandList = [ShowTotalCommand, UndoCommand, ResetCommand, HelpCommand];
    public static string CommandListString => string.Join("", CommandList.Select(command => $"{LineBreak} {command}"));
    /// <summary>
    /// Line break in markdown contains extra 2 spaces
    /// </summary>
    public const string LineBreak = "  \r\n";
    public const string StartCommand = "/start";
    public const string HelpCommand = "/help";
    public const string ResetCommand = "/reset";
    public const string ShowTotalCommand = "/showTotal";
    public const string UndoCommand = "/undo";
    public static bool IsCommand(in string command, [AllowNull] in string message) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);
}
