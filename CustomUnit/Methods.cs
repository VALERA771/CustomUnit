using System.Collections.Generic;
using System;
using System.Linq;
using CustomUnit.EventOptions;
using Exiled.Events.EventArgs.Interfaces;
using HarmonyLib;
using PlayerRoles;
using PluginAPI.Enums;
using PluginAPI.Events;
using Respawning;
using Respawning.NamingRules;
using NorthwoodLib.Pools;
using GameCore;
using System.Diagnostics;

using static Respawning.RespawnManager;

namespace CustomUnit;

public abstract class Methods
{
    public static void AddChance(IExiledEvent ev)
    {
        foreach (var unit in Plugin.Tickets.Where(unit => unit.Key.Events.ContainsKey(Options.Events[ev.GetType()])))
            Plugin.Tickets[unit.Key] += unit.Key.Events[Options.Events[ev.GetType()]];
    }

    public static void SpawnDefault(SpawnableTeamType team, List<ReferenceHub> list)
    {
        Singleton.NextKnownTeam = team;

        #region Spawn
        if (!SpawnableTeams.TryGetValue(team, out var value) || team == SpawnableTeamType.None)
        {
            ServerConsole.AddLog(string.Concat("Fatal error. Team '", team, "' is undefined."), ConsoleColor.Red);
            return;
        }

        if (!EventManager.ExecuteEvent(ServerEventType.TeamRespawn, team))
        {
            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, team);
            team = SpawnableTeamType.None;
            return;
        }

        if ((bool)AccessTools.Field(typeof(RespawnManager), nameof(RespawnManager._prioritySpawn)).GetValue(new bool()))
        {
            list = list.OrderByDescending((ReferenceHub item) => item.roleManager.CurrentRole.ActiveTime).ToList();
        }
        else
        {
            list.ShuffleList();
        }

        int maxWaveSize = value.MaxWaveSize;
        int num = list.Count;
        if (num > maxWaveSize)
        {
            list.RemoveRange(maxWaveSize, num - maxWaveSize);
            num = maxWaveSize;
        }

        if (num > 0 && UnitNamingRule.TryGetNamingRule(team, out var rule))
        {
            UnitNameMessageHandler.SendNew(team, rule);
        }

        list.ShuffleList();
        List<ReferenceHub> list2 = ListPool<ReferenceHub>.Shared.Rent();
        Queue<RoleTypeId> queue = new Queue<RoleTypeId>();
        value.GenerateQueue(queue, list.Count);
        foreach (ReferenceHub item in list)
        {
            try
            {
                RoleTypeId newRole = queue.Dequeue();
                item.roleManager.ServerSetRole(newRole, RoleChangeReason.Respawn);
                list2.Add(item);
                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + item.LoggedNameFromRefHub() + " respawned as " + newRole.ToString() + ".", ServerLogs.ServerLogType.GameEvent);
            }
            catch (Exception ex)
            {
                if (item != null)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + item.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent);
                }
                else
                {
                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent);
                }
            }
        }

        if (list2.Count > 0)
        {
            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "RespawnManager has successfully spawned " + list2.Count + " players as " + team.ToString() + "!", ServerLogs.ServerLogType.GameEvent);
            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, team);
        }

        
        ListPool<ReferenceHub>.Shared.Return(list2);
        AccessTools.Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam)).SetValue(Singleton, SpawnableTeamType.None);
        #endregion

        #region RestartSequence
        AccessTools.Field(typeof(RespawnManager), nameof(RespawnManager._timeForNextSequence)).SetValue(Singleton, UnityEngine.Random.Range(ConfigFile.ServerConfig.GetFloat("minimum_MTF_time_to_spawn", 280f), ConfigFile.ServerConfig.GetFloat("maximum_MTF_time_to_spawn", 350f)));
        AccessTools.Field(typeof(RespawnManager), nameof(RespawnManager._curSequence)).SetValue(Singleton, RespawnSequencePhase.RespawnCooldown);

        var watch = (Stopwatch)AccessTools.Field(typeof(RespawnManager), nameof(RespawnManager._stopwatch))
            .GetValue(Singleton);

        if (watch.IsRunning)
        {
            watch.Restart();
        }
        else
        {
            watch.Start();
        }

        AccessTools.Field(typeof(RespawnManager), nameof(RespawnManager._stopwatch)).SetValue(Singleton, watch);
        #endregion
    }
}