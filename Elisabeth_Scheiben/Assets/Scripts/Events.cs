using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Events : MonoBehaviour {
    public List<Vector3Int> SeqNonRandom = new List<Vector3Int>();
    public List<Vector3Int> SeqRandom = new List<Vector3Int>();
    public List<Vector3Int> activeSequence ;

    public List<Vector3Int> resultsPresented = new List<Vector3Int>();
    public List<Vector3Int> resultsPressed = new List<Vector3Int>();

    private int itemIndex= 0; // nummer des Items innerhalb einer Sequenz
    private int seqIndex = 0; // nummer der Sequenz
    private int itemBlockIndex = 0; // globaler Item wert der bis 120 im ganzen Block hochzaehlt
    private int blockIndex = 0; // gibt die Nummer des Blocks an in dem wir uns befinden
    public int blockNum = 2; // target sind 6 anzahl der Bloecke die durchlaufen werden sollen
    public int seqNum = 2; // target sind 10
    public int[] expBlock;
    [SerializeField] int[] expTrial;
    [SerializeField] int[] expRan;
    private float timeOfSequenceStart;
    private float timeOfItemStart;
    private float timeOfApplicationStart;
    private bool eventButtonPressed = false;
    private float additionalWaitingTime = 0f;
    public GameObject CSVParsing;
    public string relativeSavePath = "/Assets/Data/";
    public string relativeReadPath = "/Assets/ExperimentalDesign/";
    public string fileDesignName = "Experiment1_Day1";

    [SerializeField] float valueToAddToPresentationTime=0.0f;
    [SerializeField] float itemTimeIfNotPressed = 2.5f;

    [SerializeField] GameObject[] Circles;
    [SerializeField] GameObject cross;

    public GameSession gameSession;
    [SerializeField] GameObject panelEnde;
// Speichern der Zuordnung Farbe zu Zahl
    //[SerializeField] Dictionary<string, int> colorDict = new Dictionary<string, int>();

    SpriteRenderer m_SpriteRenderer;
    private int buttonPressed;
    // Use this for initialization
    private List<int> resultButtonPressed = new List<int>();
    private int timeBetweenItemShownAndButton;
        private int blockIdx;
        private int timeSinceBlockStart;
        private string eventType;
        private int isHit;
        private int scheibenNum;
        private int posXmouse;
        private int posYmouse;
        private int posXScheibe;
        private int posYScheibe;
        private int velocity;
        private int scheibenDiameter;
        private int existenceTime;
        private int maxExistenceTime;
        private int numScheibenPresent;


    void Awake () {
        print("awake");
        // hole mir als erstes die Sequencedaten die im csv File im Files verzeichis
        // gespeichert sind und an das CSVParsing object mit dem Script CSVParsing gekoppelt sind
        SeqNonRandom = CSVParsing.GetComponent<CSVParsing>().readDataNonRandom2();
        SeqRandom = CSVParsing.GetComponent<CSVParsing>().readDataRandom2();
        ReadExperimentalDesign();
        Application.targetFrameRate = 300;
        timeOfApplicationStart = Time.realtimeSinceStartup;
        //print("Fertig");
        gameSession = FindObjectOfType<GameSession>();
        StartCoroutine(startPresentation());

    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown("escape"))
        {
            print("now quit");
            Application.Quit();
        }

        if (Input.GetKeyDown("f"))
        {
            print("f was pressed");
            RedButtonPressed();
        }
        if (Input.GetKeyDown("g"))
        {
            print("g was pressed");
            YelloButtonPressed();
        }
        if (Input.GetKeyDown("h"))
        {
            print("h was pressed");
            BlueButtonPressed();
        }
        if (Input.GetKeyDown("j"))
        {
            print("j was pressed");
            GreenButtonPressed();
        }
       //print(Time.time.ToString());
    }

    private void ReadExperimentalDesign()
    {
        print("Reading experimental Design");
        int idx = 0;
        string file = relativeReadPath + fileDesignName;
        string line;
        using (StreamReader sr = new StreamReader(file))
        {
            // Block; Trial; Random
            sr.ReadLine();
            while (true)
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }
                string[] fields = line.Split(';');
                expBlock[idx] = int.Parse(fields[0]);
                expTrial[idx] = int.Parse(fields[1]);
                expRan[idx] = int.Parse(fields[2]);
                idx++;
            }

        }

    }

    private IEnumerator startPresentation()
    {
        for (blockIndex = 0; blockIndex<blockNum; blockIndex++)
        {
            if(blockIndex == 0)
            {
                activeSequence = SeqRandom;
            }
            else
            {
                activeSequence = SeqNonRandom;
            }
            yield return StartCoroutine(startBlock());
            print("block Index = " + blockIndex);
        }
        // alles abgeschlossen  ... speichere nun die Daten
        gameSession.playerData.SaveDataAsCSV();
        panelEnde.SetActive(true);
    }

    IEnumerator startBlock()
    {
        // Schleife ueber alle Sequenzen die wir presentieren wollen
        // ein Block besteht entsprechend dem paper von Boyed aus 10 Sequencen a 12 items also 120 Events
        // nun ein Block
        // der itemBlockIndex ist der entscheidende Index 
        itemBlockIndex = 0;
        for (seqIndex = 0; seqIndex < seqNum; seqIndex++)
        {
//            StartCoroutine(presentSequence(SeqNonRandom.GetRange(0, 12)));
            yield return StartCoroutine(presentSequence(activeSequence));
            print("sequence Index = " + seqIndex);
            // flashing of the cross
            yield return StartCoroutine(flashingOfTheCross(250));
        }
        // ein Block ist abgeschlossen

    }

    // Eine ganze Sequence
    IEnumerator presentSequence(List<Vector3Int> sequence)
    {

        timeOfSequenceStart = Time.realtimeSinceStartup;
        for (int itemIndex = 0; itemIndex < 12; itemIndex++)
        {
            // hier wird ein Item praesentiert und auf eine Antwort gewartet
            print(itemIndex.ToString() + " und Block Index = " + itemBlockIndex.ToString());
            // hier muessen nun mittels seqIndex die richtigen Infos fuer Position und Farbe geholt werden
            buttonPressed = -1;
            int targetColor = sequence[itemBlockIndex][0];
            int targetCircle = sequence[itemBlockIndex][1];
            int targetTime = sequence[itemBlockIndex][2];
            print("itemBlockIndex = " + itemBlockIndex.ToString() + "  Inhalt:" + targetColor.ToString() + " " + targetCircle.ToString() + " " + targetTime.ToString());
            eventButtonPressed = false; // beschreibt das allegmeine Event das irgendein Button gedrueckt wird
            additionalWaitingTime = 0f;
            timeBetweenItemShownAndButton = 0; // Mathf.RoundToInt((Time.realtimeSinceStartup - timeOfItemStart) * 1000);

            yield return StartCoroutine(presentItem(targetCircle, targetColor,targetTime));
            if (buttonPressed==-1){
                print("no button was pressed");
                yield return StartCoroutine(WaitForPress());
                if (buttonPressed == -1)
                {
                    RegisterButton(-1);
                    timeBetweenItemShownAndButton = Mathf.RoundToInt((itemTimeIfNotPressed) * 1000); 
                }
                
            }
            m_SpriteRenderer.color = Color.white;
            // save Result in Results Class
            //results.SetItemInformation( itemIndex, seqIndex,itemBlockIndex, buttonPressed, targetColor,
            //    itemWaitTime, timeBetweenItemShownAndButton);
            // nun wird der itemBlockIndex erhoeht
            resultsPresented.Add(sequence[itemBlockIndex]);
            resultsPressed.Add(new Vector3Int(blockIndex, buttonPressed, timeBetweenItemShownAndButton));
            // an dieser Stelle haben ist die presentation eines Items abgeschlossen und wir haben alle
            // Informationen um dieses Item und die Reaktion darauf zu speichern
            
            gameSession.playerData.AddData( blockIdx,  timeSinceBlockStart, eventType,  isHit,  scheibenNum,  posXmouse, posYmouse, posXScheibe, posYScheibe, velocity, scheibenDiameter, existenceTime, maxExistenceTime, numScheibenPresent);
            itemBlockIndex++;
        }
       
    }

    IEnumerator presentItem(int circleNumber, int colorNumber, int targetTime)
    {
        //print("entering presentItem");
        // circleNumber von 0 bis 5 , benannt im Uhrzeigersinn, start auf 10 Uhr
        // Color number 0 = red, 1 = yellow, 2= blue, 3 = green
        timeOfItemStart = Time.realtimeSinceStartup;
        //itemWaitTime = UnityEngine.Random.Range(minItemPresentationTime, maxItemPresentationTime);
        float itemWaitTime = (float)targetTime / 1000;
        m_SpriteRenderer = Circles[circleNumber].GetComponent<SpriteRenderer>();

        // waehle color aus
        switch (colorNumber)
        {
            case 0:
                m_SpriteRenderer.color = Color.red;
                break;
            case 1:
                m_SpriteRenderer.color = Color.yellow;
                break;
            case 2:
                m_SpriteRenderer.color = Color.blue;
                break;
            case 3:
                m_SpriteRenderer.color = Color.green;
                break;
            default:
                print("this line should never be reached in presentItem with colorNumber=" + colorNumber.ToString());
                break;
               
        }
        print("present item itemWaitTime = " + itemWaitTime.ToString());
        yield return new WaitForSeconds(itemWaitTime);//WaitForSecondsRealtime(itemWaitTime);
    }

    IEnumerator WaitForPress()
    {
        float timeToWait = itemTimeIfNotPressed - (Time.realtimeSinceStartup - timeOfItemStart);

        print("Additional waiting time .... to max Wait = " + timeToWait.ToString());
        float td = 0f;
        float t_init = Time.realtimeSinceStartup;
        while (td < timeToWait)
        {
            yield return new WaitForSeconds(0.002f);
            // wenn der button gepresst wurde
            if (buttonPressed != -1)
            {
                additionalWaitingTime = td;
                break;
            }
            td = Time.realtimeSinceStartup - t_init;
        }
        additionalWaitingTime = timeToWait;
    }

 

    public void RedButtonPressed() { RegisterButton(0); }
    public void YelloButtonPressed() { RegisterButton(1); }
    public void BlueButtonPressed() { RegisterButton(2); }
    public void GreenButtonPressed() { RegisterButton(3); }



    public void RegisterButton(int colNumber)
    { //0 = red, 1 = yellow, 2= blue, 3 = green
        buttonPressed = colNumber;
        print("In Block " + itemBlockIndex.ToString() + "pressed: " + buttonPressed.ToString());
        //resultButtonPressed[globalIndex] = 1;
        timeBetweenItemShownAndButton = Mathf.RoundToInt((Time.realtimeSinceStartup-timeOfItemStart)*1000);

    }


    IEnumerator flashingOfTheCross(int millisec)
    {
        print("now flashing the cross");
        int i = 0;
        while (i<millisec)
        { 
            cross.SetActive(false);
            yield return new WaitForSeconds(0.050f);
            cross.SetActive(true);
            yield return new WaitForSeconds(0.050f);
            i = i + 100; 
        }
    }

}

