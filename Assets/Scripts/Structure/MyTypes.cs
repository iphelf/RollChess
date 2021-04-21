﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;


namespace Structure_old {
    /// <summary>
    ///   <para>Tile的类型。</para>
    ///   <para>Tile有很多类型，并且这些类型可以分成几个组，于是为了构建与其定位相符的层次结构，这里采用了前缀命名法。</para>
    ///   <para>比如Token_Alien_Blue代表的是在Token层里的一种Alien块，且肤色为Blue。</para>
    ///   <para>名字中带有Begin、End的TileType是用来划定一组TileType的界限的，便于判别任一个TileType的分组、进行组内枚举等等。</para>
    ///   <para>TileType.End用来表示Null。</para>
    /// </summary>
    public enum TileType {
        Begin,
        Land_Begin,
        Land_Lawn_Green,
        Land_End,
        Special_Begin,
        Special_Portal,
        Special_DoubleStep,
        Special_BrokenBridge,
        Special_PulseOn,
        Special_PulseOff,
        Special_Stop,
        Special_Ritual,
        Special_RollAgain,
        Special_End,
        Token_Begin,
        Token_Alien_Red,
        Token_Alien_Blue,
        Token_Alien_Yellow,
        Token_Alien_Green,
        Token_Alien_Neutral,
        Token_End,
        End,
        Null = End
    };

    /// <summary>
    ///   <para> 角色对应颜色 </para>
    /// </summary>
    public enum PlayerColor {
        Red,
        Blue,
        Yellow,
        Green
    }

    /// <summary>
    ///   <para>Tilemap的种类。</para>
    ///   <para>这里的种类是按实际含义分得的。Land表示能象征地质的实体块，Special表示铺在实体块上的特殊地貌，Token表示行走在前两者上的棋子。</para>
    ///   <para>TilemapType.End用来表示Null。</para>
    /// </summary>
    public enum TilemapType {
        Begin,
        Land,
        Special,
        Token,
        End,
        Null = End
    };

    /// <summary>
    ///   <para> 角色的操控方式 </para>
    /// </summary>
    public enum PlayerChoices {
        Player, // 玩家控制
        Comuputer, // AI控制
        Banned // 此角色不参与游戏
    };

    /// <summary>
    ///   <para>提供了一系列处理TileType、TilemapType的函数，用于体现TileType内部的分组关系。</para>
    ///   <para>之所以取了MyTypes这个名字，是因为Types已经被Unity占用了。有待更合理的命名。</para>
    /// </summary>
    public static class MyTypes {
        // 用于记录各组的边界及数量
        private const int TileTypeHead = (int) TileType.Begin + 1;
        private const int TileTypeLandHead = (int) TileType.Land_Begin + 1;
        private const int TileTypeLandTail = (int) TileType.Land_End - 1;
        private const int NTileTypeLand = TileTypeLandTail - TileTypeLandHead + 1;
        private const int TileTypeSpecialHead = (int) TileType.Special_Begin + 1;
        private const int TileTypeSpecialTail = (int) TileType.Special_End - 1;
        private const int NTileTypeSpecial = TileTypeSpecialTail - TileTypeSpecialHead + 1;
        private const int TileTypeTokenHead = (int) TileType.Token_Begin + 1;
        private const int TileTypeTokenTail = (int) TileType.Token_End - 1;
        private const int NTileTypeToken = TileTypeTokenTail - TileTypeTokenHead + 1;
        private const int TileTypeTail = (int) TileType.End - 1;
        private const int NTileType = TileTypeTail - TileTypeHead + 1;
        private const int TilemapTypeHead = (int) TilemapType.Begin + 1;
        private const int TilemapTypeTail = (int) TilemapType.End - 1;
        private const int NTilemapType = TilemapTypeTail - TilemapTypeHead + 1;

        /// <summary>
        ///   <para>判断tileType在含义上属于哪一层Tilemap。</para>
        /// </summary>
        public static TilemapType GetType(TileType tileType) {
            var iTileType = (int) tileType;
            if (Inside(iTileType, TileTypeLandHead, TileTypeLandTail))
                return TilemapType.Land;
            else if (Inside(iTileType, TileTypeSpecialHead, TileTypeSpecialTail))
                return TilemapType.Special;
            else if (Inside(iTileType, TileTypeTokenHead, TileTypeTokenTail))
                return TilemapType.Token;
            else
                return TilemapType.End;
        }

        /// <summary>
        ///   <para>获取tileType在所属Tilemap下的所有TileType中所排到的位次。</para>
        ///   <para>位次最小是1。无效为0。</para>
        /// </summary>
        public static int GetId(TileType tileType) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (GetType(tileType)) {
                case TilemapType.Land:
                    return tileType - TileType.Land_Begin;
                case TilemapType.Special:
                    return tileType - TileType.Special_Begin;
                case TilemapType.Token:
                    return tileType - TileType.Token_Begin;
                default:
                    return 0;
            }
        }

