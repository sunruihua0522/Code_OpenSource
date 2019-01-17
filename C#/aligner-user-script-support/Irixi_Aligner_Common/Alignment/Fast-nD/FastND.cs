using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using M12.Base;
using M12.Excpections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Irixi_Aligner_Common.Alignment.FastND
{
    public class FastND : AlignmentBase
    {

        #region Constructors

        public FastND(FastNDArgs Args) : base(Args)
        {
            this.Args = Args;
        }

        #endregion

        #region Properties
        
        public new FastNDArgs Args { get; }

        #endregion

        public override void Start()
        {
            int cycles = 0;
            double max_measval = 0;
            ADCSamplingPointMissException pointsMissedEx = null;

            base.Start();

            Args.ClearScanCurve();

            // select enabled axes
            var arg_enabled = Args.AxisParamCollection.Where(a => a.IsEnabled == true);

            // sort by AlignOrder
            var args = arg_enabled.OrderBy(a => a.Order);

            // align by each axis.
            foreach (var arg in args)
            {
                AddLog($"Start to align {arg.Axis} ...");

                //double dist_moved = 0;
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

                List<Point2D> points = null;

                try
                {
                    m12.StartFast1D(axis, arg.Range, arg.Interval, arg.MoveSpeed, M12.Definitions.ADCChannels.CH3, out points);
                }
                catch (ADCSamplingPointMissException ex)
                {
                    pointsMissedEx = ex;

                    AddLog("Some of the sampling points are lost, reduce the speed and try again.");
                    AddLog($"{ex.Desired} points desired but only {ex.Reality} points got.");
                }

                // convert position from step to distance.
                for (int i = 0; i < points.Count; i++)
                    points[i].X = axis.UnitHelper.ConvertStepsToDistance((int)points[i].X);

                arg.ScanCurve.AddRange(points);

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

                AddLog($"Done! {(DateTime.Now - alignStarts).TotalSeconds.ToString("F1")}s costs.");
            }
        }

        public override string ToString()
        {
            return "Fast-nD";
        }
    }
}
