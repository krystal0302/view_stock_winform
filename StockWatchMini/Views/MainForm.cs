using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockWatch.Domain.Entities;
using StockWatchMini.Presenters;

namespace StockWatchMini.Views
{
    public partial class MainForm : Form, IStockView
    {
        // 必須定義一個 Presenter 實例
        private MainPresenter _presenter;


        public MainForm()
        {
            InitializeComponent();
        }

        // 2. **【新增/修正】** SetPresenter 方法
        public void SetPresenter(MainPresenter presenter)
        {
            _presenter = presenter;

            // 綁定事件到 Presenter
            // 簡化：在 Form_Load 時觸發 IStockView.LoadView 事件
            this.Load += (s, e) => LoadView?.Invoke(this, EventArgs.Empty);

            // 新增：綁定 FormClosed 事件
            this.FormClosed += (s, e) => _presenter.StopService();
        }


        // =================================================================
        // A. IStockView 介面實作
        // =================================================================

        // 實作 DisplayStockPrices：將資料繫結到 DataGridView
        public void DisplayStockPrices(IEnumerable<StockPrice> prices)
        {
            // 取得資料列表
            var dataList = prices.ToList();

            if (dgvStockPrices.InvokeRequired)
            {
                dgvStockPrices.Invoke(new Action(() =>
                {
                    dgvStockPrices.DataSource = dataList;
                    ApplyAutoSizeSettings(); // **新增：套用自動調整設定**
                }));
            }
            else
            {
                dgvStockPrices.DataSource = dataList;
                ApplyAutoSizeSettings(); // **新增：套用自動調整設定**
            }
        }

        // 實作 ShowMessage：用於彈出訊息框
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 實作事件：在 Form 載入時觸發 LoadView 事件
        public event EventHandler LoadView;
        public event EventHandler RefreshPrices; // 暫時未用

        // 實作屬性 (用於下單邏輯，暫時回傳空值)
        //public string OrderStockID => txtStockID.Text; // 假設新增了一個 txtStockID
        public string OrderType => "BUY";
        public int OrderQuantity => 100;
        public decimal OrderPrice => 100.0M;

        // =================================================================
        // B. Form 事件處理
        // =================================================================

        private void MainForm_Load(object sender, EventArgs e)
        {
            // ** 關鍵：在 Form 載入時，觸發 LoadView 事件 **
            // Presenter 已經訂閱了這個事件，它會處理後續的資料載入
            LoadView?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyAutoSizeSettings()
        {
            // 設置 DataGridView 的整體行為
            // 讓所有列的寬度自動調整以完全填充 DataGridView 的寬度
            //dgvStockPrices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 可選：如果你希望某些列固定寬度，可以單獨設置
            /* if (dgvStockPrices.Columns.Contains("StockID"))
            {
                // 股票代碼 (StockID) 讓它只適應內容，而不是被拉伸
                dgvStockPrices.Columns["StockID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            */

            // 另一種常見模式：只讓最後一列填充剩餘空間
            
            // 1. 讓所有列根據內容調整
            foreach (DataGridViewColumn col in dgvStockPrices.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            // 2. 讓最後一列（或某一列）填充剩餘空間
            dgvStockPrices.Columns[dgvStockPrices.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            

            // 確保 DataGridView 在 Form 調整大小時能跟著變化
            // 最好在 Designer 裡將 DataGridView 的 Anchor 屬性設為 Top, Bottom, Left, Right
        }
    }
}
