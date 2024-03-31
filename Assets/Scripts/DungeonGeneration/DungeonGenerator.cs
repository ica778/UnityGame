using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject entranceRoom;

    private Queue<RoomConnectorHandler> queue = new Queue<RoomConnectorHandler>();

    private int maxRooms = 300;
    private int currentRoomCount = 0;


    private void Start () {
        GameObject entrance = Instantiate(entranceRoom, transform.position, Quaternion.identity);
        Room entranceRoomScript = entrance.GetComponent<Room>();
        foreach (RoomConnectorHandler i in entranceRoomScript.GetRoomConnectors()) {
            queue.Enqueue(i);
        }
        
        while (currentRoomCount < maxRooms && queue.Count > 0) {
            RoomConnectorHandler currentRoomEntrance = queue.Dequeue();

            GameObject newRoomObject = Instantiate(
                rooms[Random.Range(0, rooms.Length)], 
                //rooms[0],
                currentRoomEntrance.transform.position,
                currentRoomEntrance.transform.rotation
                );
            Room newRoom = newRoomObject.GetComponent<Room>();
            newRoom.CoupleRoomToCouplingPoint(currentRoomEntrance.GetDoorwayCollider().transform);

            currentRoomEntrance.GetParentRoom().GetDungeonValidator().DisableColliders();
            if (!newRoom.GetDungeonValidator().CheckIfValid()) {
                // NOTE: HAVE TO DISABLE OBJECT BECAUSE DESTROY APPEARS TO RUN AFTER START INSTEAD OF DURING
                newRoomObject.SetActive(false);
                Destroy(newRoomObject);
                currentRoomEntrance.GetParentRoom().GetDungeonValidator().EnableColliders();
                continue;
            }
            currentRoomEntrance.GetParentRoom().GetDungeonValidator().EnableColliders();

            currentRoomCount++;
            currentRoomEntrance.OpenEntrance();
            foreach (RoomConnectorHandler i in newRoom.GetRoomConnectors()) {
                if (Random.Range(0, 10) < 10) {
                    queue.Enqueue(i);
                }
            }
        }
    }
}