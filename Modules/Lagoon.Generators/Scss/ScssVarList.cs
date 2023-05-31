//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Lagoon.Generators
//{
//    internal class ScssVarList : IEnumerable<string>
//    {

//        private Dictionary<string, string> _dico = new();
//        private List<string> _merge = new();

//        internal bool TryAdd(string part)
//        {
////x            bool keepPreviousValue = part.EndsWith(" default!");
////x            if (!keepPreviousValue && part[part.Length - 1] != ';')
////x                return false;
//            if (part[part.Length - 1] != ';')
//                return false;
//            int sep = part.IndexOf(':');
//            if (sep == -1)
//                return false;
//            string key = part.Substring(0, sep);
//            if (part.IndexOf("map-merge(", sep) == -1)
//            {
//                if (!_dico.ContainsKey(key))
//                {
//                    _dico.Add(key, part);
//                }
//                else //x if (!keepPreviousValue)
//                {
//                    _dico[key] = part;
//                }
//            }
//            else
//            {
//                _merge.Add(part);
//            }
//            return true;
//        }

//        public IEnumerator<string> GetEnumerator()
//        {
//            return _dico.Values.Concat(_merge).GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//    }

//}
