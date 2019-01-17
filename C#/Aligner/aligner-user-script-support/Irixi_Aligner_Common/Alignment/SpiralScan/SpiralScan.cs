using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using M12.Base;
using M12.Excpections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Irixi_Aligner_Common.Alignment.SpiralScan
{
    public class SpiralScan : AlignmentBase
    {
        #region variables
        enum MoveSequence
        {
            Down = 0,
            Right,
            Up,
            Left
        }
        #endregion
        
        #region Constructor

        public SpiralScan(SpiralScanArgs Args) : base(Args)
        {
            this.Args = Args;
        }

        #endregion

        #region Properties

        public new SpiralScanArgs Args { get; }

        #endregion

        #region Private Methods
        private void MoveBySequence(MoveSequence Sequence, int Speed, double Distance, ref Point LastPosition)
        {
            switch(Sequence)
            {
                // move along the logical X axis
                case MoveSequence.Right:
                    if (this.Args.Axis.PhysicalAxisInst.Move(MoveMode.REL, Speed, Distance))
                        LastPosition.X += Distance;
                    else
                        throw new InvalidOperationException(this.Args.Axis.PhysicalAxisInst.LastError);

                    break;

                // move along the logical X axis
                case MoveSequence.Left:
                    if (this.Args.Axis.PhysicalAxisInst.Move(MoveMode.REL, Speed, -Distance))
                        LastPosition.X -= Distance;
                    else
                        throw new InvalidOperationException(this.Args.Axis.PhysicalAxisInst.LastError);
                    break;
                    
                // move along the logical Y axis
                case MoveSequence.Up:
                    if (this.Args.Axis2.PhysicalAxisInst.Move(MoveMode.REL, Speed, Distance))
                        LastPosition.Y += Distance;
                    else
                        throw new InvalidOperationException(this.Args.Axis2.PhysicalAxisInst.LastError);

                    break;

                // move along the logical Y axis
                case MoveSequence.Down:
                    if (this.Args.Axis2.PhysicalAxisInst.Move(MoveMode.REL, Speed, -Distance))
                        LastPosition.Y -= Distance;
                    else
                        throw new InvalidOperationException(this.Args.Axis2.PhysicalAxisInst.LastError);

                    break;

                
            }
        }

        #endregion

        #region Methods

        public override void Start()
        {
            base.Start();

            ADCSamplingPointMissException pointMissEx = null;
            double maxIndensity = 0;
            //int moved_points = 0;
            //var curr_pos = new Point(0, 0);
            //var curr_point3d = new Point3D();

            AddLog("Start to align ...");

            Args.ClearScanCurve();

            var haxis = Args.Axis.PhysicalAxisInst as IrixiM12Axis;
            var vaxis = Args.Axis2.PhysicalAxisInst as IrixiM12Axis;
            var m12 = haxis.Parent as IrixiM12;
            List<Point3D> points = null;

            try
            {
                m12.StartBlindSearch(haxis, vaxis, Args.Range, Args.Interval, Args.Interval, Args.MoveSpeed, M12.Definitions.ADCChannels.CH3, out points);
            }
            catch(ADCSamplingPointMissException ex)
            {
                // ran too fast, some ADC sampling points missed.
                pointMissEx = ex;

                AddLog("Some of the sampling points are lost, reduce the speed and try again.");
                AddLog($"{ex.Desired} points desired but only {ex.Reality} points got.");
            }

            // convert position from step to distance.
            for (int i = 0; i < points.Count; i++)
            {
                points[i].X = haxis.UnitHelper.ConvertStepsToDistance((int)points[i].X);
                points[i].Y = vaxis.UnitHelper.ConvertStepsToDistance((int)points[i].Y);
            }

                // draw curve.
            Args.ScanCurve.AddRange(points);

            // move the position with maximum intensity.
            var maxPoint = Args.ScanCurve.FindMaximalPosition3D();
            var lastPoint = Args.ScanCurve.Last();
            var last_x = lastPoint.X;
            var last_y = lastPoint.Y;
            var max_x = maxPoint.X;
            var max_y = maxPoint.Y;

            maxIndensity = maxPoint.Z;

            // Axis0 acts logical X
            Args.Axis.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(last_x - max_x));

            // Axis1 acts logical Y
            Args.Axis2.PhysicalAxisInst.Move(MoveMode.REL, Args.MoveSpeed, -(last_y - max_y));

            AddLog($"Done! {(DateTime.Now - alignStarts).TotalSeconds.ToString("F1")}s costs.");

            if(pointMissEx != null)
                throw pointMissEx;
        }

        public override string ToString()
        {
            return "Blind Search";
        }
        

        #endregion
    }
}
