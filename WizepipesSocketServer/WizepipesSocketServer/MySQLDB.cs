using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

public class  MySQLDB
{
    private static string m_strConn = null;
    //   public static OperationTxT oper = new OperationTxT(@"D:\\ShopServiceLog.txt");
    public static MySqlConnection conn = null;
    public static MySQLDB MySql;
    /*public MySQLDB()
    { 
        //m_strConn = ConfigurationManager.ConnectionStrings["mysql1"].ToString();
        conn = new MySqlConnection(m_strConn);
    }*/
    //add 7-23
    public static string strDbConn
    {
        get { return m_strConn; }
        set { m_strConn = value; }
    }

    public static void InitDb()
    {
        //m_strConn = ConfigurationManager.ConnectionStrings["mysql1"].ToString();
        conn = new MySqlConnection(m_strConn);
    }

    public static bool IsConnectMysqlDB()
    {
        bool blResult = false;

        //MySqlConnection conn = new MySqlConnection(m_strConn);

        try
        {
            if (conn != null)
                conn.Open();
            blResult = true;
            conn.Close();
        }
        catch (System.Exception ex)
        {
            return blResult;
        }
        return blResult;
    }

    /*
    public static DataSet SelectDataSet(string strSQL)
    {
        DataSet ds = new DataSet();
        //MySqlConnection conn = new MySqlConnection(m_strConn);
        try
        {
            if (conn != null)
                conn.Open();
            MySqlDataAdapter comment = new MySqlDataAdapter(strSQL, conn);
            //oper.opertxt(System.DateTime.Now + "jack3333");
            comment.Fill(ds);
            //oper.opertxt(System.DateTime.Now + "jackr4444");
            conn.Close();
        }
        catch (System.Exception ex)
        {
            //oper.opertxt(System.DateTime.Now + "66444");
            return null;
        }
        return ds;
    }

    */

    /*
    public static DataSet SelectDataSet(string strSQL, MySqlParameter[] parms)
    {
        DataSet ds = new DataSet();
        //MySqlConnection conn = new MySqlConnection(m_strConn);
        try
        {
            if (conn != null)
                conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;
            if (parms != null)
            {
                foreach (MySqlParameter pram in parms)
                {
                    //oper.opertxt(System.DateTime.Now + "tom00");
                    cmd.Parameters.Add(pram);
                    //oper.opertxt(System.DateTime.Now + "tom01");
                }
            }

            MySqlDataAdapter comment = new MySqlDataAdapter(cmd);
            //oper.opertxt(System.DateTime.Now + "tom5");
            comment.Fill(ds);
            //oper.opertxt(System.DateTime.Now + "tom6");
            conn.Close();
        }
        catch (System.Exception ex)
        {
            //oper.opertxt(System.DateTime.Now + "tom7" + ex.Message.ToString());
            return null;
        }
        return ds;
    }
     * */

    public static DataSet SelectDataSet(string strSQL, MySqlParameter[] parms)
    {
        DataSet ds = new DataSet();
        MySqlConnection conn_single = new MySqlConnection(m_strConn);
        try
        {
            if (conn_single != null)
                conn_single.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn_single;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;
            if (parms != null)
            {
                foreach (MySqlParameter pram in parms)
                {
                    //oper.opertxt(System.DateTime.Now + "tom00");
                    cmd.Parameters.Add(pram);
                    //oper.opertxt(System.DateTime.Now + "tom01");
                }
            }

            MySqlDataAdapter comment = new MySqlDataAdapter(cmd);
            //oper.opertxt(System.DateTime.Now + "tom5");
            comment.Fill(ds);
            //oper.opertxt(System.DateTime.Now + "tom6");
            conn_single.Close();
            return ds;
        }
        catch (System.Exception ex)
        {
            conn_single.Close();
            return null;
        }

    }

    //public static DataSet SelectDataSetFromStoredProc(string StoredProcName, MySqlParameter[] parms)
    //{
    //    DataSet ds = new DataSet();
    //    MySqlConnection conn_single = new MySqlConnection(m_strConn);
    //    try
    //    {

