using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Interfaces.Streaming;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace StockWatchMini.Core
{
    public class TradingService
    {
        // 從 App.config 取得連線字串
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["StockWatchDB"].ConnectionString;
    }
}