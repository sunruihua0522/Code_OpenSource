using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using Irixi_Aligner_Common.MotionControllers.Base;
using M12.Base;
using M12.Definitions;
using M12.Excpections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Irixi_Aligner_Common.Alignment.FastCentralAlign
{
    public class FastCentralAlign : AlignmentBase
    {
        #region Constructors

        public FastCentralAlign(FastCentralAlignArgs Args) : base(Args)
        {
            this.Args = Args;
        }

        #endregion

        #region Properties

        public new FastCentralAlignArgs Args { get; }
        
        #endregion

        #region Methods

        public override void Start()
        {
            base.Start();

            int state = 0;

            ADCSamplingPointMissException pointsMissedEx = null;

            LogicalAxis activeAxis = Args.Axis;

            var range = Args.HorizonalRange;
            var interval = Args.HorizonalInterval;
            double moved = 0;

            AddLog($"Start to align ...");

            _align:

            // reset arguments
            Args.ScanCurveGroup.ClearCurvesContent();
            moved = 0;

            // move to the start point
            if (activeAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(range / 2)) == false)
                throw new InvalidOperationException(activeAxis.PhysicalAxisInst.LastError);


            var axis = activeAxis.PhysicalAxisInst as IrixiM12Axis;
            var m12 = axis.Parent as IrixiM12;

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

            var maxPos = Args.ScanCurve.FindPositionWithMaxIntensity();
            var maxPos2 = Args.ScanCurve2.FindPositionWithMaxIntensity();
            var diffPos = maxPos.X - maxPos2.X;

            // return to the position with maximum indensity
            var returnToPos = maxPos.X - diffPos / 2;

            // output messages
            var unitHelper = activeAxis.PhysicalAxisInst.UnitHelper;
            AddLog($"Max Intensity Position: ({maxPos.X.ToString("F1")}{unitHelper}, {maxPos.Y.ToString("F3")}), ({maxPos2.X.ToString("F1")}{unitHelper}, {maxPos2.Y.ToString("F3")})");

            AddLog($"ΔPosition: {diffPos.ToString("F1")}{activeAxis}");
            AddLog($"Middle of ΔPosition: {returnToPos.ToString("F1")}{unitHelper}");

            if (activeAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(moved - returnToPos)) == false)
                throw new InvalidOperationException(activeAxis.PhysicalAxisInst.LastError);

            // switch to the next axis to scan
            if (state < 1)
            {
                state++;
                activeAxis = Args.Axis2;
                range = Args.VerticalRange;

                Task.Delay(500);

                goto _align;
            }

            AddLog($"Done! {(DateTime.Now - alignStarts).TotalSeconds.ToString("F1")}s costs.");
        }

        public override string ToString()
        {
            return "Fast Central Align";
        }

        #endregion
    }
}
