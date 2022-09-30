using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class TurbineWindArea : MonoBehaviour, ITimed
    {
        [SerializeField] TimePhys.ObjectTimeController timeController;
        [SerializeField] Transform windLocation;
        [SerializeField] Collider windArea;
        [SerializeField] float windForce;

        [Header("Particle System")] [SerializeField]
        List<ParticleSystem> particleSystem1 = new List<ParticleSystem>();

        [SerializeField] List<ParticleSystem> particleSystem2 = new List<ParticleSystem>();

        [SerializeField] float randomDelayLow;
        [SerializeField] float randomDelayHigh;
        [SerializeField] float timeBetweenPositionSwap;

        private int particleSystem1Amount;
        private int particleSystem2Amount;

        #region Wind Particles

        void Start()
        {
            particleSystem1Amount = particleSystem1.Count;
            particleSystem2Amount = particleSystem2.Count;

            if (particleSystem1Amount > 0 && particleSystem2Amount > 0)
            {
                StartCoroutine(SpawnParticleSystems());
            }

            if (timeController == null)
            {
                timeController = GetComponent<TimePhys.ObjectTimeController>();
            }
        }

        IEnumerator SpawnParticleSystems()
        {
            yield return new WaitForSeconds(timeBetweenPositionSwap);
            while (timeController.slowMoLeft > 0)
            {
                yield return null;
            }

            DisableParticles();

            Vector3 randomVector;
            for (short i = 0; i < particleSystem1Amount; i++)
            {
                randomVector = GenerateRandomPositionInCollider();

                particleSystem1[i].transform.position = randomVector;
                particleSystem1[i].gameObject.SetActive(true);
#pragma warning disable CS0618 // Type or member is obsolete
                particleSystem1[i].startDelay = Random.Range(randomDelayLow, randomDelayHigh);
#pragma warning restore CS0618 // Type or member is obsolete
                particleSystem1[i].Play();
            }

            for (short i = 0; i < particleSystem2Amount; i++)
            {
                randomVector = GenerateRandomPositionInCollider();

                particleSystem2[i].transform.position = randomVector;
                particleSystem2[i].gameObject.SetActive(true);
#pragma warning disable CS0618 // Type or member is obsolete
                particleSystem2[i].startDelay = Random.Range(randomDelayLow, randomDelayHigh);
#pragma warning restore CS0618 // Type or member is obsolete
                particleSystem2[i].Play();
            }

            StartCoroutine(SpawnParticleSystems());
        }

        Vector3 GenerateRandomPositionInCollider()
        {
            float zpos = Random.Range(windArea.bounds.min.z, windArea.bounds.max.z);
            float yPos = Random.Range(windArea.bounds.min.y, windArea.bounds.max.y);
            float xPos = Random.Range(windArea.bounds.min.x, windArea.bounds.max.x);

            return new Vector3(xPos, yPos, zpos);
        }

        void DisableParticles()
        {
            for (short i = 0; i < particleSystem1Amount; i++)
            {
                particleSystem1[i].gameObject.SetActive(false);
            }

            for (short i = 0; i < particleSystem2Amount; i++)
            {
                particleSystem2[i].gameObject.SetActive(false);
            }
        }

        #endregion

        #region Wind Force

        private void OnTriggerStay(Collider other)
        {
            if(timeController.slowMoLeft > 0) return;
            if (other.CompareTag("Player"))
            {
                other.attachedRigidbody.AddForce(windArea.transform.right * windForce);
            }
        }

        #endregion

        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }
    }
}