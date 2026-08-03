using System.Collections.Generic;
using Study.Utilities;
using UnityEngine;

namespace Study.MiniDefence
{
    // [Strategy] 사거리 안 여러 적 중 "누구를 떄릴지" 고르는 알고리즘 객체
    
    // 조건문으로 분기하는게 아니라 기준(알고리즘)마다 클래스를 하나씩 만드는 것
    // 새 조준 방식을 추가 할떄는 기존 코드를 건드리지 않고
    // 새 클래스 하나만 만들면 된다
    
    // IReadOnluList<T>
    public interface ITargetingStrategy
    {
        string Name { get; }
        Enemy Pick(IReadOnlyList<Enemy> candidates, Vector3 from);
    }
    
    public class FirstTargeting : ITargetingStrategy
    {
        public string Name => "선두";
        public Enemy Pick(IReadOnlyList<Enemy> candidates, Vector3 from)
        {
            // 배열이나 리스트에서 최소값, 최대값 고르는 문제
            Enemy picked = null;
            float max = float.NegativeInfinity;
            
            for (int i = 0; i < candidates.Count; i++)
            {
                Enemy enemy = candidates[i];
                if (enemy.Progress > max)
                {
                    max = enemy.Progress;
                    picked = enemy;
                }
            }

            return picked;
        }
    }
    
    public class LastTargeting : ITargetingStrategy
    {
        public string Name => "후미";
        public Enemy Pick(IReadOnlyList<Enemy> candidates, Vector3 from)
        {
            Enemy picked = null;
            float min = float.PositiveInfinity;
            
            for (int i = 0; i < candidates.Count; i++)
            {
                Enemy enemy = candidates[i];
                if (enemy.Progress < min)
                {
                    min = enemy.Progress;
                    picked = enemy;
                }
            }
            return picked;
        }
    }
    
    public class NearestTargeting : ITargetingStrategy
    {
        public string Name => "최근접";
        public Enemy Pick(IReadOnlyList<Enemy> candidates, Vector3 from)
        {
            Enemy picked = null;
            float min = float.PositiveInfinity;

            for (int i = 0; i < candidates.Count; i++)
            {
                Enemy enemy =  candidates[i];
                // 우리는 정확한 거리가 아니라 단순 비교기 때문에
                // square 길이(sqrmagnitude)를 사용해서 비교합니다.
                float distance = (enemy.transform.position - from).sqrMagnitude;
                if (distance < min)
                {
                    min = distance;
                    picked = enemy;
                }
            }

            return picked;
        }
    }
    
    public class StrongestTargeting : ITargetingStrategy
    {
        public string Name => "최대체력";
        public Enemy Pick(IReadOnlyList<Enemy> candidates, Vector3 from)
        {
            Enemy picked = null;
            int max = int.MinValue;

            for (int i = 0; i < candidates.Count; i++)
            {
                Enemy enemy = candidates[i];
                if (enemy.CurrentHp > max)
                {
                    max = enemy.CurrentHp;
                    picked = enemy;
                }
            }
            
            return picked;
        }
    }
    
    public class WeakestTargeting : ITargetingStrategy
    {
        public string Name => "최소체력";
        public Enemy Pick(IReadOnlyList<Enemy> candidates, Vector3 from)
        {
            Enemy picked = null;
            int min = int.MaxValue;
            for (int i = 0; i < candidates.Count; i++)
            {
                Enemy enemy = candidates[i];
                if (enemy.CurrentHp < min)
                {
                    min = enemy.CurrentHp;
                    picked = enemy;
                }
            }

            return picked;
        }
    }

    
    public static class TargetingFactory
    {
        public static ITargetingStrategy Create(TargetingKind kind)
        {
            return kind switch
                {
                    TargetingKind.First => new FirstTargeting(),
                    TargetingKind.Last => new LastTargeting(),
                    TargetingKind.Nearest => new NearestTargeting(),
                    TargetingKind.Strongest => new StrongestTargeting(),
                    TargetingKind.Weakest => new WeakestTargeting(),
                    _ => new FirstTargeting(),
                };
        }
    }
}