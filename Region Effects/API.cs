using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;

namespace RegionEffects
{
    /// <summary>
    /// Contains public API functions and methods for external use.
    /// </summary>
    public static class API
    {
        public static List<Targeter>   Targeters = new List<Targeter>();
        public static List<RegionEffect> Effects = new List<RegionEffect>();

        #region GetRegions

        public static List<EffectsRegion> GetRegions(int x, int y)
        {
        }

        public static List<EffectsRegion> GetRegions(Vector2 pos)
        {
        }

        public static List<EffectsRegion> GetRegions(TShockAPI.DB.Region reg)
        {
        }

        public static List<EffectsRegion> GetRegions(string regionName)
        {
        }

        #endregion

        internal static void Initialize()
        {
            Targeters.AddRange(new Targeter[] {

                #region PvP targeter
                new Targeter("pvp", "Target players by their PvP stat", (RegionPlayer p, string s) =>
                    {
                        bool target = false;
                        switch (s)
                        {
                            case "true": case "on": 
                            case "enabled": case "hostile":
                            target = true; break;
                        }
                        return p.Player.hostile == target;
                    }),
                #endregion

                #region Team targeter
                new Targeter("team", "Target players based on team", (RegionPlayer p, string s) =>
                    {
                        Func<string, bool> target = (string b) => s[0] == b[0] || s == b;
                        int reqTeam = 0;

                        if (target("red")) reqTeam = 1;
                        else if (target("green")) reqTeam = 2;
                        else if (target("blue")) reqTeam = 3;
                        else if (target("yellow")) reqTeam = 4;

                        return p.Player.team == reqTeam;

                    }),
                #endregion

                #region Permission Targeter
                new Targeter("perm", "Target players by TShock group permissions", (RegionPlayer p, string s) =>
                    {
                        return p.TSPlayer.Group.HasPermission(s);
                    }),
                #endregion

                #region Item Targeter
                new Targeter("item", "Target players by if they have an item (id or name), or \"!item\" for if they don't", (RegionPlayer p, string s) =>
                    {
                        // Determine if it's !item
                        bool notItem = s[0] == '!';
                        if (notItem) s = s.Substring(1);

                        // Get items (checked with validator)
                        var posItems = TShockAPI.TShock.Utils.GetItemByIdOrName(s);

                        // Construct inventory items
                        List<Item> inventory = new List<Item>(p.Player.inventory);
                        inventory.AddRange(p.Player.armor); inventory.AddRange(p.Player.dye);

                        // XNOR is ==, mind is blown
                        return notItem == inventory.Contains(posItems[0]);
                    }, 
                    // Matched item or [0] == '!' and matched item with substring(1).
                    s => TShock.Utils.GetItemByIdOrName(s).Count == 1 || 
                        (s[0] == '!' && TShock.Utils.GetItemByIdOrName(s.Substring(1)).Count == 1)),
                #endregion

                #region Damage Targeter
                new Targeter("damage", "Target players with \"<damage\" or \">damage\"", (RegionPlayer p, string a) =>
                    {
                        Func<int, bool> valid; int val = int.Parse(a.Substring(1));

                        if (a[0] == '>') valid = i => i > val;
                        else valid = i => i < val;

                        for (int i = 0; i < p.Player.inventory.Length; i++)
                        {
                            if (valid(p.Player.inventory[i].damage)) return true;
                        }
                        return false;

                    }, DamageValidator),
                #endregion

                #region Defense Targeter
                new Targeter("defense", "Target players with \"<defense\" or \">defense\"", (RegionPlayer p, string a) =>
                    {
                        Func<int, bool> valid; int val = int.Parse(a.Substring(1));

                        if (a[0] == '>') valid = i => i > val;
                        else valid = i => i < val;

                        return valid(p.Player.statDefense);

                    }, DamageValidator),
                #endregion

                #region Token Targeter
                new Targeter("token", "Target players with a token", (RegionPlayer p, string a) => p.Tokens.Contains(a)),
                #endregion
            });

            Effects.AddRange(new RegionEffect[] {

                // Buff Effect
                new RegionEffect("buff", "Buffs the player using buff syntax", BuffImplementation, false, BuffValidator),

                #region Banned Item Effect
                new RegionEffect("ban", e =>
                    {
                        var inv = e.Player.Player.inventory.ToList();
                        inv.AddRange(e.Player.Player.armor);
                        inv.AddRange(e.Player.Player.dye);
                        
                        // TODOD Only apply for holding.

                        var item = TShock.Utils.GetItemByIdOrName(e.Argument)[0];
                        if (inv.Any(i => i.netID == item.netID))
                        {
                            e.Player.TSPlayer.Disable("Your item, " + item.name + ", is banned in this region! You may not have it.");
                        }

                    }, false, i => TShock.Utils.GetItemByIdOrName(i).Count == 1),
                #endregion

                // Kick from regions
                new RegionEffect("kick", "Attempts to teleport the user to where they were before", KickImplementation),

                #region Give
                new RegionEffect("give", "Gives the player an item", e =>
                    {
                        Commands.HandleCommand(TSPlayer.Server, "give \"" + e.Argument + "\" \"" + e.Player.Player.name + "\"");

                    }, true, s => TShock.Utils.GetItemByIdOrName(s).Count == 1),
                #endregion

                #region Warp
                new RegionEffect("warp", "Sends the player to a warp", e =>
                    {
                        var warp = TShock.Warps.FindWarp(e.Argument);
                        if (e.Player.TSPlayer.Teleport(warp.WarpPos.X*16, warp.WarpPos.Y*16))
                            e.Player.TSPlayer.SendSuccessMessage("The region has teleported you to " + warp.WarpName + "!");
                    }, true, s => TShock.Warps.FindWarp(s).WarpPos != Vector2.Zero),
                #endregion

                #region Teleport
                new RegionEffect("tp", "Teleports player to x,y coordinates", e => 
                    {
                        var text = e.Argument.Split(',').ToList();
                        var coords = text.ConvertAll(s => int.Parse(s.Trim()));

                        e.Player.TSPlayer.Teleport(coords[0], coords[1]);
                        e.Player.TSPlayer.SendSuccessMessage("The region teleported you to " + e.Argument + "!");
                    }, true, WarpValidator),
                #endregion

                #region Permissions
                new RegionEffect("perm", "Gives the player additional permissions",
                    PermImplementation),
                #endregion

                #region Various Commands
                new RegionEffect("cmd", "The server executes command on enter",  e => CmdImplementation(e, TSPlayer.Server), true, CmdValidator),
                new RegionEffect("ccmd", "The server executes command constantly", e => CmdImplementation(e, TSPlayer.Server), false, CmdValidator),
                new RegionEffect("fcmd", "The player executes command on enter", e => CmdImplementation(e, e.Player.TSPlayer), true, CmdValidator),
                new RegionEffect("fccmd", "The player executes command constantly", e=> CmdImplementation(e, e.Player.TSPlayer), false, CmdValidator),
                #endregion

                #region Force PvP
                #endregion

                #region Force Team
                #endregion

                #region Mod Tokens
                new RegionEffect("addtoken", "Gives the player that token.", s => s.Player.Tokens.Add(s.Argument), true),
                new RegionEffect("deltoken", "Removes a token from the player", s => s.Player.Tokens.Remove(s.Argument), true),
                #endregion
            });
        }

