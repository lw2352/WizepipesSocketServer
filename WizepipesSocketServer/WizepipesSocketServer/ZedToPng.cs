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
        public static void SaveDataToPng(int id)
        {
            ZedGraphControl zedGraphControl1 = new ZedGraphControl();
            GraphPane paneA = new GraphPane();
            PointPairList listA = new PointPairList();

            paneA.Title.IsVisible = false;
            paneA.XAxis.Scale.Max = 300000;
            paneA.XAxis.IsVisible = false;
            paneA.YAxis.IsVisible = false;
            LineItem myCurveA = paneA.AddCurve("", listA, Color.Blue, SymbolType.None);

            using (Graphics g = zedGraphControl1.CreateGraphics())
            {
                paneA.ReSize(g, new RectangleF(0, 0, 1800, 300));
            }

            string url = @"D:\\AdImages\\";
            string filename = DateTime.Now.ToString("yyyy-MM-dd") + "--" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "--" + 123.ToString();//以日期时间命名，避免文件名重复
            

            if (!Directory.Exists(url))//如果不存在就创建file文件夹　　             　　                
            {
                Directory.CreateDirectory(url);//创建该文件夹　
                paneA.GetImage().Save(url + filename + ".png", ImageFormat.Png);
            }
            else
            {
                paneA.GetImage().Save(url + filename + ".png", ImageFormat.Png);
            }
        }
    }
}
