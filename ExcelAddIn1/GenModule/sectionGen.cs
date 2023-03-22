using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelAddIn1
{
    class sectionGen
    {
        static public List<basicMath.line3Dseg> readSection(List<List<string>> sectionInput)
        {
            List<basicMath.line3Dseg> returnList = new List<basicMath.line3Dseg>();
            for(int i=1;i<sectionInput.Count; i+=2)
            {
                if (i + 1 > sectionInput.Count) break;
                if (sectionInput[i].Count < 3 && sectionInput[i+1].Count<3) continue;
                try
                {
                    basicMath.pointData p1 = new basicMath.pointData();
                    basicMath.pointData p2 = new basicMath.pointData();
                    p1.point.E = Double.Parse(sectionInput[i][2]);
                    p1.point.N= Double.Parse(sectionInput[i][3]);
                    p2.point.E = Double.Parse(sectionInput[i+1][2]);
                    p2.point.N = Double.Parse(sectionInput[i+1][3]);
                    p1.id = sectionInput[i][1];
                    p1.label = sectionInput[i][1];
                    p1.level = double.NegativeInfinity; p2.level = double.PositiveInfinity;
                    p1.id = sectionInput[i+1][1];
                    p1.label = sectionInput[i + 1][1];
                    returnList.Add(new basicMath.line3Dseg(p1,p2));
                }
                catch (Exception)
                {
                    continue;
                }
                

            }
            return returnList;
        }
        static public List<List<basicMath.pointData>> returnSectionLineIntersect(List<basicMath.line3Dseg> sectionList,List<basicMath.line3Dseg> regisLine)
        {
            List<List<basicMath.pointData>> returnList = new List<List<basicMath.pointData>>();
            foreach (var sectionLine in sectionList)
            {
                basicMath.lineSeg planeLine = new basicMath.lineSeg(sectionLine.p1.point, sectionLine.p2.point);
                List<basicMath.pointData> intersectPoint = new List<basicMath.pointData>();
                foreach(var regLine in regisLine)
                {
                    var result = SectionMath.TwoLineGenPoint(planeLine, regLine);
                    if (result.point.N == double.NegativeInfinity) continue;
                    else intersectPoint.Add(result);
                }
                returnList.Add(intersectPoint);
            }
            return returnList;
        }
    }
}
