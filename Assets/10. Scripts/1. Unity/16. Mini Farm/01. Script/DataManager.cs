using System;
using System.Threading.Tasks;
using Jay;
using UnityEngine;

namespace Study.MiniFarm
{
    public class DataManager : SingletonBase<DataManager>
    {
        public TileTable Tiles { get; } = new();

        public bool IsReady { get; private set; }

        // 로딩 완료 통지. 이미 끝난 뒤에 구독하면 오지 않으므로 IsReady 를 함께 본다.
        public event Action OnReady;

        // 지금 돌고 있는(또는 이미 끝난) 로딩. 두 번째 호출부터는 이걸 그대로 돌려준다.
        private Task loading;

        public Task EnsureLoadedAsync()
        {
            if (loading == null) loading = LoadAsync();
            return loading;
        }

        private async Task LoadAsync()
        {
            await Tiles.LoadAsync();

            IsReady = true;
            OnReady?.Invoke();

            Debug.Log($"[DataManager] 로드 완료 — 타일 {Tiles.Count}종");
        }
    }
}
