using System;
using System.Collections.Generic;
using System.Linq;
using BDArmory.Bullets;
using BDArmory.Competition;
using BDArmory.Control;
using BDArmory.Core;
using BDArmory.Core.Extension;
using BDArmory.Core.Module;
using BDArmory.Core.Utils;
using BDArmory.Misc;
using BDArmory.Modules;
using BDArmory.UI;
using UnityEngine;

namespace BDArmory.FX
{
    public class ExplosionFx : MonoBehaviour
    {
        public static Dictionary<string, ObjectPool> explosionFXPools = new Dictionary<string, ObjectPool>();
        public KSPParticleEmitter[] pEmitters { get; set; }
        public Light LightFx { get; set; }
        public float StartTime { get; set; }
        public AudioClip ExSound { get; set; }
        public AudioSource audioSource { get; set; }
        private float MaxTime { get; set; }
        public float Range { get; set; }
        public float Caliber { get; set; }
        public float ProjMass { get; set; }
        public ExplosionSourceType ExplosionSource { get; set; }
        public string SourceVesselName { get; set; }
        public string SourceWeaponName { get; set; }
        public float Power { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float AngleOfEffect { get; set; }
        public Part ExplosivePart { get; set; }
        public bool isFX { get; set; }
        public float CASEClamp { get; set; }
        public float dmgMult { get; set; }
        public float TimeIndex => Time.time - StartTime;

        private bool disabled = true;

        Queue<BlastHitEvent> explosionEvents = new Queue<BlastHitEvent>();
        List<BlastHitEvent> explosionEventsPreProcessing = new List<BlastHitEvent>();
        List<Part> explosionEventsPartsAdded = new List<Part>();
        List<DestructibleBuilding> explosionEventsBuildingAdded = new List<DestructibleBuilding>();
        Dictionary<string, int> explosionEventsVesselsHit = new Dictionary<string, int>();


        static RaycastHit[] lineOfSightHits;
        static RaycastHit[] reverseHits;
        static Collider[] overlapSphereColliders;
        public static List<Part> IgnoreParts;
        public static List<DestructibleBuilding> IgnoreBuildings;
        internal static readonly float ExplosionVelocity = 422.75f;

        private float particlesMaxEnergy;
        internal static HashSet<ExplosionSourceType> ignoreCasingFor = new HashSet<ExplosionSourceType> { ExplosionSourceType.Missile, ExplosionSourceType.Rocket };

        void Awake()
        {
            if (lineOfSightHits == null) { lineOfSightHits = new RaycastHit[100]; }
            if (reverseHits == null) { reverseHits = new RaycastHit[100]; }
            if (overlapSphereColliders == null) { overlapSphereColliders = new Collider[100]; }
            if (IgnoreParts == null) { IgnoreParts = new List<Part>(); }
            if (IgnoreBuildings == null) { IgnoreBuildings = new List<DestructibleBuilding>(); }
        }

        private void OnEnable()
        {
            StartTime = Time.time;
            disabled = false;
            MaxTime = Mathf.Sqrt((Range / ExplosionVelocity) * 3f) * 2f; // Scale MaxTime to get a reasonable visualisation of the explosion.
            if (!isFX)
            {
                CalculateBlastEvents();
            }
            pEmitters = gameObject.GetComponentsInChildren<KSPParticleEmitter>();
            foreach (var pe in pEmitters)
                if (pe != null)
                {
                    if (pe.maxEnergy > particlesMaxEnergy)
                        particlesMaxEnergy = pe.maxEnergy;
                    pe.emit = true;
                    var emission = pe.ps.emission;
                    emission.enabled = true;
                    EffectBehaviour.AddParticleEmitter(pe);
                }

            LightFx = gameObject.GetComponent<Light>();
            LightFx.range = Range * 3f;

            if (BDArmorySettings.DRAW_DEBUG_LABELS)
            {
                Debug.Log("[BDArmory.ExplosionFX]: Explosion started tntMass: {" + Power + "}  BlastRadius: {" + Range + "} StartTime: {" + StartTime + "}, Duration: {" + MaxTime + "}");
            }
            if (BDArmorySettings.PERSISTANT_FX && Caliber > 30 && Misc.Misc.GetRadarAltitudeAtPos(transform.position) > Caliber / 60)
            {
                FXEmitter.CreateFX(Position, (Caliber / 30), "BDArmory/Models/explosion/flakSmoke", "", 0.1f, Caliber / 4);
            }
        }

        void OnDisable()
        {
            foreach (var pe in pEmitters)
            {
                if (pe != null)
                {
                    pe.emit = false;
                    EffectBehaviour.RemoveParticleEmitter(pe);
                }
            }
            ExplosivePart = null; // Clear the Part reference.
            explosionEvents.Clear(); // Make sure we don't have any left over events leaking memory.
            explosionEventsPreProcessing.Clear();
            explosionEventsPartsAdded.Clear();
            explosionEventsBuildingAdded.Clear();
            explosionEventsVesselsHit.Clear();
        }

        private void CalculateBlastEvents()
        {
            //Let's convert this temporal list on a ordered queue
            // using (var enuEvents = temporalEventList.OrderBy(e => e.TimeToImpact).GetEnumerator())
            using (var enuEvents = ProcessingBlastSphere().OrderBy(e => e.TimeToImpact).GetEnumerator())
            {
                while (enuEvents.MoveNext())
                {
                    if (enuEvents.Current == null) continue;

                    if (BDArmorySettings.DRAW_DEBUG_LABELS)
                    {
                        Debug.Log("[BDArmory.ExplosionFX]: Enqueueing Blast Event");
                    }

                    explosionEvents.Enqueue(enuEvents.Current);
                }
            }
        }

        private List<BlastHitEvent> ProcessingBlastSphere()
        {
            explosionEventsPreProcessing.Clear();
            explosionEventsPartsAdded.Clear();
            explosionEventsBuildingAdded.Clear();
            explosionEventsVesselsHit.Clear();

            string sourceVesselName = null;
            if (BDACompetitionMode.Instance)
            {
                switch (ExplosionSource)
                {
                    case ExplosionSourceType.Missile:
                        var explosivePart = ExplosivePart ? ExplosivePart.FindModuleImplementing<BDExplosivePart>() : null;
                        sourceVesselName = explosivePart ? explosivePart.sourcevessel.GetName() : SourceVesselName;
                        break;
                    default: // Everything else.
                        sourceVesselName = SourceVesselName;
                        break;
                }
            }
            var overlapSphereColliderCount = Physics.OverlapSphereNonAlloc(Position, Range, overlapSphereColliders, 9076737);
            if (overlapSphereColliderCount == overlapSphereColliders.Length)
            {
                overlapSphereColliders = Physics.OverlapSphere(Position, Range, 9076737);
                overlapSphereColliderCount = overlapSphereColliders.Length;
            }
            using (var hitCollidersEnu = overlapSphereColliders.Take(overlapSphereColliderCount).GetEnumerator())
            {
                while (hitCollidersEnu.MoveNext())
                {
                    if (hitCollidersEnu.Current == null) continue;

                    Part partHit = hitCollidersEnu.Current.GetComponentInParent<Part>();
                    if (partHit == null) continue;
                    if (ProjectileUtils.IsIgnoredPart(partHit)) continue; // Ignore ignored parts.

                    if (partHit != null && partHit.mass > 0 && !explosionEventsPartsAdded.Contains(partHit))
                    {
                        var damaged = ProcessPartEvent(partHit, sourceVesselName, explosionEventsPreProcessing, explosionEventsPartsAdded);
                        // If the explosion derives from a missile explosion, count the parts damaged for missile hit scores.
                        if (damaged && BDACompetitionMode.Instance)
                        {
                            bool registered = false;
                            var damagedVesselName = partHit.vessel != null ? partHit.vessel.GetName() : null;
                            switch (ExplosionSource)
                            {
                                case ExplosionSourceType.Rocket:
                                    if (BDACompetitionMode.Instance.Scores.RegisterRocketHit(sourceVesselName, damagedVesselName, 1))
                                        registered = true;
                                    break;
                                case ExplosionSourceType.Missile:
                                    if (BDACompetitionMode.Instance.Scores.RegisterMissileHit(sourceVesselName, damagedVesselName, 1))
                                        registered = true;
                                    break;
                            }
                            if (registered)
                            {
                                if (explosionEventsVesselsHit.ContainsKey(damagedVesselName))
                                    ++explosionEventsVesselsHit[damagedVesselName];
                                else
                                    explosionEventsVesselsHit[damagedVesselName] = 1;
                            }
                        }
                    }
                    else
                    {
                        DestructibleBuilding building = hitCollidersEnu.Current.GetComponentInParent<DestructibleBuilding>();

                        if (building != null && !explosionEventsBuildingAdded.Contains(building))
                        {
                            ProcessBuildingEvent(building, explosionEventsPreProcessing, explosionEventsBuildingAdded);
                        }
                    }
                }
            }
            if (explosionEventsVesselsHit.Count > 0)
            {
                if (ExplosionSource != ExplosionSourceType.Bullet || ExplosionSource != ExplosionSourceType.Rocket)
                {
                    string message = "";
                    foreach (var vesselName in explosionEventsVesselsHit.Keys)
                        message += (message == "" ? "" : " and ") + vesselName + " had " + explosionEventsVesselsHit[vesselName];
                    if (ExplosionSource == ExplosionSourceType.Missile)
                    {
                        message += " parts damaged due to missile strike";
                    }
                    else //ExplosionType BattleDamage || Other
                    {
                        message += " parts damaged due to explosion";
                    }
                    message += (SourceWeaponName != null ? " (" + SourceWeaponName + ")" : "") + (sourceVesselName != null ? " from " + sourceVesselName : "") + ".";
                    BDACompetitionMode.Instance.competitionStatus.Add(message);
                }
                // Note: damage hasn't actually been applied to the parts yet, just assigned as events, so we can't know if they survived.
                foreach (var vesselName in explosionEventsVesselsHit.Keys) // Note: sourceVesselName is already checked for being in the competition before damagedVesselName is added to explosionEventsVesselsHitByMissiles, so we don't need to check it here.
                {
                    switch (ExplosionSource)
                    {
                        case ExplosionSourceType.Rocket:
                            BDACompetitionMode.Instance.Scores.RegisterRocketStrike(sourceVesselName, vesselName);
                            break;
                        case ExplosionSourceType.Missile:
                            BDACompetitionMode.Instance.Scores.RegisterMissileStrike(sourceVesselName, vesselName);
                            break;
                    }
                }
            }
            return explosionEventsPreProcessing;
        }

        private void ProcessBuildingEvent(DestructibleBuilding building, List<BlastHitEvent> eventList, List<DestructibleBuilding> buildingAdded)
        {
            Ray ray = new Ray(Position, building.transform.position - Position);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Range, 9076737))
            {
                //TODO: Maybe we are not hitting building because we are hitting explosive parts.

                DestructibleBuilding destructibleBuilding = rayHit.collider.GetComponentInParent<DestructibleBuilding>();

                // Is not a direct hit, because we are hitting a different part
                if (destructibleBuilding != null && destructibleBuilding.Equals(building))
                {
                    var distance = Vector3.Distance(Position, rayHit.point);
                    eventList.Add(new BuildingBlastHitEvent() { Distance = Vector3.Distance(Position, rayHit.point), Building = building, TimeToImpact = distance / ExplosionVelocity });
                    buildingAdded.Add(building);
                }
            }
        }

