using CustomUnit.Configs;
using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using Map = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace CustomUnit
{
    public class Plugin : Plugin<Configs.Plugin>
    {
        public static Plugin Instance { get; private set; }
        public static Unit UnitConfig => new Unit();
        public static string ExampleUnit => Path.Combine(Instance.Config.UnitPath, "ExampleUnit.yml");

        public override string Name => "Custom Unit";
        public override string Prefix => "CustomUnit";
        public override string Author => "VALERA771#1471";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);

        public static ISerializer Serializer => new Serializer();
        public static IDeserializer Deserializer => new Deserializer();

        public override void OnEnabled()
        {
            Instance = this;

            if (!Directory.Exists(Instance.Config.UnitPath))
            {
                Directory.CreateDirectory(Instance.Config.UnitPath);
                Log.Info("Created directory for plugin");
            }

            _event = new EventHadlers();
            Map.RespawningTeam += _event.OnTeamChoose;
            Player.Died += _event.OnDied;
            Player.Shot += _event.OnShooting;
            base.OnEnabled();

            LoadUnitConfig();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Map.RespawningTeam -= _event.OnTeamChoose;
            Player.Died -= _event.OnDied;
            Player.Shot -= _event.OnShooting;

            _event = null;
            base.OnDisabled();

            Soldiers.Clear();
            Configs.Clear();
            Chance.Clear();
            Tickets.Clear();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
        }

        public Plugin()
        {
        }

        public static Dictionary<Exiled.API.Features.Player, string> Soldiers = new Dictionary<Exiled.API.Features.Player, string>();
        public static EventHadlers _event;

        public static Dictionary<string, Unit> Configs = new Dictionary<string, Unit>();
        public static HashSet<Unit> Chance = new HashSet<Unit>();
        public static Dictionary<Unit, uint> Tickets = new Dictionary<Unit, uint>();


        public static void LoadUnitConfig()
        {
            if (!File.Exists(ExampleUnit))
                File.WriteAllText(ExampleUnit, Serializer.Serialize(UnitConfig));

            foreach (var file in Directory.GetFiles(Instance.Config.UnitPath))
            {
                var conf = Deserializer.Deserialize<Unit>(File.ReadAllText(file));
                if (conf != null)
                    Configs.Add(conf.UnitName, conf);
                else
                {
                    File.WriteAllText(file, Serializer.Serialize(UnitConfig));
                    Configs.Add("UnitName", Deserializer.Deserialize<Unit>(File.ReadAllText(file)));
                }
            }

            Chance = Configs.Values.ToHashSet();

            /*Chance = Configs.Values.Where(x => x.UseChance).ToHashSet();
            var tmp = Configs.Values.Where(x => !x.UseChance).ToList();

            foreach (var el in tmp)
                Tickets.Add(el, 0);*/
        }
    }
}
