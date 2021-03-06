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
        private OracleConnection cnn = null;
        private string userid       = "KOKEN_7";
        private string password     = "KOKEN_7";
        private string protocol     = "tcp";
        private string host         = "192.168.96.213";
        private string port         = "1521";
        private string servicename  = "KTEST";
        private string schema       = "KOKEN_7";
        // MySQLへの接続情報
        private string server = "localhost";
        private string database = "koken_5";
        private string user = "root";
        private string pass = "manager";
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
        private bool OraDBOpen()
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
                cnn = new OracleConnection();
                if (cnn != null)
                {
                    string connectString = "User Id=" + userid + "; "
                                    + "Password=" + password + "; "
                                    + "Data Source=" + dataSource;
                    cnn.ConnectionString = connectString;
                    cnn.Open();
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
            // DataTalbesClear
            ds.Relations.Clear();
            ds.Clear();
            dataGridView1.DataSource = "";
            while (ds.Tables.Count > 0)
            {
                DataTable table = ds.Tables[0];
                if (ds.Tables.CanRemove(table))
                {
                    ds.Tables.Remove(table);
                }
            }
            // Titles,Series,ChartAreasはchartコントロール直下のメンバ
            chart1.Titles.Clear();
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // DataGridView

            // MySQLへの接続情報
            string connectionString = string.Format("Server={0};Database={1};Uid={2};Pwd={3};Charset={4}", server, database, user, pass, charset);

            // Oracleへの接続
            //OraDBOpen();
            
            try
            {
                // MySQLに接続して、SELECT文で取得したデータをデータテーブルに格納する。
                // 手配ファイル
                string sqlD0410 = @"
                    SELECT ODRNO, HMCD, KTCD, ODCD, NEXTODCD, EDDT, ODRQTY, JIQTY, ODRSTS,
                            DATE_FORMAT(EDDT, '%Y/%m/%d') as EDDTSTR
                     FROM D0410 
                    WHERE ODRSTS='2' 
                      AND KTCD='WL15'
                    ORDER BY HMCD, EDDT";
                DataTable dtD0410 = new DataTable();


                MySqlDataAdapter adpD0410 = new MySqlDataAdapter(sqlD0410, connectionString);
                adpD0410.Fill(dtD0410);
                ds.Tables.Add(dtD0410);
                ds.Tables[ds.Tables.Count - 1].TableName = "D0410";

                ///OracleCommand myCmdD0410 = new OracleCommand(sqlD0410, cnn);
                ///OracleDataAdapter myDaD0410 = new OracleDataAdapter(myCmdD0410);
                ///myDaD0410.Fill(ds, "D0410");


                dtD0410.Columns.Add("10", typeof(string));
                dtD0410.Columns.Add("20", typeof(string));
                dtD0410.Columns.Add("30", typeof(string));
                dtD0410.Columns.Add("40", typeof(string));
                dtD0410.Columns.Add("50", typeof(string));
                dtD0410.Columns.Add("60", typeof(string));
                dtD0410.Columns.Add("70", typeof(string));
                dtD0410.PrimaryKey = new DataColumn[]
                        { dtD0410.Columns["ODRNO"] };

                /* // 手配先マスタ
                string sqlM0300 = "SELECT* FROM M0300 ORDER BY ODCD";
                DataTable dtM0300 = new DataTable();


                 MySqlDataAdapter adpM0300 = new MySqlDataAdapter(sqlM0300, connectionString);
                 adpM0300.Fill(dtM0300);
                 ds.Tables.Add(dtM0300);
                 ds.Tables[ds.Tables.Count - 1].TableName = "M0300";

                // OracleCommand myCmdM0300 = new OracleCommand(sqlM0300, cnn);
                // OracleDataAdapter myDaM0300 = new OracleDataAdapter(myCmdM0300);
                // myDaM0300.Fill(ds, "M0300");


                dtM0300.PrimaryKey = new DataColumn[]
                        { dtM0300.Columns["ODCD"] };
                // リレーションを貼る
                ds.Relations.Add("手配先マスタ",
                    ds.Tables["D0410"].Columns["ODCD"],
                    ds.Tables["M0300"].Columns["ODCD"], false);
                */
                // 品目手順詳細マスタ
                // MySQL
                    string sqlM0510 = 
                    @"select hmcd, 
                    max(case when ktseq = 10 then odrnm else null end) as '10', 
                    max(case when ktseq = 20 then odrnm else null end) as '20', 
                    max(case when ktseq = 30 then odrnm else null end) as '30', 
                    max(case when ktseq = 40 then odrnm else null end) as '40', 
                    max(case when ktseq = 50 then odrnm else null end) as '50', 
                    max(case when ktseq = 60 then odrnm else null end) as '60', 
                    max(case when ktseq = 70 then odrnm else null end) as '70' 
                    from M0510, M0300 
                    where M0510.ODCD = M0300.ODCD 
                    group by hmcd ";
                /* // Oracle
                string sqlM0510 =
                    @"select * 
                    from (
	                    select hmcd, ktseq, max(odrnm) as odrnm 
	                     from M0510, M0300 
	                    where M0300.ODCD= M0510.ODCD 
	                      and VALDTF = 
	                      (select MAX(tmp.VALDTF) from M0510 tmp where tmp.HMCD = M0510.HMCD)
	                      and exists 
	                      (select*from M0510 wk where wk.HMCD = M0510.HMCD and KTCD = 'WL15')
	                    group by hmcd, ktseq
                    )
                    pivot (
	                    max(odrnm) for ktseq in (10, 20, 30, 40, 50, 60, 70)
                    )";
                */
                DataTable dtM0510 = new DataTable();


                MySqlDataAdapter adpM0510 = new MySqlDataAdapter(sqlM0510, connectionString);
                adpM0510.Fill(dtM0510);
                ds.Tables.Add(dtM0510);
                ds.Tables[ds.Tables.Count - 1].TableName = "M0510";

                //OracleCommand myCmdM0510 = new OracleCommand(sqlM0510, cnn);
                //OracleDataAdapter myDaM0510 = new OracleDataAdapter(myCmdM0510);
                //myDaM0510.Fill(ds, "M0510");


                dtM0510.PrimaryKey = new DataColumn[]
                        { dtM0510.Columns["HMCD"] };

                // リレーションを貼る
                ds.Relations.Add("品目手順詳細マスタ",
                    ds.Tables["D0410"].Columns["HMCD"],
                    ds.Tables["M0510"].Columns["HMCD"], false);

                // 手配データ分ループ
                DataRow[] ChildRow;
                int nRecordCnt = 0;
                foreach (DataRow row in ds.Tables["D0410"].Rows)
                {
                    /*
                    // 子要素取得
                    ChildRow = row.GetChildRows("手配先マスタ");
                    // 結合されていた場合
                    if (ChildRow.Length != 0)
                    {
                        // 手配先名称格納
                        ds.Tables["D0410"].Rows[nRecordCnt]["ODRNM"] = ChildRow[0]["ODRNM"].ToString();
                    }
                    else
                    {
                        ds.Tables["D0410"].Rows[nRecordCnt]["ODRNM"] = "";
                    }
                    */

                    // 子要素取得
                    ChildRow = row.GetChildRows("品目手順詳細マスタ");
                    // 結合されていた場合
                    if (ChildRow.Length != 0)
                    {
                        // 手配先名称格納
                        ds.Tables["D0410"].Rows[nRecordCnt]["10"] = ChildRow[0]["10"].ToString();
                        ds.Tables["D0410"].Rows[nRecordCnt]["20"] = ChildRow[0]["20"].ToString();
                        ds.Tables["D0410"].Rows[nRecordCnt]["30"] = ChildRow[0]["30"].ToString();
                        ds.Tables["D0410"].Rows[nRecordCnt]["40"] = ChildRow[0]["40"].ToString();
                        ds.Tables["D0410"].Rows[nRecordCnt]["50"] = ChildRow[0]["50"].ToString();
                        ds.Tables["D0410"].Rows[nRecordCnt]["60"] = ChildRow[0]["60"].ToString();
                        ds.Tables["D0410"].Rows[nRecordCnt]["70"] = ChildRow[0]["70"].ToString();
                    }
                    else
                    {
                        ds.Tables["D0410"].Rows[nRecordCnt]["10"] = "";
                        ds.Tables["D0410"].Rows[nRecordCnt]["20"] = "";
                        ds.Tables["D0410"].Rows[nRecordCnt]["30"] = "";
                        ds.Tables["D0410"].Rows[nRecordCnt]["40"] = "";
                        ds.Tables["D0410"].Rows[nRecordCnt]["50"] = "";
                        ds.Tables["D0410"].Rows[nRecordCnt]["60"] = "";
                        ds.Tables["D0410"].Rows[nRecordCnt]["70"] = "";
                    }
                    nRecordCnt++;
                }

                // DataSourceに設定
                dataGridView1.DataSource = ds.Tables["D0410"];

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
                DataTable dtTehai2 = new DataTable();
                var sqlTehai2 = @"
                    select eddt, sum(odrqty) 'odrqty' 
                    from d0410 
                    where ktcd ='WL15' 
                    group by eddt";
                MySqlDataAdapter myDATehai2 = new MySqlDataAdapter(sqlTehai2, connectionString);
                // MySQLに接続して、SELECT文で取得したデータをデータテーブルに格納する。
                myDATehai2.Fill(dtTehai2);
                Series seriesColumn = new Series();
                seriesColumn.LegendText = "Legend:Column";
                seriesColumn.ChartType = SeriesChartType.Column;
                seriesColumn.XValueType = ChartValueType.DateTime;

                for (int i = 0; i < dtTehai2.Rows.Count; i++)
                {
                    //取得した日付をシリアル値に変換 x.ToOADate() xは日付型
                    seriesColumn.Points.Add(new DataPoint(DateTime.Parse(
                        dtTehai2.Rows[i][0].ToString()).ToOADate(), Convert.ToDouble(
                        dtTehai2.Rows[i][1]))); //(double)(dtg.Rows[i][1].ToString) dtg.Rows[i][0]
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

        private void dispSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DataTableから抽出～グループ化
            // チャート更新
            chart1.Titles.Clear();
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            Series seriesColumn = new Series();
            seriesColumn.LegendText = "Legend:Column";
            seriesColumn.ChartType = SeriesChartType.Column;
            seriesColumn.XValueType = ChartValueType.DateTime;

            DataTable newDt = new DataTable();
            newDt.Columns.Add("EDDTSTR", typeof(string));
            newDt.Columns.Add("SUMQTY", typeof(int));
            newDt = ds.Tables["D0410"]
                .Select("EDDTSTR >= '2022/04/01' and EDDTSTR <= '2022/04/20'")
                .AsEnumerable()
                .GroupBy(grp => new
                { EDDTSTR = grp.Field<string>("EDDTSTR") })
                .Select(x =>
                {
                    DataRow row = newDt.NewRow();
                    row["EDDTSTR"] = x.Key.EDDTSTR;
                    row["SUMQTY"] = x.Sum(r => r.Field<int>("ODRQTY"));
                    return row;
                }
                )
                .CopyToDataTable();

            foreach (DataRow result in newDt.Rows)
            {
                Console.WriteLine(result["eddtstr"]);

                //取得した日付をシリアル値に変換 x.ToOADate() xは日付型
                seriesColumn.Points.Add(new DataPoint(DateTime.Parse(
                    result["EDDTSTR"].ToString()).ToOADate(), Convert.ToDouble(
                    result["SUMQTY"])));
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

    }
}