        private bool ProcessPartEvent(Part part, string sourceVesselName, List<BlastHitEvent> eventList, List<Part> partsAdded)
        {
            RaycastHit hit;
            float distance = 0;
            List<Tuple<float, float, float>> intermediateParts;
            if (IsInLineOfSight(part, ExplosivePart, out hit, out distance, out intermediateParts))
            {
                if (IsAngleAllowed(Direction, hit))
                {
                    //Adding damage hit
                    if (distance <= Range)//part within blast
                    {
                        eventList.Add(new PartBlastHitEvent()
                        {
                            Distance = distance,
                            Part = part,
                            TimeToImpact = distance / ExplosionVelocity,
                            HitPoint = hit.point,
                            Hit = hit,
                            SourceVesselName = sourceVesselName,
                            IntermediateParts = intermediateParts
                        });
                    }
                    if (ProjMass > 0 && distance <= Range * 2)
                    {
                        ProjectileUtils.CalculateShrapnelDamage(part, hit, 120, Power, distance, sourceVesselName, ExplosionSource, ProjMass); //part hit by shrapnel, but not pressure wave
                    }
                    partsAdded.Add(part);
                    return true;
                }
            }
            return false;
        }

        private bool IsAngleAllowed(Vector3 direction, RaycastHit hit)
        {
            if (direction == default(Vector3))
            {
                return true;
            }

            return Vector3.Angle(direction, (hit.point - Position).normalized) <= AngleOfEffect;
        }

