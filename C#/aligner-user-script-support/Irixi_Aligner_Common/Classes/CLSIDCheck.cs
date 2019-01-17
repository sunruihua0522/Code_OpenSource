using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Irixi_Aligner_Common
{
    /// <summary>
    /// 
    /// </summary>
    public class CLSIDCheck
    {
        private delegate int DllRegisterServerDelegate();
        static DllRegisterServerDelegate _register;
        static DllRegisterServerDelegate _unregister;

        public bool CheckCLSID(string ID)
        {
            string key = string.Format("TypeLib\\{0}\\", ID);
            RegistryKey rkTest = Registry.ClassesRoot.OpenSubKey(key);
            if (rkTest == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool RegisterDLL(string FileName)
        {
            if(!File.Exists(FileName))
            {
                this.LastError = string.Format("Unable to find {0} in application dir.", FileName);
                return false;
            }

            _register = (DllRegisterServerDelegate)FunctionLoader.LoadFunction<DllRegisterServerDelegate>(FileName, "DllRegisterServer");
            int i = _register();
            if(i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UnregisterDLL(string FileName)
        {
            if (!File.Exists(FileName))
            {
                this.LastError = string.Format("Unable to find {0} in application dir.", FileName);
                return false;
            }

            _unregister = (DllRegisterServerDelegate)FunctionLoader.LoadFunction<DllRegisterServerDelegate>(FileName, "DllUnregisterServer");
            int i = _unregister();
            if (i > 0)
            {
                return true;
            }
            else
            { 
                return false;
            }
        }

        public string LastError { private set; get; }
    }

    internal class  FunctionLoader
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        public static Delegate LoadFunction<T>(string dllPath, string functionName)
        {
            var hModule = LoadLibrary(dllPath);
            var functionAddress = GetProcAddress(hModule, functionName);
            return Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
        }
    }
}
