using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StockWatch.Application.Contracts;
using StockWatch.Application.Services;
using StockWatch.Infrastructure.Persistence;
using StockWatchMini.Presenters;
using StockWatchMini.Views;

namespace StockWatchMini.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static ServiceProvider ConfigureServices()
        {
            // 創建服務集合
            var services = new ServiceCollection();

            // =======================================================
            // 1. Infrastructure 層 (資料存取) 註冊
            // =======================================================
            // 註冊 IStockRepository 介面，使用 StockRepository 實作
            // 使用 AddSingleton 或 AddScoped (此處用 Singleton 簡化 WinForm 專案)
            services.AddSingleton<IStockRepository, StockRepository>();

            // =======================================================
            // 2. Application 層 (服務/業務邏輯) 註冊
            // =======================================================
             services.AddScoped<IPricingService, PricingService>(); // 尚未實作，先註釋

            // =======================================================
            // 3. Presentation 層 (UI/MVP) 註冊
            // =======================================================
            // Presenter 負責協調邏輯，通常是 Scoped 或 Transient
            services.AddTransient<MainPresenter>();

            // MainForm 是應用程式的主視窗，通常是 Transient
            services.AddTransient<MainForm>();

            // =======================================================
            // 4. 返回 ServiceProvider
            // =======================================================
            return services.BuildServiceProvider();
        }
    }
}
