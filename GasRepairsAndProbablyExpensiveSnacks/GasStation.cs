using Expansions.Missions.Adjusters;
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
        // button to request refuelling

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Fill Her Up", isPersistent = false)]
        public void FillHerUp()
        {
            if (fuelStatus == 0)
            {
                canTransfer = TestAttachmentFuel();

                if (canTransfer)
                {
                    RefuelProcedure();
                }

                else 
                { 
                    if (cardCheck)
                    {
                        strStatus = "Fuel tanks are full...";
                    }
                    else
                    {
                        strStatus = "You have no way of paying!...";
                    }   
                }                
            }

            else if (fuelStatus == 1)
            {
                bool canAfford = CheckCredit();

                if (canAfford)
                {
                    int result = CommenceTransfer(0);

                    if (result == 0)
                    {
                        strStatus = "Fuel transfer complete, come again soon"; 
                    }
                    else if (result == 1)
                    {
                        strStatus = "The pumps are not working, please contact your vendor";
                    }

                    Events["FillHerUp"].guiName = "Fill Her Up";
                    Events["RequestRecharge"].active = true;
                    Events["RequestRepair"].active = true;
                    Events["CancelOrder"].active = false;
                    totalPrice = 0;
                    fuelStatus = 0;

                }
                else
                {
                    strStatus = "Insufficient Funds!";
                    Events["FillHerUp"].guiName = "Fill Her Up";
                    Events["RequestRecharge"].active = true;
                    Events["RequestRepair"].active = true;
                    Events["CancelOrder"].active = false;
                    totalPrice = 0;
                    fuelStatus = 0;
                }
            }
        }

        // button to request recharge

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Request Recharge", isPersistent = false)]
        public void RequestRecharge()
        {
            if (elecStatus == 0)
            {
                canRecharge = TestBatteryStatus();

                if (canRecharge)
                {
                    RechargeProcedure();
                }
                else
                {
                    if (cardCheck)
                    {
                        strStatus = "No batteries need recharging...";
                    }
                    else
                    {
                        strStatus = "You have no way of paying!...";
                    }
                }
            }

            else if (elecStatus == 1)
            {
                bool canAfford = true;

                if (canAfford)
                {
                    int done = CommenceTransfer(1);

                    if (done == 0)
                    {
                        strStatus = "Battery recharge complete, come again soon";
                    }

                    Events["RequestRecharge"].guiName = "Request Recharge";
                    Events["FillHerUp"].active = true;
                    Events["RequestRepair"].active = true;
                    Events["CancelOrder"].active = false;
                    totalPrice = 0;
                    elecStatus = 0;
                }
                else
                {
                    strStatus = "Insufficient Funds!";
                    Events["RequestRecharge"].guiName = "Request Recharge";
                    Events["FillHerUp"].active = true;
                    Events["RequestRepair"].active = true;
                    Events["CancelOrder"].active = false;
                    totalPrice = 0;
                    elecStatus = 0;
                }
            }
        }

        // button to request repair

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Request Repair", isPersistent = false)]
        public void RequestRepair()
        {
            if (repairStatus == 0)
            {
                canRepair = TestRepairStatus();

                if (canRepair)
                {
                    RepairProcedure();
                }
                else strStatus = "Nothing to repair...";
            }
            else if (repairStatus == 1)
            {
                bool canAfford = true;

                if (canAfford)
                {
                    int done = CommenceTransfer(2);

                    if (done == 0)
                    {
                        strStatus = "Repairs complete, come again soon";
                    }

                    Events["RequestRepair"].guiName = "Request Repair";
                    Events["RequestRecharge"].active = true;
                    Events["FillHerUp"].active = true;
                    Events["CancelOrder"].active = false;
                    totalPrice = 0;
                    repairStatus = 0;

                }
                else
                {
                    strStatus = "Insufficient Funds!";
                    Events["RequestRepair"].guiName = "Request Repair";
                    Events["RequestRecharge"].active = true;
                    Events["FillHerUp"].active = true;
                    Events["CancelOrder"].active = false;
                    totalPrice = 0;
                    repairStatus = 0;
                }
            }
        }

        // button to cancel order

        [KSPEvent(active = false, guiActive = true, guiActiveEditor = false, isPersistent = false, guiName = "Cancel Order")]
        public void CancelOrder()
        {
            if (fuelStatus == 1)
            {
                Events["FillHerUp"].guiName = "Fill Her Up";
                fuelStatus = 0;
            }
            else if (elecStatus == 1)
            {
                Events["RequestRecharge"].guiName = "Request Recharge";
                elecStatus = 0;
            }
            else
            {
                Events["RequestRepair"].guiName = "Request Repair";
                repairStatus = 0;
            }

                Events["FillHerUp"].active = true;
                Events["RequestRecharge"].active = true;
                Events["RequestRepair"].active = true;
                totalPrice = 0;
                strStatus = "Order Cancelled";
                Events["CancelOrder"].active = false;

        }

        // liquid fuel/oxidizer price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Liquid Fuel / Oxidizer Price", isPersistant = false)]
        public double lfoCost;

        // liquid fuel price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Liquid Fuel Only Price", isPersistant = false)]
        public double lfCost;

        // oxidiser price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Oxidizer Only Price", isPersistant = false)]
        public double oCost;

        // monopropellant price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Monopropellant Price", isPersistant = false)]
        public double mCost;

        // xenon price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Xenon Price", isPersistant = false)]
        public double xCost;

        // recharge batteries price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Recharging Price", isPersistant = false)]
        public double batCost;

        // repairs price

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Repairs Price", isPersistant = false)]
        public double repCost;

        // total price of order

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Total Price", isPersistant = false)]
        public double totalPrice;

        // status for player

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Status", isPersistant = false)]
        public string strStatus;

        public bool canTransfer;
        public bool canRepair;
        public bool canRecharge;
        public bool cardCheck;
        public CelestialBody currentOrbit;
        public double modCost;
        public double lfDif;
        public double oDif;
        public double mDif;
        public double xDif;
        public double batDif;
        public double runningFuel;
        public double runningElec;
        public double runningRep;
        public int repairCount;
        public int fuelStatus = 0; // 0 = idle, 1 = awaiting confirmation, 2 = transferring
        public int elecStatus = 0;
        public int repairStatus = 0;
        public List<double> basePrices;
        public List<Part> listOfCards;
        public Part activeCard;
        private static double lfoCost2;
        private static double lfCost2;
        private static double oCost2;
        private static double mCost2;
        private static double xCost2;
        private static double batCost2;
        private static double repCost2;

        public static GasStation Instance;

        public bool CheckCredit()
        {
            if (listOfCards.Count == 1)
            {
                double creditAmt = FlightGlobals.ActiveVessel.FindPartModuleImplementing<FuelCard>().availableCredit;

                if (creditAmt >= totalPrice)
                {
                    activeCard = listOfCards[0];
                    return true;
                }
                else return false;
            }
            else
            {
                for (int x = 0; x < listOfCards.Count; x++)
                {
                    double creditAmt = listOfCards[x].GetComponent<FuelCard>().availableCredit;

                    if (creditAmt >= totalPrice)
                    {
                        activeCard = listOfCards[x];
                        return true;
                    }
                    else continue;
                }
                return false;
            }
        }


        public int CommenceTransfer(int type)
        {
            bool pay = RemoveFunds();

            if (!pay)
            {
                strStatus = "Sorry there was a problem processing your card. Please contact your vendor";
                return 1;
            }
            else
            {
                if (type == 0)
                {
                    int trial = 1;

                    foreach (var part in FlightGlobals.ActiveVessel.Parts)
                    {
                        if (part.Resources.Contains("LiquidFuel"))
                        {
                            part.Resources.Get("LiquidFuel").amount = part.Resources.Get("LiquidFuel").maxAmount;
                            trial = 0;
                        }
                        if (part.Resources.Contains("Oxidizer"))
                        {
                            part.Resources.Get("Oxidizer").amount = part.Resources.Get("Oxidizer").maxAmount;
                            trial = 0;
                        }
                        if (part.Resources.Contains("MonoPropellant"))
                        {
                            part.Resources.Get("MonoPropellant").amount = part.Resources.Get("MonoPropellant").maxAmount;
                            trial = 0;
                        }
                        if (part.Resources.Contains("XenonGas"))
                        {
                            part.Resources.Get("XenonGas").amount = part.Resources.Get("XenonGas").maxAmount;
                            trial = 0;
                        }
                    }

                    return trial;
                }
                else if (type == 1)
                {
                    int trial = 1;

                    foreach (var part in FlightGlobals.ActiveVessel.Parts)
                    {
                        if (part.Resources.Contains("ElectricCharge"))
                        {
                            part.Resources.Get("ElectricCharge").amount = part.Resources.Get("ElectricCharge").maxAmount;
                            trial = 0;
                        }
                    }

                    return trial;
                }

                else                    // type is 2, not stated so return always permitted 
                {
                    int trial = 1;

                    try
                    {
                        foreach (var part in FlightGlobals.ActiveVessel.Parts)
                        {
                            part.PartRepair();
                        }

                        trial = 0;
                    }
                    catch
                    {
                        trial = 1;
                    }

                    return trial;
                }
            }
        }

        public bool RemoveFunds()
        {
            try
            {
                activeCard.GetComponent<FuelCard>().availableCredit -= totalPrice;
                return true;
            }
            catch
            {
                return false;
            }
        }

      

        public void RefuelProcedure()
        {
            runningFuel = 0;
            runningFuel = (lfDif * lfCost) + (oDif * oCost) + (mDif * mCost) + (xDif * xCost);
            totalPrice = Math.Round(runningFuel, 0);

            if (totalPrice < 1)
            {
                totalPrice = 1;
            }

            strStatus = "Please confirm purchase at the above price to continue";
            Events["FillHerUp"].guiName = "Press To Commit To Purchase";
            fuelStatus = 1;
            Events["CancelOrder"].active = true;
            Events["RequestRecharge"].active = false;
            Events["RequestRepair"].active = false;    
        }

        public void RepairProcedure()
        {
            runningRep = 0;
            runningRep = repairCount * repCost;
            totalPrice = Math.Round(runningRep, 0);

            if (totalPrice < 1)
            {
                totalPrice = 1;
            }

            strStatus = "Please confirm purchase at the above price to continue";
            Events["RequestRepair"].guiName = "Press To Commit To Purchase";
            repairStatus = 1;
            Events["CancelOrder"].active = true;
            Events["RequestRecharge"].active = false;
            Events["RequestRepair"].active = false;
        }

        public void RechargeProcedure()
        {
            runningElec = 0;
            runningElec = batDif * batCost;
            totalPrice = Math.Round(runningElec, 0);
           
            if (totalPrice < 1)
            {
                totalPrice = 1;
            }
            
            strStatus = "Please confirm purchase at the above price to continue";
            Events["RequestRecharge"].guiName = "Press To Commit To Purchase";
            elecStatus = 1;
            Events["FillHerUp"].active = false;
            Events["RequestRepair"].active = false;
            Events["CancelOrder"].active = true;
        }

        public bool TestRepairStatus()
        {
            bool toReturn = false;
            repairCount = 0;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                List<AdjusterPartModuleBase> list = new List<AdjusterPartModuleBase>();
              
                if (list.Count > 0)
                {
                    repairCount += 1;
                }
            }

            if (repairCount > 0)
            {
                toReturn = true;
            }

            else toReturn = false;
            return toReturn;
        }

        public bool TestBatteryStatus()
        {
            bool toReturn = false;
            cardCheck = CardCheck();

            if (cardCheck)
            {
                batDif = 0;

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

                if (batDif > 0)
                {
                    toReturn = true;
                }
                else toReturn = false;
                return toReturn;
            }
            else
            {
                return false;
            }
        }

        public bool CardCheck()
        {
            cardCheck = false;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.HasModuleImplementing<FuelCard>())
                {
                    listOfCards.Add(part);
                    cardCheck = true;       
                }
            }

            return cardCheck;
        }

        public bool TestAttachmentFuel()
        {
            bool toReturn = false;
            cardCheck = CardCheck();

            if (cardCheck)
            {
                lfDif = 0;
                oDif = 0;
                mDif = 0;
                xDif = 0;

                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.Resources.Contains("LiquidFuel"))
                    {
                        if (part.Resources.Get("LiquidFuel").amount < part.Resources.Get("LiquidFuel").maxAmount)
                        {
                            lfDif += part.Resources.Get("LiquidFuel").maxAmount - part.Resources.Get("LiquidFuel").amount;
                        }
                    }

                    if (part.Resources.Contains("Oxidizer"))
                    {
                        if (part.Resources.Get("Oxidizer").amount < part.Resources.Get("Oxidizer").maxAmount)
                        {
                            oDif += part.Resources.Get("Oxidizer").maxAmount - part.Resources.Get("Oxidizer").amount;
                        }
                    }

                    if (part.Resources.Contains("MonoPropellant"))
                    {
                        if (part.Resources.Get("MonoPropellant").amount < part.Resources.Get("MonoPropellant").maxAmount)
                        {
                            mDif += part.Resources.Get("MonoPropellant").maxAmount - part.Resources.Get("MonoPropellant").amount;
                        }
                    }

                    if (part.Resources.Contains("XenonGas"))
                    {
                        if (part.Resources.Get("XenonGas").amount < part.Resources.Get("XenonGas").maxAmount)
                        {
                            xDif += part.Resources.Get("XenonGas").maxAmount - part.Resources.Get("XenonGas").amount;
                        }
                    }

                }

                if (lfDif > 0 || oDif > 0 || mDif > 0 || xDif > 0)
                {
                    toReturn = true;
                }
                else toReturn = false;
                return toReturn;
            }
            else
            {
                return false;
            }
        }

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
                    listOfCards = new List<Part>();
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


                    canTransfer = false;
                    canRepair = false;
                    canRecharge = false;
                    cardCheck = false;
                    strStatus = "Awaiting Instructions";
                    fuelStatus = 0;
                    elecStatus = 0;
                    repairStatus = 0;
                    totalPrice = 0;

                    Events["CancelOrder"].active = false;

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






    }
}
// ToDo 
// Part payment / refuel if insuffient to fill up
