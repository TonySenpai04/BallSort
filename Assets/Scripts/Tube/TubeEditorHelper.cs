using UnityEngine;

public class TubeEditorHelper : MonoBehaviour
{
    public int ballCount = 4;

    void OnDrawGizmos()
    {
        for (int i = 0; i < ballCount; i++)
        {
            Vector3 pos = transform.position + Vector3.up * 0.3f * i;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pos, 0.15f);
        }
    }
}