        /// <summary>
        /// This method will calculate if there is valid line of sight between the explosion origin and the specific Part
        /// In order to avoid collisions with the same missile part, It will not take into account those parts beloging to same vessel that contains the explosive part
        /// </summary>
        /// <param name="part"></param>
        /// <param name="explosivePart"></param>
        /// <param name="hit"> out property with the actual hit</param>
        /// <returns></returns>
        private bool IsInLineOfSight(Part part, Part explosivePart, out RaycastHit hit, out float distance, out List<Tuple<float, float, float>> intermediateParts)
        {
            Ray partRay = new Ray(Position, part.transform.position - Position);
            var hitCount = Physics.RaycastNonAlloc(partRay, lineOfSightHits, Range, 9076737);
            if (hitCount == lineOfSightHits.Length) // If there's a whole bunch of stuff in the way (unlikely), then we need to increase the size of our hits buffer.
            {
                lineOfSightHits = Physics.RaycastAll(partRay, Range, 9076737);
                hitCount = lineOfSightHits.Length;
            }
            int reverseHitCount = 0;
            //check if explosion is originating inside a part
            reverseHitCount = Physics.RaycastNonAlloc(new Ray(part.transform.position - Position, Position), reverseHits, Range, 9076737);
            if (reverseHitCount == reverseHits.Length)
                {
                    reverseHits = Physics.RaycastAll(new Ray(part.transform.position - Position, Position), Range, 9076737);
                    reverseHitCount = reverseHits.Length;
                }
             for (int i = 0; i < reverseHitCount; ++i)
                { reverseHits[i].distance = Range - reverseHits[i].distance; }

            intermediateParts = new List<Tuple<float, float, float>>();

                using (var hitsEnu = lineOfSightHits.Take(hitCount).Concat(reverseHits.Take(reverseHitCount)).OrderBy(x => x.distance).GetEnumerator())
                    while (hitsEnu.MoveNext())
                    {
                        Part partHit = hitsEnu.Current.collider.GetComponentInParent<Part>();
                        if (partHit == null) continue;
                        if (ProjectileUtils.IsIgnoredPart(partHit)) continue; // Ignore ignored parts.
                        hit = hitsEnu.Current;
                        distance = hit.distance;
                        if (partHit == part)
                        {
                            return true;
                        }
                        if (partHit != part)
                        {
                            // ignoring collisions against the explosive, or explosive vessel for certain explosive types (e.g., missile/rocket casing)
                            if (partHit == explosivePart || (explosivePart != null && ignoreCasingFor.Contains(ExplosionSource) && partHit.vessel == explosivePart.vessel))
                            {
                                continue;
                            }
                            if (FlightGlobals.currentMainBody != null && hit.collider.gameObject == FlightGlobals.currentMainBody.gameObject) return false; // Terrain hit. Full absorption. Should avoid NREs in the following.
                            var partHP = partHit.Damage();
                            var partArmour = partHit.GetArmorThickness();
                            if (partHP > 0) // Ignore parts that are already dead but not yet removed from the game.
                                intermediateParts.Add(new Tuple<float, float, float>(hit.distance, partHP, partArmour));
                        }
                    }

                hit = new RaycastHit();
                distance = 0;
                return false;
        }

