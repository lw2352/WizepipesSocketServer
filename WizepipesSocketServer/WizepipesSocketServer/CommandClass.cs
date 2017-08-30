﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizepipesSocketServer
{
    class CmdItem
    {
        //读取/设置 当前开启和关闭时长--0x21
        public byte[] CmdReadCurrentOpenAndCloseTime = new byte[] { 0xA5, 0xA5, 0x21, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        public byte[] CmdSetCurrentOpenAndCloseTime = new byte[] { 0xA5, 0xA5, 0x21, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };

        //0x22采样开始和结束

        //(数据位用0x00填充)
        //上传AD数据包--0x23
        public byte[] CmdADPacket            = new byte[] { 0xA5, 0xA5, 0x23, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x06, 0x02, 0x57, 0x00, 0x00, 0x03, 0xE8, 0xFF, 0x5A, 0x5A };
        //设定GPS采样时间,byte[9]是小时，byte[10]是分钟--0x25
        public byte[] CmdReadCapTime          = new byte[] { 0xA5, 0xA5, 0x25, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        //8-22 共24组小时-分钟组合，占2*24=48个字节
        public byte[] CmdSetCapTime          = new byte[] { 0xA5, 0xA5, 0x25, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x30,
            0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00,
            0xFF, 0x5A, 0x5A };

        //设置开启和关闭时长--0x26
        public byte[] CmdReadOpenAndCloseTime = new byte[] { 0xA5, 0xA5, 0x26, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        public byte[] CmdSetOpenAndCloseTime = new byte[] { 0xA5, 0xA5, 0x26, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        //读取经纬度--0x27
        public byte[] CmdReadGPSData         = new byte[] { 0xA5, 0xA5, 0x27, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x5A, 0x5A };

        //0x28设置立即采样
        public byte[] CmdSetCapTimeTemporary = new byte[] { 0xA5, 0xA5, 0x28, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };

        //(数据位用0x00填充)     
        //设置/读取服务器IP
        public byte[] CmdSetServerIP = new byte[] { 0xA5, 0xA5, 0x29, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        public byte[] CmdReadServerIP = new byte[] { 0xA5, 0xA5, 0x29, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        //设置/读取服务器端口号
        public byte[] CmdSetServerPort = new byte[] { 0xA5, 0xA5, 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x02, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        public byte[] CmdReadServerPort = new byte[] { 0xA5, 0xA5, 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x02, 0x00, 0x00, 0xFF, 0x5A, 0x5A };

        //设置/读取AP名--0x31
        public byte[] CmdSetAPssid = new byte[] { 0xA5, 0xA5, 0x31, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        public byte[] CmdReadAPssid = new byte[] { 0xA5, 0xA5, 0x31, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        //设置AP的密码--0x32
        public byte[] CmdSetAPpassword = new byte[] { 0xA5, 0xA5, 0x32, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        public byte[] CmdReadAPpassword = new byte[] { 0xA5, 0xA5, 0x32, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x5A, 0x5A };
        //让设备重新联网--0x33
        public byte[] CmdReconnectTcp        = new byte[] { 0xA5, 0xA5, 0x33, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x01, 0xFF, 0xFF, 0x5A, 0x5A };

        //0x34进入休眠
        public static byte[] CmdEnterSleep = new byte[] { 0xA5, 0xA5, 0x34, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x01, 0xFF, 0xFF, 0x5A, 0x5A };
    }
}
