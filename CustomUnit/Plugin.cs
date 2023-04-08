﻿using System;
using CustomUnit.Configs;
using Exiled.API.Features;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomUnit.EventOptions;
using Exiled.Loader;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using Map = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Version = System.Version;
using Warhead = Exiled.Events.Handlers.Warhead;

namespace CustomUnit
{
    public class Plugin : Plugin<Configs.Plugin>
    {
        public static Plugin Instance { get; private set; }
        public static Unit UnitConfig => new();
        public static string ExampleUnit { get; private set; }

        public override string Name => "Custom Unit";
        public override string Prefix => Assembly.GetName().Name;
        public override string Author => "VALERA771#1471";
        public override Version Version => Assembly.GetName().Version;
        public override Version RequiredExiledVersion => new(6, 0, 0);

        public static ISerializer Serializer { get; private set; }
        public static IDeserializer Deserializer { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;

            ExampleUnit = Path.Combine(Config.UnitPath, "ExampleUnit.yml");

            if (!Directory.Exists(Config.UnitPath))
            {
                Directory.CreateDirectory(Instance.Config.UnitPath);
                Log.Info("Created directory for plugin");
            }

            Events = new EventHadlers();
            RegisterEvents();

            Serializer = Loader.Serializer;

            Deserializer = Loader.Deserializer;

            LoadUnitConfig();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            UnregisterEvents();

            Events = null;

            Soldiers.Clear();
            Configs.Clear();
            Chance.Clear();
            Tickets.Clear();

            Serializer = null;
            Deserializer = null;

            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            Player.Died += Events.OnDied;
            Warhead.Starting += Methods.AddChance;
            Warhead.Stopping += Methods.AddChance;
            Warhead.Detonating += Methods.AddChance;
            Player.ActivatingGenerator += Methods.AddChance;
            Player.Escaping += Methods.AddChance;
            Player.Shot += Events.OnShooting;
            Map.RespawningTeam += Events.OnTeamChoose;
        }

        public void UnregisterEvents()
        {
            Player.Died -= Events.OnDied;
            Warhead.Starting -= Methods.AddChance;
            Warhead.Stopping -= Methods.AddChance;
            Warhead.Detonating -= Methods.AddChance;
            Player.ActivatingGenerator -= Methods.AddChance;
            Player.Escaping -= Methods.AddChance;
            Player.Shot -= Events.OnShooting;
            Map.RespawningTeam -= Events.OnTeamChoose;
        }

        public Plugin()
        {
        }

        public static Dictionary<Exiled.API.Features.Player, string> Soldiers { get; set; } = new();
        public static EventHadlers Events { get; private set; }

        public static Dictionary<string, Unit> Configs { get; set; } = new();
        public static HashSet<Unit> Chance { get; set; } = new();
        public static Dictionary<Unit, int> Tickets { get; set; } = new ();


        public static void LoadUnitConfig()
        {
            if (!File.Exists(ExampleUnit))
                File.WriteAllText(ExampleUnit, Serializer.Serialize(UnitConfig));

            foreach (var file in Directory.GetFiles(Instance.Config.UnitPath))
            {
                int i = 0;
                try
                {
                    var conf = Deserializer.Deserialize<Unit>(File.ReadAllText(file));
                    if (conf != null)
                        Configs.Add(conf.UnitName, conf);
                    else
                    {
                        File.WriteAllText(file, Serializer.Serialize(UnitConfig));
                        Configs.Add($"UnitName{i}", Deserializer.Deserialize<Unit>(File.ReadAllText(file)));
                        i++;
                    }
                }
                catch (YamlException ex)
                {
                    Log.Error($"Error while deserializing {file} file. Skipping...\nError: {ex.Message}. For more info enable debug");
                    Log.Debug(ex.StackTrace);
                }
                catch (Exception ex)
                {
                    Log.Error($"Unhandled error occured while deserializing file {file}.\nError: {ex.Message}. For more info enable debug");
                    Log.Debug(ex.StackTrace);
                }
            }

            Chance = Configs.Values.ToHashSet();

            Chance = Configs.Values.Where(x => x.UseChance).ToHashSet();
            var tmp = Configs.Values.Where(x => !x.UseChance).ToList();

            foreach (var el in tmp)
            {
                bool leave = false;
                foreach (var eventType in el.Events.Keys)
                {
                    if (!Options.Events.ContainsValue(eventType))
                    {
                        Log.Error($"Unit {el.UnitName} param 'events' contains wrong/not supported events Skipping...\nCheck readme on github or contact developer for more information.");
                        leave = true;
                        break;
                    }
                }

                if (leave)
                    continue;

                Tickets.Add(el, el.StartTicket);
            }
        }
    }
}
