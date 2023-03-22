using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExcelAddIn1.basicMath;
using System.Text.RegularExpressions;

namespace ExcelAddIn1
{
    class LineRegister
    {
        static public List<line3Dseg> lineRegister(List<basicMath.pointData> topoD, List<List<string>> lxS,int tOffset)
        {
            List<line3Dseg>  registerLine = new List<line3Dseg>();
            for(int i =0; i < lxS.Count;i++)
            {
                if (lxS[i].Count < 3) continue;
                int firstNumber = ExtractFirstNumber(lxS[i][1]);
                int secondNumber = ExtractFirstNumber(lxS[i][2]);
                if(firstNumber==int.MinValue && secondNumber == int.MinValue&&secondNumber- firstNumber<1) continue;
                int find1 = vTopoFind(topoD, firstNumber.ToString(), firstNumber - tOffset);
                int find2 ;
                for (int ii = firstNumber+1; ii<secondNumber;ii++)
                {
                    find2 = vTopoFind(topoD, (firstNumber+1).ToString(), firstNumber+1 - tOffset);
                    if(find2 ==-1 && find1==-1)
                    {
                        find1 = find2;
                    }
                    else
                    {
                        line3Dseg line = new line3Dseg(topoD[find1], topoD[find2]);
                        registerLine.Add(line);
                    }

                }
                
            }
            return registerLine;
        }
        static int ExtractFirstNumber(string input)
        {
            // Use regex to find the first number in the input string
            Match match = Regex.Match(input, @"\d+");
            if (match.Success)
            {
                // Parse the matched string to integer
                int number = int.Parse(match.Value);
                return number;
            }
            else
            {
                return int.MinValue;
            }
        }
    }


    
}
