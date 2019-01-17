using System;
using System.Threading.Tasks;
using System.Windows;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using M12.Base;

namespace Irixi_Aligner_Common.Alignment.CentralAlign
{
    public class CentralAlign : AlignmentBase
    {
        #region Constructors

        public CentralAlign(CentralAlignArgs Args) : base(Args)
        {
            this.Args = Args;
        }

        #endregion

        #region Properties

        public new CentralAlignArgs Args { get; }

        #endregion

        #region Methods

        public override void Start()
        {
            base.Start();

            int state = 0;

            LogicalAxis activeAxis = Args.Axis;
            double restriction = Args.HorizonalRange, interval = Args.HorizonalInterval;
            double moved = 0;

            AddLog("Start to align ...");

            try
            {
                PauseInstruments();

                _align:

                // reset arguments
                Args.ScanCurveGroup.ClearCurvesContent();
                moved = 0;

                // move to the start point
                if (activeAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(restriction / 2)) == false)
                    throw new InvalidOperationException(activeAxis.PhysicalAxisInst.LastError);

                do
                {
                    // start to scan
                    var indensity = Args.Instrument.Fetch();
                    var indensity2 = Args.Instrument2.Fetch();

                    Args.ScanCurve.Add(new Point2D(moved, indensity));
                    Args.ScanCurve2.Add(new Point2D(moved, indensity2));

                    if (moved < restriction)
                    {
                        // move the interval
                        activeAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, interval);

                        moved += interval;
                    }
                    else
                    {
                        break;
                    }

                    // cancel the alignment process
                    if (cts_token.IsCancellationRequested)
                    {
                        AddLog($"{this} is stopped by user!");
                        return;
                    }

                } while (true);

                var maxPos = Args.ScanCurve.FindPositionWithMaxIntensity();
                var maxPos2 = Args.ScanCurve2.FindPositionWithMaxIntensity();
                var diffPos = maxPos.X - maxPos2.X;

                // return to the position with maximum indensity
                var returnToPos = maxPos.X - diffPos / 2;

                // output messages
                var unitHelper = activeAxis.PhysicalAxisInst.UnitHelper;
                AddLog($"Position with Max Intensity: ({maxPos.X.ToString("F1")}{unitHelper}, {maxPos.Y.ToString("F3")}), ({maxPos2.X.ToString("F1")}{unitHelper}, {maxPos2.Y.ToString("F3")})");

                AddLog($"ΔPosition: {diffPos.ToString("F1")}{activeAxis}");
                AddLog($"Middle of ΔPosition: {returnToPos.ToString("F1")}{unitHelper}");

                if (activeAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(moved - returnToPos)) == false)
                    throw new InvalidOperationException(activeAxis.PhysicalAxisInst.LastError);

                // switch to the next axis to scan
                if (state < 1)
                {
                    state++;
                    activeAxis = Args.Axis2;
                    restriction = Args.VerticalRange;

                    Task.Delay(500);

                    goto _align;
                }

                AddLog($"{this} has done!");
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
            Args.Instrument2.PauseAutoFetching();
        }

        public override void ResumeInstruments()
        {
            Args.Instrument.ResumeAutoFetching();
            Args.Instrument2.ResumeAutoFetching();
        }

        public override string ToString()
        {
            return "Central Align";
        }

        #endregion
    }
}
