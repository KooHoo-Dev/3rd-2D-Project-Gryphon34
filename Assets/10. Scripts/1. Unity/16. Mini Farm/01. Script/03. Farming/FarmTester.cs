using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniFarm
{
    public class FarmTester : MonoBehaviour
    {
        public Farm farm;

        private int index = 0;
        
        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Q))
            {
                index--;
                index = Mathf.Max(index, 0);
                Debug.Log($"[FarmTester] index = {index}");
            }
            
            if (SimpleInput.GetKeyDown(Key.E))
            {
                index++;
                index = Mathf.Min(1, index);
                Debug.Log($"[FarmTester] index = {index}");
            }
            
            if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Left))
            {
                var mousePos = SimpleInput.GetMousePosition();
                var clickPosition = Camera.main.ScreenToWorldPoint(mousePos);
                HandleMethod(index, clickPosition);
            }
        }

        private void HandleMethod(int number, Vector3 clickPosition)
        {
            FarmResult result;
            switch (number)
            {
                case 0 : 
                    result = farm.Till(clickPosition); 
                    break;
                case 1 :
                    result = farm.Sow(clickPosition, 1);
                    break;
                default:
                    result = FarmResult.UnknownError;
                    break;
            }
            Debug.Log(FarmResultText.Of(result));
        }
    }
}