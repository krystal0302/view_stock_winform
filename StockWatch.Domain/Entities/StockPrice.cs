using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWatch.Domain.Entities
{
    public class StockPrice
    {
        // 股票代碼 (VARCHAR)
        public string StockID { get; set; }

        // 當前價格 (DECIMAL)
        public decimal CurrentPrice { get; set; }

        // 當日開盤價 (DECIMAL)
        public decimal OpenPrice { get; set; }

        // 當日最高價 (DECIMAL)
        public decimal HighPrice { get; set; }

        // 當日最低價 (DECIMAL)
        public decimal LowPrice { get; set; }

        // 最後更新時間 (DATETIME)
        public DateTime LastUpdateTime { get; set; }

        // 新增一個計算屬性，方便 UI 顯示，例如：前一日收盤價和當前價的差異
        public decimal Change
        {
            get { return CurrentPrice - OpenPrice; }
        }
    }
}
