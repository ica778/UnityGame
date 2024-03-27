using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] private RoomEntrance[] roomEntrances;
    [SerializeField] private DungeonValidator dungeonValidator;

    private void Start() {
    }

    public RoomEntrance[] GetRoomEntrances() {
        return roomEntrances;
    }

    public RoomEntrance GetRoomEntrance(int index) {
        return roomEntrances[index];
    }

    public bool CheckIfRoomPlacementValid() {
        return !dungeonValidator.CheckIfCollidersOverlapWithOtherColliders();
    }

    public DungeonValidator GetDungeonValidator() {
        return dungeonValidator;
    }
}