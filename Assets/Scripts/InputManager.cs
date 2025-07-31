using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTouch(Input.GetTouch(0).position);
        }
#endif
    }

    void HandleTouch(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Ball hitBall = null;
        Tube targetTube = null;

        foreach (var hit in hits)
        {
            if (hitBall == null)
            {
                hitBall = hit.collider.GetComponent<Ball>();
            }

            if (targetTube == null)
            {
                targetTube = hit.collider.GetComponent<Tube>();
            }
        }
        if (hitBall != null)
        {
            Tube parentTube = hitBall.transform.parent.GetComponent<Tube>();

            if (GameManager.instance.HasSelectedBall())
            {
                if (parentTube != null)
                {
                    GameManager.instance.TryMoveSelectedBallTo(parentTube);
                }
                return;
            }

            if (parentTube != null)
            {
                if (GameManager.instance.IsSelectedBall(hitBall))
                {
                    GameManager.instance.UnselectBall();
                }
                else
                {
                    GameManager.instance.SelectBall(hitBall, parentTube);
                }
                return;
            }
        }

        if (GameManager.instance.HasSelectedBall() && targetTube != null)
        {
            GameManager.instance.TryMoveSelectedBallTo(targetTube);
        }
    }


}
