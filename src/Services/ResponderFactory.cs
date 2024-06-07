using ReportCounterBot.ReportCounter;

namespace ReportCounterBot.Services;

public delegate IResponder ResponderFactory(string message, UserData? data, MessageSender SendMessage);