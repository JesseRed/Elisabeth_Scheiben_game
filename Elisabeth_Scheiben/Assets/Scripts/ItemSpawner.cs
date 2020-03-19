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
    //[SerializeField] Camera cam;
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


    void Start()
    {
        // cam = GetComponent<Camera>();
        // WPos00 = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        // WPos11 = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        // print(WPos00.x);
        //print("Spawner / Movements awake");
        
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
        //print("in Movements numBlocks = " + gameSession.playerData.paradigma.numBlocks);
        StartCoroutine(startPresentation());
        //print("in Start after StartCoroutine");
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
        //print(gameSession.playerData.paradigma.numScheiben);
        for (blockIdx = 0; blockIdx < gameSession.playerData.paradigma.numBlocks; blockIdx++)
        {
            timeBlockStart = Time.time;
            // print("block index = " + block_idx);
            
            //print("setting gameSession.hitsNumInBlock to 0");
            gameSession.hitsNumInBlock = 0;
            for (scheibeIdxInBlock = 0; scheibeIdxInBlock < gameSession.playerData.paradigma.numScheiben; scheibeIdxInBlock++)
            {
                
                // print("Sequence_index = " + scheibeIdxInBlock);
                float timedelay = Random.Range(gameSession.playerData.paradigma.MinimumInterScheibenDelay, gameSession.playerData.paradigma.MaximumInterScheibenDelay);
                //StartCoroutine(CreateScheibe());
                StartCoroutine(CreateScheibe());
                yield return new WaitForSeconds(timedelay);

            }
            // automatic adaptation of the level of difficulty if adaptive is 
            if (gameSession.playerData.paradigma.Adaptive==1){
                // adaptiere die Parameter auf einen Zielwert von 80% Treffer
                float cur_hit_rate = gameSession.hitsNumInBlock/gameSession.playerData.paradigma.numScheiben*100;
                if (cur_hit_rate >80){
                    // erniedrige die Scheibendauer um 10%
                    gameSession.playerData.paradigma.MinimumInterScheibenDelay = gameSession.playerData.paradigma.MinimumInterScheibenDelay * 0.9f;
                    gameSession.playerData.paradigma.MaximumInterScheibenDelay = gameSession.playerData.paradigma.MaximumInterScheibenDelay * 0.9f;                
                    gameSession.playerData.paradigma.MinimumScheibenExistenceDuration = gameSession.playerData.paradigma.MinimumScheibenExistenceDuration * 0.9f;
                    gameSession.playerData.paradigma.MaximumScheibenExistenceDuration = gameSession.playerData.paradigma.MaximumScheibenExistenceDuration * 0.9f;                
                }
                if (cur_hit_rate <60){
                    // erniedrige die Scheibendauer um 10%
                    gameSession.playerData.paradigma.MinimumInterScheibenDelay = gameSession.playerData.paradigma.MinimumInterScheibenDelay * 1.1f;
                    gameSession.playerData.paradigma.MaximumInterScheibenDelay = gameSession.playerData.paradigma.MaximumInterScheibenDelay * 1.1f;                
                    gameSession.playerData.paradigma.MinimumScheibenExistenceDuration = gameSession.playerData.paradigma.MinimumScheibenExistenceDuration * 1.1f;
                    gameSession.playerData.paradigma.MaximumScheibenExistenceDuration = gameSession.playerData.paradigma.MaximumScheibenExistenceDuration * 1.1f;                
                }
            }
            //gameSession.playerData.AddData(1, 2, "string", 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            //print(blockIdx + " " + gameSession.playerData.paradigma.numBlocks);
            //print("Itemspawner gameSession.hitsNumInBlock = " + gameSession.hitsNumInBlock);
            // warte bis alle Scheiben weg sind
            while (GameObject.FindGameObjectsWithTag ("Scheibe").Length >0){
                    // print("number of Scheiben = " + GameObject.FindGameObjectsWithTag ("Scheibe").Length);
                    yield return new WaitForSeconds(0.1f);
                }
            if (blockIdx < gameSession.playerData.paradigma.numBlocks - 1)
            {
                yield return new WaitForSeconds(1f);
                //print("in Anzeige");
                //print(gameSession.hitsNumInBlock);
                string summary = "Getroffene Scheiben: " + gameSession.hitsNumInBlock + "/" + gameSession.playerData.paradigma.numScheiben;
                summarytext.SetText(summary);
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
                yield return new WaitForSeconds(1f);
                string summary = "Getroffene Scheiben: " + gameSession.hitsNumInBlock + "/" + gameSession.playerData.paradigma.numScheiben;
                summarytext.SetText(summary);
                waitpanel.SetActive(true);
                starttext.SetText("Congratulations ...");
                countdowntext.SetText("Task finished");
            }
        }
        //print("startPresentation Coroutine am Ende");
        print("saving Data");
        gameSession.playerData.SaveDataAsCSV();
        yield return new WaitForSeconds(1);
        Application.Quit();
    }


    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator CreateScheibe()
    {
        float viewportPosx = Random.Range(0.05f, 0.95f);
        float viewportPosy = Random.Range(0.05f, 0.95f);
        float velocity = Random.Range(gameSession.playerData.paradigma.MinimumVelocity, gameSession.playerData.paradigma.MaximumVelocity);
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
        Status status = newScheibe.GetComponent<Status>();

        float timeLokaleScheibeInstatiate = Time.time;
        float durationOfScheibe = Random.Range(gameSession.playerData.paradigma.MinimumScheibenExistenceDuration, gameSession.playerData.paradigma.MaximumScheibenExistenceDuration);
        
        status.durationOfScheibe = durationOfScheibe;
        status.timeLokaleScheibeInstatiate = timeLokaleScheibeInstatiate;
        
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
                (float)Camera.main.WorldToViewportPoint(newScheibe.transform.position).x, // Scheiben Position
                (float)Camera.main.WorldToViewportPoint(newScheibe.transform.position).y, // Scheiben Position
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
        

        float scaled = 0;
        while (Time.time-timeLokaleScheibeInstatiate < durationOfScheibe)
//        for (float f = 0; f < durationOfScheibe; f = f + durationOfScheibe / 200)
        {

            //print("time= " + Time.time);

            if (newScheibe != null && !status.wasHit)// GetComponent<Collisions>().GetFreeze())
            {
                
                scaled = Mathf.Abs((Time.time-timeLokaleScheibeInstatiate) - (durationOfScheibe/2)) /(durationOfScheibe/2);
                scaled = ((scaled * -1.0f) + 1.0f); // hier ein Wert zwischen 0.0 und 1
                // ich moechte aber keine lineare Scalierung sondern eine Saettigung
                scaled = Mathf.Pow(scaled,0.4f);
                scaled = scaled * gameSession.playerData.paradigma.MaximumScheibenDiameter;
                // //print("index " + index.ToString());
                // if (Time.time-timeLokaleScheibeInstatiate < durationOfScheibe / 2) { index++; } else { index--; }
                //print("scale = " + scaled);
                // float scaled = 0.1f + ((0.9f / 100f) * (float)index);
                transf.localScale = new Vector3(scaled, scaled, 0.1f);
                status.scale = scaled;
                 //new WaitForSeconds(durationOfScheibe / 200);
            }
            // yield return null;
            yield return null;
        }
        if (newScheibe != null && !status.wasHit)
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
                (float)Camera.main.WorldToViewportPoint(rbnewScheibe.transform.position).x, // Scheiben Position
                (float)Camera.main.WorldToViewportPoint(rbnewScheibe.transform.position).y, // Scheiben Position
                (float)velocity ,
                (float)0f, // Scheiben Diameter
                (float)(Time.time - timeLokaleScheibeInstatiate), //  int existenceTime, 
                (float)durationOfScheibe, // maxExistenceTime
                numScheibenPresent);
           // der Nachteil ist, dass wir die Info ueber die Treffer erst am Ende der Sequenz haben
            Destroy(newScheibe);
        }
    }



    private Vector3 V2W(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(x, y, 10f));

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

    public GameObject get_nearest_Scheiben_data(Vector3 mpos, out float velocity, out int numScheibenPresent){
        GameObject[] Scheiben;
        GameObject nearest_Scheibe = null;
        Rigidbody2D nearest_rbScheibe = null;
        Scheiben = GameObject.FindGameObjectsWithTag("Scheibe");
        numScheibenPresent = Scheiben.Length;
        velocity = 0f;
        float distance = Mathf.Infinity;
        foreach (GameObject Scheibe in Scheiben)
        {
            Vector3 diff = Scheibe.transform.position - mpos; // mpos ist in Worldpos
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                nearest_Scheibe = Scheibe;
                nearest_rbScheibe = nearest_Scheibe.GetComponent<Rigidbody2D>();
                distance = curDistance;
            }
        }

        if (nearest_Scheibe != null){
            velocity = Mathf.Sqrt(Mathf.Pow(nearest_rbScheibe.velocity.x,2) + Mathf.Pow(nearest_rbScheibe.velocity.y,2));
            // print("velocity = " + velocity);
            // print("velocity x  = "  + nearest_rbScheibe.velocity.x);
            // print("velocity y  = "  + nearest_rbScheibe.velocity.y);
            // print("number of Scheiben present = " + numScheibenPresent);
        }
        return nearest_Scheibe;
        // return closest;

        // print("anzahl = " + Scheiben.Length);
        // Rigidbody2D nearest_rbScheibe = Scheiben[0].GetComponent<Rigidbody2D>();
        // float dist = Mathf.Infinity;
        // velocity = 0;
        // //List<GameObject> nearest_Scheiben = new List<GameObject>();
        // int idx = 0;
        // numScheibenPresent = 0;
        // float minimum_distance = 999999999999;
        // foreach (GameObject Scheibe in Scheiben){
        //     idx +=1;
        //     print(" found Scheibe Nummer " + idx );
        //     // test distance and identify the nearest Scheibe
        //     Rigidbody2D rbnewScheibe = Scheibe.GetComponent<Rigidbody2D>();
        //     float sx = rbnewScheibe.transform.position.x;
        //     float sy = rbnewScheibe.transform.position.y;
        //     numScheibenPresent +=1;
        //     dist = Mathf.Sqrt(Mathf.Pow(sx-mouseX,2) + Mathf.Pow(sy-mouseY,2));
        //     if (dist<minimum_distance){
        //         minimum_distance=dist;
        //         nearest_Scheibe = Scheibe;
        //         nearest_rbScheibe = rbnewScheibe;
        //     }
        // }
        // velocity = Mathf.Sqrt(Mathf.Pow(nearest_rbScheibe.velocity.x,2) + Mathf.Pow(nearest_rbScheibe.velocity.y,2));
    }
}


