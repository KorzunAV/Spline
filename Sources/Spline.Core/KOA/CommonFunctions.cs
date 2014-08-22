using System;
using Spline.Interfaces;

namespace Spline.Core.KOA
{
	/// <summary>
	/// набор функций для вспомагательных расчетов 
	/// </summary>
	public class CommonFunctions
	{
		/// <summary>
		/// получение угла отклонения двух прямых
		/// </summary>
		/// <param name="point1">начало первой прямой</param>
		/// <param name="point2">общая точка</param>
		/// <param name="point3">конец второй прямой</param>
		/// <returns></returns>
		public static double GetAnglDifferent(IPoint point1, IPoint point2, IPoint point3)
		{
			var dx1 = point2.X - point1.X;
			var dx2 = point3.X - point2.X;
			var dy1 = point2.Y - point1.Y;
			var dy2 = point3.Y - point2.Y;
			var i1 = Math.Atan(dy1 / dx1);
			var i2 = Math.Atan(dy2 / dx2);
			var tan = i1 - i2;
			return tan;
		}

		/// <summary>
		/// получение углов отклонения. Первая и последняя точка = 0
		/// </summary>
		/// <param name="points">набор точек</param>
		/// <returns></returns>
		public static double[] GetAnglDifferent(IPoint[] points)
		{
			var retAngl = new double[points.Length];

			for (int i = 0; i < points.Length - 1; i++)
			{
				retAngl[i + 1] = GetAnglDifferent(points[i], points[i + 1], points[i + 2]);
			}
			return retAngl;
		}
		



		//public static LinkedList<RoadSector> FirstGroupRoadPlan(GPS2DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, ref Config Conf)
		//{
		//	// генерируем усредненный набор рабочих данных для первичной обработки
		//	if (Conf.GroupAuto)
		//		SetKof(AnglDifferentSet, ref Conf);
		//	var BfCleanAngls = KSpline.CleanNoise(AnglDifferentSet, Conf);

		//	var CleanAngls = KSpline.ToDoubleArray(BfCleanAngls);
		//	BfCleanAngls = KSpline.KLineSpline(BfCleanAngls, Conf.GroupSplainKof); // слаживаем углы
		//	var CleanAndSplineAngls = KSpline.ToDoubleArray(BfCleanAngls);

		//	// проводим комбинированный поиск прямых 
		//	var lineset = GetLineGroupList(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf);
		//	var lineGroup = LineSetToGroup(InputeRoadPoint, lineset);

		//	// уточняем наборы рабочих данных
		//	BfCleanAngls = MergeKAngle(BfCleanAngls, AnglDifferentSet, lineset);
		//	CleanAngls = KSpline.ToDoubleArray(BfCleanAngls);
		//	BfCleanAngls = KSpline.KLineSpline(BfCleanAngls, Conf.GroupSplainKof); // слаживаем углы
		//	CleanAndSplineAngls = KSpline.ToDoubleArray(BfCleanAngls);

		//	// группируем дорогу
		//	// очень чувствительный метод
		//	//var RoadGroupList = ClipCrclrGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, lineGroup);            
		//	// мало чувствительный метод
		//	var RoadGroupList = ExtremumGroupFinder(CleanAndSplineAngls, Conf);

		//	// определяем типы участков
		//	DefineGroups(CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList, true);

		//	// допилы
		//	СonnectSmall(AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList);
		//	int Oldcount = RoadGroupList.Count;

		//	do
		//	{
		//		Oldcount = RoadGroupList.Count;
		//		FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);
		//		CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, ref RoadGroupList);
		//		DefineLineByRad(Conf, ref RoadGroupList);
		//		RoadGroupList = BetweenLine(AnglDifferentSet, Conf, RoadGroupList);
		//		RoadGroupList = MergeCircular(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, RoadGroupList);
		//		СonnectGroop(AnglDifferentSet, ref RoadGroupList);
		//	} while (Oldcount != RoadGroupList.Count);

		//	RoadGroupList = Else(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, RoadGroupList);

		//	return RoadGroupList;
		//}





