using M12.Base;
using System.Collections;
using System.Windows;

namespace Irixi_Aligner_Common.Interfaces
{
    public interface IScanCurve : IList
    {


        #region Properties

        string DisplayName { set; get; }
        string Prefix { set; get; }
        string Suffix { set; get; }

        #endregion


        #region Methods

        Point2D FindPositionWithMaxIntensity();

        Point3D FindMaximalPosition3D();

        #endregion

    }
}
