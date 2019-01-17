using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Irixi_Aligner_Common.UserControls
{
    /// <summary>
    /// Interaction logic for AlignmentPanelBlindSearch.xaml
    /// </summary>
    public partial class AlignmentPanelBlindSearch : UserControl
    {
        public AlignmentPanelBlindSearch()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reset set the view of 3D chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmResetView_Click(object sender, RoutedEventArgs e)
        {
            Transform3DCollection collection = new Transform3DCollection();
            collection.Add(new RotateTransform3D(new AxisAngleRotation3D()
            {
                Angle = -40,
                Axis = new Vector3D(0, 1, 0)
            }));

            collection.Add(new RotateTransform3D(new AxisAngleRotation3D()
            {
                Angle = 20,
                Axis = new Vector3D(1, 0, 0)
            }));

            Transform3DGroup group = new Transform3DGroup()
            {
                Children = collection,
            };
        
            chartBlindSearch.ContentTransform = group;
        }
    }
}
