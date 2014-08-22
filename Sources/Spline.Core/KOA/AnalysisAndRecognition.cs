//using System;
//using System.Collections.Generic;
//using System.Globalization;

//namespace Spline.Core.KOA
//{
//#if !(SILVERLIGHT || NET_4_0)
//	public static class AnalysisAndRecognition
//	{
//		public class RoadParts
//		{
//			public int ID { get; set; }
//			public int IdRd { get; set; }
//			public double Latitude { get; set; }
//			public double Longitude { get; set; }
//			public int Type { get; set; }
//			public double Len { get; set; }
//			public double Radius { get; set; }
//			public double Angle { get; set; }
//			public string Entity { get; set; }
//		}

//		private static Config _Conf = new Config(Vincenty.GeoType.wgs84);
//		private const string user = "GEO";
//		private const string password = "geocl3isd";
//		private const string service = "UD";
//		private const string host = "DBDEV";

//		static AnalysisAndRecognition() {}

//		public static string AnalyzeRoad(int idrd)
//		{
//			try
//			{
//				var oldParts = LoadParts(idrd);

//				var InputeRoadPoint = LoadPoints(idrd);
//				InputeRoadPoint = Prepare(InputeRoadPoint);
//				var AnglDifferentSet = GetAnglDifferentSet(InputeRoadPoint);
//				var StartEndAzimuthLen = GetStartEndAzimuthLen(InputeRoadPoint);

//				var RoadGroup = GetFirstRoadGroup(InputeRoadPoint, AnglDifferentSet, StartEndAzimuthLen);
//				RoadGroup = SecondAnalyze(InputeRoadPoint, AnglDifferentSet, StartEndAzimuthLen, RoadGroup);

//				List<object> userset = null;
//				if (oldParts.Count > 0)
//				{   
//					RoadGroup = AddUserChanges(InputeRoadPoint, AnglDifferentSet, RoadGroup, oldParts, out userset);
//					GroupRoad.СonnectSmall(AnglDifferentSet, StartEndAzimuthLen, _Conf, ref RoadGroup);
//				}

//				var parts = RoadSector2RoadPointsPart(InputeRoadPoint, AnglDifferentSet, RoadGroup, idrd, userset); 
//				SaveParts(parts);
//			}
//			catch (Exception ex)
//			{
//				return "Ошибка при анализе дороги " + idrd.ToString() + " " + ex.Message;
//			}
//			return "Анализ дороги " + idrd.ToString() + " завершен.";
//		}

//		private static GPS3DCoordinat[] Prepare(GPS3DCoordinat[] points)
//		{
//			var buf = CleanPoints.CleanPoints3D(points, _Conf);
//			buf = Spline.BSpline.SplineAdd(buf, 50, _Conf);
//			buf = CleanPoints.CleanPoints3D(buf, _Conf);
//			return buf;
//		}

//		private static double[] GetAnglDifferentSet(GPS3DCoordinat[] InputeRoadPoint)
//		{
//			return CommonFunctions.GetAnglDifferent(InputeRoadPoint, _Conf);
//		}

//		private static StartEndAzimuthLen[] GetStartEndAzimuthLen(GPS3DCoordinat[] InputeRoadPoint)
//		{
//			return _Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint);
//		}

//		private static LinkedList<RoadSector> GetFirstRoadGroup(GPS3DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen)
//		{
//			var RoadGroupList = GroupRoad.FirstGroupRoadPlan(InputeRoadPoint, AnglDifferentSet, StartEndAzimuthLen, ref _Conf);
//			return RoadGroupList;
//		}

//		private static LinkedList<RoadSector> SecondAnalyze(GPS3DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, LinkedList<RoadSector> RoadGroupList)
//		{
//			GroupRoad.ScndGroupRoad(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, _Conf, ref RoadGroupList);
//			return RoadGroupList;
//		}

//		private static RoadCoordinatContainer GetCoordinatContainer(GPS3DCoordinat[] inputeRoadPoint)
//		{
//			try
//			{
//				int count = inputeRoadPoint.Length;
//				double[] La = new double[count];
//				double[] Lo = new double[count];