        public void Update()
        {
            if (!gameObject.activeInHierarchy) return;

            if (LightFx != null) LightFx.intensity -= 12 * Time.deltaTime;

            if (!disabled && TimeIndex > 0.3f && pEmitters != null) // 0.3s seems to be enough to always show the explosion, but 0.2s isn't for some reason.
            {
                foreach (var pe in pEmitters)
                {
                    if (pe == null) continue;
                    pe.emit = false;
                }
                disabled = true;
            }
        }

        public void FixedUpdate()
        {
            if (!gameObject.activeInHierarchy) return;

            //floating origin and velocity offloading corrections
            if (!FloatingOrigin.Offset.IsZero() || !Krakensbane.GetFrameVelocity().IsZero())
            {
                transform.position -= FloatingOrigin.OffsetNonKrakensbane;
            }
            if (!isFX)
            {
                while (explosionEvents.Count > 0 && explosionEvents.Peek().TimeToImpact <= TimeIndex)
                {
                    BlastHitEvent eventToExecute = explosionEvents.Dequeue();

                    var partBlastHitEvent = eventToExecute as PartBlastHitEvent;
                    if (partBlastHitEvent != null)
                    {
                        ExecutePartBlastEvent(partBlastHitEvent);
                    }
                    else
                    {
                        ExecuteBuildingBlastEvent((BuildingBlastHitEvent)eventToExecute);
                    }
                }
            }

            if (disabled && explosionEvents.Count == 0 && TimeIndex > MaxTime)
            {
                if (BDArmorySettings.DRAW_DEBUG_LABELS)
                {
                    Debug.Log("[BDArmory.ExplosionFX]: Explosion Finished");
                }

                gameObject.SetActive(false);
                return;
            }
        }

