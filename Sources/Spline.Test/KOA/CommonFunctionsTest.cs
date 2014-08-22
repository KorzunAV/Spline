using System;
using NUnit.Framework;
using Spline.Core;
using Spline.Core.KOA;

namespace Spline.Test.KOA
{
	[TestFixture]
	public class CommonFunctionsTest
	{
		[Test]
		public void GetAnglDifferentTest90()
		{
			var pt1 = new Point(0, 0);
			var pt2 = new Point(1, 1);
			var pt3 = new Point(2, 0);
			var rez = CommonFunctions.GetAnglDifferent(pt1, pt2, pt3);
			var grad = BasicConvert.RadToGrad(rez);
			Assert.IsTrue(Math.Abs(grad - 90) < 0.000001);

			pt1 = new Point(0, 0);
			pt2 = new Point(1, -1);
			pt3 = new Point(2, 0);
			rez = CommonFunctions.GetAnglDifferent(pt1, pt2, pt3);
			grad = BasicConvert.RadToGrad(rez);
			Assert.IsTrue(Math.Abs(grad + 90) < 0.000001);
		}


		[Test]
		public void GetAnglDifferentTest60()
		{
			var pt1 = new Point(0, 0);
			var pt2 = new Point(Math.Sqrt(3), 1);
			var pt3 = new Point(2 * Math.Sqrt(3), 0);
			var rez = CommonFunctions.GetAnglDifferent(pt1, pt2, pt3);
			var grad = BasicConvert.RadToGrad(rez);
			Assert.IsTrue(Math.Abs(grad - 60) < 0.000001);

			pt1 = new Point(0, 0);
			pt2 = new Point(-Math.Sqrt(3), 1);
			pt3 = new Point(-2 * Math.Sqrt(3), 0);
			rez = CommonFunctions.GetAnglDifferent(pt1, pt2, pt3);
			grad = BasicConvert.RadToGrad(rez);
			Assert.IsTrue(Math.Abs(grad + 60) < 0.000001);
		}



	}
}
