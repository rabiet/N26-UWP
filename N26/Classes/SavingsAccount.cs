using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class SavingsAccount
    {
        public string id;
        public string name;
        public double monthlyAmount;
        public List<SavingsHistory> history;
    }
}
