using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
///   <para> 继承NetworkManager </para>
/// </summary>
public class MyNetworkManager : NetworkManager {
    /// <summary>
    ///   <para> 连接成功后，在Server上回调 </para>
    /// </summary>
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("我是服务器，我完成了一次连接");
        
        // 添加
        NetworkResource.networkInfo.RpcAddPlayer();
        
        // 生成player对象
        GameObject playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerObject.transform.SetParent(transform);
        Player player = playerObject.GetComponent<Player>();
        
        // 初始化player相关
        // Debug.Log(conn.identity is null);
        player.Identity = conn.identity;
        NetworkResource.networkInfo.players.Add(player.id, player);
        Debug.Log("当前玩家数：" + NetworkResource.networkInfo.players.Count);
        
        // 推送
        NetworkResource.networkSubject.Notify(ModelModifyEvent.New_Client);
    }

    /// <summary>
    ///   <para> Server下线后回调 </para>
    /// </summary>
    public override void OnServerDisconnect(NetworkConnection conn) {
        Debug.Log("Server Disconnected");
        NetworkResource.networkInfo.players.Clear();
        // 房主回到开房界面
        // BUG: 一旦断连conn里面的东西就没了，得用其他数据判断本地是否为服务器；或者干脆让房主也退回到联机界面
        // if(conn.identity.isLocalPlayer)
        //     PanelManager.Get().NowPanel = PanelManager.Get().roomOwnerChooseMap;
        base.OnServerDisconnect(conn);
    }

    /// <summary>
    ///   <para> Client下线后回调 </para>
    /// </summary>
    public override void OnClientDisconnect(NetworkConnection conn) {
        Debug.Log("Client Disconnected");
        NetworkIdentity identity=conn.identity;
        uint playerId = 0; // 根据NetworkIdentity获取player的唯一id
        NetworkResource.networkInfo.players.Remove(playerId);
        // 断连的房客回到联机界面
        if (identity.isLocalPlayer && !identity.isServer)
            PanelManager.Get().NowPanel = PanelManager.Get().joinRoom;
        base.OnClientDisconnect(conn);
    }

    /// <summary>
    ///   <para> 连接成功后，在Client上回调 </para>
    /// </summary>
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("我是客户端，我已连接");
        NetworkResource.networkSubject.Notify(ModelModifyEvent.Client_Success);
        
        // 添加
        NetworkResource.networkInfo.RpcAddPlayer();
    }

    /// <summary>
    ///   <para> Host建立时回调 </para>
    /// </summary>
    public override void OnStartHost()
    {
        Debug.Log("启动Host");
        NetworkResource.networkSubject.Notify(ModelModifyEvent.Server_Success);
    }
}