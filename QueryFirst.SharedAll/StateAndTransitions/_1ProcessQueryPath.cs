using System;
using System.IO;

namespace QueryFirst
{
    public class _1ProcessQueryPath
    {
        public static State Go(State state, string queryPathAndFilename)
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
