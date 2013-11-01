using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;

namespace GameEffects
{
    /// <summary>
    /// Contains API info for Targeters. 
    /// Targeters are used at the beginning of Effect Chains to determine if an effect will target a player.
    /// <example>The targeter "pvp" takes an argument of on/off (uses API.PvPParse) and will
    /// only target players whos PvP state matches the one specified.</example>
    /// </summary>
    public class TargeterInfo : INameDescripted
    {
        /// <summary>
        /// The name of the Targeter to be invoked in command chains.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// A description of the targeter, for /effect list targeters.
        /// Should be a one-line quick overview.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// The usage of the targeter, to be listed in /effects help targeter [name].
        /// Can be multiline and should go into details.
        /// </summary>
        public string HelpText { get; private set; }

        /// <summary>
        /// The function called to determine if a player is targeted, taking a PlayerParamArgs.
        /// Precondidtion: The input is checked with the <paramref name="paramValidator"/>.
        /// </summary>
        public Func<PlayerParamArgs, bool> Function { get; private set; }
        /// <summary>
        /// A function that determines if input for a player is valid.
        /// Called when a host is created.
        /// For example, return false if an input cannot be parsed into an int.
        /// </summary>
        public Func<string, bool> ParamValidator { get; private set; }

        /// <summary>
        /// Creates a TargeterInfo for the API, with name, displays, and a function to check players.
        /// </summary>
        /// <param name="name">The name of the Targeter (i.e. isonline)</param>
        /// <param name="description">A description of the targeter, for /effect list targeters.
        /// Should be a one-line quick overview.</param>
        /// <param name="helpText">The usage of the targeter, to be listed in /effects help targeter [name].
        /// Can be multiline and should go into details.</param>
        /// <param name="function">The function called to determine if a player is targeted, taking a PlayerParamArgs.
        /// Precondidtion: The input is checked with the <paramref name="paramValidator"/>.</param>
        public TargeterInfo(string name, string description, string helpText, Func<PlayerParamArgs> function)
        {
            Name = name; Description = description; HelpText = helpText; 
            Function = function; ParamValidator = s => true;
        }

        /// <summary>
        /// Creates a TargeterInfo for the API, with name, displays, a function (bool) to check players,
        /// and a function (bool) to check input arguments to the Targeter when it is created.
        /// </summary>
        /// <param name="name">The name of the Targeter (i.e. isonline)</param>
        /// <param name="description">A description of the targeter, for targeters.
        /// Should be a one-line quick overview.</param>
        /// <param name="helpText">The usage of the targeter, to be listed in /effects help targeter [name].
        /// Can be multiline and should go into details.</param>
        /// <param name="function">The function called to determine if a player is targeted, taking a PlayerParamArgs.
        /// Precondidtion: The input is checked with the <paramref name="paramValidator"/>.</param>
        /// <param name="paramvalidator">The function called to determine if a given input is valid</param>
        public TargeterInfo(string name, string description, string helpText, 
            Func<PlayerParamArgs> function, Func<string, bool> paramvalidator)
            : this(name, description, helpText, function) { ParamValidator = paramvalidator; }
    }

    /// <summary>
    /// Contains API info for Effects.
    /// Effects are methods executed by the EffectHost when it is triggered by a player who satisfies the Targeters.
    /// <example>The "give" effect takes an item as an arguement, so "give(excal 1 legendary)" will give the player one Legendary Excalibur.</example>
    /// </summary>
    public class EffectInfo : INameDescripted
    {
        /// <summary>
        /// The name of the Effect to be invoked in command chains.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// A description of the effect, for /effect list effects.
        /// Should be a one-line quick overview.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// The effects of the effect, to be used in /effects help effect.
        /// Can be multiline and should go into details.
        /// </summary>
        public string HelpText { get; private set; }
        /// <summary>
        /// Set this to true to prevent anyone without this plugin's
        /// MakeDangerousGameEffectsPermission from creating this effect.
        /// </summary>
        public bool IsDangerous { get; private set; }

        /// <summary>
        /// The method that is called when the event is triggered.
        /// </summary>
        public Action<PlayerParamArgs> Action { get; set; }
        /// <summary>
        /// An optional method to determine if input is valid. Called when the effect is defined by a player.
        /// </summary>
        public Func<string, bool> ParamValidator { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isDangerous"></param>
        /// <param name="description"></param>
        /// <param name="helpText"></param>
        /// <param name="action"></param>
        public EffectInfo(string name, bool isDangerous, string description, 
            string helpText, Action<PlayerParamArgs> action)
        {
            Name = name; Description = description; HelpText = helpText; 
            IsDangerous = isDangerous; Action = action;
            ParamValidator = s => true;
        }

        public EffectInfo(string name, bool isDangerous, string description,
            string helpText, Action<PlayerParamArgs> action, Func<string, bool> paramValidator)
            : this(name, isDangerous, description, helpText, action) { ParamValidator = paramValidator; }
    }

    public class VariableInfo
    {
    }

    public class HostInfo
    {
    }

    public interface INameDescripted
    {
        string Name; string Description; string HelpText;
    }

    /// <summary>
    /// Contains public API functions and methods for external use.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Add any new Targeters your plugin adds as TargeterInfos to this list.
        /// </summary>
        public static List<Targeter>   Targeters = new List<Targeter>();
        public static List<RegionEffect> Effects = new List<RegionEffect>();
        public static List<Variable>   Variables = new List<Variable>();

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
                new RegionEffect("ban", "Disables the player from using item", e =>
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

            Variables.AddRange(new Variable[] {
                new Variable("ply", a => a.Player.Player.name),
                //new Variable("
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

        public static void ParseString(string inp)
        {
            string str = "[a [b [c [d value]]]]";

        while (str.Trim().Length > 0)
        {
            int start = str.LastIndexOf('[');
            int end = str.IndexOf(']');

            string s = str.Substring(start +1, end - (start+1)).Trim();
            string[] pair = s.Split(' ');// this is what you are looking for. its length will be 2 if it has a value

            str = str.Remove(start, (end + 1)- start);
        }
        }
    }
}
