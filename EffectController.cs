﻿﻿using UnityEngine;
using ColossalFramework;
using System.Text.RegularExpressions;
using ICities;

namespace Bordercities
{
    public class EffectController : MonoBehaviour
    {

        public enum ActiveStockPreset
        {
            Bordercities,
            BordercitiesEasier,
            BordercitiesGritty,
            BordercitiesBright,
            Sobelcities,
            SobelcitiesOD,
            SobelcitiesOD720,
            Cartoon,
            Realism,
            HighEndPC,
            LowEndMain,
            LowEndAlt,
            ClassicSobel,
            ClassicTriangle,
            Random,
            CartoonAlt,
            CartoonThree,
        }

        public bool showSettingsPanel = false;
        private Rect windowRect = new Rect(32, 32, 803, 700); //was 64,250,803,466
        private Rect dragBar = new Rect(0, 0, 803, 25);
        private Vector2 windowLoc;
        private float defaultHeight;
        private float defaultWidth;

        public Config config;
        public Preset preset;
        public PresetBank bank;

        private const string configPath = "BordercitiesConfig.xml";
        private const string bankPath = "BordercitiesPresetBank.xml";


        private EdgeDetection edge;
        private BloomOptimized bloom;
        private ToneMapping tonem;

        public Config.Tab tab;
        public bool autoEdge;
        public bool autoSobelEdge;
        public bool firstTime;
        private string displayTitle;
        private string displayText;
        private bool hasClicked = false;

        public bool overrideFirstTime;

        private CameraController cameraController;
        private InfoManager infoManager;
        public float oldToneMapBoost;
        public float toneMapBoost;
        public float oldGamma;
        public float toneMapGamma;

        private bool autoEdgeActive;
        private bool autoSobelEdgeActive;
        public bool subViewOnly;
        public bool useInfoModeSpecific;

        private string keystring = "";
        private string edgeKeyString = "";

        private bool automaticMode;

        public bool wantsToneMapper = false;
        public Color currentColor;
        public float setR;
        public float setG;
        public float setB;
        public float colorMultiplier = 1f;
        private Color newColor;

        public Color mixCurrentColor;
        public float mixSetR;
        public float mixSetG;
        public float mixSetB;
        public float mixColorMultiplier = 1f;
        private Color mixNewColor;

        private float defaultGamma;
        private float defaultBoost;
        private float prevGamma;
        private float prevBoost;


        public string[] presetEntries;

        private Color cartoonEdgeC;
        private Color cartoonMixC;
        private Color lowEndEdgeC;
        private Color realismEdgeC;
        private Color sobelcitiesODc;

        private Color lightRed;
        private Color lightGreen;
        private Color lightYellow;
        private Color lightBlue;
        private Color darkRed;
        private Color darkGreen;
        private Color darkYellow;
        private Color darkBlue;

        private ActiveStockPreset activeStockPreset;
        private InfoManager.InfoMode currentInfoMode;

        private bool isOn;
        private bool userIsPreviewing = false;
        private Color sobeCitiesC;

        public GUIStyle bordSkyStyle_header = null;
        public GUIStyle bordSkyStyle_critical = null;
        public GUIStyle bordSky_greenButton;
        public Texture2D greenTex;
        public GUIStyle bordSky_yellowButton;
        public Texture2D yellowTex;
        public GUIStyle bordSky_redButton;
        public Texture2D redTex;
        public GUISkin bordSkySkin = null;
        public float heightTester = 20f;

        public Vector2 scrollPosition;

        void OnGUI()
        {
            if (bordSkySkin == null)
                bordSkySkin = new GUISkin();
            if (bordSky_greenButton == null)
            {
                bordSky_greenButton = new GUIStyle(GUI.skin.button);
                bordSky_greenButton.normal.textColor = lightGreen;
                bordSky_greenButton.fontStyle = FontStyle.Bold;
                //greenTex = new Texture2D(1, 1);
                //greenTex.SetPixel(0, 0, darkGreen);
                //greenTex.Apply();
            }
            if (bordSky_redButton == null)
            {
                bordSky_redButton = new GUIStyle(GUI.skin.button);
                bordSky_redButton.normal.textColor = lightRed;
            }
            if (bordSky_yellowButton == null)
            {
                bordSky_yellowButton = new GUIStyle(GUI.skin.button);
                bordSky_yellowButton.normal.textColor = lightYellow;
            }
            if (bordSkyStyle_header == null)
                bordSkyStyle_header = new GUIStyle();
            if (bordSkyStyle_critical == null)
                bordSkyStyle_critical = new GUIStyle();

            bordSkySkin = GUI.skin;
            
            bordSkySkin.button.fixedHeight = 21;
            bordSkyStyle_header.fontStyle = FontStyle.Bold;
            bordSkyStyle_header.fontSize = 15;
            bordSkyStyle_header.normal.textColor = lightYellow;
            bordSkyStyle_critical.fontStyle = FontStyle.Bold;
            bordSkyStyle_critical.fontSize = 15;
            bordSkyStyle_critical.normal.textColor = lightGreen;
            bordSkySkin.button.padding = new RectOffset(5,5,0,0);


            if (firstTime)
                tab = Config.Tab.Hotkey;
            if (showSettingsPanel)
            {
                windowRect = GUI.Window(391435, windowRect, SettingsPanel, "Bordered Skylines (NEW: Draggable Window!)");
            }
        }


        void InitializeColors()
        {
            cartoonEdgeC = new Color(0.04f, 0.04f, 0.04f);
            lowEndEdgeC = new Color(0.13f, 0.13f, 0.13f);
            realismEdgeC = new Color(0.17f,0.17f,0.17f);
            sobeCitiesC = new Color(0.32f, 0.33f, 0.32f);
            sobelcitiesODc = new Color(0.35f, 0.35f, 0.35f);
            lightRed = new Color(1.0f, 0.6f, 0.6f);
            lightGreen = new Color(0.6f, 1.0f, 0.6f);
            lightBlue = new Color(0.6f, 0.6f, 1.0f);
            lightYellow = new Color(1.0f, 1.0f, 0.6f);
            darkRed = new Color(0.4f, 0.0f, 0.0f);
            darkGreen = new Color(0.0f, 0.4f, 0.0f);
            darkBlue = new Color(0.0f, 0.0f, 0.4f);
            darkYellow = new Color(0.4f, 0.4f, 0.0f);
            
        }



