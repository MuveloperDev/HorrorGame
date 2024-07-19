using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enum
{
	public enum EUICanvasLayer
	{
		None = 0,
		Static,
		Frequent,
		Popup,
		Max
	}

    public enum EResourceScope
    {
        None = 0,
        Global,
        Ingame,
        Outgame,
        Max
    }

	public enum EScenes
	{
        None = 0,
        TitleScene = 1,
		InGameScene,
        Max
    }

    public enum ESoundsType 
    { 
        None = 0,
        Store,
        Active,
        Inactive,
        Always,
    }

    public enum EAudioPlayType
    {
        OneShot = 0,
        Loop
    }

    public enum EInteractiveAnimationType
    {
        Active,
        Inactive,
    }

    public enum EStoragebleItemType
    {
        None = -1,
        Item,
        Max
    }

    // Animations
    public enum EMainPanel
    {
        None = -1,
        Start,
        Show,
        Hide,
        InstantIn
    }
}

