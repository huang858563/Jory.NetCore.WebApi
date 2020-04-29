using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace Jory.NetCore.Core.Helpers
{
    public class NLogHelper : Logger
    {
        public static readonly NLog.Logger Logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
    }
}
