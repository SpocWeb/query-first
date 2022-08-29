using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst
{
    public static class BuildUpWithTiny
    {
        public static T BuildUp<T>(this T me)
        {
            return (T)TinyIoCContainer.Current.BuildUp(me);
        }
    }
}
