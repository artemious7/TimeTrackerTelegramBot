using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Services;

#pragma warning disable CS9113 // Parameter is unread.
public class Responder(string message, UserData? data, MessageSender SendMessage, IEnumerable<IHandler> handlers)
{
    public async Task<UserData?> Process()
    {
        return data;
    }
}
