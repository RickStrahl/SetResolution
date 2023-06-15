﻿/*
 * Based on original code from:
 *
 * (c) Mohammad Elsheimy
 * Changing Display Settings Programmatically
 *
 * https://www.c-sharpcorner.com/uploadfile/GemingLeader/changing-display-settings-programmatically/
 *
 * Added support for:
 *
 * * Listing Monitors and Monitor specific display modes
 * * Set display mode for a specific monitor/driver
 *
*/

using System;
using System.Runtime.InteropServices;

namespace Westwind.SetResolution
{
    static class DisplayManagerNative
    {
        #region Enum Display Settings

        [DllImport("User32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplaySettings(
            //[param: MarshalAs(UnmanagedType.LPTStr)]
            //string lpszDeviceName, //display device
            byte[] lpszDeviceName,  // display device   ToLPTStr
            [param: MarshalAs(UnmanagedType.U4)]
            int iModeNum,         // graphics mode
            [In, Out]
            ref DEVMODE lpDevMode       // graphics mode settings
        );

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;

        [Flags()]
        public enum DmFlags : int
        {
            DM_ORIENTATION = 0x00000001,
            DM_PAPERSIZE = 0x00000002,
            DM_PAPERLENGTH = 0x00000004,
            DM_PAPERWIDTH = 0x00000008,
            DM_SCALE = 0x00000010,
            DM_POSITION = 0x00000020,
            DM_NUP = 0x00000040,
            DM_DISPLAYORIENTATION = 0x00000080,
            DM_COPIES = 0x00000100,
            DM_DEFAULTSOURCE = 0x00000200,
            DM_PRINTQUALITY = 0x00000400,
            DM_COLOR = 0x00000800,
            DM_DUPLEX = 0x00001000,
            DM_YRESOLUTION = 0x00002000,
            DM_TTOPTION = 0x00004000,
            DM_COLLATE = 0x00008000,
            DM_FORMNAME = 0x00010000,
            DM_LOGPIXELS = 0x00020000,
            DM_BITSPERPEL = 0x00040000,
            DM_PELSWIDTH = 0x00080000,
            DM_PELSHEIGHT = 0x00100000,
            DM_DISPLAYFLAGS = 0x00200000,
            DM_DISPLAYFREQUENCY = 0x00400000,
            DM_ICMMETHOD = 0x00800000,
            DM_ICMINTENT = 0x01000000,
            DM_MEDIATYPE = 0x02000000,
            DM_DITHERTYPE = 0x04000000,
            DM_PANNINGWIDTH = 0x08000000,
            DM_PANNINGHEIGHT = 0x10000000,
            DM_DISPLAYFIXEDOUTPUT = 0x20000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            // You can define the following constant
            // but OUTSIDE the structure because you know
            // that size and layout of the structure is very important
            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            // In addition you can define the last character array
            // as following:
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            //public Char[] dmDeviceName;

            // After the 32-bytes array
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public DmFlags dmFields;

            public POINTL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public short dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public short dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public short dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public short dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public short dmCollate;

            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            // Also can be defined as
            //[MarshalAs(UnmanagedType.ByValArray, 
            //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            //public Byte[] dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPanningHeight;

            /// <summary>
            /// Initializes the structure variables.
            /// </summary>
            public void Initialize()
            {
                this.dmDeviceName = new string(new char[32]);
                this.dmFormName = new string(new char[32]);
                this.dmSize = (ushort)Marshal.SizeOf(this);
            }
        }


        // 8-bytes structure
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public int x;
            public int y;
        }

        #endregion


        #region Enum DisplayDevices
        
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }
        #endregion


        #region Errors

        [DllImport("User32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings(
            [In, Out]
            ref DEVMODE lpDevMode,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwflags);


        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, 
            ref DEVMODE lpDevMode, 
            IntPtr hwnd,
            uint dwflags, 
            IntPtr lParam);



        [DllImport("kernel32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern uint FormatMessage(
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwFlags,
            [param: MarshalAs(UnmanagedType.U4)]
            uint lpSource,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwMessageId,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwLanguageId,
            [param: MarshalAs(UnmanagedType.LPTStr)]
            out string lpBuffer,
            [param: MarshalAs(UnmanagedType.U4)]
            uint nSize,
            [param: MarshalAs(UnmanagedType.U4)]
            uint arguments);

        public const uint FORMAT_MESSAGE_FROM_HMODULE = 0x800;

        public const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
        public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        public const uint FORMAT_MESSAGE_FLAGS = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM;

        #endregion

        #region Helpers

        public static byte[] ToLPTStr(string str)
        {
            if (str == null) return null;

            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
                lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }

        #endregion
    }

}
