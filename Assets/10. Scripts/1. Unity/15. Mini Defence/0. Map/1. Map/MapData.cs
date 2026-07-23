using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniDefence
{
    // 생성된 맵의 "루트"에 붙어서 그 맵을 온전히 설명하는 단 하나의 객체(컴포넌트)
    // MapBuilder에 의해 생성되며, 인게임에서 해당 맵 데이터를 확인해서
    // 게임을 진행하는데에 사용한다.
    
    public class MapData : MonoBehaviour
    {
        public int Width;
        public int Height;
        public float CellSize;
        
        public List<TileKind> TileKinds =  new List<TileKind>();
        public List<Vector3> RoutePoint = new List<Vector3>(); // 적 경로 좌표가 됩니다.


    }
}