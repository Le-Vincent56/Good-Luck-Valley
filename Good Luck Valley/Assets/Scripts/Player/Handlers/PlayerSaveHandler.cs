using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.Handlers
{
    //public class PlayerSaveHandler : MonoBehaviour, IBind<PlayerSaveData>
    //{
    //    #region REFERENCES
    //    [SerializeField] private PlayerSaveData data;
    //    [SerializeField] private PlayerController player;
    //    #endregion

    //    #region PROPERTIES
    //    [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
    //    #endregion

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        // Update save data
    //        UpdateSaveData();
    //    }

    //    public void UpdateSaveData()
    //    {
    //        // Save transform data
    //        data.position = transform.position;
    //        data.isFacingRight = player.IsFacingRight;

    //        // Save states
    //        if (player.StateMachine.PreviousState != null) data.previousState = player.StateMachine.PreviousState.ToString().Substring(48);
    //        if (player.StateMachine.CurrentState != null) data.currentState = player.StateMachine.CurrentState.ToString().Substring(48);
    //    }

    //    public void LoadStateData()
    //    {
    //        // Set previous state data
    //        if (data.previousState == "IdleState") player.StateMachine.SetPreviousState(player.IdleState);
    //        if (data.previousState == "MoveState") player.StateMachine.SetPreviousState(player.MoveState);
    //        if (data.previousState == "JumpState") player.StateMachine.SetPreviousState(player.JumpState);
    //        if (data.previousState == "FallState") player.StateMachine.SetPreviousState(player.FallState);
    //        if (data.previousState == "FastFallState") player.StateMachine.SetPreviousState(player.FastFallState);
    //        if (data.previousState == "LandState") player.StateMachine.SetPreviousState(player.LandState);
    //        if (data.previousState == "BounceState") player.StateMachine.SetPreviousState(player.BounceState);
    //        if (data.previousState == "WallSlideState") player.StateMachine.SetPreviousState(player.WallSlideState);
    //        if (data.previousState == "FastWallSlideState") player.StateMachine.SetPreviousState(player.FastWallSlideState);
    //        if (data.previousState == "WallJumpState") player.StateMachine.SetPreviousState(player.WallJumpState);
    //        if (data.previousState == "SlopeState") player.StateMachine.SetPreviousState(player.SlopeState);

    //        // Initialize current state data
    //        if (data.currentState == "IdleState") player.StateMachine.Initialize(player.IdleState);
    //        if (data.currentState == "MoveState") player.StateMachine.Initialize(player.MoveState);
    //        if (data.currentState == "JumpState") player.StateMachine.Initialize(player.JumpState);
    //        if (data.currentState == "FallState") player.StateMachine.Initialize(player.FallState);
    //        if (data.currentState == "FastFallState") player.StateMachine.Initialize(player.FastFallState);
    //        if (data.currentState == "LandState") player.StateMachine.Initialize(player.LandState);
    //        if (data.currentState == "BounceState") player.StateMachine.Initialize(player.BounceState);
    //        if (data.currentState == "WallSlideState") player.StateMachine.Initialize(player.WallSlideState);
    //        if (data.currentState == "FastWallSlideState") player.StateMachine.Initialize(player.FastWallSlideState);
    //        if (data.currentState == "WallJumpState") player.StateMachine.Initialize(player.WallJumpState);
    //        if (data.currentState == "SlopeState") player.StateMachine.Initialize(player.SlopeState);
    //    }

    //    public void Bind(PlayerSaveData data)
    //    {
    //        this.data = data;
    //        this.data.ID = ID;

    //        // Set transform data
    //        transform.position = data.position;
    //        player.CheckDirectionToFace(data.isFacingRight);

    //        // Load State Data
    //        LoadStateData();

    //        // Save
    //        SaveLoadSystem.Instance.SaveGame();
    //    }
    //}
}