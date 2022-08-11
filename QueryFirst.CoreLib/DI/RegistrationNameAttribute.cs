using System;
using System.Linq;

namespace QueryFirst
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegistrationName : Attribute
    {
        private string _name;
        public RegistrationName(string name)
        {
            _name = name;
        }
        public string Name { get { return _name; } }
    }
    public static class ExtendTypeWithRegistrationName
    {
        public static string RegistrationName(this Type t)
        {
            var attr = ((RegistrationName)t.GetCustomAttributes(typeof(RegistrationName), true).FirstOrDefault());
            if (attr != null)
                return attr.Name;
            return null;
        }
    }

}