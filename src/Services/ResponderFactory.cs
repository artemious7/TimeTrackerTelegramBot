using TimeTrackerBot.TimeTracker;

namespace TimeTrackerBot.Services;

public delegate IResponder ResponderFactory(string message, UserData? data, MessageSender SendMessage);