using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Shooter
{
    public class ShooterController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] internal Transform projectilesEmpty;
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
        }

        internal virtual void Update()
        {
            //If max projectiles, exit
            if (currentProjectiles >= maxProjectilesInScreen) return;
            
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
            SetProjectile(projGO.transform);
            ProjectileController proj = projGO.GetComponent<ProjectileController>();
            
            //If unsuccesful, exit
            if (proj == null) return;

            //Link Actions
            proj.Destroyed += OnProjectileDestroyed;
            proj.SetSelfDestroyTimer(projectileSelfDestroyTime);
            //proj.GetComponent<EnemyTarget>().SetValues(player);

            //Increase Counter
            currentProjectiles++;
        }

        internal virtual void SetProjectile(Transform proj)
        {
            proj.localPosition = Vector3.zero;
            proj.forward = projectilesEmpty.forward;
        }

        //Event Receivers
        void OnProjectileDestroyed()
        {
            currentProjectiles--;
            if (currentProjectiles < 0) currentProjectiles = 0;
        }
    }
}