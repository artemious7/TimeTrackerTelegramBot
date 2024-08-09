namespace TimeTracker.Services;

public delegate IResponder ResponderFactory(string message, UserData? data, MessageSender SendMessage);