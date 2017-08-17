using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MySql.Data.MySqlClient;

namespace WizepipesSocketServer
{
    public class NetDb
    {
        public static string addsensorinfo(int sensorintdeviceID, string sensorIP, string sensorPort,
            string sensorloginTime, int sensorStatus, int AdStage)
        {
            MySQLDB.InitDb();
            string sensorid = "0";
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensorinfo");
                string strSQL1 = "SELECT intdeviceID FROM tsensorinfo where intdeviceID=" + sensorintdeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    // 有数据集
                    {
                        sensorid = ds1.Tables[0].Rows[0][0].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                return "fail"; //数据库异常
            }

            //************************************************************
            if (sensorid == "0") //若不存在,则添加
            {
                DataSet ds = new DataSet("dssensorinfo");
                string strResult = "";
                MySqlParameter[] parmss = null;
                string strSQL = "";
                bool IsDelSuccess = false;
                strSQL = " insert into tsensorinfo (intdeviceID,IP,Port,loginTime,Status,AdStage) values" +
                         "(?sensorintdeviceID,?sensorIP,?sensorPort,?sensorloginTime,?sensorStatus,?sensorAdStage);";

                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorIP", MySqlDbType.VarChar),
                    new MySqlParameter("?sensorPort", MySqlDbType.VarChar),
                    new MySqlParameter("?sensorloginTime", MySqlDbType.DateTime),
                    new MySqlParameter("?sensorStatus", MySqlDbType.Int32),
                    new MySqlParameter("?sensorAdStage", MySqlDbType.Int32)
                };
                parmss[0].Value = sensorintdeviceID;
                parmss[1].Value = sensorIP;
                parmss[2].Value = sensorPort;
                parmss[3].Value = Convert.ToDateTime(sensorloginTime);
                parmss[4].Value = sensorStatus;
                parmss[5].Value = AdStage;

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

            else //若ID存在,就更新update
            {
                DataSet ds = new DataSet("dssensorinfo");
                string strResult = "";
                MySqlParameter[] parmss = null;
                string strSQL = "";
                bool IsDelSuccess = false;
                strSQL =
                    "Update tsensorinfo SET IP=?sensorIP,Port=?sensorPort,loginTime=?sensorloginTime,Status=?sensorStatus, AdStage=?sensorAdStage WHERE intdeviceID=?sensorintdeviceID";

                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorIP", MySqlDbType.VarChar),
                    new MySqlParameter("?sensorPort", MySqlDbType.VarChar),
                    new MySqlParameter("?sensorloginTime", MySqlDbType.DateTime),
                    new MySqlParameter("?sensorStatus", MySqlDbType.Int32),
                    new MySqlParameter("?sensorAdStage", MySqlDbType.Int32)
                };
                parmss[0].Value = sensorintdeviceID;
                parmss[1].Value = sensorIP;
                parmss[2].Value = sensorPort;
                parmss[3].Value = Convert.ToDateTime(sensorloginTime);
                parmss[4].Value = sensorStatus;
                parmss[5].Value = AdStage;

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

        }

