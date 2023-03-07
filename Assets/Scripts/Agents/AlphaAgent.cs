using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using MiniAdventure;
using System;
using MiniAdventure.Helpers;

public class AlphaAgent : Agent
{
    public Action<float> OnRewardUpdated;

    [SerializeField]
    private GameController gameController;
    private InteractionType interactionType;

    private bool isTraining = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        gameController.FirstGame(this);
        // Debug.Log("After Episode end reward: " + GetCumulativeReward());

        gameController.playerController.OnDeath += OnPlayerDeath;
        gameController.playerController.OnCoolingDown += OnPlayerCoolingDown;
        gameController.playerController.OnWarmingUp += OnPlayerWarmingUp;
        gameController.playerController.inventory.OnAxeAdded += OnAxeAdded;
        gameController.playerController.inventory.OnWoodAdded += OnWoodAdded;
        gameController.playerController.inventory.OnFlintAdded += OnFlintAdded;
        gameController.playerController.inventory.OnStickAdded += OnStickAdded;
        gameController.OnWoodChopped += OnWoodChopped;
        gameController.OnFireConstructed += OnFireConstructed;
        gameController.playerController.OnWarmAgain += OnWarmedAgain;
        gameController.OnInteractFail += OnInteractFailed;
        gameController.OnWoodAttemptFail += OnWoodAttemptFailed;
        gameController.playerController.inventory.OnAxeAttemptFail += OnAxeAttemptFailed;
        gameController.playerController.inventory.OnFireAttemptFail += OnFireAttemptFailed;
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
        gameController.OnWoodChopped -= OnWoodChopped;
        gameController.OnFireConstructed -= OnFireConstructed;
        gameController.playerController.OnWarmAgain -= OnWarmedAgain;
        gameController.OnInteractFail -= OnInteractFailed;
        gameController.OnWoodAttemptFail -= OnWoodAttemptFailed;
        gameController.playerController.inventory.OnAxeAttemptFail -= OnAxeAttemptFailed;
        gameController.playerController.inventory.OnFireAttemptFail -= OnFireAttemptFailed;
    }

    private void OnFireAttemptFailed()
    {
        AddReward(RewardController.Instance.FireFailedReward);
    }

    private void OnAxeAttemptFailed()
    {
        AddReward(RewardController.Instance.AxeFailedReward);
    }

    private void OnWoodAttemptFailed()
    {
        AddReward(RewardController.Instance.WoodFailedReward);
    }

    private void OnInteractFailed()
    {
        AddReward(RewardController.Instance.InteractFailedReward);
    }

    private void OnWarmedAgain()
    {
        Debug.Log("OnWarmedAgain");
        AddReward(RewardController.Instance.FinishedReward);
        EndEpisode();
    }

    private void OnWoodChopped()
    {
        AddReward(RewardController.Instance.WoodChoppedReward);
        // Debug.Log("Wood chopped Reward");
    }

    private void OnFireConstructed()
    {
        AddReward(RewardController.Instance.FireConstructedReward);
        // Debug.Log("Fire constructed Reward");
    }

    private void OnFlintAdded()
    {
        AddReward(RewardController.Instance.FlintAddedReward);
        // Debug.Log("Flint added Reward");
    }

    private void OnStickAdded()
    {
        AddReward(RewardController.Instance.StickAddedReward);
        // Debug.Log("Stick added Reward");
    }

    private void OnWoodAdded()
    {
        AddReward(RewardController.Instance.WoodAddedReward);
        // Debug.Log("Wood added Reward");
    }

    private void OnAxeAdded()
    {
        if (gameController.playerController.inventory.AxeAmount < 1) 
            AddReward(RewardController.Instance.AxeAddedReward);
        else
            AddReward(RewardController.Instance.ExtraAxeReward);
        // Debug.Log("Axe added Reward");
    }

    private void OnPlayerWarmingUp()
    {
        AddReward(RewardController.Instance.WarmingUpReward);
        // Debug.Log("Warming up Reward");
    }

    private void OnPlayerCoolingDown()
    {
        // AddReward(RewardController.Instance.CoolingDownReward);
    }

    private void OnPlayerDeath()
    {
        AddReward(RewardController.Instance.DeathReward);
        // Debug.Log("Before Episode end reward: " + GetCumulativeReward());
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        if (!isTraining) {
            isTraining = true;
        } else {
            gameController.ResetGame();
            // Debug.Log("After Episode end reward: " + GetCumulativeReward());
        }
        // MaxStep = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("per_agent_max_steps", 600.0f);
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameController.state);
        sensor.AddObservation(gameController.playerController.inventory.AxeAmount);
        sensor.AddObservation(gameController.playerController.inventory.FlintAmount);
        sensor.AddObservation(gameController.playerController.inventory.WoodAmount);
        sensor.AddObservation(gameController.playerController.inventory.StickAmount);
        sensor.AddObservation(gameController.playerController.Warmth);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        gameController.playerController.ConstructAxe(actions.DiscreteActions[0]);
        gameController.playerController.PerformAction(actions.DiscreteActions[1], actions.DiscreteActions[2]);
    }

    /* public override void Heuristic(in ActionBuffers actionsOut)
    {
        // base.Heuristic(actionsOut);

        ActionSegment<int> descreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.I))
        {
            interactionType = InteractionType.Interact;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            interactionType = InteractionType.ContructFire;
        }
        else
        {
            interactionType = InteractionType.Move;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            descreteActions[1] = 0;
            descreteActions[2] = int.Parse(((int)interactionType).ToString().TrimEnd('0'));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            descreteActions[1] = 1;
            descreteActions[2] = int.Parse(((int)interactionType).ToString().TrimEnd('0'));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            descreteActions[1] = 2;
            descreteActions[2] = int.Parse(((int)interactionType).ToString().TrimEnd('0'));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            descreteActions[1] = 3;
            descreteActions[2] = int.Parse(((int)interactionType).ToString().TrimEnd('0'));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            descreteActions[0] = 1;
        }
    } */

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
