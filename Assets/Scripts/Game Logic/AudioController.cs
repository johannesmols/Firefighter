using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip backgroundMusic;
    public AudioClip fireSpread;
    public List<AudioClip> unitChoose;
    public List<AudioClip> unitDeath;
    public List<AudioClip> unitMove;
    public List<AudioClip> diggerTrench;
    public List<AudioClip> missionStart;
    public List<AudioClip> missionFail;
    public List<AudioClip> missionSuccess;

    private AudioSource audioSource;
    private readonly System.Random random = new System.Random();

    public void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayMissionStartSound()
    {
        audioSource.PlayOneShot(missionStart[random.Next(missionStart.Count)]);
    }

    public void PlayMissionFailSound()
    {
        audioSource.PlayOneShot(missionFail[random.Next(missionFail.Count)]);
    }

    public void PlayMissionSuccessSound()
    {
        audioSource.PlayOneShot(missionSuccess[random.Next(missionSuccess.Count)]);
    }

    public void PlayUnitChooseSound()
    {
        audioSource.PlayOneShot(unitChoose[random.Next(unitChoose.Count)]);
    }

    public void PlayUnitMoveSound()
    {
        audioSource.PlayOneShot(unitMove[random.Next(unitMove.Count)]);
    }

    public void PlayUnitDeathSound()
    {
        audioSource.PlayOneShot(unitDeath[random.Next(unitDeath.Count)]);
    }

    public void PlayDigTrenchSound()
    {
        audioSource.PlayOneShot(diggerTrench[random.Next(diggerTrench.Count)]);
    }

    public void PlayFireSpreadSound()
    {
        audioSource.PlayOneShot(fireSpread);
    }
}
