using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject playerKnight;
    [SerializeField] private GameObject playerWizard;
    [SerializeField] private bool player1Active = true;

    private PlayerKnight pkS;
    private FollowPoint pkFP;
    private Health pkH;

    private PlayerWizard pwS;
    private FollowPoint pwFP;
    private Health pwH;
    private void Awake()
    {
        pkS = playerKnight.GetComponent<PlayerKnight>();
        pkFP = playerKnight.GetComponent<FollowPoint>();
        pkH = playerKnight.GetComponent<Health>();
        pwS = playerWizard.GetComponent<PlayerWizard>();
        pwFP = playerWizard.GetComponent<FollowPoint>();
        pwH = playerWizard.GetComponent<Health>();
    }
    void Start()
    {
        pkS.enabled = true;
        pkFP.enabled = false;
        pkH.enabled = true;

        pwS.enabled = false;
        pwFP.enabled = true;
        pwH.enabled = false;
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
            pkS.enabled = !player1Active;
            pkFP.enabled = player1Active;
            pkH.enabled = !player1Active;

            pwS.enabled = player1Active;
            pwFP.enabled = !player1Active;
            pwH.enabled = player1Active;

            player1Active = false;
        }
        else
        {
            pkS.enabled = !player1Active;
            pkFP.enabled = player1Active;
            pkH.enabled = !player1Active;

            pwS.enabled = player1Active;
            pwFP.enabled = !player1Active;
            pwH.enabled = player1Active;
        }
    }
}