		///// <summary>
		///// получение набора азимутов для направляющих к центру груговой кривой
		///// </summary>
		///// <param name="rs">данные об участке</param>
		///// <param name="StartEndAzimuthLen">данные о длинне/азимуте в точках догоги</param>
		///// <param name="AnglDifferentSet">данные о относительном угле поворота дороги</param>
		///// <returns></returns>
		//public static double[] AzimForTangentialLine(RoadSector rs, StartEndAzimuthLen[] SEAzimLen, double[] AnglDifferentSet)
		//{
		//	if (rs.IdDAStart > rs.IdDAEnd || rs.IdDAStart < 0 || SEAzimLen.Length < (rs.IdDAEnd - rs.IdDAStart)
		//		|| AnglDifferentSet.Length <= (rs.IdDAEnd - rs.IdDAStart))
		//		throw new ArgumentException("AzimforTangentialLine => кривые входные параметры");

		//	double[] tangAnglInGroup = new double[rs.IdDAEnd - rs.IdDAStart + 1]; //угол для каждой точки области
		//	int incJ = 0; // 

		//	int LR = 0; // определяем постоянную поворота по качеству отклонения элементов в AnglDifferentSet
		//	for (int j = rs.IdDAStart; j <= rs.IdDAEnd; j++)
		//		LR = AnglDifferentSet[j] > 0 ? LR + 1 : LR - 1;

		//	for (int j = rs.IdDAStart; j <= rs.IdDAEnd; j++) // для всей области
		//	{
		//		int sgt = LR > 0 ? -1 : 1;
		//		tangAnglInGroup[incJ++] = BasicConvert.RadToRad360((SEAzimLen[j + 1].StartAzimuth + sgt * Math.PI / 2.0) % (Math.PI * 2.0));
		//	}
		//	return tangAnglInGroup;
		//}
		///// <summary>
		///// поворот координат точки M
		///// </summary>
		///// <param name="MPoint">точка</param>
		///// <param name="angle">угол поворота в радианах</param>
		///// <param name="CenterPoint">точка относительно которой происходит поворот</param>
		///// <returns></returns>
		//public static GPS2DCoordinat Rotate(GPS2DCoordinat MPoint, double angle, GPS2DCoordinat CenterPoint, Config Conf)
		//{
		//	double kof = GetLaLoKof(CenterPoint, Conf);

		//	var la = (MPoint.Latitude * kof - CenterPoint.Latitude) * Math.Cos(angle) + (MPoint.Longitude - CenterPoint.Longitude) * Math.Sin(angle);
		//	var lo = -(MPoint.Latitude * kof - CenterPoint.Latitude) * Math.Sin(angle) + (MPoint.Longitude - CenterPoint.Longitude) * Math.Cos(angle);

		//	var ret = RotateObr(new GPS2DCoordinat(la, lo), angle, CenterPoint, Conf);
		//	return new GPS2DCoordinat(la, lo);
		//}
		///// <summary>
		///// поварачивает точки скопом
		///// </summary>
		///// <param name="angle">угол поворота в радианах</param>
		///// <param name="CenterPoint">точка относительно которой происходит поворот</param>
		///// <param name="MPoint">набор точек</param>
		//public static void Rotate(double angle, GPS2DCoordinat CenterPoint, Config Conf, ref GPS2DCoordinat[] MPoints)
		//{
		//	for (int i = 0; i < MPoints.Length; i++)
		//	{
		//		MPoints[i] = Rotate(MPoints[i], angle, CenterPoint, Conf);
		//	}
		//}
		///// <summary>
		///// обартный поворот точки
		///// </summary>
		///// <param name="angle">угол поворота в радианах</param>
		///// <param name="X">значение икса</param>
		///// <param name="Y">занчение игрика</param>
		///// <param name="Pt">точка относительно которой происходил поворот</param>
		///// <returns></returns>
		//public static GPS2DCoordinat RotateObr(GPS2DCoordinat MPoint, double angle, GPS2DCoordinat CenterPoint, Config Conf)
		//{
		//	double kof = GetLaLoKof(CenterPoint, Conf);

		//	var la = MPoint.Latitude * Math.Cos(angle) - MPoint.Longitude * Math.Sin(angle) + CenterPoint.Latitude;
		//	var lo = MPoint.Latitude * Math.Sin(angle) + MPoint.Longitude * Math.Cos(angle) + CenterPoint.Longitude;

		//	return new GPS2DCoordinat(la / kof, lo);
		//}
		///// <summary>
		///// получаем значение угла поворота области для направляющей к центру кривой
		///// </summary>
		///// <param name="angl">угол поворота в радианах</param>
		///// <param name="idPoint">точка относительно которой происходит поворот</param>
		///// <param name="rs">область</param>
		///// <param name="AnglDifferentSet">набор углов относительного поворота дороги</param>
		///// <param name="TangLineAzim">набор углов направляющих к центру кривой</param>
		//public static void GetRotateAngl(RoadSector rs, double[] AnglDifferentSet, double[] TangLineAzim,
		//	ref  double angl, ref int idPoint)
		//{
		//	if (rs.IdDAStart > rs.IdDAEnd || rs.IdDAStart < 0 || AnglDifferentSet.Length <= (rs.IdDAEnd - rs.IdDAStart)
		//		|| TangLineAzim.Length <= (rs.IdDAEnd - rs.IdDAStart))
		//		throw new ArgumentException("GetRotateAngl => кривые входные параметры");

