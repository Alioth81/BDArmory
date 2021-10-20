﻿
using BDArmory.Misc;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BDArmory.FX
{
	class FXEmitter : MonoBehaviour
    {
        public static Dictionary<string, ObjectPool> FXPools = new Dictionary<string, ObjectPool>();
        public KSPParticleEmitter[] pEmitters { get; set; }
        public float StartTime { get; set; }
        public AudioClip ExSound { get; set; }
        public AudioSource audioSource { get; set; }
        private float Power { get; set; }
        private float emitTime { get; set; }
        private float maxTime { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float TimeIndex => Time.time - StartTime;

        private bool disabled = true;
        public static string defaultModelPath = "BDArmory/Models/explosion/flakSmoke";
        public static string defaultSoundPath = "BDArmory/Sounds/explode1";
        private float particlesMaxEnergy;
        private void OnEnable()
        {
            StartTime = Time.time;
            disabled = false;

            pEmitters = gameObject.GetComponentsInChildren<KSPParticleEmitter>();
            foreach (var pe in pEmitters)
                if (pe != null)
                {
                    pe.maxSize *= Power;
                    pe.maxParticleSize *= Power;
                    pe.minSize *= Power;
                    if (maxTime > 0)
                    {
                        pe.maxEnergy = maxTime;
                        pe.minEnergy = maxTime * .66f;
                    }
                    if (pe.maxEnergy > particlesMaxEnergy)
                        particlesMaxEnergy = pe.maxEnergy;
                    pe.emit = true;
                    var emission = pe.ps.emission;
                    emission.enabled = true;
                    EffectBehaviour.AddParticleEmitter(pe);
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
        }
        public void Update()
        {
            if (!gameObject.activeInHierarchy) return;

            if (!disabled && TimeIndex > emitTime && pEmitters != null) 
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

            if (!FloatingOrigin.Offset.IsZero() || !Krakensbane.GetFrameVelocity().IsZero())
            {
                transform.position -= FloatingOrigin.OffsetNonKrakensbane;
            }

            if (disabled && TimeIndex > particlesMaxEnergy)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        static void CreateObjectPool(string ModelPath, string soundPath)
        {
            var key = ModelPath + soundPath;
            if (!FXPools.ContainsKey(key) || FXPools[key] == null)
            {
                var FXTemplate = GameDatabase.Instance.GetModel(ModelPath);
                if (FXTemplate == null)
                {
                    Debug.LogError("[BDArmory.FXBase]: " + ModelPath + " was not found, using the default model instead. Please fix your model.");
                    FXTemplate = GameDatabase.Instance.GetModel(defaultModelPath);
                }
                var eFx = FXTemplate.AddComponent<FXEmitter>();
                if (!String.IsNullOrEmpty(soundPath))
                {
                    var soundClip = GameDatabase.Instance.GetAudioClip(soundPath);

                    eFx.ExSound = soundClip;
                    eFx.audioSource = FXTemplate.AddComponent<AudioSource>();
                    eFx.audioSource.minDistance = 200;
                    eFx.audioSource.maxDistance = 5500;
                    eFx.audioSource.spatialBlend = 1;

                }
                FXTemplate.SetActive(false);
                FXPools[key] = ObjectPool.CreateObjectPool(FXTemplate, 10, true, true, 0f, false);
            }
        }

        public static void CreateFX(Vector3 position, float scale, string ModelPath, string soundPath, float time = 0.3f, float lifeTime = -1, Vector3 direction = default(Vector3))            
        {
            CreateObjectPool(ModelPath, soundPath);

            Quaternion rotation;
            if (direction == default(Vector3))
            {
                rotation = Quaternion.LookRotation(VectorUtils.GetUpDirection(position));
            }
            else
            {
                rotation = Quaternion.LookRotation(direction);
            }

            GameObject newFX = FXPools[ModelPath + soundPath].GetPooledObject();
            newFX.transform.SetPositionAndRotation(position, rotation);
            Debug.Log("[FXEmitter] start scale: " + newFX.transform.localScale);
            newFX.transform.localScale = Vector3.one;
            newFX.transform.localScale *= scale;
            FXEmitter eFx = newFX.GetComponent<FXEmitter>();

            eFx.Position = position;
            eFx.Power = scale;
            eFx.emitTime = time;
            eFx.maxTime = lifeTime;
            eFx.pEmitters = newFX.GetComponentsInChildren<KSPParticleEmitter>();
            if (!String.IsNullOrEmpty(soundPath))
            {
                eFx.audioSource = newFX.GetComponent<AudioSource>();
                if (scale > 3)
                {
                    eFx.audioSource.minDistance = 4f;
                    eFx.audioSource.maxDistance = 3000;
                    eFx.audioSource.priority = 9999;
                }
            }
            newFX.SetActive(true);
        }
    }
}
