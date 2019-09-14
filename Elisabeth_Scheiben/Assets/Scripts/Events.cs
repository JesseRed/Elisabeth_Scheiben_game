using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;


public class Events : MonoBehaviour
{
    public List<Vector3Int> SeqNonRandom = new List<Vector3Int>();
    public List<Vector3Int> SeqRandom = new List<Vector3Int>();
    public List<Vector3Int> activeSequence;

    public List<Vector3Int> resultsPresented = new List<Vector3Int>();
    public List<Vector3Int> resultsPressed = new List<Vector3Int>();
    public int[][] experimentConfig;
    private int itemIndex = 0; // nummer des Items innerhalb einer Sequenz
    private int seqIndex = 0; // nummer der Sequenz
    private int itemBlockIndex = 0; // globaler Item wert der bis 120 im ganzen Block hochzaehlt
    private int blockIndex = 0; // gibt die Nummer des Blocks an in dem wir uns befinden
    public int blockNum = 2; // target sind 6 anzahl der Bloecke die durchlaufen werden sollen
    public int seqNum = 2; // target sind 10
    public int[] expBlock;
    public int[] expTrial;
    public int[] expRan;
    private float timeOfSequenceStart;
    private float timeOfItemStart;
    private float timeOfApplicationStart;
    private bool eventButtonPressed = false;
    private float additionalWaitingTime = 0f;
    //public GameObject CSVParsing;
    public string relativeSavePath = "Assets/Data/";
    public string relativeReadPath = "Assets/ExperimentalDesign/";
    public string fileDesignName = "Experiment1_Day1.csv";
    public string fileSequenceNonRandomName = "SequenceNonRandom.csv";
    public string fileSequenceRandomName = "SequencesPseudoRand.csv";
    
    [SerializeField] float valueToAddToPresentationTime = 0.0f;
    [SerializeField] float itemTimeIfNotPressed = 2.5f;

    [SerializeField] GameObject[] Circles;
    [SerializeField] GameObject cross;

    public GameSession gameSession;
    [SerializeField] GameObject panelEnde;
    [SerializeField] GameObject panelEndeTutorial;
    // Speichern der Zuordnung Farbe zu Zahl
    //[SerializeField] Dictionary<string, int> colorDict = new Dictionary<string, int>();

    SpriteRenderer m_SpriteRenderer;
    private int buttonPressed;
    // Use this for initialization
    private List<int> resultButtonPressed = new List<int>();
    private int timeBetweenItemShownAndButton;
    public bool isTutorial;
    public bool isPause = false;
    public GameObject panelPause;
    void Awake()
    {
        print("awake");
        // hole mir als erstes die Sequencedaten die im csv File im Files verzeichis
        // gespeichert sind und an das CSVParsing object mit dem Script CSVParsing gekoppelt sind
        //SeqNonRandom = CSVParsing.GetComponent<CSVParsing>().readDataNonRandom2();
        //SeqRandom = CSVParsing.GetComponent<CSVParsing>().readDataRandom2();

        ReadExperimentalDesign();
        SeqNonRandom = ReadSequences(fileSequenceNonRandomName);
        SeqRandom = ReadSequences(fileSequenceRandomName);
        Application.targetFrameRate = 300;
        timeOfApplicationStart = Time.realtimeSinceStartup;
        //print("Fertig");
        gameSession = FindObjectOfType<GameSession>();
        isTutorial = gameSession.isTutorial;
        StartCoroutine(startPresentation());
        panelPause = GameObject.Find("PanelPause");
        panelPause.SetActive(false);
        //print(panelPause.name);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape") && isTutorial == false)
        {
            print("now quit");
            gameSession.playerData.SaveDataAsCSV();
            panelEnde.SetActive(true);
            StartCoroutine(WaitAndEnd(5));
            
        }
        if (Input.GetKeyDown("escape") && isTutorial == true)
        {
            print("now quit");
            gameSession.playerData.SaveDataAsCSV();
            panelEnde.SetActive(true);
           // StartCoroutine(WaitAndEnd(5));
//            print("end of tutorial");
  //          panelEndeTutorial.SetActive(true);
            StartCoroutine(WaitAndMenu(3));
        }
        // Pause setzen
        if (Input.GetKeyDown(KeyCode.Space)) {

            
//            GameObject panelPause = Resources.FindObjectsOfTypeAll("PanelPause");

            //            GameObject panelPause = GameObject.Find("PanelPause");
            if (isPause) {
                isPause = false;
                panelPause.SetActive(false);
            }
            else {
                isPause = true;
                panelPause.SetActive(true);
            }
        }


        if (Input.GetKeyDown("f"))
        {
            print("f (1ed) was pressed");
            RedButtonPressed();
        }
        if (Input.GetKeyDown("g"))
        {
            print("g (2yellow) was pressed");
            YelloButtonPressed();
        }
        if (Input.GetKeyDown("h"))
        {
            print("h (3blue) was pressed");
            BlueButtonPressed();
        }
        if (Input.GetKeyDown("j"))
        {
            print("j (4green) was pressed");
            GreenButtonPressed();
        }
        //print(Time.time.ToString());
    }

