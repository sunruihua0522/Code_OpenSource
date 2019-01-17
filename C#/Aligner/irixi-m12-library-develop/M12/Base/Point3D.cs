namespace M12.Base
{
    public class Point3D
    {
        public Point3D ()
        {
            this.X = double.NaN;
            this.Y = double.NaN;
            this.Z = double.NaN;
        }

        public Point3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        public override string ToString()
        {
            return $"X={X}, Y={Y}, Z={Z}";
        }
    }
}
