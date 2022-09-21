using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.TimePhys
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
        [SerializeField] Transform newObjectSlowed;
        [SerializeField] Transform newObjectUnSlowed;
        [SerializeField] float newTimer;
        Dictionary<int, SlowMoTarget> slowedObjectsByID;

        //Unity Events
        private void Start()
        {
            if (!manager)
            {
                manager = TimeManager.Get();
            }

            manager.ObjectSlowed += OnObjectSlowed;
            manager.ObjectUnSlowed += OnObjectUnSlowed;
            manager.ObjectDestroyed += OnObjectDestroyed;

            slowedObjectsByID = new Dictionary<int, SlowMoTarget>();
        }
        private void Update()
        {
            if (slowedObjects.Count < 1) return;
            foreach (var slowedObject in slowedObjects)
            {
                
                Color matColor = slowAuraMaterial.color;
                slowedObject.timer -= Time.deltaTime * auraFadeMod;
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
        void ApplyFX(Transform objectSlowed, float timer)
        {
            //Apply SlowFX to Transform
            MeshRenderer model = objectSlowed.GetComponent<MeshRenderer>();
            if (model != null)
            {
                SetSlowAura(model, true, timer);
            }

            //Apply SlowFX to Children
            if (objectSlowed.childCount < 1) return;
            foreach (var renderer in objectSlowed.GetComponentsInChildren<MeshRenderer>())
            {
                //If Model has FX already, skip
                if (slowedObjectsByID.ContainsKey(renderer.GetInstanceID())) continue;

                SetSlowAura(renderer, true, timer);
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
                if (!slowedObjectsByID.ContainsKey(renderer.GetInstanceID())) continue;
                SetSlowAura(renderer, false);
            }
        }

        //Routines
        IEnumerator ApplyFXRoutine(Transform objectSlowed, float timer)
        {
            yield return new WaitForSeconds(manager.publicDelay);
            ApplyFX(objectSlowed, timer);
        }
        IEnumerator RemoveFXRoutine(Transform objectSlowed)
        {
            yield return new WaitForSeconds(manager.publicDelay);
            RemoveFX(objectSlowed);
        }

        //Event Receivers
        void OnObjectSlowed(Transform objectSlowed, float timer)
        {
            StartCoroutine(ApplyFXRoutine(objectSlowed, timer));
        }
        void OnObjectUnSlowed(Transform objectSlowed)
        {
            StartCoroutine(RemoveFXRoutine(objectSlowed));
        }
        void OnObjectDestroyed(int objectID)
        {
            //Get Removed object
            SlowMoTarget target;
            if(!slowedObjectsByID.TryGetValue(objectID, out target)) return;

            //Update SlowedObjectsFX list
            slowedObjectsByID.Remove(objectID);
            slowedObjects.Remove(target);
        }
    }
}