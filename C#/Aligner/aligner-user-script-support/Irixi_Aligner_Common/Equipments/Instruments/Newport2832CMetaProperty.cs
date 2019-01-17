using GalaSoft.MvvmLight;

namespace Irixi_Aligner_Common.Equipments.Instruments
{
    public class Newport2832CMetaProperty : ViewModelBase
    {
        Newport2832C.EnumRange range;
        Newport2832C.EnumUnits unit;
        Newport2832C.EnumStatusFlag status;

        double measured_val;
        int lambda;
        bool isOverRange, isSaturated, isDataError, isRanging;

        public Newport2832C.EnumStatusFlag Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;

                if (status.HasFlag(Newport2832C.EnumStatusFlag.DATAERR))
                    IsDataError = true;
                else
                    IsDataError = false;

                if (status.HasFlag(Newport2832C.EnumStatusFlag.OVERRANGE))
                    IsOverRange = true;
                else
                    IsOverRange = false;

                if (status.HasFlag(Newport2832C.EnumStatusFlag.SATURATED))
                    IsSaturated = true;
                else
                    IsSaturated = false;

                if (status.HasFlag(Newport2832C.EnumStatusFlag.RANGING))
                    IsRanging = true;
                else
                    IsRanging = false;

                RaisePropertyChanged();
            }
        }

        public bool IsOverRange
        {
            get
            {
                return isOverRange;
            }
            set
            {
                isOverRange = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSaturated
        {
            get
            {
                return isSaturated;
            }
            set
            {
                isSaturated = value;
                RaisePropertyChanged();
            }
        }

        public bool IsDataError
        {
            get
            {
                return isDataError;
            }
            set
            {
                isDataError = value;
                RaisePropertyChanged();
            }
        }

        public bool IsRanging
        {
            get
            {
                return isRanging;
            }
            set
            {
                isRanging = value;
                RaisePropertyChanged();
            }
        }

        public Newport2832C.EnumRange Range
        {
            set
            {
                range = value;
                RaisePropertyChanged();
            }
            get
            {
                return range;
            }
        }

        public int Lambda
        {
            get
            {
                return lambda;
            }
            set
            {
                lambda = value;
                RaisePropertyChanged();
            }
        }

        public Newport2832C.EnumUnits Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
                RaisePropertyChanged();
            }
        }

        public double MeasurementValue
        {
            set
            {
                measured_val = value;
                RaisePropertyChanged();
            }
            get
            {
                return measured_val;
            }
        }
    }
}
