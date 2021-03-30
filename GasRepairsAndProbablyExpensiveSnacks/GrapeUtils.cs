using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GasRepairsAndProbablyExpensiveSnacks
{
    class GrapeUtils
    {
        public GrapeUtils()
        {
        }

        public double DistanceCalculator(string dfs)
        {
            double distanceParam;
            
            switch (dfs)
            {
                case "Moho^N":
                    distanceParam = 4.5;
                    break;
                case "Ev^N":
                    distanceParam = 3;
                    break;
                case "Gilly^N":
                    distanceParam = 3.5;
                    break;
                case "Duna^N":
                    distanceParam = 3;
                    break;
                case "Ike^N":
                    distanceParam = 3;
                    break;
                case "Dres^N":
                    distanceParam = 3.5;
                    break;
                case "Jool^N":
                    distanceParam = 4;
                    break;
                case "Laythe^N":
                    distanceParam = 4.5;
                    break;
                case "Tylo^N":
                    distanceParam = 4.5;
                    break;
                case "Pol^N":
                    distanceParam = 4.5;
                    break;
                case "Vall^N":
                    distanceParam = 4.5;
                    break;
                case "Bop^N":
                    distanceParam = 4.5;
                    break;
                case "Eeloo^N":
                    distanceParam = 5;
                    break;
                case "Kerbin^N":
                    distanceParam = 1.25;
                    break;
                case "Mun^N":
                    distanceParam = 1.5;
                    break;
                case "Minmus^N":
                    distanceParam = 1.5;
                    break;
                default:
                    distanceParam = 6;
                    break;
            }

            return distanceParam;
        }



        public List<double> BaseRates()
        {
            List<double> baseList = new List<double>();
            double[] allPrices =
            {
                0.98,       //lfo
                0.8,        //lf
                0.18,       //o
                1.2,        //mp
                4.0,        //xen
                100.0,        // bat
                300.0       // rep
            };

            baseList.AddRange(allPrices);


            return baseList;

        }

        public double TimeCalculator(string dfs)
        {
            double distanceParam;

            switch (dfs)
            {
                case "Moho^N":
                    distanceParam = 4.5;
                    break;
                case "Ev^N":
                    distanceParam = 3;
                    break;
                case "Gilly^N":
                    distanceParam = 3.5;
                    break;
                case "Duna^N":
                    distanceParam = 3;
                    break;
                case "Ike^N":
                    distanceParam = 3;
                    break;
                case "Dres^N":
                    distanceParam = 3.5;
                    break;
                case "Jool^N":
                    distanceParam = 4;
                    break;
                case "Laythe^N":
                    distanceParam = 4.5;
                    break;
                case "Tylo^N":
                    distanceParam = 4.5;
                    break;
                case "Pol^N":
                    distanceParam = 4.5;
                    break;
                case "Vall^N":
                    distanceParam = 4.5;
                    break;
                case "Bop^N":
                    distanceParam = 4.5;
                    break;
                case "Eeloo^N":
                    distanceParam = 5;
                    break;
                case "Kerbin^N":
                    distanceParam = 1.25;
                    break;
                case "Mun^N":
                    distanceParam = 1.5;
                    break;
                case "Minmus^N":
                    distanceParam = 1.5;
                    break;
                default:
                    distanceParam = 6;
                    break;
            }

            return distanceParam;
        }


    }
}