//				for (int i = 0; i < inputeRoadPoint.Length; i++)
//				{
//					La[i] = inputeRoadPoint[i].Latitude;
//					Lo[i] = inputeRoadPoint[i].Longitude;
//				}

//				return new RoadCoordinatContainer(La, Lo);
//			}
//			catch { return new RoadCoordinatContainer(); }
//		}



//		private static LinkedList<RoadSector> RoadPointsPart2RoadSector(GPS3DCoordinat[] splains, List<RoadParts> parts, bool notAuto = false)
//		{
//			//TODO: untested
//			LinkedList<RoadSector> groups = new LinkedList<RoadSector>();
//			try
//			{
//				//Создаем класс для бинарного поиска координат
//				RoadCoordinatContainer rcc = GetCoordinatContainer(splains);

//				//проверяем направление дороги
//				int pt1 = rcc.GetNearestId(parts[0].Longitude, parts[0].Latitude) - 1;
//				int pt2 = rcc.GetNearestId(parts[parts.Count - 1].Longitude, parts[parts.Count - 1].Latitude) - 1;
//				if (pt2 < pt1)
//					parts.Reverse();

//				//получаем номер ближайшей точки
//				//номера участко смешены относительно номеров точек
//				int DA_S_ID = rcc.GetNearestId(parts[0].Longitude, parts[0].Latitude) - 1;
//				DA_S_ID = DA_S_ID < 0 ? 0 : DA_S_ID;//на всякий случай...

//				if (DA_S_ID > 0 && !notAuto) //если дорога стала длиннее
//					groups.AddLast(new RoadSector(0, DA_S_ID - 1, RoadSector.rType.UNKNOWN)); //добавляем новый участок

//				for (int i = 1; i < parts.Count; i++)
//				{   //заполняем середину
//					int buf = rcc.GetNearestId(parts[i].Longitude, parts[i].Latitude) - 1;
//					if ((notAuto && parts[i].Entity != "AUTO") || (!notAuto))
//					{
//						RoadSector rs = new RoadSector(DA_S_ID, buf - 1, parts[i - 1].Type);
//						groups.AddLast(rs);
//					}
//					DA_S_ID = buf;
//				}

//				int sid = DA_S_ID;//TODO: Разобраться с работой индекса!!
//				for (int i = DA_S_ID + 2; i < splains.Length - 1; i++)
//				{   //вычисляем номер последней точки
//					double partLen = splains[i].M - splains[DA_S_ID + 1].M;
//					sid = i - 1;
//					if (partLen >= parts[parts.Count - 1].Len)
//					{
//						double partLenPrev = splains[i - 1].M - splains[DA_S_ID + 1].M;

//						if (Math.Abs(partLen - parts[parts.Count - 1].Len) < Math.Abs(partLenPrev - parts[parts.Count - 1].Len))
//							sid = i - 2;
//						else
//							sid = i - 3;

//						i = splains.Length;
//					}
//				}

//				if ((notAuto && parts[parts.Count - 1].Entity != "AUTO") || (!notAuto))
//				{
//					groups.AddLast(new RoadSector(DA_S_ID, sid, parts[parts.Count - 1].Type));
//				}
//				if (splains.Length - sid > 3 && !notAuto) //если дорога стала длиннее
//					groups.AddLast(new RoadSector(sid + 1, splains.Length - 3, RoadSector.rType.UNKNOWN));//добавляем новый участок

//			}
//			catch
//			{//TODO: LOG?
//			}

//			return groups;
//		}

//		private static List<RoadParts> RoadSector2RoadPointsPart(GPS3DCoordinat[] splains, double[] AnglDifferentSet, LinkedList<RoadSector> roadGroupList, int IDRD, List<object> userset)
//		{
//			List<RoadParts> parts = new List<RoadParts>();

