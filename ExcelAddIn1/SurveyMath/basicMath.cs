using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;

namespace ExcelAddIn1
{
    public class basicMath
    {
        public struct BearingHD
        {
            public double Bearing;
            public double HD;
            public BearingHD(double Bearing,double HD)
            {
                this.HD = HD;
                this.Bearing = Bearing;
            }
        }

        public struct planePoint
        {
            public double N;
            public double E;
            public planePoint(double N,double E)
                { this.N = N; this.E = E; } 
            static public planePoint create(double N, double E)
            {
                planePoint returnValue = new planePoint(N,E);
                return returnValue;
            }
        }
        public struct pointData
        {
            public planePoint point;
            public double level;
            public string id;
            public string label;
            public pointData(double N, double E,double level,string id,string label)
            {
                this.level = level;
                this.id = id;
                this.label = label;
                point= new planePoint(N,E);
            }

        }
        public struct fixpoint
        {
            public string name;
            public double Northing;
            public double Easting;
            public double level;
        }
        
        public struct lineSeg
        {
            public planePoint p1,p2;
            public BearingHD bearing1To2;
            public lineSeg(planePoint point1, planePoint point2)
            {
                p1= point1;
                p2= point2;
                bearing1To2 = ReturnBearingHD(p1,p2);
            }
        }
        public struct line3Dseg
        {
            public pointData p1,p2;
            public BearingHD bearing1To2;
            public line3Dseg(pointData point1, pointData point2)
            {
                p1 = point1;
                p2 = point2;
                bearing1To2 = ReturnBearingHD(p1.point, p2.point);
            }
        }
        static public double normalAngle(double angle)
        {
            return (angle+Math.PI)%(2*Math.PI)-Math.PI;
        }
       
        static public double returnHD(planePoint station, planePoint target)
        {
            return Math.Sqrt(Math.Pow(station.E - target.E, 2) + Math.Pow(station.N - target.N, 2));
        }
        static public double returnHD(fixpoint station, fixpoint target)
        {
            return Math.Sqrt(Math.Pow(station.Easting - target.Easting, 2) + Math.Pow(station.Northing - target.Northing, 2));
        }
        static public double slopeToHD(double slopeDistance, double vertialBearing, double psm)/*HD can be negative*/
        {
            return Math.Sin(vertialBearing) * (slopeDistance );
        }
        static public double slopeToVD(double slopeDistance, double vertialBearing,double stationHeight,double targetHeight, double psm)
        {
            return Math.Cos(vertialBearing) * (slopeDistance )+stationHeight-targetHeight;
        }
        static public BearingHD ReturnBearingHD(planePoint station, planePoint target)
        {
            BearingHD bearingHD;
            bearingHD.HD = Math.Sqrt(Math.Pow(station.E- target.E, 2)+ Math.Pow(station.N - target.N, 2));
            if (station.E < target.E)
            {
                bearingHD.Bearing = normalAngle(Math.Acos((target.N- station.N ) / (bearingHD.HD)));
            }
            else
            {
                bearingHD.Bearing = normalAngle(2 * Math.PI - Math.Acos((target.N - station.N)  / (bearingHD.HD )));
            }
            return bearingHD;
        }
        static public BearingHD ReturnBearingHD(fixpoint fp1,fixpoint fp2)
        {
            planePoint p1 = new planePoint(fp1.Northing,fp2.Easting);
            planePoint p2 = new planePoint(fp2.Northing, fp2.Easting);
            return ReturnBearingHD(p1,p2);
        }
        static public planePoint polarFlat(planePoint station,BearingHD BHD)
        {
            planePoint newlocation = new planePoint();
            newlocation.N = station.N+Math.Cos(BHD.Bearing)*BHD.HD;
            newlocation.E = station.E + Math.Sin(BHD.Bearing) * BHD.HD;
            return newlocation;
        }
        static public int vTopoFind(List<pointData> topoList, string findingValue,int defaultFind)
        {
            if(defaultFind>=topoList.Count ||defaultFind<=0)defaultFind = 1;
            for (int i = defaultFind; i >= 0; i--)
            {
                string code = topoList[i].id;
                if (code == findingValue)
                {
                    return i;
                }
            }
            for (int i = defaultFind+1; i < topoList.Count; i++)
            {
                string code = topoList[i].id;
                if (code == findingValue)
                {
                    return i;
                }
            }
            return -1; /* -1 is unfind */
        }
        
    }
}
