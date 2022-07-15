using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Widgets.BlankTable
{
    public class BlankTableDefaults
    {
        public static string SystemName => "Widgets.BlankTable";

        public static string UserAgent => $"nopcommerce-{NopVersion.CURRENT_VERSION}";

        public static string ConfigurationRouteName => "Plugin.Widgets.BlankTable.Configure";
    }
}
