using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject initUI;
    [SerializeField] GameObject ingameUI;
    [SerializeField] GameObject endUI;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] Image lifeFill;
    [SerializeField] PlayerState playerState; 
    [SerializeField] DamageEffect damageEffect;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        UpdateScore();
    }

    private void Init()
    {
        playerState.onChangeLife += ChangeLife;
        playerState.onDecreaseLife += ShowDamageEffect;

        initUI.SetActive(true);
        ingameUI.SetActive(false);
        endUI.SetActive(false);
    }
    public void ShowIngameUI()
    {
        initUI.SetActive(false);
        ingameUI.SetActive(true);
        endUI.SetActive(false);

        damageEffect.Reset();
    }

    public void ShowEndUI()
    {
        finalScoreText.text = scoreText.text;
        initUI.SetActive(false);
        ingameUI.SetActive(false);
        endUI.SetActive(true);
    }

    private void UpdateScore()
    {
        scoreText.text = playerState.Score.ToString();
    }

    private void ChangeLife(int life, int maxLife)
    {
        if (lifeFill != null)
            lifeFill.fillAmount = (float)life / maxLife;
    }

    private void ShowDamageEffect()
    {
        damageEffect.PlayEffect();
    }

    void OnDestroy()
    {
        playerState.onChangeLife -= ChangeLife;
        playerState.onDecreaseLife -= ShowDamageEffect;
    }


}