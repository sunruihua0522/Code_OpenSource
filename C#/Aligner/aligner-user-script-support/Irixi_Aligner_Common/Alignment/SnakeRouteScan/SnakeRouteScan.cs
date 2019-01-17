using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using M12.Base;
using System.Linq;

namespace Irixi_Aligner_Common.Alignment.SnakeRouteScan
{
    public class SnakeRouteScan : AlignmentBase
    {
        #region Variables

        #endregion

        #region Constructors

        public SnakeRouteScan(SnakeRouteScanArgs Args) : base(Args)
        {
            this.Args = Args;
        }

        #endregion

        #region Properties

        public new SnakeRouteScanArgs Args { get; }

        #endregion

        #region Methods

        public override void Start()
        {
            base.Start();

            Args.ScanCurveGroup.ClearCurvesContent();

            double hDir = 1;
            double hMoved = 0, vMoved = 0;

            LogicalAxis hAxis = Args.Axis, vAxis = Args.Axis2;
            
            do
            {
                #region Horizontal Scan

                do
                {
                    // read indensity
                    var indensity = Args.Instrument.Fetch();

                    // draw curve
                    Args.ScanCurve.Add(new Point3D(hMoved, vMoved, indensity));

                    // move along the horizontal direction
                    hAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, hDir * Args.ScanInterval);

                    // cancel the alignment process
                    if (cts_token.IsCancellationRequested)
                        break;

                    if(hDir > 0)
                    {
                        hMoved += Args.ScanInterval;

                        if (hMoved >= Args.AxisRestriction)
                            break;
                    }
                    else
                    {
                        hMoved -= Args.ScanInterval;

                        if (hMoved <= 0)
                            break;
                    }

                } while (true);

                // cancel the alignment process
                if (cts_token.IsCancellationRequested)
                    return;

                #endregion

                // check if the vertical position is out of the restriction
                if (vMoved < Args.Axis2Restriction)
                {
                    // change the direction of the next horizontal scan
                    hDir *= -1;

                    // move along the vertical direction
                    vAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, Args.ScanInterval);

                    vMoved += Args.ScanInterval;
                }
                else
                {
                    break;
                }

            } while (true);

            var maxPoint = Args.ScanCurve.FindMaximalPosition3D();
            var lastPoint = Args.ScanCurve.Last();

            var last_x = lastPoint.X;
            var last_y = lastPoint.Y;
            var max_x = maxPoint.X;
            var max_y = maxPoint.Y;

            hAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(last_x - max_x));
            
            vAxis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(last_y - max_y));

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
            return "Snake Route Scan";
        }

        #endregion
    }
}
