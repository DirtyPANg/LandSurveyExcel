using Microsoft.VisualStudio.Tools.Applications.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ExcelAddIn1.basicMath;

namespace ExcelAddIn1
{
    class graphGen
    {
        int countStation;
        public List<Dictionary<int, ascLine>> oneWayDirect;
        Dictionary<int, List<int>> twoWayDirect;
        public List<List<string>> fixNameList;
        List<basicMath.fixpoint> fpList;
        List<StationData> dataSet;
        List<bool> isFixQ;
        HashSet<int> hashSet;
        public graphGen(List<StationData> stationDataset,List<fixpoint> fixpointList)
        {
            dataSet = stationDataset;
            countStation = dataSet.Count;
            oneWayDirect = graphMath.oneWayGraph(dataSet, fixpointList.Select(s=>s.name).ToList());
            twoWayDirect = graphMath.twoWayVector(oneWayDirect);
            fpList = fixpointList;
            fixNameList = new List<List<string>>();
            hashSet=new HashSet<int>();
            isFixQ = new List<bool>();
            for (int i = 0; i < countStation; i++)
            {
                isFixQ.Add(false);
                List<string> stName     = new List<string>();
                stName.Add(dataSet[i].stationName);
                fixNameList.Add(stName);
            }

            fixNameToGRAPH();
        }

        public List<List<int>> genList()
        {
            List<List<int>> returnList = new List<List<int>>();

            for (int i=0;i< hashSet.ToList().Count; i++) 
            {
                List<int> list = graphMath.BFS(twoWayDirect, hashSet.ToList()[i], hashSet);
                if (list.Count == 0 )
                {
                    continue;
                }
                i = 0;
                foreach (var item in list)hashSet.Add(item);
                returnList.Add(list);
                updteFix(list);
            }
            return returnList;
        }





        void fixNameToGRAPH()
        {
            List<string> fixList = new List<string>();
            foreach(var i in fpList)
            {
                fixList.Add(i.name);
            }
            for(int index = 0; index < countStation;index++) 
            {
                if (isFixQ[index] == true) continue;
                List<string> list = new List<string>();
                if(fixList.Contains( dataSet[index].stationName))
                {
                    hashSet.Add(index);
                    isFixQ[index]=true;
                }
                else
                {
                    foreach(var twoWayC in twoWayDirect[index])
                    {
                        if (oneWayDirect[twoWayC][index].id == dataSet[index].stationName && fixList.Contains(oneWayDirect[twoWayC][index].label))
                         {
                                isFixQ[index] = true;
                                hashSet.Add(index);
                                fixNameList[index].Add(dataSet[index].stationName);
                                if (oneWayDirect[twoWayC][index].label != "")
                                    fixNameList[index].Add(oneWayDirect[twoWayC][index].label);
                          }
                        
                        else if(oneWayDirect[twoWayC][index].id == dataSet[index].stationName )
                            {
                            fixNameList[index].Add(dataSet[index].stationName);
                            if(oneWayDirect[twoWayC][index].label != "")
                            fixNameList[index].Add(oneWayDirect[twoWayC][index].label);
                        }
                    }
                }
                HashSet<string> hashName = new HashSet<string>(fixNameList[index]);
                fixNameList[index] = new List<string>(hashName);


            }
        }
        void updteFix(List<int> list)
        {
            for(int i=0;i<countStation;i++)
            {
                if (isFixQ[i] == true) continue;
                if(list.Contains(i))
                {
                    isFixQ[i] = true;
                    fixNameList[i].Add(dataSet[i].stationName);
                    hashSet.Add(i);
                    foreach (var twoWayC in twoWayDirect[i])
                    {
                        if (oneWayDirect[twoWayC][i].label == dataSet[i].stationName && oneWayDirect[twoWayC][i].id !="")
                        {
                            fixNameList[i].Add(oneWayDirect[twoWayC][i].id);
                        }
                    }
                }
                else
                {
                    bool trueQ = false;
                    foreach (var twoWayC in twoWayDirect[i])
                    {
                        if (isFixQ[twoWayC] && oneWayDirect[twoWayC][i].label == dataSet[i].stationName && oneWayDirect[twoWayC][i].label != "")
                        {
                            fixNameList[i].Add(oneWayDirect[twoWayC][i].label);
                            if (oneWayDirect[twoWayC][i].id != "") fixNameList[i].Add(oneWayDirect[twoWayC][i].id);
                            isFixQ[i] = true;
                            trueQ = true;
                            hashSet.Add(i);
                            break;
                        }
                        
                    }
                    if (trueQ == true)
                    {
                        i--;
                        continue;
                    }
                }

                HashSet<string> hashName = new HashSet<string>(fixNameList[i]);
                fixNameList[i] = new List<string>(hashName);
            }

        }
    }



}
