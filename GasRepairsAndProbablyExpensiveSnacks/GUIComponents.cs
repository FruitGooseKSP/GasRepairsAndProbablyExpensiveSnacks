using System;
using System.Collections.Generic;
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
        public Texture grapesTexture;

        public static ApplicationLauncherButton grapesBtn;

        // is button pressed?
        public bool btnIsPressed = false;

        // does the button exist?
        public bool btnIsPresent = false;

        public static bool closeBtn;
        public static bool refuelBtn;
        public static bool rechargeBtn;
        public static bool repairBtn;

        private Vector2 menuPR = new Vector2((Screen.width / 2) - 150, (Screen.height / 2) - 237);

        // menu size reference
        private Vector2 menuSR = new Vector2(300, 474);

        // the menu position holder
        private static Rect guiPos;

        private static List<double> rates;


        public void Awake()
        {
            // register game events
            if (HighLogic.LoadedSceneIsFlight && grapesBtn == null)
            {
                GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
                GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
            }
        }

        private void RemoveButton(GameScenes gameScenes)
        {
            // remove the button

            ApplicationLauncher.Instance.RemoveModApplication(grapesBtn);
            btnIsPressed = false;
            btnIsPresent = false;

        }
        private void AddButton()
        {
            // add the button

            if (!btnIsPresent)
            {
                grapesBtn = ApplicationLauncher.Instance.AddModApplication(onTrue, onFalse, onHover, onHoverOut, onEnable, onDisable,
                    ApplicationLauncher.AppScenes.FLIGHT, grapesTexture);

                btnIsPresent = true;
            }
        }

        private static void ItsGrapesTime()
        {

            // instantiate the menu

            bool atStation = false;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.HasModuleImplementing<GasStation>())
                {
                    atStation = true;
                }
            }

            if (atStation)
            {

                guiPos = GUILayout.Window(123456, guiPos, MenuWindow,
                    "Current Prices", new GUIStyle(HighLogic.Skin.window));

            }

        }

        private static void MenuWindow(int windowID)
        {
            // menu defs

            GUILayout.BeginVertical();
            GUILayout.Space(20);

            GUILayout.BeginArea(new Rect(20, 40, 220, 220));
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


            GUILayout.BeginArea(new Rect(20, 250, 220, 100));


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
            GUILayout.Label("Station Status = " + GetStatus(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();



            GUILayout.EndArea();

            GUILayout.BeginHorizontal();

            refuelBtn = GUI.Button(new Rect(40, 350, 220, 25), "Request Fuel", new GUIStyle(HighLogic.Skin.button));

            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            rechargeBtn = GUI.Button(new Rect(40, 375, 220, 25), "Request Recharge", new GUIStyle(HighLogic.Skin.button));

            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            repairBtn = GUI.Button(new Rect(40, 400, 220, 25), "Request Repair", new GUIStyle(HighLogic.Skin.button));

            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            closeBtn = GUI.Button(new Rect(40, 425, 220, 25), "Cancel/Close", new GUIStyle(HighLogic.Skin.button));

            GUILayout.EndHorizontal();
            

            GUILayout.EndVertical();

            GUI.DragWindow();

        }

        // long winded way (but causes bugs otherwise) of invoking onFalse
        public void CloseMenu()
        {
            onFalse();
            onDisable();
            btnIsPresent = false;
            AddButton();
            btnIsPresent = true;
        }


        public void Start()
        {
            // get the icon from file, preload menu position

            if (HighLogic.LoadedSceneIsFlight)
            {

                grapesTexture = GameDatabase.Instance.GetTexture("FruitKocktail/GRAPES/Icons/grapes", false);
                guiPos = new Rect(menuPR, menuSR);
                rates = GasStation.ProvidePrices();

            }

            else
            {
                onDisable();
            }


        }


        public void Update()
        {

            if (HighLogic.LoadedSceneIsFlight)
            {
                // handles close button being pressed on menu

                if (closeBtn)
                {
                    CloseMenu();
                    closeBtn = false;
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

        }

        public void onFalse()
        {
            // ie when clicked off
            btnIsPressed = false;
        }

        public void onHover()
        {
            // ie on hover when not currently on

          
        }

        public void onHoverOut()
        {
            // ie when leave button when not currently on

          
        }

        public void onEnable()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
        }

        public void onDisable()
        {
            // ie when button is disabled / leave scene

            GameEvents.onGUIApplicationLauncherReady.Remove(AddButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(RemoveButton);
            ApplicationLauncher.Instance.RemoveModApplication(grapesBtn);

        }


        private static string GetFillUpCost()
        {
            string stringToReturn;
            double grabbedPrice = GasStation.GetFuelAmount();

            if (grabbedPrice == 0)
            {
                return "All Tanks Full!";
                
            }

            else
            {
                stringToReturn = grabbedPrice.ToString("0.00");
                return stringToReturn;
            }

        }

        private static string GetCredit()
        {
            string stringToReturn;
            double creditAmount = GasStation.GetCreditAmount();

            if (creditAmount == 0)
            {
                return "All Cards Are Empty!";
            }

            else
            {
                stringToReturn = creditAmount.ToString("0.00");
                return stringToReturn;
            }

        }

        private static string GetStatus()
        {
            string stringToReturn = GasStation.GetStatus();
            return stringToReturn;
        }

        private static string GetRechargeAbility()
        {
            bool canRecharge = GasStation.QueryRecharge();

            if (canRecharge)
            {
                return rates[5].ToString("0.00");
            }

            else
            {
                return "N/A";
            }

        }

        private static string GetRepairAbility()
        {

            return "N/A";

        }


    }
}
