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
        if (Input.GetKeyDown(KeyCode.T) && canSwitch)
        {
            StartCoroutine(SwitchPlayer());
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
        else if(oldPlayer.TryGetComponent<PlayerWizard>(out PlayerWizard oldController2))
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
}
