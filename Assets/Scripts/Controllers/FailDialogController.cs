using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FailDialogController : MonoBehaviour
{
    public TextMeshProUGUI mockingText;

    public AudioSource audioSource;
    public AudioClip punchSound;
    public AudioClip failSound;

    public Animator animator;

    public List<string> mockLines;

    private static readonly int FailDialog = Animator.StringToHash("SpawnFailDialog");


    private void PlayPunchSound()
    {
        audioSource.clip = punchSound;
        audioSource.Play();
    }

    private void PlayFailSound()
    {
        audioSource.clip = failSound;
        audioSource.Play();
    }

    public void SpawnFailDialog()
    {
        mockingText.SetText(
            mockLines[Random.Range(0, mockLines.Count)]
        );
        animator.SetTrigger(FailDialog);
    }
}