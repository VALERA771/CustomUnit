using CustomUnit.EventOptions;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using PlayerRoles;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUnit.Configs
{
    public sealed class Plugin : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string UnitPath { get; set; } = Path.Combine(Paths.Exiled, "Units");
    }
}