//			var node = roadGroupList.First;
//			int index = 0;
//			int id = 0;
//			while (node != null)
//			{
//				var part = new RoadParts();
//				part.IdRd = IDRD;
//				part.Latitude = splains[node.Value.IdDAStart + 1].Latitude;
//				part.Longitude = splains[node.Value.IdDAStart + 1].Longitude;
//				//длинна участка + расстояние до следующего участка
//				part.Len = Math.Abs(splains[node.Value.IdDAEnd + 2].M - splains[node.Value.IdDAStart + 1].M);
//				part.Type = node.Value.Type;
//				if (node.Value.MinMaxAvgRad != null)
//					part.Radius = node.Value.MinMaxAvgRad[2];
//				part.Angle = GeoMath.CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, node.Value);
//				part.Entity = "AUTO";
//				if (userset != null && userset.Count > id && (int)userset[id] == index)
//				{
//					id++;
//					part.Entity = (string)userset[id];
//					id++;
//				}            
//				parts.Add(part);

//				index++;
//				node = node.Next;
//			}
//			return parts;
//		}

//		private static string TestConvert(LinkedList<RoadSector> date1, LinkedList<RoadSector> date2)
//		{
//			var node = date1.First;
//			var tnode = date2.First;

//			while (node != null)
//			{
//				if (node.Value.IdDAStart != tnode.Value.IdDAStart ||
//					node.Value.IdDAEnd != tnode.Value.IdDAEnd ||
//					node.Value.Type != tnode.Value.Type)
//				{
//					string msg = "date1.IdDAStart = " + node.Value.IdDAStart +
//						" date2.IdDAStart = " + tnode.Value.IdDAStart +
//					   " date1.IdDAEnd = " + node.Value.IdDAEnd +
//					   " date2.IdDAEnd = " + tnode.Value.IdDAEnd;

//					return msg;
//				}
//				tnode = tnode.Next;
//				node = node.Next;

//				if ((node == null && tnode != null) || (node != null && tnode == null))
//					return "разная длинна";
//			}
//			return "";
//		}

//		private static LinkedList<RoadSector> AddUserChanges(GPS3DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet,
//			LinkedList<RoadSector> newRoadGroup, List<RoadParts> oldParts, out List<object> userset)
//		{
//			userset = new List<object>();
//			//Создаем класс для бинарного поиска координат
//			RoadCoordinatContainer rcc = GetCoordinatContainer(InputeRoadPoint);
//			//проверяем направление дороги
//			int pt1 = rcc.GetNearestId(oldParts[0].Longitude, oldParts[0].Latitude) - 1;
//			int pt2 = rcc.GetNearestId(oldParts[oldParts.Count - 1].Longitude, oldParts[oldParts.Count - 1].Latitude) - 1;
//			if (pt2 < pt1)
//				oldParts.Reverse();

//			for (int i = 0; i < oldParts.Count; i++)
//			{
//				if (oldParts[i].Entity != "AUTO")
//				{
//					//получаем номер ближайшей начальной точки
//					int DA_S_ID = rcc.GetNearestId(oldParts[i].Longitude, oldParts[i].Latitude) - 1;
//					DA_S_ID = DA_S_ID < 0 ? 0 : DA_S_ID;//на всякий случай...

//					//получаем номер ближайшей конечной точки
//					int DA_E_ID = DA_S_ID;
//					double partLen = 0;
//					while (DA_E_ID < AnglDifferentSet.Length && partLen < oldParts[i].Len)
//					{
//						partLen = InputeRoadPoint[DA_E_ID + 2].M - InputeRoadPoint[DA_S_ID + 1].M;

//						if (partLen >= oldParts[i].Len)
//						{
//							double partLenPrev = DA_E_ID > 0 ? InputeRoadPoint[DA_E_ID+1].M - InputeRoadPoint[DA_S_ID + 1].M : 0;
//							if (Math.Abs(partLen - oldParts[i].Len) > Math.Abs(partLenPrev - oldParts[i].Len))
//								DA_E_ID--;
//						}
//						else
//						{
//							DA_E_ID++;
//						}
//					}

