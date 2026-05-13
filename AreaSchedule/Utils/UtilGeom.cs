using System;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public static class UtilGeom
    {
        /// <summary>Radians to degrees.</summary>
        public static double AngleDeg(double radians) => radians * 180.0 / Math.PI;
    }
}
