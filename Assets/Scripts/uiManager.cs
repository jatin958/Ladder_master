using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class uiManager : MonoBehaviour
{
    [Header("Panel")] [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject lossPanel;

    [Header("rewardLogic")] [SerializeField]
    private int defaultCoinReward = 50;
    [SerializeField] private TextMeshProUGUI coinText;
    [Header("References")]
    [SerializeField] private RectTransform coinPrefab;
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private Transform targetPoint;
    [Header("Coin Settings")]
    [SerializeField] private int coinCount = 10;
    [SerializeField] private float coinScale = 0.65f;

    [Header("Spread")]
    [SerializeField] private float spreadRadius = 120f;
    [SerializeField] private float spreadDuration = 0.35f;

    [Header("Fly To Target")]
    [SerializeField] private float flyDuration = 0.65f;
    [SerializeField] private float delayBetweenCoins = 0.06f;

    [Header("Animation")]
    [SerializeField] private Ease spreadEase = Ease.OutBack;
    [SerializeField] private Ease flyEase = Ease.InBack;


    private void OnEnable()
    {
        Movment.onWin += showWinPanel;
        LadderLogic.onDead += showLossPanel;
    }

    private void OnDisable()
    {
        Movment.onWin -= showWinPanel;
        LadderLogic.onDead -= showLossPanel;
    }

    private void showWinPanel()
    {
        winPanel.SetActive(true);
        GameManager.instance.setgameOver(true);
        Play();
    }

    public void showLossPanel()
    {
        lossPanel.SetActive(true);
    }
     public void Play()
    {
        StartCoroutine(PlayCoinAnimation());
    }

    private IEnumerator PlayCoinAnimation()
    {
        RectTransform[] coins = new RectTransform[coinCount];

        Vector2 spawnPosition = spawnPoint.anchoredPosition;

        // -----------------------------
        // 1. SPAWN COINS
        // -----------------------------

        for (int i = 0; i < coinCount; i++)
        {
            RectTransform coin = Instantiate(
                coinPrefab,
                spawnPoint.parent
            );

            coin.anchoredPosition = spawnPosition;
            coin.localScale = Vector3.one * coinScale;

            coin.SetAsLastSibling();

            coins[i] = coin;
        }

        // -----------------------------
        // 2. SPREAD
        // -----------------------------

        for (int i = 0; i < coinCount; i++)
        {
            RectTransform coin = coins[i];

            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            float randomDistance = Random.Range(
                spreadRadius * 0.5f,
                spreadRadius
            );

            Vector2 spreadPosition =
                spawnPosition +
                randomDirection * randomDistance;

            coin
                .DOAnchorPos(spreadPosition, spreadDuration)
                .SetEase(spreadEase);
        }

        // Wait until spread is finished
        yield return new WaitForSeconds(spreadDuration);

        // Small pause before flying
        yield return new WaitForSeconds(0.08f);

        // -----------------------------
        // 3. FLY TO TARGET
        // -----------------------------

        Vector3 targetPosition = targetPoint.position;

        for (int i = 0; i < coinCount; i++)
        {
            RectTransform coin = coins[i];

            float delay = i * delayBetweenCoins;

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(delay);

            sequence.Append(
                coin
                    .transform.DOMove(targetPosition, flyDuration)
                    .SetEase(flyEase)
            );

            sequence.Join(
                coin
                    .DOScale(coinScale * 0.55f, flyDuration)
                    .SetEase(Ease.InQuad)
            );

            // Little pop when reaching target
            sequence.Append(
                coin
                    .DOScale(coinScale * 0.8f, 0.08f)
                    .SetEase(Ease.OutQuad)
            );

            sequence.Append(
                coin
                    .DOScale(0f, 0.12f)
                    .SetEase(Ease.InBack)
            );

            sequence.OnComplete(() =>
            {
                Destroy(coin.gameObject);
            });
        }
    }

    public void setRewardText(int multiplier)
    {
        var finaleVlaue = defaultCoinReward *  multiplier;
        coinText.text = finaleVlaue.ToString();
    }
}