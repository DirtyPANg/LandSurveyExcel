using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelAddIn1
{
    class traverseGen
    {
        public List<List<int>> graphList;
        public List<Dictionary<int, ascLine>> oneWayDirect;
        public List<List<string>> fixNameList;
        public List<string> nameList;
        public List<List<string>> mapList;
        List<basicMath.fixpoint> fixpointList;
        Dictionary<int,basicMath.fixpoint> fixpointInMap;
        public traverseGen(List<List<int>> graphList, List<Dictionary<int, ascLine>> oneWayDirect, List<List<string>> fixNameList,List<basicMath.fixpoint> fixpointList)
        {
            this.graphList = graphList;
            this.oneWayDirect = oneWayDirect;
            this.fixNameList = fixNameList;
            this.fixpointList = fixpointList;
            this.fixpointInMap = new Dictionary<int, basicMath.fixpoint>();
            nameList= new List<string>();
            mapList= new List<List<string>>();
        }
        

        public closedTraverse applyBHD(int graphIndex)
        {
            mapStation();
            if (graphList.Count == 0) return null;
            if (graphList[graphIndex].Count < 3) return null;
            basicMath.fixpoint startNode;
            basicMath.fixpoint endNode;

            List<Tuple<basicMath.fixpoint, basicMath.BearingHD>> startRefnodes = new List<Tuple<basicMath.fixpoint, basicMath.BearingHD>>();
            List<Tuple<basicMath.fixpoint, basicMath.BearingHD>> endRefnodes = new List<Tuple<basicMath.fixpoint, basicMath.BearingHD>>();
            List<Tuple<string, basicMath.BearingHD, basicMath.BearingHD>> traverseList = new List<Tuple<string, basicMath.BearingHD, basicMath.BearingHD>>();
            try
            {
                int start = graphList[graphIndex].First();
                int end = graphList[graphIndex].Last();
                startNode = fixpointList.FirstOrDefault(item=>item.name == nameList[graphList[graphIndex].First()]);
                endNode = fixpointList.FirstOrDefault(item => item.name == nameList[graphList[graphIndex].Last()]);

                bool alreadyJoinTraverse = false;
                foreach (var startOneWay in oneWayDirect[start])
                {
                    int index = indexLine(startOneWay.Value);
                    basicMath.fixpoint checkIfFixpoint;
                    if (fixpointInMap.TryGetValue(index, out checkIfFixpoint))
                    {
                        startRefnodes.Add(Tuple.Create(checkIfFixpoint,ASCToBHD(startOneWay.Value)));
                    }
                    else if(index == graphList[graphIndex][1]&& alreadyJoinTraverse!=true)
                    {
                        basicMath.BearingHD BHD1 = new basicMath.BearingHD(0d,0d);
                        basicMath.BearingHD BHD2 = new basicMath.BearingHD(startOneWay.Value.bearing, startOneWay.Value.HD);
                        traverseList.Add(Tuple.Create(startNode.name, BHD1, BHD2));
                        alreadyJoinTraverse= true;
                    }
                }


                for(int i =1; i < graphList[graphIndex].Count-1;i++)
                {
                    traverseList.Add(returnBHDFromASC(graphList[graphIndex][i], graphList[graphIndex][i-1], graphList[graphIndex][i+1]));
                }


                alreadyJoinTraverse = false;
                foreach (var endOneWay in oneWayDirect[end])
                {
                    int index = indexLine(endOneWay.Value);
                    basicMath.fixpoint checkIfFixpoint;
                    if (fixpointInMap.TryGetValue(index, out checkIfFixpoint))
                    {
                        endRefnodes.Add(Tuple.Create(checkIfFixpoint, ASCToBHD(endOneWay.Value)));
                    }
                    else if (index == graphList[graphIndex][graphList[graphIndex].Count-2] && alreadyJoinTraverse != true)
                    {
                        basicMath.BearingHD BHD1 = new basicMath.BearingHD(endOneWay.Value.bearing, endOneWay.Value.HD);
                        basicMath.BearingHD BHD2 = new basicMath.BearingHD(0d,0d);
                        traverseList.Add(Tuple.Create(endNode.name, BHD1, BHD2));
                        alreadyJoinTraverse = true;
                    }
                }

            }
            catch(Exception e)
            {
                return null;
            }
            closedTraverse returnValue = new closedTraverse(startNode,endNode,startRefnodes,endRefnodes,traverseList);
            return returnValue;
        }




       basicMath.BearingHD ASCToBHD(ascLine ascData)
       {
            basicMath.BearingHD returnValue= new basicMath.BearingHD(ascData.bearing,ascData.HD);
            return returnValue;

       }
        public void mapStation()
        {
            foreach (var graph in fixNameList)
            {
                bool isInside = false;
                for(int mapIx = 0; mapIx<mapList.Count; mapIx++)
                {
                    var item = MergeIfHasCommonElement(graph, mapList[mapIx]);
                    if ( item!=null)
                    {
                        isInside = true; 
                    }
                }
                if(isInside == false)
                {
                    mapList.Add(graph);
                }
            }

            for (int i = 0; i < mapList.Count && mapList.Count != 1; i++)
            {
                for (int ii = i+1; ii < mapList.Count; ii++)
                {
                    List<string> list = MergeIfHasCommonElement(mapList[i], mapList[ii]) ;
                    if(list!=null)
                    {
                        i = 0;
                        mapList[i]= list;
                        mapList.RemoveAt(ii);
                        break;
                    }
                }
            }
            for (int stationIx=0;stationIx<mapList.Count;stationIx++)
            {
                mapList[stationIx] = SortByPriority(mapList[stationIx]);
            }
            foreach(var finalStationName in fixNameList)
            {
                foreach(var stationName in mapList)
                {
                    if (finalStationName.Intersect(stationName).Any())
                    {
                        nameList.Add(stationName[0]);
                        break;
                    }
                }
            }
            updataFixpointInMap();
        }

        Tuple<string, basicMath.BearingHD, basicMath.BearingHD> returnBHDFromASC(int graphIx,int backIx,int foreIx)
        {
            string Name = nameList[graphIx];
            basicMath.BearingHD backBHD = new basicMath.BearingHD();
            basicMath.BearingHD foreBHD = new basicMath.BearingHD();
            bool haveBack = false;
            bool haveFore = false;  
            foreach (var item in oneWayDirect[graphIx])
            {
                int index = indexLine(item.Value);
                if(haveBack==false&&index == backIx)
                {
                    haveBack = true;
                    backBHD = ASCToBHD(item.Value);
                    continue;
                }
                if (haveFore == false && index == foreIx)
                {
                    haveFore = true;
                    foreBHD = ASCToBHD(item.Value);
                    continue;
                }
            }
            return Tuple.Create(Name,backBHD,foreBHD);
        }


        int indexLine(ascLine findValue)
        {
            for(int i=0;i<mapList.Count;i++)
            {
                if (mapList[i].Contains(findValue.id) || mapList[i].Contains(findValue.label))
                    return i;
            }
            return -1;
        }


        void updataFixpointInMap()
        {
            for(int i=0;i< nameList.Count;i++)
            {
                if (fixpointInMap.ContainsKey(i)) continue;
                int fixPointIndex = fixpointList.FindIndex(findValue => findValue.name == nameList[i]);
                if (fixPointIndex != -1)
                {
                    fixpointInMap.Add(i, fixpointList[fixPointIndex]);
                }

            }
        }
        List<string> MergeIfHasCommonElement(List<string> list1, List<string> list2)
        {
            if (list1.Intersect(list2).Any())
            {
                return list1.Union(list2).ToList();
            }
            return null;
        }
        List<string> SortByPriority(List<string> inputList)
        {
            var letterItems = inputList.Where(item => Regex.IsMatch(item, "[a-zA-Z]+")).ToList();
            var numberItems = inputList.Where(item => Regex.IsMatch(item, "^[0-9]+$")).ToList();
            numberItems.Sort();
            numberItems.Reverse();
            List<string> sortedList = new List<string>();
            sortedList.AddRange(letterItems);
            sortedList.AddRange(numberItems);
            return sortedList;
        }

    }
}
