using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWatchMini.Data.Repositories
{
    public interface IStockRepository
    {
        // 驗證資料庫連線
        bool TestConnection();

        // 讀取所有報價
        List<StockPrice> GetAllPrices();

        // 更新報價
        void UpdatePrice(StockPrice price);

        // 寫入交易委託 (稍後實作)
        void PlaceOrder(TradeOrder order);
    }
}
