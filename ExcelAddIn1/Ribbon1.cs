using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Tools.Excel;
using System.IO;
using System.Xml.Linq;

namespace ExcelAddIn1
{
    public partial class Ribbon1
    {

        private System.Windows.Forms.OpenFileDialog importFile;
        private bool sectionUseAutoGen = true;
        ASCImport ASCData= new ASCImport();
        public List<List<string>> loadCsvFile(string filePath)
        {
            StreamReader reader;
            List<List<string>> searchList = new List<List<string>>();
            try
            {
                reader = new StreamReader(File.OpenRead(filePath));

            }
            catch (Exception)
            {
                return searchList;
                throw;
            }
            while (!reader.EndOfStream)
            {
                List<string> singleVar = new List<string> { "","","","",""};
                var line = reader.ReadLine();
                int num = 0;
                for(int i = 0; i < line.Length; i++)
                {
                    if(line[i] !=',')
                    {
                        singleVar[num] += line[i];
                    }
                    else if(num<4)
                    {
                        num++;
                    }
                }
                searchList.Add(singleVar);
            }
            return searchList;
        }
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
             var activeApp = Globals.ThisAddIn.GetActiveApp() ;
            var activeWB = Globals.ThisAddIn.GetActiveWB();
            activeWB.Sheets["TOPO"].Activate();
            var currentSheet = Globals.ThisAddIn.GetActiveWorkSheet() ;
            this.importFile = new System.Windows.Forms.OpenFileDialog();
            this.importFile.InitialDirectory = activeWB.Path;
            this.importFile.Title = "Open ID,North,Easting,Level,Remark Data File";
            importFile.ShowDialog();
            if(this.importFile.CheckPathExists)
            {
                /*currentSheet.Cells.Clear();*/
                List<List<string>> csvData = loadCsvFile(this.importFile.FileName);
                if (csvData.Count == 0) return;
                object[,] dataField = new object[csvData.Count, 5];
                for(int i = 0; i <csvData.Count; i++)
                {
                    var row = csvData[i];
                    for(int ii=0;ii<5;ii++)
                    {
                        dataField[i,ii] = row[ii];
                    }
                }
                Range rng = currentSheet.Range["A1", "E" + csvData.Count.ToString()];
                rng.Value= dataField;
            }

            
        }

        private void AutoGenBox_Click(object sender, RibbonControlEventArgs e)
        {
        }

        private void Run_Click(object sender, RibbonControlEventArgs e)
        {
            Excel.Worksheet topoSheet, UCSheet, LXSheet,  LayerSheet,  blockSheet, genSettingSheet, autoGenSheet;
            try
            {
                topoSheet = Globals.ThisAddIn.GetWorksheet("TOPO");
                UCSheet = Globals.ThisAddIn.GetWorksheet("UC");
                LXSheet = Globals.ThisAddIn.GetWorksheet("LX");
                LayerSheet = Globals.ThisAddIn.GetWorksheet("Layer");
                blockSheet = Globals.ThisAddIn.GetWorksheet("Block");
                genSettingSheet = Globals.ThisAddIn.GetWorksheet("Gen Setting");
                autoGenSheet = Globals.ThisAddIn.GetWorksheet("AutoGen");

            }
            catch (Exception)
            {
                return;
                throw;
            }
            int topoOffset = 0;
            List<List<string>> UCData, LXData, LayerData,blockData, settingData;
            List<basicMath.pointData> topoData = DataClean.importTOPO(topoSheet,out topoOffset);
            UCData = DataClean.sheetToString(UCSheet);
            LXData = DataClean.sheetToString(LXSheet);
            LayerData = DataClean.sheetToString(LayerSheet);
            blockData = DataClean.sheetToString(blockSheet);
            settingData = DataClean.sheetToString(genSettingSheet);
            autoGenSheet = Globals.ThisAddIn.GetWorksheet("AutoGen");
            List<List<basicMath.planePoint>> genRAData;
            genRAData = triangleGen.rightAngleGen(topoData, LXData, LayerData, blockData,settingData, topoOffset);
            autoGenSheet.Cells.Clear();
            SCR.plineToSheet(autoGenSheet, genRAData);
        }

