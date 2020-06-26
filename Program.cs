using System;
using System.IO;

namespace DCSMissionTweaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: " +
                    Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location) +
                    " <inputmissionfile.miz> <numberofclients>");
                return;
            }
            Mission mission = MissionParser.ParseFile(args[0]);
            Container playerUnit = mission.SearchPlayerUnit();
            Attribute playerType = playerUnit.SearchAttribute("type");
            Container units = playerUnit.parent;
            Console.WriteLine("Current player unit type is " + playerType.value);
            Console.WriteLine("Player is unit " + playerUnit.name + " of " + units.children.Count);
            foreach (Container unit in units.children)
            {
                if (unit == playerUnit)
                    continue;
                Console.WriteLine("Unit " + unit.name + " in group has skill " + unit.SearchAttribute("skill").value);
            }
            int numClientsRequested = int.Parse(args[1]);
            int numClients = Math.Min(numClientsRequested, units.children.Count);
            Console.WriteLine("Converting " + numClients + " units of the group to clients.");
            for(int i=1; i<=numClients; i++)
            {
                Container unit = units.children.Find(u => u.name.Equals(i) && u is Container) as Container;
                Attribute skill = unit.SearchAttribute("skill");
                skill.value = "Client";
            }

            MissionSaver.Save(args[0], mission);
        }
    }
}
