using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ExcelAddIn1.basicMath;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelAddIn1
{

    class triangleGen
    {
        static public List<List<basicMath.planePoint>> rightAngleGen(List<basicMath.pointData> topoList, List<List<string>> LXData, List<List<string>> LayerData, List<List<string>> blockData, List<List<string>> settingData,int topoOffset)
        {
            List<List<basicMath.planePoint>> enableLX3 = new List<List<basicMath.planePoint>>();
            List<string> enableCode = new List<string>();
            List<int> enableLX4 = new List<int>();

            for (int i = 0; i < blockData.Count; i++)
            {
                string codeTable = blockData[i][0];

                string value = blockData[i][3];
                if (codeTable!=null &&codeTable!= "" && value == "Y")
                {
                    enableCode.Add(codeTable);
                }
            }

            if (enableCode.Count > 0)
            {
                for (int i = 0; i < LXData.Count; i++)
                {
                    int firstValue = 0; 
                    int secondValue = 0;
                    if (Int32.TryParse(LXData[i][1], out firstValue) && Int32.TryParse(LXData[i][2], out secondValue) && secondValue - firstValue == 2 )
                    {
                        int firstFind = basicMath.vTopoFind(topoList, firstValue.ToString(), firstValue - topoOffset);
                        if(firstFind != -1 && enableCode.Contains(topoList[firstFind].label)) /* -1 is unfind */
                        {
                            basicMath.pointData first, second, third;

                            first = topoList[firstFind];
                            int secondFind = basicMath.vTopoFind(topoList, (firstValue + 1).ToString(), firstValue + 1 - topoOffset);
                            if (secondFind != -1) second =topoList[secondFind];
                            else continue;
                            int thirdFind = basicMath.vTopoFind(topoList, (firstValue + 2).ToString(), firstValue + 2 - topoOffset);
                            if (thirdFind != -1) third = topoList[thirdFind];
                            else continue;


                            BearingHD sfBearingHD = ReturnBearingHD(second.point, first.point);
                            BearingHD stBearingHD = ReturnBearingHD(second.point, third.point);
                            List<basicMath.planePoint> lineList = new List<basicMath.planePoint>(); ;
                            double absoAngle = Math.Abs(basicMath.normalAngle(sfBearingHD.Bearing - stBearingHD.Bearing));
                            if (absoAngle <= Math.PI * 0.5 * 1.08 && absoAngle >= Math.PI * 0.5 * 0.92)
                            {
                                lineList.Add(first.point);
                                lineList.Add(polarFlat(first.point,stBearingHD));
                                lineList.Add(third.point);
                            }
                            if (lineList.Count != 0)
                            {
                                enableLX3.Add(lineList);
                            }
                        }
                    }
                    else if (secondValue - firstValue == 3)
                    {

                    }
                }
            }
            return enableLX3;
            
        }





    }
}
