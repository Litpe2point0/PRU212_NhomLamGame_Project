using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private bool player1Active = true;
    [SerializeField] private CinemachineCamera cc;
    [SerializeField] private float switchCooldown = 0.5f; // Cooldown time
    private bool canSwitch = true;

    private Rigidbody2D rb1;
    private Rigidbody2D rb2;
    private PlayerKnight controller1;
    private PlayerWizard controller2;
    private bool isPlayer1Alive = true;
    private bool isPlayer2Alive = true;
    private bool once = false;
    private void Awake()
    {
        rb1 = player1.GetComponent<Rigidbody2D>();
        rb2 = player2.GetComponent<Rigidbody2D>();
        controller1 = player1.GetComponent<PlayerKnight>();
        controller2 = player2.GetComponent<PlayerWizard>();
    }

    void Start()
    {
        SwitchTo(player1, player2);
    }

    void Update()
    {
        CheckPlayerAlive();
        if (Input.GetKeyDown(KeyCode.T) && canSwitch && isPlayer1Alive && isPlayer2Alive)
        {
            StartCoroutine(SwitchPlayer());
        }
        if (!isPlayer1Alive && isPlayer2Alive && !once)
        {
            player2.transform.position = new Vector3(
                player1.transform.position.x,
                player1.transform.position.y + 0.05f,
                player1.transform.position.z
                );

            player2.transform.localScale = new Vector3(
                Mathf.Sign(player1.transform.localScale.x) * player2.transform.localScale.x,
                player2.transform.localScale.y,
                player2.transform.localScale.z
                );
            player2.SetActive(true);
            player2.GetComponent<PlayerWizard>().enabled = true;
            player2.GetComponent<PlayerInput>().enabled = true;
            cc.Follow = player2.transform;
            cc.LookAt = player2.transform;

            player1Active = (player2 == player1);
            once = true;
        }
        if (!isPlayer2Alive && isPlayer1Alive && !once)
        {
            player1.transform.position = new Vector3(
                player2.transform.position.x,
                player2.transform.position.y + 0.05f,
                player2.transform.position.z
                );

            player1.transform.localScale = new Vector3(
                Mathf.Sign(player2.transform.localScale.x) * player1.transform.localScale.x,
                player1.transform.localScale.y,
                player1.transform.localScale.z
                );
            player1.SetActive(true);
            player1.GetComponent<PlayerKnight>().enabled = true;
            player1.GetComponent<PlayerInput>().enabled = true;
            cc.Follow = player1.transform;
            cc.LookAt = player1.transform;

            player1Active = (player1 == player1);
            once = true;
        }
    }

    private IEnumerator SwitchPlayer()
    {
        canSwitch = false;

        if (player1Active)
        {
            SwitchTo(player2, player1);
        }
        else
        {
            SwitchTo(player1, player2);
        }
        yield return new WaitForSeconds(switchCooldown);
        canSwitch = true;
    }
    void CheckPlayerAlive()
    {
        if (player1.TryGetComponent<Health>(out Health player1Health) && player2.TryGetComponent<Health>(out Health player2Health))
        {
            if (player1Health.GetCurrentHealth() <= 0)
            {
                isPlayer1Alive = false;
            }
            if (player2Health.GetCurrentHealth() <= 0)
            {
                isPlayer2Alive = false;
            }
        }
    }
    private void SwitchTo(GameObject newPlayer, GameObject oldPlayer)
    {
        // 🔹 RESET INPUT to avoid movement carry-over
        if (oldPlayer.TryGetComponent<Rigidbody2D>(out Rigidbody2D oldRb))
        {
            oldRb.linearVelocity = Vector2.zero;
            oldRb.angularVelocity = 0f;
        }
        if (newPlayer.TryGetComponent<Rigidbody2D>(out Rigidbody2D newRb))
        {
            newRb.linearVelocity = Vector2.zero;
            newRb.angularVelocity = 0f;
        }

        // 🔹 If using an Input Script, disable and re-enable to clear input buffer
        if (oldPlayer.TryGetComponent<PlayerKnight>(out PlayerKnight oldController))
        {
            oldController.ResetInput();
            oldController.enabled = false; // Disable the input script
        }
        else if (oldPlayer.TryGetComponent<PlayerWizard>(out PlayerWizard oldController2))
        {
            oldController2.ResetInput();
            oldController2.enabled = false; // Disable the input script
        }
        if (newPlayer.TryGetComponent<PlayerWizard>(out PlayerWizard newController))
        {
            newController.ResetInput();
            newController.enabled = true; // Enable it on new player
        }
        else if (newPlayer.TryGetComponent<PlayerKnight>(out PlayerKnight newController2))
        {
            newController2.ResetInput();
            newController2.enabled = true; // Enable it on new player
        }
        if (oldPlayer.TryGetComponent<PlayerInput>(out PlayerInput oldInput))
        {
            oldInput.enabled = false;
        }
        if (newPlayer.TryGetComponent<PlayerInput>(out PlayerInput newInput))
        {
            newInput.enabled = true;
        }
        // 🔹 Adjust position and scale
        newPlayer.transform.position = new Vector3(
            oldPlayer.transform.position.x,
            oldPlayer.transform.position.y + 0.05f,
            oldPlayer.transform.position.z
        );

        newPlayer.transform.localScale = new Vector3(
            Mathf.Sign(oldPlayer.transform.localScale.x) * newPlayer.transform.localScale.x,
            newPlayer.transform.localScale.y,
            newPlayer.transform.localScale.z
        );

        // 🔹 Activate the new player and deactivate the old one
        oldPlayer.SetActive(false);
        newPlayer.SetActive(true);

        // 🔹 Update camera
        cc.Follow = newPlayer.transform;
        cc.LookAt = newPlayer.transform;

        // 🔹 Toggle active player
        player1Active = (newPlayer == player1);
    }
    public float GetCurrentPlayerHealth()
    {
        if (player1Active)
        {
            return player1.GetComponent<Health>().GetCurrentHealth();
        }
        else
        {
            return player2.GetComponent<Health>().GetCurrentHealth();
        }
    }
    public float GetCurrentPlayerMaxHealth()
    {
        if (player1Active)
        {
            return player1.GetComponent<Health>().GetMaxHealth();
        }
        else
        {
            return player2.GetComponent<Health>().GetMaxHealth();
        }
    }
    public bool GetPlayer1Active()
    {
        return player1Active;
    }
    public GameObject GetPlayer1()
    {
        return player1;
    }
    public GameObject GetPlayer2()
    {
        return player2;
    }
}
