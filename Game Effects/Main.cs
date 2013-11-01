using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using TShockAPI.Hooks;
namespace GameEffects
{
    [ApiVersion(1,14)]
    public class PluginMain : TerrariaPlugin
    {
        #region Variables
        internal static List<EffectPlayer> Players = new List<RegionPlayer>();
        #endregion

        #region Overrides
        public override string Name { get { return "Region Effects"; } }
        public override string Author { get { return "Snirk Immington"; } }
        public override string Description { get { return "Allows you to give different regions effects in a powerful, programmatic way."; } }
        public override Version Version { get { return System.Reflection.Assembly.GetCallingAssembly().GetName().Version; } }
        #endregion

        #region Initialize
        public override void Initialize()
        {
            Database.Initialize();
            // TODO set up config
            // TOOD print obtained things.
        }
        #endregion

        #region Hooks

        private static void OnGreet(GreetPlayerEventArgs e)
        {
            if (!e.Handled)
            {
                Players.Add(new RegionPlayer(e.Who));

                var ts = TShock.Players[e.Who];
                if (ts.Group.HasPermission("*"))
                {
                    var copyGroup = new Group(ts.Group.Name, ts.Group.Parent, ts.Group.ChatColor, string.Join(",", ts.Group.permissions));

                    ts.Group = copyGroup; ts.RPlayer().Permissions = ts.Group.permissions.ToArray();
                }
            }
        }

        private static void OnLeave(LeaveEventArgs e)
        {
            Players.RemoveAll(p => p.Index == e.Who);
        }

        private static void OnUpdate()
        {
            DateTime now = DateTime.Now;
            lock (Main.player)
            {
                try
                {
                    foreach (var ply in Main.player)
                    {
                        if (ply == null) continue;

                        // Get the regions

                        // Get the effects

                        // Apply the effects
                    }
                }
                catch (Exception ex) 
                { 
                    Log.ConsoleError("Error in Region Effects update tick! " + ex.Message); 
                    Log.Error("Full trace:\n" + ex.ToString()); 
                } 
            }
        }

        private static void OnFinishLogin(PlayerPostLoginEventArgs e)
        {
            if (!e.Player.Group.HasPermission("*"))
            {
                var copyGroup = new Group(e.Player.Group.Name, e.Player.Group.Parent, 
                    e.Player.Group.ChatColor, string.Join(",", e.Player.Group.permissions));

                e.Player.Group = copyGroup;
            }
        }

        #endregion

        #region Command

