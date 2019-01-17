using System;
using System.Collections.Generic;

namespace Irixi_Aligner_Common.Interfaces
{
    public interface IMotionController : IEquipment, IStoppable
    {
        /// <summary>
        /// Raise the event after the Home/Move/etc. action begins
        /// </summary>
        event EventHandler OnMoveBegin;

        /// <summary>
        /// Raise the event after the Home/Move/etc. action ends
        /// </summary>
        event EventHandler OnMoveEnd;

        #region Properties

        /// <summary>
        /// Get the model of this controller.
        /// </summary>
        MotionControllerType Model { get; }

        /// <summary>
        /// See <see cref="MotionControllers.MotionControllerBase"/> for the detail of the usage
        /// </summary>
        int BusyAxesCount { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the instance of physical axis object by axis name
        /// </summary>
        /// <param name="AxisName"></param>
        /// <returns></returns>
        IAxis GetAxisByName(string AxisName);
        
        /// <summary>
        /// <see cref="IAxis.Home"/> for more infomation
        /// </summary>
        /// <param name="Axis"></param>
        bool Home(IAxis Axis);

        /// <summary>
        /// <see cref="IAxis.Move(MoveMode, int, int)"/> for more infomation
        /// </summary>
        /// <param name="Axis">The instance of axis class inherited IAxis</param>
        /// <param name="Mode"></param>
        /// <param name="Speed"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        bool Move(IAxis Axis, MoveMode Mode, int Speed, int Distance);

        /// <summary>
        /// <see cref="IAxis.MoveWithTrigger(MoveMode, int, int, int, int)"/> for more infomation
        /// </summary>
        /// <param name="Axis">The instance of axis class inherited IAxis</param>
        /// <param name="Mode"></param>
        /// <param name="Speed"></param>
        /// <param name="Distance"></param>
        /// <param name="Interval"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        bool MoveWithTrigger(IAxis Axis, MoveMode Mode, int Speed, int Distance, int Interval, int Channel);

        /// <summary>
        /// <see cref="IAxis.MoveWithInnerADC(MoveMode, int, int, int, int)"/> for more infomation
        /// </summary>
        /// <param name="Axis">The instance of axis class inherited IAxis</param>
        /// <param name="Mode"></param>
        /// <param name="Speed"></param>
        /// <param name="Distance"></param>
        /// <param name="Interval"></param>
        /// <param name="AdcIndex"></param>
        /// <returns></returns>
        bool MoveWithInnerADC(IAxis Axis, MoveMode Mode, int Speed, int Distance, int Interval, int Channel);

        /// <summary>
        /// <see cref="IAxis.Stop"/> for more infomation
        /// </summary>
        //void Stop();

        #endregion
    }
}
