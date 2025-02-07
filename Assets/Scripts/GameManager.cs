using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int monkeyCount;
    public int pirateCount;

    [Header("UI")]
    public Text monkeyScore;
    public Text pirateScore;

    private void Update()
    {
        monkeyScore.text = monkeyCount.ToString();
        pirateScore.text = pirateCount.ToString();
    }
}
