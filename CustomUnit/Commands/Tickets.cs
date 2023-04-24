using System;
using System.Linq;
using CommandSystem;

namespace CustomUnit.Commands;

public class Tickets : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("cu.tickets", out response))
            return false;

        if (arguments.Count != 1)
        {
            response = this.DisplayCommandUsage();
            return false;
        }

        // ReSharper disable once SimplifyLinqExpressionUseAll
        if (!Plugin.Tickets.Any(x => x.Key.UnitName == arguments.At(0).ToLower()))
        {
            response = "Team does not exist or use chance system";
            return false;
        }

        response =
            $"Unit {arguments.At(0)} has {Plugin.Tickets[Plugin.Tickets.Keys.First(x => x.UnitName == arguments.At(0).ToLower())]} tickets";
        return true;
    }

    public string Command { get; } = "tickets";
    public string[] Aliases { get; } = { "ticks" };
    public string Description { get; } = "Get amount of ticket for a specific team";
    public string[] Usage { get; } = new[]
    {
        "Team"
    };
}