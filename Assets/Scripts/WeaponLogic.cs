using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponLogic : MonoBehaviour
{
    const int MAX_AMMO = 30;
    int _ammoCount = MAX_AMMO;

    const float SHOT_COOLDOWN = 0.15f;
    float _cooldown = 0.0f;

    Animator _animator;

    ParticleSystem _muzzleFlash;
    Light _muzzleFlashLight;
    const float MAX_LIGHT_TIME = 0.2f;
    float _lightTimer = 0.0f;

    [SerializeField]
    GameObject bulletImpactObj;

    bool _isReloading = false;

    AudioSource _audioSource;

    [SerializeField] 
    private TMP_Text ammoText;
    
    [SerializeField]
    AudioClip shootSound;

    [SerializeField]
    AudioClip emptyClipSound;

    [SerializeField]
    AudioClip reloadingSound;

    [SerializeField]
    Transform bulletSpawn;

    PlayerLogic _playerLogic;
    
    WeaponLogicMP _weaponLogicMP;
    
    void Start()
    {
        _animator = GetComponentInParent<Animator>();
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
        _muzzleFlashLight = GetComponentInChildren<Light>();

        _audioSource = GetComponent<AudioSource>();

        _playerLogic = GetComponentInParent<PlayerLogic>();
        
        _weaponLogicMP = GetComponentInParent<WeaponLogicMP>();

        SetAmmoText();
    }
    
    void Update()
    {
        if (_lightTimer > 0.0f)
        {
            _lightTimer -= Time.deltaTime;
        }
        else
        {
            _muzzleFlashLight.enabled = false;
        }

        if (_playerLogic && (_playerLogic.IsDead() || !_playerLogic.IsLocalPlayer()))
        {
            return;
        }
        
        if(_cooldown > 0.0f)
        {
            _cooldown -= Time.deltaTime;
        }
        else
        {
            if(Input.GetButton("Fire1") && !_isReloading)
            {
                if(_ammoCount > 0)
                {
                    Shoot();
                }
                else
                {
                    // Play empty clip sound
                    PlaySound(emptyClipSound);
                }

                _cooldown = SHOT_COOLDOWN;
            }
        }

        if(Input.GetButtonDown("Fire2") && _weaponLogicMP)
        {
            _weaponLogicMP.CmdReload();
        }
    }

    void Shoot()
    {
        --_ammoCount;
        SetAmmoText();
        
        if(_weaponLogicMP)
        {
            _weaponLogicMP.CmdShoot();
        }
    }

    public void ShootEffect(Vector3 impactPosition, Quaternion impactRotation)
    {
        if(bulletImpactObj)
        {
            GameObject.Instantiate(bulletImpactObj, impactPosition, impactRotation);
        }

        PlaySound(shootSound);

        if (_muzzleFlash)
        {
            _muzzleFlash.Play(true);
        }

        if (_muzzleFlashLight)
        {
            _muzzleFlashLight.enabled = true;
            _lightTimer = MAX_LIGHT_TIME;
        }

        if (_animator)
        {
            _animator.SetTrigger("Shoot");
        }
    }

    public void Reload()
    {
        _isReloading = true;

        if (_animator)
        {
            _animator.SetTrigger("Reload");
        }

        PlaySound(reloadingSound, 0.5f);
    }

    public bool IsReloading()
    {
        return _isReloading;
    }

    public void SetReloadingState(bool isReloading)
    {
        _isReloading = isReloading;

        if(!_isReloading)
        {
            _ammoCount = MAX_AMMO;
            SetAmmoText();
        }
    }

    void PlaySound(AudioClip sound, float volume = 1.0f)
    {
        if(_audioSource && sound)
        {
            _audioSource.volume = volume;
            _audioSource.PlayOneShot(sound);
        }
    }
    
    public Transform GetBulletSpawn()
    {
        return bulletSpawn;
    }
    
    void SetAmmoText()
    {
        if (ammoText)
        {
            ammoText.text = "Ammo: " + _ammoCount;
        }
    }
}
