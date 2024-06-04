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
#endregion

#region Enemy Event
public class LoseEvent : SDD.Events.Event
{
}
#endregion

#region EnemySpawn Event
public class AllEnemyHaveSpawnEvent : SDD.Events.Event
{
}
public class DestroyAllEnemiesEvent : SDD.Events.Event
{
}

#endregion