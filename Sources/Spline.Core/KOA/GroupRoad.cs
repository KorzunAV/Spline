//using System;
//using System.Collections.Generic;
//using BdcApp.RoadAnalysis.dll.CommonClass;
//using BdcApp.RoadAnalysis.dll.GeoMath;
//using BdcApp.RoadAnalysis.dll.Settings;
//using BdcApp.RoadAnalysis.dll.Spline;

//#if (SILVERLIGHT || NET_4_0)
//using System.Threading.Tasks;
//#else
//using System.Threading;
//#endif

//namespace BdcApp.RoadAnalysis.dll.Filtrs
//{
//	/// <summary>
//	/// первая группировка сплайном
//	/// </summary>
//	public static class GroupRoad
//	{
//#if !(SILVERLIGHT || NET_4_0)
//		static object workerLocker = new object();
//		static int runningWorkers = 0;
//#endif

//		private static void SetKof(double[] AnglDifferentSet, ref Config Conf)
//		{
//			double[] ptkof = getExtrSet(AnglDifferentSet, Conf); // получили набор экстремумов
//			SetKofPlan(ptkof, ref Conf);
//		}

//		internal static LinkedList<RoadSector> FirstGroupRoadPlan(GPS2DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, ref Config Conf)
//		{
//			// генерируем усредненный набор рабочих данных для первичной обработки
//			if (Conf.GroupAuto)
//				SetKof(AnglDifferentSet, ref Conf);
//			var BfCleanAngls = KSpline.CleanNoise(AnglDifferentSet, Conf);

//			var CleanAngls = KSpline.ToDoubleArray(BfCleanAngls);
//			BfCleanAngls = KSpline.KLineSpline(BfCleanAngls, Conf.GroupSplainKof); // слаживаем углы
//			var CleanAndSplineAngls = KSpline.ToDoubleArray(BfCleanAngls);

//			// проводим комбинированный поиск прямых 
//			var lineset = GetLineGroupList(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf);
//			var lineGroup = LineSetToGroup(InputeRoadPoint, lineset);

//			// уточняем наборы рабочих данных
//			BfCleanAngls = MergeKAngle(BfCleanAngls, AnglDifferentSet, lineset);
//			CleanAngls = KSpline.ToDoubleArray(BfCleanAngls);
//			BfCleanAngls = KSpline.KLineSpline(BfCleanAngls, Conf.GroupSplainKof); // слаживаем углы
//			CleanAndSplineAngls = KSpline.ToDoubleArray(BfCleanAngls);

//			// группируем дорогу
//			// очень чувствительный метод
//			//var RoadGroupList = ClipCrclrGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, lineGroup);            
//			// мало чувствительный метод
//			var RoadGroupList = ExtremumGroupFinder(CleanAndSplineAngls, Conf);

//			// определяем типы участков
//			DefineGroups(CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList, true);

//			// допилы
//			СonnectSmall(AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList);
//			int Oldcount = RoadGroupList.Count;

//			do
//			{
//				Oldcount = RoadGroupList.Count;
//				FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);
//				CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, ref RoadGroupList);
//				DefineLineByRad(Conf, ref RoadGroupList);
//				RoadGroupList = BetweenLine(AnglDifferentSet, Conf, RoadGroupList);
//				RoadGroupList = MergeCircular(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, RoadGroupList);
//				СonnectGroop(AnglDifferentSet, ref RoadGroupList);
//			} while (Oldcount != RoadGroupList.Count);

//			RoadGroupList = Else(InputeRoadPoint,StartEndAzimuthLen,AnglDifferentSet, Conf, RoadGroupList);

//			return RoadGroupList;
//		}

//		internal static LinkedList<RoadSector> FirstGroupRoadProfile(double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, ref Config Conf)
//		{
//			if (Conf.GroupAuto)
//				SetKof(AnglDifferentSet, ref Conf);
//			var BfCleanAngls = KSpline.CleanNoise(AnglDifferentSet, Conf);

//			var CleanAngls = KSpline.ToDoubleArray(BfCleanAngls);
//			BfCleanAngls = KSpline.KLineSpline(BfCleanAngls, Conf.GroupSplainKof); // слаживаем углы
//			var CleanAndSplineAngls = KSpline.ToDoubleArray(BfCleanAngls);

//			var RoadGroupList = ExtremumGroupFinder(CleanAndSplineAngls, Conf); // ишем группы на основании чишенных и сглаженных данных

//			DefineGroups(CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList);

//			return RoadGroupList;
//		}

//		internal static void ScndGroupRoad(GPS2DCoordinat[] RoadPoints, StartEndAzimuthLen[] StartEndAzimuthLen, double[] AnglDifferentSet, Config Conf,
//			ref LinkedList<RoadSector> RoadGroupList)
//		{
//			FindTransitiveCurvesByCrossAreaFiltering.Seach(RoadPoints, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);

//			FindCenters.GetMXLaLo(RoadPoints, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);

//			Filtrs.CorrectCenter.Correct(RoadPoints, Conf, ref RoadGroupList);

//			СonnectGroop(AnglDifferentSet, ref RoadGroupList, true);

//			СonnectSmall(AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList);

//			CommonFunctions.GetMinMaxAvgRadius(RoadPoints, Conf, ref RoadGroupList);
//		}

//		#region поиск экстремумов
//		private static double[] getExtrSet(double[] AnglDifferentSet, Config Conf)
//		{
//			double[][] ptkof = new double[AnglDifferentSet.Length][];
//			for (int i = 0; i < AnglDifferentSet.Length; i++)
//				ptkof[i] = new double[] { i, AnglDifferentSet[i] };
//			BSpline.SplinePoints(Conf.GroupSplainKof, ref ptkof);

//			double[] Splained = new double[ptkof.Length];
//			for (int i = 0; i < ptkof.Length; i++)
//			{
//				Splained[i] = ptkof[i][1];
//			}


//			List<double> extrm = new List<double>();
//			extrm.Add(Splained[0]);
//			int id = 0;

//			while (id < Splained.Length - 1)
//			{
//				id = ExtremumFinder(Splained, id);
//				extrm.Add(Splained[id]);
//			}

//			return extrm.ToArray();
//		}

