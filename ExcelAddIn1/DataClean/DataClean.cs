using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using static ExcelAddIn1.basicMath;
using System.Windows.Forms;

namespace ExcelAddIn1
{
    class DataClean
    {
        static public List<basicMath.pointData> importTOPO(Excel.Worksheet topoSheet,out int topoOff)
        {
            Range topoRange = topoSheet.UsedRange.Cells;

            int rowCount = topoRange.Rows.Count;
            int colCount = topoRange.Columns.Count;
            List<pointData> topoList = new List<pointData>();
            int offset = 0, offsetCount = 0, nullCount = 0;
            if (colCount < 5)
            {
                topoOff = -1;
                return topoList;
            }
                for (int i = 0; i < rowCount; i++)
            {
                if (nullCount > 2000) break;
                
                pointData single = new pointData();
                try
                {
                    single.id = topoRange[1][i + 1].Value.ToString();
                    single.label = topoRange[5][i + 1].Value.ToString();

                }
                catch (Exception)
                {
                    single.id = "";
                    single.label = "";
                }
                try
                {
                    single.point.N = Double.Parse(topoRange[2][i + 1].Value.ToString());
                    single.point.E = Double.Parse(topoRange[3][i + 1].Value.ToString());
                    single.level = Double.Parse(topoRange[4][i + 1].Value.ToString());
                }
                catch (Exception)
                {
                    nullCount++;
                    break;
                }
                topoList.Add(single);
                int singleOffset;
                if (int.TryParse(single.id, out singleOffset) == true)
                {
                    singleOffset -= topoList.Count;
                    if (singleOffset == offset)
                    {
                        offsetCount++;
                    }
                    else
                    {
                        offsetCount--;
                        if (offsetCount < 0) offset = singleOffset;
                    }

                }

            }
            topoOff = offset;
            return topoList;
        }

        static public List<List<string>> sheetToString(Excel.Worksheet sheet)
        {

            Range sheetRange = sheet.UsedRange.Cells;
            int rowCount = sheetRange.Rows.Count;
            int colCount = sheetRange.Columns.Count;
            List<List<string>> stringList= new List<List<string>>();
            int nullCount = 0;
            for(int i =0;i< rowCount; i++)
            {
                int nullWordCount = 0;
                List<string> singleLine= new List<string>(colCount);
                for (int ii = 0; ii < colCount; ii++)
                {
                    string s;
                    try
                    {
                        s = sheetRange[ii + 1][i + 1].Value.ToString() ;
                    }
                    catch (Exception)
                    {
                        s = "";
                        nullWordCount++;
                    }
                    singleLine.Add(s);
                }
                if (nullWordCount < 10)
                {
                    stringList.Add(singleLine);
                    nullCount++;
                }
                else break;
            }
            return stringList;
            
        }
        static public List<fixpoint> fixpointRead(Excel.Worksheet sheet)
        {
            List<List<string>> readFix = sheetToString(sheet);
            List<fixpoint> returnList = new List<fixpoint>();
            foreach (var line in readFix)
            {
                if (line.Count < 3) continue;
                if (line[0] != "" && line[1] != "" && line[2] != "" )
                {
                    string name = line[0];
                    double northing;
                    double easting;
                    double level=-99.9;
                    if(double.TryParse(line[1], out northing) && double.TryParse(line[2], out easting))
                    {
                        if (line.Count>3&&line[3] != "" )
                        {
                            double.TryParse(line[3], out level);
                        }
                        fixpoint newFix = new fixpoint();
                        newFix.name = name;
                        newFix.Northing = northing;
                        newFix.Easting = easting;
                        newFix.level = level;
                        returnList.Add(newFix);

                    }
                }
            }
            return returnList;
        }

    }

}
