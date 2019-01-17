using System;
using System.Linq;
using Irixi_Aligner_Common.MotionControllers.Base;

namespace Irixi_Aligner_Common.Configuration
{
    public class MotorizedStagesProfileManager
    {
        public MotorizedStageProfileContainer[] ProfileContainer { set; get; }

        /// <summary>
        /// Find the profile container by vendor
        /// </summary>
        /// <param name="Vendor"></param>
        /// <returns></returns>
        public MotorizedStageProfileContainer FindProfileContainer(string Vendor)
        {
            var containers = this.ProfileContainer.Where(p => p.Vendor == Vendor);
            if (containers.Any())
                return containers.First();
            else
                return null;
        }


        /// <summary>
        /// Find the profile by vendor and model
        /// </summary>
        /// <param name="Vendor"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public MotorizedStageProfile FindProfile(string Vendor, string Model)
        {
            var container = this.FindProfileContainer(Vendor);
            if(container != null)
            {
                var profile = container.Profiles.Where(p => p.Model == Model);
                if (profile.Any())
                    return profile.First();
                else
                    return null;
            }
            else
            {
                return null;
            }
        }        
    }

    public class MotorizedStageProfileContainer
    {
        public string Vendor { get; set; }
        public MotorizedStageProfile[] Profiles {get;set;}
    }

    public class MotorizedStageProfile : ICloneable
    {
        public string Model { set; get; }
        public RealworldUnitManager.UnitType Unit { set; get; }
        public double Resolution { set; get; }
        public double TravelDistance { set; get; }

        /// <summary>
        /// recalculate the parameters if the unit is changed
        /// </summary>
        /// <param name="TargetUnit"></param>
        public void ChangeUnit(RealworldUnitManager.UnitType TargetUnit)
        {
            // STEP 1
            //  Convert to nm & sec
            switch (this.Unit)
            {
                case RealworldUnitManager.UnitType.mm:
                    TravelDistance *= 1000000.0;
                    Resolution *= 1000000.0;
                    break;

                case RealworldUnitManager.UnitType.um:
                    TravelDistance *= 1000.0;
                    Resolution *= 1000.0;
                    break;

                case RealworldUnitManager.UnitType.deg:
                    TravelDistance *= 3600.0;
                    Resolution *= 3600.0;
                    break;

                case RealworldUnitManager.UnitType.min:
                    TravelDistance *= 60.0;
                    Resolution *= 60.0;
                    break;
            }

            // STEP 2
            //  Convert to the specified unit
            switch (TargetUnit)
            {
                case RealworldUnitManager.UnitType.mm:
                    TravelDistance /= 1000000.0;
                    Resolution /= 1000000.0;
                    break;

                case RealworldUnitManager.UnitType.um:
                    TravelDistance /= 1000.0;
                    Resolution /= 1000.0;
                    break;

                case RealworldUnitManager.UnitType.deg:
                    TravelDistance /= 3600.0;
                    Resolution /= 3600.0;
                    break;

                case RealworldUnitManager.UnitType.min:
                    TravelDistance /= 60.0;
                    Resolution /= 60.0;
                    break;
            }
            this.Unit = TargetUnit;
        }

        public object Clone()
        {
            return new MotorizedStageProfile()
            {
                Model = this.Model,
                Unit = this.Unit,
                Resolution = this.Resolution,
                TravelDistance = this.TravelDistance
            };
        }
    }
}