    //        if (conn_single != null)
    //            conn_single.Open();

    //        MySqlDataReader reader;
            
    //        MySqlCommand cmd = new MySqlCommand();
    //        cmd.Connection = conn_single;
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = StoredProcName; 

    //        if (parms != null)
    //        {
    //            foreach (MySqlParameter pram in parms)
    //            {
    //                cmd.Parameters.Add(pram);
    //            }
    //        }

    //        reader = cmd.ExecuteReader();

    //        DataTable dataTable = new DataTable();
    //        for (int i = 0; i < reader.FieldCount; i++)
    //        {
    //            DataColumn mydc = new DataColumn();    //关键的一步
    //            mydc.DataType = reader.GetFieldType(i);
    //            mydc.ColumnName = reader.GetName(i);
    //            dataTable.Columns.Add(mydc);          //关键的第二步
    //        }
    //        while (reader.Read())
    //        {
    //            DataRow mydr = dataTable.NewRow();     //关键的第三步
    //            for (int i = 0; i < reader.FieldCount; i++)
    //            {
    //                mydr[i] = reader[i].ToString();
    //            }
    //            dataTable.Rows.Add(mydr);            //关键的第四步
    //            mydr = null;
    //        }

    //        ds.Tables.Add(dataTable);//想加几个加几个 

    //        conn_single.Close();
    //        return ds;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        conn_single.Close();
    //        return null;
    //    }

    //}

    public static DataSet SelectDataSettest(string strSQL, MySqlParameter[] parms)
    {
        DataSet ds = new DataSet();
        MySqlConnection conn_single = new MySqlConnection(m_strConn);
        try
        {
            if (conn_single != null)
                conn_single.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn_single;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;
            if (parms != null)
            {
                foreach (MySqlParameter pram in parms)
                {
                    //oper.opertxt(System.DateTime.Now + "tom00");
                    cmd.Parameters.Add(pram);
                    //oper.opertxt(System.DateTime.Now + "tom01");
                }
            }

            MySqlDataAdapter comment = new MySqlDataAdapter(cmd);
            //oper.opertxt(System.DateTime.Now + "tom5");
            comment.Fill(ds);
            //oper.opertxt(System.DateTime.Now + "tom6");
            conn_single.Close();
            return ds;
        }
        catch (System.Exception ex)
        {
            conn_single.Close();
            return null;
        }

    }

    public static DataSet SelectDataSetFromStoredProc(string StoredProcName, MySqlParameter[] parms)
    {
        DataSet ds = new DataSet();
        MySqlConnection conn_single = new MySqlConnection(m_strConn);
        try
        {
            if (conn_single != null)
                conn_single.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn_single;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = StoredProcName;
            if (parms != null)
            {
                foreach (MySqlParameter pram in parms)
                {
                    //oper.opertxt(System.DateTime.Now + "tom00");
                    cmd.Parameters.Add(pram);
                    //oper.opertxt(System.DateTime.Now + "tom01");
                }
            }

            MySqlDataAdapter comment = new MySqlDataAdapter(cmd);
            //oper.opertxt(System.DateTime.Now + "tom5");
            comment.Fill(ds);
            //oper.opertxt(System.DateTime.Now + "tom6");
            conn_single.Close();
            return ds;
        }
        catch (System.Exception ex)
        {
            conn_single.Close();
            return null;
        }

    }
    /// <summary>
    /// 执行insert或者update操作
    /// </summary>
    /// <param name="strSQL"></param>
    /// <returns></returns>
    public static bool ExecuteNonQry(string strSQL, MySqlParameter[] parms)
    {
        //MySqlConnection conn = new MySqlConnection(m_strConn);
        MySqlConnection conn_single = new MySqlConnection(m_strConn);
        try
        {
            //if (conn != null)
            //    conn.Open(); //说明：这种写法可能导致操作失败，所以更改为如下的写法。2014.6.24

            if (conn_single != null)
                conn_single.Open(); //modify by zx 2014.6.24 

            MySqlCommand cmd = new MySqlCommand();
            //cmd.Connection = conn;
            cmd.Connection = conn_single;

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;

            if (parms != null) //add by zx 2014.4.16
            {
                foreach (MySqlParameter pram in parms)
                    cmd.Parameters.Add(pram);
            }

            cmd.ExecuteNonQuery();

            //conn.Close();
            conn_single.Close();
            //oper.opertxt(System.DateTime.Now+"*************");
            return true;

        }
        catch (System.Exception ex)
        {
            conn_single.Close();
            //oper.opertxt(System.DateTime.Now + ex.Message);
            return false;
        }
        return true;
    }

