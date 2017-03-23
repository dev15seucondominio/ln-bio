using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
    class Win32
    {
        public static byte[] gpImage = new byte[256 * 296];
        public static byte[] gpBin = new byte[256 * 296];
        public static byte[] gpFeature = new byte[256];
        public static byte[] gpFeatureA = new byte[256];
        public static byte[] gpFeatureB = new byte[256];
        public static byte[] gpFeatureBuf1 = new byte[512];
        public static byte[] gpFeatureBuf2 = new byte[512];
        public static byte[] gpFeatureLib1 = new byte[10000 * 256];
        public static byte[] gpFeatureLib2 = new byte[10000 * 256];
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzFindDevice(byte[] pDeviceName);
        [DllImport("AvzScanner.dll")]
        public static extern UInt32 AvzOpenDevice(Int16 uDeviceID, UInt32 hWnd);
        [DllImport("AvzScanner.dll")]
        public static extern UInt32 AvzCloseDevice(Int16 uDeviceID);
        [DllImport("AvzScanner.dll")]
        public static extern UInt32 AvzGetID(Int16 wDeviceID, byte[] pID);
        [DllImport("AvzScanner.dll")]
        public static extern void AvzGetImage(Int16 wDeviceID, byte[] pImage, ref UInt16 uStatus);
        [DllImport("AvzScanner.dll")]
        public static extern void AvzGetImage(Int16 wDeviceID, IntPtr pImage, ref UInt16 uStatus);
        [DllImport("AvzScanner.dll")]
        public static extern UInt32 AvzProcess(byte[] pimagein, byte[] feature, byte[] pimagebin, byte bthin, byte bdrawfea, UInt16 comst);
        [DllImport("AvzScanner.dll")]
        public static extern UInt32 AvzProcess(IntPtr ptr, byte[] feature, byte[] pimagebin, byte bthin, byte bdrawfea);
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzSaveHueBMPFile([MarshalAs(UnmanagedType.LPStr)]String pName, byte[] praw);
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzSaveClrBMPFile([MarshalAs(UnmanagedType.LPStr)]String pName, byte[] praw);
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzShowImage(System.IntPtr pHND, byte[] praw, UInt32 a, UInt32 b, UInt32 c, UInt32 d, UInt32 e, UInt32 f, UInt32 g, UInt32 h);
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzPackFeature(byte[] pFeature1, byte[] pFeature2, byte[] pFeatureBuf);
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzUnpackFeature(byte[] pFeatureBuf, [Out] byte[] pFeature1, [Out] byte[] pFeature2);
        [DllImport("AvzScanner.dll")]
        public static extern UInt16 AvzMatch(byte[] pFeature1, byte[] pFeature2, UInt16 level, UInt16 rotate);
        [DllImport("AvzScanner.dll")]
        public static extern Int16 AvzMatchN(byte[] pFeature, byte[] gpFeatureLib, UInt32 FingerNum, UInt16 level, UInt16 rotate);
        [DllImport("AvzScanner.dll")]
        public static extern Int16 AvzCloseLED(Int16 uDeviceID);
        [DllImport("AvzScanner.dll")]
        public static extern Int16 AvzOpenLED(Int16 uDeviceID);
    }
}