        private void button2_Click_1(object sender, RibbonControlEventArgs e)
        {
            Excel.Worksheet topoSheet, UCSheet, LXSheet, LayerSheet, blockSheet, genSettingSheet,autoGenSheet,sectionSheet;
            try
            {
                topoSheet = Globals.ThisAddIn.GetWorksheet("TOPO");
                UCSheet = Globals.ThisAddIn.GetWorksheet("UC");
                LXSheet = Globals.ThisAddIn.GetWorksheet("LX");
                LayerSheet = Globals.ThisAddIn.GetWorksheet("Layer");
                blockSheet = Globals.ThisAddIn.GetWorksheet("Block");
                genSettingSheet = Globals.ThisAddIn.GetWorksheet("Gen Setting");
                autoGenSheet = Globals.ThisAddIn.GetWorksheet("AutoGen");
                sectionSheet = Globals.ThisAddIn.GetWorksheet("Section Sheet");
            }
            catch (Exception)
            {
                return;
                throw;
            }
            int topoOffset = 0;
            List<List<string>> UCData, LXData, LayerData,blockData, settingData,sectionData;
            List<basicMath.pointData> topoData = DataClean.importTOPO(topoSheet, out topoOffset);
            UCData = DataClean.sheetToString(UCSheet);
            LXData = DataClean.sheetToString(LXSheet);
            LayerData = DataClean.sheetToString(LayerSheet);
            blockData = DataClean.sheetToString(blockSheet);
            settingData = DataClean.sheetToString(genSettingSheet);
            sectionData = DataClean.sheetToString(sectionSheet);
            autoGenSheet.Cells.Clear();
            List<basicMath.line3Dseg> regisLine =  LineRegister.lineRegister(topoData,LXData,topoOffset);
            List<basicMath.line3Dseg> sectionList = sectionGen.readSection(sectionData);
            List<List<basicMath.pointData>> cutsectionPoint = sectionGen.returnSectionLineIntersect(sectionList,regisLine);
            SCR.pointToSheet(autoGenSheet,cutsectionPoint[0]);
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            var activeApp = Globals.ThisAddIn.GetActiveApp();
            var activeWB = Globals.ThisAddIn.GetActiveWB();
            var currentSheet = Globals.ThisAddIn.GetActiveWorkSheet();
            this.importFile = new System.Windows.Forms.OpenFileDialog();
            this.importFile.InitialDirectory = activeWB.Path;
            this.importFile.Title = "Open ASC File for leica total station";
            importFile.ShowDialog();
            if(importFile.CheckFileExists)
            ASCData.importFile(importFile);
            List<basicMath.fixpoint> fixpoints= new List<basicMath.fixpoint>();
            basicMath.fixpoint f1 = new basicMath.fixpoint();
            f1.name = "N5";
            basicMath.fixpoint f2 = new basicMath.fixpoint();
            f2.name = "N6";
            fixpoints.Add(f1);
            fixpoints.Add(f2);
        }

        private void button3_Click(object sender, RibbonControlEventArgs e)
        {
            var activeApp = Globals.ThisAddIn.GetActiveApp();
            var activeWB = Globals.ThisAddIn.GetActiveWB();
            var currentSheet = Globals.ThisAddIn.GetActiveWorkSheet();
            Excel.Worksheet fixPointList;
            try
            {
                fixPointList = Globals.ThisAddIn.GetWorksheet("Sheet1");
            }
            catch (Exception)
            {
                return;

            }


            List<basicMath.fixpoint> fixpoints;
            fixpoints = DataClean.fixpointRead(fixPointList);
            if(fixpoints.Count==0)
            {
                MessageBox.Show("You havn't proper input any fixpoint on Sheet1");
                return;
            }



            graphGen tGen = new graphGen(ASCData.stationDataset, fixpoints);
            List<List<int>> result = tGen.genList();
            traverseGen tG;
            tG = new traverseGen(result, tGen.oneWayDirect, tGen.fixNameList, fixpoints);
            closedTraverse cT = tG.applyBHD(0);
            GenlayForm.OutInputForm(fixPointList, cT);
        }
    }
}
