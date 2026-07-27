using System.Collections.Generic;
using UnityEngine;

namespace Study.MiniDefence
{
    // 생성된 맵의 "루트"에 붙어서 그 맵을 온전히 설명하는 단 하나의 객체(컴포넌트)
    // MapBuilder에 의해 생성되며, 인게임에서 해당 맵 데이터를 확인해서
    // 게임을 진행하는데에 사용한다.
    
    // 역할은 "정보 보관", 좌표 계산등은 "SquareGrid"에서 함
    // 데이터의 흐름
    // : MapBuilder => MapData => 인게임 객체들
    //  로 뿌려지는 구조입니다.
    
    public class MapData : MonoBehaviour
    {
        public int Width;
        public int Height;
        public float CellSize;
        
        public List<TileKind> TileKinds =  new List<TileKind>();
        public List<Vector3> RoutePoints = new List<Vector3>(); // 적 경로 좌표가 됩니다.

        public void ImportFrom
            (SquareGrid grid, List<Vector3> routePoints)
        {
            Height = grid.Height;
            Width = grid.Width;
            CellSize = grid.CellSize;
            TileKinds = grid.ToKindList();
            RoutePoints = routePoints;
        }

        // ------ 복원 ---------

        public SquareGrid ToGrid()
        {
            return SquareGrid.FromKindList(Width, Height, CellSize, transform.position, TileKinds);
        }

        public List<Vector3> GetWorldRoutePoints()
        {
            // 실제 인게임에서 MapData.transform 위치가 달라질 수 있기 때문에
            // 컴포넌트의 위치 기반으로 좌표를 바꿔주고 넘겨줘야함
            List<Vector3> returnList = new List<Vector3>();
            foreach (Vector3 point in RoutePoints)
            {
                // .TransformPoint ?
                // : 매개변수로 전달받은 로컬좌표(상대좌표)를
                //  지정된 transform 객체의 월드 좌표로 바꿉니다. 
                returnList.Add(transform.TransformPoint(point));
            }
            
            return returnList;
        }
    }
}