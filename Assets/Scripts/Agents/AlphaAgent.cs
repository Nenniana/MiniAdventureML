using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using MiniAdventure;
using System;

public class AlphaAgent : Agent
{
    public Action<float> OnRewardUpdated;

    [SerializeField]
    private GameController gameController;

    protected override void OnEnable()
    {
        base.OnEnable();

        gameController.playerController.OnDeath += OnPlayerDeath;
        gameController.playerController.OnCoolingDown += OnPlayerCoolingDown;
        gameController.playerController.OnWarmingUp += OnPlayerWarmingUp;
        gameController.playerController.inventory.OnAxeAdded += OnAxeAdded;
        gameController.playerController.inventory.OnWoodAdded += OnWoodAdded;
        gameController.playerController.inventory.OnFlintAdded += OnFlintAdded;
        gameController.playerController.inventory.OnStickAdded += OnStickAdded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        gameController.playerController.OnDeath -= OnPlayerDeath;
        gameController.playerController.OnCoolingDown -= OnPlayerCoolingDown;
        gameController.playerController.OnWarmingUp -= OnPlayerWarmingUp;
        gameController.playerController.inventory.OnAxeAdded -= OnAxeAdded;
        gameController.playerController.inventory.OnWoodAdded -= OnWoodAdded;
        gameController.playerController.inventory.OnFlintAdded -= OnFlintAdded;
        gameController.playerController.inventory.OnStickAdded -= OnStickAdded;
    }

    private void OnFlintAdded()
    {
        AddReward(RewardController.Instance.FlintAddedReward);
    }

    private void OnStickAdded()
    {
        AddReward(RewardController.Instance.StickAddedReward);
    }

    private void OnWoodAdded()
    {
        AddReward(RewardController.Instance.WoodAddedReward);
    }

    private void OnAxeAdded()
    {
        AddReward(RewardController.Instance.AxeAddedReward);
    }

    private void OnPlayerWarmingUp()
    {
        AddReward(RewardController.Instance.WarmingUpReward);
    }

    private void OnPlayerCoolingDown()
    {
        AddReward(RewardController.Instance.CoolingDownReward);
    }

    private void OnPlayerDeath()
    {
        AddReward(RewardController.Instance.DeathReward);
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        // MaxStep = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("per_agent_max_steps", 600.0f);
        gameController.ResetGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameController.state);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        gameController.playerController.ConstructAxe(actions.DiscreteActions[0]);
        gameController.playerController.PerformAction(actions.DiscreteActions[1], actions.DiscreteActions[2]);
    }

    public override void AddReward(float reward)
    {
        base.AddReward(reward);

        OnRewardUpdated?.Invoke(GetCumulativeReward());
    }

    public override void SetReward(float reward)
    {
        base.SetReward(reward);

        OnRewardUpdated?.Invoke(GetCumulativeReward());
    }
}
