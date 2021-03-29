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

        private Vector2 menuPR = new Vector2((Screen.width / 2) - 125, (Screen.height / 2) - 125);

        // menu size reference
        private Vector2 menuSR = new Vector2(250, 250);

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

            guiPos = GUILayout.Window(123456, guiPos, MenuWindow,
                "Welcome To Uranus Gas", new GUIStyle(HighLogic.Skin.window));

        }

        private static void MenuWindow(int windowID)
        {
            // menu defs

            GUILayout.BeginVertical();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Prices", new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(25);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Liquid Fuel/Oxidiser", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[0].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Liquid Fuel", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[1].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Oxidiser", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[2].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("MonoPropellant", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[3].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Xenon", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[4].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Recharge Service", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[5].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Repair Service", new GUIStyle(HighLogic.Skin.label));
            GUILayout.Space(25);
            GUILayout.Label(rates[6].ToString(), new GUIStyle(HighLogic.Skin.label));
            GUILayout.EndHorizontal();

            GUILayout.Space(25);

        /*    GUILayout.BeginHorizontal();

            closeBtn = GUI.Button(new Rect(20, 200, 160, 25), "Close", new GUIStyle(HighLogic.Skin.button));

            GUILayout.EndHorizontal();*/


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











    }
}
