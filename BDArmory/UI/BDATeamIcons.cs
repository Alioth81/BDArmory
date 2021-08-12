﻿using System.Collections.Generic;
using BDArmory.Modules;
using UnityEngine;
using BDArmory.Control;
using System;

namespace BDArmory.UI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class BDATeamIcons : MonoBehaviour
    {
        public BDATeamIcons Instance;

        public Material IconMat;

        void Awake()
        {
            if (Instance)
            {
                Destroy(this);
            }
            else
                Instance = this;
        }
        GUIStyle IconUIStyle;

        GUIStyle mIStyle;
        Color Teamcolor;

        private void Start()
        {
            IconUIStyle = new GUIStyle();
            IconUIStyle.fontStyle = FontStyle.Bold;
            IconUIStyle.fontSize = 10;
            IconUIStyle.normal.textColor = XKCDColors.Red;//replace with BDATISetup defined value varable.

            mIStyle = new GUIStyle();
            mIStyle.fontStyle = FontStyle.Normal;
            mIStyle.fontSize = 10;
            mIStyle.normal.textColor = XKCDColors.Yellow;

            IconMat = new Material(Shader.Find("KSP/Particles/Alpha Blended"));
        }

        private void DrawOnScreenIcon(Vector3 worldPos, Texture texture, Vector2 size, Color Teamcolor, bool ShowPointer)
        {
            if (Event.current.type.Equals(EventType.Repaint))
            {
                bool offscreen = false;
                Vector3 screenPos = BDGUIUtils.GetMainCamera().WorldToViewportPoint(worldPos);
                if (screenPos.z < 0)
                {
                    offscreen = true;
                    screenPos.x *= -1;
                    screenPos.y *= -1;
                }
                if (screenPos.x != Mathf.Clamp01(screenPos.x))
                {
                    offscreen = true;
                }
                if (screenPos.y != Mathf.Clamp01(screenPos.y))
                {
                    offscreen = true;
                }
                float xPos = (screenPos.x * Screen.width) - (0.5f * size.x);
                float yPos = ((1 - screenPos.y) * Screen.height) - (0.5f * size.y);
                float xtPos = 1 * (Screen.width / 2);
                float ytPos = 1 * (Screen.height / 2);

                if (!offscreen)
                {
                    IconMat.SetColor("_TintColor", Teamcolor);
                    IconMat.mainTexture = texture;
                    Rect iconRect = new Rect(xPos, yPos, size.x, size.y);
                    Graphics.DrawTexture(iconRect, texture, IconMat);
                }
                else
                {
                    if (BDTISettings.POINTERS)
                    {
                        Vector2 head;
                        Vector2 tail;

                        head.x = xPos;
                        head.y = yPos;
                        tail.x = xtPos;
                        tail.y = ytPos;
                        float angle = Vector2.Angle(Vector3.up, tail - head);
                        if (tail.x < head.x)
                        {
                            angle = -angle;
                        }
                        if (ShowPointer && BDTISettings.POINTERS)
                        {
                            DrawPointer(calculateRadialCoords(head, tail, angle, 0.75f), angle, 4, Teamcolor);
                        }
                    }
                }

            }
        }
        private void DrawThreatIndicator(Vector3 vesselPos, Vector3 targetPos, Color Teamcolor)
        {
            if (Event.current.type.Equals(EventType.Repaint))
            {
                Vector3 screenPos = BDGUIUtils.GetMainCamera().WorldToViewportPoint(vesselPos);
                Vector3 screenTPos = BDGUIUtils.GetMainCamera().WorldToViewportPoint(targetPos);
                if (screenTPos.z > 0)
                {
                    float xPos = (screenPos.x * Screen.width);
                    float yPos = ((1 - screenPos.y) * Screen.height);
                    float xtPos = (screenTPos.x * Screen.width);
                    float ytPos = ((1 - screenTPos.y) * Screen.height);

                    Vector2 head;
                    Vector2 tail;

                    head.x = xPos;
                    head.y = yPos;
                    tail.x = xtPos;
                    tail.y = ytPos;
                    float angle = Vector2.Angle(Vector3.up, tail - head);
                    if (tail.x < head.x)
                    {
                        angle = -angle;
                    }
                    DrawPointer(tail, (angle - 180), 2, Teamcolor);
                }
            }
        }
        public Vector2 calculateRadialCoords(Vector2 RadialCoord, Vector2 Tail, float angle, float edgeDistance)
        {
            float theta = Mathf.Abs(angle);
            if (theta > 90)
            {
                theta -= 90;
            }
            theta = theta * Mathf.Deg2Rad; //needs to be in radians for Mathf. trig
            float Cos = Mathf.Cos(theta);
            float Sin = Mathf.Sin(theta);

            if (RadialCoord.y >= Tail.y)
            {
                if (RadialCoord.x >= Tail.x) // set up Quads 3-4
                {
                    RadialCoord.x = (Cos * (edgeDistance * Tail.x)) + Tail.x;
                }
                else
                {
                    RadialCoord.x = Tail.x - ((Cos * edgeDistance) * Tail.x);
                }
                RadialCoord.y = (Sin * (edgeDistance * Tail.y)) + Tail.y;
            }
            else
            {
                if (RadialCoord.x >= Tail.x) // set up Quads 1-2 
                {
                    RadialCoord.x = (Sin * (edgeDistance * Tail.x)) + Tail.x;
                }
                else
                {
                    RadialCoord.x = Tail.x - ((Sin * edgeDistance) * Tail.x);
                }
                RadialCoord.y = Tail.y - ((Cos * edgeDistance) * Tail.y);
            }
            return RadialCoord;
        }
        public static void DrawPointer(Vector2 Pointer, float angle, float width, Color color)
        {
            Camera cam = BDGUIUtils.GetMainCamera();

            if (cam == null) return;

            GUI.matrix = Matrix4x4.identity;
            float length = 60;

            Rect upRect = new Rect(Pointer.x - (width / 2), Pointer.y - length, width, length);
            GUIUtility.RotateAroundPivot(-angle + 180, Pointer);
            BDGUIUtils.DrawRectangle(upRect, color);
            GUI.matrix = Matrix4x4.identity;
        }
        void OnGUI()
        {
            if ((HighLogic.LoadedSceneIsFlight && BDArmorySetup.GAME_UI_ENABLED && !MapView.MapIsEnabled && BDTISettings.TEAMICONS) || HighLogic.LoadedSceneIsFlight && !BDArmorySetup.GAME_UI_ENABLED && !MapView.MapIsEnabled && BDTISettings.TEAMICONS && BDTISettings.PERSISTANT)
            {
                Texture icon;
                float size = 40;

                using (List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator())
                    while (v.MoveNext())
                    {
                        if (v.Current == null) continue;
                        if (!v.Current.loaded || v.Current.packed || v.Current.isActiveVessel) continue;
                        if (VesselModuleRegistry.ignoredVesselTypes.Contains(v.Current.vesselType)) continue;

                        if (BDTISettings.MISSILES)
                        {
                            using (var ml = VesselModuleRegistry.GetModules<MissileBase>(v.Current).GetEnumerator())
                                while (ml.MoveNext())
                                {
                                    if (ml.Current == null) continue;
                                    if (ml.Current.MissileState != MissileBase.MissileStates.Idle && ml.Current.MissileState != MissileBase.MissileStates.Drop)
                                    {
                                        Vector3 sPos = FlightGlobals.ActiveVessel.vesselTransform.position;
                                        Vector3 tPos = v.Current.vesselTransform.position;
                                        Vector3 Dist = (tPos - sPos);
                                        Vector2 guiPos;
                                        string UIdist;
                                        string UoM;
                                        if (Dist.magnitude > 100)
                                        {
                                            if ((Dist.magnitude / 1000) >= 1)
                                            {
                                                UoM = "km";
                                                UIdist = (Dist.magnitude / 1000).ToString("0.00");
                                            }
                                            else
                                            {
                                                UoM = "m";
                                                UIdist = Dist.magnitude.ToString("0.0");
                                            }
                                            BDGUIUtils.DrawTextureOnWorldPos(v.Current.CoM, BDTISetup.Instance.TextureIconMissile, new Vector2(20, 20), 0);
                                            if (BDGUIUtils.WorldToGUIPos(ml.Current.vessel.CoM, out guiPos))
                                            {
                                                Rect distRect = new Rect((guiPos.x - 12), (guiPos.y + 10), 100, 32);
                                                GUI.Label(distRect, UIdist + UoM, mIStyle);
                                            }

                                        }
                                    }
                                }
                        }
                        if (BDTISettings.DEBRIS)
                        {
                            if (v.Current.vesselType != VesselType.Debris && !v.Current.isActiveVessel) continue;
                            if (v.Current.LandedOrSplashed) continue;
                            {
                                Vector3 sPos = FlightGlobals.ActiveVessel.vesselTransform.position;
                                Vector3 tPos = v.Current.vesselTransform.position;
                                Vector3 Dist = (tPos - sPos);
                                if (Dist.magnitude > 100)
                                {
                                    BDGUIUtils.DrawTextureOnWorldPos(v.Current.CoM, BDTISetup.Instance.TextureIconDebris, new Vector2(20, 20), 0);
                                }
                            }
                        }
                    }
                int Teamcount = 0;
                using (var teamManagers = BDTISetup.Instance.weaponManagers.GetEnumerator())
                    while (teamManagers.MoveNext())
                    {
                        Teamcount++;
                        using (var wm = teamManagers.Current.Value.GetEnumerator())
                            while (wm.MoveNext())
                            {
                                if (wm.Current == null) continue;
                                Teamcolor = BDTISetup.Instance.ColorAssignments[wm.Current.Team.Name];
                                IconUIStyle.normal.textColor = Teamcolor;
                                if (wm.Current.vessel.isActiveVessel)
                                {
                                    if (BDTISettings.THREATICON)
                                    {
                                        if (wm.Current.currentTarget == null) continue;
                                        Vector3 sPos = FlightGlobals.ActiveVessel.CoM;
                                        Vector3 tPos = (wm.Current.currentTarget.Vessel.CoM);
                                        Vector3 RelPos = (tPos - sPos);
                                        if (RelPos.magnitude >= 100)
                                        {
                                            DrawThreatIndicator(wm.Current.vessel.CoM, wm.Current.currentTarget.Vessel.CoM, Teamcolor);
                                        }
                                    }
                                }
                                else
                                {

                                    Vector3 selfPos = FlightGlobals.ActiveVessel.CoM;
                                    Vector3 targetPos = (wm.Current.vessel.CoM);
                                    Vector3 targetRelPos = (targetPos - selfPos);
                                    Vector2 guiPos;
                                    float distance;
                                    string UIdist;
                                    string UoM;
                                    string vName;
                                    string selectedWeapon = String.Empty;
                                    string AIstate = String.Empty;
                                    distance = targetRelPos.magnitude;
                                    if (distance >= 100)
                                    {
                                        if ((distance / 1000) >= 1)
                                        {
                                            UoM = "km";
                                            UIdist = (distance / 1000).ToString("0.00");
                                        }
                                        else
                                        {
                                            UoM = "m";
                                            UIdist = distance.ToString("0.0");
                                        }
                                        if ((wm.Current.vessel.vesselType == VesselType.Ship && !wm.Current.vessel.Splashed) || wm.Current.vessel.vesselType == VesselType.Plane)
                                        {
                                            icon = BDTISetup.Instance.TextureIconPlane;
                                        }
                                        else if (wm.Current.vessel.vesselType == VesselType.Base || wm.Current.vessel.vesselType == VesselType.Lander)
                                        {
                                            icon = BDTISetup.Instance.TextureIconBase;
                                        }
                                        else if (wm.Current.vessel.vesselType == VesselType.Rover)
                                        {
                                            icon = BDTISetup.Instance.TextureIconRover;
                                        }
                                        else if (wm.Current.vessel.vesselType == VesselType.Probe)
                                        {
                                            icon = BDTISetup.Instance.TextureIconProbe;
                                        }
                                        else if (wm.Current.vessel.vesselType == VesselType.Ship && wm.Current.vessel.Splashed)
                                        {
                                            icon = BDTISetup.Instance.TextureIconShip;
                                            if (wm.Current.vessel.vesselType == VesselType.Ship && wm.Current.vessel.altitude < -10)
                                            {
                                                icon = BDTISetup.Instance.TextureIconSub;
                                            }
                                        }
                                        else if (wm.Current.vessel.vesselType == VesselType.Debris)
                                        {
                                            icon = BDTISetup.Instance.TextureIconDebris;
                                            size = 20;
                                            IconUIStyle.normal.textColor = XKCDColors.Grey;
                                            Teamcolor = XKCDColors.Grey;
                                        }
                                        else
                                        {
                                            icon = BDTISetup.Instance.TextureIconGeneric;
                                        }
                                        DrawOnScreenIcon(wm.Current.vessel.CoM, icon, new Vector2((size * BDTISettings.ICONSCALE), (size * BDTISettings.ICONSCALE)), Teamcolor, true);
                                        if (BDTISettings.THREATICON)
                                        {
                                            if (wm.Current.currentTarget != null)
                                            {
                                                if (!wm.Current.currentTarget.Vessel.isActiveVessel)
                                                {
                                                    DrawThreatIndicator(wm.Current.vessel.CoM, wm.Current.currentTarget.Vessel.CoM, Teamcolor);
                                                }
                                            }
                                        }
                                        if (BDGUIUtils.WorldToGUIPos(wm.Current.vessel.CoM, out guiPos))
                                        {
                                            if (BDTISettings.VESSELNAMES)
                                            {
                                                vName = wm.Current.vessel.vesselName;
                                                Rect nameRect = new Rect((guiPos.x + (24 * BDTISettings.ICONSCALE)), guiPos.y - 4, 100, 32);
                                                GUI.Label(nameRect, vName, IconUIStyle);
                                            }
                                            if (BDTISettings.TEAMNAMES)
                                            {
                                                Rect teamRect = new Rect((guiPos.x + (16 * BDTISettings.ICONSCALE)), (guiPos.y - (19 * BDTISettings.ICONSCALE)), 100, 32);
                                                GUI.Label(teamRect, "Team: " + $"{wm.Current.Team.Name}", IconUIStyle);
                                            }

                                            if (BDTISettings.SCORE)
                                            {
                                                BDArmory.Control.ScoringData scoreData = null;
                                                int Score = 0;

                                                if (BDACompetitionMode.Instance.Scores.ScoreData.ContainsKey(wm.Current.vessel.vesselName))
                                                {
                                                    scoreData = BDACompetitionMode.Instance.Scores.ScoreData[wm.Current.vessel.vesselName];
                                                    Score = scoreData.hits;
                                                }
                                                if (VesselSpawner.Instance.vesselsSpawningContinuously)
                                                {
                                                    if (VesselSpawner.Instance.continuousSpawningScores.ContainsKey(wm.Current.vessel.vesselName))
                                                    {
                                                        Score += VesselSpawner.Instance.continuousSpawningScores[wm.Current.vessel.vesselName].cumulativeHits;
                                                    }
                                                }

                                                Rect scoreRect = new Rect((guiPos.x + (16 * BDTISettings.ICONSCALE)), (guiPos.y + (14 * BDTISettings.ICONSCALE)), 100, 32);
                                                GUI.Label(scoreRect, "Score: " + Score, IconUIStyle);
                                            }
                                            if (BDTISettings.HEALTHBAR)
                                            {

                                                double hpPercent = 1;
                                                hpPercent = Mathf.Clamp(wm.Current.currentHP / wm.Current.totalHP, 0, 1);
                                                if (hpPercent > 0)
                                                {
                                                    Rect barRect = new Rect((guiPos.x - (32 * BDTISettings.ICONSCALE)), (guiPos.y + (30 * BDTISettings.ICONSCALE)), (64 * BDTISettings.ICONSCALE), 12);
                                                    Rect healthRect = new Rect((guiPos.x - (30 * BDTISettings.ICONSCALE)), (guiPos.y + (32 * BDTISettings.ICONSCALE)), (60 * (float)hpPercent * BDTISettings.ICONSCALE), 8);
                                                    //GUI.Label(healthRect, "Team: " + $"{wm.Current.Team.Name}", IconUIStyle);
                                                    BDGUIUtils.DrawRectangle(barRect, XKCDColors.Grey);
                                                    BDGUIUtils.DrawRectangle(healthRect, Color.HSVToRGB((85f * (float)hpPercent) / 255, 1f, 1f));

                                                }
                                                Rect distRect = new Rect((guiPos.x - 12), (guiPos.y + (45 * BDTISettings.ICONSCALE)), 100, 32);
                                                GUI.Label(distRect, UIdist + UoM, IconUIStyle);
                                            }
                                            else
                                            {
                                                Rect distRect = new Rect((guiPos.x - 12), (guiPos.y + (20 * BDTISettings.ICONSCALE)), 100, 32);
                                                GUI.Label(distRect, UIdist + UoM, IconUIStyle);
                                            }
                                            if (BDTISettings.TELEMETRY)
                                            {
                                                selectedWeapon = "Using: " + wm.Current.selectedWeaponString;
                                                AIstate = "No AI";
                                                if (wm.Current.AI != null)
                                                {
                                                    AIstate = "Pilot " + wm.Current.AI.currentStatus;
                                                }
                                                Rect telemetryRect = new Rect((guiPos.x + (32 * BDTISettings.ICONSCALE)), guiPos.y + 32, 200, 32);
                                                GUI.Label(telemetryRect, selectedWeapon, IconUIStyle);
                                                Rect telemetryRect2 = new Rect((guiPos.x + (32 * BDTISettings.ICONSCALE)), guiPos.y + 48, 200, 32);
                                                GUI.Label(telemetryRect2, AIstate, IconUIStyle);
                                                if (wm.Current.isFlaring || wm.Current.isChaffing || wm.Current.isECMJamming)
                                                {
                                                    Rect telemetryRect3 = new Rect((guiPos.x + (32 * BDTISettings.ICONSCALE)), guiPos.y + 64, 200, 32);
                                                    GUI.Label(telemetryRect3, "Deploying Counter-Measures", IconUIStyle);
                                                }
                                                Rect SpeedRect = new Rect((guiPos.x - (96 * BDTISettings.ICONSCALE)), guiPos.y + 64, 100, 32);
                                                GUI.Label(SpeedRect, "Speed: " + wm.Current.vessel.speed.ToString("0.0") + "m/s", IconUIStyle);
                                                Rect RAltRect = new Rect((guiPos.x - (96 * BDTISettings.ICONSCALE)), guiPos.y + 80, 100, 32);
                                                GUI.Label(RAltRect, "Alt: " + wm.Current.vessel.altitude.ToString("0.0") + "m", IconUIStyle);
                                                Rect ThrottleRect = new Rect((guiPos.x - (96 * BDTISettings.ICONSCALE)), guiPos.y + 96, 100, 32);
                                                GUI.Label(ThrottleRect, "Throttle: " + Mathf.CeilToInt(wm.Current.vessel.ctrlState.mainThrottle*100) + "%", IconUIStyle);
                                            }
                                        }
                                    }
                                }
                            }
                    }
            }
        }
    }
}
