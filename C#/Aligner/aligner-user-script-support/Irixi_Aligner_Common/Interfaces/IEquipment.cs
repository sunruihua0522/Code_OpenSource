using System;
using Irixi_Aligner_Common.Configuration.Base;

namespace Irixi_Aligner_Common.Interfaces
{
    public interface IEquipment : IDisposable, IHashable
    {
        /// <summary>
        /// Get the device class which makes this controller exclusively in the system.
        /// the controller could be located by the device class.
        /// </summary>
        Guid DeviceClass { get; }

        /// <summary>
        /// Get the configuration which is saved in the config file.
        /// </summary>
        ConfigurationBase Config { get; }

        /// <summary>
        /// Get the communication port of the controller.
        /// this might be serial port name, usb hid device serial number, etc.
        /// </summary>
        string Port { get; }

        /// <summary>
        /// Get whehter the controller is available or not
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Get whether the controller has been initialized
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Get the last error message
        /// </summary>
        string LastError { get; }

        /// <summary>
        /// Initialize the controller
        /// </summary>
        /// <returns></returns>
        bool Init();
    }
}