    private IEnumerator WaitAndEnd(int sec)
    {
        yield return new WaitForSeconds(sec);
        Application.Quit();
    }
    
    private IEnumerator WaitAndMenu( int sec)
    {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene("MenuScene");
    }
        


    private IEnumerator startPresentation()
    {
        // Hier kommt erst das Intro
        int sec = 3;
        GameObject panelIntro = GameObject.Find("PanelIntro");
        TextMeshProUGUI text = panelIntro.GetComponentInChildren<TextMeshProUGUI>();
        panelIntro.SetActive(true);
        for (int i = sec; i >= 0; i--)
        {
            text.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        panelIntro.SetActive(false);
        // Intro Ende
        // 

        for (blockIndex = 0; blockIndex < expBlock.Length; blockIndex++)
        {
            if (expRan[blockIndex] == 1)
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

    //IEnumerator startIntro(int sec) {
    //    GameObject panelIntro = GameObject.Find("PanelIntro");
    //    TextMeshProUGUI text = panelIntro.GetComponentInChildren<TextMeshProUGUI>();
    //    panelIntro.SetActive(true);
    //    for (int i=sec; i>=0; i--) {
    //        text.text = i.ToString();
    //        yield return new WaitForSeconds(1);
    //    }
    //    panelIntro.SetActive(false);
        
    //}

    IEnumerator startBlock()
    {
        // Schleife ueber alle Sequenzen die wir presentieren wollen
        // ein Block besteht entsprechend dem paper von Boyed aus 10 Sequencen a 12 items also 120 Events
        // nun ein Block
        // der itemBlockIndex ist der entscheidende Index 
        itemBlockIndex = 0;
        for (seqIndex = 0; seqIndex < expTrial[blockIndex]; seqIndex++)
        {
            //            StartCoroutine(presentSequence(SeqNonRandom.GetRange(0, 12)));
            yield return StartCoroutine(presentSequence(activeSequence));
            print("sequence Index = " + seqIndex);
            // flashing of the cross
            yield return StartCoroutine(flashingOfTheCross(250));
            while (isPause)
            {
                yield return null;
            }
        }
        // ein Block ist abgeschlossen

    }

    // Eine ganze Sequence
    IEnumerator presentSequence(List<Vector3Int> sequence)
    {

        timeOfSequenceStart = Time.realtimeSinceStartup;
        for (int itemIndex = 0; itemIndex < 12; itemIndex++)
        {
            while (isPause)
            {
                yield return null;
            }
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

            yield return StartCoroutine(presentItem(targetCircle, targetColor, targetTime));
            if (buttonPressed == -1)
            {
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

            gameSession.playerData.AddData(blockIndex, seqIndex, itemIndex, buttonPressed, targetColor, targetCircle, targetTime, timeBetweenItemShownAndButton);
            itemBlockIndex++;
            // Abfrage der Pausefunktion

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
        timeBetweenItemShownAndButton = Mathf.RoundToInt((Time.realtimeSinceStartup - timeOfItemStart) * 1000);

    }


    IEnumerator flashingOfTheCross(int millisec)
    {
        print("now flashing the cross");
        int i = 0;
        while (i < millisec)
        {
            cross.SetActive(false);
            yield return new WaitForSeconds(0.050f);
            cross.SetActive(true);
            yield return new WaitForSeconds(0.050f);
            i = i + 100;
        }
    }


    private void ReadExperimentalDesign()
    {
        print("Reading experimental Design");
        string file = relativeReadPath + fileDesignName;

        List<int[]> config = new List<int[]>();
        using (StreamReader sr = new StreamReader(file))
        {
            // Block; Trial; Random
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] fields = sr.ReadLine().Split(';');

                if (fields.Length > 0)
                {
                    int[] tmp = new int[fields.Length];
                    for (int i = 0; i < fields.Length; i++)
                    {
                        //print("gelesen:" + fields[i]);
                        tmp[i] = int.Parse(fields[i]);

                    }
                    config.Add(tmp);

                }

            }
        }
        int t = config.Count;
        expBlock = new int[t];
        expTrial = new int[t];
        expRan = new int[t];

        for (int i = 0; i < config.Count; i++)
        {
            //  print("i= " + i.ToString() + " configCount = " + config.Count.ToString());

            expBlock[i] = config[i][0];
            expTrial[i] = config[i][1];
            expRan[i] = config[i][2];
        }
        experimentConfig = config.ToArray();
    }



    private List<Vector3Int> ReadSequences(string filename)
    {
        List<Vector3Int> Sequence = new List<Vector3Int>();
        print("Reading Sequences");
        string file = relativeReadPath + filename;

       // List<int[]> config = new List<int[]>();
        using (StreamReader sr = new StreamReader(file))
        {
            // Block; Trial; Random
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] fields = sr.ReadLine().Split(';');

                if (fields.Length == 3)
                {
                    Vector3Int vector3int = new Vector3Int(int.Parse(fields[0]), int.Parse(fields[1]), int.Parse(fields[2]));
                    Sequence.Add(vector3int);
                }

            }
        }

        return Sequence;


    }






}