		//	//найдем приблизительный центр круговой кривой
		//	double Sum = GetAnglDifferentSum(AnglDifferentSet, rs);

		//	double bS = 0;
		//	for (int i = rs.IdDAStart; i <= rs.IdDAEnd; i++)
		//	{
		//		bS += AnglDifferentSet[i];
		//		if (Math.Abs(bS) >= Math.Abs(Sum / 2.0))
		//		{
		//			angl = TangLineAzim[i - rs.IdDAStart];
		//			idPoint = i; // номер центра круговой, !!!в координатах AnglDifferentSet!!! для перехода к InputeRoadPoint => +1
		//			i = rs.IdDAEnd + 1;
		//		}
		//	}
		//}
		///// <summary>
		///// получение координат точки пересечения двух прямых
		///// </summary>
		///// <param name="Point1"></param>
		///// <param name="Point2"></param>
		///// <param name="Point3"></param>
		///// <param name="Point4"></param>
		///// <returns></returns>
		//public static GPS2DCoordinat GetPoint(GPS2DCoordinat Point1, GPS2DCoordinat Point2, GPS2DCoordinat Point3, GPS2DCoordinat Point4)
		//{
		//	double a = (Point2.Latitude - Point1.Latitude) / (Point2.Longitude - Point1.Longitude);
		//	double b = (Point4.Latitude - Point3.Latitude) / (Point4.Longitude - Point3.Longitude);
		//	double lo = (Point1.Longitude * a - Point3.Longitude * b + Point3.Latitude - Point1.Latitude) / (a - b);
		//	double la = (lo - Point1.Longitude) * (Point2.Latitude - Point1.Latitude) / (Point2.Longitude - Point1.Longitude) + Point1.Latitude;

		//	return new GPS2DCoordinat(la, lo);
		//}




		//public static double[] GetProfileAnglDifferentSet(StartEndAzimuthLen[] SEAL)
		//{
		//	double[] AnglDifferentSet = new double[SEAL.Length - 1];

		//	for (int i = 0; i < SEAL.Length - 1; i++)
		//		AnglDifferentSet[i] = SEAL[i + 1].StartAzimuth - SEAL[i].StartAzimuth;

		//	return AnglDifferentSet;
		//}

		//public static StartEndAzimuthLen[] GetProfileStartEndAzimuthLen(GPS3DCoordinat[] InputeRoadPoint, Config Conf)
		//{
		//	StartEndAzimuthLen[] StartEndAzimuthLen = new StartEndAzimuthLen[InputeRoadPoint.Length - 1];

		//	for (int i = 0; i < InputeRoadPoint.Length - 1; i++)
		//	{
		//		var inf = Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint[i], InputeRoadPoint[i + 1]);
		//		double y = InputeRoadPoint[i + 1].Height - InputeRoadPoint[i].Height;

		//		double buf = Math.Atan(y / inf.Length);
		//		StartEndAzimuthLen[i] = new StartEndAzimuthLen(buf, buf, inf.Length);
		//	}
		//	return StartEndAzimuthLen;
		//}
		///// <summary>
		///// Получает среднеарифметическую длинну радиуса на участке
		///// </summary>
		///// <param name="RoadPoints">набор всех точек</param>
		///// <param name="rs">участок</param>
		///// <param name="BasicMath">мат. функции</param>
		///// <returns></returns>
		//public static double CircularRange(GPS2DCoordinat[] RoadPoints, RoadSector rs, Config Conf)
		//{
		//	if (rs.CenterPoint == null || rs.CenterPoint.IsEmpty)
		//		throw new ArgumentNullException("CircularRange => непредвиденная ошибка");

		//	double sum = 0;
		//	// вычислим среднеарифметическую длинну радиуса круговой кривой
		//	for (int j = rs.IdDAStart; j <= rs.IdDAEnd; j++)
		//		sum += Conf.BasicMath.GetStartEndAzimuthLen(RoadPoints[j + 1], rs.CenterPoint.Point).Length;

