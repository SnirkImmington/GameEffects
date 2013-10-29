using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace GameEffects
{
    class Config
    {
        public static string SetEffectRegionsPermission = "reffect.set";
        public static string ViewGameEffectsPermission = "reffect.see";
        public static string CantLosePermissionsFromRegionPerm = "reffect.noloss";
        public static string MakeDangerousGameEffectsPermission = "*";
        public static string ReloadConfigPermission = Permissions.maintenance;

        /// <summary>
        /// The command shortcuts, i.e. "*[Key] -> adds [Value] to a region's effects."
        /// </summary>
        public static Dictionary<string, string> GlobalEffects = new Dictionary<string, string>();
        
    }
}
