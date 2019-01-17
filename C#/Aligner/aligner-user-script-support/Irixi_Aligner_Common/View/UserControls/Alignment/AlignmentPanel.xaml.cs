using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Irixi_Aligner_Common.View.UserControls.AlignmentView
{
    /// <summary>
    /// Interaction logic for AlignmentPanel.xaml
    /// </summary>
    public partial class AlignmentPanel : UserControl
    {
        public AlignmentPanel()
        {
            InitializeComponent();
        }

        #region Properties
        
        public Uri PropertiesEditTamplatePath
        {
            get { return (Uri)GetValue(PropertiesEditTamplatePathProperty); }
            set { SetValue(PropertiesEditTamplatePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertiesTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertiesEditTamplatePathProperty =
            DependencyProperty.Register("PropertiesEditTamplatePath", typeof(Uri), typeof(UserControl), 
                new PropertyMetadata(null, (s, e)=> 
                {
                    var owner = s as AlignmentPanel;
                    owner.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = (Uri)e.NewValue });
                }));


        public Transform3DGroup Chart3DTransform
        {
            get { return (Transform3DGroup)GetValue(Chart3DTransformProperty); }
            set { SetValue(Chart3DTransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for transform3DGroup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Chart3DTransformProperty =
            DependencyProperty.Register("Chart3DTransform", typeof(Transform3DGroup), typeof(UserControl), new PropertyMetadata(new Transform3DGroup()
            {
                Children = new Transform3DCollection()
                {
                    new RotateTransform3D(new AxisAngleRotation3D()
                {
                    Angle = -40,
                    Axis = new Vector3D(0, 1, 0)
                }),

                new RotateTransform3D(new AxisAngleRotation3D()
                {
                    Angle = 20,
                    Axis = new Vector3D(1, 0, 0)
                })
                }
            }));

        #endregion
        
        #region Methods

        private RelayCommand Reset3DChartView
        {
            get
            {
                return new RelayCommand(() =>
                {

                    Transform3DCollection collection = new Transform3DCollection
                    {
                        new RotateTransform3D(new AxisAngleRotation3D()
                        {
                            Angle = -40,
                            Axis = new Vector3D(0, 1, 0)
                        }),

                        new RotateTransform3D(new AxisAngleRotation3D()
                        {
                            Angle = 20,
                            Axis = new Vector3D(1, 0, 0)
                        })
                    };

                    Transform3DGroup group = new Transform3DGroup()
                    {
                        Children = collection,
                    };

                    this.Chart3DTransform = group;
                });
            }
        }


        /// <summary>
        /// Reset set the view of 3D chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmResetView_Click(object sender, RoutedEventArgs e)
        {
            Transform3DCollection collection = new Transform3DCollection
            {
                new RotateTransform3D(new AxisAngleRotation3D()
                {
                    Angle = -40,
                    Axis = new Vector3D(0, 1, 0)
                }),

                new RotateTransform3D(new AxisAngleRotation3D()
                {
                    Angle = 20,
                    Axis = new Vector3D(1, 0, 0)
                })
            };

            Transform3DGroup group = new Transform3DGroup()
            {
                Children = collection,
            };

            this.Chart3DTransform = group;
        }

        #endregion

    }
}
