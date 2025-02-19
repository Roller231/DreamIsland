using System.Collections;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int monkeyCount;
    public int pirateCount;

    [Header("UI")]
    public Text monkeyScore;
    public Text pirateScore;


    private void Start()
    {

        StartCoroutine(StartCor());
    }

    private IEnumerator StartCor()
    {
        yield return new WaitUntil(() => GetComponent<MySQLConnectorTG>().isInitialized);


        monkeyCount = GetComponent<MySQLConnectorTG>().loadedMonkeyCoins;
        pirateCount = GetComponent<MySQLConnectorTG>().loadedPirateCoins;

    }

    private void Update()
    {
        monkeyScore.text = monkeyCount.ToString();
        pirateScore.text = pirateCount.ToString();
    }
}
