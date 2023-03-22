using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace ExcelAddIn1
{
    public class GenlayForm
    {
        public static void OutInputForm(Excel.Worksheet sheet, closedTraverse traverse)
        {

            Range sheetRange = sheet.UsedRange.Cells;
            int rowCount = sheetRange.Rows.Count;
            int colCount = sheetRange.Columns.Count;

            string basePath = @"D:\";
            int fileNumber = 1;

            while (File.Exists(Path.Combine(basePath, $"T{fileNumber}.TRA")))
            {
                fileNumber++;
            }

            string newFileName = $"T{fileNumber}.TRA";
            string newFilePath = Path.Combine(basePath, newFileName);


            string[] sections = new string[]
        {
            "**1 to 6 is ADMIN. information**",
            "**10 to 17 is CONTROL information**",
            "**20 to 22 is start orientation**",
            "**23 to 25 is closing orientation**",
            "**26 to 90 is intermediate observation**"
        };

            int[][] rowRanges = new int[][]
            {
            new int[] { 1, 6 },
            new int[] { 10, 17 },
            new int[] { 20, 22 },
            new int[] { 23, 25 },
            new int[] { 26, 90 }
            };

            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                for (int i = 0; i < sections.Length; i++)
                {
                    sw.WriteLine(sections[i]);

                    for (int row = rowRanges[i][0]; row <= rowRanges[i][1]; row++)
                    {
                        if(row==10)
                        {
                            sw.WriteLine($"{row},"+traverse.startNode.name + "," + Math.Round(traverse.startNode.Northing,3).ToString() + ","+ Math.Round(traverse.startNode.Easting, 3).ToString());
                            continue;
                        }
                        if(row==11)
                        {
                            sw.WriteLine($"{row}," + traverse.endNode.name + "," + traverse.endNode.Northing.ToString() + "," + traverse.endNode.Easting.ToString());
                            continue;

                        }
                        if(row>=12&&row<12+traverse.startRefnodes.Count&&row<15)
                        {
                            sw.WriteLine($"{row}," + traverse.startRefnodes[row-12].Item1.name+ "," + traverse.startRefnodes[row - 12].Item1.Northing.ToString() + "," + traverse.startRefnodes[row - 12].Item1.Easting.ToString());
                            continue;
                        }
                        if (row >= 15 && row < 15 + traverse.endRefnodes.Count && row < 18)
                        {
                            sw.WriteLine($"{row}," + traverse.endRefnodes[row - 15].Item1.name + "," + traverse.endRefnodes[row - 15].Item1.Northing.ToString() + "," + traverse.endRefnodes[row - 15].Item1.Easting.ToString());
                            continue;
                        }
                        if (row >= 20 && row < 20 + traverse.startRefnodes.Count && row < 23)
                        {
                            sw.WriteLine($"{row}," + traverse.startNode.name + "," + traverse.startRefnodes[row-20].Item1.name + "," + bearingToForm(traverse.startRefnodes[row - 20].Item2.Bearing) + "," + Math.Round(traverse.startRefnodes[row-20].Item2.HD,3));
                            continue;
                        }

                        if (row >= 23 && row < 23 + traverse.endRefnodes.Count && row < 26)
                        {
                            sw.WriteLine($"{row}," + traverse.endNode.name + "," + traverse.endRefnodes[row - 23].Item1.name + "," + bearingToForm(traverse.endRefnodes[row - 23].Item2.Bearing) + "," + Math.Round(traverse.endRefnodes[row - 23].Item2.HD, 3));
                            continue;
                        }



                        else if(row >=26&&row<25+traverse.traverseList.Count)
                        {
                            sw.WriteLine($"{row}," + traverse.traverseList[row-26].Item1 + ","+traverse.traverseList[row - 25].Item1+","+bearingToForm(traverse.traverseList[row - 26].Item3.Bearing)+","+ Math.Round(traverse.traverseList[row - 26].Item3.HD,3));
                            row++;
                            sw.WriteLine($"{row}," + traverse.traverseList[row - 26].Item1 + "," + traverse.traverseList[row - 27].Item1+","+ bearingToForm(traverse.traverseList[row-26].Item2.Bearing)+",");
                            row--;
                        }
                        else if(row>=25+traverse.traverseList.Count) sw.WriteLine($"{row},,,,");
                        else sw.WriteLine($"{row},,,");
                    }
                }
            }




        }


        public static string  bearingToForm(double bearing)
        {
            bearing =180+ basicMath.normalAngle(bearing+Math.PI)*180/Math.PI;
            int angle = (int)bearing;
            int mintue = (int)((bearing - angle)*60 );
           
            int second = (int)Math.Round((bearing - angle - ((double)mintue) / 60)*3600);
            if(mintue<10 && second<10)return angle.ToString() + "." + "0" + mintue.ToString() + "0" + second.ToString();
            if(mintue < 10  ) return angle.ToString() + "." + "0" + mintue.ToString() + second.ToString();
            if (second < 10) return angle.ToString() + "." + mintue.ToString() + "0" + second.ToString();
            return angle.ToString()+"."+mintue.ToString()+second.ToString();
        }
    }
}
