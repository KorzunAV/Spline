using System;

namespace Spline.Core.KOA
{
	public class BSpline
	{

		public static double[] SplinePoints(double[] points, int iteration)
		{
			var buf = new double[points.Length];
			Array.Copy(points, buf, points.Length);

			for (int i = 0; i < iteration; i++)
			{
				buf = SplinePoints(buf);
			}
			return buf;
		}


		public static double[] SplinePoints(double[] points)
		{
			var buf = new double[points.Length];
			int k = 0;
			buf[0] = SplinePart(points[k++], points[k++], points[k++], points[k], true);
			buf[1] = points[1];

			for (int i = 0; i < points.Length - 3; i++)
			{
				buf[i + 2] = SplinePart(points[i], points[i + 1], points[i + 2], points[i + 3], false);
			}

			buf[points.Length - 1] = points[points.Length - 1];
			return buf;
		}

		private static double SplinePart(double p1, double p2, double p3, double p4, bool zeroPoint)
		{
			var b3 = (-p1 + 3.0 * (p2 - p3) + p4) / 6.0;
			var b2 = (p1 - 2.0 * p2 + p3) / 2.0;
			var b1 = (p3 - p1) / 2.0;
			var b0 = (p1 + 4.0 * p2 + p3) / 6.0;

			if (zeroPoint)
			{
				return b0;
			}
			return b3 + b2 + b1 + b0;
		}
	}
}