//		private static void SetKofPlan(double[] AnglDifferentSet, ref Config Conf)
//		{
//			double min = double.MaxValue;
//			double max = double.MinValue;

//			for (int i = 0; i < AnglDifferentSet.Length; i++)
//			{
//				min = AnglDifferentSet[i] != 0 ? Math.Min(min, Math.Abs(AnglDifferentSet[i])) : min;
//				max = Math.Max(max, Math.Abs(AnglDifferentSet[i]));
//			}

//			int count = (int)(max / min);


//			int znum = 0;
//			while (min < 1)
//			{
//				min *= 10;
//				znum++;
//			}
//			bool done = false;

//			while (true)
//			{
//				Dictionary<int, int> set = new Dictionary<int, int>();

//				for (int i = 0; i < AnglDifferentSet.Length; i++)
//				{
//					double id = Math.Abs(AnglDifferentSet[i]) * Math.Pow(10, znum);
//					if (set.ContainsKey((int)id))
//					{
//						set[(int)id]++;
//						max = Math.Max(max, set[(int)id]);
//					}
//					else
//						set.Add((int)id, 1);
//				}

//				if (max < AnglDifferentSet.Length * 0.1)
//				{
//					znum--;
//				}
//				else
//				{
//					int cidm = AnglDifferentSet.Length;
//					foreach (KeyValuePair<int, int> kvp in set)
//					{
//						if (kvp.Value < AnglDifferentSet.Length * 0.1 && kvp.Value > Math.Ceiling(AnglDifferentSet.Length * 0.03))
//						{
//							cidm = Math.Min(cidm, kvp.Key);
//						}
//					}
//					if (done)
//					{
//						Conf.GroupStrongKof = cidm * Math.Pow(10, -1.0 * znum);
//						Conf.GroupSplainCropKof = Conf.GroupStrongKof / 10;
//						return;
//					}
//					if (cidm == AnglDifferentSet.Length)
//					{
//						znum++;
//						done = true;
//					}
//					else
//					{
//						znum--;
//					}
//				}
//			}
//		}

//		/// <summary>
//		/// поиск участков на основании экстремумов
//		/// </summary>
//		/// <param name="SmoothAnglDifferentSet">набор обработанных углов</param>
//		/// <returns></returns>
//		private static LinkedList<RoadSector> ExtremumGroupFinder(double[] SmoothAnglDifferentSet, Config Conf)
//		{
//			LinkedList<RoadSector> OutGroup = new LinkedList<RoadSector>();
//			int ExtrStart = 0;
//			int ExtrEnd = 0;
//			int GroupStart = 0;
//			int GroupEnd = 0;
//			double minAmpltd = double.MaxValue;
//			double maxAmpltd = 0;

//			while (ExtrStart < SmoothAnglDifferentSet.Length - 1)
//			{
//				ExtrEnd = ExtremumFinder(SmoothAnglDifferentSet, ExtrStart);
//				GroupEnd = GetCentr(SmoothAnglDifferentSet, ExtrStart, ExtrEnd, Conf, ref maxAmpltd, ref minAmpltd);
//				if ((GroupEnd > -1) && (ExtrEnd < SmoothAnglDifferentSet.Length - 1))
//				{
//					RoadSector rs = new RoadSector(GroupStart, GroupEnd, RoadSector.rType.UNKNOWN);
//					LinkedListNode<RoadSector> node = new LinkedListNode<RoadSector>(rs);
//					OutGroup.AddLast(node);
//					GroupStart = GroupEnd + 1;
//					minAmpltd = double.MaxValue;
//					maxAmpltd = 0;
//				}

//				if (ExtrEnd >= SmoothAnglDifferentSet.Length - 1) // если последний
//				{
//					RoadSector rs = new RoadSector(GroupStart, ExtrEnd, RoadSector.rType.UNKNOWN);
//					LinkedListNode<RoadSector> node = new LinkedListNode<RoadSector>(rs);
//					OutGroup.AddLast(node);
//					minAmpltd = double.MaxValue;
//					maxAmpltd = 0;
//				}
//				else
//				{
//					if (ExtrEnd >= SmoothAnglDifferentSet.Length - 2) // если последний
//					{
//						RoadSector rs = new RoadSector(ExtrEnd + 1, ExtrEnd + 1, RoadSector.rType.UNKNOWN);
//						LinkedListNode<RoadSector> node = new LinkedListNode<RoadSector>(rs);
//						OutGroup.AddLast(node);
//						minAmpltd = double.MaxValue;
//						maxAmpltd = 0;
//					}
//				}
//				ExtrStart = ExtrEnd + 1;
//			}
//			return OutGroup;
//		}

//		/// <summary>
//		/// последовательный поиск экстремумов
//		/// </summary>
//		/// <param name="SmoothAnglDifferentSet">набор обработанных углов</param>
//		/// <param name="startId">стартовый индекс</param>
//		/// <returns></returns>
//		private static int ExtremumFinder(double[] SmoothAnglDifferentSet, int startId)
//		{
//			if (startId < SmoothAnglDifferentSet.Length)
//			{
//				//смотрим стартовый знак прирашения
//				int sighOld = Math.Sign(SmoothAnglDifferentSet[startId] - SmoothAnglDifferentSet[startId + 1]);

//				for (int i = startId + 1; i < SmoothAnglDifferentSet.Length - 1; i++)
//				{
//					//смотрим шаговый знак прирашения
//					int sighCur = Math.Sign(SmoothAnglDifferentSet[i] - SmoothAnglDifferentSet[i + 1]);
//					sighCur = SmoothAnglDifferentSet[i + 1] == 0 ? 0 : sighCur;

//					// если изменился - значит началась другая область
//					if (sighCur != sighOld)
//					{
//						return i; // это и будет локальный экстремум
//					}
//				}
//			}
//			return SmoothAnglDifferentSet.Length - 1;
//		}

