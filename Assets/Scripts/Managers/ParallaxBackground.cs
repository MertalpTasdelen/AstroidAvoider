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
        public float tileWidth;  // sprite eni (X ekseni)
        public float tileHeight; // sprite boyu (Y ekseni)
    }

    public ParallaxLayer[] layers;
    public PlayerMovements player;
    public DifficultyManager difficultyManager;

    private void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerMovements>();
        if (difficultyManager == null)
            difficultyManager = FindFirstObjectByType<DifficultyManager>();
    }

    void Update()
    {
        if (player == null)
        {
            foreach (var layer in layers)
            {
                foreach (var tile in layer.tiles)
                {
                    tile.position += Vector3.left * layer.speedMultiplier * Time.deltaTime;

                    // Sonsuz yatay scroll
                    if (tile.position.x <= -layer.tileWidth)
                    {
                        float maxX = FindFarthestRight(layer.tiles);
                        tile.position = new Vector3(maxX + layer.tileWidth, tile.position.y, tile.position.z);
                    }

                    // Sonsuz dikey scroll
                    if (tile.position.y <= -layer.tileHeight)
                    {
                        float maxY = FindFarthestTop(layer.tiles);
                        tile.position = new Vector3(tile.position.x, maxY + layer.tileHeight, tile.position.z);
                    }
                }
            }
            return;
        }

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

                if (tile.position.y <= -layer.tileHeight)
                {
                    float maxY = FindFarthestTop(layer.tiles);
                    tile.position = new Vector3(tile.position.x, maxY + layer.tileHeight, tile.position.z);
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

    float FindFarthestTop(Transform[] tiles)
    {
        float maxY = float.MinValue;
        foreach (var tile in tiles)
        {
            if (tile.position.y > maxY)
                maxY = tile.position.y;
        }
        return maxY;
    }
}