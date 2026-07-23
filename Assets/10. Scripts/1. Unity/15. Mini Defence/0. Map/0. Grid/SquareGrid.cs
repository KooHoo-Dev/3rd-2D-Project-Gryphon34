
using UnityEngine;

namespace Study.MiniDefence
{
    // 사각 타일 그리드를 직접 구현한 자료구조 (교육용임)

    // 2차원배열을 이용해서 격자를 만들고 "월드 좌표"와 "타일좌표를"
    // 변환할 수 있게 해주는 클래스
    
    // PS : 본인의 프로젝트 맞게 자료구조를 직접 제작해서 사용하는 경우가
    //     매우 빈번하다. 특히 게임

    public class SquareGrid
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        
        public Vector2 Origin { get; }
        
        private Tile[,] tiles;
        
        public  SquareGrid(int width, int height, 
            float cellSize, Vector2 origin)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            Origin = origin;
            
            tiles = new Tile[width, height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[x, y] = new Tile(TileKind.Buildable);
                }
            } 
        }
        
        // 이런 자료구조 클래스를 만들때는 아래의 기능들을 함께 고려해야한다
        // - 외부 객체가 특정 데이터에 어떻게 접근하게 할것인가?
        // - 외부 객체가 요청할 내용은 어떤것인가? 그리고 어떻게 처리할 것인가?
        
        // 1. 특정 좌표가 그리드 안에 있는지 조회하는 기능 (유효성 검사)
        //  : 7,7이 유효한 좌표이니?
        public bool InBounds(int x, int y)
        {
            // x가 0보다 크로 Width 작니? (배열이니까 -1 해서 검사해야겠죠?)
            bool inBoundX = (0 <= x) && (x < Width);
            bool inBoundY = (0 <= y) && (y < Height);
            
            return (inBoundX && inBoundY);
        }

        // Vector2Int?
        // : x,y 두개로 구성된 int타입 vector 자료형.
        // Vector2는 float x, float y로 구성되어있지만
        // Vector2Int는 int x, int y로 구성되어있다.
        // 아래 함수는 함수 오버로딩을 통해서 유틸성을 더해준다
        public bool InBounds(Vector2Int point)
        {
            return InBounds(point.x, point.y);
        }
        
        // 2. 특정 좌표의 Tile 데이터를 반환하는 기능
        //  : 3,4 좌표의 타일을 반환해줘
        // Tip : Null을 반환할 수 있는 public 함수의 경우, Null 반환 여지가
        //      있음을 함수명에 함께 표기해 줍니다.
        public Tile GetTileOrNull(int x, int y)
        {
            // 예외처리
            if (InBounds(x, y) == false) return null;
            return tiles[x, y];

            // 저는 아래표현이 더 좋다고 생각합니다 (개인적으로)
            //return InBounds(x, y) ? tiles[x, y] : null;
        }
        
        public Tile GetTileOrNull(Vector2Int point)
        {
            return GetTileOrNull(point.x, point.y);
        }
        
        
        // 3. 월드 좌표 기준 -> 타일 좌표로 변환하는 기능
        //  : 월드 좌표 기준 (50.34 , 10.21)가 타일좌표로는 어떻게 되니?
        // 원리
        // : 기준점(Origin)을 빼서 그리드 내부 좌표계(로컬 좌표)로 만들고,
        //  한칸 크기로 나눈 뒤에 "내림"한다.
        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            // 반환해야 할꺼는 매개변수로 주어진 월드 좌표계에 알맞은
            // 2차원 인덱스(타일 인덱스를) 반환하는 것
            
            int x = Mathf.FloorToInt((worldPosition.x - Origin.x) / CellSize);
            int y = Mathf.FloorToInt((worldPosition.y - Origin.y) / CellSize);
            
            // 마무리 처리
            x = Mathf.Clamp(x, 0, Width - 1);
            y = Mathf.Clamp(y, 0, Height - 1);
            return new Vector2Int(x, y);
        }
        
        // 4. 타일좌표 -> 월드 좌표로 변현화는 기능
        //  : 타일좌표 (2,2)의 타일 중심점이 월드 좌표로는 어떻게 되니?
        public Vector3 CellToWorld(Vector2Int cellIndex)
        {
            // 0.5f = 월드 좌표 중앙 보정값
            float x = Origin.x + (cellIndex.x + 0.5f) * CellSize;
            //float x = Origin.x + (cellIndex.x * CellSize) + (0.5f * CellSize);
            float y = Origin.y + (cellIndex.y + 0.5f) * CellSize;
            //float x = Origin.x + (cellIndex.x * CellSize) + (0.5f * CellSize);
            
            return new Vector3(x, y, 0);
        }
        
    }
}