        /// <summary>
        ///   <para>获取tilemapType在所有TilemapType中的位次。</para>
        ///   <para>位次最小是1。无效为0。</para>
        /// </summary>
        public static int GetId(TilemapType tilemapType) {
            if (Inside((int) tilemapType, TilemapTypeHead, TilemapTypeTail))
                return tilemapType - TilemapType.Begin;
            else
                return 0;
        }

        /// <summary>
        ///   <para>枚举tileType的取值，返回下一个TileType。</para>
        ///   <para>用于在同一Tilemap下TileType中进行切换。</para>
        ///   <para>如果tileType无效则不切换。</para>
        /// </summary>
        public static TileType Shift(TileType tileType, int offset = 1) {
            var iTileType = (int) tileType;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (GetType(tileType)) {
                case TilemapType.Land:
                    iTileType = TileTypeLandHead + Mod(iTileType - TileTypeLandHead + offset, NTileTypeLand);
                    break;
                case TilemapType.Special:
                    iTileType = TileTypeSpecialHead + Mod(iTileType - TileTypeSpecialHead + offset, NTileTypeSpecial);
                    break;
                case TilemapType.Token:
                    iTileType = TileTypeTokenHead + Mod(iTileType - TileTypeTokenHead + offset, NTileTypeToken);
                    break;
            }

            return (TileType) iTileType;
        }

        /// <summary>
        ///   <para>枚举tilemapType的取值，返回下一个TilemapType。</para>
        ///   <para>用于在有效的Tilemap中进行切换。</para>
        ///   <para>如果tilemapType无效则不切换。</para>
        /// </summary>
        public static TilemapType Shift(TilemapType tilemapType, int offset = 1) {
            var iTilemapType = (int) tilemapType;
            if (Inside(iTilemapType, TilemapTypeHead, TilemapTypeTail))
                iTilemapType = TilemapTypeHead + Mod(iTilemapType - TilemapTypeHead + offset, NTilemapType);
            return (TilemapType) iTilemapType;
        }

        /// <summary>
        ///   <para>增强版的模运算。</para>
        ///   <para>保证结果为非负数。</para>
        /// </summary>
        private static int Mod(int x, int m) {
            return (x % m + m) % m;
        }

        /// <summary>
        ///   <para>判断x是否在[l, r]闭区间内。</para>
        /// </summary>
        private static bool Inside(int x, int l, int r) {
            return l <= x && x <= r;
        }
    }

    /// <summary>
    ///   <para>提供了一系列与TileType、TilemapType相关的全局静态常量。</para>
    /// </summary>
    public static class Constants {

        /// <summary>
        ///   <para>获取给定TilemapType所对应的一组TileType。</para>
        /// </summary>
        public static Dictionary<TilemapType, List<TileType>> tileTypesOfTilemapType =
            new Dictionary<TilemapType, List<TileType>>() {
                {
                    TilemapType.Land,
                    new List<TileType>() {
                        TileType.Land_Lawn_Green
                    }
                }, {
                    TilemapType.Special,
                    new List<TileType>() {
                        TileType.Special_Portal,
                        TileType.Special_BrokenBridge,
                        TileType.Special_DoubleStep,
                        TileType.Special_PulseOn,
                        TileType.Special_PulseOff,
                        TileType.Special_Stop,
                        TileType.Special_Ritual,
                        TileType.Special_RollAgain
                    }
                }, {
                    TilemapType.Token,
                    new List<TileType>() {
                        TileType.Token_Alien_Red,
                        TileType.Token_Alien_Blue,
                        TileType.Token_Alien_Yellow,
                        TileType.Token_Alien_Green,
                        TileType.Token_Alien_Neutral,
                    }
                }
            };

        

        /// <summary>
        ///   <para>获取给定棋子块TileType的玩家编号。</para>
        /// </summary>
        public static Dictionary<TileType, int> playerIdOfTileType = new Dictionary<TileType, int>() {
            {TileType.Token_Alien_Red, 0},
            {TileType.Token_Alien_Blue, 1},
            {TileType.Token_Alien_Yellow, 2},
            {TileType.Token_Alien_Green, 3},
            {TileType.Token_Alien_Neutral, 4},
        };

        /// <summary>
        ///   <para>获取给定玩家编号所对应的棋子块TileType。</para>
        /// </summary>
        public static Dictionary<int, TileType> tileTypeOfPlayerId = new Dictionary<int, TileType>() {
            {0, TileType.Token_Alien_Red},
            {1, TileType.Token_Alien_Blue},
            {2, TileType.Token_Alien_Yellow},
            {3, TileType.Token_Alien_Green},
            {4, TileType.Token_Alien_Neutral},
        };

        /// <summary>
        ///   <para>玩家数量是4</para>
        /// </summary>
        public static int PLAYERNUMBER = 4;
    }
}