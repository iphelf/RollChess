using System;
using UnityEngine;

public class RoomPage : MonoBehaviour
{
    private void Start() {
        NetworkResource.networkSubject.Attach(ModelModifyEvent.Server_Off, OnServerOff);
    }

    void OnServerOff() {
        NetworkResource.networkManager.StopHost();
    }
}