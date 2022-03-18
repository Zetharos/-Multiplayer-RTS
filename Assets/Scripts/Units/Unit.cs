using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private UnitMovement unitMovement;
    [SerializeField] private Targeter targeter;
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);

        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }

    [Server] private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);    
    }

    public override void OnStopClient() //OnStopAuthority is when you remove the authority directly from the server.
    {
        if (!hasAuthority) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }
    #endregion
}
