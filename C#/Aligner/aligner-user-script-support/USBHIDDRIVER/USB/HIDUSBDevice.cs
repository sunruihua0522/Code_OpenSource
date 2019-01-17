using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace USBHIDDRIVER.USB
{
    /// <summary>
    ///
    /// </summary>
    public class HIDUSBDevice: IDisposable
    {
        bool disposed = false;
       
        //recieve Buffer (Each report is one Element)
        //this one was replaced by the receive Buffer in the interface
        //public static ArrayList receiveBuffer = new ArrayList();
        
        //USB Object
        private USBSharp myUSB = new USBSharp();

        //thread for read operations
        CancellationTokenSource cts;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HIDUSBDevice"/> class.
        /// And tries to establish a connection to the device.
        /// </summary>
        /// <param name="VID"></param>
        /// <param name="PID"></param>
        /// <param name="SerialNumber"></param>
        public HIDUSBDevice(string VID, string PID = "", string SerialNumber = "")
        {
            this.VID = VID;
            this.PID = PID;
            this.SerialNumber = SerialNumber;

            cts = new CancellationTokenSource();
        }

        #endregion

        #region Properties

        public string PID {get; }
        public string VID {get; }
        public string SerialNumber { private set; get; }
        public string DevicePath { private set;  get; }
        public bool IsConnected { private set; get; }

        #endregion

        #region Methods

        public List<string> GetDevicesList()
        {
            List<string> devices = new List<string>();

            this.DevicePath = string.Empty;

            myUSB.CT_HidGuid();
            myUSB.CT_SetupDiGetClassDevs();

            bool? result = null;
            bool resultb = false;
            int device_count = 0;
            int size = 0;
            int requiredSize = 0;
            int numberOfDevices = 0;
            //search the device until you have found it or no more devices in list

            while (result.HasValue == false || result.Value == true)
            {
                //open the device
                result = myUSB.CT_SetupDiEnumDeviceInterfaces(device_count);
                //get size of device path
                resultb = myUSB.CT_SetupDiGetDeviceInterfaceBuffer(ref requiredSize, 0);
                size = requiredSize;
                //get device path
                resultb = myUSB.CT_SetupDiGetDeviceInterfaceDetail(ref requiredSize, size);

                //is this the device i want?
                string deviceID = this.VID;
                if (myUSB.DevicePathName.IndexOf(deviceID) > 0)
                {

                    //create HID Device Handel
                    resultb = myUSB.CT_CreateFile(myUSB.DevicePathName);

                    // Check the serial Number
                    myUSB.CT_HidD_GetHIDSerialNumber(out string device_sn);

                    myUSB.CT_CloseFile();

                    if (device_sn != "")
                        devices.Add(device_sn);

                    numberOfDevices++;
                }

                device_count++;

            }

            myUSB.CT_SetupDiDestroyDeviceInfoList();

            return devices;
        }

        
        
        public bool ConnectDevice()
        { 
            //searchDevice
            TryConnectDeviceWithSN();

            //return connection state
            return this.IsConnected;
        }

        public void DisconnectDevice()
        {
            if (this.IsConnected)
            {
                myUSB.CT_CloseFile();
                myUSB.CT_SetupDiDestroyDeviceInfoList();

                this.IsConnected = false;
            }
        }
        
        public bool WriteData(byte[] Data)
        {
            bool success = false;
            if (this.IsConnected)
            {
                try
                {
                    int outputReportByteLength = myUSB.HidpCaps.OutputReportByteLength;
                    int bytesSend = 1;

                    //if bWriteData is bigger then one report diveide into sevral reports
                    while (bytesSend < Data.Length)
                    {

                        // Set the size of the Output report buffer.
                       // byte[] OutputReportBuffer = new byte[myUSB.myHIDP_CAPS.OutputReportByteLength - 1 + 1];
                        byte[] OutputReportBuffer = new byte[outputReportByteLength];

                        // set the report id
                        OutputReportBuffer[0] = Data[0];

                        // Store the report data following the report ID.
                        for (int i = 1; i < OutputReportBuffer.Length; i++)
                        {
                            if (bytesSend < Data.Length)
                            {
                                OutputReportBuffer[i] = Data[bytesSend];
                                bytesSend++;
                            }
                            else
                            {
                                OutputReportBuffer[i] = 0;
                            }
                        }

                        success = myUSB.CT_WriteFile(OutputReportBuffer);
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                }
            }
            else 
            {
                success = false;
            }
            return success;
        }
        
        public byte[] ReadData()
        {
            return myUSB.CT_ReadFile();
        }

        bool TryConnectDeviceWithSN()
        {
            bool? result = null;
            bool resultb = false;
            int device_count = 0;
            int size = 0;
            int requiredSize = 0;

            DevicePath = string.Empty;

            // Get the GUID of the HID device Class
            myUSB.CT_HidGuid();

            // Get the device information set
            myUSB.CT_SetupDiGetClassDevs(); 
            
            // Reset the IsConnected to false
            this.IsConnected = false;

            //search the device until you have found it or no more devices in list
            while (!result.HasValue || result.Value == true)
            {
                //
                //if (result == false)
                //    break;

                //open the device
                result = myUSB.CT_SetupDiEnumDeviceInterfaces(device_count);

                //get size of device path
                resultb = myUSB.CT_SetupDiGetDeviceInterfaceBuffer(ref requiredSize, 0);

                size = requiredSize;

                //get device path
                resultb = myUSB.CT_SetupDiGetDeviceInterfaceDetail(ref requiredSize, size);

                if (resultb == false)
                {
                    int err = Marshal.GetLastWin32Error();
                }

                DevicePath = myUSB.DevicePathName;

                //is this the device i want?
                string deviceID = this.VID + "&" + this.PID;

                if (DevicePath.ToLower().IndexOf(deviceID.ToLower()) > 0)
                {
                    //create HID Device Handel
                    resultb = myUSB.CT_CreateFile(DevicePath);

                    // Check the serial Number
                    myUSB.CT_HidD_GetHIDSerialNumber(out string device_sn);

                    myUSB.CT_HidD_GetNumInputBuffers(out uint numbuffers);

                    if (SerialNumber == null || SerialNumber == string.Empty || device_sn == SerialNumber)
                    {
                        IntPtr myPtrToPreparsedData = default(IntPtr);

                        if (myUSB.CT_HidD_GetPreparsedData(myUSB.hHidFile, ref myPtrToPreparsedData))
                        {
                            if (myUSB.CT_HidP_GetCaps(myPtrToPreparsedData))
                            {
                                // we have found our device so stop searching
                                IsConnected = true;
                                break;
                            }
                        }
                        else
                        {
                            myUSB.CT_CloseFile();
                            break;
                        }
                    }
                    else
                    {
                        myUSB.CT_CloseFile();
                    }
                }

                device_count++;
            }

            myUSB.CT_SetupDiDestroyDeviceInfoList();

            //return state
            return this.IsConnected;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposeManagedResources)
        {
            if (!this.disposed)
            {
                if (disposeManagedResources)
                {
                    //only clear up managed stuff here
                }

                //clear up unmanaged stuff here
                if (myUSB.hHidFile != IntPtr.Zero)
                {
                    myUSB.CT_CloseFile();
                }

                if (myUSB.hDevInfoSet != IntPtr.Zero)
                {
                    myUSB.CT_SetupDiDestroyDeviceInfoList();
                }

                this.disposed = true;
            }
        }

        #endregion


    }
}