//		/// <summary>
//		/// поиск середины области по амплитуде
//		/// </summary>
//		/// <param name="SmoothAnglDifferentSet">набор обработанных углов</param>
//		/// <param name="startId">начало области</param>
//		/// <param name="endId">конец области</param>
//		/// <returns></returns>
//		private static int GetCentr(double[] SmoothAnglDifferentSet, int startId, int endId, Config Conf, ref double maxAmpltd, ref double minAmpltd)
//		{
//			// т.к. startId и endId это точки локального минимуму и максиму, то амплитуда находится так:
//			double Amplitude = Math.Abs(SmoothAnglDifferentSet[startId] - SmoothAnglDifferentSet[endId]);
//			if (Math.Abs(maxAmpltd) < Math.Abs(SmoothAnglDifferentSet[startId]))
//				maxAmpltd = SmoothAnglDifferentSet[startId];
//			if (Math.Abs(minAmpltd) > Math.Abs(SmoothAnglDifferentSet[startId]) && SmoothAnglDifferentSet[startId] != 0)
//				minAmpltd = SmoothAnglDifferentSet[startId];

//			if (Amplitude == 0)
//				return endId; // случай для зануленных данных. Данный участок не разбивается, а идет целиком.

//			if (startId > 0 ? SmoothAnglDifferentSet[startId - 1] == 0 ? true : false : false)
//				return -1; // если предыдуший участок - зануленный, то является частью более длинной кривой

//			if (endId < SmoothAnglDifferentSet.Length - 1 ? SmoothAnglDifferentSet[endId + 1] == 0 ? true : false : false)
//				return endId; // если следующий участок - зануленный, то текущий завершается в этой точке


//			double MaxPrcnt = Math.Abs(SmoothAnglDifferentSet[endId] / maxAmpltd) > 1 ? Math.Abs(SmoothAnglDifferentSet[endId] / maxAmpltd) : Math.Abs(maxAmpltd / SmoothAnglDifferentSet[endId]);
//			double MinPrcnt = Math.Abs(SmoothAnglDifferentSet[endId] / minAmpltd) > 1 ? Math.Abs(SmoothAnglDifferentSet[endId] / minAmpltd) : Math.Abs(minAmpltd / SmoothAnglDifferentSet[endId]);

//			if ((MaxPrcnt < 1.0 / Conf.SubPrcnt) &&
//				(MinPrcnt < 1.0 / Conf.SubPrcnt) &&
//				Math.Sign(SmoothAnglDifferentSet[startId]) == Math.Sign(SmoothAnglDifferentSet[endId]))
//				return -1; // не прошел по коф. -> является частью более длинной кривой

//			// поиск точки. кот. соответствует середине амплитуды
//			for (int i = startId; i <= endId; i++)
//			{
//				double count = Math.Abs(SmoothAnglDifferentSet[startId] - SmoothAnglDifferentSet[i]);
//				if (count > Amplitude / 2.0)
//					return i;
//			}
//			return endId;
//		}
//		#endregion

//		#region поиск участков по слоям (в качестве ключевого параметра радиус)
//		internal static LinkedList<RoadSector> GetGroup(GPS2DCoordinat[] InputeRoadPoint, StartEndAzimuthLen[] StartEndAzimuthLen,
//				  double[] AnglDifferentSet, double[] CleanAngls, double[] CleanAndSplineAngls, Config Conf)
//		{
//			var lineset = GetLineGroupList(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf);
//			var group = LineSetToGroup(InputeRoadPoint, lineset);

//			group = ClipCrclrGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, group);

//			return group;
//		}

//		private static List<int[]> GetLineGroupList(GPS2DCoordinat[] InputeRoadPoint, StartEndAzimuthLen[] StartEndAzimuthLen,
//		   double[] AnglDifferentSet, double[] CleanAngls, double[] CleanAndSplineAngls, Config Conf)
//		{
//			var lineset = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad);
//			var lineset2 = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad / 2.0);
//			lineset = MergeSet(lineset, lineset2);
//			var lineset3 = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad / 4.0);
//			lineset = MergeSet(lineset, lineset3);
//			var lineset4 = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad / 8.0);
//			lineset = MergeSet(lineset, lineset4);
//			var lineset5 = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad / 16.0);
//			lineset = MergeSet(lineset, lineset5);
//			var lineset6 = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad / 32.0);
//			lineset = MergeSet(lineset, lineset6);
//			//var lineset7 = GetLineGroup(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, CleanAngls, CleanAndSplineAngls, Conf, Conf.MaxRad / 64.0);
//			//lineset = MergeSet(lineset, lineset7);
//			return lineset;
//		}

//		private static List<int[]> GetLineGroup(GPS2DCoordinat[] RoadPoints, StartEndAzimuthLen[] StartEndAzimuthLen,
//		double[] AnglDifferentSet, double[] CleanAngls, double[] CleanAndSplineAngls, Config Conf, double LenDeflt)
//		{
//			var idset = GetCorrectGropId(RoadPoints, StartEndAzimuthLen, Conf, LenDeflt);
//			LinkedList<RoadSector> group = IntSetToGroups(idset, RoadPoints);

//#if (SILVERLIGHT || NET_4_0)
//			Parallel.ForEach(group, node =>
//			{
//				if (node.IdDAEnd - node.IdDAStart > Conf.GroupConnKof)
//				{
//					node.Type = DefineGroups(node, CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf);
//					if (node.Type != (int)RoadSector.rType.LINE)
//					{
//						node = FindCenters.GetMXLaLo(RoadPoints, StartEndAzimuthLen, AnglDifferentSet, Conf, node);
//						node.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(RoadPoints, Conf, node);
//						node.Type = DefineLineByRad(Conf, node);
//					}
//				}
//			});
//#else
//			LinkedListNode<RoadSector> node = group.First;

//			while (node != null)
//			{
//				if (node.Value.IdDAEnd - node.Value.IdDAStart > Conf.GroupConnKof)
//				{
//					lock (workerLocker)
//						runningWorkers++;

//					LinkedListNode<RoadSector> tNode = node;
//					ThreadPool.QueueUserWorkItem(delegate(object notUsed)
//					{
//						tNode.Value.Type = DefineGroups(tNode.Value, CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf);
//						if (tNode.Value.Type != (int)RoadSector.rType.LINE)
//						{
//							tNode.Value = FindCenters.GetMXLaLo(RoadPoints, StartEndAzimuthLen, AnglDifferentSet, Conf, tNode.Value);
//							tNode.Value.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(RoadPoints, Conf, tNode.Value);
//							tNode.Value.Type = DefineLineByRad(Conf, tNode.Value);
//						}
//						lock (workerLocker)
//						{
//							runningWorkers--;
//							Monitor.Pulse(workerLocker);
//						}
//					});
//				}

