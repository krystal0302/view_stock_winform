using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWatch.Domain.Entities
{
    public class TradeOrder
    {
        public int OrderID { get; set; }
        public string StockID { get; set; }
        public string OrderType { get; set; } // "BUY" 或 "SELL"
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } // "PENDING", "EXECUTED", "CANCELLED"
        public DateTime OrderTime { get; set; }
    }
}
