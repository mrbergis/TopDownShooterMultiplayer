﻿using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public enum Team
{
    Blue,
    Red
}

public class PlayerLogic : NetworkBehaviour
{
    CharacterController _characterController;

    float _horizontalInput;
    float _verticalInput;
    
    Vector3 _horizontalMovement;
    Vector3 _verticalMovement;
    
    const float MOVEMENT_SPEED = 5.0f;
    
    Animator _animator;
    
    const int MAX_HEALTH = 100;
    int _health = MAX_HEALTH;
    bool _isDead = false;
    
    [SyncVar]
    Team _team = Team.Blue;
    
    [SerializeField]
    Material blueMaterial;

    [SerializeField]
    Material redMaterial;

    [SerializeField]
    SkinnedMeshRenderer bodyRenderer;

    [SerializeField]
    SkinnedMeshRenderer headRenderer;
    
    NetworkAnimator _networkAnimator;
    
    [SerializeField]
    Light playerIndicatorLight;
    
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        
        _networkAnimator = GetComponent<NetworkAnimator>();
        if(_networkAnimator)
        {
            _networkAnimator.SetParameterAutoSend(0, true);
            _networkAnimator.SetParameterAutoSend(1, true);
        }
        
        SetHealthText();
        
        if(!isLocalPlayer)
        {
            playerIndicatorLight.enabled = false;
        }
    }

    public override void OnStartClient()
    {
        SetColor(_team);
    }
    
    private void SetHealthText()
    {
        if (isLocalPlayer)
        {
            UIManager.Instance.SetHealthText(_health);
        }
    }
    
    public void SetTeam(Team team)
    {
        RpcSetTeam(team);
    }
    
    [ClientRpc]
    void RpcSetTeam(Team team)
    {
        _team = team;
        SetColor(_team);
    }
    
    void SetColor(Team team)
    {
        if (_team == Team.Blue)
        {
            SetMaterial(blueMaterial);
        }
        else if (_team == Team.Red)
        {
            SetMaterial(redMaterial);
        }
    }
    
    void SetMaterial(Material material)
    {
        // Body
        Material[] mats = bodyRenderer.materials;
        mats[0] = material;
        bodyRenderer.materials = mats;

        // Head
        mats = headRenderer.materials;
        mats[0] = material;
        headRenderer.materials = mats;
    }
    
    void Update()
    {
        if (_isDead || !isLocalPlayer)
        {
            return;
        }
        
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        RotateTowardsMouseCursor();
        UpdateMovementAnimation();
    }

    private void FixedUpdate()
    {
        if (_isDead || !isLocalPlayer)
        {
            return;
        }
        
        _horizontalMovement = Vector3.right * _horizontalInput * MOVEMENT_SPEED * Time.deltaTime;
        _verticalMovement = Vector3.forward * _verticalInput * MOVEMENT_SPEED * Time.deltaTime;

        if(_characterController)
        {
            _characterController.Move(_horizontalMovement + _verticalMovement);
        }
    }
    
    private  void RotateTowardsMouseCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = mousePos - objectPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90.0f, Vector3.up);
    }
    
    void UpdateMovementAnimation()
    {
        Vector3 normalizedVector = Vector3.Normalize(Vector3.forward 
            * _verticalInput + Vector3.right 
            * _horizontalInput);
        float dotProduct = Vector3.Dot(transform.forward, normalizedVector);
        float dotProductStrafe = Vector3.Dot(transform.right, normalizedVector);

        if(_animator)
        {
            _animator.SetFloat("VerticalInput", dotProduct);
            _animator.SetFloat("HorizontalInput", dotProductStrafe);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if(!isServer)
        {
            return;
        }

        _health -= damage;
        _health = Mathf.Clamp(_health, 0, 100);

        RpcSetHealth(_health);

        if (_health <= 0)
        {
            RpcDie();
        }
    }
    
    [ClientRpc]
    void RpcSetHealth(int health)
    {
        _health = health;

        SetHealthText();
    }

    [ClientRpc]
    void RpcDie()
    {
        if(_animator)
        {
            _animator.SetTrigger("Death");
        }

        _isDead = true;
    }
    
    public bool IsDead()
    {
        return _isDead;
    }
    
    public bool IsLocalPlayer()
    {
        return isLocalPlayer;
    }
}