//					// используем угол отклонения в качестве ключа
//					var angl = CommonFunctions.GetAnglDifferentSum(AnglDifferentSet, DA_S_ID, DA_E_ID);
//					if (Math.Abs(angl - oldParts[i].Angle) < 0.001)
//					{   // изменение геометрии в рамках приличия
//						// производим замену на пользовательский тип
//						Change(newRoadGroup, DA_S_ID, DA_E_ID, oldParts[i].Type);
//						userset.Add(i);
//						userset.Add(oldParts[i].Entity);
//					}
//				}
//			}
//			return newRoadGroup;
//		}

//		private static void Change(LinkedList<RoadSector> newRoadGroup, int startDA, int endDA, int Type)
//		{
//			var node = newRoadGroup.First;

//			while (node != null)
//			{
//				if (node.Value.IdDAEnd > startDA && node.Value.IdDAStart < endDA)
//				{
//					if (node.Previous != null)
//					{
//						node.Previous.Value.IdDAEnd = startDA - 1;
//						node.Value.IdDAStart = startDA;
//					}
//					if (node.Next != null)
//					{
//						node.Next.Value.IdDAStart = endDA + 1;
//						node.Value.IdDAEnd = endDA;
//					}
//					node.Value.Type = Type;
//					node = newRoadGroup.Last;
//				}
//				node = node.Next;
//			}
//		}

//		private static void SaveParts(List<RoadParts> roadParts)
//		{
//			var odbh = new BdcApp.RoadAnalysis.dll.ODBHelper(user, password, service, host);

//			string selCom = "SELECT Count(*) FROM ROAD_PARTS WHERE IDRD = " + roadParts[0].IdRd.ToString() + "ORDER BY ID";
//			string del_cmd = "DELETE FROM ROAD_PARTS WHERE IDRD = " + roadParts[0].IdRd.ToString();

//			var param = odbh.SQLTextExecuteO(selCom, null);
//			if (param.First != null)
//			{
//				int count = Convert.ToInt32(param.First.Value);
//				if (count > 0)
//					odbh.SQLTextExecuteO(del_cmd, null);
//			}
//			var specifier = "G";
//			for (int i = 0; i < roadParts.Count; i++)
//			{
//				string ins_cmd = "INSERT INTO ROAD_PARTS(ID,IDRD,LATITUDE,LONGITUDE,TYPE,LEN,RADIUS,ANGLE,ENTITY) VALUES (";
//				ins_cmd += (i+1).ToString() + ",";
//				ins_cmd += roadParts[i].IdRd.ToString() + ",";
//				ins_cmd += roadParts[i].Latitude.ToString(specifier, CultureInfo.InvariantCulture) + ",";
//				ins_cmd += roadParts[i].Longitude.ToString(specifier, CultureInfo.InvariantCulture) + ",";
//				ins_cmd += roadParts[i].Type.ToString() + ","; //TYPE
//				ins_cmd += roadParts[i].Len.ToString(specifier, CultureInfo.InvariantCulture) + ",";
//				ins_cmd += roadParts[i].Radius.ToString(specifier, CultureInfo.InvariantCulture) + ",";
//				ins_cmd += roadParts[i].Angle.ToString(specifier, CultureInfo.InvariantCulture) + ",'";
//				ins_cmd += roadParts[i].Entity + "')";
//				odbh.SQLTextExecuteO(ins_cmd, null);
//			}
//		}

//		private static GPS3DCoordinat[] LoadPoints(double idroad)
//		{
//			ODBHelper db = new ODBHelper(user, password, service, host);
//			var retval = db.SQLTextExecuteTL("select * from table(GEO.GET_GEOMETRY.GET_POINTS(" + idroad.ToString() + "))", null, ReadMetod);
//			GPS3DCoordinat[] points = retval.ToArray();
//			return points;
//		}

//		private static List<RoadParts> LoadParts(double idrd)
//		{
//			ODBHelper db = new ODBHelper(user, password, service, host);
//			return db.SQLTextExecuteTL("SELECT * FROM ROAD_PARTS WHERE IDRD = " + idrd.ToString(), null, ReadRoadPartsMetod);
//		}

