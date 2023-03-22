using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using static ExcelAddIn1.basicMath;
using System.Windows.Forms;

namespace ExcelAddIn1
{
    class SCR
    {
        static public void pointToSheet(Excel.Worksheet sheet, List<pointData> outputData)
        {

            Range sheetRange = sheet.UsedRange.Cells;
            int rowCount = sheetRange.Rows.Count;
            int colCount = sheetRange.Columns.Count;
            sheetRange.Cells[rowCount , 1] = "layer";
            sheetRange.Cells[rowCount , 2] = "set";
            sheetRange.Cells[rowCount , 3] = "description";
            for (int i = 0; i < outputData.Count; i++)
            {
                
                sheetRange.Cells[rowCount + 4 * i + 1, 1] = "text";
                sheetRange.Cells[rowCount + 4 * i + 1, 2] = Math.Round(outputData[i].point.E,3).ToString("F3") + "," + Math.Round(outputData[i].point.N,3).ToString("F3");
                sheetRange.Cells[rowCount + 4 * i + 1, 3] = "0.1";
                sheetRange.Cells[rowCount + 4 * i + 1, 4] = "0";
                sheetRange.Cells[rowCount + 4 * i + 1, 5] = outputData[i].id;


                

                /*sheetRange.Cells[rowCount + 4 * i + 2, 1] = "layer";
                sheetRange.Cells[rowCount + 4 * i + 2, 2] = "set";
                sheetRange.Cells[rowCount + 4 * i + 2, 3] = outputData[i].label;
                sheetRange.Cells[rowCount + 4 * i + 3, 1] = "insert";
                sheetRange.Cells[rowCount + 4 * i + 3, 2] = "cir";
                sheetRange.Cells[rowCount + 4 * i + 3, 3] = Math.Round(outputData[i].point.E,3).ToString("F3") + "," + Math.Round(outputData[i].point.N,3).ToString("F3");
                sheetRange.Cells[rowCount + 4 * i + 3, 4] = "1 1 0";
                sheetRange.Cells[rowCount + 4 * i + 3, 5] = outputData[i].level.ToString();*/

            }
        }
        static public void plineToSheet(Excel.Worksheet sheet,List<List<basicMath.planePoint>> lineData)
        {
            Range sheetRange = sheet.UsedRange.Cells;
            int rowCount = sheetRange.Rows.Count;
            for (int i = 0; i < lineData.Count; i++)
            {
                sheetRange.Cells[rowCount + 3 * i , 1] = "Layer";
                sheetRange.Cells[rowCount + 3 * i , 2] = "set";
                sheetRange.Cells[rowCount + 3 * i , 3] = "ptno";
                sheetRange.Cells[rowCount + 3 * i + 1, 1] = "pline";
                for (int ii = 0; ii < lineData[i].Count; ii++)
                {
                    sheetRange.Cells[rowCount + 3 * i + 1, ii + 2] =  Math.Round(lineData[i][ii].E,3).ToString("F3") + "," + Math.Round(lineData[i][ii].N,3).ToString("F3");
                }
                sheetRange.Cells[rowCount + 3 * i +2, 1] = "";
            }

        }

    }
}
