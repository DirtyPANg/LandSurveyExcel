using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExcelAddIn1.basicMath;

namespace ExcelAddIn1
{
    class circleMath
    {

        static public Tuple<basicMath.planePoint,double> thridPointCircle(basicMath.planePoint p1, basicMath.planePoint p2,basicMath.planePoint p3)
        {
            double x1,x2,x3,y1,y2,y3;
            planePoint xy;
            double radius;
            x1 = p1.E;
            x2 = p2.E;
            x3 = p3.E;
            y1 = p1.N;
            y2 = p2.N;
            y3 = p3.N;
            try
            {
                xy.E = ((y2 - y1) * (y3 * y3 - y1 * y1 + x3 * x3 - x1 * x1) - (y3 - y1) * (y2 * y2 - y1 * y1 + x2 * x2 - x1 * x1)) / (2.0 * ((x3 - x1) * (y2 - y1) - (x2 - x1) * (y3 - y1)));

                xy.N = ((x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1) - (x3 - x1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)) / (2.0 * ((y3 - y1) * (x2 - x1) - (y2 - y1) * (x3 - x1)));

                radius = (x1 - xy.E) * (x1 - xy.E + (y1 - xy.N) * (y1 - xy.N));

            }
            catch (Exception)
            {

                xy.E = Double.NegativeInfinity;
                xy.N = Double.NegativeInfinity;
                radius = Double.NegativeInfinity;
            }

            return Tuple.Create(xy,radius);
        }
        static public double checkThridPointQuality(basicMath.planePoint p1, basicMath.planePoint p2, basicMath.planePoint p3)
        {
            Tuple<basicMath.planePoint, double> circleP = thridPointCircle(p1, p2, p3);
            if (circleP.Item2 == Double.NegativeInfinity) return Double.NegativeInfinity;
            double angleP1, angleP2, angleP3;
            angleP1 = Math.Abs(basicMath.ReturnBearingHD(circleP.Item1, p1).Bearing);
            angleP2 = Math.Abs(basicMath.ReturnBearingHD(circleP.Item1, p2).Bearing);
            angleP3 = Math.Abs(basicMath.ReturnBearingHD(circleP.Item1, p3).Bearing);
            if(angleP1>angleP2 || angleP1 > angleP3)
            {
                return angleP2 * angleP3;
            }

            if (angleP2 > angleP1 || angleP2 > angleP3)
            {
                return angleP1 * angleP3;
            }
            else
            {
                return angleP1 * angleP2;
            }
        }
        static List<basicMath.planePoint> twoPointRadiusTwoCircle(basicMath.planePoint p1, basicMath.planePoint p2, double radius)
        {
            List<basicMath.planePoint> twoCircle = new List<planePoint> ();
            
            if (p1.N==p2.N&&p1.E==p2.E && radius <= 0)
            {
                twoCircle.Add(p1);
                return twoCircle; 
            }
            else if(basicMath.returnHD(p1,p2)>=radius)
            {
                basicMath.BearingHD middleBearingHD = ReturnBearingHD(p1, p2);
                middleBearingHD.HD /= 2;
                twoCircle.Add(basicMath.polarFlat(p1, middleBearingHD));
                return twoCircle;
            }
            else
            {
                basicMath.BearingHD middleBearingHD = ReturnBearingHD(p1, p2);
                middleBearingHD.HD /= 2;
                basicMath.planePoint middlePoint = basicMath.polarFlat(p1, middleBearingHD);
                middleBearingHD.HD = Math.Pow(radius * radius - middleBearingHD.HD * middleBearingHD.HD, 2);
                middleBearingHD.Bearing +=0.5 * Math.PI;
                twoCircle.Add(polarFlat(middlePoint, middleBearingHD));
                middleBearingHD.Bearing -= Math.PI;
                twoCircle.Add(polarFlat(middlePoint, middleBearingHD));
                return twoCircle;
            }
        }
        /*
        static public Tuple<basicMath.planePoint, double> placeCircle(double radius, basicMath.planePoint p1, basicMath.planePoint p2, basicMath.planePoint p3)
        {
            basicMath.planePoint circleCenter;
            circleCenter.N = (p1.N+p2.N+p3.N)/3;
            circleCenter.E= (p1.E + p2.E + p3.E) / 3;
            for (int i = 0; i < 1000; i++)
            {
                double distanceP1, distanceP2, distanceP3;
                distanceP1 = basicMath.returnHD(circleCenter, p1);
                distanceP2 = basicMath.returnHD(circleCenter, p2);
                distanceP3 = basicMath.returnHD(circleCenter, p3);
                circleCenter.N += 0.1*(circleCenter.N-p1.)
            }
        }
        */

    }
    
}
