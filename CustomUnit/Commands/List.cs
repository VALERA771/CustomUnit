using System;
using System.Text;
using CommandSystem;

namespace CustomUnit.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class List : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("cu.list", out response))
            return false;

        StringBuilder sb = new();

        foreach (var name in Plugin.Configs.Keys)
            sb.AppendLine(name);

        response = $"All loaded units:\n{string.Join(" ", sb)}";
        return true;
    }

    public string Command { get; } = "list_units";
    public string[] Aliases { get; } = { "lu" };
    public string Description { get; } = "Shows a list with all loaded units";
}