//				node = node.Next;
//			}

//			lock (workerLocker)
//				while (runningWorkers > 0)
//					Monitor.Wait(workerLocker);
//#endif

//			List<int[]> LineSet = new List<int[]>();
//			var bnode = group.First;
//			while (bnode != null)
//			{
//				if (bnode.Value.Type == (int)RoadSector.rType.LINE)
//				{
//					LineSet.Add(new int[] { bnode.Value.IdDAStart, bnode.Value.IdDAEnd });
//				}
//				bnode = bnode.Next;
//			}
//			return LineSet;
//		}

//		private static LinkedList<RoadSector> ClipCrclrGroup(GPS2DCoordinat[] RoadPoints, StartEndAzimuthLen[] StartEndAzimuthLen,
//			double[] AnglDifferentSet, Config Conf, LinkedList<RoadSector> lineGroup)
//		{
//			var node = lineGroup.First;

//			while (node != null)
//			{
//				if (node.Value.Type != (int)RoadSector.rType.LINE)
//				{
//					node.Value = FindCenters.GetMXLaLo(RoadPoints, StartEndAzimuthLen, AnglDifferentSet, Conf, node.Value);
//					node.Value.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(RoadPoints, Conf, node.Value);

//					if (node.Value.MinMaxAvgRad != null)
//					{
//						var idset = GetCorrectGropId(RoadPoints, StartEndAzimuthLen, Conf, node.Value.MinMaxAvgRad[1] * 1.2);

//						for (int i = 0; i < idset.Count; i++)
//						{
//							if (node.Value.IdDAStart < idset[i] && idset[i] < node.Value.IdDAEnd)
//							{
//								RoadSector rs = new RoadSector(node.Value.IdDAStart, idset[i], RoadSector.rType.UNKNOWN);
//								lineGroup.AddBefore(node, rs);
//								node.Value.IdDAStart = idset[i] + 1;
//							}
//							if (idset[i] > node.Value.IdDAEnd)
//								i = idset.Count;
//						}
//					}
//				}
//				node = node.Next;
//			}
//			return lineGroup;
//		}

//		public static List<int> GetCorrectGropId(GPS2DCoordinat[] RoadPoints, StartEndAzimuthLen[] StartEndAzimuthLen, Config Conf, double LenDeflt = 3000)
//		{
//			GPS2DCoordinat[] slp1 = new GPS2DCoordinat[RoadPoints.Length];
//			GPS2DCoordinat[] slp2 = new GPS2DCoordinat[RoadPoints.Length];
//			double HWKof = CommonFunctions.GetHWKof(RoadPoints[0], Conf);

//			double La = 0;
//			double Lo = 0;

//			for (int i = 0; i < RoadPoints.Length; i++)
//			{
//				LatitudeLongitudeAzimuthe pt1;
//				LatitudeLongitudeAzimuthe pt2;
//				if (StartEndAzimuthLen.Length > i)
//				{
//					pt1 = Conf.BasicMath.GetLatitudeLongitudeAzimuthe(RoadPoints[i], Math.PI / 2.0 + StartEndAzimuthLen[i].StartAzimuth, LenDeflt);
//					pt2 = Conf.BasicMath.GetLatitudeLongitudeAzimuthe(RoadPoints[i], StartEndAzimuthLen[i].StartAzimuth - Math.PI / 2.0, LenDeflt);
//				}
//				else
//				{
//					pt1 = Conf.BasicMath.GetLatitudeLongitudeAzimuthe(RoadPoints[i], Math.PI / 2.0 + StartEndAzimuthLen[i - 1].EndAzimuth, LenDeflt);
//					pt2 = Conf.BasicMath.GetLatitudeLongitudeAzimuthe(RoadPoints[i], StartEndAzimuthLen[i - 1].EndAzimuth - Math.PI / 2.0, LenDeflt);
//				}

//				double la1 = Math.Abs(La - pt1.Point.Latitude);
//				double lo1 = Math.Abs(Lo - pt1.Point.Longitude);
//				double la2 = Math.Abs(La - pt2.Point.Latitude);
//				double lo2 = Math.Abs(Lo - pt2.Point.Longitude);

//				if (la1 / HWKof + lo1 < la2 / HWKof + lo2)
//				{

//					slp1[i] = (GPS2DCoordinat)pt1.Point.Clone();
//					slp2[i] = (GPS2DCoordinat)pt2.Point.Clone();
//					La = pt1.Point.Latitude;
//					Lo = pt1.Point.Longitude;
//				}
//				else
//				{
//					slp1[i] = (GPS2DCoordinat)pt2.Point.Clone();
//					slp2[i] = (GPS2DCoordinat)pt1.Point.Clone();
//					La = pt2.Point.Latitude;
//					Lo = pt2.Point.Longitude;
//				}
//			}

//			return GetDirectionChangeId(slp1, slp2, Conf);
//		}

//		private static List<int> GetDirectionChangeId(GPS2DCoordinat[] slp1, GPS2DCoordinat[] slp2, Config Conf)
//		{
//			var ads1 = CommonFunctions.GetAnglDifferent(slp1, Conf);
//			var ads2 = CommonFunctions.GetAnglDifferent(slp2, Conf);

//			List<int> idset1 = new List<int>();

//			idset1.Add(0);
//			for (int i = 0; i < ads1.Length; i++)
//			{
//				var angl = BasicConvert.RadToGrad(ads1[i]);
//				if (Math.Abs(ads1[i]) > Math.PI / 2.0)
//					idset1.Add(i);
//			}

//			for (int i = 0; i < ads2.Length; i++)
//			{
//				var angl = BasicConvert.RadToGrad(ads2[i]);
//				if (Math.Abs(ads2[i]) > Math.PI / 2.0)
//					idset1.Add(i);
//			}
//			idset1.Add(ads1.Length - 1);

//			idset1.Sort();

