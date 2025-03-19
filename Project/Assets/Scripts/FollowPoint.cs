using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        gameObject.GetComponent<Transform>().position = target.position;
    }
}
