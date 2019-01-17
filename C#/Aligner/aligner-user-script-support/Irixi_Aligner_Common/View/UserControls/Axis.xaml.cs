using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Irixi_Aligner_Common.View.UserControls
{
    /// <summary>
    /// Axis.xaml 的交互逻辑
    /// </summary>
    public partial class Axis : UserControl
    {
        public Axis()
        {
            InitializeComponent();
        }

        #region Events
        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            var axis = this.DataContext as LogicalAxis;

            Button btn = sender as Button;

            string dir = btn.Tag.ToString();
            double distance = axis.MoveArgs.Distance;
            
            // if move backword, the distance should be minus
            if (dir == "CCW")
            {
                if(axis.Config.SwitchButtonDirection == false)
                    distance *= -1;
                
            }
            else if(dir == "CW")
            {
                if(axis.Config.SwitchButtonDirection == true)
                    distance *= -1;
            }

            // call the move function of the systemservice
            axis.Move.Execute(
                new AxisMoveArgs(axis.MoveArgs.Mode, axis.MoveArgs.Speed, distance, axis.MoveArgs.Unit));
        }

        private void tbtnAbsMode_Click(object sender, RoutedEventArgs e)
        {
            var axis = this.DataContext as LogicalAxis;
            axis.ToggleMoveMode();
        }

        private void BtnCurise_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var axis = this.DataContext as LogicalAxis;
            var args = new AxisCruiseArgs();
            Button btn = sender as Button;
            var dir = btn.Tag.ToString();


            int selectedIndex = (int)cbxCruiseSpeed.SelectedIndex;
            switch (selectedIndex)
            {
                case 0: // Fast
                    args.Speed = AxisCruiseArgs.CruiseSpeed.FAST;
                    break;

                case 1: // Middle
                    args.Speed = AxisCruiseArgs.CruiseSpeed.MID;
                    break;

                case 2: // Slow
                    args.Speed = AxisCruiseArgs.CruiseSpeed.SLOW;
                    break;
            }

            // if move backword, the distance should be minus
            if (dir == "CCW")
            {
                if (axis.Config.SwitchButtonDirection == false)
                    args.Direction = AxisCruiseArgs.CruiseDirection.CCW;
                else
                    args.Direction = AxisCruiseArgs.CruiseDirection.CW;

            }
            else if (dir == "CW")
            {
                if (axis.Config.SwitchButtonDirection == false)
                    args.Direction = AxisCruiseArgs.CruiseDirection.CW;
                else
                    args.Direction = AxisCruiseArgs.CruiseDirection.CCW;
            }

            axis.Curise.Execute(args);
        }

        private void BtnCurise_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var axis = this.DataContext as LogicalAxis;
            axis.Stop.Execute(null);
        }

        #endregion


    }

    //class AxisUserControlViewModel
    //{
    //    public SystemService Service { get; set; }
    //    public ConfigLogicalAxis LogicalAxis { get; set; }
    //}

    /// <summary>
    /// The converter converts the IsCheck value to the caption text of the ABS/REL toggle button
    /// </summary>
    class ConvertAbsModeToCaption : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MoveMode mode = (MoveMode)value;

            // if IsChecked == true, the ABS mode is available
            if (mode == MoveMode.ABS)
            {
                return "ABS";
            }
            else
            {
                return "REL";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();
            if(str == "ABS")
            {
                return MoveMode.ABS;
            }
            else
            {
                return MoveMode.REL;
            }
        }
    }

    class ConvertMoveModeToSolidColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MoveMode mode = (MoveMode)value;
            if (mode == MoveMode.REL)
            {
                return new SolidColorBrush(Colors.Black);
            }
            else
            {
                
                return new SolidColorBrush(Color.FromRgb(0xFF, 0x7A, 0x1E));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ConvertBoolToSolidColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool)value;
            if (v == true)
            {
                return new SolidColorBrush(Color.FromRgb(0xFF, 0x7A, 0x1E));
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// The converter converts the IsCheck value to the caption text of the Manual toggle button
    /// </summary>
    class ConvertManualEnabledToCaption : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ret = (bool)value;

            // if IsChecked == true, the axis is locked
            if (ret)
            {
                return "N";
            }
            else
            {
                return "Y";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
