using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

namespace MommyChicken
{
    public class MommyChicken : MonoBehaviour
    {
        public float speed = 4;
        
        private Vector3 _inputDirection = Vector3.zero;
        
        private CharacterController _characterController;
        private Transform _transform;
        private float _initY;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _transform = transform;
            _initY = _transform.position.y;
            if (InputManager.Instance != null) InputManager.Instance.PlayerMoveEvent.AddListener(OnPlayerMove);
        }

        private void Update()
        {
            // move!
            _characterController.Move(_inputDirection * (speed * Time.deltaTime));
            // physicBug, stay same y
            var position = _transform.position;
            position = new Vector3(position.x, _initY, position.z);
            _transform.position = position;
        }

        private void OnPlayerMove(Vector2 inputVec)
        {
            // move mommy chicken
            _inputDirection = new Vector3(inputVec.x, 0, inputVec.y);
        }
    }
}