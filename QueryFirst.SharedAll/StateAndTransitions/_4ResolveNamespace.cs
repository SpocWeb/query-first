using System;

namespace QueryFirst
{
    public class _4ResolveNamespace
    {
        // user partial class is deprecated. We need to get all this from query config
        public static State Go(State state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            // Life is simpler at the console...
            if (!string.IsNullOrEmpty(state._3Config.Namespace))
                state._4Namespace = state._3Config.Namespace;
            else
            {
                // namespace is project namespace plus containing folders
                state._4Namespace =
                (
                    state._3Config.ProjectNamespace
                    + '.'
                    + state._1CurrDir.Substring(state._3Config.ProjectRoot.Length)
                        .Replace('\\', '.')
                        .Replace('/', '.')
                        .Trim('.')
                ).Trim('.');
            }
            state._4ResultClassName = state._1BaseName + state._3Config.DtoSuffix;
            state._4RepoClassName = state._1BaseName + state._3Config.RepoSuffix;
            state._4ResultInterfaceName = state._3Config.ResultInterfaceName ?? state._4ResultClassName;
            return state;
        }
    }
}
