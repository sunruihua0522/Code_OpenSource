
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.BaseClass;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Irixi_Aligner_Common.ViewModel
{
    public class ViewMassMove : ViewModelBase
    {
        const string FOLDER = "PresetPosition";

        public ViewMassMove(SystemService service, LogicalMotionComponent component)
        {
            Service = service;
            MotionComponent = component;

            // create user controls of Axis4MassMove
            AxisControlCollection = new List<UserControls.Axis4MassMove>();
            for(int i = 0; i < component.Count; i++)
            {
                var logicalaxis = component[i];

                AxisControlCollection.Add(new UserControls.Axis4MassMove()
                {
                    TotalAxes = component.Count,
                    AxisName = logicalaxis.AxisName,
                    Position = logicalaxis.PhysicalAxisInst.UnitHelper.RelPosition,
                    IsAbsMode = logicalaxis.PhysicalAxisInst.IsAbsMode,
                    LogicalAxis = logicalaxis
                });
            }
        }

        #region  Properties
        public LogicalMotionComponent MotionComponent
        {
            private set;
            get;
        }

        public List<UserControls.AxisForMassMove> AxisControlCollection
        {
            private set;
            get;
        }

        public SystemService Service
        {
            private set;
            get;
        }

        #endregion

        #region Commands

        public RelayCommand MassMove
        {
            get
            {
                return new RelayCommand(() =>
                {
                    List<Tuple<int, LogicalAxis, AxisMoveArgs>> axesgrp = new List<Tuple<int, LogicalAxis, AxisMoveArgs>>();

                    foreach(var axis in AxisControlCollection)
                    {
                        axesgrp.Add(new Tuple<int, LogicalAxis, AxisMoveArgs>(
                            axis.Order,
                            axis.LogicalAxis,
                            new AxisMoveArgs(axis.IsAbsMode ? MoveMode.ABS : MoveMode.REL, axis.Speed, axis.Position)));
                    }

                    // start to mass move
                    Service.MassMoveLogicalAxis(axesgrp.ToArray());
                });
            }
        }

        public RelayCommand Stop
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Service.CommandStop.Execute(null);
                });
            }
        }

        #endregion

        #region Methods

        public void SavePresetPosition(string Name)
        {
            try
            {
                PresetPosition pos = new PresetPosition()
                {
                    Name = Name,
                    MotionComponentHashCode = MotionComponent.GetHashCode(),
                    Items = new PresetPositionItem[AxisControlCollection.Count]
                };
                for (int i = 0; i < AxisControlCollection.Count; i++)
                {
                    pos.Items[i] = AxisControlCollection[i].PresetAggregation;
                }

                //using (SqliteDb db = new SqliteDb())
                //{
                //    // check if the preset exists
                //    if (db.ReadPresetPosition(pos.MotionComponentHashCode, Name, out PresetPosition preset))
                //    {
                //        if (preset == null)
                //        {
                //            // save the preset position
                //            db.SavePresetPosition(pos);

                //            Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(
                //                this,
                //                string.Format("The preset position saved successfully."),
                //                "NOTIFY"));
                //        }
                //        else
                //        {
                //            // overwrite 
                //            Messenger.Default.Send<NotificationMessageAction<MessageBoxResult>>(new NotificationMessageAction<MessageBoxResult>(
                //                this,
                //                "AskForOverwrite",
                //                (ret) =>
                //                {
                //                    if (ret == MessageBoxResult.Yes)
                //                    {
                //                        // overwrite the current preset position
                //                        if (db.DeletePresetPosition(pos.MotionComponentHashCode, pos.Name))
                //                        {
                //                            // save the preset position
                //                            db.SavePresetPosition(pos);

                //                            Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(
                //                                this,
                //                                string.Format("The preset position saved successfully."),
                //                                "NOTIFY"));
                //                        }
                //                        else
                //                        {
                //                            Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(
                //                                this,
                //                                string.Format("Unable to delete the preset position, {0}", db.LastError),
                //                                "ERROR"));
                //                        }
                //                    }
                //                }
                //                ));
                //        }
                //    }
                //    else
                //    {
                //        // error while reading preset position from database
                //        Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(
                //            this,
                //            string.Format("Unable to check the existence of the preset position, {0}", db.LastError),
                //            "ERROR"));
                //    }
                //}
            }
            catch(Exception ex)
            {
                Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(
                    this,
                    string.Format("Unable to save the preset position, {0}", ex.Message), 
                    "ERROR"));
            }
        }

        public string[] GetPresetPositionList()
        {
            //// load the existed preset
            //using (SqliteDb db = new SqliteDb())
            //{
            //    return db.GetPresetPositionNames(MotionComponent.GetHashCode());
            //}
            return null;
        }
        
        public override int GetHashCode()
        {
            return MotionComponent.GetHashCode();
        }
        #endregion


        
    }
}
