using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utility;

namespace MommyChicken
{
    public class InputManager : Singleton<InputManager>
    {
        public UnityEvent<Vector2> PlayerMoveEvent;
        
        private PlayerControl _playerControl;

        protected override void Awake()
        {
            _playerControl = new PlayerControl();
        }

        private void OnEnable()
        {
            _playerControl.Enable();
        }

        private void OnDisable()
        {
            _playerControl.Disable();
        }

        private void Start()
        {
            _playerControl.Player.Move.started += OnMove;
            _playerControl.Player.Move.performed += OnMove;
            _playerControl.Player.Move.canceled += OnMove;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            PlayerMoveEvent.Invoke(ctx.ReadValue<Vector2>());
        }
    }
}