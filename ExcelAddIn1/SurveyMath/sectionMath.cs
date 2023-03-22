using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExcelAddIn1.basicMath;

namespace ExcelAddIn1
{
    class SectionMath
    {
        public static pointData TwoLineGenPoint(lineSeg inputLineSeg, line3Dseg dataLine)
        {
            // Create a new line segment using point1 and point2
            lineSeg secondLineSeg = new lineSeg(dataLine.p1.point, dataLine.p2.point);

            // Find the intersection between inputLineSeg and secondLineSeg
            planePoint intersection = lineMath.FindLineIntersection(inputLineSeg, secondLineSeg);

            if (intersection.N !=double.NegativeInfinity)
            {
                // Calculate the distance from the first point of inputLineSeg to the intersection point
                double distanceE = basicMath.returnHD(inputLineSeg.p1, intersection);

                // Calculate the distance from the first point of secondLineSeg to the intersection point
                double distanceN = basicMath.returnHD(dataLine.p1.point, intersection);

                // Estimate the level (height) based on the level difference of point1 and point2
                double levelDifference = dataLine.p2.level - dataLine.p1.level;
                double ratio;
                if (basicMath.returnHD(dataLine.p1.point, dataLine.p2.point) == 0)  ratio = 0;
                else  ratio = distanceN / basicMath.returnHD(dataLine.p1.point, dataLine.p2.point);
                double estimatedLevel = dataLine.p1.level + (ratio * levelDifference);

                // Create and return the pointData with calculated values
                return new pointData(estimatedLevel, distanceE, distanceN, dataLine.p1.id+dataLine.p1.label+dataLine.p2.id, dataLine.p1.label);
            }
            else
            {
                // Return null if there is no intersection
                return new pointData(intersection.N, intersection.E, double.NegativeInfinity, "", ""); ;
            }
        }
    }
}