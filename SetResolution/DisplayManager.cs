/*
 * Original code based on:
 *
 * (c) Mohammad Elsheimy
 * Changing Display Settings Programmatically
 *
 * https://www.c-sharpcorner.com/uploadfile/GemingLeader/changing-display-settings-programmatically/
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Westwind.SetResolution
{
    /// <summary>
    /// Represents a wrapper to the native methods.
    /// </summary>
    public static class DisplayManager
    {
        /// <summary>
        /// Returns a DisplaySettings object encapsulates the current display settings.
        /// </summary>
        public static DisplaySettings GetCurrentSettings()
        {
            return CreateDisplaySettingsObject(-1, GetDeviceMode());
        }

        /// <summary>
        /// Changes the current display settings with the new settings provided. May throw InvalidOperationException if failed. Check the exception message for more details.
        /// </summary>
        /// <param name="set">The new settings.</param>
        /// <remarks>
        /// Internally calls ChangeDisplaySettings() native function.
        /// </remarks>
        public static void SetDisplaySettings(DisplaySettings set)
        {
            SafeNativeMethods.DEVMODE mode = GetDeviceMode();

            mode.dmPelsWidth = (uint)set.Width;
            mode.dmPelsHeight = (uint)set.Height;
            mode.dmDisplayOrientation = (uint)set.Orientation;
            mode.dmBitsPerPel = (uint)set.BitCount;
            mode.dmDisplayFrequency = (uint)set.Frequency;

            DisplayChangeResult result = (DisplayChangeResult)SafeNativeMethods.ChangeDisplaySettings(ref mode, 0);

            string msg = null;
            switch (result)
            {
                case DisplayChangeResult.BadDualView:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadDualView;
                    break;
                case DisplayChangeResult.BadParam:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadParam;
                    break;
                case DisplayChangeResult.BadFlags:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadFlags;
                    break;
                case DisplayChangeResult.NotUpdated:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_NotUpdated;
                    break;
                case DisplayChangeResult.BadMode:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadMode;
                    break;
                case DisplayChangeResult.Failed:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Failed;
                    break;
                case DisplayChangeResult.Restart:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Restart;
                    break;
            }

            if (msg != null)
                throw new InvalidOperationException(msg);
        }


        /// <summary>
        /// Returns a list of all the display settings
        /// </summary>
        /// <returns></returns>
        public static List<DisplaySettings> GetAllDisplayModes()
        {
            var list = new List<DisplaySettings>();
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();

            mode.Initialize();

            int idx = 0;

            while (SafeNativeMethods.EnumDisplaySettings(null, idx, ref mode))
                list.Add(CreateDisplaySettingsObject(idx++, mode));

            return list;
        }

        /// <summary>
        /// Rotates the screen from its current location by 90 degrees either clockwise or anti-clockwise.
        /// </summary>
        /// <param name="clockwise">Set to true to rotate the screen 90 degrees clockwise from its current location, or false to rotate it anti-clockwise.</param>
        public static void RotateScreen(bool clockwise)
        {
            DisplaySettings set = DisplayManager.GetCurrentSettings();

            int tmp = set.Height;
            set.Height = set.Width;
            set.Width = tmp;

            if (clockwise)
                set.Orientation++;
            else
                set.Orientation--;

            if (set.Orientation < Orientation.Default)
                set.Orientation = Orientation.Clockwise270;
            else if (set.Orientation > Orientation.Clockwise270)
                set.Orientation = Orientation.Default;

            SetDisplaySettings(set);
        }


        /// <summary>
        /// A private helper methods used to derive a DisplaySettings object from the DEVMODE structure.
        /// </summary>
        /// <param name="idx">The mode index attached with the settings. Starts form zero. Is -1 for the current settings.</param>
        /// <param name="mode">The current DEVMODE object represents the display information to derive the DisplaySettings object from.</param>
        private static DisplaySettings CreateDisplaySettingsObject(int idx, SafeNativeMethods.DEVMODE mode)
        {
            return new DisplaySettings()
            {
                Index = idx,
                Width = (int)mode.dmPelsWidth,
                Height = (int)mode.dmPelsHeight,
                Orientation = (Orientation)mode.dmDisplayOrientation,
                BitCount = (int)mode.dmBitsPerPel,
                Frequency = (int)mode.dmDisplayFrequency
            };
        }

        /// <summary>
        /// A private helper method used to retrieve current display settings as a DEVMODE object.
        /// </summary>
        /// <remarks>
        /// Internally calls EnumDisplaySettings() native function with the value ENUM_CURRENT_SETTINGS (-1) to retrieve the current settings.
        /// </remarks>
        private static SafeNativeMethods.DEVMODE GetDeviceMode()
        {
            SafeNativeMethods.DEVMODE mode = new SafeNativeMethods.DEVMODE();

            mode.Initialize();

            if (SafeNativeMethods.EnumDisplaySettings(null, SafeNativeMethods.ENUM_CURRENT_SETTINGS, ref mode))
                return mode;
            else
                throw new InvalidOperationException(GetLastError());
        }

        private static string GetLastError()
        {
            int err = Marshal.GetLastWin32Error();

            string msg;

            if (SafeNativeMethods.FormatMessage(SafeNativeMethods.FORMAT_MESSAGE_FLAGS, SafeNativeMethods.FORMAT_MESSAGE_FROM_HMODULE, (uint)err, 0, out msg, 0, 0) == 0)
                return Properties.Resources.InvalidOperation_FatalError;
            else
                return msg;
        }
    }

    public enum Orientation
    {
        Default = 0,
        Clockwise90 = 1,
        Clockwise180 = 2,
        Clockwise270 = 3
    }

    public class DisplaySettings
    {
        public int Index { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Orientation Orientation { get; set; }
        public int BitCount { get; set; }
        public int Frequency { get; set; }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,
                "{0} x {1}, {2} Bit, {3} Hertz",
                Width, Height, BitCount, Frequency);
        }
    }

    enum DisplayChangeResult
    {
        /// <summary>
        /// Windows XP: The settings change was unsuccessful because system is DualView capable.
        /// </summary>
        BadDualView = -6,
        /// <summary>
        /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
        /// </summary>
        BadParam = -5,
        /// <summary>
        /// An invalid set of flags was passed in.
        /// </summary>
        BadFlags = -4,
        /// <summary>
        /// Windows NT/2000/XP: Unable to write settings to the registry.
        /// </summary>
        NotUpdated = -3,
        /// <summary>
        /// The graphics mode is not supported.
        /// </summary>
        BadMode = -2,
        /// <summary>
        /// The display driver failed the specified graphics mode.
        /// </summary>
        Failed = -1,
        /// <summary>
        /// The settings change was successful.
        /// </summary>
        Successful = 0,
        /// <summary>
        /// The computer must be restarted in order for the graphics mode to work.
        /// </summary>
        Restart = 1
    }


    static class SafeNativeMethods
    {
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
            public UInt16 dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmFields;

            public POINTL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmCollate;

            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            // Also can be defined as
            //[MarshalAs(UnmanagedType.ByValArray, 
            //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            //public Byte[] dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningHeight;

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
            public Int32 x;
            public Int32 y;
        }



        [DllImport("User32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumDisplaySettings(
            [param: MarshalAs(UnmanagedType.LPTStr)]
            String lpszDeviceName,  // display device
            [param: MarshalAs(UnmanagedType.U4)]
            Int32 iModeNum,         // graphics mode
            [In, Out]
            ref DEVMODE lpDevMode       // graphics mode settings
            );

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;



        [DllImport("User32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings(
            [In, Out]
            ref DEVMODE lpDevMode,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwflags);



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
            uint Arguments);

        public const uint FORMAT_MESSAGE_FROM_HMODULE = 0x800;

        public const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
        public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        public const uint FORMAT_MESSAGE_FLAGS = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM;
    }
}