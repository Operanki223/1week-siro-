using UnityEngine;

public class Police : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hitDistance = 10f;

    private bool hitByPlayer = false;
    private bool isHit = false;

    private void Update()
    {
        HitPlayer();

        // プレイヤーを新しく発見した瞬間
        if (hitByPlayer && !isHit)
        {
            isHit = true;

            Debug.Log("プレイヤーを発見！");

            DialogueManager.instance.StartDialogue(0);
        }

        // プレイヤーを見失った
        if (!hitByPlayer)
        {
            isHit = false;
        }
    }

    private void HitPlayer()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 0.5f,
            transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            hitDistance,
            playerLayer))
        {
            hitByPlayer = true;
        }
        else
        {
            hitByPlayer = false;
        }

        Debug.DrawRay(
            transform.position + Vector3.up * 0.5f,
            transform.forward * hitDistance,
            hitByPlayer ? Color.red : Color.green
        );
    }
}