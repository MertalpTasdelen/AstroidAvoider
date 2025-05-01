using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxBackground : MonoBehaviour
{
    public enum Mode { AutoScroll, FollowPlayer }
    public Mode parallaxMode = Mode.FollowPlayer;

    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform[] tiles; // birden fazla tile
        public float speedMultiplier;
        public float tileWidth; // her tile'ın genişliği (Unity units)
    }

    public ParallaxLayer[] layers;
    public PlayerMovements player;
    public DifficultyManager difficultyManager;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerMovements>();
        if (difficultyManager == null)
            difficultyManager = FindObjectOfType<DifficultyManager>();
    }

    void Update()
    {
        if (player == null)
        {
            // Ana Menü: Auto Scroll + Sonsuz Kaydırma
            foreach (var layer in layers)
            {
                foreach (var tile in layer.tiles)
                {
                    tile.position += Vector3.left * layer.speedMultiplier * Time.deltaTime;

                    if (tile.position.x <= -layer.tileWidth)
                    {
                        float maxX = FindFarthestRight(layer.tiles);
                        tile.position = new Vector3(maxX + layer.tileWidth, tile.position.y, tile.position.z);
                    }
                }
            }
            return;
        }

        // Oyun: Player'a bağlı paralaks + zorluk dinamiği
        Vector3 movement = player.GetCurrentMovementDirection();
        int difficulty = difficultyManager != null ? difficultyManager.GetCurrentDifficulty() : 1;

        foreach (var layer in layers)
        {
            float speedFactor = layer.speedMultiplier + (difficulty * 0.005f);
            Vector3 delta = new Vector3(movement.x, movement.y, 0f) * -1f * speedFactor * Time.deltaTime;

            foreach (var tile in layer.tiles)
            {
                tile.position += delta;

                if (tile.position.x <= -layer.tileWidth)
                {
                    float maxX = FindFarthestRight(layer.tiles);
                    tile.position = new Vector3(maxX + layer.tileWidth, tile.position.y, tile.position.z);
                }
            }
        }
    }

    float FindFarthestRight(Transform[] tiles)
    {
        float maxX = float.MinValue;
        foreach (var tile in tiles)
        {
            if (tile.position.x > maxX)
                maxX = tile.position.x;
        }
        return maxX;
    }
}