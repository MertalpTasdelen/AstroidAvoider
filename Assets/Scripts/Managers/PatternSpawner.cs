using System;
using UnityEngine;

/// <summary>
/// O2: Pattern-based spawn controller.
/// Attach this to a GameObject in the gameplay scene and it will drive AstreoidSpawner with wave segments,
/// including gaps, alternating sides, and "aimed" spawns.
/// </summary>
public class PatternSpawner : MonoBehaviour
{
    [Serializable]
    public enum PatternType
    {
        AlternatingSides,
        BurstAndGap,
        AimedShots,
        MixedBehaviors,
    }

    [Serializable]
    public class PatternSegment
    {
        public PatternType type = PatternType.MixedBehaviors;

        [Min(1f)]
        public float durationSeconds = 10f;

        [Min(0.05f)]
        public float spawnIntervalSeconds = 0.75f;

        [Header("Speed")]
        [Range(0.5f, 2.0f)]
        public float speedMultiplier = 1.0f;

        [Header("Behavior Mix")]
        [Range(0f, 1f)]
        public float zigzagChance = 0.5f;

        [Range(0f, 1f)]
        public float homingChance = 0.25f;

        [Range(0f, 1f)]
        public float splitChance = 0.25f;

        [Header("Burst+Gap")]
        [Min(1)]
        public int burstCount = 4;

        [Min(0f)]
        public float gapSeconds = 1.2f;

        [Header("Aimed")]
        [Range(0f, 1f)]
        public float aimStrength = 0.75f;
    }

    [Header("Pattern List")]
    [Tooltip("If empty, a default set will be used.")]
    [SerializeField] private PatternSegment[] segments;

    [Header("Difficulty Influence")]
    [Tooltip("Higher difficulty will speed up pattern cycling and reduce spawn interval.")]
    [SerializeField, Range(0f, 0.5f)] private float spawnIntervalReductionPerDifficulty = 0.03f;

    private AstreoidSpawner spawner;
    private int segmentIndex;
    private float segmentRemaining;
    private float spawnTimer;

    // Burst+gap state
    private int burstRemaining;
    private float gapRemaining;
    private int alternatingSide;

    private void Awake()
    {
        if (segments == null || segments.Length == 0)
        {
            segments = new[]
            {
                    new PatternSegment { type = PatternType.MixedBehaviors, durationSeconds = 12f, spawnIntervalSeconds = 0.85f, zigzagChance = 0f, homingChance = 0.15f, splitChance = 0.15f },
                    new PatternSegment { type = PatternType.AlternatingSides, durationSeconds = 10f, spawnIntervalSeconds = 0.75f, zigzagChance = 0f, homingChance = 0.25f, splitChance = 0.25f },
                    new PatternSegment { type = PatternType.BurstAndGap, durationSeconds = 10f, spawnIntervalSeconds = 0.18f, burstCount = 5, gapSeconds = 1.4f, zigzagChance = 0f, homingChance = 0.20f, splitChance = 0.10f },
                    new PatternSegment { type = PatternType.AimedShots, durationSeconds = 9f, spawnIntervalSeconds = 0.8f, aimStrength = 0.8f, zigzagChance = 0f, homingChance = 0.35f, splitChance = 0.10f },
            };
        }

        segmentIndex = 0;
        segmentRemaining = segments[0].durationSeconds;
        alternatingSide = 0;

        ResetBurstState();
    }

    private void Start()
    {
        spawner = AstreoidSpawner.Instance != null ? AstreoidSpawner.Instance : FindFirstObjectByType<AstreoidSpawner>();
        if (spawner != null)
        {
            spawner.SetExternalControl(true);
        }
    }

    private void Update()
    {
        if (spawner == null || segments == null || segments.Length == 0)
            return;

        // Advance segment
        float dt = Time.deltaTime;
        segmentRemaining -= dt;
        if (segmentRemaining <= 0f)
        {
            segmentIndex = (segmentIndex + 1) % segments.Length;
            segmentRemaining = segments[segmentIndex].durationSeconds;
            ResetBurstState();
        }

        // Spawn timing
        int difficulty = DifficultyManager.Instance != null ? DifficultyManager.Instance.GetCurrentDifficulty() : 1;
        float intervalAdjust = Mathf.Max(0f, (difficulty - 1) * spawnIntervalReductionPerDifficulty);

        PatternSegment seg = segments[segmentIndex];
        float interval = Mathf.Max(0.08f, seg.spawnIntervalSeconds - intervalAdjust);

        // Burst+gap segment overrides internal timing.
        if (seg.type == PatternType.BurstAndGap)
        {
            RunBurstAndGap(seg, difficulty, dt, interval);
            return;
        }

        spawnTimer -= dt;
        if (spawnTimer > 0f)
            return;

        spawnTimer = interval;
        SpawnForSegment(seg, difficulty);
    }

    private void RunBurstAndGap(PatternSegment seg, int difficulty, float dt, float interval)
    {
        if (gapRemaining > 0f)
        {
            gapRemaining -= dt;
            return;
        }

        spawnTimer -= dt;
        if (spawnTimer > 0f)
            return;

        spawnTimer = interval;

        SpawnForSegment(seg, difficulty);

        burstRemaining--;
        if (burstRemaining <= 0)
        {
            gapRemaining = seg.gapSeconds;
            burstRemaining = Mathf.Max(1, seg.burstCount);
        }
    }

    private void ResetBurstState()
    {
        spawnTimer = 0f;
        PatternSegment seg = segments != null && segments.Length > 0 ? segments[Mathf.Clamp(segmentIndex, 0, segments.Length - 1)] : null;
        burstRemaining = seg != null ? Mathf.Max(1, seg.burstCount) : 4;
        gapRemaining = 0f;
    }

    private void SpawnForSegment(PatternSegment seg, int difficulty)
    {
        AstreoidSpawner.SpawnRequest req = AstreoidSpawner.SpawnRequest.CreateDefault();

        req.speedMultiplier = seg.speedMultiplier * (1f + (difficulty - 1) * 0.03f);
        req.zigzagChance = seg.zigzagChance;
        req.homingChance = seg.homingChance;
        req.splitChance = seg.splitChance;

        if (seg.type == PatternType.AlternatingSides)
        {
            req.sideOverride = alternatingSide;
            alternatingSide = (alternatingSide + 1) % 4;
        }
        else if (seg.type == PatternType.AimedShots)
        {
            // Spawn from a random side but bias direction towards player.
            Vector2 aimDir = GetAimDirectionFromEdge();
            if (aimDir.sqrMagnitude > 0.0001f)
            {
                req.directionOverride = aimDir;
            }
        }

        spawner.SpawnWithRequest(ref req);
    }

    private Vector2 GetAimDirectionFromEdge()
    {
        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            return Vector2.zero;
        }

        // We don't know the exact spawn point here (spawner picks it), so we approximate by aiming inward.
        // The spawner will normalize this.
        Vector2 toPlayer = ((Vector2)player.position - Vector2.zero).normalized;
        if (toPlayer.sqrMagnitude < 0.0001f)
        {
            toPlayer = UnityEngine.Random.insideUnitCircle.normalized;
        }

        return toPlayer;
    }
}
