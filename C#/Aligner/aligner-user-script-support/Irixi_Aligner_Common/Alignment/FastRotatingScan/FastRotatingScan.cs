
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using M12.Base;
using M12.Definitions;
using M12.Excpections;
using System;
using System.Collections.Generic;

namespace Irixi_Aligner_Common.Alignment.FastRotatingScan
{
    public class FastRotatingScan : AlignmentBase
    {
        #region Variables

        

        #endregion

        #region Constructors
        public FastRotatingScan(FastRotatingScanArgs Args) : base(Args)
        {
            this.Args = Args;
        }
        #endregion

        #region Properties

        public new FastRotatingScanArgs Args { get; }

        #endregion

        public override void Start()
        {
            base.Start();

            ADCSamplingPointMissException pointsMissedEx = null;

            double deltaPos = double.MaxValue;

            Log.Clear();

            //while(true)
            //{
            double distMovedLinear = 0, distMovedRotating = 0;
            double halfRangeLinear = Args.LinearRange / 2;

            // get the unit of axes
            var unitLinearAxis = Args.AxisLinear.PhysicalAxisInst.UnitHelper;
            var unitRotatingAxis = Args.AxisRotating.PhysicalAxisInst.UnitHelper;

            AddLog($"Start to align ...");

            #region Linear Alignment

            // clear the previous scan curve
            Args.ClearScanCurve();

            // clear the variables to restart now process of linear align
            distMovedLinear = 0;

            /// <summary>
            /// move to alignment start position.
            /// the move methods of the physical axis MUST BE called because the move methods of logical
            /// axis will trigger the changing of system status in SystemService.
            /// <see cref="SystemService.MoveLogicalAxis(LogicalAxis, MoveByDistanceArgs)"/>
            /// </summary>
            if (Args.AxisLinear.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -halfRangeLinear) == false)
                throw new InvalidOperationException($"unable to move the linear axis, {Args.AxisLinear.PhysicalAxisInst.LastError}");

            var axis = Args.AxisLinear.PhysicalAxisInst as IrixiM12Axis;
            var m12 = axis.Parent as IrixiM12;

            var range = Args.LinearRange;
            var interval = Args.LinearInterval;

            List<Point2D> points = null;
            List<Point2D> points2 = null;

            try
            {
                m12.StartFast1D(axis, range, interval, Args.MoveSpeed, ADCChannels.CH3, out points, ADCChannels.CH4, out points2);
            }
            catch (ADCSamplingPointMissException ex)
            {
                pointsMissedEx = ex;
            }

            // convert position from step to distance.
            for (int i = 0; i < points.Count; i++)
            {
                points[i].X = axis.UnitHelper.ConvertStepsToDistance((int)points[i].X);
                points2[i].X = axis.UnitHelper.ConvertStepsToDistance((int)points2[i].X);
            }

            Args.ScanCurve.AddRange(points);
            Args.ScanCurve2.AddRange(points2);

            // find the position of max power and draw the constant lines
            /// <seealso cref="ScanCurve.MaxPowerConstantLine"/>
            var maxPos = Args.ScanCurve.FindPositionWithMaxIntensity();
            var maxPos2 = Args.ScanCurve2.FindPositionWithMaxIntensity();

            // calculate the position differential
            deltaPos = maxPos.X - maxPos2.X;

            // move to the middle position of the max power
            var returnToPos = maxPos.X - deltaPos / 2;
            
            // output messages
            AddLog($"Position with Max Intensity: ({maxPos.X.ToString("F2")}{unitLinearAxis}, {maxPos.Y.ToString("F3")}), ({maxPos2.X.ToString("F2")}{unitLinearAxis}, {maxPos2.Y.ToString("F3")}");

            AddLog($"ΔPosition: {deltaPos.ToString("F2")}{unitLinearAxis}");
            AddLog($"Middle of ΔPosition: {returnToPos.ToString("F2")}{unitLinearAxis}");

            // move to the middle position
            if (Args.AxisLinear.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(distMovedLinear - returnToPos)) == false)
                throw new InvalidOperationException(Args.AxisLinear.PhysicalAxisInst.LastError);

            #endregion

            // if the first cycle, rotate to the position calculate according to the delta position and the length of the two DUTs
            double angle = -1 * Math.Asin(deltaPos / Args.Pitch) * (180 / Math.PI);
            AddLog($"The predicted rolling angle is: {angle.ToString("F2")}{unitRotatingAxis}");

            // record the angle rotated
            distMovedRotating += angle;

            if (Args.AxisRotating.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, angle) == false)
                throw new InvalidOperationException(Args.AxisRotating.PhysicalAxisInst.LastError);

            AddLog($"{this} has done!");
        }
        
        public override string ToString()
        {
            return "Fast Rotating Scan";
        }

    }
}
