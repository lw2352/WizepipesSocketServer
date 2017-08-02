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


        public static string addsensorcfg(int sensorintdeviceID, int sensorCmdNumHex, int sensorCapTimeHour,
            int sensorCapTimeMinute, int sensorOpenTime, int sensorCloseTime, int IsCaptureNow) //sensorcfg可由多个参数组成,后台再解析
        {
            MySQLDB.InitDb();
            string sensorid = "0";
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensorcfg");
                string strSQL1 = "  SELECT intdeviceID FROM tsensorcfg where intdeviceID=" + sensorintdeviceID;
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
                return "fail";
            }

            //************************************************************
            if (sensorid == "0") //若不存在,则添加
            {
                DataSet ds = new DataSet("dssensorcfg");
                string strResult = "";
                MySqlParameter[] parmss = null;
                string strSQL = "";
                bool IsDelSuccess = false;
                strSQL =
                    " insert into tsensorcfg (intdeviceID, CmdNumHex, CapTimeHour,CapTimeMinute,OpenTime,CloseTime, IsCaptureNow) values" +
                    "(?sensorintdeviceID, ?sensorCmdNumHex, ?sensorCapTimeHour, ?sensorCapTimeMinute, ?sensorOpenTime, ?sensorCloseTime, ?sensorIsCaptureNow);";

                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCmdNumHex", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCapTimeHour", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCapTimeMinute", MySqlDbType.Int32),
                    new MySqlParameter("?sensorOpenTime", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCloseTime", MySqlDbType.Int32),
                    new MySqlParameter("?sensorIsCaptureNow", MySqlDbType.Int32)
                };
                parmss[0].Value = sensorintdeviceID;
                parmss[1].Value = sensorCmdNumHex;
                parmss[2].Value = sensorCapTimeHour;
                parmss[3].Value = sensorCapTimeMinute;
                parmss[4].Value = sensorOpenTime;
                parmss[5].Value = sensorCloseTime;
                parmss[6].Value = IsCaptureNow;

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

            else //若ID存在,就更新update
            {
                DataSet ds = new DataSet("dssensorcfg");
                string strResult = "";
                MySqlParameter[] parmss = null;
                string strSQL = "";
                bool IsDelSuccess = false;
                strSQL =
                    "Update tsensorcfg SET CmdNumHex=?sensorCmdNumHex, CapTimeHour =?sensorCapTimeHour ,CapTimeMinute = ?sensorCapTimeMinute,OpenTime = ?sensorOpenTime,CloseTime = ?sensorCloseTime, IsCaptureNow = ?sensorIsCaptureNow WHERE intdeviceID=?sensorintdeviceID";

                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCmdNumHex", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCapTimeHour", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCapTimeMinute", MySqlDbType.Int32),
                    new MySqlParameter("?sensorOpenTime", MySqlDbType.Int32),
                    new MySqlParameter("?sensorCloseTime", MySqlDbType.Int32),
                    new MySqlParameter("?sensorIsCaptureNow", MySqlDbType.Int32)
                };
                parmss[0].Value = sensorintdeviceID;
                parmss[1].Value = sensorCmdNumHex;
                parmss[2].Value = sensorCapTimeHour;
                parmss[3].Value = sensorCapTimeMinute;
                parmss[4].Value = sensorOpenTime;
                parmss[5].Value = sensorCloseTime;
                parmss[6].Value = IsCaptureNow;

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
        }

        //读取后台添加设备命令数和命令参数 add 3-17//3-18改成多列
        //"读取后台添加设备命令数和命令参数")]
        public static int[] readsensorcfg(int sensorintdeviceID)
        {
            MySQLDB.InitDb();
            int[] sensorcfg = new int[7];
            int CmdNumHex = 0;
            int CapTimeHour = 0;
            int CapTimeMinute = 0;
            int OpenTime = 0;
            int CloseTime = 0;
            int IsCaptureNow = 0;
            int IsGetGPSinfo = 0;
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensorcfg");
                string strSQL1 =
                    "  SELECT CmdNumHex, CapTimeHour, CapTimeMinute, OpenTime, CloseTime, IsCaptureNow, IsGetGPSinfo FROM tsensorcfg where intdeviceID=" +
                    sensorintdeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    // 有数据集
                    {
                        CmdNumHex = (int)ds1.Tables[0].Rows[0][0];
                        CapTimeHour = (int)ds1.Tables[0].Rows[0][1];
                        CapTimeMinute = (int)ds1.Tables[0].Rows[0][2];
                        OpenTime = (int)ds1.Tables[0].Rows[0][3];
                        CloseTime = (int)ds1.Tables[0].Rows[0][4];
                        IsCaptureNow = (int)ds1.Tables[0].Rows[0][5];
                        IsGetGPSinfo = (int)ds1.Tables[0].Rows[0][6];

                        sensorcfg[0] = CmdNumHex;
                        sensorcfg[1] = CapTimeHour;
                        sensorcfg[2] = CapTimeMinute;
                        sensorcfg[3] = OpenTime;
                        sensorcfg[4] = CloseTime;
                        sensorcfg[5] = IsCaptureNow;
                        sensorcfg[6] = IsGetGPSinfo;

                        return sensorcfg;
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                return null;
                //return PublicMethod.getResultJson(ErrorCodeDefinition.DB_ERROR, ErrorCodeDefinition.getErrorMessageByErrorCode(ErrorCodeDefinition.DB_ERROR));//数据库异常
            }
        }

        //add 7-26
        //命令发送成功后把CmdNumHex置1
        public static string UpdateSensorCfg(int sensorintdeviceID, int cmdNumHex)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tsensorcfg SET CmdNumHex=?sensorCmdNumHex WHERE intdeviceID=?sensorintdeviceID";
            parmss = new MySqlParameter[]
            {
                new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                new MySqlParameter("?sensorCmdNumHex", MySqlDbType.Int32),
            };
            parmss[0].Value = sensorintdeviceID;
            parmss[1].Value = cmdNumHex;

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

        public static string UpdateSensorCfgBySetIsGetGpsInfo(int sensorintdeviceID, int IsGetGPSinfo)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tsensorcfg SET IsGetGPSinfo=?sensorIsGetGPSinfo WHERE intdeviceID=?sensorintdeviceID";
            parmss = new MySqlParameter[]
            {
                new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                new MySqlParameter("?sensorIsGetGPSinfo", MySqlDbType.Int32),
            };
            parmss[0].Value = sensorintdeviceID;
            parmss[1].Value = IsGetGPSinfo;

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


    }
}