using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EvtSystem;

#region Possible Future Dialogue System
//DIALOGUE SYSTEM STUFF FOR POSSIBLE FUTURE IMPLEMENTATION
/*
    public class ShowDialougeText : EvtSystem.Event
{
    public string text;
    public CharacterID id;
    public float duration;
}

 public struct ResponseData
{
    public string text;
    public int karmaScore;

    public UnityAction buttonAction;
}\

public class ShowResponses : EvtSystem.Event
{
    public ResponseData[] responses; 
}

 
 */
#endregion
public class PlayAudio : EvtSystem.Event
{
    public AudioClip clipToPlay;
    public bool isPriority;
}

public class DisableUI : EvtSystem.Event
{
    public GameObject priorityUI;
}

public class ReplaceUI : EvtSystem.Event
{
    public string replacementMessage;
    public Color  replacementColor;
}
public class PlayerInteract : EvtSystem.Event
{
    public Vector3 interactPosition;
    public Vector3 interactDirection;
    public float   interactDistance;
}

public class CursorMovement : EvtSystem.Event
{
    public bool  canMove;
    public float lookSpeed;
    public CursorLockMode lockMode;
}

public class FreezePlayerMovement : EvtSystem.Event
{
    public bool  canMove;
    public float moveSpeed;
}

public class StartTeamRespawn : EvtSystem.Event
{
    public float respawnTime;
    public List<GameObject> teamToRespawn;
    public PlayerScript.Team team;
}

public class ChangeBaseState : EvtSystem.Event
{
    public bool isBaseVulnerable;
    public Base thisBase;
    public BaseEffects thisBaseEffects;
    public PlayerScript.Team team;
}

public class PlayerDied : EvtSystem.Event
{
    public GameObject playerThatDied;
}

public class BaseDestroyed : EvtSystem.Event
{
    public Base thisBase;
    public BaseEffects baseEffects;
    public StatusEffectData statusEffect;
}

public class EndGame : EvtSystem.Event
{

}

public class TiedGame : EvtSystem.Event
{

}

public class ApplyStatusEffects : EvtSystem.Event
{
    public PlayerScript.Team team;
    public GameObject player;
    public StatusEffectData statusEffect;
}

public class WhoBrokeBase : EvtSystem.Event
{
    public PlayerScript.Team playerTeam;
    public GameObject whatBase;
}

public class DisplayUI : EvtSystem.Event
{
    public string textToDisplay;
    public string statusEffectName;
    public Color  colorOfText;
}

public class ShootSound : EvtSystem.Event
{
    public Vector3 position;
    public string GunName;
}

public class ReloadSound : EvtSystem.Event
{
    public Vector3 position;
    public string GunName;
}