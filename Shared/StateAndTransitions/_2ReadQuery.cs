using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst
{
    public class _2ReadQuery
    {
        /// <summary>
        /// Reads from filesystem. Not testable.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public State Go(State state)
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
