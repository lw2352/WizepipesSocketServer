using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace WizepipesSocketServer
{
    class ZedToPng
    {
        //TODO:本地测试程序，分析窗口卡死的原因和内存占用
        private static ZedGraphControl zedGraphControl1 = new ZedGraphControl();
        private static GraphPane paneA = new GraphPane();
        private static PointPairList listA = new PointPairList();

        public static string SaveDataToPng(double[] data,int id)
        {         
            paneA.Title.IsVisible = false;
            paneA.XAxis.IsVisible = false;
            paneA.YAxis.IsVisible = false;
            if (id != 0)
            {
                paneA.XAxis.Scale.Max = 300000;
            }
            else
            {
                paneA.XAxis.Scale.Max = 550000;
            }

            for (int i = 0; i < data.Length; i++)
            {
                listA.Add(i, data[i]);
            }
            LineItem myCurveA = paneA.AddCurve("", listA, Color.Blue, SymbolType.None);
            
            using (Graphics g = zedGraphControl1.CreateGraphics())
            {
                paneA.AxisChange(g);
                paneA.ReSize(g, new RectangleF(0, 0, 1800, 300));
            }

            string url = @"D:\\AdImages\\";
            string filename = DateTime.Now.ToString("yyyy-MM-dd") + "--" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "--" + id.ToString();//以日期时间命名，避免文件名重复
            string strName = url + filename + ".png";

            if (!Directory.Exists(url))//如果不存在就创建file文件夹　　             　　                
            {               
                Directory.CreateDirectory(url);//创建该文件夹　
                paneA.GetImage().Save(strName, ImageFormat.Png);
            }
            else
            {
                paneA.GetImage().Save(strName, ImageFormat.Png);
            }
            return strName;
        }
    }
}