//			for (int i = 0; i < idset1.Count - 1; i++)
//			{
//				if (idset1[i] == idset1[i + 1])
//				{
//					idset1.RemoveAt(i + 1);
//					i--;
//				}
//			}

//			return idset1;
//		}

//		private static List<int[]> MergeSet(List<int[]> lineset1, List<int[]> lineset2)
//		{
//			if (lineset1.Count == 0)
//				return lineset2;
//			if (lineset2.Count == 0)
//				return lineset1;

//			lineset1.AddRange(lineset2);

//			for (int i = 0; i < lineset1.Count - 1; i++)
//			{
//				for (int j = i + 1; j < lineset1.Count; j++)
//				{
//					if (lineset1[i][0] > lineset1[j][0])
//					{
//						var buf = lineset1[i];
//						lineset1[i] = lineset1[j];
//						lineset1[j] = buf;
//					}
//					if (lineset1[i][0] == lineset1[j][0] && lineset1[i][1] == lineset1[j][1])
//					{
//						lineset1.RemoveAt(j);
//						j--;
//					}
//				}
//			}

//			for (int i = 0; i < lineset1.Count - 1; i++)
//			{
//				if (lineset1[i][0] > 9886 && lineset1[i][0] < 10299)
//				{
//					var pt1 = lineset1[i];
//					var pt2 = lineset1[i + 1];
//				}

//				if (lineset1[i][0] == lineset1[i + 1][0])
//				{
//					lineset1[i][1] = Math.Max(lineset1[i][1], lineset1[i + 1][1]);
//					lineset1.RemoveAt(i + 1);
//					i--;
//				}
//				else
//				{
//					if (lineset1[i][1] >= lineset1[i + 1][0])
//					{
//						lineset1[i][1] = Math.Max(lineset1[i][1], lineset1[i + 1][1]);
//						lineset1.RemoveAt(i + 1);
//						i--;
//					}
//				}
//			}

//			return lineset1;
//		}

//		private static LinkedList<RoadSector> MergeCrclr(GPS2DCoordinat[] InputeRoadPoint, StartEndAzimuthLen[] StartEndAzimuthLen,
//			double[] AnglDifferentSet, Config Conf, LinkedList<RoadSector> RoadGroup)
//		{
//			var node = RoadGroup.First;
//			bool isMerge = false;

//			while (node != null)
//			{
//				if ((node.Value.Type == (int)RoadSector.rType.CIRCULAR || node.Value.Type == (int)RoadSector.rType.UNKNOWN) &&
//				(node.Next != null && (node.Next.Value.Type == (int)RoadSector.rType.CIRCULAR || node.Next.Value.Type == (int)RoadSector.rType.UNKNOWN)))
//				{
//					var s1 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Value);
//					var s2 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Next.Value);

//					if (Math.Sign(s1) == Math.Sign(s2))
//					{
//						double len = Conf.BasicMath.GetStartEndAzimuthLen(node.Value.CenterPoint.Point, node.Next.Value.CenterPoint.Point).Length;

//						if (len < 500)
//						{
//							isMerge = true;
//						}
//						else
//						{
//							RoadSector rs = new RoadSector(node.Value.IdDAStart, node.Next.Value.IdDAEnd, (int)RoadSector.rType.UNKNOWN);
//							rs = FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, rs);
//							rs.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, rs);

//							if (node.Value.MinMaxAvgRad[2] < 500)
//							{
//								isMerge = true;
//							}
//							else
//							{
//								if ((node.Value.IdDAEnd - node.Value.IdDAStart) < Conf.GroupConnKof &&
//							   (node.Next.Value.IdDAEnd - node.Next.Value.IdDAStart) < Conf.GroupConnKof)
//									isMerge = true;
//							}
//						}
//						if (isMerge)
//						{
//							node.Value.IdDAEnd = node.Next.Value.IdDAEnd;
//							RoadGroup.Remove(node.Next);
//							node.Value = FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, node.Value);
//							node.Value.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, node.Value);

//							while (node.Previous != null && (node.Value.Type == (int)RoadSector.rType.CIRCULAR || node.Value.Type == (int)RoadSector.rType.UNKNOWN))
//								node = node.Previous;
//						}
//					}
//				}
//				if (!isMerge)
//					node = node.Next;
//				isMerge = false;
//			}
//			return RoadGroup;
//		}

//		private static LinkedList<RoadSector> LineSetToGroup(GPS2DCoordinat[] RoadPoints, List<int[]> lineset)
//		{
//			LinkedList<RoadSector> group = new LinkedList<RoadSector>();

//			int sId = 0;
//			int index = 0;
//			while (sId < RoadPoints.Length - 2)
//			{
//				if (index < lineset.Count)
//				{
//					if (sId < lineset[index][0])
//					{
//						RoadSector rs = new RoadSector(sId, lineset[index][0] - 1, RoadSector.rType.UNKNOWN);
//						group.AddLast(rs);
//						sId = lineset[index][0];
//					}
//					else
//					{
//						RoadSector rs = new RoadSector(lineset[index][0], lineset[index][1], RoadSector.rType.LINE);
//						group.AddLast(rs);
//						sId = lineset[index][1] + 1;
//						index++;
//					}
//				}
//				else
//				{
//					RoadSector rs = new RoadSector(sId, RoadPoints.Length - 3, RoadSector.rType.UNKNOWN);
//					group.AddLast(rs);
//					sId = RoadPoints.Length - 2;
//				}
//			}
//			return group;
//		}

//		private static LinkedList<RoadSector> IntSetToGroups(List<int> idset, GPS2DCoordinat[] RoadPoints)
//		{
//			int sId = 0;
//			int eId = 0;
//			LinkedList<RoadSector> group = new LinkedList<RoadSector>();
//			for (int i = 0; i < idset.Count; i++)
//			{
//				RoadSector rs = new RoadSector(sId, eId, RoadSector.rType.UNKNOWN);
//				group.AddLast(rs);

//				sId = eId + 1;
//				eId = sId > idset[i] - 1 ? idset[i] : idset[i] - 1;

//				if (sId <= eId)
//				{
//					RoadSector rs2 = new RoadSector(sId, eId, RoadSector.rType.UNKNOWN);
//					group.AddLast(rs2);
//				}