        void SettingsPanel(int wnd)
        {
            //heightTester = GUILayout.HorizontalSlider(heightTester, 2f, 100f);
           // GUILayout.Label(heightTester.ToString("F0"));
            


            GUI.DragWindow(dragBar);
            #region Top Navigation Buttons
            GUILayout.BeginHorizontal();
            if (!firstTime || !automaticMode)
            {
                if (GUILayout.Button("Main"))
                {
                    tab = Config.Tab.EdgeDetection;
                }
                if (!automaticMode)
                {
                    if (GUILayout.Button("Bonus Effects"))
                    {
                        tab = Config.Tab.Bloom;
                    }
                    if (GUILayout.Button("Presets"))
                    {
                        LoadBank();
                        tab = Config.Tab.Presets;

                    }
                }
                if (useInfoModeSpecific)
                {
                    if (GUILayout.Button("Info Modes"))
                    {
                        tab = Config.Tab.ViewModes;
                    }
                }
                if (GUILayout.Button("Hotkey Configuration"))
                {
                    tab = Config.Tab.Hotkey;
                }

            }

            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
            #endregion
            #region Tab - Edge Detection
            if (tab == Config.Tab.EdgeDetection)
            {
                ResizeDefaults();
                if (edge != null)
                {
                    if (!isOn)
                    {
                        ResizeWindow(803, 190);
                        GUILayout.Space(30f);
                        if (GUILayout.Button("Enable Bordered Skylines", bordSky_greenButton))
                        {
                            if (firstTime && automaticMode)
                                LowEndAutomatic();
                            ToggleBorderedSkylines(true);
                        }
                        GUILayout.Space(30f);

                    }
                    if (isOn)
                    {

                        if (automaticMode)
                        {
                            ResizeWindow(803, 710);

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("PLUG & PLAY PRESETS:", bordSkyStyle_header);
                            GUILayout.Label("Use whichever of the below presets looks best to you, regardless of the listed resolutions.  The resolutions are simply recommendations to ensure a quick & easy setup for you.  Know that the higher a suggested intended resolution, the thicker the edges in that preset will look at lower resolutions. Dynamic Resolution (DR) strongly recommended.");
                            GUILayout.EndHorizontal();
                            GUILayout.Space(6f);



                            //scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(750), GUILayout.Height(200)); 

                            GUILayout.BeginHorizontal();
                            GUILayout.BeginVertical();
                            GUILayout.Label("720p/1080p & NO DR:", bordSkyStyle_header, GUILayout.Width(165));
                            GUILayout.Space(5f);
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Sobel Skies", GUILayout.Width(79.5f)))
                            {
                                LowEndAutomatic();
                            }
                            if (GUILayout.Button("SS++", GUILayout.Width(85.5f)))
                            {
                                SobelcitiesOD720Automatic();
                            }
                            GUILayout.EndHorizontal();
                            if (GUILayout.Button("Classic Auto-Sobel", GUILayout.Width(165)))
                            {
                                ClassicSobelAutomatic();
                            }
                            if (GUILayout.Button("Classic Auto-Triangle", GUILayout.Width(165)))
                            {
                                ClassicTriangleAutomatic();
                            }
                            if (GUILayout.Button("AT|Eyefriendly(er)", GUILayout.Width(165)))
                            {
                                LowEndAltAutomatic();
                                GUI.color = darkGreen;
                            }
                            GUILayout.EndVertical();


                            GUILayout.BeginVertical();
                            GUILayout.Label("1080p & 175+ DR", bordSkyStyle_header, GUILayout.Width(165));
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Sobelcities", GUILayout.Width(90.5f)))
                            {
                                SobelcitiesAutomatic();
                            }
                            if (GUILayout.Button("SC++", GUILayout.Width(74.5f)))
                            {
                                SobelcitiesODAutomatic();
                            }
                            GUILayout.EndHorizontal();
                            if (GUILayout.Button("Bordercities", GUILayout.Width(165)))
                            {
                                BordercitiesAutomatic();
                            }
                            if (wantsToneMapper)
                            {
                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button("BC|Bright", GUILayout.Width(82.5f)))
                                {
                                    BordercitiesBrightAutomatic();
                                }
                                if (GUILayout.Button("BC|Grit", GUILayout.Width(82.5f)))
                                {
                                    BordercitiesGrittyAutomatic();
                                }
                                GUILayout.EndHorizontal();
                            }
                            if (GUILayout.Button("BC|EasierViewing", GUILayout.Width(165)))
                            {
                                BordercitiesEasyViewingAutomatic();
                            }
                            if (GUILayout.Button("Realism", GUILayout.Width(165)))
                            {
                                RealismAutomatic();
                            }
                            if (GUILayout.Button("Cartoon", GUILayout.Width(165)))
                            {
                                CartoonThreeAutomatic();
                            }
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Retro", GUILayout.Width(72.5f)))
                            {
                                CartoonAutomatic();
                            }
                            if (GUILayout.Button("Colorful", GUILayout.Width(92.5f)))
                            {
                                CartoonAltAutomatic();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();

                            GUILayout.BeginVertical();
                            GUILayout.Label("1080p & 250+ DR:", bordSkyStyle_header, GUILayout.Width(165));
                            if (GUILayout.Button("Ultra", GUILayout.MaxWidth(165)))
                            {
                                UltraAutomatic();
                            }
                            GUILayout.EndVertical();

                            GUILayout.BeginVertical();
                            GUILayout.Label("ARTISTIC", bordSkyStyle_header, GUILayout.Width(165));
                            if (GUILayout.Button("Random (WARNING:Bright)", GUILayout.MaxWidth(190)))
                            {
                                RandomAutomatic();
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                            //GUILayout.EndScrollView();
                            if (displayText != null && displayTitle != null)
                            {
                                GUILayout.Space(5f);
                                GUILayout.Label("MAKE SURE TO SAVE (bottom of panel) WHEN YOU'VE FOUND YOUR FAVORITE!", bordSkyStyle_critical);
                                GUILayout.Space(5f);
                                GUILayout.Label("CURRENT MODE: " + displayTitle, bordSkyStyle_header);
                                GUILayout.Space(3f);
                                GUILayout.Label(displayText);
                            }
                            if (activeStockPreset == ActiveStockPreset.Cartoon || activeStockPreset == ActiveStockPreset.CartoonAlt)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("CARTOON SPECIFIC OPTIONS:", bordSkyStyle_header);
                                if (GUILayout.Button("Randomize Color Theme)"))
                                {
                                    Color tempColor = new Color(Random.Range(0.00f, 1.00f), Random.Range(0.00f, 1.00f), Random.Range(0.00f, 1.00f));
                                    cartoonMixC = tempColor;
                                    edge.edgesOnlyBgColor = cartoonMixC;
                                    mixSetR = edge.edgesOnlyBgColor.r;
                                    mixSetG = edge.edgesOnlyBgColor.g;
                                    mixSetB = edge.edgesOnlyBgColor.b;
                                    setR = edge.edgeColor.r;
                                    setG = edge.edgeColor.g;
                                    setB = edge.edgeColor.b;
                                    mixColorMultiplier = 1.0f;
                                    colorMultiplier = 1.0f;
                                }
                                if (GUILayout.Button("Reset Color Theme"))
                                {
                                    cartoonMixC = Color.white;
                                    edge.edgesOnlyBgColor = cartoonMixC;
                                    mixSetR = 1.0f;
                                    mixSetG = 1.0f;
                                    mixSetB = 1.0f;
                                    setR = 1.0f;
                                    setG = 1.0f;
                                    setB = 1.0f;
                                    mixColorMultiplier = 1.0f;
                                    colorMultiplier = 1.0f;
                                }
                                GUILayout.EndHorizontal();
                            }
                           
                            GUILayout.Space(11f);
                            GUILayout.Label("If you are unsatisfied with these stock presets, or wish to build from scratch:", bordSkyStyle_header);
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Enter Advanced Mode", bordSky_yellowButton, GUILayout.Height(27)))
                            {
                                automaticMode = false;

                            }
                            GUILayout.EndHorizontal();
                            if (!firstTime)
                            {
                                GUILayout.Space(2f);
                                if (config.edgeToggleKeyCode == KeyCode.None)
                                    GUILayout.Label("Did you know: you can set a hotkey for toggling the effect?  See 'Hotkey Config' tab!'", bordSkyStyle_header);
                                GUILayout.Space(2f);
                            }
                            else
                                GUILayout.Space(5f);



                        }
                        else
                        {
                            GUILayout.Space(5f);
                            ResizeWindow(803, 700);
                            GUILayout.Label("ADVANCED MODE:", bordSkyStyle_header);
                            switch (edge.mode)
                            {
                                case EdgeDetection.EdgeDetectMode.TriangleDepthNormals:
                                    {
                                        GUILayout.Label("Edge mode: Triangle Depth Normals");
                                        break;
                                    }
                                case EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals:
                                    {
                                        GUILayout.Label("Edge mode: Roberts Cross Depth Normals");
                                        break;
                                    }
                                case EdgeDetection.EdgeDetectMode.SobelDepth:
                                    {
                                        GUILayout.Label("Edge mode: Classic 'Sobel Depth'");
                                        break;
                                    }
                                case EdgeDetection.EdgeDetectMode.SobelDepthThin:
                                    {
                                        GUILayout.Label("Edge mode: 'Sobel Skylines'");
                                        break;
                                    }
                                default:
                                    {
                                        GUILayout.Label("Edge mode:");
                                        break;
                                    }
                            }
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Triangle Depth Normals"))
                            {

                                edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
                                edge.SetCameraFlag();
                            }
                            if (GUILayout.Button("Roberts Cross Depth Normals"))
                            {
                                edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
                                edge.SetCameraFlag();
                            }
                            if (GUILayout.Button("Classic Sobel"))
                            {
                                edge.mode = EdgeDetection.EdgeDetectMode.SobelDepth;
                                edge.SetCameraFlag();
                            }
                            if (GUILayout.Button("'Sobel Skylines' (New)"))
                            {
                                edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                                edge.SetCameraFlag();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Label("Edge distance: " + (edge.sampleDist - 0.5f).ToString("F0"));
                            edge.sampleDist = GUILayout.HorizontalSlider(edge.sampleDist, 1, 5, GUILayout.MaxWidth(100));
                            GUILayout.Label("Mix: " + edge.edgesOnly.ToString("F2"));
                            edge.edgesOnly = GUILayout.HorizontalSlider(edge.edgesOnly, 0.000f, 1.000f, GUILayout.MaxWidth(500));


                            if (edge.mode == EdgeDetection.EdgeDetectMode.TriangleDepthNormals || edge.mode == EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals)
                            {
                                autoEdge = GUILayout.Toggle(autoEdge, "Automatic zoom-level compensation?");
                                GUILayout.Space(15f);
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Depth sensitivity: " + edge.sensitivityDepth.ToString("F2"), GUILayout.MaxWidth(300));
                                if (!autoEdge)
                                    edge.sensitivityDepth = GUILayout.HorizontalSlider(edge.sensitivityDepth, 0.000f, 50.000f, GUILayout.MaxWidth(125));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Normal sensitivity: " + edge.sensitivityNormals.ToString("F2"), GUILayout.MaxWidth(300));
                                if (!autoEdge)
                                    edge.sensitivityNormals = GUILayout.HorizontalSlider(edge.sensitivityNormals, 0.000f, 5.000f, GUILayout.MaxWidth(125));
                                GUILayout.EndHorizontal();
                                GUILayout.Space(15f);

                            }
                            if (edge.mode == EdgeDetection.EdgeDetectMode.SobelDepthThin)
                            {


                                GUILayout.Space(5f);
                                autoSobelEdge = GUILayout.Toggle(autoSobelEdge, "Automatic zoom-level compensation?");

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Diagonal Depth: " + edge.depthsDiagonal.ToString("F1"), GUILayout.MaxWidth(150));
                                //if (!autoSobelEdge)
                                edge.depthsDiagonal = GUILayout.HorizontalSlider(edge.depthsDiagonal, 0.000f, 1.000f);
                                GUILayout.EndHorizontal();
                                if (!autoSobelEdge)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Axis/Center: " + edge.axisVsCenter.ToString("F3"), GUILayout.MaxWidth(150));
                                    edge.axisVsCenter = GUILayout.HorizontalSlider(edge.axisVsCenter, 0.001f, 1.000f);
                                    GUILayout.EndHorizontal();
                                }


                                GUILayout.Space(5f);

                                GUILayout.BeginHorizontal();
                                if (autoSobelEdge)
                                    GUILayout.Label("Horizontal Offset:" + edge.mult1.ToString("F3"), GUILayout.MaxWidth(150));
                                else
                                    GUILayout.Label("Horizontal:" + edge.mult1.ToString("F3"), GUILayout.MaxWidth(150));
                                edge.mult1 = GUILayout.HorizontalSlider(edge.mult1, 0.000f, 10.100f);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                if (!autoSobelEdge)
                                {
                                    GUILayout.Label("Fine-tune H:" + edge.mult3.ToString("F3"), GUILayout.MaxWidth(150));
                                    edge.mult3 = GUILayout.HorizontalSlider(edge.mult3, 0.000f, 10.000f);
                                }

                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();

                                if (autoSobelEdge)
                                    GUILayout.Label("Vertical Offset:" + edge.mult2.ToString("F3"), GUILayout.MaxWidth(150));
                                else
                                    GUILayout.Label("Vertical:" + edge.mult2.ToString("F3"), GUILayout.MaxWidth(150));

                                edge.mult2 = GUILayout.HorizontalSlider(edge.mult2, 0.000f, 20.000f);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                if (!autoSobelEdge)
                                {
                                    GUILayout.Label("Fine-tune V: " + edge.mult4.ToString("F3"), GUILayout.MaxWidth(150));
                                    edge.mult4 = GUILayout.HorizontalSlider(edge.mult4, 0.000f, 10.000f);
                                }
                                GUILayout.EndHorizontal();
                                if (GUILayout.Button("Reset SobelSkylines-specfic parameters"))
                                {
                                    edge.mult1 = 1.750f;
                                    edge.mult2 = 3.750f;
                                    edge.mult3 = 1.0f;
                                    edge.mult4 = 1.0f;
                                }

                                GUILayout.Space(3f);


                            }
                            if (edge.mode == EdgeDetection.EdgeDetectMode.SobelDepth)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Edge exponent: " + edge.edgeExp.ToString("F2"), GUILayout.MaxWidth(50));
                                edge.edgeExp = GUILayout.HorizontalSlider(edge.edgeExp, 0.004f, 1.000f, GUILayout.MaxWidth(100));
                                GUILayout.EndHorizontal();
                            }

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("R " + setR.ToString("F2"), GUILayout.MaxWidth(10f));
                            setR = GUILayout.HorizontalSlider(setR, 0.000f, 3.000f, GUILayout.Width(90));


                            GUILayout.Label("G " + setG.ToString("F2"), GUILayout.MaxWidth(10f));
                            setG = GUILayout.HorizontalSlider(setG, 0.000f, 3.000f, GUILayout.Width(90));


                            GUILayout.Label("B " + setB.ToString("F2"), GUILayout.MaxWidth(10f));
                            setB = GUILayout.HorizontalSlider(setB, 0.000f, 3.000f, GUILayout.Width(90));

                            GUILayout.Label("x" + colorMultiplier.ToString("F1"), GUILayout.MaxWidth(20f));
                            colorMultiplier = GUILayout.HorizontalSlider(colorMultiplier, 0.0f, 10.0f, GUILayout.Width(90));

                            if (GUILayout.Button("Apply Edge Color", GUILayout.MaxWidth(120)))
                            {
                                EdgeColor(setR, setG, setB);
                                EdgeColor(setR, setG, setB);
                            }
                            if (GUILayout.Button("Black", GUILayout.MaxWidth(55)))
                            {
                                setR = 0.0f;
                                setG = 0.0f;
                                setB = 0.0f;
                                colorMultiplier = 1.0f;
                                EdgeColor(0.0f, 0.0f, 0.0f);
                                EdgeColor(0.0f, 0.0f, 0.0f);
                            }
                            GUILayout.EndHorizontal();




                            GUILayout.BeginHorizontal();
                            GUILayout.Label("R " + mixSetR.ToString("F2"), GUILayout.MaxWidth(10f));
                            mixSetR = GUILayout.HorizontalSlider(mixSetR, 0.000f, 3.000f, GUILayout.Width(90));


                            GUILayout.Label("G " + mixSetG.ToString("F2"), GUILayout.MaxWidth(10f));
                            mixSetG = GUILayout.HorizontalSlider(mixSetG, 0.000f, 3.000f, GUILayout.Width(90));


                            GUILayout.Label("B " + mixSetB.ToString("F2"), GUILayout.MaxWidth(10f));
                            mixSetB = GUILayout.HorizontalSlider(mixSetB, 0.000f, 3.000f, GUILayout.Width(90));

                            GUILayout.Label("x" + mixColorMultiplier.ToString("F2"), GUILayout.MaxWidth(20f));
                            mixColorMultiplier = GUILayout.HorizontalSlider(mixColorMultiplier, 0.0f, 10.0f, GUILayout.Width(90));

                            if (GUILayout.Button("Apply Mix Color", GUILayout.MaxWidth(120)))
                            {
                                MixColor(mixSetR, mixSetG, mixSetB);
                                MixColor(mixSetR, mixSetG, mixSetB);
                            }
                            if (GUILayout.Button("White", GUILayout.MaxWidth(55)))
                            {
                                mixSetR = 1.0f;
                                mixSetG = 1.0f;
                                mixSetB = 1.0f;
                                mixColorMultiplier = 1.0f;
                                MixColor(1.0f, 1.0f, 1.0f);
                                MixColor(1.0f, 1.0f, 1.0f);
                            }

                            GUILayout.EndHorizontal();



                            if (infoManager.CurrentMode == InfoManager.InfoMode.None)
                            {


                            }
                            else
                            {
                                GUILayout.Label("Gamma and boost settings have no effect in View Modes.");
                            }
                            GUILayout.Space(5f);

                            if (GUILayout.Button("Switch back to 'Plug & Play' Mode.", bordSky_yellowButton))
                            {
                                automaticMode = true;
                                DetermineMode();
                            }

                            GUILayout.Space(5);
                        }
                    }
                }
            }
            #endregion
            #region Tab - Bloom
            if (tab == Config.Tab.Bloom)
            {
                ResizeWindow(803, 415);
                GUILayout.Label("NOTE: There is already a Bloom effect in Cities Skylines, and it is quite better than what Bordered Skylines provides here.  However, because they both achieve a different effect, Bordered Skylines maintains this bloom in case you wish to stack both bloom effects.");
                GUILayout.Space(5f);
                if (!bloom.enabled)
                    bloom.enabled = GUILayout.Toggle(bloom.enabled, "Click to enable Bloom.");
                else
                    bloom.enabled = GUILayout.Toggle(bloom.enabled, "Click to disable Bloom.");

                if (bloom.enabled)
                {
                    GUILayout.Label("Threshold: " + bloom.threshold.ToString("F2"));
                    bloom.threshold = GUILayout.HorizontalSlider(bloom.threshold, 0.00f, 1.50f, GUILayout.Width(570));
                    GUILayout.Label("Intensity: " + bloom.intensity.ToString("F2"));
                    bloom.intensity = GUILayout.HorizontalSlider(bloom.intensity, 0.00f, 2.50f, GUILayout.Width(570));
                    GUILayout.Label("Blur size: " + bloom.blurSize.ToString("F2"));
                    bloom.blurSize = GUILayout.HorizontalSlider(bloom.blurSize, 0.00f, 5.50f, GUILayout.Width(570));
                }

            }
            #endregion
            #region Tab - Hotkey
            if (tab == Config.Tab.Hotkey)
            {
                if (firstTime)
                {
                    ResizeWindow(575, 400);
                    GUILayout.Label("BORDERED SKYLINES FIRST-TIME INITIALIZATION (Never popups again after choice)");
                    GUILayout.Label("Choose and confirm your hotkey for Bordered Skylines.  LeftBracket is default.");
                    GUILayout.Label("NOTE: Bordered Skylines will -never- automatically pop-up again as soon as you've confirmed your hotkey choice.   This initialization process ensures that all users, regardless of hardware, operating system, or current keyboard configuration, will be able to enjoy Bordered Skylines.");


                }


                if (!firstTime)
                {
                    ResizeWindow(538, 600);
                    GUILayout.Label("WARNING: HOTKEY BUTTONS WILL SAVE UPON CLICK.  THIS INCLUDES YOUR EFFECTS SETTINGS.  If you wish to create a safe backup of your active Effect, navigate to the 'XML' tab above and create a permanent copy before selecting a hotkey here.  This will not be like this for much longer.");

                }
                KeyboardGrid(0);




                if (firstTime && config.keyCode != KeyCode.None)
                {
                    GUILayout.Space(3f);
                    if (hasClicked)
                        GUILayout.Label("Hotkey '" + KeyToString(config.keyCode) + "' has been chosen and is active.  Confirm it now by using the hotkey.");
                    GUILayout.Space(10f);
                    GUILayout.Label("NOTE: Hotkey can be changed at anytime via the 'Hotkey' window tab in the config panel.");
                }

                if (!firstTime)
                {
                    if (config.keyCode != KeyCode.None)
                        GUILayout.Label("Current 'Config' hotkey: " + config.keyCode);
                    else
                        GUILayout.Label("No config hotkey is bound to Bordered Skylines.");
                    GUILayout.Space(45f);
                    GUILayout.Label("Set edge toggle hotkey below: ");
                    KeyboardGrid(1);
                    if (config.edgeToggleKeyCode != KeyCode.None)
                        GUILayout.Label("Current 'Edge Enable' hotkey: " + config.edgeToggleKeyCode);
                    else
                        GUILayout.Label("No edge enable hotkey is bound to Bordered Skylines.");
                    GUILayout.Space(5f);
                    GUILayout.Label("More key options coming soon!");
                }

            }
            #endregion
            #region Tab - Presets
            if (tab == Config.Tab.Presets)
            {
                ResizeWindow(600, 750);
                GUILayout.Label("NOTE: Your preset bank list is saved automatically, and as a whole, upon either saving any of your fields, or, proper exit of the game.  Note that your changes to this list will NOT be saved in the event of an Alt-F4 or other similarly graceless exit.  You will the preset files stored within steamapps/common/Cities_Skylines/BordercitiesPresets.");

                GUILayout.Space(2f);

                for (int i = 0; i < presetEntries.Length; i++)
                {
                    GUILayout.BeginHorizontal();

                    presetEntries[i] = GUILayout.TextField(presetEntries[i], 31, GUILayout.MaxWidth(280));
                    presetEntries[i] = Regex.Replace(presetEntries[i], @"[^a-zA-Z0-9 ]", "");
                    if (GUILayout.Button("Save", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        if (IsValidFilename(presetEntries[i]))
                        {
                            SavePreset(presetEntries[i], true);
                            SavePreset(presetEntries[i], true);
                            SaveBank();
                        }

                    }
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        LoadPreset(presetEntries[i], true);
                    }

                    i++;

                    presetEntries[i] = GUILayout.TextField(presetEntries[i], 31, GUILayout.MaxWidth(280));
                    presetEntries[i] = Regex.Replace(presetEntries[i], @"[^a-zA-Z0-9 ]", "");
                    if (GUILayout.Button("Save", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        if (IsValidFilename(presetEntries[i]))
                        {
                            SavePreset(presetEntries[i], true);
                            SavePreset(presetEntries[i], true);

                        }
                    }
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(60), GUILayout.MaxHeight(25)))
                    {
                        LoadPreset(presetEntries[i], true);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("Support for custom presets is sort of thrown together for the moment. The name of the input field must match the filename, and is case sensitive.  Having a nicer preset browser is on the todo list.");

                if (GUILayout.Button("Reset Bank (This will save!)"))
                {
                    ResetBank();
                }

            }
            #endregion
            #region View Modes
            if (tab == Config.Tab.ViewModes)
            {
                ResizeWindow(800, 755);

                ViewModeGUI("Building Level", "BuildingLevel", existBuildingLevel, "Connections", "Connections", existConnections);
                ViewModeGUI("Crime Rate", "CrimeRate", existCrimeRate, "Density", "Density", existDensity);
                ViewModeGUI("Districts", "Districts", existDistricts, "Education", "Education", existEducation);
                ViewModeGUI("Electricity", "Electricity", existElectricity, "Entertainment", "Entertainment", existEntertainment);
                ViewModeGUI("Fire Safety", "FireSafety", existFireSafety, "Garbage", "Garbage", existGarbage);
                ViewModeGUI("Happiness", "Happiness", existHappiness, "Health", "Health", existHealth);
                ViewModeGUI("Land Value", "LandValue", existLandValue, "Natural Resources", "NaturalResources", existNaturalResources);
                ViewModeGUI("Noise Pollution", "NoisePollution", existNoisePollution, "Pollution", "Pollution", existPollution);
                ViewModeGUI("Terrain Height", "TerrainHeight", existTerrainHeight, "Traffic", "Traffic", existTraffic);
                ViewModeGUI("Transport", "Transport", existTransport, "Water", "Water", existWater);
                ViewModeGUI("Wind", "Wind", existWind);


                GUILayout.Space(15);
                if (automaticMode)
                    GUILayout.Label("Dial in custom colors! You'll want to go easy on 'Mix' so that blue<->red gameplay info remains discernible.");
                else
                    GUILayout.Label("Advanced Mode detected: For clarity, these are the exact same color sliders from the main page.  You can technically mix and match all possible settings, and per info mode, if you so desire.  The colors are here as a shortcut, as it is assumed that you'll want to use very different colors for each Info Mode.");

                GUILayout.Space(5);
                GUILayout.Label("Mix: " + edge.edgesOnly.ToString("F2"));
                edge.edgesOnly = GUILayout.HorizontalSlider(edge.edgesOnly, 0.000f, 1.000f, GUILayout.MaxWidth(500));
                GUILayout.BeginHorizontal();
                GUILayout.Label("R " + setR.ToString("F2"), GUILayout.MaxWidth(10f));
                setR = GUILayout.HorizontalSlider(setR, 0.000f, 3.000f, GUILayout.Width(90));


                GUILayout.Label("G " + setG.ToString("F2"), GUILayout.MaxWidth(10f));
                setG = GUILayout.HorizontalSlider(setG, 0.000f, 3.000f, GUILayout.Width(90));


                GUILayout.Label("B " + setB.ToString("F2"), GUILayout.MaxWidth(10f));
                setB = GUILayout.HorizontalSlider(setB, 0.000f, 3.000f, GUILayout.Width(90));

                GUILayout.Label("x" + colorMultiplier.ToString("F1"), GUILayout.MaxWidth(20f));
                colorMultiplier = GUILayout.HorizontalSlider(colorMultiplier, 0.0f, 10.0f, GUILayout.Width(90));

                if (GUILayout.Button("Apply Edge Color", GUILayout.MaxWidth(120)))
                {
                    EdgeColor(setR, setG, setB);
                    EdgeColor(setR, setG, setB);
                }
                if (GUILayout.Button("Black", GUILayout.MaxWidth(55)))
                {
                    setR = 0.0f;
                    setG = 0.0f;
                    setB = 0.0f;
                    colorMultiplier = 1.0f;
                    EdgeColor(0.0f, 0.0f, 0.0f);
                    EdgeColor(0.0f, 0.0f, 0.0f);
                }
                GUILayout.EndHorizontal();




                GUILayout.BeginHorizontal();
                GUILayout.Label("R " + mixSetR.ToString("F2"), GUILayout.MaxWidth(10f));
                mixSetR = GUILayout.HorizontalSlider(mixSetR, 0.000f, 3.000f, GUILayout.Width(90));


                GUILayout.Label("G " + mixSetG.ToString("F2"), GUILayout.MaxWidth(10f));
                mixSetG = GUILayout.HorizontalSlider(mixSetG, 0.000f, 3.000f, GUILayout.Width(90));


                GUILayout.Label("B " + mixSetB.ToString("F2"), GUILayout.MaxWidth(10f));
                mixSetB = GUILayout.HorizontalSlider(mixSetB, 0.000f, 3.000f, GUILayout.Width(90));

                GUILayout.Label("x" + mixColorMultiplier.ToString("F2"), GUILayout.MaxWidth(20f));
                mixColorMultiplier = GUILayout.HorizontalSlider(mixColorMultiplier, 0.0f, 10.0f, GUILayout.Width(90));

                if (GUILayout.Button("Apply Mix Color", GUILayout.MaxWidth(120)))
                {
                    MixColor(mixSetR, mixSetG, mixSetB);
                    MixColor(mixSetR, mixSetG, mixSetB);
                }
                if (GUILayout.Button("White", GUILayout.MaxWidth(55)))
                {
                    mixSetR = 1.0f;
                    mixSetG = 1.0f;
                    mixSetB = 1.0f;
                    mixColorMultiplier = 1.0f;
                    MixColor(1.0f, 1.0f, 1.0f);
                    MixColor(1.0f, 1.0f, 1.0f);
                }
                GUILayout.EndHorizontal();
                if (infoManager.CurrentMode != InfoManager.InfoMode.None)
                {
                    if (GUILayout.Button("Set '" + GetCurrentInfoModeLabelString() + "'! (Shortcut: Active Info Mode.)"))
                    {
                        QuicksaveActiveViewMode();
                    }
                }
                else
                {
                    if (GUILayout.Button("Not currently in an Info Mode."))
                    {

                    }
                }
                GUILayout.Space(10f);
                if (GUILayout.Button("Return to configuration tab"))
                {
                    tab = Config.Tab.EdgeDetection;
                }
                GUILayout.Space(3f);


                GUILayout.Label("Any 'Set' for a given Info Mode is generated and saved to disk ('steamapps/common/Cities_Skylines/BordercitiesPresets/InfoModes') in its own dedicated XML file (IE 'Wind.xml' 'Water.xml'), and only upon clicking will the file be created, if not already existing.  Your main config will be loaded in the absence of a custom 'Info Mode' XML.  When clicking 'Set', whichever settings are currently on-screen will be the settings saved to your desired Info Mode's XML file -- regardless of whether or not you had first pressed 'Save Settings'.  For ease of understanding, know that your main config is entirely separate from the Info-Mode XML database here, as well as separate from the general 'Custom' XML config-saves located in the 'Custom' tab.  Your primary config (the one that auto-loads when you start the game via 'Save Settings') is the configuration that will be automatically loaded upon exiting an Info Mode (if you are also not using 'Effect Enabled in Info Modes Only.')");

                GUILayout.Space(4);



            }





            #endregion

            #region Bottom Navigation
            if (!firstTime)
            {
                if (tab == Config.Tab.EdgeDetection && isOn)
                {
                    subViewOnly = GUILayout.Toggle(subViewOnly, "Optional: Effect enabled within 'Info Modes' only? (Auto-disables/enables accordingly)");
                    useInfoModeSpecific = GUILayout.Toggle(useInfoModeSpecific, "Optional: Use 'Info-Mode'-specific Presets? (upon activation, 'Info Modes' tab will appear above.)");
                    wantsToneMapper = GUILayout.Toggle(wantsToneMapper, "Optional: Incorporate brightness/gamma settings into Bordered Skylines' functionality? (Plug & Play/Advanced share this)");
                    if (wantsToneMapper)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Gamma: " + toneMapGamma.ToString("F2"), GUILayout.MaxWidth(60));
                        toneMapGamma = GUILayout.HorizontalSlider(toneMapGamma, 0.0f, 30.0f, GUILayout.MaxWidth(100));

                        GUILayout.Label("Boost: " + toneMapBoost.ToString("F2"), GUILayout.MaxWidth(60));
                        toneMapBoost = GUILayout.HorizontalSlider(toneMapBoost, 0.0f, 30.0f, GUILayout.MaxWidth(100));
                        GUILayout.EndHorizontal();
                    }
                    if (isOn)
                    {
                        if (GUILayout.Button("Disable Bordered Skylines", bordSky_redButton))
                        {
                            ToggleBorderedSkylines(false);
                        }
                    }
                }
                GUILayout.BeginHorizontal();
                if (tab != Config.Tab.Hotkey)
                {

                    
                        if (GUILayout.Button("Reset Brightness") && tab != Config.Tab.Presets)
                        {
                            ResetTonemapper();
                        }
                    
                    if (!automaticMode)
                    {
                        if (tab != Config.Tab.Presets)
                        {
                            if (GUILayout.Button("Load from last saved"))
                            {
                                LoadConfig(true);
                            }
                        }

                    }
                    if (tab != Config.Tab.Presets)
                    {
                        if (automaticMode && tab == Config.Tab.EdgeDetection)
                        {
                            if (isOn)
                            {
                                if (GUILayout.Button("Save (Active preset will load by default in future sessions)", bordSky_greenButton))
                                {
                                    SaveConfig();
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Save (Bordered Skylines will be Disabled by default in future sessions)", bordSky_yellowButton))
                                {
                                    SaveConfig();
                                }
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Save (Your active configuration will load by default in future sessions", bordSky_greenButton))
                            {
                                SaveConfig();
                            }
                        }
                    }
                    else
                    {

                        if (GUILayout.Button("Save the current look as default"))
                        {
                            SaveConfig();
                        }
                    }
                }

                if (GUILayout.Button("Close Window"))
                {
                    showSettingsPanel = false;
                    if (!overrideFirstTime)
                        overrideFirstTime = true;
                }

                GUILayout.EndHorizontal();


            }
            #endregion
            dragBar.width = windowRect.width;
            windowLoc.x = windowRect.x;
            windowLoc.y = windowRect.y;

        }

       

        void Awake()
        {
            InitializeColors();

            cameraController = GetComponent<CameraController>();
            infoManager = InfoManager.instance;
            currentInfoMode = infoManager.CurrentMode;
            config = Config.Deserialize(configPath);
            bank = PresetBank.Deserialize(bankPath);
            
            defaultWidth = windowRect.width;
            defaultHeight = windowRect.height;

            InitializeFromConfig();
            InitializeBanking();
            InitializeExistBools();

        }

        

        void InitializeFromConfig()
        {
            if (config == null) 
            {
                config = new Config();
                #region parameters#
                config.automaticMode = true;
                config.edgeEnabled = false;
                config.edgeMode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                config.edgeSamp = 1.0f;
                config.edgeOnly = 0;
                //TriangleRoberts
                config.sensNorm = 1.63f;
                config.sensDepth = 2.12f;
                config.autoEdge = true;

                //Sobel
                config.edgeExpo = 0.09f;
                config.depthsAxis = 0.100f;
                config.depthsDiagonal = 1.000f;
                config.axisVsCenter = 0.100f;
                config.sobelMult1 = 1.000f;
                config.sobelMult2 = 1.000f;
                config.sobelMult3 = 1.000f;
                config.sobelMult4 = 1.000f;
                config.autoSobelEdge = true;

                config.wantsTonemapper = false;

                config.autoSobelEdge = true;
                config.firstTime = true;

                config.subViewOnly = false;
                config.useInfoModeSpecific = false;

                config.oldGamma = 2.2f;
                config.toneMapGamma = 2.2f;
                config.oldBoost = 1.15f;
                config.toneMapBoost = 1.15f;

                config.currentColor = new Color(0, 0, 0, 0);
                config.colorMultiplier = 1.0f;

                config.mixColorMultiplier = 1.0f;
                config.mixCurrentColor = new Color(1, 1, 1, 0);

                config.bloomEnabled = false;
                config.bloomThresh = 0.27f;
                config.bloomIntens = 0.39f;
                config.bloomBlurSize = 5.50f;

                config.cartoonMixColor = Color.white;
                config.activeStockPreset = ActiveStockPreset.LowEndMain;
                config.windowLoc = new Vector2(32, 32);

                
                
                #endregion
            }
            else 
            {
                #region parameters
                if (IsNull(config.automaticMode))
                    config.automaticMode = true;
                if (IsNull(config.edgeEnabled))
                    config.edgeEnabled = false;
                if (IsNull(config.edgeMode))
                    config.edgeMode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
                if (IsNull(config.edgeSamp))
                    config.edgeSamp = 1.0f;
                if (IsNull(config.edgeOnly))
                    config.edgeOnly = 0;

                if (IsNull(config.wantsTonemapper))
                    config.wantsTonemapper = false;

                if (IsNull(config.sensNorm))
                    config.sensNorm = 1.63f;
                if (IsNull(config.sensDepth))
                    config.sensDepth = 2.12f;


                if (IsNull(config.edgeExpo))
                    config.edgeExpo = 0.09f;
                if (IsNull(config.depthsDiagonal))
                    config.depthsDiagonal = 1.0f;
                if (IsNull(config.depthsAxis))
                    config.depthsAxis = 1.0f;
                if (IsNull(config.depthsAxis))
                    config.axisVsCenter = 0.100f;
                if (IsNull(config.sobelMult1))
                    config.sobelMult1 = 1.000f;
                if (IsNull(config.sobelMult2))
                    config.sobelMult2 = 1.000f;
                if (IsNull(config.sobelMult3))
                    config.sobelMult3 = 1.000f;
                if (IsNull(config.sobelMult4))
                    config.sobelMult4 = 1.000f;



                if (IsNull(config.autoEdge))
                    config.autoEdge = true;
                if (IsNull(config.autoSobelEdge))
                    config.autoSobelEdge = true;
                if (IsNull(config.firstTime))
                    config.firstTime = true;
                if (IsNull(config.subViewOnly))
                    config.subViewOnly = false;
                if (IsNull(config.useInfoModeSpecific))
                    config.useInfoModeSpecific = false;
                if (IsNull(config.oldGamma))
                    config.oldGamma = 2.2f;
                if (IsNull(config.toneMapGamma))
                    config.toneMapGamma = 2.2f;
                if (IsNull(config.oldBoost))
                    config.oldBoost = 1.15f;
                if (IsNull(config.toneMapBoost))
                    config.toneMapBoost = 1.15f;

                if (IsNull(config.currentColor))
                    config.currentColor = new Color(0, 0, 0, 0);
                if (IsNull(config.colorMultiplier))
                    config.colorMultiplier = 1.0f;

                if (IsNull(config.mixColorMultiplier))
                    config.mixColorMultiplier = 1.0f;
                if (IsNull(config.mixCurrentColor))
                    config.mixCurrentColor = new Color(1, 1, 1, 0);

                if (IsNull(config.bloomEnabled))
                    config.bloomEnabled = false;
                if (IsNull(config.bloomThresh))
                    config.bloomThresh = 0.27f;
                if (IsNull(config.bloomIntens))
                    config.bloomIntens = 0.39f;
                if (IsNull(config.bloomBlurSize))
                    config.bloomBlurSize = 5.50f;
                if (IsNull(config.activeStockPreset))
                    config.activeStockPreset = ActiveStockPreset.LowEndMain;
                if (IsNull(config.cartoonMixColor))
                    config.cartoonMixColor = new Color(1, 1, 1, 0);
                if (IsNull(config.windowLoc))
                    config.windowLoc = new Vector2(32,32);
                
                #endregion

            }
        }

        void InitializeBanking()
        {
            presetEntries = new string[30];
            for (int i = 0; i < presetEntries.Length; i++)
            {
                presetEntries[i] = "";
            }

            if (bank == null)
            {
                bank = new PresetBank();
                bank.presetEntries = new string[30];

                for (int i = 0; i < presetEntries.Length; i++)
                {
                    bank.presetEntries[i] = "";
                }
            }
            LoadBank();
        }

        void ResetBank()
        {
            for (int i = 0; i < presetEntries.Length; i++)
            {
                presetEntries[i] = "";
            }
            SaveBank();
        }

        void LoadBank()
        {

            presetEntries = bank.presetEntries;
            for (int i = 0; i < presetEntries.Length; i++)
            {
                presetEntries[i] = bank.presetEntries[i];
            }
        }

        static bool IsNull(System.Object aObj)
        {
            return aObj == null || aObj.Equals(null);
        }

        void Start()
        {
            edge = GetComponent<EdgeDetection>();
            bloom = GetComponent<BloomOptimized>();
            tonem = GetComponent<ToneMapping>();
            defaultBoost = tonem.m_ToneMappingBoostFactor;
            defaultGamma = tonem.m_ToneMappingGamma;
            prevGamma = defaultGamma;
            prevBoost = defaultBoost;
            LoadConfig(false);
            
            if (config.keyCode == KeyCode.None)
            {
                config.keyCode = KeyCode.LeftBracket;
            }
            SaveBank();
            SaveConfig();
            

            if (firstTime)
            {
                showSettingsPanel = true;
                tab = Config.Tab.Hotkey;
            }
            else
            {
                tab = Config.Tab.EdgeDetection;
            }
            if (wantsToneMapper)
            {
                toneMapGamma = config.toneMapGamma;
                toneMapBoost = config.toneMapBoost;
            }
            
            if (config.edgeEnabled)
                ToggleBorderedSkylines(true);
            else
                ToggleBorderedSkylines(false);

            FixTonemapperIfZeroed();

        }



        void FixTonemapperIfZeroed()
        {
            if (wantsToneMapper)
            {
                if (tonem.m_ToneMappingGamma == 0)
                    tonem.m_ToneMappingGamma = defaultGamma;
                if (tonem.m_ToneMappingBoostFactor == 0)
                    tonem.m_ToneMappingBoostFactor = defaultBoost;
            }
            
        }

        void EdgeColor(float r, float g, float b)
        {
            float newR = r * colorMultiplier;
            float newG = g * colorMultiplier;
            float newB = b * colorMultiplier;
            newColor = new Color(newR, newG, newB, 0);
            edge.SetEdgeColor(newColor);
            currentColor = newColor;
        }

        void MixColor(float r, float g, float b)
        {
            float mixNewR = r * mixColorMultiplier;
            float mixNewG = g * mixColorMultiplier;
            float mixNewB = b * mixColorMultiplier;
            mixNewColor = new Color(mixNewR, mixNewG, mixNewB, 0);
            edge.SetMixColor(mixNewColor);
            mixCurrentColor = mixNewColor;
        }

        void LoadConfig(bool falseIfInitializationLoadOnly)
        {
            if (!falseIfInitializationLoadOnly)
            {
                automaticMode = config.automaticMode;
                edge.enabled = config.edgeEnabled;
                subViewOnly = config.subViewOnly;
                useInfoModeSpecific = config.useInfoModeSpecific;
                firstTime = config.firstTime;
                windowLoc = config.windowLoc;
                windowRect.x = windowLoc.x;
                windowRect.y = windowRect.y;
                wantsToneMapper = config.wantsTonemapper;
            }
            toneMapGamma = config.toneMapGamma;
            toneMapBoost = config.toneMapBoost;
            cartoonMixC = config.cartoonMixColor;
            activeStockPreset = config.activeStockPreset;
            edge.mode = config.edgeMode;
            edge.edgesOnly = config.edgeOnly;
            edge.sampleDist = config.edgeSamp;
            //TriangleRoberts
            edge.sensitivityNormals = config.sensNorm;
            edge.sensitivityDepth = config.sensDepth;
            //Sobel
            edge.edgeExp = config.edgeExpo;
            edge.depthsDiagonal = config.depthsDiagonal;
            edge.depthsAxis = config.depthsAxis;
            edge.axisVsCenter = config.axisVsCenter;
            edge.mult1 = config.sobelMult1;
            edge.mult2 = config.sobelMult2;
            edge.mult3 = config.sobelMult3;
            edge.mult4 = config.sobelMult4;
            autoEdge = config.autoEdge;
            autoSobelEdge = config.autoSobelEdge;
            currentColor = config.currentColor;
            edge.edgeColor = currentColor;
            colorMultiplier = config.colorMultiplier;
            mixCurrentColor = config.mixCurrentColor;
            edge.edgesOnlyBgColor = mixCurrentColor;
            mixColorMultiplier = config.mixColorMultiplier;
            setR = config.setR;
            setG = config.setG;
            setB = config.setB;
            mixSetR = config.mixSetR;
            mixSetG = config.mixSetG;
            mixSetB = config.mixSetB;
            bloom.enabled = config.bloomEnabled;
            bloom.threshold = config.bloomThresh;
            bloom.intensity = config.bloomIntens;
            bloom.blurSize = config.bloomBlurSize;
            

            if (automaticMode && !firstTime)
            {
                DetermineMode();
            }
        }


        void DetermineMode()
        {
            switch (activeStockPreset)
            {
                case ActiveStockPreset.HighEndPC:
                    UltraAutomatic();
                    break;
                case ActiveStockPreset.ClassicTriangle:
                    ClassicTriangleAutomatic();
                    break;
                case ActiveStockPreset.CartoonAlt:
                    CartoonAltAutomatic();
                    break;
                case ActiveStockPreset.Cartoon:
                    CartoonAutomatic();
                    break;
                case ActiveStockPreset.CartoonThree:
                    CartoonThreeAutomatic();
                    break;
                case ActiveStockPreset.Bordercities:
                    BordercitiesAutomatic();
                    break;
                case ActiveStockPreset.BordercitiesEasier:
                    BordercitiesEasyViewingAutomatic();
                    break;
                case ActiveStockPreset.BordercitiesGritty:
                    BordercitiesGrittyAutomatic();
                    break;
                case ActiveStockPreset.BordercitiesBright:
                    BordercitiesGrittyAutomatic();
                    break;
                case ActiveStockPreset.Sobelcities:
                    SobelcitiesAutomatic();
                    break;
                case ActiveStockPreset.SobelcitiesOD:
                    SobelcitiesODAutomatic();
                    break;
                case ActiveStockPreset.SobelcitiesOD720:
                    SobelcitiesOD720Automatic();
                    break;
                case ActiveStockPreset.LowEndMain:
                    LowEndAutomatic();
                    break;
                case ActiveStockPreset.LowEndAlt:
                    LowEndAltAutomatic();
                    break;
                case ActiveStockPreset.ClassicSobel:
                    ClassicSobelAutomatic();
                    break;
                case ActiveStockPreset.Realism:
                    RealismAutomatic();
                    break;
                case ActiveStockPreset.Random:
                    RandomAutomatic();
                    break;
                default:
                    LowEndAutomatic();
                    break;
            }
        }
        public void SaveConfig()
        {
            config.subViewOnly = subViewOnly;
            config.useInfoModeSpecific = useInfoModeSpecific;

            config.wantsTonemapper = wantsToneMapper;
            config.automaticMode = automaticMode;
            config.edgeEnabled = edge.enabled;
            config.edgeMode = edge.mode;
            config.edgeOnly = edge.edgesOnly;
            config.edgeSamp = edge.sampleDist;
            //TriangleRobert
            config.autoEdge = autoEdge;
            config.sensNorm = edge.sensitivityNormals;
            config.sensDepth = edge.sensitivityDepth;
            //Sobel
            config.autoSobelEdge = autoSobelEdge;
            config.edgeExpo = edge.edgeExp;
            config.depthsDiagonal = edge.depthsDiagonal;
            config.depthsAxis = edge.depthsAxis;
            config.axisVsCenter = edge.axisVsCenter;
            config.sobelMult1 = edge.mult1;
            config.sobelMult2 = edge.mult2;
            config.sobelMult3 = edge.mult3;
            config.sobelMult4 = edge.mult4;

            config.subViewOnly = subViewOnly;
            config.useInfoModeSpecific = useInfoModeSpecific;

            config.currentColor = currentColor;
            config.setR = setR;
            config.setG = setG;
            config.setB = setB;
            config.colorMultiplier = colorMultiplier;

            config.mixCurrentColor = mixCurrentColor;
            config.mixSetR = mixSetR;
            config.mixSetG = mixSetG;
            config.mixSetB = mixSetB;
            config.mixColorMultiplier = mixColorMultiplier;

            config.bloomEnabled = bloom.enabled;
            config.bloomThresh = bloom.threshold;
            config.bloomIntens = bloom.intensity;
            config.bloomBlurSize = bloom.blurSize;
            config.firstTime = firstTime;

            config.toneMapBoost = toneMapBoost;
            config.toneMapGamma = toneMapGamma;

            config.activeStockPreset = activeStockPreset;
            config.cartoonMixColor = cartoonMixC;

            config.windowLoc = windowLoc;

            


            Config.Serialize(configPath, config);
        }

        public void SaveBank()
        {
            bank.presetEntries = presetEntries;
            for (int i = 0; i < presetEntries.Length; i++ )
            {
                bank.presetEntries[i] = presetEntries[i];
            }
            PresetBank.Serialize(bankPath, bank);
        }

        

        

        void LowEndAutomatic()
        {
            displayTitle = "Sobel Skies (Default)";
            activeStockPreset = ActiveStockPreset.LowEndMain;
            automaticMode = true;
            autoSobelEdge = true;
            edge.depthsDiagonal = 1.0f;
            edge.mult1 = 1.753f;
            edge.mult2 = 3.679f;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.sampleDist = 1f;
            edge.edgesOnly = 0;
            edge.edgeExp = 0.43f;
            edge.edgeColor = realismEdgeC;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "Suited for users of low-end hardware who cannot afford the performance hit of supersampling via nlight's Dynamic Resolution.  This is a balanced default look.  You can achieve more foreground subtlety (at the cost of distant line enlargement) in 'Auto-Sobel', or achieve more overall effect via 'Auto-Triangle'.  This mode was fine-tuned in 720p with all settings as low as possible.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            
        }

        

        void ClassicSobelAutomatic()
        {
            displayTitle = "Classic Auto-Sobel";
            automaticMode = true;
            activeStockPreset = ActiveStockPreset.ClassicSobel;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepth;
            edge.sampleDist = 1.0f;
            edge.edgeExp = 0.44f;
            edge.edgesOnly = 0;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "'Classic Auto-Sobel' was the original 'Sobel' edge detection mode in Bordered Skylines.  'Sobel Skylines' was later implemented as a C:S-specific adaptation of the original 'Auto-Sobel' algorithm.  'Classic Auto-Sobel' has been retained should you prefer it.";
        }


        void ClassicTriangleAutomatic()
        {
            displayTitle = "Classic Auto-Triangle";
            automaticMode = true;
            activeStockPreset = ActiveStockPreset.ClassicTriangle;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.sensitivityNormals = 0.63f;
            edge.sensitivityDepth = 2.12f;
            edge.edgeExp = 0.09f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "Like 'Classic Auto-Sobel,' this mode was one of the originals in Bordered Skylines.  In comparison to its brother 'Auto-Sobel', 'Auto-Triangle' imparts a TREMENDOUS effect... for better or for worse.  Going on a stroll in first person camera?  Downright breathtaking.  Zoomed out and viewing your entire city?  ..not so much.";
        }
        void LowEndAltAutomatic()
        {
            displayTitle = "Eyefriendly(er)";
            activeStockPreset = ActiveStockPreset.LowEndAlt;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.sampleDist = 1f;
            autoEdge = true;
            edge.edgeExp = 0.5f;
            edge.edgesOnly = 0f;
            edge.edgeColor = lowEndEdgeC;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "This preset attempts to get around the harshness of the above 'Auto-Triangle' edge detection mode's edges when viewing at low resolutions by adding a slight gray tint to detected edges.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void BordercitiesAutomatic()
        {
            displayTitle = "Bordercities";
            activeStockPreset = ActiveStockPreset.Bordercities;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "'Bordercities', providing you're viewing at 175% DR and 1080p or more, is my personal favorite for strolling cities up close.  If you are looking for a 24/7 gameplay preset ('fully-zoomed-out'-compatible, yet still maintaining the strength of effect,) try 'SC++'.  If you wish for a softer look than that, try 'Sobelcities.'  If you'd rather use a lighter version of -this- particular edge style, try 'BC|EasierViewing.'";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void BordercitiesEasyViewingAutomatic()
        {
            displayTitle = "Bordercities: Easier Viewing";
            activeStockPreset = ActiveStockPreset.BordercitiesEasier;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "A spin-off of 'Bordercities'.  Sacrifices a little bit of the edge grit in favor of eye-friendly visuals when zoomed out.  I want to look into a way of automatically switching upon a certain threshold of being zoomed out (provided that the transition would look smooth.)";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }

        }

        void BordercitiesGrittyAutomatic()
        {
            displayTitle = "Bordercities: Gritty";
            activeStockPreset = ActiveStockPreset.BordercitiesGritty;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "Bordercities, as above, though with additional Tonemapper settings to achieve a grittier look.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 1.123f;
                    toneMapBoost = 3.061f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void BordercitiesBrightAutomatic()
        {
            displayTitle = "Bordercities: Bright";
            activeStockPreset = ActiveStockPreset.BordercitiesBright;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.0f;
            edge.edgesOnly = 0f;
            autoEdge = true;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "Bordercities, as above, though with additional Tonemapper settings to achieve a brighter & more pleasant look.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 1.881f;
                    toneMapBoost = 2.849f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void SobelcitiesAutomatic()
        {
            displayTitle = "Sobelcities";
            activeStockPreset = ActiveStockPreset.Sobelcities;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 1.77f;
            edge.edgesOnly = 0f;
            autoSobelEdge = true;
            edge.depthsDiagonal = 0.779f;
            edge.mult1 = 1.753f;
            edge.mult2 = 3.679f;

            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "Sobelcities is the 1080p/175%-DynamicResolution-tuned version of the default 'Sobel Skies' mode.  Designed for those wishing to have a Borderlands-esque feel without the excess visual noise of the Triangle-based presets (IE Bordercities, Auto-Triangle.)";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 2.2f;
                    toneMapBoost = 1.365f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
        }

        void SobelcitiesODAutomatic()
        {
            displayTitle = "Sobelcities++";
            activeStockPreset = ActiveStockPreset.SobelcitiesOD;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 4f;
            edge.edgesOnly = 0;
            autoSobelEdge = true;
            edge.depthsDiagonal = 0.779f;
            edge.mult1 = 1.702f;
            edge.mult2 = 6.742f;

            edge.edgeColor = sobelcitiesODc;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "A spinoff of 'Sobelcities,' 'Sobelcities++' creates a stronger effect than its less intense brother.. however, at a cost.  When zoomed out, the sides of certain tall buildings will be improperly detected as edges.  To compensate for this, Sobelcities:Overdrive gives the edge coloring a slight gray tint, so that these improper 'edges' can be perceived as shadows rather than as a glitchy, 'overdriven' effect, hence this preset's name.  Use this preset if you don't mind the 'shadowing' upon tall buildings when fully zoomed out.  COMING SOON: Improvements to the 'Sobel Skylines' 'auto-zoom-compensation' algorithm to achieve the best of both worlds.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 2.2f;
                    toneMapBoost = 1.365f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
        }

        void SobelcitiesOD720Automatic()
        {
            displayTitle = "Sobel Skies ++";
            activeStockPreset = ActiveStockPreset.SobelcitiesOD720;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.edgeExp = 0.5f;
            edge.sampleDist = 2f;
            edge.edgesOnly = 0;
            autoSobelEdge = true;
            edge.depthsDiagonal = 0.779f;
            edge.mult1 = 1.702f;
            edge.mult2 = 6.742f;

            edge.edgeColor = sobelcitiesODc;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            displayText = "A spinoff of 'Sobel Skies,' 'Sobel Skies ++' creates a stronger effect than its less intense brother.. however, at a cost.  When zoomed out, the sides of certain tall buildings will be improperly detected as edges.  To compensate for this, Sobelcities:Overdrive gives the edge coloring a slight gray tint, so that these improper 'edges' can be perceived as shadows rather than as a glitchy, 'overdriven' effect, hence this preset's name.  Use this preset if you don't mind the 'shadowing' upon tall buildings when fully zoomed out.  COMING SOON: Improvements to the 'Sobel Skylines' 'auto-zoom-compensation' algorithm to achieve the best of both worlds.";
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 2.2f;
                    toneMapBoost = 1.365f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
        }

        void RealismAutomatic()
        {
            displayTitle = "Realism";
            activeStockPreset = ActiveStockPreset.Realism;
            automaticMode = true;
            autoSobelEdge = true;
            edge.mult1 = 0.162f;
            edge.mult2 = 1.683f;
            edge.mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
            edge.sampleDist = 1;
            edge.edgesOnly = 0;
            edge.edgeExp = 0.5f;
            edge.edgeColor = realismEdgeC;
            edge.edgesOnlyBgColor = Color.white;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            bloom.threshold = 0.27f;
            bloom.intensity = 0.39f;
            bloom.blurSize = 5.50f;
            displayText = "For users who aim to use Bordered Skylines as a visual enhancement tool rather than as an obvious visual effect.  Using Bordered Skylines in this manner helps to maintain the visual detection of distant shapes which would otherwise be lost by the renderer, such as distant street lamps. Do keep in mind that Bordered Skylines is not an ambient occlusion mod.  If you are looking for -shadowing- at -edge intersections-, try Ulysius' Ambient Occlusion.  HIGHLY RECOMMENDED to use DLAA AA from MazK's 'PostProcessFX' with this preset!  DLAA provides the 'knock-out punch' to complete the look, cancelling out the negative side effects of this 'Realism' preset by smoothing out the results of edge detection --after-- (that's the key) the fact!";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void CartoonAutomatic()
        {
            displayTitle = "Retro";
            activeStockPreset = ActiveStockPreset.Cartoon;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.sampleDist = 1.09f;
            edge.edgesOnly = 0.28f;
            autoEdge = false;
            edge.sensitivityDepth = 0;
            edge.sensitivityNormals = 1.68f;
            mixColorMultiplier = 1.0f;
            edge.edgeColor = cartoonEdgeC;
            edge.edgesOnlyBgColor = cartoonMixC;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            displayText = "Add a very 50's cartoon look to your game.  Press the below button to generate a color theme (make sure to save it afterwards!)  Coming soon, a special 'Cartoonish-based' automatic-zoom-compensation algorithm, removing any unpleasant black surfaces when zoomed in close enough.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 3.064f;
                    toneMapBoost = 0.537f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void CartoonAltAutomatic()
        {
            displayTitle = "Colorful";
            activeStockPreset = ActiveStockPreset.CartoonAlt;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            edge.sampleDist = 1.09f;
            edge.edgesOnly = 0.067f;
            autoEdge = false;
            edge.sensitivityDepth = 0;
            edge.sensitivityNormals = 1.38f;
            mixColorMultiplier = 1.0f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = cartoonMixC;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            displayText = "A little bit more modern than 'Retro', yet still old-school looking.  Coming soon, a special 'Cartoonish-based' automatic-zoom-compensation algorithm, removing any unpleasant black surfaces when zoomed in close enough.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 5.376f;
                    toneMapBoost = 0.128f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }
        void CartoonThreeAutomatic()
        {
            displayTitle = "Cartoon";
            activeStockPreset = ActiveStockPreset.CartoonThree;
            automaticMode = true;
            edge.mode = EdgeDetection.EdgeDetectMode.TriangleDepthNormals;
            edge.sampleDist = 1;
            edge.edgesOnly = 0f;
            autoEdge = false;
            edge.sensitivityDepth = 0;
            edge.sensitivityNormals = 1.68f;
            mixColorMultiplier = 1.0f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = cartoonMixC;
            if (!CheckTonemapper())
                ResetTonemapper();
            bloom.enabled = false;
            displayText = "No explanation required!  Coming soon, a special 'Cartoonish-based' automatic-zoom-compensation algorithm, removing any unpleasant black surfaces when zoomed in close enough.";
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapGamma = 3.978f;
                    toneMapBoost = 0.376f;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
        }

        void RandomAutomatic()
        {
            displayTitle = "Random";
            activeStockPreset = ActiveStockPreset.Random;
            automaticMode = true;
            edge.mode = (EdgeDetection.EdgeDetectMode)Random.Range(0, 3);
            edge.sampleDist = Random.Range(0.00f,5.00f);
            edge.edgesOnlyBgColor = new Color(Random.Range(0.0f,5.0f),Random.Range(0.0f,5.0f), Random.Range(0.0f,5.0f));
            mixColorMultiplier = 1.0f;
            edge.edgeColor = new Color(Random.Range(0.0f,5.0f),Random.Range(0.0f,5.0f), Random.Range(0.0f,5.0f));
            edge.edgesOnly = Random.Range(0.00f, 1.00f);
            autoEdge = true;
            autoSobelEdge = true;
            edge.edgeExp = 0.5f;
            displayText = "This preset was generated randomly.  Enter Advanced Mode below to tweak further.   Note that you will lose this effect if you fail to save it before returning to 'Plug & Play' mode.";
            bloom.enabled = false;
            
        }

        void UltraAutomatic()
        {
            automaticMode = true;
            displayTitle = "Ultra - REQUIRES 1920x1080 + DR 250-300% + 'AMBIENT OCC' + 'SUN SHAFTS' + 'POSTPROCESSFX' WITH BLOOM, MOTION BLUR, AND DLAA ANTI-ALIASING FOR INTENDED LOOK.";
            displayText = "This is BEAUTIFUL.  You may need a super-computer for this.";
            edge.sampleDist = 2.0f;
            activeStockPreset = ActiveStockPreset.HighEndPC;
            edge.mode = EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
            autoEdge = true;
            edge.edgesOnly = 0f;
            edge.edgeColor = Color.black;
            edge.edgesOnlyBgColor = Color.white;
            MatchColorsOnGUI();
            bloom.enabled = false;
            if (wantsToneMapper)
            {
                if (!subViewOnly)
                {
                    toneMapBoost = defaultBoost;
                    toneMapGamma = defaultGamma;
                }
                else
                {
                    toneMapGamma = defaultGamma;
                    toneMapBoost = defaultBoost;
                }
            }
            
            


        }


        

        void MatchColorsOnGUI()
        {
            mixSetR = edge.edgesOnlyBgColor.r;
            mixSetG = edge.edgesOnlyBgColor.g;
            mixSetB = edge.edgesOnlyBgColor.b;
            setR = edge.edgeColor.r;
            setG = edge.edgeColor.g;
            setB = edge.edgeColor.b;
            mixColorMultiplier = 1.0f;
            colorMultiplier = 1.0f; 
        }
        string KeyToString(KeyCode kc)
        {
            switch (kc)
            {
                case KeyCode.F5:
                    return "F5";
                case KeyCode.F6:
                    return "F6";
                case KeyCode.F7:
                    return "F7";
                case KeyCode.F8:
                    return "F8";
                case KeyCode.F9:
                    return "F9";
                case KeyCode.F10:
                    return "F10";
                case KeyCode.F11:
                    return "F11";
                case KeyCode.F12:
                    return "F12";
                case KeyCode.LeftBracket:
                    return "LeftBracket";
                case KeyCode.RightBracket:
                    return "RightBracket";
                case KeyCode.Equals:
                    return "=";
                case KeyCode.Slash:
                    return "Slash";
                case KeyCode.Backslash:
                    return "Backslash";
                case KeyCode.Home:
                    return "Home";
                case KeyCode.End:
                    return "End";
                case KeyCode.KeypadDivide:
                    return "Numpad /";
                case KeyCode.KeypadMultiply:
                    return "Numpad *";
                case KeyCode.KeypadMinus:
                    return "Numpad -";
                case KeyCode.KeypadPlus:
                    return "Numpad +";
                case KeyCode.KeypadEquals:
                    return "Numpad =";
                default:
                    return kc.ToString();
            }

        }

        void SavePreset(string name, bool falseIfInfoMode)
        {
            Preset.MakeFolderIfNonexistent();
            Preset presetToSave;
            if (falseIfInfoMode)
                presetToSave = Preset.Deserialize(name);
            else
                presetToSave = Preset.DeserializeInfoMode(name);
            if (presetToSave == null) // If no presets file, create one.
            {
                presetToSave = new Preset();
            }
            presetToSave.edgeMode = edge.mode;
            presetToSave.sensNorm = edge.sensitivityNormals;
            presetToSave.sensDepth = edge.sensitivityDepth;
            presetToSave.edgeOnly = edge.edgesOnly;
            presetToSave.edgeSamp = edge.sampleDist;
            presetToSave.autoEdge = autoEdge;
            presetToSave.autoSobelEdge = autoSobelEdge;
            presetToSave.subViewOnly = subViewOnly;

            presetToSave.edgeExpo = edge.edgeExp;
            presetToSave.depthsAxis = edge.depthsAxis;
            presetToSave.depthsDiagonal = edge.depthsDiagonal;
            presetToSave.depthsAxis = edge.axisVsCenter;
            presetToSave.sobelMult1 = edge.mult1;
            presetToSave.sobelMult2 = edge.mult2;
            presetToSave.sobelMult3 = edge.mult3;
            presetToSave.sobelMult4 = edge.mult4;

            presetToSave.currentColor = currentColor;
            presetToSave.setR = setR;
            presetToSave.setG = setG;
            presetToSave.setB = setB;
            presetToSave.colorMultiplier = colorMultiplier;

            presetToSave.mixCurrentColor = mixCurrentColor;
            presetToSave.mixSetR = mixSetR;
            presetToSave.mixSetG = mixSetG;
            presetToSave.mixSetB = mixSetB;
            presetToSave.mixColorMultiplier = mixColorMultiplier;

            presetToSave.bloomEnabled = bloom.enabled;
            presetToSave.bloomThresh = bloom.threshold;
            presetToSave.bloomIntens = bloom.intensity;
            presetToSave.bloomBlurSize = bloom.blurSize;

            presetToSave.toneMapBoost = tonem.m_ToneMappingBoostFactor;
            presetToSave.toneMapGamma = tonem.m_ToneMappingGamma;

            if (falseIfInfoMode)
                Preset.Serialize(name, presetToSave);
            else
                Preset.SerializeInfoMode(name, presetToSave);
        }

        

        bool LoadPreset(string name, bool falseIfInfoMode)
        {
            Preset presetToLoad;
            if (falseIfInfoMode)
                presetToLoad = Preset.Deserialize(name);
            else
                presetToLoad = Preset.DeserializeInfoMode(name);
            if (presetToLoad == null)
            {
                return false;
            }
            else
            {
                edge.mode = presetToLoad.edgeMode;
                edge.sensitivityNormals = presetToLoad.sensNorm;
                edge.sensitivityDepth = presetToLoad.sensDepth;
                edge.edgeExp = presetToLoad.edgeExpo;
                edge.depthsAxis = presetToLoad.depthsAxis;
                edge.depthsDiagonal = presetToLoad.depthsDiagonal;
                edge.axisVsCenter = presetToLoad.axisVsCenter;
                edge.mult1 = presetToLoad.sobelMult1;
                edge.mult2 = presetToLoad.sobelMult2;
                edge.mult3 = presetToLoad.sobelMult3;
                edge.mult4 = presetToLoad.sobelMult4;
                edge.edgesOnly = presetToLoad.edgeOnly;
                edge.sampleDist = presetToLoad.edgeSamp;
                autoEdge = presetToLoad.autoEdge;
                autoSobelEdge = presetToLoad.autoSobelEdge;
                currentColor = presetToLoad.currentColor;
                edge.edgeColor = currentColor;
                colorMultiplier = presetToLoad.colorMultiplier;
                mixCurrentColor = presetToLoad.mixCurrentColor;
                edge.edgesOnlyBgColor = mixCurrentColor;
                mixColorMultiplier = presetToLoad.mixColorMultiplier;
                setR = presetToLoad.setR;
                setG = presetToLoad.setG;
                setB = presetToLoad.setB;
                mixSetR = presetToLoad.mixSetR;
                mixSetG = presetToLoad.mixSetG;
                mixSetB = presetToLoad.mixSetB;
                if (wantsToneMapper)
                {
                    tonem.m_ToneMappingBoostFactor = presetToLoad.toneMapBoost;
                    tonem.m_ToneMappingGamma = presetToLoad.toneMapGamma;
                }
                bloom.enabled = presetToLoad.bloomEnabled;
                bloom.threshold = presetToLoad.bloomThresh;
                bloom.intensity = presetToLoad.bloomIntens;
                bloom.blurSize = presetToLoad.bloomBlurSize;
                return true;
            }
        }

        bool IsValidFilename(string testName)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(testName))
                return false;
            return true;
        }

        void ResizeDefaults()
        {
            windowRect.width = defaultWidth;
            windowRect.height = defaultHeight;
        }

        void QuicksaveActiveViewMode()
        {
            string saveName = GetCurrentInfoModeString();
            SavePreset(saveName, false);
            SavePreset(saveName, false);
        }

        string GetCurrentInfoModeString()
        {
            switch (infoManager.CurrentMode)
            {
                case InfoManager.InfoMode.BuildingLevel:
                    return "BuildingLevel";
                case InfoManager.InfoMode.Connections:
                    return "Connections";
                case InfoManager.InfoMode.CrimeRate:
                    return "CrimeRate";
                case InfoManager.InfoMode.Density:
                    return "Density";
                case InfoManager.InfoMode.Districts:
                    return "Districts";
                case InfoManager.InfoMode.Education:
                    return "Education";
                case InfoManager.InfoMode.Electricity:
                    return "Electricity";
                case InfoManager.InfoMode.Entertainment:
                    return "Entertainment";
                case InfoManager.InfoMode.FireSafety:
                    return "FireSafety";
                case InfoManager.InfoMode.Garbage:
                    return "Garbage";
                case InfoManager.InfoMode.Happiness:
                    return "Happiness";
                case InfoManager.InfoMode.Health:
                    return "Health";
                case InfoManager.InfoMode.LandValue:
                    return "LandValue";
                case InfoManager.InfoMode.NaturalResources:
                    return "NaturalResources";
                case InfoManager.InfoMode.NoisePollution:
                    return "NoisePollution";
                case InfoManager.InfoMode.None:
                    return "None";
                case InfoManager.InfoMode.Pollution:
                    return "Pollution";
                case InfoManager.InfoMode.TerrainHeight:
                    return "TerrainHeight";
                case InfoManager.InfoMode.Traffic:
                    return "Traffic";
                case InfoManager.InfoMode.Transport:
                    return "Transport";
                case InfoManager.InfoMode.Water:
                    return "Water";
                case InfoManager.InfoMode.Wind:
                    return "Wind";
                default:
                    return "Default";
            }
        }

        string GetCurrentInfoModeLabelString()
        {
            switch (infoManager.CurrentMode)
            {
                case InfoManager.InfoMode.BuildingLevel:
                    return "Building Level";
                case InfoManager.InfoMode.Connections:
                    return "Connections";
                case InfoManager.InfoMode.CrimeRate:
                    return "Crime Rate";
                case InfoManager.InfoMode.Density:
                    return "Density";
                case InfoManager.InfoMode.Districts:
                    return "Districts";
                case InfoManager.InfoMode.Education:
                    return "Education";
                case InfoManager.InfoMode.Electricity:
                    return "Electricity";
                case InfoManager.InfoMode.Entertainment:
                    return "Entertainment";
                case InfoManager.InfoMode.FireSafety:
                    return "Fire Safety";
                case InfoManager.InfoMode.Garbage:
                    return "Garbage";
                case InfoManager.InfoMode.Happiness:
                    return "Happiness";
                case InfoManager.InfoMode.Health:
                    return "Health";
                case InfoManager.InfoMode.LandValue:
                    return "Land Value";
                case InfoManager.InfoMode.NaturalResources:
                    return "Natural Resources";
                case InfoManager.InfoMode.NoisePollution:
                    return "Noise Pollution";
                case InfoManager.InfoMode.None:
                    return "None";
                case InfoManager.InfoMode.Pollution:
                    return "Pollution";
                case InfoManager.InfoMode.TerrainHeight:
                    return "Terrain Height";
                case InfoManager.InfoMode.Traffic:
                    return "Traffic";
                case InfoManager.InfoMode.Transport:
                    return "Transport";
                case InfoManager.InfoMode.Water:
                    return "Water";
                case InfoManager.InfoMode.Wind:
                    return "Wind";
                default:
                    return "Default";
            }
        }

        private bool existBuildingLevel = false;
        private bool existConnections = false;
        private bool existCrimeRate = false;
        private bool existDensity = false;
        private bool existDistricts = false;
        private bool existEducation = false;
        private bool existElectricity = false;
        private bool existEntertainment = false;
        private bool existFireSafety = false;
        private bool existGarbage = false;
        private bool existHappiness = false;
        private bool existHealth = false;
        private bool existLandValue = false;
        private bool existNaturalResources = false;
        private bool existNoisePollution = false;
        private bool existPollution = false;
        private bool existTerrainHeight = false;
        private bool existTraffic = false;
        private bool existTransport = false;
        private bool existWater = false;
        private bool existWind = false;
        void InitializeExistBools()
        {
            existBuildingLevel = Preset.CheckIfExists("BuildingLevel");
            existConnections = Preset.CheckIfExists("Connections");
            existCrimeRate = Preset.CheckIfExists("CrimeRate");
            existDensity = Preset.CheckIfExists("Density");
            existDistricts = Preset.CheckIfExists("Districts");
            existEducation = Preset.CheckIfExists("Education");
            existElectricity = Preset.CheckIfExists("Electricity");
            existEntertainment = Preset.CheckIfExists("Entertainment");
            existFireSafety = Preset.CheckIfExists("FireSafety");
            existGarbage = Preset.CheckIfExists("Garbage");
            existHappiness = Preset.CheckIfExists("Happiness");
            existHealth = Preset.CheckIfExists("Health");
            existLandValue = Preset.CheckIfExists("LandValue");
            existNaturalResources = Preset.CheckIfExists("NaturalResources");
            existNoisePollution = Preset.CheckIfExists("NoisePollution");
            existPollution = Preset.CheckIfExists("Pollution");
            existTerrainHeight = Preset.CheckIfExists("TerrainHeight");
            existTraffic = Preset.CheckIfExists("Traffic");
            existTransport = Preset.CheckIfExists("Transport");
            existWater = Preset.CheckIfExists("Water");
            existWind = Preset.CheckIfExists("Wind");
        }
        void InfoModes()
        {

            if (infoManager.CurrentMode != currentInfoMode)
            {
                if (infoManager.CurrentMode == InfoManager.InfoMode.None)
                {
                    currentInfoMode = infoManager.CurrentMode;
                    LoadConfig(true);
                    useInfoModeSpecific = true;
                    return;
                }
                ViewModeCheckAndSet(InfoManager.InfoMode.BuildingLevel, "BuildingLevel");
                ViewModeCheckAndSet(InfoManager.InfoMode.Connections, "Connections");
                ViewModeCheckAndSet(InfoManager.InfoMode.CrimeRate, "CrimeRate");
                ViewModeCheckAndSet(InfoManager.InfoMode.Density, "Density");
                ViewModeCheckAndSet(InfoManager.InfoMode.Districts, "Districts");
                ViewModeCheckAndSet(InfoManager.InfoMode.Education, "Education");
                ViewModeCheckAndSet(InfoManager.InfoMode.Electricity, "Electricity");
                ViewModeCheckAndSet(InfoManager.InfoMode.Entertainment, "Entertainment");
                ViewModeCheckAndSet(InfoManager.InfoMode.FireSafety, "FireSafety");
                ViewModeCheckAndSet(InfoManager.InfoMode.Garbage, "Garbage");
                ViewModeCheckAndSet(InfoManager.InfoMode.Happiness, "Happiness");
                ViewModeCheckAndSet(InfoManager.InfoMode.Health, "Health");
                ViewModeCheckAndSet(InfoManager.InfoMode.LandValue, "LandValue");
                ViewModeCheckAndSet(InfoManager.InfoMode.NaturalResources, "NaturalResources");
                ViewModeCheckAndSet(InfoManager.InfoMode.NoisePollution, "NoisePollution");
                ViewModeCheckAndSet(InfoManager.InfoMode.Pollution, "Pollution");
                ViewModeCheckAndSet(InfoManager.InfoMode.TerrainHeight, "TerrainHeight");
                ViewModeCheckAndSet(InfoManager.InfoMode.Traffic, "Traffic");
                ViewModeCheckAndSet(InfoManager.InfoMode.Transport, "Transport");
                ViewModeCheckAndSet(InfoManager.InfoMode.Water, "Water");
                ViewModeCheckAndSet(InfoManager.InfoMode.Wind, "Wind");
                GUILayout.Space(10);
                if (GUILayout.Button("Load Main Panel Configuration"))
                {
                    LoadConfig(true);
                }
                useInfoModeSpecific = true;
                GUILayout.Space(10); 
            }
        }

        void ResetTonemapper()
        {
            toneMapBoost = defaultBoost;
            toneMapGamma = defaultGamma;
            if (!wantsToneMapper)
            {
                tonem.m_ToneMappingGamma = defaultGamma;
                tonem.m_ToneMappingBoostFactor = defaultBoost;
            }
        }
        void SetTonemapper(float gam, float boost)
        {
            if (wantsToneMapper)
            {
                tonem.m_ToneMappingBoostFactor = boost;
                tonem.m_ToneMappingGamma = gam;
            }
        }


       

        void KeyboardGrid(int purpose)
        {
            GUILayout.BeginHorizontal();
            Hotkey("F5", KeyCode.F5, purpose);
            Hotkey("F6", KeyCode.F6, purpose);
            Hotkey("F7", KeyCode.F7, purpose);
            Hotkey("F8", KeyCode.F8, purpose);
            Hotkey("F9", KeyCode.F9, purpose);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Hotkey("F10", KeyCode.F10, purpose);
            Hotkey("F11", KeyCode.F11, purpose);
            Hotkey("F12", KeyCode.F12, purpose);
            Hotkey("[", KeyCode.LeftBracket, purpose);
            Hotkey("]", KeyCode.RightBracket, purpose);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Hotkey("=", KeyCode.Equals, purpose);
            Hotkey("Slash", KeyCode.Slash, purpose);
            Hotkey("Backslash", KeyCode.Backslash, purpose);
            Hotkey("Home", KeyCode.Home, purpose);
            Hotkey("End", KeyCode.End, purpose);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Hotkey("Numpad *", KeyCode.KeypadMultiply, purpose);
            Hotkey("Numpad -", KeyCode.KeypadMinus, purpose);
            Hotkey("Numpad =", KeyCode.KeypadEquals, purpose);
            Hotkey("Numpad +", KeyCode.KeypadPlus, purpose);
            Hotkey("Numpad /", KeyCode.KeypadDivide, purpose);
            GUILayout.EndHorizontal();

        }
        void Hotkey(string label, KeyCode keycode, int purpose)
        {
            if (GUILayout.Button(label, GUILayout.MaxWidth(100)))
            {
                switch (purpose)
                {
                    case 0:
                        config.keyCode = keycode;
                        keystring = label;
                        break;
                    case 1:
                        config.edgeToggleKeyCode = keycode;
                        edgeKeyString = label;
                        break;
                    default:
                        break;
                }
                hasClicked = true;
                SaveConfig();
            }
        }

        void ResizeWindow(int width, int height)
        {
            if (windowRect.height != height)
                windowRect.height = height;
            if (windowRect.width != width)
                windowRect.width = width;
           
        }

        bool CheckTonemapper()
        {
            if (tonem.m_ToneMappingGamma != defaultGamma || tonem.m_ToneMappingBoostFactor != defaultBoost)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        void SetActiveLookToTemporary()
        {
            userIsPreviewing = true;
            tempAutoEdge = autoEdge;
            tempAutoSobelEdge = autoSobelEdge;
            tempEdgeBgrdColor = mixCurrentColor;
            tempEdgeColor = currentColor;
            tempEdgeExpo = edge.edgeExp;
            tempEdgeMode = edge.mode;
            tempEdgeOnly = edge.edgesOnly;
            tempEdgeSamp = edge.sampleDist;
            tempSensDepth = edge.sensitivityDepth;
            tempSensNorm = edge.sensitivityNormals;
            tempToneMapBoost = toneMapBoost;
            tempToneMapGamma = toneMapGamma;
        }

        void RestoreFromTemporary()
        {
            if (userIsPreviewing)
            {
                autoEdge = tempAutoEdge;
                autoSobelEdge = tempAutoSobelEdge;
                mixCurrentColor = tempEdgeBgrdColor;
                currentColor = tempEdgeColor;
                edge.edgeExp = tempEdgeExpo;
                edge.mode = tempEdgeMode;
                edge.edgesOnly = tempEdgeOnly;
                edge.sampleDist = tempEdgeSamp;
                edge.sensitivityDepth = tempSensDepth;
                edge.sensitivityNormals = tempSensNorm;
                toneMapBoost = tempToneMapBoost;
                toneMapGamma = tempToneMapGamma;
            }
            userIsPreviewing = false;
        }


        void ViewModeGUI(string label, string filename, bool exists)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
            {
                SavePreset(filename, false);
                SavePreset(filename, false);
                InitializeExistBools();
            }
            if (exists)
            {
                if (GUILayout.Button("Load " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    LoadPreset(filename, false);
                }
            }
            else
            {
                if (GUILayout.Button("NONE", GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {

                }
            }
            GUILayout.EndHorizontal();
        }

            void ViewModeGUI(string label, string filename, bool exists, string label2, string filename2, bool exists2)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
            {
                SavePreset(filename, false);
                SavePreset(filename, false);
                InitializeExistBools();
            }
            if (exists)
            {
                if (GUILayout.Button("Load " + label, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    LoadPreset(filename, false);
                }
            }
            else
            {
                if (GUILayout.Button("NONE", GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    
                }
            }

            GUILayout.Space(30f);


            if (GUILayout.Button("Set " + label2, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
            {
                SavePreset(filename2, false);
                SavePreset(filename2, false);
                InitializeExistBools();
            }
            if (exists2)
            {
                if (GUILayout.Button("Load " + label2, GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {
                    LoadPreset(filename2, false);
                }
            }
            else
            {
                if (GUILayout.Button("NONE", GUILayout.MaxWidth(180), GUILayout.MinWidth(180)))
                {

                }
            }

            GUILayout.EndHorizontal();
        }
        

        bool ShouldWeExitPreview(Preset infoModePreset)
        {
            if (config.toneMapBoost != infoModePreset.toneMapBoost)
                return false;
            if (config.toneMapGamma != infoModePreset.toneMapGamma)
                return false;
            if (config.autoEdge != infoModePreset.autoEdge)
                return false;
            if (config.autoSobelEdge != infoModePreset.autoSobelEdge)
                return false;
            if (config.edgeExpo != infoModePreset.edgeExpo)
                return false;
            if (config.edgeMode != infoModePreset.edgeMode)
                return false;
            if (config.edgeOnly != infoModePreset.edgeOnly)
                return false;
            if (config.edgeSamp != infoModePreset.edgeSamp)
                return false;
            if (config.sensDepth != infoModePreset.sensDepth)
                return false;
            if (config.sensNorm != infoModePreset.sensNorm)
                return false;
            return true;
        }

        public float tempToneMapBoost;
        public float tempToneMapGamma;
        public bool tempAutoEdge;
        public bool tempAutoSobelEdge;
        public float tempEdgeExpo;
        public EdgeDetection.EdgeDetectMode tempEdgeMode;
        public float tempEdgeOnly;
        public float tempEdgeSamp;
        public float tempSensDepth;
        public float tempSensNorm;
        public Color tempEdgeColor;
        public Color tempEdgeBgrdColor;
        


        

        void ViewModeCheckAndSet(InfoManager.InfoMode infoMode, string preset)
        {
            if (infoManager.CurrentMode == infoMode)
            {
                currentInfoMode = infoManager.CurrentMode;
                if (!LoadPreset(preset, false))
                {
                    LoadConfig(false);
                    useInfoModeSpecific = true;
                }
            }
        }

        public void Update()
        {
            if (edge.depthsAxis != 1.000f)
            {
                edge.depthsAxis = 1;
            }
            EffectState();
            if (useInfoModeSpecific)
                InfoModes();
            if (isOn)
            {
                if (wantsToneMapper)
                {
                    tonem.m_ToneMappingBoostFactor = toneMapBoost;
                    tonem.m_ToneMappingGamma = toneMapGamma; 
                }
            }
            if (Input.GetKeyUp(config.keyCode))
            {
                if (!showSettingsPanel)
                    tab = Config.Tab.EdgeDetection;
                showSettingsPanel = !showSettingsPanel;
          
            }
            if (Input.GetKeyUp(KeyCode.Escape) && showSettingsPanel)
            {
                overrideFirstTime = true;
                showSettingsPanel = false;
            }
            if (firstTime && Input.GetKeyUp(config.keyCode))
            {
                firstTime = false;
                showSettingsPanel = false;
                tab = Config.Tab.EdgeDetection;
                LowEndAutomatic();
                SaveConfig();

            }

            if (firstTime)
            {
                if (!overrideFirstTime)
                    showSettingsPanel = true;
            }


            if (Input.GetKeyUp(config.edgeToggleKeyCode))
            {
                if (isOn)
                {
                    ToggleBorderedSkylines(false);
                }
                else
                {
                    ToggleBorderedSkylines(true);
                }
                
                
            }


        }

        void ToggleBorderedSkylines(bool state)
        {
            if (state)
            {
                isOn = true;
                edge.enabled = true;
                if (wantsToneMapper)
                {
                    prevGamma = tonem.m_ToneMappingGamma;
                    prevBoost = tonem.m_ToneMappingBoostFactor;
                    tonem.m_ToneMappingGamma = toneMapGamma;
                    tonem.m_ToneMappingBoostFactor = toneMapBoost;
                }
                
                FixTonemapperIfZeroed();
            }
            else
            {
                isOn = false;
                edge.enabled = false;
                if (wantsToneMapper)
                {
                    tonem.m_ToneMappingGamma = prevGamma;
                    tonem.m_ToneMappingBoostFactor = prevBoost;
                    FixTonemapperIfZeroed();
                }
            }
        }
        
        void EffectState()
        {
            if (isOn)
            {
                if (subViewOnly)
                {

                    if (infoManager.CurrentMode == InfoManager.InfoMode.None)
                    {
                        edge.enabled = false;
                    }
                    else
                    {
                        edge.enabled = true;
                    }
                }
                else
                    if (!edge.enabled)
                        edge.enabled = true;

            }
        }

        void SizeCheck(bool value, float min, float max, float depthLimit)
        {
            float size = cameraController.m_currentSize;
            if (min < size && max > size)
            {
                if (value)
                {
                    if (edge.sensitivityDepth >= depthLimit)
                        edge.sensitivityDepth = depthLimit;
                }
                if (!value)
                {
                    edge.sensitivityNormals = Mathf.Lerp(edge.sensitivityNormals, depthLimit, 0.5f);

                }
            }
        }

        void SobelVerticalCheck(bool value, float min, float max, float depthLimit)
        {
            float size = cameraController.m_currentSize;
            if (min < size && max > size)
            {
              edge.mult4 = Mathf.Lerp(edge.mult4, depthLimit, 0.5f);
            }
        }

        
        

        void AutomaticAlgorithms()
        {
            SizeCheck(true, 40f, 100f, 1.523f);
            SizeCheck(true, 100, 200, 2.956f);
            SizeCheck(true, 200, 300, 3.405f);
            SizeCheck(true, 300, 400, 3.584f);
            SizeCheck(true, 400, 500, 5.017f);
            SizeCheck(true, 500, 600, 6.989f);
            SizeCheck(true, 600, 700, 8.691f);
            SizeCheck(true, 700, 800, 9.408f);
            SizeCheck(true, 800, 1000, 12.186f);
            SizeCheck(true, 1000, 1100, 15.681f);

            SizeCheck(false, 40, 222f, 0.65f);
            SizeCheck(false, 100, 476f, 0.833f);
            SizeCheck(false, 476f, 700f, 1.1827f);
            SizeCheck(false, 700, 1274, 1.2f);
            SizeCheck(false, 1274f, 4000, 1.24f);
            //cool mode


            float size = cameraController.m_currentSize;
            if (size < 222f)
            {
                edge.sensitivityDepth = size / 125;
                //edge.sensitivityNormals = Mathf.Lerp(0.57f, 0.93f, 1f);
            }
            if (size >= 222f)
            {
                edge.sensitivityDepth = size / 250;
            }






            if (edge.sensitivityDepth <= 0.44f)
                edge.sensitivityDepth = 0.44f;
            if (edge.sensitivityDepth >= 30f)
                edge.sensitivityDepth = 30f;
            if (edge.sensitivityNormals >= 1.29f)
                edge.sensitivityNormals = 1.29f;
            if (edge.sensitivityNormals <= 0.65f)
                edge.sensitivityNormals = 0.65f;
        }


        void SizeCheckInverted(bool value, float min, float max, float depthLimit)
        {
            float size = cameraController.m_currentSize;
            if (min < size && max > size)
            {
                if (value)
                {
                    edge.axisVsCenter = Mathf.Lerp(edge.axisVsCenter, depthLimit, 0.5f);
                }
            }
        }
        void AutomaticSobelAlg()
        {
            SizeCheckInverted(true, 0, 109, 0.680f);
            SizeCheckInverted(true, 110, 139, 0.620f);
            SizeCheckInverted(true, 130, 169, 0.590f);
            SizeCheckInverted(true, 170, 199, 0.470f);
            SizeCheckInverted(true, 200, 249, 0.446f);
            SizeCheckInverted(true, 250, 299, 0.346f);
            SizeCheckInverted(true, 300, 349, 0.321f);
            SizeCheckInverted(true, 350, 399, 0.271f);
            SizeCheckInverted(true, 400, 459, 0.266f);
            SizeCheckInverted(true, 460, 499, 0.216f);
            SizeCheckInverted(true, 500, 629, 0.176f);
            SizeCheckInverted(true, 630, 699, 0.152f);
            SizeCheckInverted(true, 700, 799, 0.106f);
            SizeCheckInverted(true, 850, 889, 0.099f);
            SizeCheckInverted(true, 889, 919, 0.082f);
            SizeCheckInverted(true, 920, 999, 0.072f);
            SizeCheckInverted(true, 1000, 1119, 0.058f);
            SizeCheckInverted(true, 1120, 1299, 0.035f);
            SizeCheckInverted(true, 1300, 1499, 0.026f);
            SizeCheckInverted(true, 1500, 1799, 0.016f);
            SizeCheckInverted(true, 1800, 2099, 0.012f);
            SizeCheckInverted(true, 2100, 2999, 0.010f);
            SizeCheckInverted(true, 3000, 3699, 0.008f);
            SizeCheckInverted(true, 3700, 4000, 0.007f);
            SobelVerticalCheck(true, 0, 699, 1.953f);
            SobelVerticalCheck(true, 700, 999, 0.663f);
            SobelVerticalCheck(true, 1000, 1399, 0.483f);
            SobelVerticalCheck(true, 1400, 1749, 0.394f);
            SobelVerticalCheck(true, 1750, 4000, 0.268f);



            if (edge.axisVsCenter <= 0.006f)
                edge.axisVsCenter = 0.007f;
            if (edge.axisVsCenter >= 0.591f)
                edge.axisVsCenter = 0.590f;
        }
        public void LateUpdate()
        {
            if (cameraController != null && autoEdge)
            {


                autoEdgeActive = true;
                AutomaticAlgorithms();
            }
            if (!autoEdge && autoEdgeActive)
            {
                edge.sensitivityDepth = config.sensDepth;
                edge.sensitivityNormals = config.sensNorm;
                autoEdgeActive = false;
            }


            if (autoSobelEdge)
            {
                autoSobelEdgeActive = true;
                AutomaticSobelAlg();
            }
            if (!autoSobelEdge && autoSobelEdgeActive)
            {
                autoSobelEdgeActive = false;
            }
        }
    }
}