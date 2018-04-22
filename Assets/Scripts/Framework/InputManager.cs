using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;
using System;

public class InputManager : Singleton<InputManager>
{
    ///////////////////////////////////////////////////////////////////
    /// Constants
    ///////////////////////////////////////////////////////////////////
    public static readonly string Identifier = "InputManager";
    public static readonly int MaxPlayerCount = 4;
    public static readonly int PrimaryPlayerId = 0;

    ///////////////////////////////////////////////////////////////////
    /// Serialized Field Member Variables
    ///////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////
    /// Private Member Variables
    ///////////////////////////////////////////////////////////////////

    private List<Rewired.Player> m_RewiredPlayers;
    private List<Action<InputActionEventData>> m_InputDelegateCache;    //! Use this to ensure we clear all delegates if the input manager is shut down before they are removed from the player.

    ///////////////////////////////////////////////////////////////////
    /// Singleton MonoBehaviour Implementation
    ///////////////////////////////////////////////////////////////////

    public override void Awake()
    {
        m_RewiredPlayers = new List<Rewired.Player>(MaxPlayerCount);
        for (int i = 0; i < MaxPlayerCount; i++)
        {
            Rewired.Player p = ReInput.players.GetPlayer(i);
            if (p != null)
            {
                m_RewiredPlayers.Add(p);
            }
        }

        m_InputDelegateCache = new List<Action<InputActionEventData>>();

        Debug.AssertFormat(ValidateManager() != false, "{0} : Failed to validate, please ensure that all required components are set and not null.", InputManager.Identifier);
        base.Awake();
    }

    public override void OnDestroy()
    {
        // Handle the mouse input
        //m_RewiredPlayer.RemoveInputEventDelegate(OnMouseInput);

        // When we shut down, let's take care of all this junk.
        if (m_RewiredPlayers.Count > 0 && m_InputDelegateCache != null && m_InputDelegateCache.Count > 0)
        {
            for (int i = 0; i < m_InputDelegateCache.Count; ++i)
            {
                for (int j = 0; j < m_RewiredPlayers.Count; j++)
                {
                    m_RewiredPlayers[i].RemoveInputEventDelegate(m_InputDelegateCache[i]);
                }
            }
            m_InputDelegateCache.Clear();
        }
        else if (m_InputDelegateCache != null && m_InputDelegateCache.Count > 0)
        {
            m_InputDelegateCache.Clear();
        }

        base.OnDestroy();
    }

    /// <summary>
    /// Validate that any serialized fields are properly set.
    /// A Valid Manager should function properly.
    /// </summary>
    /// <returns></returns>
    protected override bool ValidateManager()
    {
        bool isValid = true;

        isValid = isValid && (m_RewiredPlayers.Count > 0);
        isValid = isValid && base.ValidateManager();

        return isValid;
    }

    ///////////////////////////////////////////////////////////////////
    /// InputManager Implementation
    ///////////////////////////////////////////////////////////////////


    /// <summary>
    /// Add an input event delegate to the player. Use Rewired to handle this.. so great!
    /// </summary>
    /// <param name="inputDelegate"></param>
    /// <param name="updateType"></param>
    public void AddInputEventDelegate(Action<InputActionEventData> inputDelegate, UpdateLoopType updateType)
    {
        Debug.Assert(m_RewiredPlayers.Count > 0 && m_InputDelegateCache != null, "No Rewired players found, cannot add input delegate!");

        if (m_InputDelegateCache != null)
        {
            for (int i = 0; i < m_RewiredPlayers.Count; i++)
            {
                m_RewiredPlayers[i].AddInputEventDelegate(inputDelegate, updateType);
            }

            m_InputDelegateCache.Add(inputDelegate);
        }
    }

    /// <summary>
    /// Remove an input event delegate from the player. Use Rewired to handle this.. even better!
    /// </summary>
    /// <param name="inputDelegate"></param>
    public void RemoveInputEventDelegate(Action<InputActionEventData> inputDelegate)
    {
        Debug.Assert(m_RewiredPlayers.Count > 0 && m_InputDelegateCache != null, "Rewired Player is null, or input cache is null cannot add input delegate!");

        if (m_InputDelegateCache != null)
        {
            for (int i = 0; i < m_RewiredPlayers.Count; i++)
            {
                m_RewiredPlayers[i].RemoveInputEventDelegate(inputDelegate);
            }

            bool didRemove = m_InputDelegateCache.Remove(inputDelegate);

            Debug.Assert(didRemove == true, "Attempted to remove delegate from the input cache but it was not found. This is odd, investigate.");
        }
    }

    /// <summary>
    /// Callback for Mouse Input
    /// </summary>
    /// <param name="data"></param>
    //private void OnMouseInput(InputActionEventData data)
    //{
    //    switch(data.actionId)
    //    {
    //        case RewiredConsts.Action.PRIMARY_ACTION:
    //            if(data.GetButtonDown())
    //            {
    //                Debug.Log("PRIMARY_ACTION Pressed (LMB)");

    //                RaycastHit hit;
    //                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //                if(Physics.Raycast(ray, out hit, MAX_MOUSE_RAYCAST_DISTANCE, m_MouseInteractionLayerMask))
    //                {
    //                    Debug.Log("Hit Something");
    //                    m_NavMeshDebugHelper.goal = hit.point;
    //                }
    //            }
    //            break;
    //        case RewiredConsts.Action.SECONDARY_ACTION:
    //            if (data.GetButtonDown())
    //            {
    //                Debug.Log("SECONDARY_ACTION Pressed (RMB)");
    //            }
    //            break;
    //    }
    //}
}