//				sId = eId + 1;
//				eId = sId;
//			}
//			// особый случай для последнего элемента
//			RoadSector rse1 = new RoadSector(sId, eId, RoadSector.rType.UNKNOWN);
//			group.AddLast(rse1);
//			sId = eId + 1;
//			if (sId < RoadPoints.Length - 2)
//			{
//				RoadSector rse3 = new RoadSector(sId, sId, RoadSector.rType.UNKNOWN);
//				group.AddLast(rse3);
//			}
//			return group;
//		}
//		#endregion

//		#region DefineGroups
//		/// <summary>
//		/// определение типов участков
//		/// </summary>
//		/// <param name="CleanAngls">набор чищеных углов</param>
//		/// <param name="CleanAndSplineAngls">набор чищенных и сглаженых углов</param>
//		/// <param name="StartEndAzimuthLen">набор длин и азимутов</param>
//		/// <param name="RoadGroupList">список участков</param>
//		private static void DefineGroups(double[] CleanAngls, double[] CleanAndSplineAngls, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, Config Conf,
//			ref LinkedList<RoadSector> RoadGroupList, bool isNullEnable = false)
//		{
//			LinkedListNode<RoadSector> node = RoadGroupList.First;
//			while (node != null)
//			{
//				if (node.Value.IdDAEnd - node.Value.IdDAStart > Conf.GroupConnKof)
//				{
//					//node.Value.Type = DefineGroups(node.Value, AnglDifferentSet, StartEndAzimuthLen, Conf);
//					if (node.Value.Type == (int)RoadSector.rType.UNKNOWN)
//						node.Value.Type = DefineGroups(node.Value, CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf, isNullEnable);
//				}
//				node = node.Next;
//			}
//		}

//		public static int DefineGroups(RoadSector rs, double[] CleanAngls, double[] CleanAndSplineAngls, double[] AnglDifferentSet,
//			StartEndAzimuthLen[] StartEndAzimuthLen, Config Conf, bool isNullEnable = false)
//		{
//			double Sh1 = 0; double Sh4 = 0;
//			double Sh2 = 0; double Sh5 = 0;
//			double Sh3 = 0; double Sh6 = 0;

//			var set = new List<double>();

//			// считаем длинну участка, угол поворота, и количество перемен знака
//			for (int i = rs.IdDAStart; i <= rs.IdDAEnd; i++)
//			{
//				Sh1 += StartEndAzimuthLen[i + 1].Length * Math.Tan(CleanAngls[i]);
//				Sh2 += StartEndAzimuthLen[i + 1].Length * Math.Tan(CleanAndSplineAngls[i]);
//				Sh3 += StartEndAzimuthLen[i + 1].Length * Math.Tan(AnglDifferentSet[i]);

//				Sh4 += CleanAngls[i];
//				Sh5 += CleanAndSplineAngls[i];
//				Sh6 += AnglDifferentSet[i];
//				set.Add(AnglDifferentSet[i]);
//			}

//			//  var avg = EmpiricalDistributionFunction.GetAVG(set);

//			#region 1 фильтр

//			// если отклонения больше граничных +- 30см
//			if (Math.Abs(Sh1) > Conf.LineDirtCount * 2 && Math.Abs(Sh2) > Conf.LineDirtCount * 2 && Math.Abs(Sh3) > Conf.LineDirtCount * 2)
//				// если прямая линия зацепила точек с круговой, то Sh1,2,3 будут болеше граничных.
//				// Проверим что участок действительно круговая - если сплайн Sh2 меньше чем Sh1 и Sh3, 
//				// то у нас круговая т.к. сплайн скругляет прямые и выпрямляет кривые линии.
//				if (Math.Abs(Sh1) > Math.Abs(Sh2) && Math.Abs(Sh3) > Math.Abs(Sh2))
//					return (int)RoadSector.rType.CIRCULAR;

//			// если отклонения небольшие / проверку сплайна не делаем т.к. отклонения небольшие для всех Sh.
//			if (Math.Abs(Sh1) < Conf.LineDirtCount && Math.Abs(Sh2) < Conf.LineDirtCount && Math.Abs(Sh3) < Conf.LineDirtCount)
//				return (int)RoadSector.rType.LINE;
//			#endregion

//			#region тонкий фильтр / порядок выполнения может иметь значение
//			int isline = -1;
//			// устанавливает значение в 1 если угол отклонения дороги менее 1 градуса
//			if ((Math.Abs(Sh6) <= Math.PI / 1800) || (Math.Abs(Sh6) <= Math.PI / 180 && Math.Abs(Sh4) <= Math.PI / 180 && Math.Abs(Sh5) <= Math.PI / 180))
//				isline = 1;

//			// если отклонения чувствительны, но ниже установленной граници и отклонение по углу меньше 1 градуса
//			if (Math.Abs(Sh1) < Conf.LineDirtCount * 2 && Math.Abs(Sh2) < Conf.LineDirtCount * 2 && Math.Abs(Sh3) < Conf.LineDirtCount * 2
//				&& Conf.GroupConnKof < rs.IdDAEnd - rs.IdDAStart && isline == 1)
//				return (int)RoadSector.rType.LINE;

//			// если незначительны, но больше граничных +- 30см и выполняется свойство сплайнов для круговых (идет выпрямление)
//			if (Math.Abs(Sh1) > Conf.LineDirtCount && Math.Abs(Sh2) > Conf.LineDirtCount && Math.Abs(Sh3) > Conf.LineDirtCount)
//			{
//				if (isline != 1) //соблюдаем заданные рамки по точности
//					if (Math.Abs(Sh1) > Math.Abs(Sh2) && Math.Abs(Sh3) > Math.Abs(Sh2))
//						return (int)RoadSector.rType.CIRCULAR;
//			}
//			#endregion

//			if (isNullEnable && Sh4 == 0 && Sh5 == 0) //пришел зануленный участок
//				return (int)RoadSector.rType.LINE;

//			return (int)RoadSector.rType.UNKNOWN;
//		}

//		public static int DefineGroups(RoadSector rs, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, Config Conf)
//		{
//			double Sh1 = 0; double Sh2 = 0;
//			double MaxSh = 0;

