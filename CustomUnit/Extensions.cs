using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MapGeneration;
using UnityEngine;

namespace CustomUnit;

public static class Extensions
{
    public static bool CheckPermission(this ICommandSender sender, string permission, out string response)
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

    public static Vector3 GetSpawnPos(this RoomName rm)
    {
        var dr = Room.Get(x => x.RoomName == rm).ToList().RandomItem().Doors.ToList().RandomItem();
        return dr.Position + Vector3.up * 0.5f + dr.Transform.forward * 3f;
    }
}