//		private static RoadParts ReadRoadPartsMetod(OracleDataReader reader)
//		{  
//			try
//			{
//				var rp = new RoadParts();
//				rp.ID = (int)reader.GetDecimal(0);
//				rp.IdRd = (int)reader.GetDecimal(1);
//				rp.Latitude = reader.GetDouble(2);
//				rp.Longitude = reader.GetDouble(3);
//				rp.Type = (int)reader.GetDecimal(4);
//				rp.Len = reader.GetDouble(5);
//				rp.Radius = reader.GetDouble(6);
//				rp.Angle = reader.GetDouble(7);
//				rp.Entity = reader.GetString(8);
//				return rp;
//			}
//			catch
//			{
//				return null;
//			}
//		}

//		private static GPS3DCoordinat ReadMetod(OracleDataReader reader)
//		{
//			var pt = new GPS3DCoordinat(
//				(double)reader.GetDecimal(1),
//				(double)reader.GetDecimal(2),
//				(double)reader.GetDecimal(3));
//			try
//			{
//				pt.M = (double)reader.GetDecimal(4);
//			}
//			catch
//			{ }

//			return pt;
//		}
//	}
//#else
//	public class OutData
//	{
//		public GPS2DCoordinat[] InputeRoadPoint { get; set; }
//		public LinkedList<RoadSector> RoadGroupList { get; set; }
//		public StartEndAzimuthLen[] StartEndAzimuthLen { get; set; }
//		public double[] AnglDifferentSet { get; set; }
//		public double[] Visibility { get; set; }
//		public double[] LongitudinalGradient { get; set; }
//	}

//	public class AnalysisAndRecognition
//	{
//		private Config _Conf;
//		public Config Conf { get { return _Conf; } set { _Conf = value; } }

//		public double _progress;
//		public double Progress { get { return _progress; } }

//		public AnalysisAndRecognition()
//		{
//			Conf = new Config();
//		}

//		public AnalysisAndRecognition(Config LibConfig)
//		{
//			if (LibConfig == null)
//				Conf = new Config();
//			Conf = LibConfig;
//		}

//		public double[] GetAnglDifferentSet(GPS3DCoordinat[] InputeRoadPoint)
//		{
//			return CommonFunctions.GetAnglDifferent(InputeRoadPoint, Conf);
//		}

//		public StartEndAzimuthLen[] GetStartEndAzimuthLen(GPS3DCoordinat[] InputeRoadPoint)
//		{
//			return Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint);
//		}

//		public LinkedList<RoadSector> GetFirstRoadGroup(GPS3DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen)
//		{
//			var RoadGroupList = GroupRoad.FirstGroupRoadPlan(InputeRoadPoint, AnglDifferentSet, StartEndAzimuthLen, ref _Conf);
//			return RoadGroupList;
//		}

//		public LinkedList<RoadSector> SecondAnalyze(GPS3DCoordinat[] InputeRoadPoint, double[] AnglDifferentSet, StartEndAzimuthLen[] StartEndAzimuthLen, LinkedList<RoadSector> RoadGroupList)
//		{
//			GroupRoad.ScndGroupRoad(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);
//			return RoadGroupList;
//		}

//		public double GetHWKof(GPS3DCoordinat point)
//		{
//			return CommonFunctions.GetHWKof(point, Conf);
//		}

//		public OutData PlanInfo(GPS3DCoordinat[] InputeRoadPoint)
//		{
//			double[] AnglDifferentSet = CommonFunctions.GetAnglDifferent(InputeRoadPoint, Conf);
//			StartEndAzimuthLen[] StartEndAzimuthLen = Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint);

//			// перавая группировка
//			LinkedList<RoadSector> RoadGroupList = GroupRoad.FirstGroupRoadPlan(InputeRoadPoint, AnglDifferentSet, StartEndAzimuthLen, ref _Conf);
//			// получение точек пересечения
//			FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);
//			// вторая группировка
//			GroupRoad.ScndGroupRoad(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, ref RoadGroupList);

