using CommandSystem;
using Exiled.Permissions.Extensions;

namespace CustomUnit;

public static class Extensions
{
    public static bool CheckPermissions(this ICommandSender sender, string permission, out string response)
    {
        if (sender.CheckPermission(permission))
        {
            response = null;
            return true;
        }
        else
        {
            response = "You don't have permission to execute this command";
            return false;
        }
    }
}