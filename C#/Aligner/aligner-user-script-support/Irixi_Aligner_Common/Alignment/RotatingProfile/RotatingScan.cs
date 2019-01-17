
using System;
using System.Threading;
using System.Windows;
using Irixi_Aligner_Common.Alignment.Base;
using M12.Base;

namespace Irixi_Aligner_Common.Alignment.RotatingProfile
{
    public class RotatingScan : AlignmentBase
    {
        #region Variables

        

        #endregion

        #region Constructors
        public RotatingScan(RotatingScanArgs Args) : base(Args)
        {
            this.Args = Args;
        }
        #endregion

        #region Properties

        public new RotatingScanArgs Args { get; }

        #endregion

        public override void Start()
        {
            base.Start();

            int cycles = 0;
            double deltaPos = double.MaxValue;
            Log.Clear();

            //while(true)
            //{
            double distMovedLinear = 0, distMovedRotating = 0;
            double halfRangeLinear = Args.LinearRestriction / 2;

            AddLog("Start to align ...");

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

            while (distMovedLinear <= Args.LinearRestriction)
            {
                // read power of channel 1
                var ret = Args.Instrument.Fetch();
                var p = new Point2D(distMovedLinear, ret);
                Args.ScanCurve.Add(p);

                // read power of channel 2
                ret = Args.Instrument2.Fetch();
                p = new Point2D(distMovedLinear, ret);
                Args.ScanCurve2.Add(p);

                // record distance moved
                distMovedLinear += Args.LinearInterval;

                // move to the next point
                if (Args.AxisLinear.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, Args.LinearInterval) == false)
                    throw new InvalidOperationException(Args.AxisLinear.PhysicalAxisInst.LastError);

                // cancel the alignment process
                if (cts_token.IsCancellationRequested)
                {
                    AddLog($"{this} is stopped by user!");
                    return;
                }
            }

            // find the position of max power and draw the constant lines
            /// <seealso cref="ScanCurve.MaxPowerConstantLine"/>
            var maxPos = Args.ScanCurve.FindPositionWithMaxIntensity();
            var maxPos2 = Args.ScanCurve2.FindPositionWithMaxIntensity();

            // calculate the position differential
            deltaPos = maxPos.X - maxPos2.X;

            // move to the middle position of the max power
            var returnToPos = maxPos.X - deltaPos / 2;

            // get the unit of axes
            var unitLinearAxis = Args.AxisLinear.PhysicalAxisInst.UnitHelper;
            var unitRotatingAxis = Args.AxisRotating.PhysicalAxisInst.UnitHelper;

            // output messages
            AddLog($"Position with Max Intensity: ({maxPos.X.ToString("F2")}{unitLinearAxis}, {maxPos.Y.ToString("F3")}), ({maxPos2.X.ToString("F2")}{unitLinearAxis}, {maxPos2.Y.ToString("F3")}");

            AddLog($"ΔPosition: {deltaPos.ToString("F2")}{unitLinearAxis}");
            AddLog($"Middle of ΔPosition: {returnToPos.ToString("F2")}{unitLinearAxis}");

            // move to the middle position
            if (Args.AxisLinear.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(distMovedLinear - returnToPos)) == false)
                throw new InvalidOperationException(Args.AxisLinear.PhysicalAxisInst.LastError);

            //// if it's touched the target, exit the loop
            //if (Math.Abs(deltaPos) <= Args.TargetPositionDifferentialOfMaxPower)
            //    break;

            #endregion

            // if the first cycle, rotate to the position calculate according to the delta position and the length of the two DUTs
            double angle = -1 * Math.Asin(deltaPos / Args.Pitch) * (180 / Math.PI);
            AddLog($"The predicted rolling angle is {angle.ToString("F2")}{unitRotatingAxis}");

            // record the angle rotated
            distMovedRotating += angle;

            if (Args.AxisRotating.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, angle) == false)
                throw new InvalidOperationException(Args.AxisRotating.PhysicalAxisInst.LastError);

            AddLog($"{this} has done!");
        }

        public override void PauseInstruments()
        {
            Args.Instrument.PauseAutoFetching();
            Args.Instrument2.PauseAutoFetching();
        }

        public override void ResumeInstruments()
        {
            Args.Instrument.ResumeAutoFetching();
            Args.Instrument2.ResumeAutoFetching();
        }

        public override string ToString()
        {
            return "Rotating Scan";
        }

    }
}