//			for (int i = rs.IdDAStart; i <= rs.IdDAEnd; i++)
//			{
//				Sh1 += StartEndAzimuthLen[i + 1].Length * Math.Tan(AnglDifferentSet[i]);
//				Sh2 += AnglDifferentSet[i];

//				MaxSh = Math.Max(MaxSh, Math.Abs(Sh1));
//			}

//			int isline = -1;
//			if ((Math.Abs(Sh2) <= Math.PI / 1800) || (Math.Abs(Sh2) <= Math.PI / 180))
//				isline = 1;

//			if ((Math.Abs(Sh1) > Conf.LineDirtCount || Math.Abs(Sh1) > Conf.LineDirtCount * 2) && isline != 1)
//				return (int)RoadSector.rType.CIRCULAR;

//			if ((Math.Abs(Sh1) < Conf.LineDirtCount) ||
//				(Math.Abs(Sh1) < Conf.LineDirtCount * 2 && Conf.GroupConnKof < rs.IdDAEnd - rs.IdDAStart && isline == 1))
//			{
//				return (int)RoadSector.rType.LINE;
//			}

//			return (int)RoadSector.rType.UNKNOWN;
//		}
//		#endregion

//		#region допилы
//		public static void СonnectGroop(double[] AnglDifferentSet, ref LinkedList<RoadSector> RoadGroupList, bool ignoreAngle = false)
//		{
//			LinkedListNode<RoadSector> node = RoadGroupList.First;

//			while (node.Next != null)
//			{
//				double angl = 0;

//				if ((node.Value.Type == (int)RoadSector.rType.LINE) &&
//					(node.Next.Value.Type == (int)RoadSector.rType.LINE))
//				{
//					node.Value.IdDAEnd = node.Next.Value.IdDAEnd;
//					RoadGroupList.Remove(node.Next);

//					if (!ignoreAngle)
//					{
//						for (int i = node.Value.IdDAStart; i <= node.Value.IdDAEnd; i++)
//							angl += AnglDifferentSet[i];

//						if (Math.Abs(angl) > 3.0 * Math.PI / 180)//3 градуса
//							node.Value.Type = (int)RoadSector.rType.UNKNOWN;
//					}

//					if (node.Previous != null)
//						node = node.Previous;
//				}
//				node = node.Next;
//			}
//		}

//		public static void СonnectSmall(double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, Config Conf, ref LinkedList<RoadSector> RoadGroupList)
//		{
//			int Old = 0;
//			int Curent = RoadGroupList.Count;

//			do
//			{   // до тех пор пока неостанется коротких участков
//				Old = Curent; // Old - Curent = количество коротких участков
//				LinkedListNode<RoadSector> node = RoadGroupList.First;
//				while (node != null)
//				{
//					if (node.Value.IdDAEnd - node.Value.IdDAStart < 3)
//					{
//						if (node.Next == null)
//						{   // если точа стоит в конце
//							node = node.Previous;
//							node.Value.IdDAEnd = node.Next.Value.IdDAEnd;
//							RoadGroupList.Remove(node.Next);
//						}
//						else
//						{
//							if (node.Previous == null)
//							{   //если точка стоит в начале
//								node = node.Next;
//								node.Value.IdDAStart = node.Previous.Value.IdDAStart;
//								RoadGroupList.Remove(node.Previous);
//							}
//							else
//							{   // если точка стоит в середине
//								double ShP = 0; double ShN = 0;

//								if (node.Previous.Value.Type == (int)RoadSector.rType.LINE && node.Next.Value.Type == (int)RoadSector.rType.LINE)
//								{
//									for (int i = node.Previous.Value.IdDAStart; i <= node.Next.Value.IdDAEnd; i++)
//										ShP += StartEndAzimuthLen[i + 1].Length * Math.Tan(AnglDifferentSet[i]);

//									if (Math.Abs(ShP) < Conf.LineDirtCount * 2)
//									{
//										node.Previous.Value.IdDAEnd = node.Next.Value.IdDAEnd;
//										node = node.Previous;
//										RoadGroupList.Remove(node.Next);
//										RoadGroupList.Remove(node.Next);
//									}
//									else
//									{
//										node.Value.Type = (int)RoadSector.rType.CIRCULAR;
//										node = node.Next;
//									}
//								}
//								else
//								{
//									if (node.Previous.Value.Type == (int)RoadSector.rType.LINE)
//									{
//										node.Next.Value.IdDAStart = node.Value.IdDAStart;
//										node = node.Previous;
//										RoadGroupList.Remove(node.Next);
//									}
//									else
//									{
//										if (node.Next.Value.Type == (int)RoadSector.rType.LINE)
//										{
//											node.Previous.Value.IdDAEnd = node.Value.IdDAEnd;
//											node = node.Next;
//											RoadGroupList.Remove(node.Previous);
//										}
//										else
//										{
//											for (int i = node.Previous.Value.IdDAStart; i <= node.Previous.Value.IdDAEnd; i++)
//												ShP += StartEndAzimuthLen[i + 1].Length * Math.Tan(AnglDifferentSet[i]);

//											for (int i = node.Next.Value.IdDAStart; i <= node.Next.Value.IdDAEnd; i++)
//												ShN += StartEndAzimuthLen[i + 1].Length * Math.Tan(AnglDifferentSet[i]);

//											var ehP = ShP / (node.Previous.Value.IdDAEnd - node.Previous.Value.IdDAStart + 1);
//											var ehN = ShN / (node.Next.Value.IdDAEnd - node.Next.Value.IdDAStart + 1);

//											for (int i = node.Value.IdDAStart; i <= node.Value.IdDAEnd; i++)
//											{
//												var buf = StartEndAzimuthLen[i + 1].Length * Math.Tan(AnglDifferentSet[i]);
//												if (Math.Abs(buf - ehP) < Math.Abs(buf - ehN))
//												{
//													node.Previous.Value.IdDAEnd = i;

//													ShP = 0;
//													for (int j = node.Previous.Value.IdDAStart; j <= node.Previous.Value.IdDAEnd; j++)
//														ShP += StartEndAzimuthLen[j + 1].Length * Math.Tan(AnglDifferentSet[j]);
//												}
//												else
//												{
//													node.Next.Value.IdDAStart = i;
//													i = node.Value.IdDAEnd + 1;
//												}
//											}
//											node = node.Next;
//											RoadGroupList.Remove(node.Previous);
//										}
//									}
//								}
//							}
//						}
//					}
//					else
//					{
//						node = node.Next;
//					}
//				}
//				Curent = RoadGroupList.Count;
//			} while (Curent != Old);
//		}

