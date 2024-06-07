using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

#region GameManager Events
public class GameMenuEvent : SDD.Events.Event
{
}
public class GamePlayEvent : SDD.Events.Event
{
}
public class GamePauseEvent : SDD.Events.Event
{
}
public class GameResumeEvent : SDD.Events.Event
{
}
public class GameLoseEvent : SDD.Events.Event
{
}
public class GameWinEvent : SDD.Events.Event
{
}
public class UpdateScoreEvent : SDD.Events.Event
{
    public int score { get; set; }
}
#endregion

#region PlayerManager Events
public class KillAllEnemiesEvent : SDD.Events.Event
{
}
#endregion

#region LevelManager Events
public class FinishAllLevelEvent : SDD.Events.Event
{
}

public class LoadLevelEvent : SDD.Events.Event
{
    public Level level { get; set; }
}
#endregion

#region MenuManager Events
public class PlayButtonClickedEvent : SDD.Events.Event
{
}
public class ReplayButtonClickedEvent : SDD.Events.Event
{
}
public class QuitButtonClickedEvent : SDD.Events.Event
{
}
public class MenuButtonClickedEvent : SDD.Events.Event
{
}
#endregion

#region ScoreManager Events
#endregion

#region PauseManager Events
public class PauseEvent : SDD.Events.Event
{
}
public class ResumeEvent : SDD.Events.Event
{
}
#endregion

#region AudioManager Events
public class SoundMixAllEvent : SDD.Events.Event
{
    public float? eSFXVolume;
    public float? eGameplayVolume;
    public float? eMenuVolume;
}

public class SoundMixSoundEvent : SDD.Events.Event
{
    public string eNameClip; // Check audioTypes list in AudioManager
    public float eVolume;
}

public class PlaySoundEvent : SDD.Events.Event
{
    public string eNameClip;
    public bool eLoop;
    public bool eCanStack;
    public bool eDestroyWhenFinished = false;
    public float ePitch = 1;
    public float eVolumeMultiplier = 1;
}

public class StopSoundEvent : SDD.Events.Event
{
    public string eNameClip;
}
public class StopSoundAllEvent : SDD.Events.Event
{ }

public class StopSoundByTypeEvent : SDD.Events.Event
{
    public string eType; // Check audioTypes list in AudioManager
}

public class MuteAllSoundEvent : SDD.Events.Event
{
    public bool eMute;
}
#endregion

#region Enemy Event
public class LoseEvent : SDD.Events.Event
{
}
public class ScreamEvent : SDD.Events.Event
{
}
#endregion

#region EnemySpawn Event
public class AllEnemyDeadEvent : SDD.Events.Event
{
}
public class DestroyAllEnemiesEvent : SDD.Events.Event
{
}

#endregion

#region ScoreManager Events
public class UpdateScoresTextEvent : SDD.Events.Event
{
}
#endregion

#region Weapon events

public class ShootEvent : SDD.Events.Event
{
}

#endregion