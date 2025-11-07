using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Interfaces.Streaming;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StockWatch.Domain.Entities;
using StockWatch.Application.Contracts;
using System.Configuration; // 讀取 App.config
using System.Data;
using System.Data.SqlClient; // SQL Server 連線

namespace StockWatch.Infrastructure.Persistence
{
    public class StockRepository : IStockRepository
    {
        private readonly string _connectionString;

        public StockRepository()
        {
            // 建構子：讀取連線字串
            var connSetting = ConfigurationManager.ConnectionStrings["StockWatchDB"];
            if (connSetting == null || string.IsNullOrEmpty(connSetting.ConnectionString))
            {
                // 如果連線字串不存在，應該拋出配置錯誤
                throw new InvalidOperationException("配置錯誤：App.config 中找不到名為 'StockWatchDB' 的連線字串。");
            }
            _connectionString = connSetting.ConnectionString;
        }

        // =================================================================
        // A. 實作 TestConnection() - 驗證連線
        // =================================================================
        public bool TestConnection()
        {
            try
            {
                // 使用 using 確保資源釋放，是資深工程師的標準寫法
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // 在 Infrastructure 層，捕獲異常，記錄日誌（省略），並回傳失敗狀態
                Console.WriteLine($"DAL 連線失敗: {ex.Message}");
                return false;
            }
        }

        // =================================================================
        // B. 實作 GetAllPrices() - 讀取即時報價
        // =================================================================
        public List<StockPrice> GetAllPrices()
        {
            var prices = new List<StockPrice>();
            const string sql = "SELECT StockID, CurrentPrice, OpenPrice, HighPrice, LowPrice, LastUpdateTime FROM StockPrice";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prices.Add(new StockPrice
                            {
                                StockID = reader["StockID"].ToString(),
                                // 確保型別轉換正確，避免潛在的裝箱/拆箱錯誤
                                CurrentPrice = (decimal)reader["CurrentPrice"],
                                OpenPrice = (decimal)reader["OpenPrice"],
                                HighPrice = (decimal)reader["HighPrice"],
                                LowPrice = (decimal)reader["LowPrice"],
                                LastUpdateTime = (DateTime)reader["LastUpdateTime"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理資料庫錯誤，例如日誌記錄
                Console.WriteLine($"讀取報價失敗: {ex.Message}");
                // 實務上應拋出 DAL 專屬異常
                throw;
            }

            return prices;
        }

        // =================================================================
        // C. 其他介面方法 (暫時保留，留待後續實作)
        // =================================================================
        public void UpdatePrice(StockPrice price)
        {
            const string sql = @"
        UPDATE StockPrice 
        SET CurrentPrice = @CurrentPrice, 
            HighPrice = @HighPrice, 
            LowPrice = @LowPrice, 
            LastUpdateTime = @LastUpdateTime
        WHERE StockID = @StockID";

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    // 關鍵：使用參數化查詢
                    cmd.Parameters.AddWithValue("@CurrentPrice", price.CurrentPrice);
                    cmd.Parameters.AddWithValue("@HighPrice", price.HighPrice);
                    cmd.Parameters.AddWithValue("@LowPrice", price.LowPrice);
                    cmd.Parameters.AddWithValue("@LastUpdateTime", price.LastUpdateTime);
                    cmd.Parameters.AddWithValue("@StockID", price.StockID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新報價失敗: {ex.Message}");
                // 實務上應記錄錯誤，並決定是否重新拋出
                throw;
            }
        }

        public void PlaceOrder(TradeOrder order)
        {
            throw new NotImplementedException("PlaceOrder 尚未實作，將在交易邏輯時完成。");
        }
    }
}