using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEffects
{
    public class CommandInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Func<string, bool> InputValidator { get; private set; }

        public Action<PlayerParamArgs> Action { get; private set; }

        
    }

    public class Command
    {
        public Action<PlayerParamArgs> Action { get; set; }
        public string Parameter { get; set; }

        public Command(string parameter, Action<PlayerParamArgs> action)
        {
            Action = action; Parameter = parameter;
        }

        public void Execute(PlayerParamArgs e)
        {
            Action.Invoke(e);
        }
    }

    public class Effect
    {
        public Targeter Targeter { get; private set; }
        public List<Command> Commands { get; private set; }

        public Effect(Targeter targeter, List<Command> commands)
        {
            Commands = commands; Targeter = targeter;
        }

        public void Invoke(PlayerParamArgs e)
        {
            if (Targeter.Affects(e.Player))
                foreach (var com in Commands)
                    com.Execute(e);
        }
    }

    public class EffectHostBase
    {
        public virtual string Name { get; private set; }
        public string InputText { get; set; }
        public List<Effect> Effects { get; private set; }

        public EffectHostBase(string name, string description)
        {

        }

        public void Invoke(PlayerParamArgs args)
        {
            foreach (var effect in Effects)
                effect.Invoke(args);
        }
    }

    /// <summary>
    /// Contains information for an argument and an EffectPlayer.
    /// </summary>
    public class PlayerParamArgs : EventArgs
    {
        /// <summary>
        /// The argument supplied to the method.
        /// </summary>
        public string Argument { get; set; }
        /// <summary>
        /// The player who is being affected.
        /// </summary>
        public EffectPlayer Player { get; set; }
        
        /// <summary>
        /// Constructor with an argument and EffectPlayer.
        /// </summary>
        /// <param name="argument">The argument to the method.</param>
        /// <param name="player">The index of the player involved.</param>
        public PlayerParamArgs(string argument, EffectPlayer player)
        {
            Argument = argument; Player = player;
        }

        /// <summary>
        /// Constructor with an argument and an index.
        /// </summary>
        /// <param name="argument">The argument to the method.</param>
        /// <param name="who">The index of the player involved.</param>
        public PlayerParamArgs(string argument, int who)
        {
            Argument = argument; Player = PluginMain.Players[who];
        }
    }

    // n w
    // n w is h
    // pvp(false) 
}
