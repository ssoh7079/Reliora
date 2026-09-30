using System;
using System.Collections;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [Header("방")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private RoomBackground roomBackground;

    [Header("카메라 경계")]
    [SerializeField] private BoxCollider2D leftWall;
    [SerializeField] private BoxCollider2D rightWall;

    [Header("보스")]
    [SerializeField] private BossController bossPrefab;

    [Header("인트로")]
    [SerializeField] private GameObject bossIntroText;
    [SerializeField] private float introTime = 2.0f;

    private PlayerController player;
    private BossController currentBoss;

    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public BoxCollider2D LeftWall => leftWall;
    public BoxCollider2D RightWall => rightWall;

    public event Action OnBossClear;


    private void Awake()
    {
        if (bossIntroText != null) bossIntroText.SetActive(false);
    }

    public void EnterRoom(PlayerController targetPlayer)
    {
        player = targetPlayer;
        roomBackground.Activate();
        SpawnBoss();
        StartCoroutine(BossIntro());
    }
    private void SpawnBoss()
    {
        currentBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity, transform);
        currentBoss.SetPlayer(player.transform);
        currentBoss.StartIntro();
    }
    private IEnumerator BossIntro()
    {
        player.SetControlLock(true);

        if (bossIntroText != null) bossIntroText.SetActive(true);

        yield return new WaitForSeconds(introTime);

        if (bossIntroText != null) bossIntroText.SetActive(false);

        player.SetControlLock(false);

        if (currentBoss != null) currentBoss.StartBattle();
    }
    public void BossDead()
    {
        player.SetControlLock(true);
        OnBossClear?.Invoke();
    }
    public void ExitRoom()
    {
        StopAllCoroutines();

        if (bossIntroText != null) bossIntroText.SetActive(false);
        if (player != null)
        {
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (!status.IsDead) player.SetControlLock(false);
        }
        roomBackground.Deactivate();
    }
}