    ///// <summary>
    ///// 执行insert或者update操作
    ///// </summary>
    ///// <param name="strSQL"></param>
    ///// <returns></returns>
    //public static string ExecuteNonQrytest(string strSQL, MySqlParameter[] parms)
    //{
    //    //MySqlConnection conn = new MySqlConnection(m_strConn);

    //    try
    //    {
    //        if (conn != null)
    //            conn.Open();

    //        MySqlCommand cmd = new MySqlCommand();
    //        cmd.Connection = conn;
    //        cmd.CommandType = CommandType.Text;
    //        cmd.CommandText = strSQL;

    //        if (parms != null) //add by zx 2014.4.16
    //        {
    //            foreach (MySqlParameter pram in parms)
    //                cmd.Parameters.Add(pram);
    //        }

    //        cmd.ExecuteNonQuery();
    //        conn.Close();
    //        //oper.opertxt(System.DateTime.Now+"*************");
    //        return "1";

    //    }
    //    catch (System.Exception ex)
    //    {
    //        //oper.opertxt(System.DateTime.Now + ex.Message);
    //        return ex.Message;
    //    }
      
    //}

/// <summary>
/// 本方法适用于一个输出参数的情况
/// </summary>
/// <param name="StoredProcName"></param>
/// <param name="parms"></param>
/// <param name="outPara"></param>
/// <returns></returns>
    public static string ExecuteStoredProc(string StoredProcName, MySqlParameter[] parms, MySqlParameter outPara)
    {
        MySqlConnection conn_single = new MySqlConnection(m_strConn);
        try
        {
            //if (conn != null)
            //    conn.Open(); //说明：这种写法可能导致操作失败，所以更改为如下的写法。2014.6.24

            if (conn_single != null)
                conn_single.Open(); //modify by zx 2014.6.24 

            //MySqlCommand cmd = new MySqlCommand(StoredProcName, conn);
            MySqlCommand cmd = new MySqlCommand(StoredProcName, conn_single);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parms != null) //add by zx 2014.4.16
            {
                foreach (MySqlParameter pram in parms)
                    cmd.Parameters.Add(pram);
            }

            cmd.Parameters.Add(outPara);
            cmd.ExecuteNonQuery();
            //conn.Close();
            conn_single.Close();
            return outPara.Value.ToString(); 

        }
        catch (System.Exception ex)
        {
            conn_single.Close();
            //oper.opertxt(System.DateTime.Now + ex.Message);
            return ex.Message;
           //return "-1";
        }
    }

    //public static string ExecuteStoredProctest(string StoredProcName, MySqlParameter[] parms, MySqlParameter outPara)
    //{

    //    try
    //    {
    //        if (conn != null)
    //            conn.Open();

    //        MySqlCommand cmd = new MySqlCommand(StoredProcName, conn);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        foreach (MySqlParameter pram in parms)
    //            cmd.Parameters.Add(pram);

    //        cmd.Parameters.Add(outPara);
    //        cmd.ExecuteNonQuery();
    //        conn.Close();
    //        return outPara.Value.ToString();

    //    }
    //    catch (System.Exception ex)
    //    {
    //        oper.opertxt(System.DateTime.Now + ex.Message);
    //        return ex.Message;
    //    }
    //}

}
