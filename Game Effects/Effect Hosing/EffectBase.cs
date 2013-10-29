using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEffects
{
    public class Effect
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        
        
    }

    public class EffectHost
    {
    }

    public class EffectEventArgs : EventArgs
    {
        public string Argument { get; set; }
        public EffectPlayer Player { get; set; }
    }
}
