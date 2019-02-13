using DevComponents.DotNetBar;
using Huali.Common;
using Ray.Framework.Config;
using Ray.Framework.CustomDotNetBar;
using Ray.Framework.DBUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace Huali.DS9208
{
    /// <summary>
    /// 合并镜片/护理液程序
    /// 只显示和删除“XOUT”/“QOUT”类型单据
    /// 产品类型强制成大写“DS9208”/“DS9209”
    /// </summary>
    public partial class FrmDeleteByBill : Office2007Form
    {

        public FrmDeleteByBill()
        {
            InitializeComponent();
        }
        private static readonly string conn = CommonProcess.GetAppSettingConString();
        string sql = "";
        
        private void Form8_Load(object sender, EventArgs e)
        {
            comboBoxEx2.SelectedIndex = 0;
        }

        private void TextBoxX2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //过滤只显示要删除的的数据
                string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                string billNo = billType + textBoxX2.Text;
                sql = string.Format("SELECT [FActQty] as 已扫数量, [日期], [单据编号],[FEntryID]  as 分录号,[购货单位],[产品名称], [发货仓库],[实发数量], [批号], [摘要]  FROM [icstock] WHERE [单据编号] = '{0}' ORDER BY FEntryID", billNo);
                dataGridViewX1.DataSource = SqlHelper.ExecuteDataTable(conn, sql);
                dataGridViewX1.Columns["购货单位"].Width = 240;
                dataGridViewX1.Columns["产品名称"].Width = 300;
            }
        }

        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonX1_Click(object sender, EventArgs e)
        {
            //确认后删除
            if (MessageBox.Show("你真的要删除这些数据吗？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int res = 0;
                int resTotal = 0;
                string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                string billNo = billType + textBoxX2.Text;
                if (dataGridViewX1.SelectedRows.Count > 0)
                {
                    for (int i = 0; i < dataGridViewX1.Rows.Count; i++)
                    {
                        if (dataGridViewX1.Rows[i].Selected == true)
                        {
                            string entryID = dataGridViewX1.Rows[i].Cells["分录号"].Value.ToString();
                            sql = string.Format("DELETE [icstock] WHERE [单据编号] = '{0}' AND FEntryID = {1}", billNo, entryID.ToString());
                            resTotal += SqlHelper.ExecuteNonQuery(conn, sql);

                            string fID = entryID.PadLeft(4, '0');
                            res += DeleteDetailTable(billNo + fID);
                        }
                    }
                    if (resTotal > 0)
                    {
                        CustomDesktopAlert.H2(string.Format("{0} 条分录,{1} 条二维码被删除！", resTotal, res));

                        //刷新
                        sql = string.Format("SELECT [FActQty] AS 已扫数量, [日期], [单据编号],[FEntryID]  as 分录号,[购货单位],[产品名称], [发货仓库],[实发数量], [批号], [摘要]  FROM [icstock] WHERE [单据编号] = '{0}' Order By FEntryID", billNo);
                        dataGridViewX1.DataSource = (DataTable)null;
                        dataGridViewX1.DataSource = SqlHelper.ExecuteDataTable(conn, sql);
                        dataGridViewX1.Columns["购货单位"].Width = 240;
                        dataGridViewX1.Columns["产品名称"].Width = 300;
                    }
                }
            }
        }

        /// <summary>
        /// 循环所有数据表，找到对应数据删除
        /// </summary>
        /// <param name="EntryID">分录号</param>
        /// <returns>删除成功的条数</returns>
        private int DeleteDetailTable(string EntryID)
        {
            int retVal = 0;
            string[] prodT = null;
            //string prodType0 = "17";
            //string prodType1 = "16;18";
            string confProdType = ConfigHelper.ReadValueByKey(ConfigHelper.ConfigurationFile.AppConfig, "Modules");
            if (confProdType == EnumProductType.DS9209.ToString())
            {
                prodT = "17".Split(';');
            }
            else if (confProdType == EnumProductType.DS9208.ToString())
            {
                prodT = "16;18".Split(';');
            }
            else
            {
                CustomDesktopAlert.H2("产品类型设置错误！");
            }

            string baseTableName = "dbo.t_QRCode";

            for (int j = 0; j < prodT.Length; j++)
            {
                for (int i = 0; i < 100; i++)
                {
                    string fID = i < 10 ? "0" + i.ToString() : i.ToString();
                    sql = string.Format("DELETE {0}{1}{2} WHERE FEntryID = '{3}'",baseTableName, prodT[j], fID ,EntryID);
                    retVal += SqlHelper.ExecuteNonQuery(conn, sql);
                }
            }
            return retVal;
        }
    }
    public enum EnumProductType { DS9209, DS9208, }
}
