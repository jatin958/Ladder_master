using System;
using DG.Tweening;

using UnityEngine;


public class Movment : MonoBehaviour
{
    
    public static event Action onWin;
    [Header("var ref")] private Vector3 lastSwipePos;
    [SerializeField] private float swipeSensitivity = 0.01f;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float moveSpeedDivider;
    [SerializeField] private float boundry;

    [Header("climb ref")] [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float climbSpeed;
    [SerializeField] bool isClimbing = false;

    [Header("jump ref")] [SerializeField] private bool isJumping = false;
    [SerializeField] private float upjumpforce;
    [SerializeField] private float forwardjumpforce;

    [Header("componants")] [SerializeField]
    private Animator anim;

    [SerializeField] private Rigidbody rb;
    private LadderLogic LL;
    [SerializeField] private bool isDead = false;

    [Header("Tilt")] [SerializeField] private Transform playerVisual; // Character model
    [SerializeField] private float maxTilt = 25f;
    [SerializeField] private float tiltSpeed = 10f;
    [Header("jump ref")] [SerializeField] private Transform JumpLadderTrans;
    [SerializeField] private Transform walkLadderTrans;
    [SerializeField] private Transform ladderHolder;


    private void Awake()
    {
        LL = GetComponent<LadderLogic>();
    }

    private void OnEnable()
    {
        LadderLogic.onDead += die;
    }

    private void OnDisable()
    {
        LadderLogic.onDead -= die;
    }

    private void Update()
    {
        if (isDead || isJumping
                   || GameManager.instance.GetisGameOver()) return;

        movement();
    }

    private void LateUpdate()
    {
        if (isDead || isJumping|| GameManager.instance.GetisGameOver()) return;
        if (Input.touchCount == 0 ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Moved))
        {
            playerVisual.localRotation = Quaternion.Lerp(
                playerVisual.localRotation,
                Quaternion.identity,
                Time.deltaTime * tiltSpeed
            );
        }
    }

    void movement()
    {
        if (Input.touchCount > 0 && !isClimbing)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastSwipePos = touch.position;
                GameManager.instance.StartGame();
            }

            if (touch.phase == TouchPhase.Moved)
            {
                float differenceX = touch.position.x - lastSwipePos.x;

// Move
                Vector3 targetPos = transform.position;
                targetPos.x += differenceX * swipeSensitivity;
                targetPos.x = Mathf.Clamp(targetPos.x, -boundry, boundry);
                transform.position = targetPos;

// Tilt
                float tilt = Mathf.Clamp(-differenceX * 2, -maxTilt, maxTilt);
                Quaternion targetRot = Quaternion.Euler(0, -tilt, 0);
                playerVisual.localRotation = Quaternion.Lerp(
                    playerVisual.localRotation,
                    targetRot,
                    Time.deltaTime * tiltSpeed
                );

                lastSwipePos = touch.position;
            }
        }

        if (GameManager.instance.isGameStart())
        {
            if (isWall())
            {
                LL.checkladder();
                if (LL.getLaaderCount() == 0)
                {
                    if (LL.getIsBonusBuilding())
                    {
                        if (!GameManager.instance.GetisGameOver())
                        {
                            GameManager.instance.uiManager.setRewardText(LL.getCurrentMultiplier());
                            onWin?.Invoke();
                           
                        }
                            
                        return;
                    }

                    die();
                    return;
                }

                isClimbing = true;
                transform.position += Vector3.up * climbSpeed * Time.deltaTime;

                if (!rb.isKinematic)
                {
                    rb.isKinematic = true;
                    LL.DisattachLadder();
                }
            }
            else
            {
                if (isClimbing)
                {
                    LL.remainLadderCount();
                }

                isClimbing = false;
                var finaleMoveSpeed = moveSpeed * moveSpeedDivider;
                transform.position += transform.forward * finaleMoveSpeed * Time.deltaTime;
                if (rb.isKinematic)
                {
                    rb.isKinematic = false;
                }

                anim.SetFloat("speed", 1);
            }

            anim.SetBool("climb", isClimbing);
        }
        else
        {
            anim.SetFloat("speed", 0);
        }
    }

    bool isWall()
    {
        if (Physics.Raycast(wallCheck.position, transform.forward, wallCheckDistance, wallLayer))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.forward * wallCheckDistance);
    }

    void die()
    {
        isDead = true;
        rb.isKinematic = false;
        anim.SetTrigger("death");
    }

    public void setDivider(float value)
    {
        moveSpeedDivider = value;
    }

    public void Jump()
    {
        isJumping = true;
        ladderHolder.localPosition = JumpLadderTrans.localPosition;
        ladderHolder.localRotation = JumpLadderTrans.localRotation;
        anim.SetBool("isJump", true);
        var dir = new Vector3(0, upjumpforce, forwardjumpforce);
        rb.AddForce(dir, ForceMode.Impulse);
    }

    public void enableMovment()
    {
        if (!isJumping) return;
        ladderHolder.localPosition = walkLadderTrans.localPosition;
        ladderHolder.localRotation = walkLadderTrans.localRotation;
        isJumping = false;
        anim.SetBool("isJump", false);
        rb.linearVelocity = Vector3.zero;
    }
}