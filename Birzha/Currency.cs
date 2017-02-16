using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birzha
{
    class Currency
    {
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string ParentCode
        {
            get
            {
                return _parentCode;
            }
        }
        string _name, _parentCode;
        public Currency(string n, string pc)
        {
            _name = n;
            _parentCode = pc;
        }
    }
}