        #region Implementations

        private static void KickImplementation(RegionEffectArgs e)
        {
            var lastRegions = API.GetRegions(e.Player.LastPosition);
            bool needToDefault = false;

            // Determine if the player used to be in a region.
            if (lastRegions.Count != 0)
                foreach (var reg in lastRegions)
                    if (reg.Things.Any(t => t.Targeter.Affects(e.Player) &&
                        t.Effects.Any(f => f.Name == "kick")))
                    { needToDefault = true; break; }

            // If they used to be in a kick region, send them to their spawn point.
            if (needToDefault)
            {
                e.Player.TSPlayer.Spawn();
                e.Player.TSPlayer.SendWarningMessage("You are not allowed in that region" + (string.IsNullOrEmpty(e.Argument) ? "!" : " - " + e.Argument));
                e.Player.TSPlayer.SendErrorMessage("Unable to send you to your last position, sent you to your spawn point.");
            }
            else // It's ok to send them to their last point
            {
                e.Player.TSPlayer.Teleport(e.Player.LastPosition.X, e.Player.LastPosition.Y);
                e.Player.TSPlayer.SendWarningMessage("You are not allowed in that region" + (string.IsNullOrEmpty(e.Argument) ? "!" : " - " + e.Argument));
                e.Player.TSPlayer.SendSuccessMessage("Teleported you to your last position.");
            }
        }

        private static void BuffImplementation(RegionEffectArgs e)
        {
        }
        private static bool BuffValidator(string input)
        {
            // input: +well_fed, -shadow, %well

            var splitComma = input.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder buffBuilder = new StringBuilder();
            for (int i = 0; i < splitComma.Length; i++)
            {
                // splitComma[i] = " +well_fed" //

                // Trim the start and end
                splitComma[i] = splitComma[i].Trim();

                // Determine if it's set up properly
                switch (splitComma[i][0])
                {
                    case '+':
                    case '-':
                    case '~': break;

                    default:
                        if (char.IsLetter(splitComma[i][0]))
                        {
                            splitComma[i] = splitComma[i].Substring(1); break;
                        }
                        return false;
                }

                // Test the buff string, get the buff name.

                if (TShock.Utils.GetBuffByName(splitComma[i]).Count != 1) return false;
            }

            // All of the buffs worked.
            return true;
        }

        private static bool DamageValidator(string input)
        {
            int val; return input != null && input[0] == '>' || input[0] == '<'
                && int.TryParse(input, out val);
        }

        private static void CmdImplementation(RegionEffectArgs e, TSPlayer ply)
        {
            Commands.HandleCommand(ply, e.Argument.Replace("%ply%", '"'+e.Player.Player.name+'"'));
        }
        private static bool CmdValidator(string input)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != ' ') sb.Append(input[i]);

                else break;
            }

            if (Commands.ChatCommands.Any(c => c.Names.Contains(sb.ToString()))) return true;

            return false;

        }

        private static bool WarpValidator(string input)
        {
            var texts = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (texts.Length != 2) return false;
            int x, y;

            if (int.TryParse(texts[0].Trim(), out x) && int.TryParse(texts[0].Trim(), out y))
            {
                return (x > 0 && x < Main.maxTilesX) && (y > 0 && y < Main.maxTilesY);
            }
            return false;
        }

        private static void PermImplementation(RegionEffectArgs e)
        {
            var perms = e.Argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < perms.Length; i++)
            {
                perms[i] = perms[i].Trim();
            }
        }

        #endregion

        public static RegionPlayer RPlayer(this TSPlayer ply)
        {
            return PluginMain.Players.FirstOrDefault(p => p.Index == ply.Index);
        }
    }
}
