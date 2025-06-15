using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private int m_score;
    public Vampires vampires;
    public UI_MonitorActive uiMonitor;

    public Weapon_Base[] weapons;

    private void Update()
    {
        if (vampires.Finished)
        {
            GameOver();
        }
    }

    public void SetScore(int score)
    {
        m_score += score;
        uiMonitor.SetScore(m_score);
    }

    public void EndGame()
    {
        foreach (Weapon_Base weapon in weapons)
        {
            weapon.enabled = false;
        }
    }

    public void GameOver()
    {
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(0).name);
    }
}