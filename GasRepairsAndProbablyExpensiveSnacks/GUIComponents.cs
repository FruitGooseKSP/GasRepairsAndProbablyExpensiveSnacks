using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using KSP.UI;
using KSP.UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace GasRepairsAndProbablyExpensiveSnacks
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GUIComponents : MonoBehaviour
    {
        [KSPField(isPersistant = true)]
        public bool isAwaitingDelivery = false;

        [KSPField(isPersistant = true)]
        public double timerEnd;

        // reference the textures
        public Texture grapesTextureOff;
        public Texture grapesTextureOn;

        // the button reference
        public static ApplicationLauncherButton grapesBtn;

        // is button pressed?
        public bool btnIsPressed = false;

        // does the button exist?
        public bool btnIsPresent = false;

        public static GUIComponents instance;

        // status text holder
        public static string statusStringToReturn = "";

        // code for action type
        public static int code;

        // are there tanks with space in them?
        private static bool canRefuel;

        // are there batteries with spare capacity?
        private static bool canRecharge;

        // close button on menu
        public static bool closeBtn;

        // refuel request button on menu
        public static bool refuelBtn;

        //recharge request button on menu
        public static bool rechargeBtn;

        // repair request button (not currently implemented)
        public static bool repairBtn;

        // are we at a fuel station?
        public static bool atStation;

        // menu position reference ie in the middle of the screen
        private Vector2 menuPR = new Vector2((Screen.width / 2) - 200, (Screen.height / 2) - 237);

        // menu size reference
        private Vector2 menuSR = new Vector2(400, 474);

        // the menu position holder
        private static Rect guiPos;

        // current prices for location
        private static List<double> rates;

       

        public void Start()
        {

            // get the icons from file, preload menu position, get prices, instantiate the toolbar button & set it's status

            if (HighLogic.LoadedSceneIsFlight)
            {
                instance = this;

                if (grapesBtn != null)
                {
                    onDestroy();
                    grapesBtn = null;
                }

                
                grapesTextureOff = GameDatabase.Instance.GetTexture("FruitKocktail/GRAPES/Icons/grapesoff", false);
                grapesTextureOn = GameDatabase.Instance.GetTexture("FruitKocktail/GRAPES/Icons/grapeson", false);
                guiPos = new Rect(menuPR, menuSR);
                rates = GasStation.ProvidePrices();

                grapesBtn = ApplicationLauncher.Instance.AddModApplication(onTrue, onFalse, onHover, onHoverOut, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, grapesTextureOff);

                btnIsPresent = true;

                if (btnIsPressed)
                {
                    grapesBtn.SetTrue();
                }
                else grapesBtn.SetFalse();

            }

           


        }


        private static void ItsGrapesTime()
        {

            // instantiate the menu if we're at a refueling station

            atStation = false;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.HasModuleImplementing<GasStation>())
                {
                    atStation = true;
                }
            }

            if (atStation)
            {
                // instantiate the menu
                guiPos = GUILayout.Window(123456, guiPos, MenuWindow,
                    "Current Prices", new GUIStyle(HighLogic.Skin.window));
                grapesBtn.SetTrue();

            }

        }

        private static void MenuWindow(int windowID)
        {
            // the menu

            GUILayout.BeginVertical();
            GUILayout.Space(20);

            GUILayout.BeginArea(new Rect(20, 40, 360, 220));
            GUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Liquid Fuel/Oxidiser = " + rates[0].ToString("0.00"), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Liquid Fuel = " + rates[1].ToString("0.00"), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Oxidiser = " + rates[2].ToString("0.00"), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("MonoPropellant = " + rates[3].ToString("0.00"), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Xenon = " + rates[4].ToString("0.00"), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Recharge Service = " + GetRechargeAbility(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Repair Service = " + GetRepairAbility(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(40);

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(20, 250, 360, 100));

            GUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Cost To Fill Up = " + GetFillUpCost() , new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Your Available Credit = " + GetCredit(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Station Status = " + LabelStatus(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUILayout.BeginHorizontal();
            refuelBtn = GUI.Button(new Rect(40, 350, 320, 25), "Request Fuel", new GUIStyle(HighLogic.Skin.button));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            rechargeBtn = GUI.Button(new Rect(40, 375, 320, 25), "Request Recharge", new GUIStyle(HighLogic.Skin.button));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            repairBtn = GUI.Button(new Rect(40, 400, 320, 25), "Request Repair", new GUIStyle(HighLogic.Skin.button));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            closeBtn = GUI.Button(new Rect(40, 425, 320, 25), "Cancel/Close", new GUIStyle(HighLogic.Skin.button));
            GUILayout.EndHorizontal();
            

            GUILayout.EndVertical();

            GUI.DragWindow();

        }

        
        public void Update()
        {

           if (HighLogic.LoadedSceneIsFlight)
            {
                if (grapesBtn != null)
                {
                    // menu button handlers

                    if (closeBtn)
                    {
                        grapesBtn.SetFalse();
                        closeBtn = false;
                    }

                    if (refuelBtn && !isAwaitingDelivery)
                    {
                        TryRefuel();
                        refuelBtn = false;
                    }

                    if (rechargeBtn && !isAwaitingDelivery)
                    {
                        TryRecharge();
                        rechargeBtn = false;
                    }
                }
            }
        }
    
        public void OnGUI()
        {
            // handles GUI event (ie button clicked)

            if (btnIsPressed)
            {
                ItsGrapesTime();
            }
        }

        // button callbacks

        public void onTrue()
        {
            // ie when clicked on

            btnIsPressed = true;

            if (atStation)
            {
                grapesBtn.SetTexture(grapesTextureOn);
                
            }
        }

        public void onFalse()
        {
            // ie when clicked off
            if (btnIsPressed)
            {
                grapesBtn.SetTexture(grapesTextureOff);
                btnIsPressed = false;
            }
        }

        public void onHover()
        {
            // ie on hover
        }

        public void onHoverOut()
        {
            // ie when leave
        }



        private void onDestroy()
        {
            // when destroyed
            ApplicationLauncher.Instance.RemoveModApplication(grapesBtn);
            grapesBtn = null;
        }

        // gets cost to fill up in current context
        private static string GetFillUpCost()
        {
            
            double grabbedPrice = GasStation.GetFuelAmount();

            if (grabbedPrice == 0)
            {
                canRefuel = false;
                return "All Tanks Full!";
                
            }

            else
            {
                canRefuel = true;
                string stringToReturn = grabbedPrice.ToString("0.00");
                return stringToReturn;
            }

        }

        // finds how much credit the player has available on their cards
        private static string GetCredit()
        {
            
            double creditAmount = GasStation.GetCreditAmount();

            if (creditAmount == 0)
            {
                canRecharge = false;
                canRefuel = false;
                return "All Cards Are Empty!";
            }

            else
            {
                canRecharge = true;
                canRefuel = true;
                string stringToReturn = creditAmount.ToString("0.00");
                return stringToReturn;
            }

        }

        // method to populate menu status field
        private static string LabelStatus()
        {
            if (!instance.isAwaitingDelivery)
            {
                return statusStringToReturn;
            }
            else
            {
                return GetTimeRemaining();
            }
        }

        private static string GetTimeRemaining()
        {
            double currentTime = Planetarium.fetch.time;
            double timeRem = Math.Round((instance.timerEnd - currentTime), 0);
            double daysRem = Math.Round(timeRem / 21600, 0);

            if (daysRem == 0)
            {
                instance.isAwaitingDelivery = false;
                statusStringToReturn = "Awaiting Your Order";
                return statusStringToReturn;
            }

            else if (daysRem == 1)
            {
                return "Delivery Due Tomorrow";
            }
            
            else
            {
                return "Next Delivery In " + daysRem + " Days";
            }




        }


        // gets cost to recharge or n/a if no capacity
        private static string GetRechargeAbility()
        {
            canRecharge = GasStation.QueryRecharge();

            if (canRecharge)
            {
                return rates[5].ToString("0.00");
            }

            else
            {
                return "N/A";
            }

        }

        // not currently implemented
        private static string GetRepairAbility()
        {

            return "N/A";

        }

        // initiates recharge and sends result to status
        private static void TryRecharge()
        {
            if (canRecharge)
            {
                code = GasStation.Recharge();

                if (code == 4)
                {
                    statusStringToReturn = "Recharge complete!, come again soon!";
                    LabelStatus();
                }
                 
            }

        }

        // initiates refuel and sends result to status
        private static void TryRefuel()
        {
            if (canRefuel)
            {
                code = GasStation.Refuel();
                
                if (code == 1)
                {
                    statusStringToReturn = "All tanks now full, come again soon!";
                }
                else if (code == 2)
                {
                    statusStringToReturn = "Part refill complete, come again soon!";
                }

                LabelStatus();

                StaticCoroutine.Start(Wait(5));
                
            }
        }

       

        public static IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            string timeGet = GasStation.TimeReturn();
            instance.timerEnd = GasStation.TimerEnd();
            instance.isAwaitingDelivery = true;
            //statusStringToReturn = "Next delivery due in " + timeGet + " days";
            LabelStatus();
        }


    }
}
