using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using M12.Base;
using M12.Excpections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Irixi_Aligner_Common.Alignment.ProfileND
{
    public class ProfileND : AlignmentBase
    {

        #region Constructors

        public ProfileND(ProfileNDArgs Args) : base(Args)
        {
            this.Args = Args;
        }

        #endregion

        #region Properties
        
        public new ProfileNDArgs Args { get; }

        #endregion

        public override void Start()
        {
            int cycles = 0;
            double max_measval = 0;

            base.Start();

            PauseInstruments();

            try
            {

                do
                {
                    Args.ClearScanCurve();

                    // select enabled axes
                    var arg_enabled = Args.AxisParamCollection.Where(a => a.IsEnabled == true);

                    // sort by AlignOrder
                    var args = arg_enabled.OrderBy(a => a.Order);

                    // align by each axis.
                    foreach (var arg in args)
                    {
                        double dist_moved = 0;
                        double halfrange = arg.Range / 2;

                        /// <summary>
                        /// move to alignment start position.
                        /// the move methods of the physical axis MUST BE called because the move methods of logical
                        /// axis will trigger the changing of system status in SystemService.
                        /// <see cref="SystemService.MoveLogicalAxis(MotionControllers.Base.LogicalAxis, MoveByDistanceArgs)"/>
                        /// </summary>
                        if (arg.Axis.PhysicalAxisInst.Move(MoveMode.REL, arg.MoveSpeed, -halfrange) == false)
                            throw new InvalidOperationException(arg.Axis.PhysicalAxisInst.LastError);

                        var axis = arg.Axis.PhysicalAxisInst as IrixiM12Axis;
                        var m12 = axis.Parent as IrixiM12;

                        // start to 1D scan
                        while (dist_moved <= arg.Range)
                        {
                            // read measurement value
                            var ret = Args.Instrument.Fetch();
                            var point = new Point2D(dist_moved, ret);
                            arg.ScanCurve.Add(point);

                            // record distance moved
                            dist_moved += arg.Interval;

                            // move to the next point
                            if (arg.Axis.PhysicalAxisInst.Move(MoveMode.REL, arg.MoveSpeed, arg.Interval) == false)
                                throw new InvalidOperationException(arg.Axis.PhysicalAxisInst.LastError);

                            // cancel the alignment process
                            if (cts_token.IsCancellationRequested)
                                break;
                        }

                        // cancel the alignment process
                        if (cts_token.IsCancellationRequested)
                            break;

                        // return to the position with the maximnm measurement data
                        var max_pos = arg.ScanCurve.FindPositionWithMaxIntensity();
                        max_measval = max_pos.Y;

                        // move to the position of max power
                        // Note: The distance to move is minus
                        if (arg.Axis.PhysicalAxisInst.Move(MoveMode.REL, arg.Axis.MoveArgs.Speed, -(arg.Range - max_pos.X)) == false)
                            throw new InvalidOperationException(arg.Axis.PhysicalAxisInst.LastError);
                    }

                    cycles++;

                } while (cycles < Args.MaxCycles && max_measval < Args.Target);
            }
            catch(Exception ex)
            {
                AddLog($"{ex.Message}");
                throw ex;
            }
            finally
            {
                ResumeInstruments();
            }
        }

        public override void PauseInstruments()
        {
            Args.Instrument.PauseAutoFetching();
        }

        public override void ResumeInstruments()
        {
            Args.Instrument.ResumeAutoFetching();
        }

        public override string ToString()
        {
            return "Profile nD";
        }
    }
}
