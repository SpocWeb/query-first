using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst
{
    public interface IResultClassMaker
    {
        string Usings();
        string StartClass(State state);
        string MakeProperty(ResultFieldDetails fld);
        string CloseClass();
    }
    public class ResultClassMaker : IResultClassMaker
    {
        public static readonly string n = Environment.NewLine;
        public virtual string Usings() { return ""; }

        public virtual string StartClass(State state)
        {
            string implements = "";
            return $"public partial class {state._4ResultClassName} {implements} {{{n}";
        }
        public virtual string MakeProperty(ResultFieldDetails fld)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine($"protected {fld.TypeCsShort} _{fld.CSColumnName}; // {fld.TypeDb ?? "unknown_DB_type"}{fld.ColumnSizeInBrackets} {(fld.AllowDBNull ? "null" : "not null")}");
            code.AppendLine($"public {fld.TypeCsShort} {fld.CSColumnName}{{{n}get{{return _{fld.CSColumnName};}}{n}set{{_{fld.CSColumnName} = value;}}{n}}}");
            return code.ToString();
        }

        public virtual string CloseClass()
        {
            return $@"protected internal virtual void OnLoad(){{}}
}}
";
        }
        public virtual string StartNamespace(State state)
        {
            if (!string.IsNullOrEmpty(state._4Namespace))
                return "namespace " + state._4Namespace + "{" + Environment.NewLine;
            else
                return "";
        }
        public virtual string CloseNamespace(State state)
        {
            if (!string.IsNullOrEmpty(state._4Namespace))
                return "}" + Environment.NewLine;
            else
                return "";
        }
    }
}
