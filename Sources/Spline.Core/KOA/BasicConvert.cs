using System;

namespace Spline.Core.KOA
{
    public class BasicConvert
    {
        /// <summary>
        /// преобразут значение угла в угол от 0 до 2*PI радиан
        /// </summary>
        /// <param name="rad">угол в радианах</param>
        /// <returns></returns>
        public static double RadToRad360(double rad)
        {
            double retRad = rad % (Math.PI * 2);

            if (rad < 0)
            {
                retRad = Math.PI * 2 + retRad;
            }
            return retRad;
        }

        /// <summary>
        /// переводит из градусов в радианы
        /// </summary>
        /// <param name="grad">угол в градусах</param>
        /// <returns></returns>
        public static double GradToRad(double grad)
        {
            return Math.PI * grad / 180.0;
        }

        /// <summary>
        /// переводит из радиан в градусы
        /// </summary>
        /// <param name="rad">угол в радианах</param>
        /// <returns></returns>
        public static double RadToGrad(double rad)
        {
            return 180.0 * rad / Math.PI;
        }
    }
}