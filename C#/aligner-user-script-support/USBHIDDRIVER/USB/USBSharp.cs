using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace USBHIDDRIVER.USB
{
    /// <summary>
    /// Summary description
    /// </summary>
    internal class USBSharp : IDisposable
    {

        #region Constant Variables

        const int ERROR_IO_PENDING = 997;

        const int DIGCF_PRESENT = 0x00000002;
        const int DIGCF_DEVICEINTERFACE = 0x00000010;
        const int DIGCF_INTERFACEDEVICE = 0x00000010;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const int FILE_SHARE_READ = 0x00000001;
        const int FILE_SHARE_WRITE = 0x00000002;
        const int OPEN_EXISTING = 3;
        const int EV_RXFLAG = 0x0002;       // received certain character
        
        const int WAIT_TIMEOUT = 0x102;
        const short WAIT_OBJECT_0 = 0;
        const int INVALID_HANDLE_VALUE = -1;
        const int ERROR_INVALID_HANDLE = 6;
        const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            internal int nLength;
            internal IntPtr lpSecurityDescriptor;
            internal bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVINFO_DATA
        {
            internal int cbSize;
            internal Guid ClassGuid;
            internal int DevInst;
            internal IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal int cbSize;
            internal Guid InterfaceClassGuid;
            internal int Flags;
            internal IntPtr Reserved;
        }

        /// <summary>
        /// Device interface detail data
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            internal int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        struct NativeDeviceInterfaceDetailData
        {
            public int size;
            public char devicePath;
        }

        /// <summary>
        /// HIDD_ATTRIBUTES
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct HIDD_ATTRIBUTES
        {
            internal int Size;
            internal ushort VendorID;
            internal ushort ProductID;
            internal ushort VersionNumber;
        }

        // HIDP_CAPS
        [StructLayout(LayoutKind.Sequential)]
        internal struct HIDP_CAPS
        {
            internal short Usage;
            internal short UsagePage;
            internal short InputReportByteLength;
            internal short OutputReportByteLength;
            internal short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            internal short[] Reserved;
            internal short NumberLinkCollectionNodes;
            internal short NumberInputButtonCaps;
            internal short NumberInputValueCaps;
            internal short NumberInputDataIndices;
            internal short NumberOutputButtonCaps;
            internal short NumberOutputValueCaps;
            internal short NumberOutputDataIndices;
            internal short NumberFeatureButtonCaps;
            internal short NumberFeatureValueCaps;
            internal short NumberFeatureDataIndices;
        }

        /// <summary>
        /// HIDP_REPORT_TYPE 
        /// </summary>
        internal enum HIDP_REPORT_TYPE
        {
            HidP_Input,     // 0 input
            HidP_Output,    // 1 output
            HidP_Feature    // 2 feature
        }

        #endregion

        #region Structures in the union belonging to HIDP_VALUE_CAPS (see below)

        [StructLayout(LayoutKind.Sequential)]
        internal struct Range
        {
            internal ushort UsageMin;
            internal ushort UsageMax;
            internal ushort StringMin;
            internal ushort StringMax;
            internal ushort DesignatorMin;
            internal ushort DesignatorMax;
            internal ushort DataIndexMin;
            internal ushort DataIndexMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NotRange
        {
            internal ushort Usage;
            internal ushort Reserved1;
            internal ushort StringIndex;
            internal ushort Reserved2;
            internal ushort DesignatorIndex;
            internal ushort Reserved3;
            internal ushort DataIndex;
            internal ushort Reserved4;
        }

        //HIDP_VALUE_CAPS
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        internal struct HIDP_VALUE_CAPS
        {
            //
            [FieldOffset(0)] internal ushort UsagePage;
            [FieldOffset(2)] internal Byte ReportID;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(3)]
            internal Boolean IsAlias;
            [FieldOffset(4)] internal ushort BitField;
            [FieldOffset(6)] internal ushort LinkCollection;
            [FieldOffset(8)] internal ushort LinkUsage;
            [FieldOffset(10)] internal ushort LinkUsagePage;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(12)]
            internal Boolean IsRange;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(13)]
            internal Boolean IsStringRange;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(14)]
            internal Boolean IsDesignatorRange;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(15)]
            internal Boolean IsAbsolute;
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(16)]
            internal Boolean HasNull;
            [FieldOffset(17)] internal Char Reserved;  
            [FieldOffset(18)] internal ushort BitSize;    
            [FieldOffset(20)] internal ushort ReportCount;
            [FieldOffset(22)] internal ushort Reserved2a; 
            [FieldOffset(24)] internal ushort Reserved2b; 
            [FieldOffset(26)] internal ushort Reserved2c; 
            [FieldOffset(28)] internal ushort Reserved2d; 
            [FieldOffset(30)] internal ushort Reserved2e; 
            [FieldOffset(32)] internal ushort UnitsExp;   
            [FieldOffset(34)] internal ushort Units;      
            [FieldOffset(36)] internal Int16 LogicalMin;  
            [FieldOffset(38)] internal Int16 LogicalMax;  
            [FieldOffset(40)] internal Int16 PhysicalMin; 
            [FieldOffset(42)] internal Int16 PhysicalMax;                                            	
            [FieldOffset(44)] internal Range Range;
            [FieldOffset(44)] internal Range NotRange;
        }

        #endregion

        #region Variables
        internal string DevicePathName = "";
        internal IntPtr hHidFile = IntPtr.Zero;
        
        internal IntPtr hDevInfoSet = IntPtr.Zero;
        

        Guid hidClass = new Guid();
        HIDP_CAPS myHIDP_CAPS;

        //SP_DEVINFO_DATA deviceInfoData;
        SP_DEVICE_INTERFACE_DATA deviceInterfaceData;
        HIDD_ATTRIBUTES myHIDD_ATTRIBUTES;
        HIDP_VALUE_CAPS[] myHIDP_VALUE_CAPS;

        /// <summary>
        ///  Unmanaged buffer for input package over USB HID, 
        ///  The buffer is initialized after HIDP_CAPS returned.
        /// </summary>
        IntPtr unmanagedInputBuffer = IntPtr.Zero;

        #endregion

        #region HID APIs
        
        /// <summary>
        /// Get GUID for the HID Class
        /// </summary>
        /// <param name="lpHidGuid"></param>
        [DllImport("hid.dll", SetLastError = true)]
        static extern void HidD_GetHidGuid(ref Guid lpHidGuid);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetAttributes(
            IntPtr hidDeviceObject,
            ref HIDD_ATTRIBUTES Attributes);
        
        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetPreparsedData(
            IntPtr hidDeviceObject,
            ref IntPtr PreparsedData);
        
        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidP_GetCaps(
            IntPtr preparsedData,
            ref HIDP_CAPS capabilities);
        
        [DllImport("hid.dll", SetLastError = true)]
        internal static extern bool HidD_GetSerialNumberString(
            IntPtr hidDeviceObject,
            ref byte lpReportBuffer,
            int reportBufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_SetOutputReport(
            IntPtr hidDeviceObject,
            byte[] lpReportBuffer,
            int reportBufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetInputReport(
            IntPtr HidDeviceObject,
            ref byte lpReportBuffer,
            int ReportBufferLength);
        
        [DllImport("hid.dll", SetLastError = true)]
        private static extern int HidP_GetValueCaps(
            short reportType,
            [In, Out] HIDP_VALUE_CAPS[] valueCaps,
            ref short valueCapsLength,
            IntPtr preparsedData);
        
        [DllImport("hid.dll", SetLastError = true)]
        static extern int HidD_FreePreparsedData(
            IntPtr preparsedData
            );

        [DllImport("hid", SetLastError = true)]
        static extern bool HidD_GetNumInputBuffers(
            IntPtr HidDeviceObject,
            ref uint NumberBuffers
            );

        #endregion

        #region WIN32 APIs

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static internal extern bool CancelIo(
            IntPtr hFile);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static internal extern bool CancelIoEx(
            IntPtr hFile, IntPtr
            lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static internal extern bool CloseHandle(
            IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern int WaitForSingleObject(
            IntPtr hHandle, 
            int dwMilliseconds);
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static internal extern IntPtr CreateEvent(
            ref SECURITY_ATTRIBUTES SecurityAttributes,
            int bManualReset,
            int bInitialState,
            string lpName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static internal extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            int dwShareMode,
            ref SECURITY_ATTRIBUTES lpSecurityAttributes,
            int dwCreationDisposition,
            uint dwFlagsAndAttributes,
            int hTemplateFile);

        [DllImport("kernel32.dll")]
        static internal extern bool ReadFile(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            [In] ref NativeOverlapped lpOverlapped);
        
        [DllImport("kernel32.dll")]
        static internal extern bool WriteFile(
            IntPtr hFile,
            ref byte lpBuffer,
            int nNumberOfBytesToWrite,
            out int lpNumberOfBytesWritten,
            [In] ref NativeOverlapped lpOverlapped);


        /// <summary>
        /// Get array of structures with the HID info
        /// </summary>
        /// <param name="lpHidGuid"></param>
        /// <param name="Enumerator"></param>
        /// <param name="hwndParent"></param>
        /// <param name="Flags"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern IntPtr SetupDiGetClassDevs(
            ref Guid lpHidGuid,
            string Enumerator,
            int hwndParent,
            int Flags);


        [DllImport("setupapi.dll")]
        static internal extern bool SetupDiEnumDeviceInfo(
            IntPtr deviceInfoSet,
            int memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData);


        /// <summary>
        /// 
        /// Get context structure for a device interface element
        /// 
        ///   SetupDiEnumDeviceInterfaces returns a context structure for a device 
        ///   interface element of a device information set. Each call returns information 
        ///   about one device interface; the function can be called repeatedly to get information
        ///   about several interfaces exposed by one or more devices.
        /// 
        /// </summary>
        /// <param name="DeviceInfoSet"></param>
        /// <param name="DeviceInfoData"></param>
        /// <param name="lpHidGuid"></param>
        /// <param name="MemberIndex"></param>
        /// <param name="lpDeviceInterfaceData"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr DeviceInfoSet,
            IntPtr DeviceInfoData,
            ref Guid interfaceClassGuid,
            int MemberIndex,
            ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData);


        [DllImport("setupapi.dll", CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
        static internal extern bool SetupDiGetDeviceInterfaceDetailBuffer(
            IntPtr deviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            int deviceInterfaceDetailDataSize,
            ref int requiredSize,
            IntPtr deviceInfoData);


        /// <summary>
        /// 
        /// Get device Path name
        /// Works for second pass (overide), once size value is known
        /// 
        /// </summary>
        /// <param name="DeviceInfoSet"></param>
        /// <param name="lpDeviceInterfaceData"></param>
        /// <param name="myPSP_DEVICE_INTERFACE_DETAIL_DATA"></param>
        /// <param name="detailSize"></param>
        /// <param name="requiredSize"></param>
        /// <param name="bPtr"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        unsafe static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
            ref NativeDeviceInterfaceDetailData lpDeviceInterfaceDetail,
            int detailSize,
            ref int requiredSize,
            IntPtr bPtr);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern int SetupDiDestroyDeviceInfoList(
            IntPtr deviceInfoSet
            );

        #endregion

        #region Constructor

        public USBSharp()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get collection capability
        /// </summary>
        public HIDP_CAPS HidpCaps
        {
            get
            {
                return myHIDP_CAPS;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get class GUID
        /// </summary>
        internal void CT_HidGuid()
        {
            HidD_GetHidGuid(ref hidClass);
        }
        
        /// <summary>
        /// The functions returns a handle to a device information set that contains requested device(Specified by hidClass) informatioin elements for a local computer
        /// </summary>
        /// <returns></returns>
		internal IntPtr CT_SetupDiGetClassDevs()
        {
            hDevInfoSet = SetupDiGetClassDevs(
                ref hidClass,
                null,
                0,
                DIGCF_INTERFACEDEVICE | DIGCF_PRESENT);

            return hDevInfoSet;
        }
        
        internal bool CT_SetupDiEnumDeviceInterfaces(int memberIndex)
        {
            deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

            bool result = SetupDiEnumDeviceInterfaces(
                hDevInfoSet,
                IntPtr.Zero,
                ref hidClass,
                memberIndex,
                ref deviceInterfaceData);

            return result;
        }

        /// <summary>
        /// results = 0 is OK with the first pass of the routine since we are
        /// trying to get the RequiredSize parameter so in the next call we can read the entire detail
        /// </summary>
        /// <param name="RequiredSize"></param>
        /// <param name="DeviceInterfaceDetailDataSize"></param>
        /// <returns></returns>
        internal bool CT_SetupDiGetDeviceInterfaceBuffer(ref int RequiredSize, int DeviceInterfaceDetailDataSize)
        {

            bool results =
            SetupDiGetDeviceInterfaceDetailBuffer(
                hDevInfoSet,
                ref deviceInterfaceData,
                IntPtr.Zero,
                DeviceInterfaceDetailDataSize,
                ref RequiredSize,
                IntPtr.Zero);
            return results;
        }
        
        /// <summary>
        /// results = 1 in the second pass of the routine is success
        /// DeviceInterfaceDetailDataSize parameter (RequiredSize) came from the first pass
        /// </summary>
        /// <param name="RequiredSize"></param>
        /// <param name="DeviceInterfaceDetailDataSize"></param>
        /// <returns></returns>
        internal bool CT_SetupDiGetDeviceInterfaceDetail(ref int RequiredSize, int DeviceInterfaceDetailDataSize)
        {
            // 
            //!                                *** IMPORTANT NOTICE OF THE FIELD cbSize ***
            //
            // A pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA structure to receive information about the specified interface. 
            // This parameter is optional and can be NULL. This parameter must be NULL if DeviceInterfaceDetailSize is zero. 
            // If this parameter is specified, the caller must set DeviceInterfaceDetailData.cbSize to sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA) 
            // before calling this function. The cbSize member always contains the size of the fixed part of the data structure, not a size 
            // reflecting the variable-length string at the end.
            //
            // ref to https://msdn.microsoft.com/en-us/library/windows/hardware/ff551120(v=vs.85).aspx
            // ref to http://pinvoke.net/default.aspx/setupapi.SetupDiGetDeviceInterfaceDetail
            // 

            bool ret;


            IntPtr detail = Marshal.AllocHGlobal(DeviceInterfaceDetailDataSize);

            uint size = (uint)DeviceInterfaceDetailDataSize;

            int cbSize = 0;

            if (IntPtr.Size == 8) // for 64 bit operating systems
                cbSize = 8;
            else
                cbSize = 4 + Marshal.SystemDefaultCharSize; // for 32 bit systems


            Marshal.WriteInt32(detail, cbSize);

            try
            {

                ret = SetupDiGetDeviceInterfaceDetailBuffer(
                    hDevInfoSet,
                    ref deviceInterfaceData,
                    detail,
                    DeviceInterfaceDetailDataSize,
                    ref RequiredSize,
                    IntPtr.Zero);

                if (ret == false)
                {
                    int err = Marshal.GetLastWin32Error();
                }

                IntPtr pDevicePathName = new IntPtr(detail.ToInt32() + 4);
                DevicePathName = Marshal.PtrToStringAuto(pDevicePathName);
            }
            catch
            {
                throw new Win32Exception();
            }
            finally
            {
                Marshal.FreeHGlobal(detail);
            }

            return ret;
        }
        
        internal int CT_HidD_GetHIDSerialNumber(out string SerialNumber)
        {
            byte[] data = new byte[254];
            SerialNumber = string.Empty;

            if (HidD_GetSerialNumberString(hHidFile, ref data[0], data.Length))
            {
                // Convert data bytes to unicode string
                var value = Encoding.Unicode.GetString(data);
                SerialNumber = value.Remove(value.IndexOf((char)0));

                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// The HidD_GetNumInputBuffers routine returns the current size, in number of reports, of the ring buffer that the HID class driver uses to queue input reports from a specified top-level collection.
        /// <see cref="https://msdn.microsoft.com/library/windows/hardware/ff539675"/>
        /// </summary>
        /// <param name="NumberBuffers"></param>
        /// <returns></returns>
        internal bool CT_HidD_GetNumInputBuffers(out uint NumberBuffers)
        {
            NumberBuffers = 0;
            return HidD_GetNumInputBuffers(hHidFile, ref NumberBuffers);
        }

        internal bool CT_HidD_GetInputReport(out byte[] Buffer)
        {
            Buffer = new byte[64];

            if (HidD_GetInputReport(hHidFile, ref Buffer[0], Buffer.Length))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        
        internal bool CT_HidD_GetAttributes(IntPtr hObject)
        {
            // Create an instance of HIDD_ATTRIBUTES
            myHIDD_ATTRIBUTES = new HIDD_ATTRIBUTES();
            // Calculate its size
            myHIDD_ATTRIBUTES.Size = Marshal.SizeOf(myHIDD_ATTRIBUTES);

            return HidD_GetAttributes(
                    hObject,
                    ref myHIDD_ATTRIBUTES);
        }
        
        internal bool CT_HidD_GetPreparsedData(IntPtr hObject, ref IntPtr pPHIDP_PREPARSED_DATA)
        {
            return HidD_GetPreparsedData(hObject, ref pPHIDP_PREPARSED_DATA);
        }
        
        internal bool CT_HidD_SetOutputReport(IntPtr HidDeviceObject, ref byte[] lpReportBuffer, int ReportBufferLength)
        {
            return HidD_SetOutputReport(HidDeviceObject, lpReportBuffer, ReportBufferLength);
        }
        
        internal bool CT_HidP_GetCaps(IntPtr pPreparsedData)
        {
            myHIDP_CAPS = new HIDP_CAPS();
            if(HidP_GetCaps(pPreparsedData, ref myHIDP_CAPS))
            {
                unmanagedInputBuffer = Marshal.AllocHGlobal(myHIDP_CAPS.InputReportByteLength);
                return true;
            }
            else
            {
                int err = Marshal.GetLastWin32Error();
                return false;
            }
        }
        
        internal int CT_HidP_GetValueCaps(ref short CalsCapsLength, IntPtr pPHIDP_PREPARSED_DATA)
        {

            HIDP_REPORT_TYPE myType = 0;
            myHIDP_VALUE_CAPS = new HIDP_VALUE_CAPS[5];

            return HidP_GetValueCaps(
                (short)myType,
                myHIDP_VALUE_CAPS,
                ref CalsCapsLength,
                pPHIDP_PREPARSED_DATA);

        }

        internal bool CT_CreateFile(string DeviceName)
        {
            var security = new SECURITY_ATTRIBUTES
            {
                lpSecurityDescriptor = IntPtr.Zero,
                bInheritHandle = true
            };
            security.nLength = Marshal.SizeOf(security);

            hHidFile = CreateFile(
                DeviceName,
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                ref security,
                OPEN_EXISTING,
                0, //FILE_FLAG_OVERLAPPED | FILE_FLAG_NO_BUFFERING,
                0);

            if (hHidFile == IntPtr.Zero)
                return false;
            else
                return true;
            //return fileRW.Open(DeviceName);

        }

        internal byte[] CT_ReadFile()
        {
            DateTime start = DateTime.Now;

            byte[] buffer = null;
            //var security = new SECURITY_ATTRIBUTES();
            var overlapped = new NativeOverlapped()
            {
                //EventHandle = CreateEvent(ref security, 0, 0, "")
            };

            //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tStart to read, takes {1:F6}ms ...", DateTime.Now, (DateTime.Now - start).TotalMilliseconds));

            var ret = ReadFile(hHidFile, unmanagedInputBuffer, (uint)myHIDP_CAPS.InputReportByteLength, out uint bytesRead, ref overlapped);

            //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tData is received, takes {1:F6}ms ...", DateTime.Now, (DateTime.Now - start).TotalMilliseconds));

            /// the err should be 0x3E5 which indicates the overlapped I/O operation is in progress
            /// <see cref="https://msdn.microsoft.com/en-us/library/ms681388(v=vs.85).aspx"/>
            int err = Marshal.GetLastWin32Error();
            //if (err != ERROR_IO_PENDING)
            //{
            //    ret = false;
            //}

            //int evt = WaitForSingleObject(overlapped.EventHandle, 500);
            //switch (evt)
            //{
            //    case 0x0:  // the state of the specified object is signaled
            //        buffer = new byte[myHIDP_CAPS.InputReportByteLength];
            //        Marshal.Copy(unmanagedInputBuffer, buffer, 0, buffer.Length);
            //        break;

            //    case 0x102:  // The time-out interval elapsed, and the object's state is nonsignaled
            //    default:  // unsolved errors
            //        buffer = null;
            //        break;
            //}

            buffer = new byte[myHIDP_CAPS.InputReportByteLength];
            Marshal.Copy(unmanagedInputBuffer, buffer, 0, buffer.Length);

            return buffer;

        }
        
        internal bool CT_WriteFile(byte[] Buffer)
        {
            DateTime start = DateTime.Now;

            //var security = new SECURITY_ATTRIBUTES();
            var overlapped = new NativeOverlapped()
            {
                //EventHandle = CreateEvent(ref security, 0, 0, "")
            };

            //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tStart to write, takes {1:F6}ms ...", DateTime.Now, (DateTime.Now - start).TotalMilliseconds));

            bool ret = WriteFile(hHidFile, ref Buffer[0], Buffer.Length, out int lpNumberOfBytesWritten, ref overlapped);

            //Trace.WriteLine(string.Format("{0:mm:ss.ffffff}\tData is written, takes {1:F6}ms ...", DateTime.Now, (DateTime.Now - start).TotalMilliseconds));

            /// the err should be 0x3E5
            /// which indicates the overlapped I/O operation is in progress
            /// <see cref="https://msdn.microsoft.com/en-us/library/ms681388(v=vs.85).aspx"/>
            int err = Marshal.GetLastWin32Error();
            //if (err != ERROR_IO_PENDING)
            //{
            //    // error in communications
            //    ret = false;
            //}

            //int evt = WaitForSingleObject(overlapped.EventHandle, 500); // wait for 500ms

            //switch (evt)
            //{
            //    case 0x0:  // the state of the specified object is signaled
            //        ret = true;
            //        break;

            //    case 0x102:  // The time-out interval elapsed, and the object's state is nonsignaled
            //    default:  // unsolved errors
            //        ret = false;
            //        break;
            //}

            return ret;
        }

        internal void CT_CloseFile()
        {
            if (Environment.OSVersion.Version.Major > 5)
            {
                CancelIoEx(hHidFile, IntPtr.Zero);
            }
            else
            {
                CloseHandle(hHidFile);
            }

        }
        
        internal int CT_SetupDiDestroyDeviceInfoList()
        {
            return SetupDiDestroyDeviceInfoList(hDevInfoSet);

        }
        
        internal int CT_HidD_FreePreparsedData(IntPtr pPHIDP_PREPARSED_DATA)
        {
            return SetupDiDestroyDeviceInfoList(pPHIDP_PREPARSED_DATA);
        }

        public void Dispose()
        {
            if(unmanagedInputBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanagedInputBuffer);
            }
        }

        #endregion
    }
}
