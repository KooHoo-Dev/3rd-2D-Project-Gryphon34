using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniFarm
{
    // Farmer 계열의 코드를 테스트하기 위한 컴포넌트
    public class FarmerTester : MonoBehaviour
    {
        [Header("Settings")]
        public float moveSpeed = 3f;
        
        private FarmerController farmerController;

        private void Awake()
        {
            farmerController = GetComponent<FarmerController>();
        }

        private void Update()
        {
            HandleMove();
            HandleAction();
        }

        private void HandleMove()
        {
            Vector2 input = SimpleInput.GetMoveAxisRaw();
            Debug.Log(input);
            // 입력이 있니?
            bool walking = input.sqrMagnitude > 0;
            farmerController.Face(input);
            farmerController.SetWalking(walking);

            if (walking == false) return;
            Vector2 direction = farmerController.Facing;
            transform.Translate(((Vector3)direction) * moveSpeed * Time.deltaTime);
        }

        private void HandleAction()
        {
            if(SimpleInput.GetKeyDown(Key.Digit1))
                farmerController.Play(FarmerAction.PickUp);
            if(SimpleInput.GetKeyDown(Key.Digit2))
                farmerController.Play(FarmerAction.Throwing);
            if(SimpleInput.GetKeyDown(Key.Digit3))
                farmerController.Play(FarmerAction.Hoe);
            if(SimpleInput.GetKeyDown(Key.Digit4))
                farmerController.Play(FarmerAction.Watering);
        }
    }
}