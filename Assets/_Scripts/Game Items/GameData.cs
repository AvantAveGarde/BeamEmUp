using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public int numberOfPlayers = 2;

    public Material[] playerColors;

    public int timeLimit = 30;
    public float distanceToZap = 6.0f;
    public float timeToZap = 1.5f;

    public int[] playerScores;
    public bool HorizontalMode;

    public bool[] playerYAxisInverted;
}
