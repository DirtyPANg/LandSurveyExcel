using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelAddIn1
{
    class ASCImport
    {
        public List<StationData> stationDataset;
        public ASCImport()
        {
            stationDataset = new List<StationData>();
        }
        public bool importFile(OpenFileDialog filePath)
        {
            var fileStream = filePath.OpenFile();
            List<string> ascText=new List<string>();
            StationData stationData = new StationData();
            stationData.measureList = new List<ascLine>();
            bool onMeasuing = false;
            try 
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ascText.Add(line);

                    }
                }
            } 
            catch
            {
                return false;
            }
            for(int i =0;i<ascText.Count;i++)
            {
                if (onMeasuing == false && ascText.Count-i>2)
                {
                    stationData = importStationData(ascText[i], ascText[i+1], ascText[i+2]);
                    if (stationData.isTrue == false) continue;
                    onMeasuing= true;
                    i += 2;
                }
                else if(onMeasuing == true&& stationData.isTrue==true)
                {
                    ascLine singleLine = importLineData(ascText[i],stationData);
                    onMeasuing = singleLine.isTrue;
                    if (onMeasuing == false )
                    {
                        i--;
                        stationDataset.Add(stationData);
                        continue;
                    }
                    stationData.measureList.Add(singleLine);
                    if(i==ascText.Count-1)stationDataset.Add(stationData);
                }

            }
            return true;
        }






        static StationData importStationData(string firstLine,string secondLine,string thridLine)
        {
            string pattern = @"\S+";
            string linePattern = @"\+0*([A-Za-z\d]+)";
            StationData data = new StationData();
            MatchCollection matche1 = Regex.Matches(firstLine, pattern);
            MatchCollection matche2 = Regex.Matches(secondLine, pattern);
            MatchCollection matche3 = Regex.Matches(thridLine, pattern);
            data.measureList = new List<ascLine>();
            if (matche1.Count !=7 && matche2.Count !=5 && matche3.Count !=7)
            {
                data.isTrue = false;
                return data;
            }
            try
            {
                data.stationName = Regex.Match(matche2[1].Groups[0].Value, linePattern).Groups[1].Value.ToString();
                data.backSignName = Regex.Match(matche2[4].Groups[0].Value, linePattern).Groups[1].Value.ToString();
                double.TryParse(Regex.Match(matche2[2].Groups[0].Value, linePattern).Groups[1].Value, out data.stationHeight);
                double.TryParse(Regex.Match(matche2[3].Groups[0].Value, linePattern).Groups[1].Value, out data.backSignHeight);
                double.TryParse(Regex.Match(matche3[1].Groups[0].Value, linePattern).Groups[1].Value, out data.firstBearing);
            }
            catch(Exception e)
            {
                data.isTrue = false;
            }
            data.firstBearing = ascBearingToNorm(data.firstBearing);
            data.stationHeight /= 1000d;
            data.backSignHeight /= 1000d;
            data.isTrue = true;
            return data;

        }
        static ascLine importLineData(string input,StationData stationInfo)
        {
            string pattern = @"\S+";
            string linePattern = @"\+0*([A-Za-z\d]+)";
            ascLine data= new ascLine();
            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches.Count != 10)
            {
                data.isTrue = false;
                return data;
            }
            try
            {
                data.id = Regex.Match(matches[0].Groups[0].Value, linePattern).Groups[1].Value.ToString();
                data.label = Regex.Match(matches[9].Groups[0].Value, linePattern).Groups[1].Value.ToString();

                 double.TryParse(Regex.Match(matches[4].Groups[0].Value, @"\+(\d+)$").Groups[1].Value,out data.psmValue);
                 double.TryParse(Regex.Match(matches[1].Groups[0].Value, linePattern).Groups[1].Value, out data.bearing);
                 double.TryParse(Regex.Match(matches[2].Groups[0].Value, linePattern).Groups[1].Value, out data.verticalAngle);
                 double.TryParse(Regex.Match(matches[3].Groups[0].Value, linePattern).Groups[1].Value, out data.slopeDistance);
                 double.TryParse(Regex.Match(matches[5].Groups[0].Value, linePattern).Groups[1].Value, out data.targetHeight);
                 double.TryParse(Regex.Match(matches[7].Groups[0].Value, linePattern).Groups[1].Value, out data.Northing);
                 double.TryParse(Regex.Match(matches[6].Groups[0].Value, linePattern).Groups[1].Value, out data.Easting);
                 double.TryParse(Regex.Match(matches[8].Groups[0].Value, linePattern).Groups[1].Value, out data.measureLevel);
            }
            catch {
                data.isTrue = false;
                return data;
            }
            data.psmValue /= 1000d;
            data.slopeDistance /= 1000d;
            data.bearing =ascBearingToNorm(data.bearing);
            data.verticalAngle= ascBearingToNorm(data.verticalAngle);
            data.Northing = 800000+data.Northing / 1000d;
            data.Easting = 800000 + data.Easting / 1000d;
            data.HD = basicMath.slopeToHD(data.slopeDistance,data.verticalAngle,data.psmValue);
            data.VD = basicMath.slopeToVD(data.slopeDistance, data.verticalAngle,stationInfo.stationHeight ,stationInfo.backSignHeight,data.psmValue);
            data.isTrue = true;
            return data;
        }
        static double ascBearingToNorm(double ascBearing)
        {
            double degree = (int)(ascBearing/100000);
            double minutes = (int)((ascBearing-degree*100000) / 1000);
            double seconds = ascBearing-degree*100000 -minutes*1000;
            return basicMath.normalAngle(degree*Math.PI/180+minutes * Math.PI / (180*60) + seconds * Math.PI / (180 * 36000));
        }
           
    }
    public struct StationData
    {
        public bool isTrue;
        public string stationName;
        public double stationHeight;
        public string backSignName;
        public double backSignHeight;
        public double firstBearing;
        public List<ascLine> measureList;
    }
    public struct ascLine
    {
        public bool isTrue;
        public string id;
        public string label;
        public double psmValue;
        public double bearing;
        public double verticalAngle;
        public double slopeDistance;
        public double targetHeight;
        public double Northing;
        public double Easting;
        public double measureLevel;
        public double HD;
        public double VD;
    }
    
}
