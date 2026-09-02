using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;

    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("追跡設定")]
    [SerializeField] private float chaseDistance = 8.0f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("ランダム移動")]
    [SerializeField] private float randomMoveRadius = 5.0f;
    [SerializeField] private float randomMoveInterval = 3.0f;

    private Vector3 randomTarget;
    private float randomMoveTimer;

    private bool isChasing = false;

    void Start()
    {
        // プレイヤーを自動取得
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        SetRandomTarget();
    }

    void Update()
    {
        if (player == null)
            return;

        // プレイヤーとの距離
        float distance = Vector3.Distance(transform.position, player.position);

        // 一定距離以内なら追跡
        if (distance <= chaseDistance)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer(distance);
        }
        else
        {
            RandomMove();
        }
    }

    // =========================
    // プレイヤーを追跡
    // =========================
    private void ChasePlayer(float distance)
    {
        // プレイヤーに近づきすぎたら停止
        if (distance <= stopDistance)
            return;

        Vector3 direction = player.position - transform.position;

        // 高さ方向は無視
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            // 移動
            transform.position += direction * moveSpeed * Time.deltaTime;

            // プレイヤーの方向を向く
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // =========================
    // ランダム移動
    // =========================
    private void RandomMove()
    {
        randomMoveTimer -= Time.deltaTime;

        // 一定時間ごとに目的地を変更
        if (randomMoveTimer <= 0)
        {
            SetRandomTarget();
        }

        Vector3 direction = randomTarget - transform.position;

        // 高さ方向は無視
        direction.y = 0;

        // 目的地に到着
        if (direction.magnitude < 0.2f)
        {
            SetRandomTarget();
            return;
        }

        direction.Normalize();

        // 移動
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 移動方向を向く
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // =========================
    // ランダムな目的地を設定
    // =========================
    private void SetRandomTarget()
    {
        randomMoveTimer = randomMoveInterval;

        Vector2 randomPosition = Random.insideUnitCircle * randomMoveRadius;

        randomTarget = transform.position + new Vector3(
            randomPosition.x,
            0,
            randomPosition.y
        );
    }

    // =========================
    // Scene上で追跡範囲を表示
    // =========================
    private void OnDrawGizmosSelected()
    {
        // 追跡範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // 停止距離
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // ランダム移動範囲
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, randomMoveRadius);
    }
}