		//	sum = sum / (rs.IdDAEnd - rs.IdDAStart + 1);

		//	return sum;
		//}
		///// <summary>
		///// выдает сведения о разности между максимальным и минимальным значение радиуса для данного кругового участка
		///// </summary>
		///// <param name="RoadPoints">входной набор</param>
		///// <param name="rs">сектор</param>
		///// <param name="BasicMath">мат. функции</param>
		///// <returns></returns>
		//public static double AreaAcurRange(GPS2DCoordinat[] RoadPoints, RoadSector rs, Config Conf)
		//{
		//	if (rs.CenterPoint.IsEmpty)
		//		throw new ArgumentNullException("AreaAcurRange => непредвиденная ошибка");

		//	double Max = double.MinValue;
		//	double Min = double.MaxValue;

		//	for (int j = rs.IdDAStart; j <= rs.IdDAEnd; j++)
		//	{
		//		var iLen = Conf.BasicMath.GetStartEndAzimuthLen(RoadPoints[j + 1], rs.CenterPoint.Point);
		//		Max = Math.Max(Max, iLen.Length);
		//		Min = Math.Min(Min, iLen.Length);
		//	}
		//	return Max - Min;
		//}
		///// <summary>
		///// выдает сведения о разности между максимальным и минимальным значение радиуса для данного кругового участка
		///// </summary>
		///// <param name="RoadPoints">входной набор</param>
		///// <param name="rs">сектор</param>
		///// <param name="BasicMath">мат. функции</param>
		///// <returns></returns>
		//public static double AreaAcurRange(GPS2DCoordinat[] RoadPoints, RoadSector rs, GPS2DCoordinat Point, Config Conf)
		//{
		//	if (rs.CenterPoint.IsEmpty)
		//		throw new ArgumentNullException("AreaAcurRange => непредвиденная ошибка");

		//	double Max = double.MinValue;
		//	double Min = double.MaxValue;

		//	for (int j = rs.IdDAStart; j <= rs.IdDAEnd; j++)
		//	{
		//		var iLen = Conf.BasicMath.GetStartEndAzimuthLen(RoadPoints[j + 1], Point);
		//		Max = Math.Max(Max, iLen.Length);
		//		Min = Math.Min(Min, iLen.Length);
		//	}
		//	return Max - Min;
		//}

		//public static void GetMinMaxAvgRadius(GPS2DCoordinat[] InputeRoadPoint, Config Conf, ref LinkedList<RoadSector> RoadGroopList)
		//{
		//	LinkedListNode<RoadSector> node = RoadGroopList.First;

		//	for (int i = 0; i < RoadGroopList.Count; i++)
		//	{
		//		if (node.Value.Type != (int)RoadSector.rType.LINE)
		//		{
		//			double Max = double.MinValue;
		//			double Min = double.MaxValue;
		//			double Sum = 0;
		//			int count = 0;

		//			if (node.Value.CenterPoint != null && 
		//				node.Value.CenterPoint.Point != null)
		//					for (int j = node.Value.IdDAStart; j <= node.Value.IdDAEnd; j++)
		//					{
		//						var inf = Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint[j + 1], node.Value.CenterPoint.Point);
		//						Max = Math.Max(Max, inf.Length);
		//						Min = Math.Min(Min, inf.Length);
		//						Sum += inf.Length;
		//						count++;
		//					}

		//			double avg = count > 0 ? Sum / count : 0;

		//			node.Value.MinMaxAvgRad = new double[] { Min, Max, avg };
		//		}
		//		node = node.Next;
		//	}
		//}

		//public static double[] GetMinMaxAvgRadius(GPS2DCoordinat[] InputeRoadPoint, Config Conf, RoadSector rs)
		//{
		//	if (rs.Type != (int)RoadSector.rType.LINE)
		//	{
		//		double Max = double.MinValue;
		//		double Min = double.MaxValue;
		//		double Sum = 0;
		//		int count = 0;

		//		if (rs.CenterPoint != null)
		//			if (rs.CenterPoint.Point != null)
		//				for (int j = rs.IdDAStart; j <= rs.IdDAEnd; j++)
		//				{
		//					var inf = Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint[j + 1], rs.CenterPoint.Point);
		//					Max = Math.Max(Max, inf.Length);
		//					Min = Math.Min(Min, inf.Length);
		//					Sum += inf.Length;
		//					count++;
		//				}

		//		double avg = count > 0 ? Sum / count : 0;

		//		return new double[] { Min, Max, avg };
		//	}
		//	return null;
		//}

