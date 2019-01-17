using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Message;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Zaber;

namespace Irixi_Aligner_Common.MotionControllers.Luminos
{
    public class LuminosP6A : MotionControllerBase<LuminosAxis>
    {
        #region Variables
        ZaberPortFacade _zaber_port_facade = null;
        ConversationCollection _zaber_conversation_collection = null;
        #endregion

        #region Constructor
        public LuminosP6A(ConfigPhysicalMotionController Config) : base(Config)
        {
            //_wait_axis_register = new SemaphoreSlim(0);

            if (Config.Enabled)
            {
                _zaber_port_facade = new ZaberPortFacade()
                {
                    DefaultDeviceType = new DeviceType() { Commands = new List<CommandInfo>() },
                    Port = new TSeriesPort(new SerialPort(), new PacketConverter()),
                    QueryTimeout = 1000
                };

                ZaberDevice allDevices = _zaber_port_facade.GetDevice(0);

                allDevices.MessageReceived += AllDevices_MessageReceived;

                allDevices.MessageSent += AllDevices_MessageSent;
            }
        }

        #endregion

        #region Events of Zaber

        private void AllDevices_MessageSent(object sender, DeviceMessageEventArgs e)
        {

        }

        private void AllDevices_MessageReceived(object sender, DeviceMessageEventArgs e)
        {
            // if the command relative to position is received, flush the AbsPosition of axis 
            int devNum = e.DeviceMessage.DeviceNumber;

            if (this[devNum] is LuminosAxis axis)
            {
                switch (e.DeviceMessage.Command)
                {
                    case Command.Home:
                    case Command.ManualMove:
                    case Command.ManualMoveTracking:
                    case Command.MoveAbsolute:
                    case Command.MoveToStoredPosition:
                    case Command.MoveTracking:
                    case Command.LimitActive:
                    case Command.SlipTracking:
                    case Command.UnexpectedPosition:
                    case Command.MoveRelative:
                        axis.AbsPosition = e.DeviceMessage.Data;
                        break;
                }
            }
        }


        #endregion

        #region Methods
        protected override bool InitProcess()
        {
            bool initRet = true;

            try
            {
                _zaber_port_facade.Open(Port);
                Thread.Sleep(1000);

                if (_zaber_port_facade.Conversations.Count > 1)
                {
                    // The axes of the luminos p6a have been found
                    foreach (var conversation in _zaber_port_facade.Conversations)
                    {
                        if (conversation is ConversationCollection) // this conversation with device number 0 is used to control all axis
                        {
                            _zaber_conversation_collection = conversation as ConversationCollection;
                        }
                        else
                        {
                            string axisID = conversation.Device.DeviceNumber.ToString();
                            
                            if (this[conversation.Device.DeviceNumber] is LuminosAxis phyAxis)
                            {
                                phyAxis.RegisterZaberConversation(conversation);

                                LogHelper.WriteLine("One luminos axis was find, the id is {0}.", conversation.Device.DeviceNumber);
                            }
                            else
                            {
                                LogHelper.WriteLine("**ERROR** The device number {0} reported by luminos sdk is not defined in the config file.", conversation.Device.DeviceNumber);
                                initRet = false;
                            }
                        }
                    }
                }
                else // no axis was found by the sdk
                {
                    _zaber_port_facade.Close();
                    LogHelper.WriteLine("**ERROR** No axis was found.");
                    initRet = false;
                }


                if (initRet == false)
                    this.LastError = "we encountered some problem while initializing the device, please see the log file for detail information.";
                else
                    this.IsInitialized = true;

                return initRet;

            }
            catch (Exception ex)
            {
                this.LastError = string.Format("{0}", ex.Message);
                return false;
            }
        }
        
        protected override bool HomeProcess(Interfaces.IAxis Axis)
        {
            bool ret = false;
            LuminosAxis _axis = Axis as LuminosAxis;

            // lock the axis
            if (_axis.Lock())
            {
                try
                {
                    _axis.IsHomed = false;
                    DeviceMessage zaber_ret = _axis.ZaberConversation.Request(Command.Home);

                    if (zaber_ret.HasFault == false)
                    {
                        _axis.IsHomed = true;

                        ret = true;
                    }
                    else
                    {
                        _axis.LastError = string.Format("sdk reported error code {0}", zaber_ret.FlagText);

                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    _axis.LastError = string.Format("{0}", ex.Message);
                    ret = false;
                }
                finally
                {
                    // release the axis
                    _axis.Unlock();
                }
            }
            else
            {
                _axis.LastError = "axis is busy";
                ret = false;
            }

            return ret;
        }

        protected override bool MoveProcess(Interfaces.IAxis Axis, MoveMode Mode, int Speed, int Distance)
        {
            LuminosAxis axis = Axis as LuminosAxis;

            bool ret = false;
            int target_pos = 0;

            if (axis.IsHomed == false)
            {
                axis.LastError = "the axis is not homed";
                return false;
            }

            // lock the axis
            if (axis.Lock())
            {
                // Set the move speed
                if (Mode == MoveMode.ABS)
                {
                    target_pos = Math.Abs(Distance);
                }
                else
                {
                    target_pos = axis.AbsPosition + Distance;
                }

                // Move the the target position
                if (axis.CheckSoftLimitation(target_pos))
                {
                    try
                    {
                        DeviceMessage zaber_msg = axis.ZaberConversation.Request(Command.MoveAbsolute, target_pos);

                        if (zaber_msg.HasFault == false)
                        {
                            ret = true;
                        }
                        else
                        {
                            axis.LastError = string.Format("sdk reported error code {0}", zaber_msg.Text);

                            ret = false;
                        }
                    }
                    catch(RequestReplacedException ex)
                    {
                        DeviceMessage actualResp = ex.ReplacementResponse;
                        if (actualResp.Command == Command.Stop)
                        {
                            // if STOP was send while moving
                            axis.AbsPosition = (int)actualResp.Data;
                            ret = true;
                        }
                        else
                        {
                            axis.LastError = ex.Message;
                            ret = false;
                        }
                    }
                    catch (Exception ex)
                    {

                        axis.LastError = ex.Message;
                        ret = false;
                    }
                }
                else
                {
                    axis.LastError = "target position exceeds the limitation.";

                    ret = false;
                }
                
                axis.Unlock();
                
            }
            else
            {
                axis.LastError = "the axis is locked.";
                ret = false;
            }

            return ret;
        }
        
        public override void Stop()
        {
            if (this.IsInitialized)
                _zaber_conversation_collection.Request(Command.Stop);
        }

        public override void Dispose()
        {
            try
            {
                _zaber_port_facade.Close();
            }
            catch
            {

            }
        }

        #endregion
    }
}
