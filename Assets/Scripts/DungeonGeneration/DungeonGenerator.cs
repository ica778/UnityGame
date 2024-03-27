using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject entranceRoom;

    private Stack<RoomEntrance> stack = new Stack<RoomEntrance>();

    private int maxRooms = 15;
    private int currentRoomCount = 0;


    private void Start () {
        GameObject entrance = Instantiate(entranceRoom, transform.position, Quaternion.identity);
        Room entranceRoomScript = entrance.GetComponent<Room>();
        foreach (RoomEntrance i in entranceRoomScript.GetRoomEntrances()) {
            stack.Push(i);
        }
        
        while (currentRoomCount < maxRooms && stack.Count > 0) {
            RoomEntrance currentRoomEntrance = stack.Pop();
            if (Random.Range(0, 10) < 5) {
                continue;
            }

            GameObject newRoomObject = Instantiate(
                rooms[Random.Range(0, rooms.Length)], 
                currentRoomEntrance.transform.position + (currentRoomEntrance.transform.forward * 5f),
                currentRoomEntrance.transform.rotation
                );
            Room newRoom = newRoomObject.GetComponent<Room>();
            currentRoomEntrance.GetParentRoom().GetDungeonValidator().DisableColliders();
            if (!newRoom.CheckIfRoomPlacementValid()) {
                Destroy(newRoomObject);
                currentRoomEntrance.GetParentRoom().GetDungeonValidator().EnableColliders();
                continue;
            }
            currentRoomEntrance.GetParentRoom().GetDungeonValidator().EnableColliders();

            currentRoomCount++;
            currentRoomEntrance.OpenEntrance();
            foreach (RoomEntrance i in newRoom.GetRoomEntrances()) {
                stack.Push(i);
            }
        }
    }
}