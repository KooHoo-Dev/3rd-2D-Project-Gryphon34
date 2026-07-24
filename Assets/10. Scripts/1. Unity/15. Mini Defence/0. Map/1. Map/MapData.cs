using System;
using System.Collections.Generic;
using UnityEditor;
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

#if UNITY_EDITOR
        // 씬에 특정 정보를 렌더링 하는 디버깅용 이벤트 함수입니다.
        // 개발하면서 중요한 정보들을 시각화하는데 사용됩니다.
        // 이번에는 RoutePoints를 Scene화면에 렌더링 하는 용도로 사용합니다
        private void OnDrawGizmos()
        {
            List<Vector3> worldRoutePoints = GetWorldRoutePoints();
            
            // Gizmos는 Unity Scene에서 특정 객체를 그리는데 사용하는 클래스입니다
            // Color등을 설정하고 Line(선), Sphere(구체), Cube(박스) 등을 렌더링하는
            // 함수들을 갖고 있습니다.
            
            Gizmos.color = Color.cyan;

            for (int i = 0; i < worldRoutePoints.Count; i++)
            {
                // .DrawSphere(위치좌표, 반지름)
                Gizmos.DrawSphere(worldRoutePoints[i], 0.08f);
                //다음 단계 녀석이 있으면
                if(i+1 < worldRoutePoints.Count)
                    // 선을 그려준다
                    Gizmos.DrawLine(worldRoutePoints[i], worldRoutePoints[i+1]);
            }
            
            // 스폰타일과 목표타일은 따로 처리해준다
            Gizmos.color = Color.green; // 스폰 타일 색깔
            // Wire계열의 함수들은 내부가 비어있습니다.
            Gizmos.DrawWireSphere(worldRoutePoints[0], 0.2f);
            
            Gizmos.color = Color.red; // 목표 타일 색깔
            // Wire계열의 함수들은 내부가 비어있습니다.
            // ^1 : 배열의 마지막 요소
            // ^2 : 배열의 마지막 전 요소
            Gizmos.DrawWireSphere(worldRoutePoints[^1], 0.2f);
        }

        [Header("타일 종류별 스프라이트 (종류 하나에 그림 하나)")]
        public Sprite BuilableSprite;
        public Sprite PathSprite;
        public Sprite SpawnSprite;
        public Sprite GoalSprite;
        public Sprite BlockedSprite;

        public Sprite SpriteOf(TileKind kind)
        {
            return kind switch
            {
                TileKind.Buildable => BuilableSprite,
                TileKind.Path => PathSprite,
                TileKind.Spawn => SpawnSprite,
                TileKind.Goal => GoalSprite,
                _ => BlockedSprite,
            };
        }
        
        // MapData를 토대로 새로운 스프라이트를 덮어 씌우는 기능
        [ContextMenu("Apply Tile Sprites")]
        public void ApplyTileSprites()
        {
            for (int i = 0; i < TileKinds.Count; i++)
            {
                // Transform.GetChild(int index)
                // : index 번쨰 자식 Transform을 반환합니다.
                SpriteRenderer sr = transform.GetChild(i).GetComponent<SpriteRenderer>();
                
                sr.sprite = SpriteOf(TileKinds[i]);
                sr.color = Color.white;
            }
        }
#endif
        

    }
}