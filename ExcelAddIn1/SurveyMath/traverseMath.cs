using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ExcelAddIn1.basicMath;

namespace ExcelAddIn1
{
    public class closedTraverse
    {
        public basicMath.fixpoint startNode;
        public basicMath.fixpoint endNode;
        public List<Tuple<basicMath.fixpoint,basicMath.BearingHD>> startRefnodes;
        public List<Tuple<basicMath.fixpoint, basicMath.BearingHD>> endRefnodes;
        public List<Tuple<string, basicMath.BearingHD,basicMath.BearingHD>> traverseList;
        public List<Tuple<string, basicMath.BearingHD>> angleHDList;
        public List<Tuple<string, basicMath.BearingHD>> adjList;
        
        public closedTraverse(basicMath.fixpoint startNode,basicMath.fixpoint endNode, List<Tuple<basicMath.fixpoint, basicMath.BearingHD>> 
            startRefnodes, List<Tuple<basicMath.fixpoint, basicMath.BearingHD>> endRefnodes, List<Tuple<string, basicMath.BearingHD, basicMath.BearingHD>> traverseList) 
        {
            this.startNode = startNode;
            this.endNode = endNode;
            this.startRefnodes= startRefnodes;
            this.endRefnodes = endRefnodes;
            this.traverseList = traverseList;
            angleHDList = new List<Tuple<string, basicMath.BearingHD>>();
            adjList = new  List<Tuple<string, basicMath.BearingHD>>();



        }

        void adjecting()
        {
            applyAngle();

        }

        void applyAdj()
        {
            double sumOfAgnle = 0;
            double angleMistake;
            for(int i = 0;i<angleHDList.Count-1;i++)
            {
                sumOfAgnle += angleHDList[i].Item2.Bearing;
            }
            angleMistake = (basicMath.normalAngle( angleHDList[angleHDList.Count].Item2.Bearing) - basicMath.normalAngle( sumOfAgnle))/angleHDList.Count;
            foreach(var angleHD in angleHDList)
            {
                basicMath.BearingHD adjAHD = new basicMath.BearingHD();
                adjAHD.Bearing+=angleMistake;
                adjList.Add(Tuple.Create(angleHD.Item1,adjAHD));
            }


        }
        void applyAngle()
        {
            basicMath.BearingHD startAngleHD = new basicMath.BearingHD();
            startAngleHD.Bearing = 0;
            startAngleHD.HD = 0;
            foreach (var startCheck in startRefnodes)
            {
                startAngleHD.Bearing += startCheck.Item2.Bearing+basicMath.ReturnBearingHD(startNode,startCheck.Item1).Bearing;

            }
            startAngleHD.Bearing /= startRefnodes.Count;
            angleHDList.Add(Tuple.Create(startNode.name, startAngleHD));  /*apply True Bearing on starting*/


            basicMath.BearingHD endAngleHD = new basicMath.BearingHD();
            endAngleHD.Bearing = 0;
            endAngleHD.HD = 0;
            foreach (var endAHD in endRefnodes)
            {
                endAngleHD.Bearing += endAHD.Item2.Bearing + basicMath.ReturnBearingHD(endNode, endAHD.Item1).Bearing;

            }
            endAngleHD.Bearing /= startRefnodes.Count;




            for (int i = 1; i<traverseList.Count-1;i++)
            {
                basicMath.BearingHD angleHD = new BearingHD();
                angleHD.Bearing = traverseList[i].Item3.Bearing - traverseList[i].Item2.Bearing;
                angleHD.HD = (traverseList[i - 1].Item3.HD + traverseList[i].Item2.HD)/2;
                angleHDList.Add(Tuple.Create(traverseList[i].Item1, angleHD));
            }

            angleHDList.Add(Tuple.Create(endNode.name,endAngleHD));
        }

        
    }
    

}
