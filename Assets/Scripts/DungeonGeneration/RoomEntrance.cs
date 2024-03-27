using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntrance : MonoBehaviour {
    [SerializeField] private GameObject entranceway;
    [SerializeField] private GameObject wall;
    private Room parentRoom;

    private void Awake() {
        parentRoom = GetComponentInParent<Room>();
    }

    public void OpenEntrance() {
        entranceway.SetActive(true);
        wall.SetActive(false);
    }

    public void CloseEntrance() {
        entranceway.SetActive(false);
        wall.SetActive(true);
    }

    public GameObject GetEntranceway() {
        return entranceway;
    }

    public GameObject GetWall() { 
        return wall; 
    }

    public Room GetParentRoom() {
        return parentRoom;
    }
}