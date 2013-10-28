using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace RegionEffects
{
    /// <summary>
    /// Contains data for regions with effects.
    /// </summary>
    class EffectsRegion
    {
        /// <summary>
        /// The name of the matched region.
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// The text given in the /re set command.
        /// </summary>
        public string Text { get; set; }

        public List<RegionPart> Things { get; set; }
        public List<string> ExternalCommands { get; set; }

        public static EffectsRegion Parse(string input)
        {
            //| Input = "team[red] buff(well) buff(-can);

            StringBuilder sb = new StringBuilder();

            // Loop through input
            for (int i = 0; i < input.Length; i++)
            {
            }
            return null; // TODO
        }

        public void Affect(TSPlayer ply, bool firstTime)
        {
            foreach (var part in Things)
                if (part.Targeter.Affects(ply, part.Targeter.Parameter))
                    foreach (var eff in part.Effects)
                        eff.Affect(ply, firstTime);
        }
    }

    /// <summary>
    /// Hi!
    /// </summary>
    class RegionPart
    {
        public Targeter Targeter { get; set; }
        public List<RegionEffect> Effects { get; set; }

        public RegionPart(Targeter target, List<RegionEffect> effects)
        { Targeter = target; Effects = effects; }
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
        #region Properties
        /// <summary>
        /// The keyword to be listening for.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// A description to show players about this Targeter.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// The test for effect, given the player and argument.
        /// </summary>
        private Func<RegionPlayer, string, bool> Target { get; private set; }
        /// <summary>
        /// Determines whether the input to the region is valid.
        /// </summary>
        private Func<string, bool> ArgumentValidator { get; set; }
        /// <summary>
        /// The parameter used in the targeter.
        /// </summary>
        public string Parameter { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Targeter (string name, string description, Func<RegionPlayer, string, bool> function)
        {
            Name = name; Target = function; ArgumentValidator = s => true; Description = description;
        }
        /// <summary>
        /// Constructor with an argument validator, used for cases where arguments could be invalid (i.e. item targeter).
        /// </summary>
        public Targeter(string name, string description, Func<RegionPlayer, string, bool> function, 
            Func<string, bool> validator)  : this(name, description, function)
        {
            ArgumentValidator = validator;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether a player is targeted by this Targeter,
        /// using this targeter's saved parameter for reference.
        /// </summary>
        /// <param name="ply">The player to target.</param>
        public bool Affects(RegionPlayer ply)
        {
            return Target(ply, this.Parameter);
        }
        /// <summary>
        /// Determines if the Targeter applies to a given RegionPlayer,
        /// given an external string argument.
        /// </summary>
        /// <param name="ply">The RegionPlayer being targeted.</param>
        /// <param name="argument">The argument to the Targeter.</param>
        public bool Affects(RegionPlayer ply, string argument)
        {
            return Target(ply, argument);
        }
        /// <summary>
        /// Determines if a given input (i.e. item(!excal)) is valid.
        /// </summary>
        /// <param name="input">The input for this <typeparamref name="RegionEffects.Targeter"/> is valid.</param>
        /// <returns>Whether the input is valid.</returns>
        public bool IsValidInput(string input)
        {
            return ArgumentValidator.Invoke(input);
        }
        /// <summary>
        /// Returns a new Targeter instance with this one's values, with <paramref name="param"/>
        /// as in input, ready for application on the regions.
        /// </summary>
        public Targeter WithInput(string param)
        {
            return new Targeter(Name, Target, ArgumentValidator) { Parameter = this.Parameter };
        }
        /// <summary>
        /// Returns a string representation of this Targeter as it would be typed in game.
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Parameter))
                return Name;

            return Name + "[" + Parameter + "]";
        }
        #endregion
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
        public RegionPlayer Player { get; set; }
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
        public RegionEffectArgs(RegionPlayer player, string arg, bool entered)
        { Player = player; Argument = arg; IsEntered = entered; }
    }

    /// <summary>
    /// Creates a possible effect on a region.
    /// </summary>
    public class RegionEffect
    {
        #region Properties
        /// <summary>
        /// The name of the effect (i.e. buff).
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Describes the RegionEffect in help menus.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Whether this can be called only when someone has entered.
        /// </summary>
        public bool OnlyOnEnter { get; set; }
        /// <summary>
        /// The function for the player and arguments that will be applied.
        /// </summary>
        public Action<RegionEffectArgs> Effect { get; set; }
        /// <summary>
        /// Validates the argument to the effect.
        /// </summary>
        private Func<string, bool> ArgumentValidator { get; set; }
        /// <summary>
        /// The parameters for the EffectRegion.
        /// </summary>
        public string Parameter { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// API costructor for Region Effects.
        /// </summary>
        /// <param name="keyword">The name of the effect, i.e. "buff".</param>
        /// <param name="effect">A function pointer taking RegionEffectArgs only.</param>
        /// <param name="onlyEnter">Whether the function should only be called when a player enters a region.</param>
        public RegionEffect(string keyword, string description, Action<RegionEffectArgs> effect, bool onlyEnter = false)
        { Name = keyword; Effect = effect; ArgumentValidator = s => true; Description = description; }
        /// <summary>
        /// Construcs a RegionEffect, also specifying use time and a way of determining if 
        /// given arguments are valid.
        /// </summary>
        /// <param name="name">The name of the effect, i.e. "buff".</param>
        /// <param name="act">The action it will have on players.</param>
        /// <param name="enter">Whether if it is only applied once per region.</param>
        /// <param name="argValid">A validator function for its argument, i.e. "buff(well)"
        /// would take the argument "well".</param>
        public RegionEffect(string name, string description, Action<RegionEffectArgs> act, bool enter, Func<string, bool> argValid)
        { Name = name; Effect = act; OnlyOnEnter = enter; ArgumentValidator = argValid; Description = description; }
        #endregion

        #region Methods
        /// <summary>
        /// Activates the effect on a player.
        /// </summary>
        /// <param name="ply">The player to affect.</param>
        /// <param name="firstTime">Whether the player has just entered the region.</param>
        public void Affect(TSPlayer ply, bool firstTime)
        {
            Effect.Invoke(new RegionEffectArgs(ply, Parameter, firstTime));
        }
        /// <summary>
        /// Determines whether a given argument to this RegionEffect is valid.
        /// </summary>
        /// <param name="argument">The argument to use, i.e. "well" in "buff(well)".</param>
        /// <returns></returns>
        public bool IsValid(string argument)
        {
            return ArgumentValidator.Invoke(argument);
        }
        /// <summary>
        /// Creates a new RegionEffect with an input, for storing in an EffectsRegion object.
        /// </summary>
        /// <param name="param">The parameter applied in the EffectsRegion.</param>
        /// <returns></returns>
        public RegionEffect WithInput(string param)
        {
            return new RegionEffect(Name, Effect, OnlyOnEnter) { Parameter = param };
        }
        /// <summary>
        /// Returns a string representation as the effect would be written in-game.
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Parameter))
                return Name;

            else return Name + "(" + Parameter + ")";
        }
        #endregion
    }

    /// <summary>
    /// Commands included in effect regions. 
    /// </summary>
    public class RegionCommand
    {
    }



    /// <summary>
    /// Provides plugin-specific regional player properties 
    /// and is used as arguments in all RegionEffect funcitons.
    /// </summary>
    public class RegionPlayer
    {
        /// <summary>
        /// The index of the player.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The corresponding TSPlayer.
        /// </summary>
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        /// <summary>
        /// The corresponding Terraria Player.
        /// </summary>
        public Player Player { get { return Main.player[Index]; } }
        /// <summary>
        /// RegionEffects saves player's permissions so it can temporarily give the player extra permissions.
        /// </summary>
        public string[] Permissions { get; internal set; }
        /// <summary>
        /// Plugins and users can assign tokens to players to determine if they should be affected.
        /// </summary>
        public HashSet<string> Tokens = new HashSet<string>();
        
        /// <summary>
        /// The EffectRegions the player is in and whether it's their first time.
        /// </summary>
        public Dictionary<EffectsRegion, bool> Regions { get; set; }
        /// <summary>
        /// The last position of the player recorded by the plugin. 
        /// This is used for regions that prevent players from entering.
        /// </summary>
        public Vector2 LastPosition { get; set; }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegionPlayer(int index)
        {
            Index = index; LastPosition = Player.position;
            Regions = new Dictionary<EffectsRegion, bool>();
            Permissions = TSPlayer.Group.permissions.ToArray();
        }
    }
}
