using System;
using System.Collections;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
                index = Mathf.Min(3, index);
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
                    result = farm.Sow(clickPosition, Random.Range(1,5));
                    break;
                case 2 :
                    result = farm.Water(clickPosition);
                    break;
                case 3 :
                    result = farm.Harvest(clickPosition, out Crop crop);
                    break;
                default:
                    result = FarmResult.UnknownError;
                    break;
            }
            Debug.Log(FarmResultText.Of(result));
        }
    }
}