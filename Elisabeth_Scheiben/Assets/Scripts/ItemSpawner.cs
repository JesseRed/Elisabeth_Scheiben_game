using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject ScheibePrefab;
    [SerializeField] GameObject Dotgray;
    [SerializeField] Rigidbody2D rbScheibePrefab;
    [SerializeField] Camera cam;
    [Header("Scheiben Geschwindigkeit")]
    // public float minTimeBetweenScheiben = 0.5f;
    // public float maxTimeBetweenScheiben = 2.5f;
    // public float durationOfScheibe = 2.5f;
    [Space(20)]
    // public int numberOfScheibenPerSequence = 10;
    public char fieldSeperator = ';'; // It defines field seperate chracter
    public Vector3 WPos00;
    public Vector3 WPos11;
    public GameObject CSVParsing;
    public GameSession gameSession;
    public GameObject waitpanel;
    public TextMeshProUGUI countdowntext;
    public GameObject countdownTMP;
    public TextMeshProUGUI starttext;
    public GameObject startTMP;
    public TextMeshProUGUI summarytext;
    public GameObject summaryTMP;
    //Block;
    //Scheiben;
    //MinimumInterScheibenDelay;
    //MaximumInterScheibenDelay;
    //MinimumVelocity;
    //MaximumVelocity;
    //MinimumScheibenExistenceDuration;
    //MaximumScheibenExistenceDuration;
    //Adaptive
    // Start is called before the first frame update
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
    private float timeExperimetStart;
    private float timeBlockStart;
    private int scheibeIdxInBlock;
    private int hitsNumInBlock;

    void Start()
    {
        WPos00 = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        WPos11 = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        print("Spawner / Movements awake");
        // hole mir als erstes die Sequencedaten die im csv File im Files verzeichis
        // gespeichert sind und an das CSVParsing object mit dem Script CSVParsing gekoppelt sind
        //SeqNonRandom = CSVParsing.GetComponent<CSVParsing>().readDataNonRandom2();
        //SeqRandom = CSVParsing.GetComponent<CSVParsing>().readDataRandom2();
        //ReadExperimentalDesign();
        Application.targetFrameRate = 300;
        //timeOfApplicationStart = Time.realtimeSinceStartup;
        //print("Fertig");
        gameSession = FindObjectOfType<GameSession>();
        countdownTMP = GameObject.Find("countdowntext (TMP)");
        countdowntext = countdownTMP.GetComponent<TextMeshProUGUI>();
        startTMP = GameObject.Find("starttext (TMP)");
        starttext = startTMP.GetComponent<TextMeshProUGUI>();
        summaryTMP = GameObject.Find("summarytext (TMP)");
        summarytext = summaryTMP.GetComponent<TextMeshProUGUI>();
        waitpanel = GameObject.Find("WaitPanel");
        print("in Movements numBlocks = " + gameSession.playerData.paradigma.numBlocks);
        StartCoroutine(startPresentation());
        print("in Start after StartCoroutine");
    }

    IEnumerator startPresentation()
    {
        waitpanel.SetActive(true);
        countdowntext.SetText("3");
        yield return new WaitForSeconds(1);
        countdowntext.SetText("2");
        yield return new WaitForSeconds(1);
        countdowntext.SetText("1");
        yield return new WaitForSeconds(1);
        countdowntext.SetText("0");
        yield return new WaitForSeconds(1);
        waitpanel.SetActive(false);
        timeExperimetStart = Time.time;
        print(gameSession.playerData.paradigma.numScheiben);
        for (blockIdx = 0; blockIdx < gameSession.playerData.paradigma.numBlocks; blockIdx++)
        {
            timeBlockStart = Time.time;
            // print("block index = " + block_idx);
            hitsNumInBlock = gameSession.playerData.paradigma.numScheiben;
            for (scheibeIdxInBlock = 0; scheibeIdxInBlock < gameSession.playerData.paradigma.numScheiben; scheibeIdxInBlock++)
            {
                
                // print("Sequence_index = " + scheibeIdxInBlock);
                float timedelay = Random.Range(gameSession.playerData.paradigma.MinimumInterScheibenDelay, gameSession.playerData.paradigma.MaximumInterScheibenDelay) / 1000;
                //StartCoroutine(CreateScheibe());
                StartCoroutine(CreateScheibe());
                yield return new WaitForSeconds(timedelay);

            }

            gameSession.playerData.AddData(1, 2, "string", 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            if (blockIdx < gameSession.playerData.paradigma.numBlocks - 1)
            {
                string summary = "Getroffene Scheiben: " + hitsNumInBlock + "/" + gameSession.playerData.paradigma.numScheiben;
                summarytext.SetText(summary);
                yield return new WaitForSeconds(4);
                waitpanel.SetActive(true);
                countdowntext.SetText("3");
                yield return new WaitForSeconds(1);
                countdowntext.SetText("2");
                yield return new WaitForSeconds(1);
                countdowntext.SetText("1");
                yield return new WaitForSeconds(1);
                countdowntext.SetText("0");
                yield return new WaitForSeconds(1);
                waitpanel.SetActive(false);
            }
            else
            {
                waitpanel.SetActive(true);
                starttext.SetText("Congratulations ...");
                countdowntext.SetText("Task finished");
            }
        }
        print("startPresentation Coroutine am Ende");
        print("saving Data");
        gameSession.playerData.SaveDataAsCSV();
    }


    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator CreateScheibe()
    {
        float viewportPosx = Random.Range(0.02f, 0.98f);
        float viewportPosy = Random.Range(0.02f, 0.98f);
        float velocity = Random.Range(gameSession.playerData.paradigma.MinimumVelocity, gameSession.playerData.paradigma.MaximumVelocity) / 10.0f;
        // aufteilung der Geschwindigkeit auf die X und die Y Richtung 
        float div = Random.Range(0.0f, 1f);
        float velInX = div * velocity;
        float velInY = (1.0f - div) * velocity;
        if (Random.value > 0.5f) { velInX = velInX * -1.0f; }
        if (Random.value > 0.5f) { velInY = velInY * -1.0f; }

        // float velInX = Random.Range(gameSession.playerData.paradigma.MinimumVelocity, gameSession.playerData.paradigma.MaximumVelocity) / 10;
        // float velInY = Random.Range(gameSession.playerData.paradigma.MinimumVelocity, gameSession.playerData.paradigma.MaximumVelocity) / 10;
        Vector3 newPos = V2W(viewportPosx, viewportPosy);
        // print("pos = " + newPos.ToString());
        GameObject newScheibe = Instantiate(ScheibePrefab);
        numScheibenPresent += 1;
        float timeLokaleScheibeInstatiate = Time.time;
        float durationOfScheibe = Random.Range(gameSession.playerData.paradigma.MinimumScheibenExistenceDuration, gameSession.playerData.paradigma.MaximumScheibenExistenceDuration) / 1000;
        // print("Scheibe wurde instanziert");
        Rigidbody2D rbnewScheibe = newScheibe.GetComponent<Rigidbody2D>();
        //Instantiate(rbScheibePrefab, newPos, Quaternion.identity);
        rbnewScheibe.transform.position = newPos;
        //rbnewScheibe = Instantiate(rbScheibePrefab, newPos, Quaternion.identity);
        rbnewScheibe.velocity = transform.TransformDirection(velInX, velInY, 0f);
        //CircleCollider2D circleCollider = rbnewScheibe.GetComponent<CircleCollider2D>();
        Transform transf = rbnewScheibe.transform;

        // Add the new Scheibe to the Player Data

            gameSession.playerData.AddData(
                blockIdx,
                (float)(Time.time - timeBlockStart),
                "instantiate",
                (int)0, // Hit
                scheibeIdxInBlock, // Number der Scheibe im aktuellen Block
                (float)Camera.main.ScreenToViewportPoint(Input.mousePosition).x, // Mouse position
                (float)Camera.main.ScreenToViewportPoint(Input.mousePosition).y, // Mouse position
                (float)Camera.main.ScreenToViewportPoint(rbnewScheibe.transform.position).x, // Scheiben Position
                (float)Camera.main.ScreenToViewportPoint(rbnewScheibe.transform.position).y, // Scheiben Position
                (float)velocity ,
                (float)0, // Scheiben Diameter
                (float)(Time.time - timeLokaleScheibeInstatiate), //  int existenceTime, 
                (float)durationOfScheibe, // maxExistenceTime
                numScheibenPresent);

        //circleCollider.radius = 0.5f;
        // starte klein
        transf.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        // wir teilen die duration durch 100 und machen die Scheibe dann in 100
        // Schritten gross und klein
        int index = 0;
        for (float f = 0; f < durationOfScheibe; f = f + durationOfScheibe / 200)
        {
            //print("f= " +f.ToString());
            if (newScheibe != null && !newScheibe.GetComponent<Collisions>().GetFreeze())
            {
                //print("index " + index.ToString());
                if (f < durationOfScheibe / 2) { index++; } else { index--; }

                float scaled = 0.1f + ((0.9f / 100f) * (float)index);
                transf.localScale = new Vector3(scaled, scaled, 0.1f);
                yield return null; //new WaitForSeconds(durationOfScheibe / 200);
            }
            // yield return null;
        }
        if (newScheibe != null && !newScheibe.GetComponent<Collisions>().GetFreeze())
        {

            // speichere das destroy in den Playerdata ab
            gameSession.playerData.AddData(
                blockIdx,
                (float)(Time.time - timeBlockStart),
                "destroy",
                (int)0, // Hit
                scheibeIdxInBlock, // Number der Scheibe im aktuellen Block
                (float)Camera.main.ScreenToViewportPoint(Input.mousePosition).x, // Mouse position
                (float)Camera.main.ScreenToViewportPoint(Input.mousePosition).y, // Mouse position
                (float)Camera.main.ScreenToViewportPoint(rbnewScheibe.transform.position).x, // Scheiben Position
                (float)Camera.main.ScreenToViewportPoint(rbnewScheibe.transform.position).y, // Scheiben Position
                (float)velocity ,
                (float)0, // Scheiben Diameter
                (float)(Time.time - timeLokaleScheibeInstatiate), //  int existenceTime, 
                (float)durationOfScheibe, // maxExistenceTime
                numScheibenPresent);
            hitsNumInBlock-=1; // wenn die scheibe nach Zeit zerstoert wird, dann wurde sie nicht getroffen und wir ziehen von allen Scheiben eine ab
            // der Nachteil ist, dass wir die Info ueber die Treffer erst am Ende der Sequenz haben
            Destroy(newScheibe);
        }
    }



    private Vector3 V2W(float x, float y)
    {
        return cam.ViewportToWorldPoint(new Vector3(x, y, 10f));

    }

    public int get_blockIdx(){
        return blockIdx;
    }

    public float get_timeBlockStart(){
        return timeBlockStart;
    }

    public int get_scheibeIdxInBlock(){
        return scheibeIdxInBlock;
    }

    public void get_nearest_Scheiben_data(float mouseX, float mouseY, out float velocity, out int numScheibenPresent){
        GameObject [] Scheiben;
        GameObject nearest_Scheibe;
        Scheiben = GameObject.FindGameObjectsWithTag("Scheibe");
        print("anzahl = " + Scheiben.Length);
        Rigidbody2D nearest_rbScheibe = Scheiben[1].GetComponent<Rigidbody2D>();
        float dist;
        velocity = 0;
        //List<GameObject> nearest_Scheiben = new List<GameObject>();
        int idx = 0;
        numScheibenPresent = 0;
        float minimum_distance = 999999999999;
        foreach (GameObject Scheibe in Scheiben){
            idx +=1;
            // test distance and identify the nearest Scheibe
            Rigidbody2D rbnewScheibe = Scheibe.GetComponent<Rigidbody2D>();
            float sx = rbnewScheibe.transform.position.x;
            float sy = rbnewScheibe.transform.position.y;
            numScheibenPresent +=1;
            dist = Mathf.Sqrt(Mathf.Pow(sx-mouseX,2) + Mathf.Pow(sy-mouseY,2));
            if (dist<minimum_distance){
                minimum_distance=dist;
                nearest_Scheibe = Scheibe;
                nearest_rbScheibe = rbnewScheibe;
            }
        }
        velocity = Mathf.Sqrt(Mathf.Pow(nearest_rbScheibe.velocity.x,2) + Mathf.Pow(nearest_rbScheibe.velocity.x,2));
    }

}
