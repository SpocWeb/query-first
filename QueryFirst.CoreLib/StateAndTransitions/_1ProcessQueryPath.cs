using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst
{
    public class _1ProcessQueryPath
    {
        public State Go(State state, string queryPathAndFilename)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            state._1SourceQueryFullPath = queryPathAndFilename;
            state._1BaseName = Path.GetFileNameWithoutExtension(queryPathAndFilename);
            state._1CurrDir = Path.GetDirectoryName(queryPathAndFilename) + "\\";
            return state;
        }
    }
}
