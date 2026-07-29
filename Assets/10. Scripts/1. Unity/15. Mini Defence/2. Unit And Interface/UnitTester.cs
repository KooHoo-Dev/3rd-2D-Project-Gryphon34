using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniDefence
{
    public class UnitTester : MonoBehaviour
    {
        private UnitTable unitTable;
        private Camera mainCam;
        private int selectedUnitId;
        
        private void Awake()
        {
            unitTable = new UnitTable();
            unitTable.Load();
            
            mainCam = Camera.main;
        }

        private void Update()
        {
            if(SimpleInput.GetKeyDown(Key.Q)) Select(0);
            if(SimpleInput.GetKeyDown(Key.W)) Select(1);
            if(SimpleInput.GetKeyDown(Key.E)) Select(2);

            if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Left))
                BuildAtMouse();
            if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Right))
                RemoveAtMouse();
        }

        private void Select(int unitId)
        {
            selectedUnitId = unitId;
            UnitDef def = unitTable.Get(unitId);
            Debug.Log($"[UnitTester] Selected Unit ID: {selectedUnitId}");
        }

        private void BuildAtMouse()
        {
            // 좌표 변환을 해주고
            Vector3 position = GetMouseWorldPosition();
            if(BoardMap.Instance.Contains(position) == false) return;
            // 해당 좌표의 타일을 가져온다
            Tile selectTile = BoardMap.Instance.GetTileAtWorldOrNull(position);
            // UnitDef를 준비한다
            UnitDef unitDef = unitTable.Get(selectedUnitId);
            PlacementSystem.Instance.Build(unitDef, selectTile.Coord);
        }

        private void RemoveAtMouse()
        {
            // 좌표 변환을 해주고
            Vector3 position = GetMouseWorldPosition();
            if(BoardMap.Instance.Contains(position) == false) return;
            // 해당 좌표의 타일을 가져온다
            Tile selectTile = BoardMap.Instance.GetTileAtWorldOrNull(position);
            PlacementSystem.Instance.Remove(selectTile.Coord);
        }

        private Vector3 GetMouseWorldPosition()
        {
            // z값 처리를 위해서 보정 함수를 만들어 준다
            Vector3 screenPosition = SimpleInput.GetMousePosition();
            screenPosition.z = 0;
            return mainCam.ScreenToWorldPoint(screenPosition);
        }
    }
}