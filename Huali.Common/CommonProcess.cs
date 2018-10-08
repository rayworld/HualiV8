using System.Data;

namespace Huali.Common
{
    public sealed class CommonProcess
    {
        /// <summary>
        /// 过滤符合条件的数据
        /// 过滤不同单据类型
        /// </summary>
        /// <param name="dt">Excel 数据表</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public static DataTable FilterData(DataTable dt, string where)
        {
            DataRow[] rows = dt.Select(where);
            DataTable tmpdt = dt.Clone();
            foreach (DataRow row in rows)  // 将查询的结果添加到tempdt中； 
            {
                tmpdt.Rows.Add(row.ItemArray);
            }
            return tmpdt;
        }

        /// <summary>
        /// 得到唯一的单号列表
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="billNoFieldName">单号列的名字</param>
        /// <returns></returns>
        public static string GetDistinctBillNo(DataTable dt, string billNoFieldName)
        {
            string tempBillNo = "";
            string billNo = "";
            string retVal = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                billNo = dt.Rows[i][billNoFieldName].ToString();
                if (billNo != tempBillNo)
                {
                    retVal += billNo + ";";
                    tempBillNo = billNo;
                }
            }

            //去掉最后一个分号
            return retVal.Substring(0, retVal.Length - 1);
        }

        /// <summary>  
        /// 判读字符串是否为数值型
        /// </summary>  
        /// <param name="strNumber">字符串</param>  
        /// <returns>是否</returns>  
        public static bool IsNumber(string strNumber)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"^-?\d+\.?\d*$");
            return r.IsMatch(strNumber);
        }

    }
}
