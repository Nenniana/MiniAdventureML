using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using MiniAdventure;
using System;
using MiniAdventure.Helpers;
using Sirenix.OdinInspector;
using System.Linq;

public class AlphaAgent : Agent
{
    public Action<float> OnRewardUpdated;

    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private int actionMaskSize = 21;
    private InteractionType interactionType;
    private bool isTraining = false;

    private VectorSensor observationSensor;

    public VectorSensor ObservationSensor { get { observationSensor = FullViewObservation(new VectorSensor(47)); return observationSensor; } private set => observationSensor = value; }

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
        gameController.playerController.OnWarmAgain += OnWon;
        gameController.OnInteractFail += OnInteractFailed;
        gameController.OnWoodAttemptFail += OnWoodAttemptFailed;
        gameController.playerController.inventory.OnAxeAttemptFail += OnAxeAttemptFailed;
        gameController.playerController.inventory.OnFireAttemptFail += OnFireAttemptFailed;
        gameController.playerController.PlayerStoodStill += OnPlayerStoodStill;
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
        gameController.playerController.OnWarmAgain -= OnWon;
        gameController.OnInteractFail -= OnInteractFailed;
        gameController.OnWoodAttemptFail -= OnWoodAttemptFailed;
        gameController.playerController.inventory.OnAxeAttemptFail -= OnAxeAttemptFailed;
        gameController.playerController.inventory.OnFireAttemptFail -= OnFireAttemptFailed;
        gameController.playerController.PlayerStoodStill -= OnPlayerStoodStill;
    }

    public float[] SensorToFloatArray(VectorSensor sensor) {
        float[] floatArray = new float[sensor.ObservationSize()];

        for (int i = 0; i < sensor.ObservationSize(); i++) {
            floatArray[i] = sensor.GetObservations()[i];
        }

        return floatArray;
    }

    private void OnPlayerStoodStill()
    {
        /* if (gameController.playerController.isWarmingUp)
            AddReward(RewardController.Instance.WarmingUpReward); */
    }

    private void OnFireAttemptFailed()
    {
        // AddReward(RewardController.Instance.FireFailedReward);
    }

    private void OnAxeAttemptFailed()
    {
        // AddReward(RewardController.Instance.AxeFailedReward);
    }

    private void OnWoodAttemptFailed()
    {
        // AddReward(RewardController.Instance.WoodFailedReward);
    }

    private void OnInteractFailed()
    {
        // AddReward(RewardController.Instance.InteractFailedReward);
    }

    private void OnWon()
    {
        Debug.Log("Won.");
        AddReward(RewardController.Instance.FinishedReward * gameController.playerController.Warmth);
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
        // OnWon();
    }

    private void OnFlintAdded()
    {
        if (gameController.playerController.inventory.FlintAmount >= 1)
            AddReward(RewardController.Instance.FlintAddedReward);
        // Debug.Log("Flint added Reward");
    }

    private void OnStickAdded()
    {
        if (gameController.playerController.inventory.StickAmount >= 1)
            AddReward(RewardController.Instance.StickAddedReward);
        // Debug.Log("Stick added Reward");
    }

    private void OnWoodAdded()
    {
        if (gameController.playerController.inventory.WoodAmount >= 1)
            AddReward(RewardController.Instance.WoodAddedReward);
        // Debug.Log("Wood added Reward");
    }

    private void OnAxeAdded()
    {
        if (gameController.playerController.inventory.AxeAmount == 1)
            AddReward(RewardController.Instance.AxeAddedReward);
        else if (gameController.playerController.inventory.AxeAmount > 1) {
            Debug.Log("Made an extra Axe and got reward of: " + RewardController.Instance.ExtraAxeReward + ".");
            AddReward(RewardController.Instance.ExtraAxeReward);
        }
        // Debug.Log("Axe added Reward");
    }

    private void OnPlayerWarmingUp()
    {
        // AddReward(RewardController.Instance.WarmingUpReward);
    }

    private void OnPlayerCoolingDown()
    {
        AddReward(RewardController.Instance.CoolingDownReward);
    }

    private void OnPlayerDeath()
    {
        AddReward(RewardController.Instance.DeathReward);
        // Debug.Log("Lost.");
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        if (!isTraining)
        {
            isTraining = true;
        }
        else
        {
            gameController.ResetGame();
        }

        OnRewardUpdated?.Invoke(GetCumulativeReward());
        // MaxStep = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("per_agent_max_steps", 600.0f);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        FullViewObservation(sensor);
    }

    private VectorSensor FullViewObservation(VectorSensor sensor) {
        if (GameManager.Instance.debugInfo)
            Debug.Log("Player at " + gameController.playerController.playerInfo[0] + ", " + gameController.playerController.playerInfo[1] + ".");

        sensor.AddObservation(gameController.playerController.playerInfo);
        sensor.AddObservation(gameController.playerController.inventory.AxeAmount);
        sensor.AddObservation(gameController.playerController.inventory.FlintAmount);
        sensor.AddObservation(gameController.playerController.inventory.WoodAmount);
        sensor.AddObservation(gameController.playerController.inventory.StickAmount);

        AddSensorArray(sensor, gameController.closestFlints, GameManager.Instance.maxFlintsToKeep);
        AddSensorArray(sensor, gameController.closestFires, GameManager.Instance.maxFiresToKeep);
        AddSensorArray(sensor, gameController.closestLogs, GameManager.Instance.maxLogsToKeep);
        AddSensorArray(sensor, gameController.closestSticks, GameManager.Instance.maxSticksToKeep);
        AddSensorArray(sensor, gameController.closestTrees, GameManager.Instance.maxTreesToKeep);

        return sensor;
    }

    private void AddSensorArray(VectorSensor sensor, List<DistanceStruct> information, int max) {
        for (int i = 0; i < max; i++) {
            sensor.AddObservation(information[i].GetInformation());
            if (GameManager.Instance.debugInfo)
                ConstructObservationString(information[i].GetInformation());
        }
    }

    private void ConstructObservationString(float[] infoArray) {
        Debug.Log(string.Format("Info: {3} \t at (X:{0}, Y:{1}) \t\t with Null: {2}.", infoArray[0], infoArray[1], infoArray[2], new string(((WorldObject)infoArray[3]).ToString().Take(4).ToArray())));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 0) {
            gameController.playerController.ConstructAxe(actions.DiscreteActions[1]);

            if (GameManager.Instance.debugInfo)
                Debug.Log("Action: Constructed Axe.");
        }
        else if (actions.DiscreteActions[0] == 1) {
            gameController.playerController.PerformAction(actions.DiscreteActions[2], 0);

            if (GameManager.Instance.debugInfo) {
                Debug.Log("Action: Move. \t Position: " + GetPosition(actions.DiscreteActions[2]));
            }
        }
        else if (actions.DiscreteActions[0] == 2) {
            gameController.playerController.PerformAction(actions.DiscreteActions[3], 1);

            if (GameManager.Instance.debugInfo) {
                Debug.Log("Action: Inter. \t Position: " + GetPosition(actions.DiscreteActions[3]));
            }
        }
        else if (actions.DiscreteActions[0] == 3) {
            gameController.playerController.PerformAction(actions.DiscreteActions[4], 2);

            if (GameManager.Instance.debugInfo) {
                Debug.Log("Action: Fire. \t Position: " + GetPosition(actions.DiscreteActions[4]));
            }
        } else {
            Debug.Log("Action: Nothing.");
        }
    }

    private string GetPosition(int index)
    {
        if (index == 0)
            return "Up.";
        else if (index == 1)
            return "Right.";
        else if (index == 2)
            return "Down.";
        else if (index == 3)
            return "Left.";
        else
            return "None.";
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

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (!gameController.playerController.inventory.IsAxeBuildable()) {
            actionMask.SetActionEnabled(0, 0, false);
            actionMask.SetActionEnabled(1, 1, false);
        }
        if (!gameController.playerController.inventory.IsFireConstructable()) {
            actionMask.SetActionEnabled(0, 3, false);
            actionMask.SetActionEnabled(4, 0, false);
            actionMask.SetActionEnabled(4, 1, false);
            actionMask.SetActionEnabled(4, 2, false);
            actionMask.SetActionEnabled(4, 3, false);
        }

        int counter = 0;

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 3; j++) {
                bool canPerform = gameController.playerController.CanPerformAction(i, j);
                
                if (j == 1 && !canPerform)
                    counter++;

                actionMask.SetActionEnabled(j + 2, i, canPerform);
            }
        }

        if (counter == 4)
            actionMask.SetActionEnabled(0, 2, false);
    }

    public float[] GetActionMaskFloatArray() {
        float[] floats = new float[actionMaskSize];
        for (int i = 0; i < floats.Length; i++)
        {
            floats[i] = 1f;
        }

        if (!gameController.playerController.inventory.IsAxeBuildable())
        {
            floats[0] = 0;
            floats[5] = 0;
            // actionMask.SetActionEnabled(0, 0, false);
            // actionMask.SetActionEnabled(1, 1, false);
        }
        if (!gameController.playerController.inventory.IsFireConstructable())
        {
            floats[4] = 0;
            floats[17] = 0;
            floats[18] = 0;
            floats[19] = 0;
            floats[20] = 0;
            // actionMask.SetActionEnabled(0, 3, false);
            // actionMask.SetActionEnabled(4, 0, false);
            // actionMask.SetActionEnabled(4, 1, false);
            // actionMask.SetActionEnabled(4, 2, false);
            // actionMask.SetActionEnabled(4, 3, false);
        }

        int counter = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                bool canPerform = gameController.playerController.CanPerformAction(i, j);
                

                if (j == 1 && !canPerform)
                    counter++;

                floats[((j + 1) * 5) + (i + 1)] = canPerform ? 1f : 0f;
                // actionMask.SetActionEnabled(j + 2, i, canPerform);
            }
        }

        if (counter == 4)
            floats[3] = 0;
            // actionMask.SetActionEnabled(0, 2, false);

        return floats;
    }
}
