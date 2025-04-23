using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public float speedMultiplier;
    }

    public ParallaxLayer[] layers;
    public PlayerMovements player;
    public DifficultyManager difficultyManager; 

    void Update()
    {
        if (player == null || difficultyManager == null) return;

        Vector3 movement = player.GetCurrentMovementDirection();
        int difficulty = difficultyManager.GetCurrentDifficulty(); 

        foreach (var layer in layers)
        {
            if (layer.layerTransform == null) continue;

            float speedFactor = layer.speedMultiplier + (difficulty * 0.005f); // dinamik artış

            Vector3 delta = new Vector3(movement.x, movement.y, 0f) * -1f * speedFactor * Time.deltaTime;
            layer.layerTransform.position += delta;
        }
    }
}
