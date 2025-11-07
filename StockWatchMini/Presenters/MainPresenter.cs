using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockWatch.Application.Contracts;
using StockWatch.Domain.Entities;
using StockWatchMini.Views;

namespace StockWatchMini.Presenters
{
    public class MainPresenter
    {
        // 依賴的介面 (DI 的目標)
        private  IStockView _view;
        private readonly IStockRepository _repository;
        private readonly IPricingService _pricingService; // 未來實作即時報價用

        // 關鍵：建構子只注入服務/Repository
        public MainPresenter(IStockRepository repository, IPricingService pricingService)
        {
            _repository = repository;
             _pricingService = pricingService;
        }

        // *** 【新增】核心方法：用於綁定 View ***
        public void SetView(IStockView view)
        {
            _view = view;

            // 訂閱 View 的事件 (這是 Presenter 開始工作的訊號)
            _view.LoadView += OnLoadView;
            _pricingService.PriceUpdated += OnPriceUpdated;

            // 在 Presenter 中，確保 IView 已經綁定
        }

        private void OnLoadView(object sender, EventArgs e)
        {
            // 執行初始化邏輯
            LoadStockData();

            // ** 啟動即時報價服務 **
            _pricingService.StartSimulationAsync();
        }

        public async void LoadStockData()
        {
            try
            {
                // 1. 呼叫 Repository (DAL) 取得資料
                // 實際專案中，這裡應該呼叫 Application Service 
                var prices = _repository.GetAllPrices();

                // 2. 將資料傳回給 View 顯示
                _view.DisplayStockPrices(prices);
            }
            catch (Exception ex)
            {
                // 3. 處理錯誤並通知 View
                _view.ShowMessage($"載入資料失敗: {ex.Message}", "錯誤");
            }
        }

        private void OnPriceUpdated(object sender, StockPrice updatedPrice)
        {
            // 收到單個股票更新事件
            // 實務上，Presenter 會更新 View 綁定的資料源 (例如 BindingList<T>)

            // 為了簡化，我們暫時重新讀取所有數據並更新 View
            // 但更好的做法是只更新列表中的單個對象。
            LoadStockData();
        }

        // ... (新增停止服務方法，用於 MainForm 關閉事件)
        public void StopService()
        {
            _pricingService.StopSimulation();
        }

        // 交易邏輯等其他方法將在未來實作...
    }
}
