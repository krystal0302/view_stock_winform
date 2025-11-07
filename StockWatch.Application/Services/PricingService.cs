using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StockWatch.Application.Contracts;
using StockWatch.Domain.Entities;

namespace StockWatch.Application.Services
{
    public class PricingService : IPricingService
    {
        // 關鍵：使用 CancellationToken 來安全停止異步任務
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IStockRepository _repository;
        private List<StockPrice> _trackedStocks;
        private readonly Random _random = new Random();

        // 宣告事件
        public event EventHandler<StockPrice> PriceUpdated;

        // DI 注入 Repository 介面
        public PricingService(IStockRepository repository)
        {
            _repository = repository;
        }

        public async Task StartSimulationAsync()
        {
            // 避免重複啟動
            if (_cancellationTokenSource != null) return;

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            // 1. 載入需要追蹤的股票清單 (從資料庫獲取初始數據)
            _trackedStocks = _repository.GetAllPrices();

            // 2. 啟動後台無限循環任務
            await Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // 模擬每 500 毫秒更新一次報價
                        await Task.Delay(500, token);

                        // 3. 執行報價更新邏輯
                        UpdateRandomPrice();

                        // 4. 通知所有訂閱者 (Presenter)
                        if (_trackedStocks != null && _trackedStocks.Any())
                        {
                            foreach (var stock in _trackedStocks)
                            {
                                // 發送更新事件
                                PriceUpdated?.Invoke(this, stock);

                                // 5. 將變動寫回資料庫 (這是 I/O 操作)
                                // 注意：如果頻率過高，這可能會是瓶頸！
                                _repository.UpdatePrice(stock);
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // 任務被取消，正常退出
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 記錄日誌，防止服務崩潰
                        Console.WriteLine($"報價模擬錯誤: {ex.Message}");
                    }
                }
            }, token);
        }

        public void StopSimulation()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        // 模擬價格隨機變動
        private void UpdateRandomPrice()
        {
            if (_trackedStocks == null || !_trackedStocks.Any()) return;

            // 隨機選一支股票
            var stockToUpdate = _trackedStocks[_random.Next(_trackedStocks.Count)];

            Console.WriteLine(stockToUpdate);

            // 產生一個 -0.5% 到 +0.5% 的隨機變動
            decimal factor = (decimal)(1 + (_random.NextDouble() * 0.01 - 0.005));
            decimal newPrice = Math.Round(stockToUpdate.CurrentPrice * factor, 2);

            stockToUpdate.CurrentPrice = newPrice;
            stockToUpdate.LastUpdateTime = DateTime.Now;

            // 更新 High/Low
            stockToUpdate.HighPrice = Math.Max(stockToUpdate.HighPrice, newPrice);
            stockToUpdate.LowPrice = Math.Min(stockToUpdate.LowPrice, newPrice);
        }
    }
}
