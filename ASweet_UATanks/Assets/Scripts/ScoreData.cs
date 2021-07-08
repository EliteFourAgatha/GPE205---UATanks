using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Show class details in inspector
[System.Serializable]
//Use IComparable interface to tell C# this class has a method (CompareTo) to compare its data
// to that of other instances of this class
public class ScoreData : IComparable<ScoreData>
{
    public float score;
    public string name;
    List<ScoreData> scoreList;
    void Start()
    {
        //Add up all score objects
        scoreList.Sort();
        //Reverse list to get highest scores instead of lowest
        scoreList.Reverse();
        //Only return top 3 scores
        scoreList = scoreList.GetRange(0, 2);
    }
    public int CompareTo(ScoreData other) 
    {
        if(other == null)
        {
            score = 1;
        }
        if(this.score > other.score)
        {
            return 1;
        }
        if(other.score > this.score)
        {
            return -1;
        }
        return 0;
    }
}
