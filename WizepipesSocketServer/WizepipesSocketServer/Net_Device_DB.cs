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

        public static string UpdateSensorInfo(int sensorintdeviceID, string updateItem, int updateNum)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tsensorinfo SET " + updateItem + " =?sensorupdateItem WHERE intdeviceID=?sensorintdeviceID";
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

        public static string UpdateSensorInfoWithTime(int sensorintdeviceID, string updateItem, string updateNum)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tsensorinfo SET " + updateItem + " =?sensorupdateItem WHERE intdeviceID=?sensorintdeviceID";
            parmss = new MySqlParameter[]
            {
                new MySqlParameter("?sensorintdeviceID", MySqlDbType.Int32),
                new MySqlParameter("?sensorupdateItem", MySqlDbType.DateTime)
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
        public static List<int> readsensorcfg(int sensorintdeviceID)
        {
            List<int> resultList = new List<int>();

            List<string> itemList = new List<string>();
            itemList.Add("IsSetCapTime");
            itemList.Add("IsSetOpenAndCloseTime");
            itemList.Add("IsCaptureNow");
            itemList.Add("IsGetGpsInfo");
            itemList.Add("IsSetApName");
            itemList.Add("IsSetApPassword");
            itemList.Add("IsSetServerIP");
            itemList.Add("IsSetServerPort");
            itemList.Add("IsReconnect");

            MySQLDB.InitDb();

            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensorcfg");
                string strSQL1 =
                    "SELECT * FROM tsensorcfg where intdeviceID=" + sensorintdeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    // 有数据集
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < itemList.Count; i++)
                        {
                            string item = (ds1.Tables[0].Rows[0][itemList[i]]).ToString();
                            if (item != null && item != "")
                            {
                                resultList.Add(Convert.ToInt32(item));
                            }
                            else resultList.Add(0);
                        }

                        //0-8，共9个
                        /*var item0 = ds1.Tables[0].Rows[0]["IsSetCapTime"];
                        if (item0 != null)
                        {
                            resultList.Add(Convert.ToInt32(item0));
                        }
                        else resultList.Add(0);

                        var item1 = ds1.Tables[0].Rows[0]["IsSetOpenAndCloseTime"].ToString();
                        if (item1 != null)
                        {
                            resultList.Add(Convert.ToInt32(item1));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsCaptureNow"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsCaptureNow"]));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsGetGpsInfo"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsGetGpsInfo"]));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsSetApName"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsSetApName"]));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsSetApPassword"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsSetApPassword"]));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsSetServerIP"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsSetServerIP"]));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsSetServerPort"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsSetServerPort"]));
                        }
                        else resultList.Add(0);

                        if ((ds1.Tables[0].Rows[0]["IsReconnect"]) != null)
                        {
                            resultList.Add(Convert.ToInt32(ds1.Tables[0].Rows[0]["IsReconnect"]));
                        }
                        else resultList.Add(0);*/


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

        public static string readsensorcfgItem(int sensorintdeviceID, string readItem)
        {
            string resultItem = null;
            MySQLDB.InitDb();

            try
            {
                DataSet ds1 = new DataSet("tsensorcfg");
                string strSQL1 =
                    "SELECT " + readItem + " FROM tsensorcfg where intdeviceID=" + sensorintdeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    // 有数据集
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        //下面8个
                        /*resultList.Add(ds1.Tables[0].Rows[0]["CapTimeHour"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["CapTimeMinute"] as string);

                        resultList.Add(ds1.Tables[0].Rows[0]["OpenTime"] as string);
                        resultList.Add(ds1.Tables[0].Rows[0]["CloseTime"] as string);

                        resultList.Add(ds1.Tables[0].Rows[0]["ApName"] as string);

                        resultList.Add(ds1.Tables[0].Rows[0]["ApPassword"] as string);

                        resultList.Add(ds1.Tables[0].Rows[0]["ServerIP"] as string);

                        resultList.Add(ds1.Tables[0].Rows[0]["ServerPort"] as string);*/

                        string item = (ds1.Tables[0].Rows[0][0]).ToString();
                        if (item != null && item != "")
                        {
                            resultItem = item;
                        }
                        return resultItem;
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
                "Update tsensorcfg SET " + updateItem + " =?sensorupdateItem WHERE (intdeviceID=?sensorintdeviceID)";
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

        public static List<int> GetpipeInfo(int idA, int idB)
        {
            MySQLDB.InitDb();
            int sensorAID = 0;
            int sensorBID = 0;
            int pipeID = 0;
            int pipeLength = 0;
            List<int> resultList = new List<int>();
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tsensor");
                string strSQL1 =
                    "SELECT SensorID FROM tsensor where (intdeviceID=" + idA + " or intdeviceID=" + idB + ")";
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    // 有数据集
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        string item0 = (ds1.Tables[0].Rows[0]["SensorID"]).ToString();
                        if (item0 != null && item0 != "")
                        {
                            sensorAID = (Convert.ToInt32(item0));
                        }
                        else sensorAID = 0;

                        string item1 = (ds1.Tables[0].Rows[1]["SensorID"]).ToString();
                        if (item1 != null && item1 != "")
                        {
                            sensorBID = (Convert.ToInt32(item1));
                        }
                        else sensorBID = 0;

                    }
                } //end of if (ds1 != null)

                if (sensorAID != 0 && sensorBID != 0)
                {
                    DataSet ds2 = new DataSet("tsensormatch");
                    string strSQL2 =
                        "SELECT PipeID FROM tsensormatch where ((SensorAID=" + sensorAID + " and SensorBID=" +
                        sensorBID + ") or (SensorAID=" + sensorBID + " and SensorBID=" + sensorAID + "))";
                    ds2 = MySQLDB.SelectDataSet(strSQL2, null);
                    if (ds2 != null)
                    {
                        // 有数据集
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            string item3 = (ds2.Tables[0].Rows[0]["PipeID"]).ToString();
                            if (item3 != null && item3 != "")
                            {
                                pipeID = (Convert.ToInt32(item3));
                            }
                            else pipeID = 0;
                        }
                    }
                } //end of if (sensorAID != 0 && sensorBID != 0)

                if (pipeID != 0)
                {
                    DataSet ds3 = new DataSet("tpipe");
                    string strSQL3 =
                        "SELECT PipeLength FROM tpipe where PipeID=" + pipeID;
                    ds3 = MySQLDB.SelectDataSet(strSQL3, null);
                    if (ds3 != null)
                    {
                        // 有数据集
                        if (ds3.Tables[0].Rows.Count > 0)
                        {
                            string length = (ds3.Tables[0].Rows[0]["PipeLength"]).ToString();
                            if (length != null && length != "")
                            {
                                pipeLength = (Convert.ToInt32(length));
                            }
                            else pipeLength = 0;
                        }
                    }
                }
                //读数据以存储的经纬度
                if (pipeLength == 0)
                {
                    double lng1, lat1, lng2, lat2;
                    lng1 = lat1 = lng2 = lat2 = 0;

                    DataSet ds4 = new DataSet("tpipe");
                    string strSQL4 =
                        "SELECT StartLocation, EndLocation FROM tpipe where PipeID=" + pipeID;
                    ds4 = MySQLDB.SelectDataSet(strSQL4, null);
                    if (ds4 != null)
                    {
                        // 有数据集
                        if (ds4.Tables[0].Rows.Count > 0)
                        {
                            string StartLocation = (ds4.Tables[0].Rows[0]["StartLocation"]).ToString();
                            if (StartLocation != null && StartLocation != "")
                            {
                                string[] sArray = StartLocation.Split(new char[] { ',' });
                                lng1 = Convert.ToDouble(sArray[0]);
                                lat1 = Convert.ToDouble(sArray[1]);
                            }
                            else lng1 = lat1 = 0;

                            string EndLocation = (ds4.Tables[0].Rows[0]["EndLocation"]).ToString();
                            if (EndLocation != null && EndLocation != "")
                            {
                                string[] sArray = EndLocation.Split(new char[] { ',' });
                                lng2 = Convert.ToDouble(sArray[0]);
                                lat2 = Convert.ToDouble(sArray[1]);
                            }
                            else lng2 = lat2 = 0;
                        }
                        if ((lng1 != 0) && (lat1 != 0) && (lng2 != 0) && (lat2 != 0))
                        {
                            pipeLength = (int)GPSDistance.getGpsDistance(lng1, lat1, lng2, lat2);
                        }
                        else pipeLength = 0;
                    }
                }
                //读设备的经纬度
                if (pipeLength == 0)
                {
                    double lng1, lat1, lng2, lat2;
                    lng1 = lat1 = lng2 = lat2 = 0;

                    DataSet ds5 = new DataSet("tsensorinfo");
                    string strSQL5 =
                        "SELECT Longitude, Latitude FROM tsensorinfo where intdeviceID=" + idA + " or intdeviceID=" +
                        idB;
                    ds5 = MySQLDB.SelectDataSet(strSQL5, null);
                    if (ds5 != null)
                    {
                        // 有数据集
                        if (ds5.Tables[0].Rows.Count > 0)
                        {
                            string LongitudeA = (ds5.Tables[0].Rows[0]["Longitude"]).ToString();
                            string LongitudeB = (ds5.Tables[0].Rows[1]["Longitude"]).ToString();
                            string LatitudeA = (ds5.Tables[0].Rows[0]["Latitude"]).ToString();
                            string LatitudeB = (ds5.Tables[0].Rows[1]["Latitude"]).ToString();
                            if (LongitudeA != "" && LongitudeB != "" && LatitudeA != "" && LatitudeB != "")
                            {
                                lng1 = Convert.ToDouble(LongitudeA);
                                lng2 = Convert.ToDouble(LongitudeB);
                                lat1 = Convert.ToDouble(LatitudeA);
                                lat2 = Convert.ToDouble(LatitudeB);
                            }
                            else lng1 = lat1 = lng2 = lat2 = 0;
                        }
                        if ((lng1 != 0) && (lat1 != 0) && (lng2 != 0) && (lat2 != 0))
                        {
                            pipeLength = (int)GPSDistance.getGpsDistance(lng1, lat1, lng2, lat2);
                        }
                        else pipeLength = 0;
                    }
                }

                resultList.Add(sensorAID);
                resultList.Add(sensorBID);
                resultList.Add(pipeID);
                resultList.Add(pipeLength);
                return resultList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string[] GetSensorNameAndID(int intDeviceID)
        {
            string[] resultItem = new string[2];
            MySQLDB.InitDb();
            string sensorName = null;
            string sensorID = null;
            try
            {
                DataSet ds1 = new DataSet("tsensor");
                string strSQL1 =
                    "SELECT SensorID, SensorName FROM tsensor where IntdeviceID=" + intDeviceID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    // 有数据集
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        sensorID = (ds1.Tables[0].Rows[0]["SensorID"]).ToString();
                        sensorName = (ds1.Tables[0].Rows[0]["SensorName"]).ToString();
                    }
                }
                if (sensorID != "" && sensorName != "")
                {
                    resultItem[0] = sensorID;
                    resultItem[1] = sensorName;
                    return resultItem;
                }
                
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        #endregion

        public static int[,] GetDevicePair()
        {
            string resultItem = null;
            MySQLDB.InitDb();

            try
            {
                DataSet ds1 = new DataSet("tmultiuser");
                string strSQL1 =
                    "SELECT * FROM tmultiuser";
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    int i = 0;
                    int j = 0;
                    int k = 0;
                    int l = 0;
                    int count = ds1.Tables[0].Rows.Count;
                    while (j < count)
                    {
                        string item = (ds1.Tables[0].Rows[j]["IsCapture"]).ToString();
                        if (item != "" && Convert.ToInt32(item) == 1)
                        {
                            k++;
                        }
                        j++;
                    }
                    int[,] result = new int[k,3];
                    while (i < count && l<k)
                    {
                        string item = (ds1.Tables[0].Rows[i]["IsCapture"]).ToString();
                        if (item != "" && Convert.ToInt32(item) == 1)
                        {
                            result[l,0] = (int)(ds1.Tables[0].Rows[i]["userID"]);
                            result[l,1] = (int)(ds1.Tables[0].Rows[i]["SensorAID"]);
                            result[l,2] = (int)(ds1.Tables[0].Rows[i]["SensorBID"]);
                            l++;
                        }
                        i++;
                    }//end  of while
                    return result;
                }
                else return null;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string UpdateMultiUser(string updateItem, int userID, int updateNum)
        {
            MySQLDB.InitDb();
            string strResult = "";
            MySqlParameter[] parmss = null;
            string strSQL = "";
            bool IsDelSuccess = false;
            strSQL =
                "Update tmultiuser SET " + updateItem + " =?sensorupdateItem WHERE userID=?userid";
            parmss = new MySqlParameter[]
            {
                new MySqlParameter("?userid", MySqlDbType.Int32),
                new MySqlParameter("?sensorupdateItem", MySqlDbType.Int32),
            };
            parmss[0].Value = userID;
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

        public static string UpdateLeakTimes(int pipeID)
        {
            MySQLDB.InitDb();
            int times = -1;
            MySqlParameter[] parmss = null;
            bool IsDelSuccess = false;
            //从数据库中查找当前ID是否存在
            try
            {
                DataSet ds1 = new DataSet("tpipeleakreport");
                string strSQL1 =
                    "SELECT LeakTimes FROM tpipeleakreport where pipeID=" + pipeID;
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    // 有数据集
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        times = (int)(ds1.Tables[0].Rows[0]["LeakTimes"]);
                    }
                }

                if (times == -1) //没有对应的pipeID，需要插入
                {
                    MySqlParameter[] parmssInsert = null;
                    string strSQL = "";
                    bool IsDelSuccessInsert = false;
                    strSQL = " insert into tpipeleakreport (PipeID,LeakTimes) values" +
                             " (?pipeID,?leakTimes);";

                    parmssInsert = new MySqlParameter[]
                    {
                        new MySqlParameter("?pipeID", MySqlDbType.Int32),
                        new MySqlParameter("?leakTimes", MySqlDbType.Int32)
                    };
                    parmssInsert[0].Value = pipeID;
                    parmssInsert[1].Value = 1;

                    try
                    {
                        IsDelSuccessInsert = MySQLDB.ExecuteNonQry(strSQL, parmssInsert);

                        if (IsDelSuccessInsert != false)
                        {
                            return "1";
                        }
                        else
                        {
                            return null;
                        }
                    }

                    catch (Exception ex)
                    {
                        return null; //数据库异常
                    }
                }
                else
                {

                    string strSQL2 =
                        "UPDATE tpipeleakreport SET LeakTimes=?leakTimes where pipeID=" + pipeID;
                    parmss = new MySqlParameter[]
                    {
                        new MySqlParameter("?leakTimes", MySqlDbType.Int32),
                    };
                    parmss[0].Value = ++times;

                    try
                    {
                        IsDelSuccess = MySQLDB.ExecuteNonQry(strSQL2, parmss);

                        if (IsDelSuccess != false)
                        {
                            return times.ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }

                    catch (Exception ex)
                    {
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string getSensorIsPipeStart(string PipeID, string SensorID)
        {
            MySQLDB.InitDb();
            try
            {
                DataSet ds = new DataSet("tsensorid");
                string strSQL = "SELECT * FROM tsensor WHERE PipeID = ?PipeID and SensorID = ?SensorID";

                MySqlParameter[] parms = null;


                parms = new MySqlParameter[]
                {
                    new MySqlParameter("?PipeID", MySqlDbType.Int32),
                    new MySqlParameter("?SensorID", MySqlDbType.Int32)
                };

                parms[0].Value = Convert.ToInt32(PipeID);
                parms[0].Value = Convert.ToInt32(SensorID);

                ds = MySQLDB.SelectDataSet(strSQL, parms);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                        // 有数据集
                    {
                        //SencondHead为1是尾部，0为头部
                        string IsHead = (ds.Tables[0].Rows[0]["SencondHead"]).ToString();
                        return IsHead;
                    }
                    else
                    {
                        //头部
                        return "0";
                    };
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string UpdateLeakPointScale(int pipeID, string updateItem,string item)
        {
            MySQLDB.InitDb();
            MySqlParameter[] parmss = null;
            bool IsDelSuccess = false;

            string strSQL2 =
                "UPDATE tpipe SET "+ updateItem+" =?item where pipeID=" + pipeID;
            
            if (updateItem == "LeakPointScale")
            {
                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?item", MySqlDbType.VarChar)
                };
                parmss[0].Value = item;

            }
            else if (updateItem == "Status")
            {
                parmss = new MySqlParameter[]
                {
                    new MySqlParameter("?item", MySqlDbType.Int32)
                };
                parmss[0].Value = Convert.ToInt32(item);
            }

            try
            {
                IsDelSuccess = MySQLDB.ExecuteNonQry(strSQL2, parmss);

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

        public static List<int[]> readAllDeviceIdByAreaID()
        {
            MySQLDB.InitDb();
            int[] result;
            List<int[]> returnList;
            try
            {
                DataSet ds1 = new DataSet("tuser");
                string strSQL1 =
                    "SELECT distinct AreaID FROM tuser order by AreaID ASC";//返回不重复的目标字段
                ds1 = MySQLDB.SelectDataSet(strSQL1, null);
                if (ds1 != null)
                {
                    int i = 0;
                    int count = ds1.Tables[0].Rows.Count;
                    if (count > 0)
                    {
                        result = new int[count];
                        returnList = new List<int[]>(count);//初始化
                        while (i < count)
                        {
                            string item = (ds1.Tables[0].Rows[i]["AreaID"]).ToString();
                            if (item != "")
                            {
                                result[i] = Convert.ToInt32(item);
                            }
                            i++;
                        }
                    }
                    else return null;
                }
                else return null;

                for (int i = 0; i < result.Length; i++)
                {
                    DataSet ds2 = new DataSet("tsensor");
                    string strSQL2 =
                        "SELECT distinct IntdeviceID FROM tsensor where AreaID = "+result[i]+ " and IntdeviceID is not null order by IntdeviceID ASC"; //返回不重复的目标字段
                    ds2 = MySQLDB.SelectDataSet(strSQL2, null);
                    if (ds2 != null)
                    {
                        int j = 0;
                        int k = 0;
                        int count = ds2.Tables[0].Rows.Count;
                        if (count > 0)
                        {
                            int[] temp = new int[count];
                            while (j < count)
                            {
                                string item = (ds2.Tables[0].Rows[j]["IntdeviceID"]).ToString();
                                if (item != "")
                                {
                                    temp[k++] = Convert.ToInt32(item);
                                }
                                j++;
                            }
                            returnList.Add(temp);
                        }
                        else returnList[i] = null;
                    }//end of if  
                    else returnList[i] = null;
                }

                return returnList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



    }
}