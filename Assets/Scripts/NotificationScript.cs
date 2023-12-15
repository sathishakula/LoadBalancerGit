using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;

// LER notification popup script
public class NotificationScript : MonoBehaviour
{
    // declaring objects
    public TMP_Text notifText;
    public GameObject notifWindow;
    private Animator notifAnimator;

    // declaring logical fields
    private Queue<string> notifQueue;
    private Coroutine queueChecker;

    private void Start()
    {
        notifAnimator = GetComponent<Animator>();
        notifWindow.SetActive(false);
        notifQueue = new Queue<string>();
    }

    public void AddToQueue(string text)
    {
        notifQueue.Enqueue(text);
        notifWindow.SetActive(true);
        if (queueChecker == null)
        {
            queueChecker = StartCoroutine(CheckQueue());
        }
    }
    
    // call to notification animation
    private void ShowNotification(string text)
    {
        notifText.text = text;
        notifAnimator.Play("NotificationAnimation");
    }

    private IEnumerator CheckQueue()
    {
        do
        {
            ShowNotification(notifQueue.Dequeue());
            do
            {
                yield return null;
            } while (!notifAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
        } while (notifQueue.Count > 0);
        notifWindow.SetActive(false);
        queueChecker = null;
    }
}
