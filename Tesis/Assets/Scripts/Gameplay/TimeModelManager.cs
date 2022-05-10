using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class TimeModelManager : MonoBehaviourSingletonInScene<TimeModelManager>
    {
        [Header("Set Values")]
        [SerializeField] TimeManager manager;
        [SerializeField] Material slowAuraMaterial;

        //Unity Events
        private void Start()
        {
            if (!manager)
            {
                manager = TimeManager.Get();
            }

            manager.ObjectSlowed += OnObjectSlowed;
            manager.ObjectUnSlowed += OnObjectUnSlowed;
        }

        //Methods
        void SetSlowAura(MeshRenderer model, bool auraOn)
        {
            List<Material> modelMaterials = new List<Material>();
            modelMaterials.AddRange(model.materials);
            Material aura = slowAuraMaterial;
            if (auraOn)
            {
                modelMaterials.Add(aura);
            }
            else
            {
                modelMaterials.RemoveAt(modelMaterials.Count-1);
            }
            model.materials = modelMaterials.ToArray();
        }

        //Event Receivers
        void OnObjectSlowed(Transform objectSlowed)
        {
            MeshRenderer model = objectSlowed.GetComponent<MeshRenderer>();
            SetSlowAura(model, true);
        }
        void OnObjectUnSlowed(Transform objectSlowed)
        {
            MeshRenderer model = objectSlowed.GetComponent<MeshRenderer>();
            SetSlowAura(model, false);
        }
    }
}