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

        Tube hitTube = null;

        foreach (var hit in hits)
        {
            Ball ball = hit.collider.GetComponent<Ball>();
            if (ball != null)
            {
                Tube parentTube = ball.transform.parent.GetComponent<Tube>();
                if (parentTube != null)
                {
                    // Nếu click lại chính bóng đang được chọn → hủy chọn
                    if (GameManager.instance.IsSelectedBall(ball))
                    {
                        GameManager.instance.UnselectBall();
                    }
                    // Nếu chưa có bóng nào được chọn → chọn bóng
                    else if (!GameManager.instance.HasSelectedBall())
                    {
                        GameManager.instance.SelectBall(ball, parentTube);
                    }

                    return; // Ưu tiên xử lý ball xong thì không xét tiếp
                }
            }

            // Nếu là tube thì lưu lại (chỉ xử lý sau nếu đã có bóng được chọn)
            if (hitTube == null)
            {
                hitTube = hit.collider.GetComponent<Tube>();
            }
        }

        // Nếu có bóng đang chọn và có tube bị hit → xử lý move
        if (GameManager.instance.HasSelectedBall() && hitTube != null)
        {
            hitTube.OnMouseDown();
        }
    }

}
