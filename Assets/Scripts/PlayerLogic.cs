using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    CharacterController _characterController;

    float _horizontalInput;
    float _verticalInput;
    
    Vector3 _horizontalMovement;
    Vector3 _verticalMovement;
    
    const float MOVEMENT_SPEED = 5.0f;
    
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        _horizontalMovement = Vector3.right * _horizontalInput * MOVEMENT_SPEED * Time.deltaTime;
        _verticalMovement = Vector3.forward * _verticalInput * MOVEMENT_SPEED * Time.deltaTime;

        if(_characterController)
        {
            _characterController.Move(_horizontalMovement + _verticalMovement);
        }
    }
}
