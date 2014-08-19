using System.Collections.Generic;
using ZedGraph;
using System.Drawing;

namespace Lib.Graphic.ZedGraph
{
	// http://jenyay.net/Programming/ZedGraph
	public class ZedGHelper
	{
		public class Settings
		{
			public string GrafikName { get; set; }
			public Color GrafikColor { get; set; }
			public bool isFill { get; set; }
			public SymbolType Type { get; set; }
			public bool EnablRefresh { get; set; }
			public bool invers { get; set; }

			public Settings()
			{
				GrafikName = null;
				GrafikColor = Color.Black;
				isFill = false;
				Type = SymbolType.None;
				EnablRefresh = false;
				invers = false;
			}
		}

		public static int Count(ZedGraphControl zedGraphControl)
		{
			return zedGraphControl.GraphPane.CurveList.Count;
		}
		/// <summary>
		/// удаляет точки из списка
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="count">Количество удаляемых записей</param>
		public static void Clean(ZedGraphControl zedGraphControl, int count, bool EnablRefresh)
		{
			int delC = count;
			while ((delC--) > -1)
			{
				if (zedGraphControl.GraphPane.CurveList.Count > 0)
				{
					zedGraphControl.GraphPane.CurveList.RemoveAt(0);
					zedGraphControl.AxisChange();
				}
				else
				{
					delC = -1;
				}
			}
			if (EnablRefresh)
				zedGraphControl.Refresh();
		}
		/// <summary>
		/// удаляет точки из списка
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="count">Количество удаляемых записей</param>
		public static void CleanAll(ZedGraphControl zedGraphControl, bool EnablRefresh)
		{
			int count = ZedGHelper.Count(zedGraphControl);
			int delC = count;
			while ((delC--) > -1)
			{
				if (zedGraphControl.GraphPane.CurveList.Count > 0)
				{
					zedGraphControl.GraphPane.CurveList.RemoveAt(0);
					zedGraphControl.AxisChange();
				}
				else
				{
					delC = -1;
				}
			}
			if (EnablRefresh)
				zedGraphControl.Refresh();
		}
		/// <summary>
		/// Рисует график по точкам
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="GrafikName">название графика</param>
		/// <param name="GrafikColor">цвет линии</param>
		/// <param name="Dots">лист точек</param>
		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, List<double[]> Dots)
		{
			PointPairList PL = new PointPairList();
			if (conf.invers)
			{
				foreach (double[] point in Dots)
				{
					PL.Add(point[1], point[0]);
				}
			}
			else
			{
				foreach (double[] point in Dots)
				{
					PL.Add(point[0], point[1]);
				}
			}
			zedGraphControl.GraphPane.AddCurve(conf.GrafikName, PL, conf.GrafikColor, conf.Type);

			if (conf.isFill)
				zedGraphControl.GraphPane.Chart.Fill.Color = conf.GrafikColor;
			zedGraphControl.AxisChange();
			if (conf.EnablRefresh)
				zedGraphControl.Refresh();
		}


		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, double[] x, double[] y)
		{
			PointPairList PL = new PointPairList();
			if (conf.invers)
			{
				for(int i = 0 ; i < x.Length ; i++)
				{
					PL.Add(y[i], x[i]);
				}
			}
			else
			{
				for (int i = 0; i < x.Length; i++)
				{
					PL.Add(x[i], y[i]);
				}
			}
			zedGraphControl.GraphPane.AddCurve(conf.GrafikName, PL, conf.GrafikColor, conf.Type);

			if (conf.isFill)
				zedGraphControl.GraphPane.Chart.Fill.Color = conf.GrafikColor;
			zedGraphControl.AxisChange();
			if (conf.EnablRefresh)
				zedGraphControl.Refresh();
		}


		/// <summary>
		/// Рисует график по точкам
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="GrafikName">название графика</param>
		/// <param name="GrafikColor">цвет линии</param>
		/// <param name="Dots">лист точек</param>
		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, double[][] Dots)
		{
			PointPairList PL = new PointPairList();
			if (conf.invers)
			{
				foreach (double[] point in Dots)
				{
					PL.Add(point[1], point[0]);
				}
			}
			else
			{
				foreach (double[] point in Dots)
				{
					PL.Add(point[0], point[1]);
				}
			}
			zedGraphControl.GraphPane.AddCurve(conf.GrafikName, PL, conf.GrafikColor, conf.Type);
			if (conf.isFill)
				zedGraphControl.GraphPane.Chart.Fill.Color = conf.GrafikColor;
			zedGraphControl.AxisChange();
			if (conf.EnablRefresh)
				zedGraphControl.Refresh();

		}

		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, IEnumerable<Spline.Core.Point> dots)
		{
			var pl = new PointPairList();
			if (conf.invers)
			{
				foreach (var point in dots)
				{
					pl.Add(point.Y, point.X);
				}
			}
			else
			{
				foreach (var point in dots)
				{
					pl.Add(point.X, point.Y);
				}
			}
			zedGraphControl.GraphPane.AddCurve(conf.GrafikName, pl, conf.GrafikColor, conf.Type);
			if (conf.isFill)
				zedGraphControl.GraphPane.Chart.Fill.Color = conf.GrafikColor;
			zedGraphControl.AxisChange();
			if (conf.EnablRefresh)
				zedGraphControl.Refresh();

		}

		public static void PaintClose(ZedGraphControl zedGraphControl, Settings conf, double[][] Dots)
		{
			PointPairList PL = new PointPairList();
			if (conf.invers)
			{
				foreach (double[] point in Dots)
				{
					PL.Add(point[1], point[0]);
				}
				PL.Add(Dots[0][1], Dots[0][0]);
			}
			else
			{
				foreach (double[] point in Dots)
				{
					PL.Add(point[0], point[1]);
				}
				PL.Add(Dots[0][0], Dots[0][1]);
			}

			zedGraphControl.GraphPane.AddCurve(conf.GrafikName, PL, conf.GrafikColor, conf.Type);

			if (conf.isFill)
				zedGraphControl.GraphPane.Chart.Fill.Color = conf.GrafikColor;
			zedGraphControl.AxisChange();
			if (conf.EnablRefresh)
				zedGraphControl.Refresh();
		}

		/// <summary>
		/// изменение текста
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="Title">заголовок</param>
		/// <param name="XAxis">нижняя грань</param>
		/// <param name="YAxis">левая грань</param>
		public static void Text(ZedGraphControl zedGraphControl, string Title, string XAxis, string YAxis)
		{
			GraphPane myPane = zedGraphControl.GraphPane;
			myPane.Title.Text = Title;
			myPane.XAxis.Title.Text = XAxis;
			myPane.YAxis.Title.Text = YAxis;
		}

		public static void ScaledPenWidth(ZedGraphControl zedGraphControl, float penWidth, float scaleFactor)
		{
			GraphPane myPane = zedGraphControl.GraphPane;
			myPane.ScaledPenWidth(3, 3);
		}

		public static void XYScale(ZedGraphControl zedGraphControl, double dsize)
		{
			zedGraphControl.ZoomEvent += new ZedGraphControl.ZoomEventHandler(
				delegate(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
				{
					sender.GraphPane.YAxis.Scale.Max =
						sender.GraphPane.YAxis.Scale.Min +
						(sender.GraphPane.XAxis.Scale.Max - sender.GraphPane.XAxis.Scale.Min) / dsize;
				});
		}
	}
}