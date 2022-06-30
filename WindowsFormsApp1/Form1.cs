using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        DataSet ds = new DataSet();
        // Oracleへの接続情報
        private OracleConnection s_OracleConnection = null;
        private string userid       = "KOKEN_7";
        private string password     = "KOKEN_7";
        private string protocol     = "tcp";
        private string host         = "192.168.96.214";
        private string port         = "1521";
        private string servicename  = "KTEST";
        private string schema       = "KOKEN_7";
        // MySQLへの接続情報
        private string server = "192.168.96.213";
        private string database = "koken_7";
        private string user = "koken_7";
        private string pass = "koken_7";
        private string charset = "utf8";

        public Form1()
        {
            InitializeComponent();
            this.Text = "DataGridViewテスト";


            toolStripStatusLabel1.Text = "";
            // Titles,Series,ChartAreasはchartコントロール直下のメンバ
            chart1.Titles.Clear();
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

        }

        // DBオープン
        private bool OraDBOpen(string userId, string password, string protocol, string host, int port, string serviceName)
        {

            bool ret = false;

            string dataSource =
                "(DESCRIPTION=" +
                "(ADDRESS=" +
                "(PROTOCOL=" + protocol + ")" +
                "(HOST=" + host + ")" +
                "(PORT=" + port + ")" + ")" +
                "(CONNECT_DATA=" + "(SERVICE_NAME=" + servicename+ ")" + ")" +
                ")";

            try
            {
                s_OracleConnection = new OracleConnection();
                if (s_OracleConnection != null)
                {
                    string connectString = "User Id=" + userid + "; "
                                    + "Password=" + password + "; "
                                    + "Data Source=" + dataSource;
                    s_OracleConnection.ConnectionString = connectString;

                    s_OracleConnection.Open();
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                ret = false;
            }
            return ret;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            // コントロールのサイズをフォームの大きさから設定
            dataGridView1.Width = this.Width - (402 - 372);
            dataGridView1.Height = this.Height - (350 - 227);
            tabControl1.Width = this.Width - (402 - 386);
            tabControl1.Height = this.Height - (350 - 259); 
            chart1.Width = this.Width - (402 - 364);
            chart1.Height = this.Height - (350 - 221);

        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DataGridViewテスト Version 1.0.0");
        }

        private void 表示ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            // MySQLへの接続情報
            string server = "192.168.96.213";
            string database = "koken_7";
            string user = "koken_7";
            string pass = "koken_7";
            string charset = "utf8";
            string connectionString = string.Format("Server={0};Database={1};Uid={2};Pwd={3};Charset={4}", server, database, user, pass, charset);
            // DataGridView
            // MySQLへの接続
            try
            {
                // MySQLに接続して、SELECT文で取得したデータをデータテーブルに格納する。
                // 手配ファイル
                MySqlDataAdapter adpD0410 = new MySqlDataAdapter("SELECT* FROM D0410", connectionString);
                DataTable dtD0410 = new DataTable();
                adpD0410.Fill(dtD0410);
                dtD0410.PrimaryKey = new DataColumn[]
                        { dtD0410.Columns["ODRNO"] };
                ds.Tables.Add(dtD0410);
                ds.Tables[ds.Tables.Count - 1].TableName = "D0410";

                // 手配先マスタ
                MySqlDataAdapter adpM0300 = new MySqlDataAdapter("SELECT* FROM M0300", connectionString);
                DataTable dtM0300 = new DataTable();
                adpM0300.Fill(dtM0300);
                dtM0300.PrimaryKey = new DataColumn[]
                        { dtM0300.Columns["ODCD"] };
                ds.Tables.Add(dtM0300);
                ds.Tables[ds.Tables.Count - 1].TableName = "M0300";

                // 品目手順詳細マスタ
                string sqlM0510 =
                    @"select hmcd, 
                    max(case when ktseq = 10 then odcd else null end) as '10', 
                    max(case when ktseq = 20 then odcd else null end) as '20', 
                    max(case when ktseq = 30 then odcd else null end) as '30',
                    max(case when ktseq = 40 then odcd else null end) as '40',
                    max(case when ktseq = 50 then odcd else null end) as '50',
                    max(case when ktseq = 60 then odcd else null end) as '60',
                    max(case when ktseq = 70 then odcd else null end) as '70'
                    from M0510
                    group by hmcd ";
                MySqlDataAdapter adpM0510 = new MySqlDataAdapter(sqlM0510, connectionString);
                DataTable dtM0510 = new DataTable();
                adpM0510.Fill(dtM0510);
                dtM0510.PrimaryKey = new DataColumn[]
                        { dtM0510.Columns["HMCD"] };
                ds.Tables.Add(dtM0510);
                ds.Tables[ds.Tables.Count - 1].TableName = "M0510";

                // リレーションを貼る
                ds.Relations.Add("手配先マスタ",
                    ds.Tables["D0410"].Columns["ODCD"],
                    ds.Tables["M0300"].Columns["ODCD"], false);
                ds.Relations.Add("品目手順詳細マスタ",
                    ds.Tables["D0410"].Columns["HMCD"],
                    ds.Tables["M0510"].Columns["HMCD"], false);

                // 手配データ分ループ
                DataRow[] ChildRow;
                int nRecordCnt = 0;
                foreach (DataRow row in ds.Tables["D0410"].Rows)
                {
                    // 子要素取得
                    ChildRow = row.GetChildRows("手配先マスタ");
                    // 結合されていた場合
                    if (ChildRow.Length != 0)
                    {
                        // 会社名称格納
                        ds.Tables["D0410"].Rows[nRecordCnt]["ODCD"] = ChildRow[0]["ODNM"].ToString();
                    }
                    else
                    {
                        ds.Tables["D0410"].Rows[nRecordCnt]["ODCD"] = "";
                    }
                    // 子要素取得
                    ChildRow = row.GetChildRows("品目手順詳細マスタ");
                    // 結合されていた場合
                    if (ChildRow.Length != 0)
                    {
                        // 会社名称格納
                        ds.Tables["D0410"].Rows[nRecordCnt]["HMCD"] = ChildRow[0]["10"].ToString();
                    }
                    else
                    {
                        ds.Tables["D0410"].Rows[nRecordCnt]["HMCD"] = "";
                    }
                    nRecordCnt++;
                }

                // DataSourceに設定
                dataGridView1.DataSource = ds.Tables["D0410"]; ;

                //MessageBox.Show("MySQL接続完了");
                toolStripStatusLabel1.Text = "手配ファイル: " + ds.Tables["D0410"].Rows.Count.ToString() + "件の取得";
            }
            catch (MySqlException me)
            {
                Console.WriteLine("ERROR: " + me.Message);
            }

            // series
            // MySQLへの接続
            try
            {
                DataTable dtg = new DataTable();
                MySqlDataAdapter mysqlAdpg = new MySqlDataAdapter("select judt, sum(juqty) 'juqty' from d0410 group by judt", connectionString);
                // MySQLに接続して、SELECT文で取得したデータをデータテーブルに格納する。
                mysqlAdpg.Fill(dtg);
                Series seriesColumn = new Series();
                seriesColumn.LegendText = "Legend:Column";
                seriesColumn.ChartType = SeriesChartType.Column;
                seriesColumn.XValueType = ChartValueType.DateTime;

                for (int i = 0; i < dtg.Rows.Count; i++)
                {
                    //取得した日付をシリアル値に変換 x.ToOADate() xは日付型
                    seriesColumn.Points.Add(new DataPoint(DateTime.Parse(dtg.Rows[i][0].ToString()).ToOADate(), Convert.ToDouble(dtg.Rows[i][1]))); //(double)(dtg.Rows[i][1].ToString) dtg.Rows[i][0]
                }

                // chartarea
                ChartArea area1 = new ChartArea();
                //    area1.AxisX.Title = "日付";
                // X軸のフォーマットを設定します
                area1.AxisX.LabelStyle.Format = "MM/dd(ddd)";
                area1.AxisY.Title = "数量 (本数)";

                Title title1 = new Title("WL15: 日別集計");
                chart1.Titles.Add(title1);
                chart1.ChartAreas.Add(area1);
                chart1.Series.Add(seriesColumn);

            }
            catch (MySqlException me)
            {
                Console.WriteLine("ERROR: " + me.Message);
            }

        }

        private void 閉じるCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
