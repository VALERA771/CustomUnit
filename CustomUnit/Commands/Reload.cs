using CommandSystem;
using System;

namespace CustomUnit.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Reload : ICommand
    {
        public string Command => "reload_units";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Reloads units files";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("cu.reload", out response))
                return false;

            try
            {
                Plugin.Configs.Clear();
                Plugin.Tickets.Clear();
                Plugin.Chance.Clear();

                Plugin.LoadUnitConfig();
            }
            catch (Exception ex)
            {
                response = $"{ex.Message}\n{ex.StackTrace}";
                return false;
            }

            response = "Teams configs have been reloaded successfully";
            return true;
        }
    }
}