        private void ExecuteBuildingBlastEvent(BuildingBlastHitEvent eventToExecute)
        {
            //TODO: Review if the damage is sensible after so many changes
            //buildings
            DestructibleBuilding building = eventToExecute.Building;
            building.damageDecay = 600f;

            if (building)
            {
                var distanceFactor = Mathf.Clamp01((Range - eventToExecute.Distance) / Range);
                float damageToBuilding = (BDArmorySettings.DMG_MULTIPLIER / 100) * BDArmorySettings.EXP_DMG_MOD_BALLISTIC_NEW * Power * distanceFactor;

                damageToBuilding *= 2f;

                building.AddDamage(damageToBuilding);

                if (building.Damage > building.impactMomentumThreshold)
                {
                    building.Demolish();
                }
                if (BDArmorySettings.DRAW_DEBUG_LABELS)
                {
                    Debug.Log("[BDArmory.ExplosionFX]: Explosion hit destructible building! Hitpoints Applied: " + Mathf.Round(damageToBuilding) +
                             ", Building Damage : " + Mathf.Round(building.Damage) +
                             " Building Threshold : " + building.impactMomentumThreshold);
                }
            }
        }

        private void ExecutePartBlastEvent(PartBlastHitEvent eventToExecute)
        {
            if (eventToExecute.Part == null || eventToExecute.Part.Rigidbody == null || eventToExecute.Part.vessel == null || eventToExecute.Part.partInfo == null) return;

            Part part = eventToExecute.Part;
            Rigidbody rb = part.Rigidbody;
            var realDistance = eventToExecute.Distance;
            var vesselMass = part.vessel.totalMass;
            if (vesselMass == 0) vesselMass = part.mass; // Sometimes if the root part is the only part of the vessel, then part.vessel.totalMass is 0, despite the part.mass not being 0.

            if (!eventToExecute.IsNegativePressure)
            {
                BlastInfo blastInfo = BlastPhysicsUtils.CalculatePartBlastEffects(part, realDistance, vesselMass * 1000f, Power, Range);

                // Overly simplistic approach: simply reduce damage by amount of HP/2 and Armour in the way. (HP/2 to simulate weak parts not fully blocking damage.) Does not account for armour reduction or angle of incidence of intermediate parts.
                // A better approach would be to properly calculate the damage and pressure in CalculatePartBlastEffects due to the series of parts in the way.
                var damageWithoutIntermediateParts = blastInfo.Damage;
                var cumulativeHPOfIntermediateParts = eventToExecute.IntermediateParts.Select(p => p.Item2).Sum();
                var cumulativeArmourOfIntermediateParts = eventToExecute.IntermediateParts.Select(p => p.Item3).Sum();
                blastInfo.Damage = Mathf.Max(0f, blastInfo.Damage - 0.5f * cumulativeHPOfIntermediateParts - cumulativeArmourOfIntermediateParts);

                if (CASEClamp > 0)
                {
                    if (CASEClamp < 1000)
                    {
                        blastInfo.Damage = Mathf.Clamp(blastInfo.Damage, 0, Mathf.Min((part.Modules.GetModule<HitpointTracker>().GetMaxHitpoints() * 0.9f), CASEClamp));
                    }
                    else
                    {
                        blastInfo.Damage = Mathf.Clamp(blastInfo.Damage, 0, CASEClamp);
                    }
                }

                if (blastInfo.Damage > 0)
                {
                    if (BDArmorySettings.DRAW_DEBUG_LABELS)
                    {
                        Debug.Log(
                            "[BDArmory.ExplosionFX]: Executing blast event Part: {" + part.name + "}, " +
                            " VelocityChange: {" + blastInfo.VelocityChange + "}," +
                            " Distance: {" + realDistance + "}," +
                            " TotalPressure: {" + blastInfo.TotalPressure + "}," +
                            " Damage: {" + blastInfo.Damage + "} (reduced from " + damageWithoutIntermediateParts + " by " + eventToExecute.IntermediateParts.Count + " parts)," +
                            " EffectiveArea: {" + blastInfo.EffectivePartArea + "}," +
                            " Positive Phase duration: {" + blastInfo.PositivePhaseDuration + "}," +
                            " Vessel mass: {" + Math.Round(vesselMass * 1000f) + "}," +
                            " TimeIndex: {" + TimeIndex + "}," +
                            " TimePlanned: {" + eventToExecute.TimeToImpact + "}," +
                            " NegativePressure: {" + eventToExecute.IsNegativePressure + "}");
                    }

                    // Add Reverse Negative Event
                    explosionEvents.Enqueue(new PartBlastHitEvent()
                    {
                        Distance = Range - realDistance,
                        Part = part,
                        TimeToImpact = 2 * (Range / ExplosionVelocity) + (Range - realDistance) / ExplosionVelocity,
                        IsNegativePressure = true,
                        NegativeForce = blastInfo.VelocityChange * 0.25f
                    });

                    if (rb != null && rb.mass > 0)
                    {
                        AddForceAtPosition(rb,
                            (eventToExecute.HitPoint + rb.velocity * TimeIndex - Position).normalized *
                            blastInfo.VelocityChange *
                            BDArmorySettings.EXP_IMP_MOD,
                            eventToExecute.HitPoint + rb.velocity * TimeIndex);
                    }
                    var damage = 0f;
                    if (dmgMult < 0)
                    {
                        part.AddInstagibDamage();
                        //Debug.Log("[ExplosionFX] applying instagib!");
                    }
                    if (!ProjectileUtils.CalculateExplosiveArmorDamage(part, blastInfo.TotalPressure, SourceVesselName, eventToExecute.Hit, ExplosionSource)) //false = armor blowthrough
                    {
                        damage = part.AddExplosiveDamage(blastInfo.Damage, Caliber, ExplosionSource, dmgMult);
                    }
                    if (damage > 0) //else damage from spalling done in CalcExplArmorDamage
                    {
                        if (BDArmorySettings.BATTLEDAMAGE)
                        {
                            Misc.BattleDamageHandler.CheckDamageFX(part, 50, 0.5f, true, false, SourceVesselName, eventToExecute.Hit);
                        }
                        // Update scoring structures
                        var aName = eventToExecute.SourceVesselName; // Attacker
                        var tName = part.vessel.GetName(); // Target
                        switch (ExplosionSource)
                        {
                            case ExplosionSourceType.Bullet:
                                BDACompetitionMode.Instance.Scores.RegisterBulletDamage(aName, tName, damage);
                                break;
                            case ExplosionSourceType.Rocket:
                                BDACompetitionMode.Instance.Scores.RegisterRocketDamage(aName, tName, damage);
                                break;
                            case ExplosionSourceType.Missile:
                                BDACompetitionMode.Instance.Scores.RegisterMissileDamage(aName, tName, damage);
                                break;
                            case ExplosionSourceType.BattleDamage:
                                BDACompetitionMode.Instance.Scores.RegisterBattleDamage(aName, part.vessel, damage);
                                break;
                        }
                    }
                }
                else if (BDArmorySettings.DRAW_DEBUG_LABELS)
                {
                    Debug.Log("[BDArmory.ExplosiveFX]: Part " + part.name + " at distance " + realDistance + "m took no damage due to parts with " + cumulativeHPOfIntermediateParts + "HP and " + cumulativeArmourOfIntermediateParts + " Armour in the way.");
                }
            }
            else
            {
                if (BDArmorySettings.DRAW_DEBUG_LABELS)
                {
                    Debug.Log(
                        "[BDArmory.ExplosionFX]: Executing blast event Part: {" + part.name + "}, " +
                        " VelocityChange: {" + eventToExecute.NegativeForce + "}," +
                        " Distance: {" + realDistance + "}," +
                        " Vessel mass: {" + Math.Round(vesselMass * 1000f) + "}," +
                        " TimeIndex: {" + TimeIndex + "}," +
                        " TimePlanned: {" + eventToExecute.TimeToImpact + "}," +
                        " NegativePressure: {" + eventToExecute.IsNegativePressure + "}");
                }
                if (rb != null && rb.mass > 0)
                    AddForceAtPosition(rb, (Position - part.transform.position).normalized * eventToExecute.NegativeForce * BDArmorySettings.EXP_IMP_MOD * 0.25f, part.transform.position);
            }
        }

