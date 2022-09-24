using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class TimeChangerModel : MonoBehaviourSingletonInScene<TimeChangerModel>
    {
        [System.Serializable]
        class SlowMoTarget
        {
            public Transform transform;
            public Material material;
            public int modelID;

            public SlowMoTarget(Transform transform, Material material, int modelID)
            {
                this.transform = transform;
                this.material = material;
                this.modelID = modelID;
            }
        }

        [Header("Set Values")]
        [SerializeField] TimeChanger controller;
        [SerializeField] Material slowAuraMaterial;
        [SerializeField] float auraFadeMod;
        [Header("Runtime Values")]
        [SerializeField] ObjectTimeModel[] slowableObjs;
        [SerializeField] List<SlowMoTarget> slowableObjects;
        [SerializeField] Transform newObjectSlowed;
        [SerializeField] Transform newObjectUnSlowed;
        [SerializeField] float newTimer;
        Dictionary<int, SlowMoTarget> slowableObjectsByID;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = TimeChanger.Get();
            }

            controller.ActivatingCharge += OnActivatingCharge;
            controller.ReleasedCharge += OnReleasedCharge;

            slowableObjectsByID = new Dictionary<int, SlowMoTarget>();
        }
        private void Update()
        {
            if (slowableObjects.Count < 1) return;
            foreach (var slowedObject in slowableObjects)
            {
                Color matColor = slowAuraMaterial.color;
                matColor.a = matColor.a * (controller.publicCharge);
                slowedObject.material.color = matColor;
            }
        }

        //Methods
        void SetSlowAura(MeshRenderer model, bool auraOn)
        {
            //Materials from model
            List<Material> modelMaterials = new List<Material>();
            modelMaterials.AddRange(model.materials);

            if (auraOn)
            {
                if (slowableObjectsByID.ContainsKey(model.GetInstanceID())) return;

                //Add SlowMoFX
                Material aura = new Material(slowAuraMaterial);
                modelMaterials.Add(aura);

                //Update model Materials
                model.materials = modelMaterials.ToArray();
                aura = model.materials[model.materials.Length - 1];

                //Update SlowableObjectsFX list
                int modelID = model.GetInstanceID();
                SlowMoTarget target = new SlowMoTarget(transform, aura, modelID);
                slowableObjects.Add(target);
                slowableObjectsByID.Add(target.modelID, target);
            }
            else
            {
                SlowMoTarget target;
                if (!slowableObjectsByID.TryGetValue(model.GetInstanceID(), out target))
                {
                    Debug.Log(model + " ERROR WITH AURA");
                    //Debug.Break();
                    //bool errorDebugHelper = false;
                }

                //Remove SlowableFX
                modelMaterials.Remove(target.material);
                slowableObjectsByID.Remove(target.modelID);

                //Update SlowableObjectsFX list
                slowableObjects.Remove(target);

                //Update model Materials
                model.materials = modelMaterials.ToArray();
            }
        }
        void ApplyFX(Transform objectSlowed)
        {
            //Apply SlowFX to Transform
            MeshRenderer model = objectSlowed.GetComponent<MeshRenderer>();
            if (model != null)
            {
                SetSlowAura(model, true);
            }

            //Apply SlowFX to Children
            if (objectSlowed.childCount < 1) return;
            foreach (var renderer in objectSlowed.GetComponentsInChildren<MeshRenderer>())
            {
                //If Model has FX already, skip
                if (slowableObjectsByID.ContainsKey(renderer.GetInstanceID())) continue;

                SetSlowAura(renderer, true);
            }
        }
        void RemoveFX(Transform objectSlowed)
        {
            //UnApply SlowFX to Transform
            MeshRenderer model = objectSlowed.GetComponent<MeshRenderer>();
            if (model != null)
            {
                SetSlowAura(model, false);
            }

            //UnApply SlowFX to Children
            if (objectSlowed.childCount < 1) return;
            foreach (var renderer in objectSlowed.GetComponentsInChildren<MeshRenderer>())
            {
                //If Model doesn't have FX, skip
                if (!slowableObjectsByID.ContainsKey(renderer.GetInstanceID())) continue;
                SetSlowAura(renderer, false);
            }
        }

        //Event Receivers
        void OnActivatingCharge()
        {
            ObjectTimeModel[] slowableObjs = FindObjectsOfType<ObjectTimeModel>();

            foreach (var timeableObj in slowableObjs)
            {
                ApplyFX(timeableObj.transform);
            }
        }
        void OnReleasedCharge()
        {
            foreach (var timeableObj in slowableObjects)
            {
                RemoveFX(timeableObj.transform); 
            }
        }
    }
}