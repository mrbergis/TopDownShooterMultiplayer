using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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

    [SerializeField] 
    private TMP_Text healthText;
    
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        SetHealthText();
    }

    private void SetHealthText()
    {
        if (healthText)
        {
            healthText.text = "Health: " + _health;
        }
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
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, 100);
        
        SetHealthText();

        if (_health <= 0)
        {
            Die();
        }
    }
    
    void Die()
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