        public static string UpdateSensorGPSinfo(int sensorintdeviceID, double Longitude, double Latitude)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tsensorinfo SET Longitude=?sensorLongitude, Latitude=?sensorLatitude WHERE intdeviceID=?sensorintdeviceID";
            parmss = new MySqlParameter[]
            {
                new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                new MySqlParameter("?sensorLongitude", MySqlDbType.Double),
                new MySqlParameter("?sensorLatitude", MySqlDbType.Double),
            };
            parmss[0].Value = sensorintdeviceID;
            parmss[1].Value = Longitude;
            parmss[2].Value = Latitude;

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
                return "fail";
            }
        }

        public static string addsensorad(int sensorintdeviceID, string sensorDataDate, string sensorDataPath)
        {
            MySQLDB.InitDb();
            string sensorid = "0";
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensorad");
                string strSQL1 = "  SELECT intdeviceID FROM tsensorad where intdeviceID=" + sensorintdeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    // 有数据集
                    {
                        sensorid = ds1.Tables[0].Rows[0][0].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                return "fail"; //数据库异常
            }

            //************************************************************
            if (sensorid == "0") //若不存在,则添加,创建子表并新增数据
            {
                //DataSet ds = new DataSet("dssensorad");
                //string strResult = "";
                MySqlParameter[] parmss = null;
                string strSQL = "";
                //string strSQL2 = "";
                bool IsDelSuccess = false;
                //先在母表中插入ID和字表名
                string childName = "tsensoradchild" + sensorintdeviceID.ToString();
                strSQL =
                    "insert into tsensorad (intdeviceID,ChildTable) values (?sensorintdeviceID , ?sensorChildTable);";
                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorChildTable", MySqlDbType.VarChar)
                };
                parmss[0].Value = sensorintdeviceID;
                parmss[1].Value = childName;

                try
                {
                    IsDelSuccess = MySQLDB.ExecuteNonQry(strSQL, parmss);

                    if (IsDelSuccess != false)
                    {
                        creatNewTable(childName); //创建子表
                        insertADData(childName, sensorDataDate, sensorDataPath);

                        return "ok";
                    }
                    else
                    {
                        return "fail";
                    }
                }
                catch (Exception ex)
                {
                    return "fail";
                }
            }

            else //若ID存在,就查找子表,在子表中新增数据
            {
                DataSet ds = new DataSet("dssensorinfo");
                //MySqlParameter[] parmss = null;
                string strSQL1 = "";
                //bool IsDelSuccess = false;
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
                            //向子表插入数据
                            insertADData(sensorid, sensorDataDate, sensorDataPath);
                        }
                    }
                    return "ok";
                }
                catch (Exception ex)
                {
                    return "fail";
                }
            }
        }

        //创建新表 add 3-17
        //"创建新表")]
        public static string creatNewTable(string childName)
        {
            MySQLDB.InitDb();
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL = "CREATE TABLE " + childName +
                     "(DataID INT AUTO_INCREMENT, DataDate VARCHAR(45), DataPath VARCHAR(45), PRIMARY KEY (`DataID`));"; //建立新表
            /*parmss = new MySqlParameter[]
                                     {
                                         new MySqlParameter("?sensorChildTable", MySqlDbType.VarChar)
                                     };
            parmss[0].Value = childName;*/
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

                return "fail";
            }
        }

        //插入AD数据信息 add 3-17
        //"插入AD数据信息")]
        public static string insertADData(string tableName, string sensorDataDate, string sensorDataPath)
        {
            MySQLDB.InitDb();
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            //添加数据
            strSQL = " insert into " + tableName + " (DataDate,DataPath) values" +
                     "(?sensorDataDate,?sensorDataPath);";

            parmss = new MySqlParameter[]
            {
                //new MySqlParameter("?tableName", MySqlDbType.VarChar),
                new MySqlParameter("?sensorDataDate", MySqlDbType.DateTime),
                new MySqlParameter("?sensorDataPath", MySqlDbType.VarChar)
            };
            //parmss[0].Value = tableName;
            parmss[0].Value = Convert.ToDateTime(sensorDataDate);
            parmss[1].Value = sensorDataPath;
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
                return "fail";
            }
        }

        #region 操作cfg表

        //"读取后台添加设备命令数和命令参数")]
        public static List<string> readsensorcfg(int sensorintdeviceID)
        {
            List<string> resultList = new List<string>();
            MySQLDB.InitDb();

            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensorcfg");
                string strSQL1 =
                    "SELECT * FROM tsensorcfg where intdeviceID=" +sensorintdeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    // 有数据集
                    if (ds1.Tables[0].Rows.Count > 0)       
                    {
                        //共16个
                        //0-7
                        resultList.Add(ds1.Tables[0].Rows[0]["IsSetCapTime"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsSetOpenAndCloseTime"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsCaptureNow"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsGetGpsInfo"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsSetApName"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsSetApPassword"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsSetServerIP"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["IsSetServerPort"] as string);

                        //下面8个,8--15
                        resultList.Add(ds1.Tables[0].Rows[0]["CapTimeHour"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["CapTimeMinute"] as string);
                        
                        resultList.Add(ds1.Tables[0].Rows[0]["OpenTime"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["CloseTime"] as string);
                        
                        resultList.Add(ds1.Tables[0].Rows[0]["ApName"] as string);
                        
                        resultList.Add(ds1.Tables[0].Rows[0]["ApPassword"] as string);
                        
                        resultList.Add(ds1.Tables[0].Rows[0]["ServerIP"] as string);
                        
                        resultList.Add(ds1.Tables[0].Rows[0]["ServerPort"] as string);

                        return resultList;
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static string UpdateSensorCfg(int sensorintdeviceID, string updateItem, int updateNum)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tsensorcfg SET" + updateItem+"?sensorupdateItem WHERE intdeviceID=?sensorintdeviceID";
            parmss = new MySqlParameter[]
            {
                new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                new MySqlParameter("?sensorupdateItem", MySqlDbType.Int32),
            };
            parmss[0].Value = sensorintdeviceID;
            parmss[1].Value = updateNum;

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
                return "fail";
            }
        }

      #endregion

    }
}