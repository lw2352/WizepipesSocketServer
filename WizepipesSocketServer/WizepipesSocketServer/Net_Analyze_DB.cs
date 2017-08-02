using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace WizepipesSocketServer
{
    class Net_Analyze_DB
    {
        //"读取设备ID对应的最后一次AD数据保存路径")]//add 3-28
        public static string readDataPath(int sensorintdeviceID)
        {
            MySQLDB.InitDb();
            string sensorid = "0";
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds = new DataSet("tsensorad");
                string strSQL = "  SELECT intdeviceID FROM tsensorad where intdeviceID=" + sensorintdeviceID;
                ds = MySQLDB.SelectDataSet(strSQL, null);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    // 有数据集
                    {
                        sensorid = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return "fail";//数据库异常
            }

            if (sensorid != "0")//若id存在，就找出最大dataID对应的路径
            {
                string strSQL1 = "";
                //查找子表
                try
                {
                    DataSet ds1 = new DataSet("tsensorad");
                    strSQL1 = "  SELECT ChildTable FROM tsensorad where intdeviceID=" + sensorintdeviceID;
                    ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                    if (ds1 != null)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        // 有数据集
                        {
                            sensorid = ds1.Tables[0].Rows[0][0].ToString();
                            //查询子表数据
                            try
                            {
                                DataSet ds2 = new DataSet("tsensorad1");
                                string strSQL2 = "  SELECT DataPath FROM " + sensorid + " ORDER BY DataID DESC limit 1";
                                ds2 = MySQLDB.SelectDataSet(strSQL2, null);
                                if (ds2 != null)
                                {
                                    if (ds2.Tables[0].Rows.Count > 0)
                                    // 有数据集
                                    {
                                        string path = null;
                                        path = ds2.Tables[0].Rows[0][0].ToString();
                                        return path;
                                    }
                                    else return "fail";
                                }
                                else return "fail";
                            }
                            catch (Exception ex)
                            {
                                //return null;
                                return "fail";//数据库异常
                            }
                        }
                        else return "fail";
                    }
                    else return "fail";

                }
                catch (Exception ex)
                {
                    return "fail";//数据库异常
                }
            }
            else return "fail";
        }

        //"读取设备ID对应的最后一次AD数据保存时刻")]//add 3-28
        public static string readDataDate(int sensorintdeviceID)
        {
            MySQLDB.InitDb();
            string sensorid = "0";
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds = new DataSet("tsensorad");
                string strSQL = "  SELECT intdeviceID FROM tsensorad where intdeviceID=" + sensorintdeviceID;
                ds = MySQLDB.SelectDataSet(strSQL, null);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    // 有数据集
                    {
                        sensorid = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return "fail";//数据库异常
            }

            if (sensorid != "0")//若id存在，就找出最大dataID对应的路径
            {
                string strSQL1 = "";
                //查找子表
                try
                {
                    DataSet ds1 = new DataSet("tsensorad");
                    strSQL1 = "  SELECT ChildTable FROM tsensorad where intdeviceID=" + sensorintdeviceID;
                    ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                    if (ds1 != null)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        // 有数据集
                        {
                            sensorid = ds1.Tables[0].Rows[0][0].ToString();
                            //查询子表数据
                            try
                            {
                                DataSet ds2 = new DataSet("tsensorad1");
                                string strSQL2 = "  SELECT DataDate FROM " + sensorid + " ORDER BY DataID DESC limit 1";
                                ds2 = MySQLDB.SelectDataSet(strSQL2, null);
                                if (ds2 != null)
                                {
                                    if (ds2.Tables[0].Rows.Count > 0)
                                    // 有数据集
                                    {
                                        string date = null;
                                        date = ds2.Tables[0].Rows[0][0].ToString();
                                        return date;
                                    }
                                    else return "fail";
                                }
                                else return "fail";
                            }
                            catch (Exception ex)
                            {
                                return "fail";//数据库异常
                            }
                        }
                        else return "fail";
                    }
                    else return "fail";

                }
                catch (Exception ex)
                {
                    return "fail";//数据库异常
                }
            }
            else return "fail";
        }

        //把分析结果写入数据库保存 8-2
        public static string writeAnalyzeResult(int SensorAID, int SensorBID,
            int AnalyzeResult, string AnalyzeDate, int AnalyzePipe)
        {
            MySQLDB.InitDb();
            string sensorid = "0";

            DataSet ds = new DataSet("dssensorinfo");
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL = " insert into tsensorresult (SensorAID,SensorBID,AnalyzeResult,AnalyzeDate,AnalyzePipe) values" +
                     "(?sensorSensorAID,?sensorSensorBID,?sensorAnalyzeResult,?sensorAnalyzeDate,?sensorAnalyzePipe);";

            parmss = new MySqlParameter[]
            {
                    new MySqlParameter("?sensorSensorAID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorSensorBID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorAnalyzeResult", MySqlDbType.Int32),
                    new MySqlParameter("?sensorAnalyzeDate", MySqlDbType.DateTime),
                    new MySqlParameter("?sensorAnalyzePipe", MySqlDbType.Int32)
            };
            parmss[0].Value = SensorAID;
            parmss[1].Value = SensorBID;
            parmss[2].Value = AnalyzeResult;
            parmss[3].Value = Convert.ToDateTime(AnalyzeDate);
            parmss[4].Value = AnalyzePipe;

            try
            {
                IsDelSuccess = MySQLDB.ExecuteNonQry(strSQL, parmss);

                if (IsDelSuccess != false)
                {
                    return "ok";
                }
                else
                {
                    return "fail";
                }
            }

            catch (Exception ex)
            {
                return "fail"; //数据库异常
            }
        }



        //"获取path对应的归一化DataA")]//add 3-28
        public static double[] readDataA(string path)
        {
            double[] DataA = Net_Analyze.getDataA(path);
            return DataA;
        }

        //"获取path对应的归一化DataB")]//add 3-28
        public static double[] readDataB(string path)
        {
            double[] DataB = Net_Analyze.getDataB(path);
            return DataB;
        }

        //"获取经过算法分析后的DataC")]//add 3-28
        public static double[] readDataC(string path)
        {
            double[] DataC = Net_Analyze.getAnalyzeDataC();
            return DataC;
        }

        //"获取偏移值")]//add 3-28
        public static int readOffSet(string path)
        {
            int offset = Net_Analyze.GetOffSet();
            return offset;
        }

        //"AutoAnalyze")]//add 5-10
        public static int autoAnalyze(int idA, int idB)
        {
            int offset = Net_Analyze.AutoAnalyze(idA, idB);//Net_Analyze.AutoAnalyze会调用DB中的读数据库函数
            return offset;
        }
    }
}
