namespace Spline.Core
{
    public struct Point : IPoint
    {
        private double _x;
        private double _y;
        private double _z;


        public Point(double x, double y)
        {
            _x = x;
            _y = y;
            _z = 0;
        }

        public Point(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }
    }
}
