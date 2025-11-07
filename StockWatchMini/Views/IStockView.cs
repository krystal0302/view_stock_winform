using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockWatch.Domain.Entities;

namespace StockWatchMini.Views
{
    public interface IStockView
    {
        // 讓 View 能夠顯示報價資料
        void DisplayStockPrices(IEnumerable<StockPrice> prices);

        // 讓 View 能夠顯示操作結果訊息
        void ShowMessage(string message, string title);

        // 定義使用者介面事件 (Presenter 訂閱這些事件)
        event EventHandler LoadView; // 視圖載入事件
        event EventHandler RefreshPrices; // 刷新按鈕點擊事件 (未來可以加上)

        // 取得下單介面的輸入值 (未來實作交易邏輯時需要)
        //string OrderStockID { get; }
        string OrderType { get; }
        int OrderQuantity { get; }
        decimal OrderPrice { get; }
    }
}