		//public static void GetMinMaxAvgRadius(double[][] HL, ref LinkedList<RoadSector> RoadGroopList)
		//{
		//	LinkedListNode<RoadSector> node = RoadGroopList.First;

		//	while (node != null)
		//	{
		//		if (node.Value.Type != (int)RoadSector.rType.LINE)
		//		{
		//			double Max = double.MinValue;
		//			double Min = double.MaxValue;
		//			double Sum = double.MaxValue;
		//			int count = 0;

		//			if (node.Value.CenterPoint.Point != null)
		//				for (int i = node.Value.IdDAStart; i <= node.Value.IdDAEnd; i++)
		//				{
		//					double H = Math.Sqrt(
		//						Math.Pow(HL[i][0] - node.Value.CenterPoint.Point.Latitude, 2) +
		//						Math.Pow(HL[i][1] - node.Value.CenterPoint.Point.Longitude, 2));

		//					Max = Math.Max(Max, H);
		//					Min = Math.Min(Min, H);

		//					Sum += H;
		//					count++;
		//				}

		//			double avg = count > 0 ? Sum / count : 0;

		//			node.Value.MinMaxAvgRad = new double[] { Min, Max, avg };
		//		}
		//		node = node.Next;
		//	}
		//}

		///// <summary>
		///// Возвращает суммарный угол отклонения для участка
		///// </summary>
		///// <param name="AnglDifferentSet"></param>
		///// <param name="rs"></param>
		///// <returns></returns>
		//public static double GetAnglDifferentSum(double[] AnglDifferentSet, RoadSector rs)
		//{
		//	double angl = 0;

		//	for (int i = rs.IdDAStart; i <= rs.IdDAEnd; i++)            
		//		angl += AnglDifferentSet[i];            

		//	return angl;
		//}
		///// <summary>
		///// Возвращает суммарный угол отклонения для участка
		///// </summary>
		///// <param name="AnglDifferentSet"></param>
		///// <param name="startDA"></param>
		///// <param name="endDA"></param>
		///// <returns></returns>
		//public static double GetAnglDifferentSum(double[] AnglDifferentSet, int startDA, int endDA)
		//{
		//	double angl = 0;
		//	for (int i = startDA; i <= endDA; i++)
		//		angl += AnglDifferentSet[i];
		//	return angl;
		//}

		//public static double[] GetPoint(double[] p1, double[] p2, double[] p3, double[] p4)
		//{
		//	double a = (p2[0] - p1[0]) / (p2[1] - p1[1]);
		//	double b = (p4[0] - p3[0]) / (p4[1] - p3[1]);
		//	double Y = (p1[1] * a - p3[1] * b + p3[0] - p1[0]) / (a - b);
		//	double X = (Y - p1[1]) * (p2[0] - p1[0]) / (p2[1] - p1[1]) + p1[0];
		//	return new double[] { X, Y };
		//}

		///// <summary>
		///// плучает коэффициент соответствия длинны по широте и долготе (широта / долгота)
		///// </summary>
		///// <param name="Point"></param>
		///// <param name="Conf"></param>
		///// <returns></returns>
		//private static double GetLaLoKof(GPS2DCoordinat Point, Config Conf)
		//{
		//	var pt1 = new GPS2DCoordinat(Point.Latitude + 0.0001, Point.Longitude);
		//	var pt2 = new GPS2DCoordinat(Point.Latitude, Point.Longitude + 0.0001);
		//	var inf1 = Conf.BasicMath.GetStartEndAzimuthLen(pt1, Point);
		//	var inf2 = Conf.BasicMath.GetStartEndAzimuthLen(pt2, Point);
		//	return inf1.Length / inf2.Length;
		//}

		///// <summary>
		///// Отношение растояний (La / Lo)
		///// </summary>
		///// <param name="point"></param>
		///// <param name="Conf"></param>
		///// <returns></returns>
		//public static double GetHWKof(GPS2DCoordinat point, Config Conf)
		//{
		//	double bH = 0.01;
		//	var bPt2 = new GPS2DCoordinat(point.Latitude + bH, point.Longitude);
		//	var bPt3 = new GPS2DCoordinat(point.Latitude, point.Longitude + bH);

		//	var bInf1 = Conf.BasicMath.GetStartEndAzimuthLen(point, bPt2);
		//	var bInf2 = Conf.BasicMath.GetStartEndAzimuthLen(point, bPt3);

		//	return bInf1.Length / bInf2.Length;
		//}
	}
}