using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Update_WPF.Models
{
    public class Stock
    {
        public string StockName { get; set; }
        public double OpenPrice { get; set; }
        public double Price { get; set; }
        public string LastUpdated { get; set; }
    }


}
