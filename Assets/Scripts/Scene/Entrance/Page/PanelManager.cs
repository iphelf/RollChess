using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///   <para> 单人玩家选地图 </para>
/// </summary>
public class PanelManager : MonoBehaviour {
    // 单例
    static PanelManager menuManager;

    // 当前显示的页面
    private Image nowPanel;

    // 主菜单
    public Image mainMenu;
    // 单人页面
    public Image single;
    // 地图编辑页面
    public Image mapEdit;
    // 创建和加入房间页面
    public Image joinRoom;
    // 房主选地图页面
    public Image roomOwnerChooseMap;
    // 多人玩家选角色页面
    public Image room;

    // 返回按钮
    [SerializeField] Button backButton;

    /// <summary>
    ///    <para> 页面变化的推送 </para>
    /// </summary>
    static public Subject panelChangeSubject;

    // 初始化，页面最开始是主菜单
    void Start() {
        NowPanel = mainMenu;
        menuManager = this;
        panelChangeSubject = new Subject();
    }

    /// <summary>
    ///   <para> 获取单例 </para>
    /// </summary>
    public static PanelManager Get() {
        return menuManager;
    }

    /// <summary>
    ///   <para> 点击返回按钮 </para>
    /// </summary>
    public void Back() {
        // 单人页 / 地图编辑页 / 联机页 -> 主页
        if(nowPanel == single || nowPanel == mapEdit || nowPanel == joinRoom)
            NowPanel = mainMenu;
        // 联机房主页 -> 联机页
        if (nowPanel == roomOwnerChooseMap)
            NowPanel = joinRoom;
        // 联机房间页 -> 联机页
        if (nowPanel == room) {
            NowPanel = joinRoom;
            if (NetworkResource.networkInfo.identity.isServer)
                NetworkResource.networkSubject.Notify(ModelModifyEvent.Server_Off);
            else
                NetworkResource.networkSubject.Notify(ModelModifyEvent.Client_Off);
        }
    }

    /// <summary>
    ///   <para> 当前显示的页面 </para>
    /// </summary>
    public Image NowPanel {
        get {return nowPanel;}
        set {
            if (nowPanel == value)
                return;
            if(!(nowPanel is null))
                nowPanel.gameObject.SetActive(false);
            nowPanel = value;
            nowPanel.gameObject.SetActive(true);

            // 主菜单不显示按钮，其他页面显示
            backButton.gameObject.SetActive(nowPanel != mainMenu);
        }
    }
}