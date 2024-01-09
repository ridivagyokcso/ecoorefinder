using System;
using System.IO;
using Eco.Core;
using Eco.Core.Utils;
using Eco.Shared.Utils;
using Eco.World;
using Eco.Mods.TechTree;
using Eco.Shared.Math;
using Eco.World.Blocks;
using Eco.Gameplay.Systems.Messaging.Chat;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Notifications;
using Eco.Shared.Localization;

namespace EcoOreFinder
{
    [ChatCommandHandler]
    public class EcoOreFinder
    {
        [ChatCommand("Search gold", ChatAuthorizationLevel.User)]
        public static void sau(User user)
        {
            searchBlockChat(user, 100, "gold", typeof(GoldOreBlock));
        }

        [ChatCommand("Search copper", ChatAuthorizationLevel.User)]
        public static void scu(User user)
        {
            searchBlockChat(user, 100, "copper", typeof(CopperOreBlock));
        }

        [ChatCommand("Search iron", ChatAuthorizationLevel.User)]
        public static void sfe(User user)
        {
            searchBlockChat(user, 100, "iron", typeof(IronOreBlock));
        }

        [ChatCommand("Search coal", ChatAuthorizationLevel.User)]
        public static void sc(User user)
        {
            searchBlockChat(user, 100, "coal", typeof(CoalBlock));
        }

        static void searchBlockChat(User user, int range, string name, Type type)
        {
            Vector3i userPos = new Vector3i();
            userPos.X = (int) user.Position.X;
            userPos.Y = (int) user.Position.Y;
            userPos.Z = (int) user.Position.Z;

            LocString lcstring = new LocString("Search " + name + ", starting at user's location " + userPos.ToString());
            NotificationManager.ServerMessageToPlayer(lcstring, user);

            Vector3i? blockPos = searchBlock(userPos, range, type);
            if (blockPos == null)
            {
                lcstring = new LocString("No " + name + " found within " + range + " blocks of user.");
                NotificationManager.ServerMessageToPlayer(lcstring, user);
            }
            else
            {
                lcstring = new LocString("Nearest " + name + " found at " + blockPos.ToString());
                NotificationManager.ServerMessageToPlayer(lcstring, user);

                Vector3i rel = (Vector3i)blockPos - userPos;

                lcstring = new LocString("Relative " + rel.x + ", " + rel.y + ", " + rel.z);
                NotificationManager.ServerMessageToPlayer(lcstring, user);
                user.Player.DropExactWaypoint((Vector3i)blockPos, name);
            }
        }

        static Vector3i? searchBlock(Vector3i startPos, int range, Type type)
        {
            Vector3i s = startPos;
            // search the nearest block of the target class, cubically.
            for (int radius = 1; radius <= range; radius++)
            {
                // Search the shell with the current radius
                int r = radius;
                // X-sides, big on both Y and Z.
                int x = s.x + r, y, z;
                for (y = s.y - r; y <= s.y + r; y++)
                {
                    for (z = s.z - r; z <= s.z + r; z++)
                    {
                        Vector3i p = new Vector3i(x, y, z);
                        if (testBlock(p, type))
                            return p;
                    }
                }
                x = s.x - r;
                for (y = s.y - r; y <= s.y + r; y++)
                {
                    for (z = s.z - r; z <= s.z + r; z++)
                    {
                        Vector3i p = new Vector3i(x, y, z);
                        if (testBlock(p, type))
                            return p;
                    }
                }

                // Y-sides, small X, big Z.
                y = s.y + r;
                for (x = s.x + 1 - r; x < s.x + r; x++)
                {
                    for (z = s.z - r; z <= s.z + r; z++)
                    {
                        Vector3i p = new Vector3i(x, y, z);
                        if (testBlock(p, type))
                            return p;
                    }
                }
                y = s.y - r;
                for (x = s.x + 1 - r; x < s.x + r; x++)
                {
                    for (z = s.z - r; z <= s.z + r; z++)
                    {
                        Vector3i p = new Vector3i(x, y, z);
                        if (testBlock(p, type))
                            return p;
                    }
                }

                // Z-sides, small X and Y.
                z = s.z + r;
                for (x = s.x + 1 - r; x < s.x + r; x++)
                {
                    for (y = s.y + 1 - r; y < s.y + r; y++)
                    {
                        Vector3i p = new Vector3i(x, y, z);
                        if (testBlock(p, type))
                            return p;
                    }
                }
                z = s.z - r;
                for (x = s.x + 1 - r; x < s.x + r; x++)
                {
                    for (y = s.y + 1 - r; y < s.y + r; y++)
                    {
                        Vector3i p = new Vector3i(x, y, z);
                        if (testBlock(p, type))
                            return p;
                    }
                }
            }
            return null;
        }

        static bool testBlock(Vector3i pos, Type type)
        {
            // Test a block.
            Block b = World.GetBlock(pos);
            if (b.GetType().IsEquivalentTo(type))
            {
                return true;
            }
            return false;
        }
    }
}
