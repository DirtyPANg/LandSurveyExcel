using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.Tools.Applications.Runtime;

namespace ExcelAddIn1
{
    class graphMath
    {


        public static List<int> BFS(Dictionary<int, List<int>> graph, int startNode, HashSet<int> fixNodes)
        {
            Queue<List<int>> queue = new Queue<List<int>>();

            queue.Enqueue(new List<int>() { startNode });
            HashSet<int> endNodes = new HashSet<int>  (fixNodes);
            endNodes.Remove(startNode);
            while (queue.Count > 0)
            {
                List<int> path = queue.Dequeue();
                int currentNode = path.Last();

                if (path.Count >= 3 && endNodes.Contains(currentNode))
                {
                    return path;
                }

                if (graph.ContainsKey(currentNode) && !endNodes.Contains(currentNode))
                {
                    foreach (int neighbor in graph[currentNode])
                    {
                        if (!path.Contains(neighbor))
                        {
                            List<int> newPath = new List<int>(path) { neighbor };
                            queue.Enqueue(newPath);
                        }
                    }
                }
            }

            return new List<int>();
        }


        public static Dictionary<int,List<int>> twoWayVector(List<Dictionary<int, ascLine>> oneWayGraph)
        {
            Dictionary<int, List<int>> returnList = new Dictionary<int, List<int>>();
            for(int graphIx = 0; graphIx<oneWayGraph.Count;graphIx++)
            {
                List<int> singleGraph = new List<int>();
                for(int checkIx =0; checkIx<oneWayGraph.Count;checkIx++)
                {
                    if (graphIx == checkIx) continue;
                    if (oneWayGraph[checkIx].ContainsKey(graphIx)) singleGraph.Add(checkIx);
                }
                returnList.Add(graphIx,singleGraph);
            }
            return returnList;
        }
        



        public static List<Dictionary<int, ascLine>> oneWayGraph(List<StationData> stationSet ,List<string> fixName)
        {
            List < Dictionary<int, ascLine> > returnGraph = new List<Dictionary<int, ascLine>>();
            for (int stIx = 0; stIx<stationSet.Count;stIx++)
            {
                Dictionary<int,ascLine> stationGraph = new Dictionary<int,ascLine>();
                foreach(var meas in stationSet[stIx].measureList)
                {
                    if(meas.id=="18")
                    {

                    }
                    bool isFound = false;
                    int key;
                    isFound = TryFindKeyByValue(stationGraph,meas.label,out key);



                    bool isInGraph = false;
                    for(int index =0;index<stationSet.Count;index++)
                    {
                        if (index == stIx) continue; 
                        
                        if (stationSet[index].stationName == meas.label || stationSet[index].stationName == meas.id || (isFound &&( stationGraph[key].label == stationSet[index].stationName|| stationGraph[key].id == stationSet[index].stationName)))
                        {
                            isInGraph= true;
                            if (stationGraph.ContainsKey(index))
                            {
                                stationGraph[index] = TakeMean(stationGraph[index], meas);
                            }
                            else 
                            {
                                stationGraph.Add(index,meas);
                            }
                        }
                    }


                    if(isInGraph == false)
                    {
                        int idCountain, labelCountain;
                        idCountain = fixName.IndexOf(meas.id);
                        if(idCountain!=-1&& isFound)
                        {
                            stationGraph[key] = TakeMean(stationGraph[key], meas);
                            continue;
                        }
                        else if(idCountain != -1)
                        {
                            stationGraph.Add(-idCountain - 1, meas);
                            
                        }
                        labelCountain = fixName.IndexOf(meas.label);
                        if (labelCountain != -1&& isFound)
                        {
                            stationGraph[key] = TakeMean(stationGraph[key], meas);
                            
                        }
                        else if(labelCountain != -1)
                        {
                            stationGraph.Add(-labelCountain - 1, meas);
                        }

                    }
                }
                returnGraph.Add(stationGraph);
            }
            return returnGraph;
        }


        static bool TryFindKeyByValue(Dictionary<int, ascLine> dictionary, string value, out int key)
        {
            foreach (KeyValuePair<int, ascLine> kvp in dictionary)
            {
                if (kvp.Value.label == value || kvp.Value.id == value)
                {
                    key = kvp.Key;
                    return true;
                }
            }

            key = default;
            return false;
        }
        static public bool singleConnectQ(double HD1, double HD2,string stationName,string foreID, string foreLabel)
        {
            if (HDQ(HD1, HD2) && labelQ(stationName,foreID, foreLabel)) return true;
            return false;
        }
        static bool HDQ(double HD1,double HD2)
        {
            return Math.Abs(HD1-HD2)<0.016+0.0005*Math.Pow(Math.Abs(HD1 - HD2), 0.5) && Math.Abs(HD1 - HD2) >0.5 ? true : false;
        }
        public static bool labelQ(string stationName,string foreID,string foreLabel)
        {
            if(foreID == stationName || foreLabel == stationName ) {
                return true;
            }
            return false;
        }

        
        static ascLine TakeMean(ascLine first, ascLine second)
        {
            ascLine newAsc = new ascLine();
            newAsc = first;
            if(first.HD<0)
            {
                first.bearing += Math.PI;
                first.verticalAngle = basicMath.normalAngle(Math.PI-first.verticalAngle)+Math.PI;
                first.HD = Math.Abs(first.HD);
            }
            if (second.HD < 0)
            {
                second.bearing+= Math.PI;
                second.verticalAngle = basicMath.normalAngle(Math.PI - second.verticalAngle)+Math.PI;
                second.HD= Math.Abs(second.HD);
            }
            first.bearing = basicMath.normalAngle(first.bearing + Math.PI) + Math.PI;
            second.bearing = basicMath.normalAngle(second.bearing + Math.PI) + Math.PI;



            if (first.label == "9002" && first.id == "22")
            {
            }
            if (second.label == "9002" && second.id == "22")
            {
                string s = GenlayForm.bearingToForm((first.bearing + second.bearing) / 2);
            }

            newAsc.bearing = (first.bearing+second.bearing) / 2;
            newAsc.verticalAngle = (first.verticalAngle + second.verticalAngle) / 2;
            newAsc.HD = (first.HD + second.HD) / 2;
            return newAsc;
        }

    }
}
