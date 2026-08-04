using System;
using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniDefence._3._Combat
{
    // 살아 있는 적들의 "명부"
    // : 유닛은 "내 사거리 안에 누가 있지?"를 계속 물어봐야 한다
    //  콜라이더로 물리 검사를 할 수도 있지만, 여기서는 이런 명부를
    //  이용해서 적을 찾아내는 방법을 사용합니다.
    //  (콜라이더 쓰는것도 추천 드려요)
    // 적이 태어날 때(OnEnable) 이름을 올리고, 사라질 떄(OnDisable)
    // 이름을 뺀다
    public static class EnemyRegistry
    {
        private static readonly List<Enemy> alives = new();
        
        // IReadOnlyCollection일 경우에는 외부에서 해당
        // 콜렉션을 수정할수 없습니다.
        public static IReadOnlyCollection<Enemy> Alive => alives;
        public static int AliveCount => alives.Count;

        public static event Action<Enemy> Registered;
        public static event Action<Enemy> UnRegistered;

        public static void Register(Enemy enemy)
        {
            alives.Add(enemy);
            Registered?.Invoke(enemy);
        }

        public static void Unregister(Enemy enemy)
        {
            alives.Remove(enemy);
            UnRegistered?.Invoke(enemy);
        }

        // 판을 다시 시작할때 한방에 비워버릴려고 맹글어 둔겁니다.
        public static void Clear()
        {
            alives.Clear();
        }

        // center에서 radius 안에 있는 살아 있는 적들을 result에 담는다
        // 호출할 때마다 새 List를 만들면 쓰레기가 계속 쌓이므로
        // Unit쪽에서 쓰던 리스트를 넘겨받아서 그 안을 채우는 방식으로 씁니다.
        
        // ref ?
        // : 매개변수의 형태가 레퍼런스 타입으로 넘어갑니다
        //  value일경우에도 값복사를 하지않고 해당 value의 메모리 주소(레퍼런스)가
        //  전달됩니다. 특이사항으로 null을 허용하지 않습니다.
        public static void FindInRange(Vector3 center, float radius, ref List<Enemy> result)
        {
            result.Clear();

            // 거리 비교는 제곱끼리 하면 계산을 아낄수있습니다
            float sqrRadius = radius * radius; // 범위값을 구해놓고 
            
            // 반복문을 돌아서 개체를 검사하고 추가합니다.
            for (int i = 0; i < alives.Count; i++)
            {
                Enemy enemy = alives[i];
                if(enemy.IsDead) continue; // 죽은 적은 검색 대상이 아니다
                if(IsIn(enemy)) result.Add(enemy);
            }
            bool IsIn(Enemy e) 
                => (e.transform.position - center).sqrMagnitude <= sqrRadius;
        }
    }
    
    // 배치되어 있는 아군 유닛들의 "명부"
    // 지휘관이 "내 주변 아군"을 찾거나,
    // 반대로 유닛이 "나를 강화해 주는 지휘관"을 찾을 때 사용한다
    public static class UnitRegistry
    {
        private static readonly List<Unit> all = new();

        public static IReadOnlyList<Unit> All => all;
        public static int Count => all.Count;

        // 프리뷰 개체일때랑, 유닛 자체가 초기화가 안되어 있을때 에러남
        // (Def가 Null일 경우 에러가 납니다) 
        public static event Action Updated;

        public static void Register(Unit unit)
        {
            all.Add(unit);
            Updated?.Invoke();
        }

        public static void Unregister(Unit unit)
        {
            all.Remove(unit);
            Updated?.Invoke();
        }

        // 판을 다시 시작할 때 명부를 비운다. (static 이라 씬을 넘어 남는다)
        public static void Clear()
        {
            all.Clear();
        }
    }
}