﻿using Expansions.Missions.Adjusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GasRepairsAndProbablyExpensiveSnacks
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GasStation : PartModule
    {
        public bool isWaitingForDelivery;
        public CelestialBody currentOrbit;
        public double lfoCost;
        public double lfCost;
        public double oCost;
        public double mCost;
        public double xCost;
        public double batCost;
        public double repCost;
        public double totalPrice;
        public double modCost;
        public double lfDif;
        public double oDif;
        public double mDif;
        public double xDif;
        public double batDif;
        public double runningFuel;
        public double creditAmount = 0;
        public List<double> basePrices;
        public string timeTillDelivery;
        private static double lfoCost2;
        private static double lfCost2;
        private static double oCost2;
        private static double mCost2;
        private static double xCost2;
        private static double batCost2;
        private static double repCost2;
        public static GasStation Instance;


        public void Start()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                Instance = this;

                try
                {
                    basePrices = new List<double>();
                    currentOrbit = FlightGlobals.ActiveVessel.mainBody;
                    string strOrbit = currentOrbit.displayName;

                    GrapeUtils priceModifier = new GrapeUtils();
                    modCost = priceModifier.DistanceCalculator(strOrbit);
                    basePrices = priceModifier.BaseRates();

                    lfoCost = Math.Round(basePrices[0] * modCost, 2);
                    lfCost = Math.Round(basePrices[1] * modCost, 2);
                    oCost = Math.Round(basePrices[2] * modCost, 2);
                    mCost = Math.Round(basePrices[3] * modCost, 2);
                    xCost = Math.Round(basePrices[4] * modCost, 2);
                    batCost = Math.Round(basePrices[5] * modCost, 2);
                    repCost = Math.Round(basePrices[6] * modCost, 2);
                    
                    totalPrice = 0;

                }
                catch
                {
                        // internal error
                }
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                CelestialBody loc = FlightGlobals.ActiveVessel.mainBody;
               
                if (loc != currentOrbit)
                {
                    basePrices = new List<double>();
                    currentOrbit = loc;
                    string strOr2 = currentOrbit.displayName;
                    GrapeUtils priceModifier = new GrapeUtils();
                    modCost = priceModifier.DistanceCalculator(strOr2);
                    basePrices = priceModifier.BaseRates();

                    lfoCost = Math.Round(basePrices[0] * modCost, 2);
                    lfCost = Math.Round(basePrices[1] * modCost, 2);
                    oCost = Math.Round(basePrices[2] * modCost, 2);
                    mCost = Math.Round(basePrices[3] * modCost, 2);
                    xCost = Math.Round(basePrices[4] * modCost, 2);
                    batCost = Math.Round(basePrices[5] * modCost, 2);
                    repCost = Math.Round(basePrices[6] * modCost, 2);
                }
            }
        }

        public static double GetFuelAmount()
        {
            Instance.lfDif = 0;
            Instance.oDif = 0;
            Instance.mDif = 0;
            Instance.xDif = 0;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.Resources.Contains("LiquidFuel"))
                {
                    if (part.Resources.Get("LiquidFuel").amount < part.Resources.Get("LiquidFuel").maxAmount)
                    {
                        Instance.lfDif += part.Resources.Get("LiquidFuel").maxAmount - part.Resources.Get("LiquidFuel").amount;
                    }
                }

                if (part.Resources.Contains("Oxidizer"))
                {
                    if (part.Resources.Get("Oxidizer").amount < part.Resources.Get("Oxidizer").maxAmount)
                    {
                        Instance.oDif += part.Resources.Get("Oxidizer").maxAmount - part.Resources.Get("Oxidizer").amount;
                    }
                }

                if (part.Resources.Contains("MonoPropellant"))
                {
                    if (part.Resources.Get("MonoPropellant").amount < part.Resources.Get("MonoPropellant").maxAmount)
                    {
                        Instance.mDif += part.Resources.Get("MonoPropellant").maxAmount - part.Resources.Get("MonoPropellant").amount;
                    }
                }

                if (part.Resources.Contains("XenonGas"))
                {
                    if (part.Resources.Get("XenonGas").amount < part.Resources.Get("XenonGas").maxAmount)
                    {
                        Instance.xDif += part.Resources.Get("XenonGas").maxAmount - part.Resources.Get("XenonGas").amount;
                    }
                }

            }

            if (Instance.lfDif > 0 || Instance.oDif > 0 || Instance.mDif > 0 || Instance.xDif > 0)
            {
                Instance.runningFuel = 0;
                Instance.runningFuel = (Instance.lfDif * Instance.lfCost) + (Instance.oDif * Instance.oCost) +
                    (Instance.mDif * Instance.mCost) + (Instance.xDif * Instance.xCost);
                Instance.totalPrice = Math.Round(Instance.runningFuel, 0);

                if (Instance.totalPrice < 1)
                {
                    Instance.totalPrice = 1;
                }

                return Instance.totalPrice;
            }

            else return 0;

        }

        public static double GetCreditAmount()
        {
            List<Part> cardList = new List<Part>();
            double totalCredit = 0;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.HasModuleImplementing<FuelCard>())
                {
                    cardList.Add(part);
                }
            }

            if (cardList.Count == 0)
            {
                Instance.creditAmount = totalCredit;
                return 0;
            }
            else
            {
                for (int x = 0; x < cardList.Count; x++)
                {
                    if (cardList[x].GetComponent<FuelCard>().availableCredit > 0)
                    {
                        totalCredit += cardList[x].GetComponent<FuelCard>().availableCredit;
                    }
                }

                Instance.creditAmount = totalCredit;

                if (totalCredit == 0)
                {
                    
                    return 0;
                }
                else return totalCredit;
            }

        }

        public static string GetStatus(int _code)
        {
            if (_code == 0)
            {
                if (Instance.isWaitingForDelivery)
                {
                    return Instance.timeTillDelivery;
                }
                else
                {
                    return "Awaiting Your Order";
                }
            }

            else if (_code == 1)
            {
                return "Refuel complete, come again soon!";
            }

            else if (_code == 2)
            {
                return "Partial refuel complete, come again soon!";
            }

            else if (_code == 3)
            {
                return "Cannot recharge - insufficient funds!";
            }

            else if (_code == 4)
            {
                return "All batteries recharged, come again soon!";
            }




            else return "There has been a problem processing your order. Please contact your card provider.";

        }

        public static bool QueryRecharge()
        {
            double batDif = 0;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.Resources.Contains("ElectricCharge"))
                {
                    if (part.Resources.Get("ElectricCharge").amount < part.Resources.Get("ElectricCharge").maxAmount)
                    {
                        batDif += part.Resources.Get("ElectricCharge").maxAmount - part.Resources.Get("ElectricCharge").amount;
                    }
                }
            }

            Instance.batDif = batDif;

            if (batDif > 0)
            {
                return true;
            }

            else return false;



        }

        public static List<double> ProvidePrices()
        {
            List<double> sendList = new List<double>();

            lfoCost2 = Instance.lfoCost;
            lfCost2 = Instance.lfCost;
            oCost2 = Instance.oCost;
            mCost2 = Instance.mCost;
            xCost2 = Instance.xCost;
            batCost2 = Instance.batCost;
            repCost2 = Instance.repCost;


            double[] allOfThem =
            {
                lfoCost2,
                lfCost2,
                oCost2,
                mCost2,
                xCost2,
                batCost2,
                repCost2
            };

            sendList.AddRange(allOfThem);

            return sendList;
        }


        public static int Refuel()
        {
            double _creditAmount = Instance.creditAmount;
            double _refuelCost = Instance.totalPrice;
            double difference = _creditAmount - _refuelCost;
            double fuelToAdd;


            if (difference > 0)
            {
                // can refuel

                TakePayment(_refuelCost);

                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.Resources.Contains("LiquidFuel"))
                    {
                        part.Resources.Get("LiquidFuel").amount = part.Resources.Get("LiquidFuel").maxAmount;
                    }
                    if (part.Resources.Contains("Oxidizer"))
                    {
                        part.Resources.Get("Oxidizer").amount = part.Resources.Get("Oxidizer").maxAmount;
                    }
                    if (part.Resources.Contains("MonoPropellant"))
                    {
                        part.Resources.Get("MonoPropellant").amount = part.Resources.Get("MonoPropellant").maxAmount;  
                    }
                    if (part.Resources.Contains("XenonGas"))
                    {
                        part.Resources.Get("XenonGas").amount = part.Resources.Get("XenonGas").maxAmount;
                    }
                }

                TimerEvent();
                return 1;
                
            }

            else
            {
                TakePayment(0);

                fuelToAdd = (100 / _refuelCost) * _creditAmount;

                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.Resources.Contains("LiquidFuel"))
                    {
                        part.Resources.Get("LiquidFuel").amount += (part.Resources.Get("LiquidFuel").maxAmount / 100) * fuelToAdd;
                    }
                    if (part.Resources.Contains("Oxidizer"))
                    {
                        part.Resources.Get("Oxidizer").amount += (part.Resources.Get("Oxidizer").maxAmount / 100) * fuelToAdd;
                    }
                    if (part.Resources.Contains("MonoPropellant"))
                    {
                        part.Resources.Get("MonoPropellant").amount += (part.Resources.Get("MonoPropellant").maxAmount / 100) * fuelToAdd;
                    }
                    if (part.Resources.Contains("XenonGas"))
                    {
                        part.Resources.Get("XenonGas").amount += (part.Resources.Get("XenonGas").maxAmount / 100) * fuelToAdd;
                    }
                }

                TimerEvent();
                return 2;



            }





        }

        public static int Recharge()
        {

            double _creditAmount = Instance.creditAmount;
            double _rechargeRate = Instance.batCost;
            double difference = _creditAmount - _rechargeRate;
            
            if (difference < 0)
            {
                return 3;
            }

            else
            {
                TakePayment(_rechargeRate);

                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.Resources.Contains("ElectricCharge"))
                    {
                        part.Resources.Get("ElectricCharge").amount = part.Resources.Get("ElectricCharge").maxAmount;
                    }
                }

                
                return 4;

            }


        }

        public static void TakePayment(double typeToTake)
        {
            if (typeToTake == 0)
            {
                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.HasModuleImplementing<FuelCard>())
                    {
                        part.GetComponent<FuelCard>().availableCredit = 0;
                    }
                }
            }

            else
            {
                double runningType = typeToTake;

                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.HasModuleImplementing<FuelCard>())
                    {
                        if (part.GetComponent<FuelCard>().availableCredit < typeToTake)
                        {
                            typeToTake -= part.GetComponent<FuelCard>().availableCredit;
                            part.GetComponent<FuelCard>().availableCredit = 0;
                        }

                        else if (part.GetComponent<FuelCard>().availableCredit >= typeToTake)
                        {
                            part.GetComponent<FuelCard>().availableCredit -= typeToTake;
                        }
                    }
                }



            }



        }

        public static void TimerEvent()
        {
            

            




        }


    }
}

