using System;
using System.Collections.Generic;
using System.IO;
using Jay.FileIO;   // JsonReader · JsonWriter (프로젝트 로컬, Newtonsoft 기반)
using Newtonsoft.Json;
using UnityEngine;
using JsonReader = Jay.FileIO.JsonReader;
using JsonWriter = Jay.FileIO.JsonWriter;

namespace Study.MiniFarm
{
    // [DTO] 맵 하나를 통째로 담는 JSON 파일의 모양.
    //
    // 지형과 "그 위에 놓인 것"을 나눠서 담는다는 것이 이 형식의 핵심이다.
    //   - Tiles   : 칸마다 하나씩. 빠짐없이 채워진다 (24 x 16 이면 384개)
    //   - Objects : 있는 자리에만. 대부분의 칸은 비어 있다
    
    [Serializable]
    public class MapFile
    {
        // 형식이 바뀌었을 때 옛 파일을 알아보기 위한 번호.
        // 지금은 쓰지 않지만, 나중에 열이 바뀌면 이 값으로 갈라 읽는다.
        public int Version = 1;

        public string Name = "Farm";

        public int Width;
        public int Height;
        public float CellSize = 1f;

        // 칸의 종류. 한 줄로 눕혀 담는다. (y 줄을 다 채우고 다음 y 로)
        // (x, y) 의 자리는 y * Width + x 다.
        public List<TileType> Tiles = new List<TileType>();

        // 칸 위에 놓인 것들. 있는 자리만 적는다.
        public List<MapObjectEntry> Objects = new List<MapObjectEntry>();

        #region 파일 입출력

        // 맵 파일들이 사는 곳. 표(Table)와 나눠 둔다. 지형은 밸런스 수치가 아니기 때문이다.
        private static string FolderPath
            => Path.Combine(Application.streamingAssetsPath, "Maps", "MiniFarm");

        private static string PathOf(string mapName) => Path.Combine(FolderPath, mapName + ".json");

        public void Save(string mapName)
        {
            JsonWriter.Save(this, PathOf(mapName));
            Debug.Log($"[MapFile] {mapName} 저장 — 칸 {Tiles.Count}개 · 물체 {Objects.Count}개");
        }

        public static MapFile LoadOrNull(string mapName)
        {
            string path = PathOf(mapName);

            if (File.Exists(path) == false)
            {
                Debug.LogError($"[MapFile] 파일이 없습니다 : {path}");
                return null;
            }

            MapFile file = 
                JsonReader.Load<MapFile>(path);

            // 아래 두줄은 파일 유효성 검사
            if (file == null) return null;
            if (file.IsValid() == false) return null;
            return file;
        }

        #endregion

        public bool IsValid()
        {
            if (Width <= 0 || Height <= 0)
            {
                Debug.LogError($"[MapFile] 맵 크기가 {Width} x {Height} 입니다. 둘 다 1 이상이어야 합니다.");
                return false;
            }

            if (Tiles.Count != Width * Height)
            {
                Debug.LogError($"[MapFile] 칸 수가 맞지 않습니다. {Width} x {Height} = {Width * Height} 이어야 하는데 {Tiles.Count} 개입니다.");
                return false;
            }

            return true;
        }
    }

    // [DTO] 칸 위에 놓인 것 하나.
    //
    // 무엇이든 담을 수 있게 일부러 헐겁게 만들었다.
    //   Kind  : Crop, Npc, Decor 처럼 무엇인지
    //   Id    : 그 종류 안에서 어느 것인지
    //   Value : 여분의 숫자 하나 (심어 둔 작물의 성장 단계 같은 것)
    [Serializable]
    public class MapObjectEntry
    {
        public string Kind = "";
        public int X;
        public int Y;
        public int Id;
        public int Value;

        // 계산해서 만드는 값이라 파일에 적지 않는다.
        // Newtonsoft 는 프로퍼티도 저장하므로 빼라고 알려 줘야 한다.
        // [JsonIgnore] 키워드가 그 역할을 함.
        [JsonIgnore]
        public Vector2Int Coord => new Vector2Int(X, Y);
    }
}
