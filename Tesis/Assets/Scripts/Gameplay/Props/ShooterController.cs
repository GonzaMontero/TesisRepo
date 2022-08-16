using UnityEngine;
using TimeDistortion.Gameplay.Handler;

namespace TimeDistortion.Gameplay.Props
{
    public class ShooterController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] Transform projectilesEmpty;
        [SerializeField] CameraHandler mainCam;
        [SerializeField] InputHandler player;
        [SerializeField] float timeBetweenProjectiles;
        [SerializeField] float projectileSelfDestroyTime = 15f;
        [SerializeField] int maxProjectilesInScreen;
        [Header("Runtime Values")]
        [SerializeField] float timer;
        [SerializeField] int currentProjectiles;

        //Unity Events
        private void Start()
        {
            if (projectilesEmpty == null)
            {
                projectilesEmpty = transform;
            }
            if (mainCam == null)
            {
                mainCam = GameObject.FindGameObjectWithTag("CameraHolder").GetComponent<CameraHandler>();
            }
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<InputHandler>(); ;
            }
        }

        private void Update()
        {
            if (timer >= timeBetweenProjectiles)
            {
                CreateProjectile();
                timer = 0;
            }

            timer += Time.deltaTime;
        }

        //Methods
        void CreateProjectile()
        {
            //If max projectiles, exit
            if (currentProjectiles >= maxProjectilesInScreen) return;

            //Create Projectile
            GameObject projGO = Instantiate(projectilePrefab, projectilesEmpty);
            projGO.transform.localPosition = Vector3.zero;
            projGO.transform.forward = projectilesEmpty.forward;
            ProjectileController proj = projGO.GetComponent<ProjectileController>();
            
            //If unsuccesful, exit
            if (proj == null) return;

            //Link Actions
            proj.Destroyed += OnProjectileDestroyed;
            proj.SetSelfDestroyTimer(projectileSelfDestroyTime);
            proj.GetComponent<EnemyTarget>().SetValues(player);

            //Increase Counter
            currentProjectiles++;
        }

        //Event Receivers
        void OnProjectileDestroyed()
        {
            currentProjectiles--;
            if (currentProjectiles < 0) currentProjectiles = 0;
        }
    }
}