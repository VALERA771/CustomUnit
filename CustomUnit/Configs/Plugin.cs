using Exiled.API.Features;
using Exiled.API.Interfaces;
using System.IO;

namespace CustomUnit.Configs
{
    public sealed class Plugin : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string UnitPath { get; set; } = Path.Combine(Paths.Exiled, "Units");
    }
}