        // We use an ObjectPool for the ExplosionFx instances as they leak KSPParticleEmitters otherwise.
        static void CreateObjectPool(string explModelPath, string soundPath)
        {
            var key = explModelPath + soundPath;
            if (!explosionFXPools.ContainsKey(key) || explosionFXPools[key] == null)
            {
                var explosionFXTemplate = GameDatabase.Instance.GetModel(explModelPath);
                if (explosionFXTemplate == null)
                {
                    Debug.LogError("[BDArmory.ExplosionFX]: " + explModelPath + " was not found, using the default explosion instead. Please fix your model.");
                    explosionFXTemplate = GameDatabase.Instance.GetModel(ModuleWeapon.defaultExplModelPath);
                }
                var soundClip = GameDatabase.Instance.GetAudioClip(soundPath);
                if (soundClip == null)
                {
                    Debug.LogError("[BDArmory.ExplosionFX]: " + soundPath + " was not found, using the default sound instead. Please fix your model.");
                    soundClip = GameDatabase.Instance.GetAudioClip(ModuleWeapon.defaultExplSoundPath);
                }
                var eFx = explosionFXTemplate.AddComponent<ExplosionFx>();
                eFx.ExSound = soundClip;
                eFx.audioSource = explosionFXTemplate.AddComponent<AudioSource>();
                eFx.audioSource.minDistance = 200;
                eFx.audioSource.maxDistance = 5500;
                eFx.audioSource.spatialBlend = 1;
                eFx.LightFx = explosionFXTemplate.AddComponent<Light>();
                eFx.LightFx.color = Misc.Misc.ParseColor255("255,238,184,255");
                eFx.LightFx.intensity = 8;
                eFx.LightFx.shadows = LightShadows.None;
                explosionFXTemplate.SetActive(false);
                explosionFXPools[key] = ObjectPool.CreateObjectPool(explosionFXTemplate, 10, true, true, 0f, false);
            }
        }

