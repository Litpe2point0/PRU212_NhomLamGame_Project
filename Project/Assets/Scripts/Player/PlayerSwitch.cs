using Unity.Cinemachine;
using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private bool player1Active = true;
    [SerializeField] private CinemachineCamera cc;

    private void Awake()
    {
    }
    void Start()
    {
        cc.Follow = player1.transform;
        cc.LookAt = player1.transform;
        player1.SetActive(true);
        player2.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchPlayer();
        }
    }
    public void SwitchPlayer()
    {
        if(player1Active)
        {
            player2.transform.position = player1.transform.position;

            player1.SetActive(false);
            player2.SetActive(true);
            cc.Follow = player2.transform;
            cc.LookAt = player2.transform;
            player1Active = false;
        }
        else
        {
            player1.transform.position = player2.transform.position;

            player1.SetActive(true);
            player2.SetActive(false);
            cc.Follow = player1.transform;
            cc.LookAt = player1.transform;
            player1Active = true;
        }
    }
}
