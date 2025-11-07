using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using StockWatch.Application.Contracts; // 引用 IStockRepository 介面
using StockWatch.Infrastructure.Persistence; // 引用 StockRepository 實作類別
using StockWatchMini.Views;
using StockWatchMini.Configuration;
using StockWatchMini.Presenters;
using Microsoft.Extensions.DependencyInjection;

namespace StockWatchMini
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 1. 配置並建立 DI 容器
            using (var serviceProvider = DependencyInjectionConfig.ConfigureServices())
            {
                // 2. 從容器中取得 Repository 實例進行連線測試
                // DI 確保我們拿到的是 IStockRepository 的具體實作 (StockRepository)
                var repository = serviceProvider.GetRequiredService<IStockRepository>();

                // 3. 執行連線測試 (使用 Infrastructure 層的方法)
                if (!repository.TestConnection())
                {
                    MessageBox.Show("資料庫連線失敗，請檢查配置和 SQL Server 狀態。", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // 連線失敗，中止程式
                }

                // 4. 連線成功，啟動 UI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // *** 關鍵：從 DI 容器取得 MainForm ***
                // DI 會自動解析 MainForm 的建構子，並注入 MainPresenter
                var mainForm = serviceProvider.GetRequiredService<MainForm>();

                // 5. 確保 Presenter 被正確綁定到 View (手動綁定，因為是 WinForm)
                var presenter = serviceProvider.GetRequiredService<MainPresenter>();

                // 我們需要在這裡將 MainPresenter 注入到 MainForm 中，因為 MS.DI 不直接支持
                // 具體實作 IView/IPresenter 之間的自動關聯。

                // 由於我們在 MainForm 實作了雙建構子，並且我們希望 DI 啟動，我們需要這樣處理：
                // [注意：如果您堅持使用雙建構子，請確保您的 MainForm 邏輯能正確接收 Presenter]

                // ********* [最佳實作方式] *********
                // 讓 Presenter 知道它要控制哪個 View 
                presenter.SetView(mainForm);

                // 讓 View 知道它的 Presenter 是誰 (以便在 Form_Load 觸發事件)
                mainForm.SetPresenter(presenter);

                // **********************************

                Application.Run(mainForm);
            }

            //IStockRepository repository = null;

            //try
            //{
            //    // 1. 實例化 Infrastructure 層的 Repository
            //    // 實例化過程中會嘗試讀取 App.config 的連線字串
            //    repository = new StockRepository();
            //}
            //catch (InvalidOperationException ex)
            //{
            //    // 處理連線字串配置錯誤（例如 App.config 中找不到 "StockWatchDB"）
            //    MessageBox.Show($"配置錯誤: {ex.Message}", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return; // 中止程式
            //}

            //// 2. 調用 DAL 層的 TestConnection() 方法
            //if (repository.TestConnection())
            //{
            //    // 連線成功
            //    MessageBox.Show("SQL 連線測試成功！(DAL 層測試通過)", "連線成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //    // 接著啟動應用程式
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new MainForm()); // 假設 Form1 已更名為 MainForm
            //}
            //else
            //{
            //    // TestConnection() 內部會捕獲 SQL 異常並回傳 false
            //    MessageBox.Show("資料庫連線失敗，請檢查 App.config 中的連線字串和 SQL Server 服務狀態。", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            ////Application.EnableVisualStyles();
            ////Application.SetCompatibleTextRenderingDefault(false);
            ////Application.Run(new Form1());
        }

       
    }
}
