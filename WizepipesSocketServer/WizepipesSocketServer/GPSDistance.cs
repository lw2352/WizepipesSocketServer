using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizepipesSocketServer
{
    /// <summary>
    /// 计算根据GPS数据计算经纬度和距离
    /// </summary>
    class GPSDistance
    {
        private const double EARTH_RADIUS = 6378.137 * 1000;//地球半径，单位为米
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public static void getGPSData(int[] gpsData, out double latitude, out double longitude)
        {
            latitude = (gpsData[1] - 0x30) * 10.0 + (gpsData[2] - 0x30);

            latitude += ((gpsData[3] - 0x30) * 10 + (gpsData[4] - 0x30)) / 60.0;

            latitude += ((gpsData[6] - 0x30) / 10.0 + (gpsData[7] - 0x30) / 100.0 + (gpsData[8] - 0x30) / 1000.0 + (gpsData[9] - 0x30) / 10000.0 + (gpsData[10] - 0x30) / 100000.0) / 60.0;

            longitude = (gpsData[12] - 0x30) * 100 + (gpsData[13] - 0x30) * 10 + (gpsData[14] - 0x30);

            longitude += ((gpsData[15] - 0x30) * 10 + (gpsData[16] - 0x30)) / 60.0;

            longitude += ((gpsData[18] - 0x30) / 10.0 + (gpsData[19] - 0x30) / 100.0 + (gpsData[20] - 0x30) / 1000.0 + (gpsData[21] - 0x30) / 10000.0 + (gpsData[22] - 0x30) / 100000.0) / 60.0;
        }

        public static double getGpsDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                                               Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
    }
}
