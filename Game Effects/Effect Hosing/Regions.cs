using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace GameEffects
{
    class RegionHost : EffectHostBase
    {
        public Region Region { get; set; }
        public override string Name { get { return this.Region.Name; } }

        public RegionHost(Region region)
        {
        }


    }
}
