//using System;

//namespace Spline.Core
//{
//	internal class KSpline
//	{
//		/// <summary>
//		/// экспоненциальное приближение к нулю (чем меньше значение - тем больше коф. зануления)
//		/// </summary>
//		/// <param name="difangl">Входной набор</param>
//		/// <param name="Conf">Настройки</param>
//		/// <returns></returns>
//		public static double[][] CleanNoise(double[] difangl, Config Conf)
//		{
//			double[][] ptWork = new double[difangl.Length][];
//			double[][] ptkof = new double[difangl.Length][];

//			//формируем набор пригодный для работы с BSpail
//			for (int i = 0; i < difangl.Length; i++)
//			{
//				ptWork[i] = new double[] { i, difangl[i] };
//				ptkof[i] = new double[] { i, difangl[i] };
//			}

//			//получаем набор сглаженных точек - используются в роли параметра зануления
//			BSpline.SplinePoints(Conf.GroupCleanSplKof, ref ptkof);

//			double x2 = Math.Pow(Math.Abs(Conf.GroupStrongKof), 2.0 / 3.0);
//			for (int i = 0; i < ptWork.Length; i++)
//			{
//				double x1 = Math.Pow(Math.Abs(ptkof[i][1]), 2.0 / 3.0);

//				double x = x2 - x1;
//				double astroid = Math.Pow(Math.Abs(x), Conf.AstroidKof);

//				double bfx = (ptWork[i][1]) - Math.Sign(ptWork[i][1]) * Math.Abs(astroid);

//				if (double.IsNaN(bfx))
//					ptWork[i][1] = 0;
//				else
//					if (Math.Sign(bfx) != Math.Sign(ptWork[i][1]) || bfx == 0 || Math.Abs(ptkof[i][1]) < Conf.GroupSplainCropKof)
//						ptWork[i][1] = 0;  //изменение привело к перемене знака -> зануляем
//			}

//			bool Event = false;
//			do
//			{
//				CleanLonelyPoints(ref ptWork); // зануляет одиночные выбросы
//				//CleanDualPoints(ref ptWork, ref Event);  
//				Event = false;
//			} while (Event);

//			FillEmptiness(difangl, Conf.GroupConnKof, ref ptWork);

//			return ptWork;
//		}

//		/// <summary>
//		/// фрагментарное сглаживание входного массива
//		/// набор разбивается на участки, разделенные нулевыми элементами
//		/// -> точки не входяшие в участок не оказывают влияние при сглаживании
//		/// </summary>
//		/// <param name="Points">входные точки</param>
//		/// <param name="splKof">коф. сглаживания</param>
//		/// <returns></returns>
//		public static double[][] KLineSpline(double[][] Points, int splKof)
//		{
//			double[][] SplPoint = new double[Points.Length][];

//			int startId = 0; // левая граница множества точек для сглаживания
//			int endId = 0;   // правая граница множества точек для сглаживания
//			for (int i = 0; i < Points.Length; i++) //для всех входных точек
//			{
//				if (Points[i][1] == 0) // если точка - зануленная (зануленные точки служат границой для множества точек подлежаших сглаживаню)
//				{
//					int zNum = 0;
//					// считаем количество последовательных, зануленных точек
//					for (int j = i + 1; j < Points.Length; j++)
//					{
//						if (Points[j][1] == 0)
//							zNum++;
//						else
//							j = Points.Length;
//					}
//					// если точек было больше чем 3
//					if (zNum > 3)
//					{
//						// проверяем достаточно ли точек (НЕ зануленное множество) для сглаживания
//						if (endId - startId > 3)
//							BSpline.SplinePoints(startId, endId, Points, splKof, ref SplPoint);
//						// намечаем новые границы для НЕ зануленного множества
//						startId = i + zNum;
//						endId = i + zNum;
//					}
//					else
//					{
//						//иначе добавим зануленные точки в НЕ зануленное множество
//						endId = i + zNum;
//					}
//					i = i + zNum;
//				}
//				else
//				{
//					endId = i + 1;
//				}
//			}
//			// проверка хвоста
//			if (endId - 1 - startId > 3)
//				BSpline.SplinePoints(startId, endId - 1, Points, splKof, ref SplPoint);

