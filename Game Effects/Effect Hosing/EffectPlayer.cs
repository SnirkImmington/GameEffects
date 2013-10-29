using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;

namespace GameEffects
{
    internal class EffectPlayer
    {
        public int Index { get; private set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public Player   Player   { get { return Main.player[Index]; } }

        internal string[] DefaultPermissions { get; set; }

        internal EffectPlayer(int index)
        {
            Index = index;
        }
    }
}
