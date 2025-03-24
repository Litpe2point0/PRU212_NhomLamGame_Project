using Unity.Cinemachine;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] CinemachineCamera cc;
    [SerializeField] AudioClip slashClip;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip landClip;
    [SerializeField] AudioClip magicOrbBlastClip;
    [SerializeField] AudioClip magicOrbHitClip;
    [SerializeField] AudioClip fireBreathingClip;
    [SerializeField] AudioClip magicSpellClip;
    [SerializeField] AudioClip darkHandSpellClip;


    private void Awake()
    {
        
    }


    public void PlaySlashCLip()
    {
        if (slashClip != null)
        {
            PlayClip(slashClip, 0.7f);
        }
    }
    public void PlayHitCLip()
    {
        if (hitClip != null)
        {
            PlayClip(hitClip, 0.7f);
        }
    }
    public void PlayJumpCLip()
    {
        if (jumpClip != null)
        {
            PlayClip(jumpClip, 0.7f);
        }
    }
    public void PlayLandCLip()
    {
        if (landClip != null)
        {
            PlayClip(landClip, 0.7f);
        }
    }
    public void PlayMagicOrbBlastClip()
    {
        if (magicOrbBlastClip != null)
        {
            PlayClip(magicOrbBlastClip, 0.7f);
        }
    }
    public void PlayMagicOrbHitClip()
    {
        if (magicOrbHitClip != null)
        {
            PlayClip(magicOrbHitClip, 0.7f);
        }
    }
    public void PlayFireBreathingClip()
    {
        if (fireBreathingClip != null)
        {
            PlayClip(fireBreathingClip, 0.7f);
        }
    }
    public void PlayMagicSpellClip()
    {
        if (magicSpellClip != null)
        {
            PlayClip(magicSpellClip, 0.7f);
        }
    }
    public void PlayDarkHandSpellClip()
    {
        if (darkHandSpellClip != null)
        {
            PlayClip(darkHandSpellClip, 0.7f);
        }
    }
    void PlayClip(AudioClip audio, float volume)
    {
        if (audio != null)
        {
            Vector3 cameraPos = cc.transform.position;
            AudioSource.PlayClipAtPoint(audio, cameraPos, volume);
        }
    }
}
