using System;
using CustomUnit.Configs;
using Exiled.API.Features;
using Exiled.Loader;
using Exiled.Loader.Features.Configs.CustomConverters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomUnit.EventOptions;
using Exiled.Events.Handlers;
using Exiled.Loader.Features.Configs;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Map = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using YamlDotNet.Serialization.NodeDeserializers;
using Version = System.Version;
using Warhead = Exiled.Events.Handlers.Warhead;

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

        public static ISerializer Serializer => new SerializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            .WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();
        public static IDeserializer Deserializer => new DeserializerBuilder().WithTypeConverter(new VectorsConverter()).WithTypeConverter(new ColorConverter()).WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), delegate (ITrackingRegistrationLocationSelectionSyntax<INodeDeserializer> deserializer)
            {
                deserializer.InsteadOf<ObjectNodeDeserializer>();
            })
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        public override void OnEnabled()
        {
            Instance = this;

            if (!Directory.Exists(Instance.Config.UnitPath))
            {
                Directory.CreateDirectory(Instance.Config.UnitPath);
                Log.Info("Created directory for plugin");
            }

            _event = new EventHadlers();
            
            RegisterEvents();

            base.OnEnabled();

            LoadUnitConfig();
        }

        public override void OnDisabled()
        {
            Instance = null;

            UnregisterEvents();

            _event = null;
            base.OnDisabled();

            Soldiers.Clear();
            Configs.Clear();
            Chance.Clear();
            Tickets.Clear();
        }

        public void RegisterEvents()
        {
            Player.Died += _event.OnDied;
            Warhead.Starting += Methods.AddChance;
            Warhead.Stopping += Methods.AddChance;
            Warhead.Detonating += Methods.AddChance;
            Player.ActivatingGenerator += Methods.AddChance;
            Player.Escaping += Methods.AddChance;
            Player.Shot += _event.OnShooting;
            Map.RespawningTeam += _event.OnTeamChoose;
        }

        public void UnregisterEvents()
        {
            Player.Died -= _event.OnDied;
            Warhead.Starting -= Methods.AddChance;
            Warhead.Stopping -= Methods.AddChance;
            Warhead.Detonating -= Methods.AddChance;
            Player.ActivatingGenerator -= Methods.AddChance;
            Player.Escaping -= Methods.AddChance;
            Player.Shot -= _event.OnShooting;
            Map.RespawningTeam -= _event.OnTeamChoose;
        }

        public Plugin()
        {
        }

        public static Dictionary<Exiled.API.Features.Player, string> Soldiers = new Dictionary<Exiled.API.Features.Player, string>();
        public static EventHadlers _event;

        public static Dictionary<string, Unit> Configs = new Dictionary<string, Unit>();
        public static HashSet<Unit> Chance = new HashSet<Unit>();
        public static Dictionary<Unit, int> Tickets = new Dictionary<Unit, int>();


        public static void LoadUnitConfig()
        {
            if (!File.Exists(ExampleUnit))
                File.WriteAllText(ExampleUnit, Serializer.Serialize(UnitConfig));

            foreach (var file in Directory.GetFiles(Instance.Config.UnitPath))
            {
                try
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
                        Log.Error($"Unit {el.UnitName} param 'events' contains wrong/not supported events. Check readme on github or contact developer.");
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
