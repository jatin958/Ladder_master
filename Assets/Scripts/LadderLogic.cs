using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LadderLogic : MonoBehaviour
{
    public static event Action onDead;
    [SerializeField] private Movment mv;
    [SerializeField] private colorType PlayerColor;
    [SerializeField] private Transform ladderHolder;
    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private int ladderCount;
    [SerializeField] private int remailLadderCount;

    [Space(3)] [SerializeField] private List<Transform> laddersList = new List<Transform>();

    [SerializeField] private Rig ladderRig;

    [Header("Ladder Animation")] [SerializeField]
    private float animationDuration = 0.3f;

    [SerializeField] private float jumpPower = 0.4f;
    [SerializeField] private Ease ladderEase = Ease.OutBack;

    [Header("Ladder remove logic")] [SerializeField]
    private float lastPosY;

    [SerializeField] private float threshold = 0.2f;

    [SerializeField]private bool isBonusBuilding = false;
    [SerializeField] private int currentMultiplier;

    public void checkladder()
    {
        if (laddersList.Count > 0)
        {
            var diff = transform.position.y - lastPosY;
            if (diff >= threshold)
            {
                laddersList.Remove(laddersList[0]);
                lastPosY = transform.position.y;
            }
        }
        else
        {
            GameManager.instance.setgameOver(true);
            onDead?.Invoke();
        }
    }

    private void AddLadder()
    {
        if (ladderRig.weight == 0)
        {
            ladderRig.weight = 1;
        }

        GameObject newLadder = Instantiate(ladderPrefab, ladderHolder);
        Transform ladderTransform = newLadder.transform;
        Vector3 targetPosition = new Vector3(
            0f,
            ladderCount * 0.2f,
            0f
        );

        // Animation ki starting position
        ladderTransform.localPosition = targetPosition + new Vector3(0f, -0.4f, 0f);
        ladderTransform.localScale = Vector3.zero;
        ladderTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            ladderTransform
                .DOLocalJump(targetPosition, jumpPower, 1, animationDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Join(
            ladderTransform
                .DOScale(Vector3.one, animationDuration)
                .SetEase(ladderEase)
        );

        sequence.Join(
            ladderTransform
                .DOLocalRotate(Vector3.zero, animationDuration)
                .SetEase(Ease.OutBack)
        );

        laddersList.Add(ladderTransform);
        ladderCount++;
    }

    private void removeLadder()
    {
        if (laddersList.Count > 0)
        {
            var ladder = laddersList[^1];
            laddersList.Remove(ladder);
            if (ladder.TryGetComponent(out ladder OtherLadder))
            {
                OtherLadder.destroyLadder();
            }

            ladderCount--;
        }
        else
        {
            GameManager.instance.setgameOver(true);
            onDead?.Invoke();
        }
    }

    public void DisattachLadder()
    {
        List<Transform> temporaryLadders = laddersList.ToList();

        foreach (Transform ladder in temporaryLadders)
        {
            ladder.DOKill();
            ladder.SetParent(null);
        }

        ladderCount = 0;
    }

    public void remainLadderCount()
    {
        /*var f = Mathf.RoundToInt(yPos / 0.2f);
        laddersList.RemoveRange(0, f);*/
        foreach (var ladder in laddersList.ToList())
        {
            ladder.transform.SetParent(ladderHolder);
            ladder.localPosition = Vector3.zero + new Vector3(0, ladderCount * 0.2f, 0);
            ladderCount++;
        }

        lastPosY = 0;

        if (laddersList.Count == 0)
        {
            ladderRig.weight = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.GetisGameOver()) return;

        if (other.CompareTag("jump"))
        {
            mv.Jump();
            ladderRig.weight = 0;
        }

        if (other.TryGetComponent(out ladderTile OtherLadder))
        {
            if (OtherLadder.getLadderColor() == PlayerColor)
            {
                AddLadder();
                Destroy(other.gameObject);
            }
            else
            {
                removeLadder();
                mv.setDivider(0.5f);
            }
        }

        if (other.TryGetComponent(out changeColor color))
        {
            PlayerColor = color.color;
            setcolor(PlayerColor);
        }
        if (other.gameObject.TryGetComponent(out ScoreMultiply scoreMultiply))
        {
            isBonusBuilding = true;
            currentMultiplier = scoreMultiply.getMultiplayier();
            scoreMultiply.ChangeToSelected();
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("plateform"))
        {
            Debug.Log("Platform name " + other.collider.name);
            mv.enableMovment();
            if (ladderCount > 1)
                ladderRig.weight = 1;
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ladderTile OtherLadder))
        {
            mv.setDivider(1f);
        }
    }

    public int getCurrentMultiplier()
    {
        return currentMultiplier;
    }

    public bool getIsBonusBuilding()
    {
        return isBonusBuilding;
    }

    void setcolor(colorType color)
    {
        switch (color)
        {
            case colorType.blue:
                materialManager.instance.setColor(Color.blue);
                break;
            case colorType.red:
                materialManager.instance.setColor(Color.red);
                break;
            case colorType.green:
                materialManager.instance.setColor(Color.green);
                break;
            case colorType.yellow:
                materialManager.instance.setColor(Color.yellow);
                break;
        }
    }

    public int getLaaderCount()
    {
        return laddersList.Count;
    }
}