using System.Collections.Generic;
using System.Drawing;
using ZedGraph;

namespace Lib.Graphic.ZedGraph
{
    // http://jenyay.net/Programming/ZedGraph
    public class ZedGHelper
    {
        //public static void CreateGraph_ThreeVerticalPanes(ZedGraphControl zedGraphControl, Settings conf, List<double[]> dots)
        //{
        //    MasterPane master = z1.MasterPane;

        //    // Fill the background
        //    master.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
        //    // Clear out the initial GraphPane
        //    master.PaneList.Clear();

        //    // Show the masterpane title
        //    master.Title.IsVisible = true;
        //    master.Title.Text = "Synchronized Panes Demo";

        //    // Leave a margin around the masterpane, but only a small gap between panes
        //    master.Margin.All = 10;
        //    master.InnerPaneGap = 5;

        //    // The titles for the individual GraphPanes
        //    string[] yLabels = { "Rate, m/s", "Pressure, dynes/cm", "Count, units/hr" };

        //    ColorSymbolRotator rotator = new ColorSymbolRotator();

        //    for (int j = 0; j < 3; j++)
        //    {
        //        // Create a new graph -- dimensions to be set later by MasterPane Layout
        //        GraphPane myPaneT = new GraphPane(new Rectangle(10, 10, 10, 10),
        //           "",
        //           "Time, Days",
        //           yLabels[j]);

        //        myPaneT.Fill.IsVisible = false;

        //        // Fill the Chart background
        //        myPaneT.Chart.Fill = new Fill(Color.White, Color.LightYellow, 45.0F);
        //        // Set the BaseDimension, so fonts are scaled a little bigger
        //        myPaneT.BaseDimension = 3.0F;

        //        // Hide the XAxis scale and title
        //        myPaneT.XAxis.Title.IsVisible = false;
        //        myPaneT.XAxis.Scale.IsVisible = false;
        //        // Hide the legend, border, and GraphPane title
        //        myPaneT.Legend.IsVisible = false;
        //        myPaneT.Border.IsVisible = false;
        //        myPaneT.Title.IsVisible = false;
        //        // Get rid of the tics that are outside the chart rect
        //        myPaneT.XAxis.MajorTic.IsOutside = false;
        //        myPaneT.XAxis.MinorTic.IsOutside = false;
        //        // Show the X grids
        //        myPaneT.XAxis.MajorGrid.IsVisible = true;
        //        myPaneT.XAxis.MinorGrid.IsVisible = true;
        //        // Remove all margins
        //        myPaneT.Margin.All = 0;
        //        // Except, leave some top margin on the first GraphPane
        //        if (j == 0)
        //            myPaneT.Margin.Top = 20;
        //        // And some bottom margin on the last GraphPane
        //        // Also, show the X title and scale on the last GraphPane only
        //        if (j == 2)
        //        {
        //            myPaneT.XAxis.Title.IsVisible = true;
        //            myPaneT.XAxis.Scale.IsVisible = true;
        //            myPaneT.Margin.Bottom = 10;
        //        }

        //        if (j > 0)
        //            myPaneT.YAxis.Scale.IsSkipLastLabel = true;

        //        // This sets the minimum amount of space for the left and right side, respectively
        //        // The reason for this is so that the ChartRect's all end up being the same size.
        //        myPaneT.YAxis.MinSpace = 80;
        //        myPaneT.Y2Axis.MinSpace = 20;

        //        // Make up some data arrays based on the Sine function
        //        double x, y;
        //        PointPairList list = new PointPairList();
        //        for (int i = 0; i < 36; i++)
        //        {
        //            x = (double)i + 5 + j * 3;
        //            y = (j + 1) * (j + 1) * 10 * (1.5 + Math.Sin((double)i * 0.2 + (double)j));
        //            list.Add(x, y);
        //        }

        //        LineItem myCurve = myPaneT.AddCurve("Type " + j.ToString(),
        //           list, rotator.NextColor, rotator.NextSymbol);
        //        myCurve.Symbol.Fill = new Fill(Color.White);

        //        master.Add(myPaneT);
        //    }

        //    using (Graphics g = z1.CreateGraphics())
        //    {

        //        master.SetLayout(g, PaneLayout.SingleColumn);
        //        master.AxisChange(g);

        //        // Synchronize the Axes
        //        z1.IsAutoScrollRange = true;
        //        z1.IsShowHScrollBar = true;
        //        z1.IsSynchronizeXAxes = true;

        //        //g.Dispose();
        //    }

        //}


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

            public string Title { get; set; }

            public string xTitle { get; set; }

            public string yTitle { get; set; }
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



        public static void Paint(ZedGraphControl zedGraphControl, Settings conf, PointPairList pl, bool isNewPane)
        {
            GraphPane myPaneT;
            if (isNewPane)
            {
                myPaneT = new GraphPane(new Rectangle(), conf.Title, conf.xTitle, conf.yTitle);
            }
            else
            {
                myPaneT = zedGraphControl.GraphPane;
            }

            myPaneT.AddCurve(conf.GrafikName, pl, conf.GrafikColor, conf.Type);

            if (conf.isFill)
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
        
        public static void Paint(ZedGraphControl zedGraphControl, Settings conf, IEnumerable<Spline.Core.Point> dots, bool isNewPane)
        {
            var pl = GetPointPairList(conf, dots);
            Paint(zedGraphControl, conf, pl, isNewPane);
        }

        public static void Paint(ZedGraphControl zedGraphControl, Settings conf, IEnumerable<double[]> dots, bool isNewPane)
        {
            PointPairList pl = GetPointPairList(conf, dots);
            Paint(zedGraphControl, conf, pl, isNewPane);
        }

       

        private static PointPairList GetPointPairList(Settings conf, IEnumerable<Spline.Core.Point> dots)
        {
            var pl = new PointPairList();
            if (conf.invers)
            {
                foreach (var point in dots)
                {
                    pl.Add(point.Y, point.X, point.Z);
                }
            }
            else
            {
                foreach (var point in dots)
                {
                    pl.Add(point.X, point.Y, point.Z);
                }
            }
            return pl;
        }

        private static PointPairList GetPointPairList(Settings conf, double[] x, double[] y)
        {
            var pl = new PointPairList();
            if (conf.invers)
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
            if (conf.invers)
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