using UnityEngine;
using System.Collections;

namespace BeautifyEffect {
				public class Demo2 : MonoBehaviour {
								int demoMode = 0;
								GUIStyle labelStyle;

								void Start () {
												UpdateDemoMode ();
								}

								void OnGUI () {
												Rect rect = new Rect (20, Screen.height - 60, Screen.width - 20, 30);
												GUI.Label (rect, "Move around with WASD or cursor keys and mouse to look. Press left mouse button to change mode and T to toggle on/off.");

												rect = new Rect (20, Screen.height - 40, Screen.width - 20, 30);
												if (labelStyle == null) {
																labelStyle = new GUIStyle (GUI.skin.label);
																labelStyle.fontStyle = FontStyle.Bold;
												}

												switch (demoMode) {
												case 0:
																GUI.Label (rect, "BEAUTIFY OFF (click to enable)", labelStyle);
																break;
												case 1:
																GUI.Label (rect, "BEAUTIFY ON", labelStyle);
																break;
												case 2:
																GUI.Label (rect, "BEAUTIFY ON + vignetting", labelStyle);
																break;
												case 3:
																GUI.Label (rect, "BEAUTIFY ON + vignetting + bloom", labelStyle);
																break;
												case 4:
																GUI.Label (rect, "BEAUTIFY ON + vignetting + bloom + lens dirt", labelStyle);
																break;
												case 5:
																GUI.Label (rect, "BEAUTIFY ON + vignetting + lens dirt + anamorphic flares", labelStyle);
																break;
												case 6:
																GUI.Label (rect, "BEAUTIFY ON + vignetting + lens dirt + vertical anamorphic flares", labelStyle);
																break;
												case 7:
																GUI.Label (rect, "BEAUTIFY ON + vignetting + bloom + lens dirt + night vision", labelStyle);
																break;
												case 8:
																GUI.Label (rect, "BEAUTIFY ON + red vignetting + bloom + lens dirt + thermal vision", labelStyle);
																break;
												case 9:
																GUI.Label (rect, "BEAUTIFY ON + sepia + outline + frame", labelStyle);
																break;
												}
								}

								void Update () {
												if (Input.GetMouseButtonDown (0)) {
																demoMode++;
																if (demoMode >= 10)
																				demoMode = 0;
																UpdateDemoMode ();
												} else if (Input.GetKeyDown (KeyCode.T)) {
																if (demoMode > 0)
																				demoMode = 0;
																else
																				demoMode = 1;
																UpdateDemoMode ();
												}
								}

								void UpdateDemoMode () {
												if (demoMode == 0) {
																Beautify.instance.enabled = false;
																return;
												}

												Beautify.instance.enabled = true;

												switch (demoMode) {
												case 1:
																Beautify.instance.sepia = false;
																Beautify.instance.outline = false;
																Beautify.instance.nightVision = false;
																Beautify.instance.bloom = false;
																Beautify.instance.anamorphicFlares = false;
																Beautify.instance.lensDirt = false;
																Beautify.instance.vignetting = false;
																Beautify.instance.frame = false;
																break;
												case 2:
																Beautify.instance.vignetting = true;
																Beautify.instance.vignettingColor = new Color (0, 0, 0, 0.05f);
																break;
												case 3:
																Beautify.instance.bloom = true;
																break;
												case 4:
																Beautify.instance.lensDirt = true;
																break;
												case 5:
																Beautify.instance.bloom = false;
																Beautify.instance.anamorphicFlares = true;
																break;
												case 6:
																Beautify.instance.anamorphicFlaresVertical = true;
																break;
												case 7:
																Beautify.instance.bloom = true;
																Beautify.instance.anamorphicFlares = false;
																Beautify.instance.nightVision = true;
																break;
												case 8:
																Beautify.instance.thermalVision = true;
																break;
												case 9:
																Beautify.instance.thermalVision = false;
																Beautify.instance.vignetting = false;
																Beautify.instance.sepia = true;
																Beautify.instance.outline = true;
																Beautify.instance.bloom = false;
																Beautify.instance.anamorphicFlares = false;
																Beautify.instance.anamorphicFlaresVertical = false;
																Beautify.instance.lensDirt = false;
																Beautify.instance.frame = true;
																break;
												}
								}
				}

}