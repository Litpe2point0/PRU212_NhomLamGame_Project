using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockAndParry : MonoBehaviour
{
    [Header("Parrying Parameter")]
    [SerializeField] float parryWindowTime;
    [SerializeField] float parryCooldown;
    private float parryWindowTimer;
    private float parryCooldownTimer;
    private bool parryWindow = true;

    private int m_facingDirection = 1;
    private bool m_blocking = false;
    private bool m_parrying;
    public void StartBlockAndParry()
    {
        StartCoroutine(PlayerBlockAndParry());
    }

    IEnumerator PlayerBlockAndParry()
    {
        m_blocking = true;
        if (parryWindow)
        {
            m_parrying = true;
        }

        while (m_blocking && parryWindowTimer < parryWindowTime && parryWindow)
        {
            parryWindowTimer += Time.deltaTime;
            Debug.Log("Parrying");
            yield return null;
        }

        Debug.Log("Stop Parrying");
        m_parrying = false;
        yield break;
    }
    public void EndBlockAndParry()
    {
        m_blocking = false;
        m_parrying = false;
        parryWindowTimer = 0;
        parryWindow = false;
    }
    public void CheckParryWindow()
    {
        if (!parryWindow)
        {
            parryCooldownTimer += Time.deltaTime;
        }
        if (parryCooldownTimer >= parryCooldown)
        {
            parryWindow = true;
            parryCooldownTimer = 0;
        }
    }
    public bool IsBlocking()
    {
        return m_blocking;
    }

    public bool IsParrying()
    {
        return m_parrying;
    }
    public int GetDirection()
    {
        return m_facingDirection;
    }
    public void SetDirection(int value)
    {
        m_facingDirection = value;
    }
}
