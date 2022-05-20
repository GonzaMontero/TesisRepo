using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class TimeManagerModel : MonoBehaviourSingletonInScene<TimeManagerModel>
    {
        [System.Serializable]
        class SlowMoTarget
        {
            public Material material;
            public float timer;
            public float maxTimer;
            public int modelID;

            public SlowMoTarget(Material newMaterial, float newTimer, int newModelID)
            {
                material = newMaterial;
                timer = newTimer;
                maxTimer = newTimer;
                modelID = newModelID;
            }
        }

        [Header("Set Values")]
        [SerializeField] TimeManager manager;
        [SerializeField] Material slowAuraMaterial;
        [SerializeField] List<SlowMoTarget> slowedObjects;
        [SerializeField] float auraFadeMod;
        [Header("Runtime Values")]
        [SerializeField] Dictionary<int, SlowMoTarget> slowedObjectsByID;
        [SerializeField] Transform newObjectSlowed;
        [SerializeField] Transform newObjectUnSlowed;
        [SerializeField] float newTimer;

        //Unity Events
        private void Start()
        {
            if (!manager)
            {
                manager = TimeManager.Get();
            }

            manager.ObjectSlowed += OnObjectSlowed;
            manager.ObjectUnSlowed += OnObjectUnSlowed;

            slowedObjectsByID = new Dictionary<int, SlowMoTarget>();
        }
        private void Update()
        {
            if (slowedObjects.Count < 1) return;
            foreach (var slowedObject in slowedObjects)
            {
                
                Color matColor = slowAuraMaterial.color;
                slowedObject.timer -= Time.unscaledDeltaTime * auraFadeMod;
                matColor.a = matColor.a * (slowedObject.timer / slowedObject.maxTimer);
                slowedObject.material.color = matColor;
            }
        }

        //Methods
        void SetSlowAura(MeshRenderer model, bool auraOn, float timer = 0)
        {
            //Materials from model
            List<Material> modelMaterials = new List<Material>();
            modelMaterials.AddRange(model.materials);

            if (auraOn)
            {
                //Add SlowMoFX
                Material aura = new Material(slowAuraMaterial);
                modelMaterials.Add(aura);

                //Update SlowedObjectsFX list
                SlowMoTarget target = new SlowMoTarget(aura, timer, model.GetInstanceID());
                slowedObjects.Add(target);
                slowedObjectsByID.Add(target.modelID, target);
            }
            else
            {
                SlowMoTarget target;
                if (!slowedObjectsByID.TryGetValue(model.GetInstanceID(), out target))
                {
                    Debug.Log(model + " ERROR WITH AURA");
                    //Debug.Break();
                    //bool errorDebugHelper = false;
                }

                //Remove SlowMoFX
                //modelMaterials.RemoveAt(modelMaterials.Count - 1);
                modelMaterials.Remove(modelMaterials[modelMaterials.Count-1]);
                slowedObjectsByID.Remove(target.modelID);

                //Update SlowedObjectsFX list
                slowedObjects.Remove(target);
            }

            //Update model Materials
            model.materials = modelMaterials.ToArray();
        }
        void ApplyFX()
        {
            //Apply SlowFX to Transform
            MeshRenderer model = newObjectSlowed.GetComponent<MeshRenderer>();
            if (model != null)
            {
                SetSlowAura(model, true, newTimer);
            }

            //Apply SlowFX to Children
            if (newObjectSlowed.childCount < 1) return;
            foreach (var renderer in newObjectSlowed.GetComponentsInChildren<MeshRenderer>())
            {
                //If Model has FX already, skip
                if (slowedObjectsByID.ContainsKey(renderer.GetInstanceID())) continue;

                SetSlowAura(renderer, true, newTimer);
            }
        }
        void RemoveFX()
        {
            //UnApply SlowFX to Transform
            MeshRenderer model = newObjectUnSlowed.GetComponent<MeshRenderer>();
            if (model != null)
            {
                SetSlowAura(model, false);
            }

            //UnApply SlowFX to Children
            if (newObjectUnSlowed.childCount < 1) return;
            foreach (var renderer in newObjectUnSlowed.GetComponentsInChildren<MeshRenderer>())
            {
                //If Model doesn't have FX, skip
                if (!slowedObjectsByID.ContainsKey(renderer.GetInstanceID())) continue;
                SetSlowAura(renderer, false);
            }
        }

        //Event Receivers
        void OnObjectSlowed(Transform objectSlowed, float timer)
        {
            newObjectSlowed = objectSlowed;
            newTimer = timer;
            Invoke("ApplyFX", manager.publicDelay * Time.timeScale);
        }
        void OnObjectUnSlowed(Transform objectSlowed)
        {
            newObjectUnSlowed = objectSlowed;
            Invoke("RemoveFX", manager.publicDelay * Time.timeScale);
        }
    }
}