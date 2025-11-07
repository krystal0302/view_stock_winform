using StockWatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWatch.Application.Contracts
{
    public interface IPricingService
    {
        // 定義一個事件，用於通知 Presenter 報價資料已更新
        event EventHandler<StockPrice> PriceUpdated;

        // 啟動報價模擬服務
        Task StartSimulationAsync();

        // 停止服務
        void StopSimulation();
    }
}