//		private static LinkedList<RoadSector> MergeCircular(GPS2DCoordinat[] InputeRoadPoint, StartEndAzimuthLen[] StartEndAzimuthLen,
//		  double[] AnglDifferentSet, Config Conf, LinkedList<RoadSector> RoadGroup)
//		{
//			var node = RoadGroup.First;
//			bool isMerge = false;

//			while (node != null)
//			{
//				if ((node.Value.Type == (int)RoadSector.rType.CIRCULAR || node.Value.Type == (int)RoadSector.rType.UNKNOWN) &&
//				(node.Next != null && (node.Next.Value.Type == (int)RoadSector.rType.CIRCULAR || node.Next.Value.Type == (int)RoadSector.rType.UNKNOWN)))
//				{
//					var s1 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Value);
//					var s2 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Next.Value);

//					if (Math.Sign(s1) == Math.Sign(s2))
//					{
//						double maxR = Math.Max(node.Value.MinMaxAvgRad[2], node.Next.Value.MinMaxAvgRad[2]);
//						double len = Conf.BasicMath.GetStartEndAzimuthLen(node.Value.CenterPoint.Point, node.Next.Value.CenterPoint.Point).Length;
//						double delta1 = len / (2.0 * maxR);
//						double delta2 = Math.Abs(node.Value.MinMaxAvgRad[2] - node.Next.Value.MinMaxAvgRad[2]) / (2.0 * maxR);

//						if (delta1 < 0.2 && delta2 < 0.2)
//						{
//							node.Value.IdDAEnd = node.Next.Value.IdDAEnd;
//							RoadGroup.Remove(node.Next);
//							node.Value = FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, node.Value);
//							node.Value.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, node.Value);

//							while (node.Previous != null && (node.Value.Type == (int)RoadSector.rType.CIRCULAR || node.Value.Type == (int)RoadSector.rType.UNKNOWN))
//								node = node.Previous;
//						}
//					}
//				}
//				if (!isMerge)
//					node = node.Next;
//				isMerge = false;
//			}
//			return RoadGroup;
//		}

//		private static LinkedList<RoadSector> BetweenLine(double[] AnglDifferentSet, Config Conf, LinkedList<RoadSector> RoadGroupList)
//		{
//			LinkedListNode<RoadSector> node = RoadGroupList.First;

//			while (node != null)
//			{
//				if (node.Value.Type == (int)RoadSector.rType.UNKNOWN &&
//					(node.Previous != null && node.Previous.Value.Type == (int)RoadSector.rType.LINE) &&
//					(node.Next != null && node.Next.Value.Type == (int)RoadSector.rType.LINE))
//				{
//					if (node.Value.MinMaxAvgRad[2] < Conf.MaxRad / 2.0)
//						node.Value.Type = (int)RoadSector.rType.CIRCULAR;
//					else 
//					{
//						double sum1 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Previous.Value);
//						double sum2 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Value);
//						double sum3 = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Next.Value);

//						if (sum1 + sum2 + sum3 > sum2)
//							node.Value.Type = (int)RoadSector.rType.CIRCULAR;
//						else
//							node.Value.Type = (int)RoadSector.rType.LINE;
//					}
//				}

//				node = node.Next;
//			}
//			return RoadGroupList;
//		}

//		private static void DefineLineByRad(Config Conf, ref LinkedList<RoadSector> RoadGroupList)
//		{
//			var node = RoadGroupList.First;

//			while (node != null)
//			{
//				node.Value.Type = DefineLineByRad(Conf, node.Value);
//				node = node.Next;
//			}
//		}

//		public static int DefineLineByRad(Config Conf, RoadSector rs)
//		{
//			if (rs.Type == (int)RoadSector.rType.UNKNOWN)
//			{
//				if (rs.MinMaxAvgRad != null && rs.CenterPoint != null && !rs.CenterPoint.IsEmpty)
//				{
//					double eRad = rs.MinMaxAvgRad[2];

//					if (eRad > Conf.MaxRad)
//						return (int)RoadSector.rType.LINE;
//				}
//			}
//			return (int)rs.Type;
//		}

//		private static LinkedList<RoadSector> Else(GPS2DCoordinat[] InputeRoadPoint,StartEndAzimuthLen[] StartEndAzimuthLen, double[] AnglDifferentSet, Config Conf, LinkedList<RoadSector> RoadGroupList)
//		{
//			LinkedListNode<RoadSector> node = RoadGroupList.First;

//			while (node != null)
//			{
//				if (node.Value.Type == (int)RoadSector.rType.UNKNOWN)
//				{
//					double sum = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Value);
                                       
//					if (Math.Abs(sum) > BasicConvert.GradToRad(15))
//					{
//						node.Value.Type = (int)RoadSector.rType.CIRCULAR;
//						node.Value = FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, node.Value);
//						node.Value.MinMaxAvgRad = CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, node.Value);

//						if(node.Value.MinMaxAvgRad == null || node.Value.MinMaxAvgRad[2] > Conf.MaxRad)
//							node.Value.Type = (int)RoadSector.rType.LINE;

//					}
//					else
//						node.Value.Type = (int)RoadSector.rType.LINE;
//				}

//				node = node.Next;
//			}
//			return RoadGroupList;
//		}
//		#endregion

//		private static double[][] MergeKAngle(double[][] BfCleanAngls, double[] AnglDifferentSet, List<int[]> lineGroups)
//		{
//			for (int i = 0; i < BfCleanAngls.Length; i++)
//			{
//				BfCleanAngls[i][1] = AnglDifferentSet[i];
//			}
//			for (int i = 0; i < lineGroups.Count; i++)
//			{
//				for (int j = lineGroups[i][0]; j <= lineGroups[i][1]; j++)
//				{
//					BfCleanAngls[j][1] = 0;
//				}
//			}
//			return BfCleanAngls;
//		}
//	}
//}