        public static void CreateExplosion(Vector3 position, float tntMassEquivalent, string explModelPath, string soundPath, ExplosionSourceType explosionSourceType,
            float caliber = 120, Part explosivePart = null, string sourceVesselName = null, string sourceWeaponName = null, Vector3 direction = default(Vector3), float angle = 100f, bool isfx = false, float projectilemass = 0, float caseLimiter = -1, float dmgMutator = 1)
        {
            CreateObjectPool(explModelPath, soundPath);

            Quaternion rotation;
            if (direction == default(Vector3))
            {
                rotation = Quaternion.LookRotation(VectorUtils.GetUpDirection(position));
            }
            else
            {
                rotation = Quaternion.LookRotation(direction);
            }

            GameObject newExplosion = explosionFXPools[explModelPath + soundPath].GetPooledObject();
            newExplosion.transform.SetPositionAndRotation(position, rotation);
            ExplosionFx eFx = newExplosion.GetComponent<ExplosionFx>();
            eFx.Range = BlastPhysicsUtils.CalculateBlastRange(tntMassEquivalent);
            eFx.Position = position;
            eFx.Power = tntMassEquivalent;
            eFx.ExplosionSource = explosionSourceType;
            eFx.SourceVesselName = !string.IsNullOrEmpty(sourceVesselName) ? sourceVesselName : explosionSourceType == ExplosionSourceType.Missile ? (explosivePart != null && explosivePart.vessel != null ? explosivePart.vessel.GetName() : null) : null; // Use the sourceVesselName if specified, otherwise get the sourceVesselName from the missile if it is one.
            eFx.SourceWeaponName = sourceWeaponName;
            eFx.Caliber = caliber;
            eFx.ExplosivePart = explosivePart;
            eFx.Direction = direction;
            eFx.AngleOfEffect = angle >= 0f ? Mathf.Clamp(angle, 0f, 180f) : 100f;
            eFx.isFX = isfx;
            eFx.ProjMass = projectilemass;
            eFx.CASEClamp = caseLimiter;
            eFx.dmgMult = dmgMutator;
            eFx.pEmitters = newExplosion.GetComponentsInChildren<KSPParticleEmitter>();
            eFx.audioSource = newExplosion.GetComponent<AudioSource>();
            if (tntMassEquivalent <= 5)
            {
                eFx.audioSource.minDistance = 4f;
                eFx.audioSource.maxDistance = 3000;
                eFx.audioSource.priority = 9999;
            }
            newExplosion.SetActive(true);
        }

        public static void AddForceAtPosition(Rigidbody rb, Vector3 force, Vector3 position)
        {
            //////////////////////////////////////////////////////////
            // Add The force to part
            //////////////////////////////////////////////////////////
            if (rb == null || rb.mass == 0) return;
            rb.AddForceAtPosition(force, position, ForceMode.VelocityChange);
            if (BDArmorySettings.DRAW_DEBUG_LABELS)
            {
                Debug.Log("[BDArmory.ExplosionFX]: Force Applied | Explosive : " + Math.Round(force.magnitude, 2));
            }
        }
    }

    public abstract class BlastHitEvent
    {
        public float Distance { get; set; }
        public float TimeToImpact { get; set; }
        public bool IsNegativePressure { get; set; }
    }

    internal class PartBlastHitEvent : BlastHitEvent
    {
        public Part Part { get; set; }
        public Vector3 HitPoint { get; set; }
        public RaycastHit Hit { get; set; }
        public float NegativeForce { get; set; }
        public string SourceVesselName { get; set; }
        public List<Tuple<float, float, float>> IntermediateParts { get; set; } // distance, HP, armour
    }

    internal class BuildingBlastHitEvent : BlastHitEvent
    {
        public DestructibleBuilding Building { get; set; }
    }
}
