using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int monkeyCount;

    [Header("UI")]
    public Text monkeyScore;

    private void Update()
    {
        monkeyScore.text = monkeyCount.ToString();
    }
}
