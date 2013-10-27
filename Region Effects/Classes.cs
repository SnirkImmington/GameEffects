using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace RegionEffects
{
    class EffectsRegion
    {
        public string RegionName { get; set; }
        public string EffectsText { get; set; }
        public List<RegionPart> Things { get; set; }
        public List<string> ExternalCommands { get; set; }

        public void Affect(Player ply)
        {
            foreach (var part in Things)
            {
                if (part.Target.Affects(ply, part.TargeterArg))
                    foreach (var eff in part.Effects)
                        eff.Value.Effect.Invoke(new RegionEffectArgs(ply, eff.Key, false));
            }
        }
    }

    class RegionPart
    {
        public Targeter Target { get; set; }
        public string TargeterArg { get; set; }
        public Dictionary<string, RegionEffect> Effects { get; set; }
    }

    /// <summary>
    /// Provides APIstyle targeting ability for region effects.
    /// <example> An example PvP-tracker.
    /// Keyword = "pvp"
    /// Targets = (Player p, string s) => p.hostile == bool.Parse(s)
    /// So a region with effect "@pvp:true= ..." would target PvP players.
    /// </example>
    /// </summary>
    public class Targeter
    {
        /// <summary>
        /// The keyword to be listening for.
        /// </summary>
        public string Keyword { get; private set; }
        /// <summary>
        /// The test for effect, given the player and argument.
        /// </summary>
        public Func<TSPlayer, string, bool> Targets { get; private set; }

        public Func<string, bool> ArgumentValidator { get; private set; }

        /// <summary>
        /// Constructor for the API and 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="function"></param>
        public Targeter (string name, Func<TSPlayer, string, bool> function)
        {
            Keyword = name; Targets = function; ArgumentValidator = s => true;
        }

        public Targeter(string name, Func<TSPlayer, string, bool> function, Func<string, bool> validator)
            : this(name, function)
        {
            ArgumentValidator = validator;
        }

        public bool Affects(TSPlayer ply, string argument)
        {
            return Targets.Invoke(ply, argument);
        }
    }

    /// <summary>
    /// Contains data for methods referring to the effects of
    /// EffectsReions.
    /// </summary>
    public class RegionEffectArgs
    {
        /// <summary>
        /// The player affected.
        /// </summary>
        public Player Player { get; set; }
        /// <summary>
        /// The argument to the call.
        /// </summary>
        public string Argument { get; set; }
        /// <summary>
        /// Whether this is applied the first time.
        /// </summary>
        public bool IsEntered { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegionEffectArgs(Player player, string arg, bool entered)
        { Player = player; Argument = arg; IsEntered = entered; }
    }

    /// <summary>
    /// Creates a possible effect on a region.
    /// </summary>
    public class RegionEffect
    {
        /// <summary>
        /// The name of the effect (i.e. buff).
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Whether this can be called only when someone has entered.
        /// </summary>
        public bool OnlyOnEnter { get; set; }
        /// <summary>
        /// The function for the player and arguments that will be applied.
        /// </summary>
        public Action<RegionEffectArgs> Effect { get; set; }
        /// <summary>
        /// The parameters for the EffectRegion.
        /// </summary>
        public string SavedParameters { get; internal set; }

        public RegionEffect(string keyword, Action<RegionEffectArgs> effect)
        { Keyword = keyword; Effect = effect; }
    }

    /// <summary>
    /// Contains public API functions and methods for external use.
    /// </summary>
    public static class API
    {
        public List<Targeter>   Targeters = new List<Targeter>();
        public List<RegionEffect> Effects = new List<RegionEffect>();

        internal void Initialize()
        {
            Targeters.AddRange(new Targeter[] {

                #region PvP targeter
                new Targeter("pvp", (TSPlayer p, string s) =>
                    {
                        bool target = false;
                        switch (s)
                        {
                            case "true": case "on": 
                            case "enabled": case "hostile":
                            target = true; break;
                        }
                        return p.TPlayer.hostile == target;
                    }),
                #endregion

                #region Team targeter
                new Targeter("team", (TSPlayer p, string s) =>
                    {
                        Func<string, bool> target = (string b) => s[0] == b[0] || s == b;
                        int reqTeam = 0;

                        if (target("red")) reqTeam = 1;
                        else if (target("green")) reqTeam = 2;
                        else if (target("blue")) reqTeam = 3;
                        else if (target("yellow")) reqTeam = 4;

                        return p.Team == reqTeam;

                    }),
                #endregion

                #region Permission Targeter
                new Targeter("perm", (TSPlayer p, string s) =>
                    {
                        return p.Group.HasPermission(s);
                    }),
                #endregion

                #region Item Targeter
                new Targeter("item", (TSPlayer p, string s) =>
                    {
                        // Determine if it's !item
                        bool notItem = s[0] == '!';
                        if (notItem) s = s.Substring(1);

                        // Get items (checked with validator)
                        var posItems = TShockAPI.TShock.Utils.GetItemByIdOrName(s);

                        // Construct inventory items
                        List<Item> inventory = new List<Item>(p.TPlayer.inventory);
                        inventory.AddRange(p.TPlayer.armor); inventory.AddRange(p.TPlayer.dye);

                        // XNOR is ==, mind is blown
                        return notItem == inventory.Contains(posItems[0]);
                    }, 
                    // Matched item or [0] == '!' and matched item with substring(1).
                    s => TShock.Utils.GetItemByIdOrName(s).Count == 1 || 
                        (s[0] == '!' && TShock.Utils.GetItemByIdOrName(s.Substring(1)).Count == 1)),
                #endregion

            });

            Effects.AddRange(new RegionEffect[] {

                #region Buff Effect
                #endregion

            });
        }
    }
}
