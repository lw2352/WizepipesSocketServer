﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MyClassLibrary;

namespace WizepipesSocketServer
{
    class Net_Analyze
    {
        private static double[] g_DataA = new double[300000 - 10000];//归一化后的A
        private static double[] g_DataB = new double[300000 - 10000];
        //private static double[] g_DataC = new double[300000 - 10000];
        private static int g_DataLength = 300000 - 10000;
        private static int g_OffSet = 0;
        public static int cutNum = 10000;

        private static double averageA = 0;
        private static double averageB = 0;
        private static double varianceA = 0;
        private static double varianceB = 0;       

        /**
	 * 只遍历数组一次求方差，利用公式DX^2=EX^2-(EX)^2
	 * @param a
	 * @return
	 */
        private static double ComputeVariance(double[] a)
        {
            double variance = 0;//方差
            double sum = 0, sum2 = 0;
            int i = 0, len = a.Length;
            for (; i < len; i++)
            {
                sum += a[i];
                sum2 += a[i] * a[i];
            }
            variance = sum2 / len - (sum / len) * (sum / len);
            return variance;
        }

        public static double[] getDataA(string FileName)
        {
            int datalength = 0;
            double dataA_avg = 0;
            double[] dataA;

            FileStream fs = new FileStream(FileName, FileMode.Open);
            datalength = (int)((fs.Length - 2) / 2 - cutNum);
            dataA = new double[datalength];
            byte[] data = new byte[datalength * 2];

            fs.Seek(cutNum + 1, SeekOrigin.Begin);//跳过第一个字符
            fs.Read(data, 0, datalength * 2);
            fs.Dispose();

            for (int i = 0; i < datalength; i++)
                dataA[i] = (double)(data[2 * i] * 0x100 + data[2 * i + 1]);
            dataA_avg = 0;
            for (int i = 0; i < datalength; i++)
                dataA_avg += dataA[i] / datalength;

            //保存原始数据的平均值和均方差（标准差）
            averageA = dataA_avg;
            varianceA = Math.Sqrt(ComputeVariance(dataA));

            for (int i = 0; i < datalength; i++)//归一处理
                dataA[i] = (dataA[i] - dataA_avg) / dataA_avg;

            g_DataA = dataA;
            return dataA;
        }

        public static double[] getDataB(string FileName)
        {
            int datalength = 0;
            double dataB_avg = 0;
            double[] dataB;

            FileStream fs = new FileStream(FileName, FileMode.Open);
            datalength = (int)((fs.Length - 2) / 2 - cutNum);
            dataB = new double[datalength];
            byte[] data = new byte[datalength * 2];

            fs.Seek(cutNum + 1, SeekOrigin.Begin);//跳过第一个字符
            fs.Read(data, 0, datalength * 2);
            fs.Dispose();

            for (int i = 0; i < datalength; i++)
                dataB[i] = (double)(data[2 * i] * 0x100 + data[2 * i + 1]);
            dataB_avg = 0;
            for (int i = 0; i < datalength; i++)
                dataB_avg += dataB[i] / datalength;

            //保存原始数据的平均值和均方差（标准差）
            averageB = dataB_avg;
            varianceB = Math.Sqrt(ComputeVariance(dataB));

            for (int i = 0; i < datalength; i++)//归一处理
                dataB[i] = (dataB[i] - dataB_avg) / dataB_avg;

            g_DataB = dataB;
            return dataB;
        }

        public static double[] getAnalyzeDataC()
        {
            g_OffSet = 0;//初始化
            //使用互相关算法分析
            int len = 262144;
            double[] A = new double[len];
            double[] B = new double[len];

            for (int i = 0; i < len; i++)
            {
                A[i] = g_DataA[i];
                B[i] = g_DataB[i];
            }

            CrossCorrelation ccorr = new CrossCorrelation();
            double[] R = ccorr.Rxy(A, B);
            int R_length = R.Length;

            Position p = new Position();
            int location = p.max(R);
            int d = location - R_length / 2;

            g_OffSet = d;//偏移值

            return R;//经过互相关计算后的数组
        }

        public static int GetOffSet()
        {//返回偏移值
            return g_OffSet;
        }

        public static List<string> AutoAnalyze(int idA, int idB)
        {
            List<string> resultList = new List<string>();//计算结果列表

            string dateA = Net_Analyze_DB.readDataDate(idA);
            string dateB = Net_Analyze_DB.readDataDate(idB);
            if (dateA != "fail" && dateB != "fail")
            {
                DateTime dt1 = DateTime.Parse(dateA);
                DateTime dt2 = DateTime.Parse(dateB);
                TimeSpan ts = dt1.Subtract(dt2);

                double t = ts.TotalMinutes;

                if (t < 10)
                {
                    string pathA = Net_Analyze_DB.readDataPath(idA);
                    pathA = pathA.Replace("\\\\", "\\");

                    string pathB = Net_Analyze_DB.readDataPath(idB);
                    pathB = pathB.Replace("\\\\", "\\");

                    //计算的同时调用画图
                    string resultA = ZedToPng.SaveDataToPng(getDataA(pathA), idA);
                    string resultB = ZedToPng.SaveDataToPng(getDataB(pathB), idB);
                    string resultC = ZedToPng.SaveDataToPng(getAnalyzeDataC(), 0);

                    string offSet = g_OffSet.ToString();

                    resultList.Add(offSet);
                    resultList.Add(resultA);
                    resultList.Add(resultB);
                    resultList.Add(resultC);

                    resultList.Add(averageA.ToString());
                    resultList.Add(averageB.ToString());
                    resultList.Add(varianceA.ToString());
                    resultList.Add(varianceB.ToString());
                    //return resultList;
                }
                else
                {
                    resultList.Add("fail");
                    resultList.Add("fail");
                    resultList.Add("fail");
                    resultList.Add("fail");

                    resultList.Add("fail");
                    resultList.Add("fail");
                    resultList.Add("fail");
                    resultList.Add("fail");
                }
            }
            else
            {
                resultList.Add("fail");
                resultList.Add("fail");
                resultList.Add("fail");
                resultList.Add("fail");

                resultList.Add("fail");
                resultList.Add("fail");
                resultList.Add("fail");
                resultList.Add("fail");
            }
            return resultList;
        }
    }
}
