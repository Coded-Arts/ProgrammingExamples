using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileHolder;

    private GameObject[] projectiles;
    private WeaponController weaponController;
    private int lastRoundRobinFireIndex = -1;
    private Weapon weapon;
    private int lastFiredProjectileIndex = -1;

    private void Awake() {
        weaponController = GetComponent<WeaponController>();
    }

    private void Start() {
        weapon = weaponController.Weapon;
        projectiles = new GameObject[weapon.MagazineSize];
        CreatePooledProjectiles(projectilePrefab, weapon.MagazineSize);
    }

    private void CreatePooledProjectiles(GameObject _projectile, int numberOfProjectiles) {
        Transform _projectileTransform;
        if (!projectileHolder) {
            projectileHolder = new GameObject(string.Concat(gameObject.name, " ", weapon.name, "-Projectiles")).transform;
        }

        for (int i = 0; i < numberOfProjectiles; i++) {
            GameObject proj = Instantiate(_projectile, projectileHolder);
            GameObjectWeaponDamager projectile = proj.GetComponent<GameObjectWeaponDamager>();

            _projectileTransform = proj.transform;
            projectiles[i] = proj;

            projectile.Damage = weapon.MaxDamage;
            projectile.Weapon = this.weapon;
            //projectile.impactPrefabs = this.impactPrefabs;

            proj.SetActive(false);
        }
    }

    private void EnableProjectilePlease (int index, Transform muzzlePosition) {
        Rigidbody rigidbody = projectiles[index].GetComponent<Rigidbody>();
        GameObjectWeaponDamager gameObjectWeaponDamager = projectiles[index].GetComponent<GameObjectWeaponDamager>();

        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = false;
        projectiles[index].transform.position = muzzlePosition.position;
        projectiles[index].transform.rotation = transform.rotation;
        projectiles[index].SetActive(true);
        gameObjectWeaponDamager.EnableProjectile();
        rigidbody.AddForce(muzzlePosition.forward * gameObjectWeaponDamager.MovementSpeed, ForceMode.Impulse);
        //rigidbody.AddForce(weaponController.WeaponParts[0].muzzle.forward * gameObjectWeaponDamager.MovementSpeed, ForceMode.Impulse);
    }

    public void EnableProjectile() {
        if (weapon.MagazineRemainingAmmo > -1 && !weapon.OutOfAmmo) {
            lastFiredProjectileIndex++;
            if (lastFiredProjectileIndex >= weapon.MagazineSize) {
                lastFiredProjectileIndex = 0;
            }

            switch (weaponController.NextFirePositionDeterminationMode) {
                case NextFirePositionDeterminationMode.Random:
                    int index = Random.Range(0, weaponController.WeaponParts.Length);
                    EnableProjectilePlease(lastFiredProjectileIndex, weaponController.WeaponParts[index].muzzle);
                    break;
                case NextFirePositionDeterminationMode.RoundRobin:
                    lastRoundRobinFireIndex++;
                    if (lastRoundRobinFireIndex >= weaponController.WeaponParts.Length) {
                        lastRoundRobinFireIndex = 0;
                    }
                    EnableProjectilePlease(lastFiredProjectileIndex, weaponController.WeaponParts[lastRoundRobinFireIndex].muzzle);
                    break;
                case NextFirePositionDeterminationMode.Simultaneous:
                    for (int i = 0; i < weaponController.WeaponParts.Length; i++) {
                        EnableProjectilePlease(lastFiredProjectileIndex, weaponController.WeaponParts[i].muzzle);
                    }
                    break;
                default:
                    EnableProjectilePlease(lastFiredProjectileIndex, weaponController.WeaponParts[0].muzzle);
                    break;
            }
            
        }
    }


}
