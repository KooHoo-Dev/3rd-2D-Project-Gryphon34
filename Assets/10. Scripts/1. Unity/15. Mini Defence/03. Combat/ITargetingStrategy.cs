using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniDefence
{
    // [Strategy] 사거리 안 여러 적 중 "누구를 떄릴지" 고르는 알고리즘 객체
    
    // 조건문으로 분기하는게 아니라 기준(알고리즘)마다 클래스를 하나씩 만드는 것
    // 새 조준 방식을 추가 할떄는 기존 코드를 건드리지 않고
    // 새 클래스 하나만 만들면 된다
    public interface ITargetingStrategy
    {
        string Name { get; }
        Enemy Pick(IReadOnlyCollection<Enemy> candidates, Vector3 from);
    }
    
    public class FirstTargeting : ITargetingStrategy
    {
        public string Name { get; }
        public Enemy Pick(IReadOnlyCollection<Enemy> candidates, Vector3 from)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class LastTargeting : ITargetingStrategy
    {
        public string Name { get; }
        public Enemy Pick(IReadOnlyCollection<Enemy> candidates, Vector3 from)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class NearestTargeting : ITargetingStrategy
    {
        public string Name { get; }
        public Enemy Pick(IReadOnlyCollection<Enemy> candidates, Vector3 from)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class StrongestTargeting : ITargetingStrategy
    {
        public string Name { get; }
        public Enemy Pick(IReadOnlyCollection<Enemy> candidates, Vector3 from)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class WeakestTargeting : ITargetingStrategy
    {
        public string Name { get; }
        public Enemy Pick(IReadOnlyCollection<Enemy> candidates, Vector3 from)
        {
            throw new System.NotImplementedException();
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