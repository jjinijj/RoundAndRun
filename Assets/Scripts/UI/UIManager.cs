using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject initUI;
    [SerializeField] GameObject ingameUI;
    [SerializeField] GameObject endUI;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI lifeText;
    [SerializeField] PlayerState playerState; 

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        playerState.onChangeLife += ChangeLife;
        playerState.onChangeScore += ChangeScore;

        initUI.SetActive(true);
        ingameUI.SetActive(false);
        endUI.SetActive(false);
    }
    public void ShowIngameUI()
    {
        initUI.SetActive(false);
        ingameUI.SetActive(true);
        endUI.SetActive(false);
    }

    public void ShowEndUI()
    {
        initUI.SetActive(false);
        ingameUI.SetActive(false);
        endUI.SetActive(true);
    }

    private void ChangeScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void ChangeLife(int life)
    {
        lifeText.text = life.ToString();
    }

    void OnDestroy()
    {
        playerState.onChangeLife -= ChangeLife;
        playerState.onChangeScore -= ChangeScore;
    }


}