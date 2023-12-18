using CustomUnit.EventOptions;
using Exiled.Events.EventArgs.Interfaces;
using GameCore;
using NorthwoodLib.Pools;
using PlayerRoles;
using PluginAPI.Events;
using Respawning;
using Respawning.NamingRules;
using System;
using System.Collections.Generic;
using System.Linq;

using static Respawning.RespawnManager;

using Random = System.Random;

namespace CustomUnit;

public static class Methods
{
    public static void AddChance(IExiledEvent ev)
    {
        if (Plugin.Instance.Config.Options.Any(x => x.Key == Options.Events[ev.GetType()]))
        {
            var opt = Plugin.Instance.Config.Options[Options.Events[ev.GetType()]];

            if (!opt.IsEnabled)
                return;

            if (new Random().Next(0, 101) <= opt.Chance)
                return;

            if (ev is IPlayerEvent pev)
            {
                if (!opt.Allow.Contains(pev.Player.Role.Type) || opt.Disallow.Contains(pev.Player.Role.Type))
                    return;
            }

            if (ev is IDeniableEvent { IsAllowed: false }) return;

            if (ev is IAttackerEvent aev)
            {
                if (!opt.Allow.Contains(aev.Player.Role.Type) || opt.Disallow.Contains(aev.Player.Role.Type))
                    return;
            }
        }

        foreach (var unit in Plugin.Tickets.Where(unit => unit.Key.Events.ContainsKey(Options.Events[ev.GetType()])))
        {
            Plugin.Tickets[unit.Key] += unit.Key.Events[Options.Events[ev.GetType()]];
        }
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

        if (!EventManager.ExecuteEvent(new TeamRespawnEvent(team, list)))
        {
            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, team);
            Singleton.NextKnownTeam = SpawnableTeamType.None;
            return;
        }

        if (Singleton._prioritySpawn)
        {
            list = list.OrderByDescending(item => item.roleManager.CurrentRole.ActiveTime).ToList();
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
        Singleton.NextKnownTeam = SpawnableTeamType.None;
        #endregion

        #region RestartSequence
        
        Singleton._timeForNextSequence = UnityEngine.Random.Range(ConfigFile.ServerConfig.GetFloat("minimum_MTF_time_to_spawn", 280f), ConfigFile.ServerConfig.GetFloat("maximum_MTF_time_to_spawn", 350f));
        Singleton._curSequence = RespawnSequencePhase.RespawnCooldown;

        if (Singleton._stopwatch.IsRunning)
        {
            Singleton._stopwatch.Restart();
        }
        else
        {
            Singleton._stopwatch.Start();
        }

        #endregion
    }
}