using UnityEngine;

internal class RewardController : MonoBehaviour
{
    [HideInInspector]
    public static RewardController Instance { get; private set; }

    [SerializeField]
    private float flintAddedReward = 2;
    [SerializeField]
    private float stickAddedReward = 1;
    [SerializeField]
    private float woodAddedReward = 5;
    [SerializeField]
    private float axeAddedReward = 3;
    [SerializeField]
    private float warmingUpReward = 10;
    [SerializeField]
    private float coolingDownReward = -0.5f;
    [SerializeField]
    private float deathReward = -100;
    [SerializeField]
    private float fireConstructedReward = 30;
    [SerializeField]
    private float woodChoppedReward = 10;

    internal float FlintAddedReward { get => flintAddedReward; set => flintAddedReward = value; }
    internal float StickAddedReward { get => stickAddedReward; set => stickAddedReward = value; }
    internal float WoodAddedReward { get => woodAddedReward; set => woodAddedReward = value; }
    internal float AxeAddedReward { get => axeAddedReward; set => axeAddedReward = value; }
    internal float WarmingUpReward { get => warmingUpReward; set => warmingUpReward = value; }
    internal float CoolingDownReward { get => coolingDownReward; set => coolingDownReward = value; }
    internal float DeathReward { get => deathReward; set => deathReward = value; }
    internal float FireConstructedReward { get => fireConstructedReward; set => fireConstructedReward = value; }
    internal float WoodChoppedReward { get => woodChoppedReward; set => woodChoppedReward = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}