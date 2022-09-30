using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class ObjectTimeModel : MonoBehaviour
    {
        [System.Serializable]
        class ModelWithFX
        {
            public Material material;
            public int modelID;

            public ModelWithFX(Material newMaterial, int newModelID)
            {
                material = newMaterial;
                modelID = newModelID;
            }
        }

        [Header("Set Values")]
        [SerializeField] ObjectTimeController controller;
        [SerializeField] Material slowAuraMaterial;
        [SerializeField] float auraFadeMod = 1;
        [Header("Runtime Values")]
        [SerializeField] List<ModelWithFX> models;
        [SerializeField] Material currentFX;
        Dictionary<int, ModelWithFX> modelsByID;


        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GetComponent<ObjectTimeController>();
            }

            models = new List<ModelWithFX>();
            modelsByID = new Dictionary<int, ModelWithFX>();

            controller.TimeChanged += OnTimeChanged;
        }
        private void Update()
        {
            if (!(controller.slowMoLeft > 0)) return;
            if (currentFX == null) return;

            Color matColor = currentFX.color;
            matColor.a = matColor.a * controller.slowMoLeft * auraFadeMod;

            foreach (ModelWithFX model in models)
            {
                model.material.color = matColor;
            }
        }

        //Methods
        void UpdateAura(MeshRenderer model, Material material = null)
        {
            //Materials from model
            List<Material> modelMaterials = new List<Material>();
            modelMaterials.AddRange(model.materials);

            if (material != null)
                ApplyAura(model, modelMaterials, material);
            else
                RemoveAura(model, modelMaterials);

            //Update model Materials
            model.materials = modelMaterials.ToArray();

            //If a material was applied, update target material
            if (material != null)
            {
                ModelWithFX target;
                modelsByID.TryGetValue(model.GetInstanceID(), out target);
                int matIndex = modelMaterials.Count - 1;
                target.material = model.materials[matIndex];
            }
        }
        void ApplyAura(MeshRenderer model, List<Material> materials, Material aura)
        {
            //Add SlowMoFX
            Material newAura = new Material(aura);
            materials.Add(newAura);

            //Update SlowedObjectsFX list
            ModelWithFX target = new ModelWithFX(newAura, model.GetInstanceID());
            models.Add(target);
            modelsByID.Add(target.modelID, target);
        }
        void RemoveAura(MeshRenderer model, List<Material> materials)
        {
            ModelWithFX target;
            if (!modelsByID.TryGetValue(model.GetInstanceID(), out target))
            {
                Debug.Log(model + " ERROR WITH AURA");
            }

            //Remove SlowMoFX
            materials.Remove(target.material);
            modelsByID.Remove(target.modelID);

            //Update SlowedObjectsFX list
            models.Remove(target);
        }
        void ApplyFX(Material FX)
        {
            if (models.Count > 0) return;
            currentFX = FX;

            //Apply SlowFX to Transform
            MeshRenderer model = transform.GetComponent<MeshRenderer>();
            if (model != null)
            {
                UpdateAura(model, FX);
            }

            //Apply SlowFX to Children
            if (transform.childCount < 1) return;
            foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>())
            {
                //if renderer is from parent object, skip
                if (renderer.GetInstanceID() == model.GetInstanceID()) continue;
                UpdateAura(renderer, FX);
            }
        }
        void RemoveFX()
        {
            currentFX = null;
            if (!(models.Count > 0)) return;

            //UnApply SlowFX to Transform
            MeshRenderer model = transform.GetComponent<MeshRenderer>();
            if (model != null)
            {
                UpdateAura(model);
            }

            //UnApply SlowFX to Children
            if (transform.childCount < 1) return;
            foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>())
            {
                //if renderer is from parent object, skip
                if (renderer.GetInstanceID() == model.GetInstanceID()) continue;
                UpdateAura(renderer);
            }
        }

        //Event Receivers
        void OnTimeChanged()
        {
            if(controller.slowMoLeft > 0)
            ApplyFX(slowAuraMaterial);
            else
            RemoveFX();
        }
    }
}