        private static void Command(CommandArgs com)
        {
            //|     /re add
            //|     /re del
            //|     /re select
            //|     /re set
            //|     /re help
            //|     /re ????

            #region No Parameters
            if (com.Parameters.Count == 0)
            {
                com.Player.SendInfoMessage("Region Effects command subcommands [for more info see /re help wiki]:");
                com.Player.SendSuccessMessage("/re add|del|set - commands for Region Effects on TShock regions.");
                com.Player.SendInfoMessage("/re list effects|targets|regions - lists available commands or EffectRegions.");
                com.Player.SendInfoMessage("/re help [subcommand] - gets info about the plugin or a subcommand!");
                return;
            }
            #endregion

            #region Proper Command
            else switch (com.Parameters[0].ToLower())
                {
                    #region Help
                    case "help":
                    if (com.Parameters.Count == 2)
                        switch (com.Parameters[1].ToLower())
                        {
                            #region Help Effects
                            case "effects":
                                return;
                            #endregion

                            #region Help Targeters
                            #endregion

                            #region Help Buffs


                            #endregion

                            #region Help Wiki
                            case "wiki": case "help": case "info":
                                com.Player.SendInfoMessage("A comprehensive guide to the plugin can be found on the GitHub Wiki.");
                                com.Player.SendInfoMessage("You can visit its page in the TShock forums for information and examples.");
                                com.Player.SendInfoMessage("Github - https://github.com/SnirkImmington/GameEffects"); return;
                            #endregion

                            #region Help ????
                            default:
                                com.Player.SendInfoMessage("Usage: /re help [effects|targeters|buffs]. Also try /re examples.");
                                return;
                            #endregion
                        }

                    #region Help <default>
                    else // No param
                    {
                        // Tell about plugin
                        com.Player.SendInfoMessage("GameEffects is a plugin designed to allow you to program regions with cool effects.");
                        com.Player.SendInfoMessage("These Effects are things like buffing a player, sending them a message, or giving them an item.");
                        com.Player.SendInfoMessage("You can target specific players with Targeters. For example, I can autoheal people on only the blue team.");
                        com.Player.SendInfoMessage("These effects are attached to a region with /re set {effects} and /re setr {region} {effects}.");
                        com.Player.SendInfoMessage("You can use /re help [subcommand] to learn more, or /re list to list Effects or Targeters.");
                        com.Player.SendInfoMessage("Finally, you can use /re example for a list of example ideas and their implementations.");
                        return;
                    }
                    #endregion

                    #endregion

                    #region List
                    case "list": case "l":
                    if (com.Parameters.Count == 2 || com.Parameters.Count == 3)
                    {
                        int page; if (!PaginationTools.TryParsePageNumber(com.Parameters, 2, com.Player, out page)) return;

                        switch (com.Parameters[1].ToLower())
                        {
                            #region List Effects
                            case "effects": case "GameEffects": case "effect": case "e":
                                PaginationTools.SendPage(com.Player, page, PaginationTools.BuildLinesFromTerms(
                                    API.Effects.ConvertAll(e => e.Name + " - " + e.Description)),
                                    new PaginationTools.Settings
                                    {
                                        HeaderFormat = "Effects ({0}/{1}):", FooterFormat = "Type /re 'e'ffects {0} for more.",
                                        NothingToDisplayString = "There are currently no GameEffects. This is bad! Tell Snirk.",
                                    }); return;
                            #endregion

                            #region List Targeters
                            case "targeters": case "targets": case "targeter": case "target": case "t":
                                PaginationTools.SendPage(com.Player, page, PaginationTools.BuildLinesFromTerms(
                                    API.Targeters.ConvertAll(t => t.Name + " - " + t.Description)),
                                    new PaginationTools.Settings
                                    {
                                        HeaderFormat = "Targeters ({0}/{1}):", FooterFormat = "Type /re 't'argeters {0} for more.",
                                        NothingToDisplayString = "There are currently no Targeters. This is bad! Tell Snirk."
                                    }); return;
                            #endregion

                            #region List Regions
                            case "regions": case "region": case "r":
                                PaginationTools.SendPage(com.Player, page, PaginationTools.BuildLinesFromTerms(
                                    Database.Regions.ConvertAll(r => r.RegionName)), new PaginationTools.Settings
                                    {
                                        HeaderFormat = "Region Effects Regions ({0}/{1}):", FooterFormat = "Type /re 'r'egions {0} for more.",
                                        NothingToDisplayString = "There are currently no EffectRegions. Create one using /re set today!"
                                    }); return;
                            #endregion
                        }
                    }
                    #region List <default>
                    else // Invalid /re list
                    {
                        // Tell about plugin
                        com.Player.SendErrorMessage("Usage: /re 'l'ist ['e'ffects|'t'argeters|'r'egions] {page}");
                        return;
                    }
                    #endregion
                    return;
                    #endregion

                    #region Add
                    #endregion

                    #region Del
                    #endregion

                    #region Select
                    #endregion

                    #region Example
                    #endregion
                }
            #endregion
        }

        private static void CheckToken(CommandArgs com)
        {
            com.Player.SendMessage("Tokens: " + string.Join(", ", com.Player.RPlayer().Tokens));
        }

        private static void CheckPerms(CommandArgs com)
        {
            Console.WriteLine("TShock: " + string.Join(", ", com.Player.Group.permissions));
            Console.WriteLine("Saved: " + string.Join(", ", com.Player.RPlayer().Permissions));
        }

        #endregion
    }
}
