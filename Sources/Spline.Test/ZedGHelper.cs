using System.Collections.Generic;
using System.Drawing;
using ZedGraph;

namespace Spline.Test
{
	// http://jenyay.net/Programming/ZedGraph
	public class ZedGHelper
	{
		public class Settings
		{
			public string GrafikName { get; set; }
			public Color GrafikColor { get; set; }
			public bool IsFill { get; set; }
			public SymbolType Type { get; set; }
			public bool EnablRefresh { get; set; }
			public bool Invers { get; set; }

			public Settings()
			{
				GrafikName = null;
				GrafikColor = Color.Black;
				IsFill = false;
				Type = SymbolType.None;
				EnablRefresh = false;
				Invers = false;
			}

			public string Title { get; set; }

			public string XTitle { get; set; }

			public string YTitle { get; set; }
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
		/// <param name="enablRefresh"></param>
		public static void Clean(ZedGraphControl zedGraphControl, int count, bool enablRefresh)
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
			if (enablRefresh)
				zedGraphControl.Refresh();
		}

		/// <summary>
		/// удаляет точки из списка
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="enablRefresh"></param>
		public static void CleanAll(ZedGraphControl zedGraphControl, bool enablRefresh)
		{
			int count = Count(zedGraphControl);
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
			if (enablRefresh)
				zedGraphControl.Refresh();
		}



		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, PointPairList pl, bool isNewPane)
		{
			GraphPane myPaneT;
			if (isNewPane)
			{
				myPaneT = new GraphPane(new Rectangle(), conf.Title, conf.XTitle, conf.YTitle);
			}
			else
			{
				myPaneT = zedGraphControl.GraphPane;
			}

			myPaneT.AddCurve(conf.GrafikName, pl, conf.GrafikColor, conf.Type);

			if (conf.IsFill)
				myPaneT.Chart.Fill.Color = conf.GrafikColor;

			if (isNewPane)
			{
				zedGraphControl.MasterPane.Add(myPaneT);
			}

			using (Graphics g = zedGraphControl.CreateGraphics())
			{
				zedGraphControl.MasterPane.SetLayout(g, PaneLayout.SingleColumn);
				zedGraphControl.MasterPane.AxisChange(g);

				// Synchronize the Axes
				zedGraphControl.IsAutoScrollRange = false;
				//zedGraphControl.IsShowHScrollBar = true;
				zedGraphControl.IsSynchronizeXAxes = true;
				//g.Dispose();
			}
			//zedGraphControl.AxisChange();
			//if (conf.EnablRefresh)
			//    zedGraphControl.Refresh();
		}

		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, double[] x, double[] y, bool isNewPane)
		{
			PointPairList pl = GetPointPairList(conf, x, y);
			Paint(zedGraphControl, conf, pl, isNewPane);
		}

		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, IEnumerable<Core.Point> dots, bool isNewPane)
		{
			var pl = GetPointPairList(conf, dots);
			Paint(zedGraphControl, conf, pl, isNewPane);
		}

		public static void Paint(ZedGraphControl zedGraphControl, Settings conf, IEnumerable<double[]> dots, bool isNewPane)
		{
			PointPairList pl = GetPointPairList(conf, dots);
			Paint(zedGraphControl, conf, pl, isNewPane);
		}



		private static PointPairList GetPointPairList(Settings conf, IEnumerable<Core.Point> dots)
		{
			var pl = new PointPairList();
			if (conf.Invers)
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
			return pl;
		}

		private static PointPairList GetPointPairList(Settings conf, double[] x, double[] y)
		{
			var pl = new PointPairList();
			if (conf.Invers)
			{
				for (int i = 0; i < x.Length; i++)
				{
					pl.Add(y[i], x[i]);
				}
			}
			else
			{
				for (int i = 0; i < x.Length; i++)
				{
					pl.Add(x[i], y[i]);
				}
			}
			return pl;
		}

		private static PointPairList GetPointPairList(Settings conf, IEnumerable<double[]> dots)
		{
			var pl = new PointPairList();
			if (conf.Invers)
			{
				foreach (double[] point in dots)
				{
					pl.Add(point[1], point[0]);
				}
			}
			else
			{
				foreach (double[] point in dots)
				{
					pl.Add(point[0], point[1]);
				}
			}
			return pl;
		}



		public static void PaintClose(ZedGraphControl zedGraphControl, Settings conf, double[][] dots)
		{
			var pl = new PointPairList();
			if (conf.Invers)
			{
				foreach (double[] point in dots)
				{
					pl.Add(point[1], point[0]);
				}
				pl.Add(dots[0][1], dots[0][0]);
			}
			else
			{
				foreach (double[] point in dots)
				{
					pl.Add(point[0], point[1]);
				}
				pl.Add(dots[0][0], dots[0][1]);
			}

			zedGraphControl.GraphPane.AddCurve(conf.GrafikName, pl, conf.GrafikColor, conf.Type);

			if (conf.IsFill)
				zedGraphControl.GraphPane.Chart.Fill.Color = conf.GrafikColor;
			zedGraphControl.AxisChange();
			if (conf.EnablRefresh)
				zedGraphControl.Refresh();
		}

		/// <summary>
		/// изменение текста
		/// </summary>
		/// <param name="zedGraphControl">контрол</param>
		/// <param name="title">заголовок</param>
		/// <param name="xAxis">нижняя грань</param>
		/// <param name="yAxis">левая грань</param>
		public static void Text(ZedGraphControl zedGraphControl, string title, string xAxis, string yAxis)
		{
			GraphPane myPane = zedGraphControl.GraphPane;
			myPane.Title.Text = title;
			myPane.XAxis.Title.Text = xAxis;
			myPane.YAxis.Title.Text = yAxis;
		}

		public static void ScaledPenWidth(ZedGraphControl zedGraphControl, float penWidth, float scaleFactor)
		{
			GraphPane myPane = zedGraphControl.GraphPane;
			myPane.ScaledPenWidth(3, 3);
		}

		public static void XYScale(ZedGraphControl zedGraphControl, double dsize)
		{
			zedGraphControl.ZoomEvent += delegate(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
			{
				sender.GraphPane.YAxis.Scale.Max =
					sender.GraphPane.YAxis.Scale.Min +
					(sender.GraphPane.XAxis.Scale.Max - sender.GraphPane.XAxis.Scale.Min) / dsize;
			};
		}
	}
}