//			// дописываем в выходной массив незаполненные элементы (зануленные точки)
//			for (int i = 0; i < SplPoint.Length; i++)
//				if (SplPoint[i] == null)
//					SplPoint[i] = Points[i];

//			return SplPoint;
//		}

//		/// <summary>
//		/// удаляет из рабочего массива данные о номере точки
//		/// -> делает из [номер][значение] - [значение]
//		/// [номер][значение] используется для промежуточных вычислений, 
//		/// позволяет избежать проблемм возникающих при сглаживании (смешение точек по оси Х)
//		/// </summary>
//		/// <param name="CleanLineSplinePoints">рабочий массив [номер][значение]</param>
//		/// <returns></returns>
//		public static double[] ToDoubleArray(double[][] CleanLineSplinePoints)
//		{
//			double[] outPoints = new double[CleanLineSplinePoints.Length];
//			for (int i = 0; i < CleanLineSplinePoints.Length; i++)
//				outPoints[i] = CleanLineSplinePoints[i][1];
//			return outPoints;
//		}

//		/// <summary>
//		/// востанавливает зауленные точки если размер множества 
//		/// точек меньше чем steadyNum
//		/// </summary>
//		/// <param name="difangl">исходное значение точек</param>
//		/// <param name="steadyNum">мин. количество точек множества</param>
//		/// <param name="ptWork">рабочий набор</param>
//		private static void FillEmptiness(double[] difangl, int steadyNum, ref double[][] ptWork)
//		{
//			for (int i = 0; i < ptWork.Length; i++)
//			{
//				if (ptWork[i][1] == 0)
//				{
//					int zNum = 0;
//					for (int j = i + 1; j < ptWork.Length; j++)
//					{
//						if (ptWork[j][1] == 0)
//							zNum++;
//						else
//							j = ptWork.Length;
//					}
//					if (zNum >= steadyNum)
//						i = i + zNum;
//					else
//						ptWork[i][1] = difangl[i];
//				}
//				else
//				{
//					ptWork[i][1] = difangl[i];
//				}
//			}
//		}

//		/// <summary>
//		/// зануление одиночных выбросов
//		/// </summary>
//		/// <param name="ptWork"></param>
//		private static void CleanLonelyPoints(ref double[][] ptWork)
//		{
//			for (int i = 1; i < ptWork.Length - 1; i++)
//			{
//				if ((ptWork[i][1] != 0) && (ptWork[i - 1][1] == 0) && (ptWork[i + 1][1] == 0))
//					ptWork[i][1] = 0;
//				if ((ptWork[i][1] != 0) && (ptWork[i - 1][1] == 0) && (Math.Sign(ptWork[i + 1][1]) != Math.Sign(ptWork[i][1])))
//					ptWork[i][1] = 0;
//				if ((ptWork[i][1] != 0) && (Math.Sign(ptWork[i - 1][1]) != Math.Sign(ptWork[i][1])) && (ptWork[i + 1][1] == 0))
//					ptWork[i][1] = 0;
//			}
//		}

//		/// <summary>
//		/// зануление парных выбросов
//		/// </summary>
//		/// <param name="ptWork"></param>
//		/// <param name="Event"></param>
//		private static void CleanDualPoints(ref double[][] ptWork, ref bool Event)
//		{
//			for (int i = 2; i < ptWork.Length - 2; i++)
//			{
//				if ((ptWork[i][1] != 0) &&
//					((ptWork[i - 2][1] == 0) || (ptWork[i - 1][1] == 0)) &&
//					((ptWork[i + 1][1] == 0) || (ptWork[i + 2][1] == 0))
//					)
//				{
//					Event = true;
//					ptWork[i][1] = 0;
//				}
//				if ((ptWork[i][1] != 0) &&
//					((ptWork[i - 2][1] == 0) || (ptWork[i - 1][1] == 0)) &&
//					((Math.Sign(ptWork[i + 1][1]) != Math.Sign(ptWork[i][1])) ||
//					 (Math.Sign(ptWork[i + 2][1]) != Math.Sign(ptWork[i][1]))
//					)
//				   )
//				{
//					Event = true;
//					ptWork[i][1] = 0;
//				}
//			}
//		}
//	}
//}