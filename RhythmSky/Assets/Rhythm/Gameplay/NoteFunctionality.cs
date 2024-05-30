using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Scores
{ 
    Perfect,
    Great, 
    Good,
    Bad,
    Miss
}


public class NoteFunctionality : MonoBehaviour
{
    bool reactRed = false;
    bool reactYellow = false;
    bool reactGreen = false;
    int laneNo = 0;

    public void SetLaneNo(int lane) {  laneNo = lane; }
    public int GetLaneNo() {  return laneNo; }

    public Scores React()
    {
        Debug.Log("react called");
        if (reactGreen && reactYellow) { return Scores.Great; }
        else if (reactYellow && reactRed) {  return Scores.Bad;  }
        else if (reactGreen) {  return Scores.Perfect; }
        else if (reactYellow) {  return Scores.Good; }
        else if (reactRed) { return Scores.Miss; }
        return Scores.Miss;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Red":
                reactRed = true;
                break;
            case "Yellow":
                reactYellow = true;
                break;
            case "Green":
                reactGreen = true;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Red":
                reactRed = false;
                break;
            case "Yellow":
                reactYellow = false;
                break;
            case "Green":
                reactGreen = false;
                break;
        }
    }
}
