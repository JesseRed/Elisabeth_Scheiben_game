using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;


public class CSVParsing : MonoBehaviour
{
    public TextAsset csvFileNonRandom; // Reference of CSV file
    public TextAsset csvFilePseudoRandom; // Reference of CSV file
    public int numberOfHeaderLines = 1;
    public char lineSeperater = '\n'; // It defines line seperate character
    public char fieldSeperator = ';'; // It defines field seperate chracter
    public List<Vector3Int> SeqNonRandom = new List<Vector3Int>();
    public List<Vector3Int> SeqRandom = new List<Vector3Int>();

    //public List<int> seqCol = new List<int>();
    //public List<int> seqPos = new List<int>();
    //public List<int> seqInt = new List<int>();
    //public List<int> seqNRCol = new List<int>();
    //public List<int> seqNRPos = new List<int>();
    //public List<int> seqNRInt = new List<int>();


    void Start()
    {
        //readDataNonRandom();
        //readDataRandom();
    }
    public List<Vector3Int> readDataNonRandom2()
    {
        string[] records = csvFileNonRandom.text.Split(lineSeperater);
        print("records laenge ... number an lines = " + records.Length.ToString());
        // erste und letze lassen wir aus
        // hier einfach alles 10 mal wiederholen, damit wir einen ganzen Block haben (120 Events)
        for (int j = 0; j < 10; j++)
        {
            for (int line_idx = numberOfHeaderLines; line_idx < records.Length - 1; line_idx++)
            {
                string[] fields = records[line_idx].Split(fieldSeperator);
                if (fields.Length == 3)
                {
                    //seqCol.Add(int.Parse(fields[0]));
                    //seqPos.Add(int.Parse(fields[1]));
                    //seqInt.Add(int.Parse(fields[2]));
                    Vector3Int vector3int = new Vector3Int(int.Parse(fields[0]), int.Parse(fields[1]), int.Parse(fields[2]));
                    SeqNonRandom.Add(vector3int);
                }
                else
                {
                    print("error mit folgendem String: " + records[line_idx] + "x " + fields.Length.ToString() + "x");
                }
            }
        }
        return SeqNonRandom;
    }

    public List<Vector3Int> readDataRandom2()
    {
        string[] records = csvFilePseudoRandom.text.Split(lineSeperater);
        //print("records laenge ... number an lines = " + records.Length.ToString());
        // erste und letze lassen wir aus
        for (int line_idx = numberOfHeaderLines; line_idx < records.Length - 1; line_idx++)
        {
            string[] fields = records[line_idx].Split(fieldSeperator);
            if (fields.Length == 3)
            {
                //                seqNRCol.Add(int.Parse(fields[0]));
                //                seqNRPos.Add(int.Parse(fields[1]));
                //                seqNRInt.Add(int.Parse(fields[2]));
                Vector3Int vector3int = new Vector3Int(int.Parse(fields[0]), int.Parse(fields[1]), int.Parse(fields[2]));
                SeqRandom.Add(vector3int);

            }
            else
            {
                print("error mit folgendem String: " + records[line_idx] + "x " + fields.Length.ToString() + "x");
            }
        }
        return SeqRandom;
    }

    // Read data from CSV file
    private void readDataNonRandom()
    {
        string[] records = csvFileNonRandom.text.Split(lineSeperater);
        print("records laenge ... number an lines = " + records.Length.ToString());
        // erste und letze lassen wir aus
        for (int line_idx = numberOfHeaderLines; line_idx < records.Length - 1; line_idx++)
        {
            string[] fields = records[line_idx].Split(fieldSeperator);
            if (fields.Length == 3)
            {
                //seqCol.Add(int.Parse(fields[0]));
                //seqPos.Add(int.Parse(fields[1]));
                //seqInt.Add(int.Parse(fields[2]));
                Vector3Int vector3int = new Vector3Int(int.Parse(fields[0]), int.Parse(fields[1]), int.Parse(fields[2]));
                SeqNonRandom.Add(vector3int);
            }
            else
            {
                print("error mit folgendem String: " + records[line_idx] + "x " + fields.Length.ToString() + "x");
            }
        }
    }
    private void readDataRandom()
    {
        string[] records = csvFilePseudoRandom.text.Split(lineSeperater);
        print("records laenge ... number an lines = " + records.Length.ToString());
        // erste und letze lassen wir aus
        for (int line_idx = numberOfHeaderLines; line_idx < records.Length - 1; line_idx++)
        {
            string[] fields = records[line_idx].Split(fieldSeperator);
            if (fields.Length == 3)
            {
//                seqNRCol.Add(int.Parse(fields[0]));
//                seqNRPos.Add(int.Parse(fields[1]));
//                seqNRInt.Add(int.Parse(fields[2]));
                Vector3Int vector3int = new Vector3Int(int.Parse(fields[0]), int.Parse(fields[1]), int.Parse(fields[2]));
                SeqNonRandom.Add(vector3int);

            }
            else
            {
                print("error mit folgendem String: " + records[line_idx] + "x " + fields.Length.ToString() + "x");
            }
        }
    }
}