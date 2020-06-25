using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB2Editor.ResourceManager
{
    static class FileTypeManager
    {
        public static string[] NameSpaces
        {
            get
            {
                return registeredType.Keys.ToArray();
            }
        }

        public static string[] GetElement(string nameSpace)
        {
            HashSet<string> set;
            if (registeredType.TryGetValue(nameSpace, out set))
            {
                return set.ToArray();
            }
            return new string[0];
        }

        static Dictionary<string, HashSet<string>> registeredType = new Dictionary<string, HashSet<string>>();

        public static void Register(string nameSpace, string element)
        {
            HashSet<string> set;
            if (!registeredType.TryGetValue(nameSpace, out set))
            {
                set = new HashSet<string>();
                registeredType.Add(nameSpace, set);
            }
            set.Add(element);
        }
    }
}
