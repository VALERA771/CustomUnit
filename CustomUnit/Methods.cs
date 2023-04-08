using System.Linq;
using CustomUnit.EventOptions;
using Exiled.Events.EventArgs.Interfaces;

namespace CustomUnit;

public abstract class Methods
{
    public static void AddChance(IExiledEvent ev)
    {
        foreach (var unit in Plugin.Tickets.Where(unit => unit.Key.Events.ContainsKey(Options.Events[ev.GetType()])))
            Plugin.Tickets[unit.Key] += unit.Key.Events[Options.Events[ev.GetType()]];
    }
}