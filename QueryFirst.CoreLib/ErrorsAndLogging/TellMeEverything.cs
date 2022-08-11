using System;

namespace QueryFirst
{
    public static class Utils
    {
        public static string TellMeEverything(this Exception ex, string indent = "")
        {
            return "\r\n" + indent + ex.Message + "\r\n"
                + indent + ex?.StackTrace.Replace("\r\n", "\r\n" + indent)
                + ex?.InnerException?.TellMeEverything(indent + "  ");
        }
    }
}