//			OutData Out = new OutData();
//			Out.InputeRoadPoint = InputeRoadPoint;
//			Out.RoadGroupList = RoadGroupList;
//			Out.StartEndAzimuthLen = StartEndAzimuthLen;
//			Out.AnglDifferentSet = AnglDifferentSet;
//			//Out.LongitudinalGradient = RoadLongitudinalGradient(InputeRoadPoint, StartEndAzimuthLen);
//			//Out.Visibility = MaxVisibility(InputeRoadPoint, StartEndAzimuthLen, 1, 1);
//			return Out;
//		}

//		public OutData ProfileInfo(GPS3DCoordinat[] InputeRoadPoint)
//		{
//			var StartEndAzimuthLen = CommonFunctions.GetProfileStartEndAzimuthLen(InputeRoadPoint, Conf);
//			var AnglDifferentSet = CommonFunctions.GetProfileAnglDifferentSet(StartEndAzimuthLen);

//			var RoadGroupList = GroupRoad.FirstGroupRoadProfile(AnglDifferentSet, StartEndAzimuthLen, ref _Conf);

//			double sumL = 0;
//			double[][] HL = new double[InputeRoadPoint.Length][];
//			for (int i = 0; i < InputeRoadPoint.Length; i++)
//			{
//				HL[i] = new double[] { sumL, InputeRoadPoint[i].Height };
//				if (InputeRoadPoint.Length - 1 > i)
//					sumL += Conf.BasicMath.GetStartEndAzimuthLen(InputeRoadPoint[i], InputeRoadPoint[i + 1]).Length;
//			}

//			FindCenters.GetMXLaLoProfil(HL, StartEndAzimuthLen, AnglDifferentSet, Conf, RoadGroupList);
//			CommonFunctions.GetMinMaxAvgRadius(HL, ref RoadGroupList);

//			OutData Out = new OutData();
//			Out.InputeRoadPoint = InputeRoadPoint;
//			Out.RoadGroupList = RoadGroupList;
//			Out.StartEndAzimuthLen = StartEndAzimuthLen;
//			Out.AnglDifferentSet = AnglDifferentSet;
//			return Out;
//		}

//		public GPS3DCoordinat[] Prepare(GPS3DCoordinat[] points)
//		{
//			var buf = CleanPoints.CleanPoints3D(points, Conf);
//			buf = Spline.BSpline.SplineAdd(buf, 50, Conf);
//			buf = CleanPoints.CleanPoints3D(buf, Conf);
//			return buf;
//		}

//		public RoadSector GetType(GPS3DCoordinat[] InputeRoadPoint, StartEndAzimuthLen[] StartEndAzimuthLen, double[] AnglDifferentSet, RoadSector rs)
//		{
//			try
//			{
//				//Like Filtrs.GroupRoad.FirstGroupRoadPlan

//				//DefineGroups(CleanAngls, CleanAndSplineAngls, AnglDifferentSet, StartEndAzimuthLen, Conf, ref RoadGroupList);

//				//TODO: Сделать определение типа! ибо тут пусто!
//				GeoMath.FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, rs);
//				rs.MinMaxAvgRad = GeoMath.CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, rs);
//				rs.Type = GroupRoad.DefineLineByRad(Conf, rs);
//			}
//			catch { }
//			return rs;
//		}

//		public RoadSector GetMinMaxAvgRad(GPS3DCoordinat[] InputeRoadPoint, StartEndAzimuthLen[] StartEndAzimuthLen, double[] AnglDifferentSet, RoadSector rs)
//		{
//			try
//			{
//				rs = GeoMath.FindCenters.GetMXLaLo(InputeRoadPoint, StartEndAzimuthLen, AnglDifferentSet, Conf, rs);
//				rs.MinMaxAvgRad = GeoMath.CommonFunctions.GetMinMaxAvgRadius(InputeRoadPoint, Conf, rs);
//			}
//			catch { }
//			return rs;
//		}
//	}
//#endif
//}