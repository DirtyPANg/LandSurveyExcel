using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAddIn1
{
    class lineMath
    {
        static public basicMath.planePoint returnLinePointDist(basicMath.lineSeg Line, basicMath.planePoint point)              
        {
            basicMath.planePoint returnXandZ= new basicMath.planePoint(double.NegativeInfinity, double.NegativeInfinity);
            basicMath.BearingHD point1ToLine = basicMath.ReturnBearingHD(Line.p1, point); /*First check point 1 on line with checkingPoint*/
            point1ToLine.Bearing=  basicMath.normalAngle(point1ToLine.Bearing-Line.bearing1To2.Bearing);
            
            if(Math.Abs(point1ToLine.Bearing) <=0.5*Math.PI)
            {
                if(Math.Cos(point1ToLine.Bearing)* point1ToLine.HD<=Line.bearing1To2.HD)
                {
                    returnXandZ.N = Math.Sin(point1ToLine.Bearing) * point1ToLine.HD;/*It is in Z axis*/
                    returnXandZ.E = Math.Cos(point1ToLine.Bearing) * point1ToLine.HD;
                    return returnXandZ; /*return X ,Z axis value*/
                }
            }
            return returnXandZ;
        }


        static public basicMath.planePoint FindLineIntersection(basicMath.lineSeg line1, basicMath.lineSeg line2)
        {
            basicMath.planePoint negative =new basicMath.planePoint(double.NegativeInfinity, double.NegativeInfinity);
            double A1 = line1.p2.N - line1.p1.N;
            double B1 = line1.p1.E - line1.p2.E;
            double C1 = A1 * line1.p1.E + B1 * line1.p1.N;

            double A2 = line2.p2.N - line2.p1.N;
            double B2 = line2.p1.E - line2.p2.E;
            double C2 = A2 * line2.p1.E + B2 * line2.p1.N;

            double determinant = A1 * B2 - A2 * B1;

            if (determinant == 0)   
            {
                // 线段平行，无交点
                return negative;
            }
            else
            {
                double intersectE = (B2 * C1 - B1 * C2) / determinant;
                double intersectN = (A1 * C2 - A2 * C1) / determinant;

                basicMath.planePoint intersection = new basicMath.planePoint(intersectN, intersectE);

                // 检查交点是否在两个线段上
                if (IsPointOnLineSegment(line1, intersection) && IsPointOnLineSegment(line2, intersection))
                {

                    return intersection;
                }
                else
                {
                    return negative;
                }
            }
        }

        static private bool IsPointOnLineSegment(basicMath.lineSeg line, basicMath.planePoint point)
        {
            double minX = Math.Min(line.p1.E, line.p2.E);
            double maxX = Math.Max(line.p1.E, line.p2.E);
            double minY = Math.Min(line.p1.N, line.p2.N);
            double maxY = Math.Max(line.p1.N, line.p2.N);

            return point.E >= minX && point.E <= maxX && point.N >= minY && point.N <= maxY;
        }
        
    }
}
