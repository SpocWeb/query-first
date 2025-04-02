using System;
using System.IO;

namespace QueryFirst
{
    public class _2ReadQuery
    {
        /// <summary>
        /// Reads from filesystem. Not testable.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static State Go(State state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            // read source query
            // We've already checked the file exists. No sophistication.
            state._2InitialQueryText = File.ReadAllText(state._1SourceQueryFullPath);
            return state;
        }
    }
}
