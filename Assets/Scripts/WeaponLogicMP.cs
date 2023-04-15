using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponLogicMP : NetworkBehaviour
{
    WeaponLogic _weaponLogic;
    
    void Start()
    {
        _weaponLogic = GetComponentInChildren<WeaponLogic>();
    }
    
    void Update()
    {
        
    }

    [Command]
    public void CmdReload()
    {
        RpcReload();
    }

    [ClientRpc]
    void RpcReload()
    {
        _weaponLogic.Reload();
    }

    [Command]
    public void CmdShoot()
    {
        if(!isServer || !_weaponLogic)
        {
            return;
        }

        Transform bulletSpawn = _weaponLogic.GetBulletSpawn();

        Ray ray = new Ray(bulletSpawn.position, bulletSpawn.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            Debug.Log("Hit object: " + rayHit.collider.name);
            Debug.Log("Hit Pos: " + rayHit.point);

            // Deal damage to other player
            if (rayHit.collider.tag == "Player")
            {
                PlayerLogic playerLogic = rayHit.collider.GetComponent<PlayerLogic>();
                if (playerLogic)
                {
                    playerLogic.TakeDamage(30);
                }
            }

            RpcShoot(rayHit.point, Quaternion.FromToRotation(Vector3.up, rayHit.normal) * Quaternion.Euler(-90, 0, 0));
        }
    }

    [ClientRpc]
    void RpcShoot(Vector3 impactPosition, Quaternion impactRotation)
    {
        if(_weaponLogic)
        {
            _weaponLogic.ShootEffect(impactPosition, impactRotation);
        }
    }
}
