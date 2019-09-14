using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results : MonoBehaviour {
    private List<int> itemNumber = new List<int>();
    private List<int> sequenceNumber = new List<int>();
    private List<int> buttonPressed = new List<int>();
    private List<int> buttonTarget = new List<int>();
    private List<int> timeAvailable = new List<int>();
    private List<int> timeToButtonPress = new List<int>();

    public void SetItemInformation(int itemNum, int seqNum, int bp, int bt, int ta, int timex)
    {
        itemNumber.Add(itemNum);
        sequenceNumber.Add(seqNum);
        buttonPressed.Add(bp);
        buttonTarget.Add(bt);
        timeAvailable.Add(ta);
        timeToButtonPress.Add(timex);
    }
}
