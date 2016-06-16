using System;
using PlethoraModels;

namespace ImportJsonApplication.Utilities
{
    public class CostUtilities
    {
        /// <summary>
        /// Returns the material cost
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public static double GetMaterialCost(double area)
        {
            return area*PlethoraDefinitions.MaterialCost;
        }

        /// <summary>
        /// Returns the laser speed for an arc
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetLaserSpeedForArc(double radius)
        {
            return PlethoraDefinitions.MaxLaserSpeed*Math.Exp(-1/radius);
        }

        /// <summary>
        /// Returns the Machien time cost
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double GetMachineTimeCost(double time)
        {
            return time*PlethoraDefinitions.MachineTimeCost;
        }

        /// <summary>
        /// Get the time that the laser needs to run for
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="perimeter"></param>
        /// <returns></returns>
        public static double GetRequiredTime(double speed, double perimeter)
        {
            return perimeter / speed;
        }
    }
}
