using System;
using System.Threading;
using System.Threading.Tasks;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.Async
{
    // 오래 걸리는 비동기에서 반드시 생기는 이슈와 그 처리
    // 상황
    // 1. 사용자가 중간에 그만두고 싶다.
    // 2. 대기 중에 이 오브젝트가 파괴된다.
    
    public class AsyncCancelExample : MonoBehaviour
    {
        public int stepCount = 10;
        public float stepSeconds = 0.5f;
        
        // 비동기 작업의 취소버튼 : CancellationTokenSource
        // (진짜로 줄여서 cts라고 함)
        private CancellationTokenSource cts;

        private void Update()
        {
            if(SimpleInput.GetKeyDown(Key.Digit1))StartWork();
        }

        private void OnDestroy()
        {
            
        }

        private void CancelWork()
        {
                   
        }
        
        private async void StartWork()
        {
            // Cancle
            await DoWork();
        }

        private async Task DoWork()
        {
            for (int i = 0; i < stepCount; i++)
            {
                await Task.Delay(Mathf.RoundToInt(stepSeconds*1000));
                Debug.Log($"Step: {i}/{stepCount}");
            }
        }
    }
}