using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBarkTrigger : MonoBehaviour
{
    public int playerBarkAmount = -1;                   //  -1 for infinity, overridden if Recyle modes is used
    public string[] playerBarkMessages;
    public PlayerBarkMode playerBarkMode;
    public enum PlayerBarkMode { SequentialRecyle, RandomRecyle, Sequential, Random }

    private int curPlayerBarkIndex = 0;
    private List<string> remainingPlayerBarks = new List<string>();

    private void Start()
    {
        remainingPlayerBarks = playerBarkMessages.ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        //  Send bark message to player
        if (playerBarkAmount > 0 || playerBarkAmount == -1)
        {
            PlayerDialogue playerDialogue = other.GetComponentInChildren<PlayerDialogue>();

            if (!playerDialogue.isDialogueEnabled)
                return;

            if (playerBarkMode == PlayerBarkMode.SequentialRecyle)
            {
                playerDialogue.Bark(remainingPlayerBarks[curPlayerBarkIndex]);

                curPlayerBarkIndex = (curPlayerBarkIndex + 1) % remainingPlayerBarks.Count;
            }
            else if (playerBarkMode == PlayerBarkMode.RandomRecyle)
            {
                playerDialogue.Bark(remainingPlayerBarks[Random.Range(0, remainingPlayerBarks.Count)]);
            }
            else if (playerBarkMode == PlayerBarkMode.Sequential && remainingPlayerBarks.Count > 0)
            {
                playerDialogue.Bark(remainingPlayerBarks[curPlayerBarkIndex]);

                //  remove bark
                remainingPlayerBarks.RemoveAt(curPlayerBarkIndex);
            }
            else if (playerBarkMode == PlayerBarkMode.Random && remainingPlayerBarks.Count > 0)
            {
                curPlayerBarkIndex = Random.Range(0, remainingPlayerBarks.Count);

                playerDialogue.Bark(remainingPlayerBarks[curPlayerBarkIndex]);

                //  remove bark
                remainingPlayerBarks.RemoveAt(curPlayerBarkIndex);
            }
        }

        if (remainingPlayerBarks.Count <= 0)
        {
            Destroy(gameObject);
        }
    }
}
