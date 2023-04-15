using System;
using System.Text;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace CustomUnit.Commands;

public class List : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermissions("cu.list", out response))
            return false;

        StringBuilder sb = new();

        foreach (var name in Plugin.Configs.Keys)
            sb.AppendLine(name);

        response = $"All loaded units:\n{string.Join(" ", sb)}";
        return true;
    }

    public string Command { get; }
    public string[] Aliases { get; }
    public string Description { get; }
}