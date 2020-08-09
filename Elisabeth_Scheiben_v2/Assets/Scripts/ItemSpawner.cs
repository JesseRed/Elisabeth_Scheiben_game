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
    private bool isBetweenBlocks;
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
    private float timeBlockPause;
    private float timeStartPause;
    private float timeEndPause;
    private int scheibeIdxInBlock;
    private int overall_hits = 0;
    private float timeToMovementInitiation = 0;
    private float cursorPathError = 0;
    private float finalPosError = 0; 
    private float maxMovementVelocity = 0;
    private float specialPosForDistancingX = 0;
    private float specialPosForDistancingY = 0;
    public int last_scheibe_hit = 0;
    private bool isPause = false;
    
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
        // die num_Scheiben werden hier neu definiert, da 
        // dies die Zielzahl der zu treffenden Scheiben ist
        // das gilt aber nciht fuer den FAll, dass wir nach Zeit arbeiten
        int num_Scheiben = gameSession.playerData.paradigma.numScheiben;
        int num_presented_Scheiben_in_block = 0;
        float timedelay = 0.05f;

        waitpanel.SetActive(true);
 
        for (int i=gameSession.playerData.paradigma.initialCountdown; i>0 ; i--){
            countdowntext.SetText(i.ToString());
            yield return new WaitForSeconds(1);
        }
        waitpanel.SetActive(false);
        timeExperimetStart = Time.time;

        //print(gameSession.playerData.paradigma.numScheiben);
        for (blockIdx = 0; blockIdx < gameSession.playerData.paradigma.numBlocks; blockIdx++)
        {
            num_presented_Scheiben_in_block = 0;
            timeBlockStart = Time.time;
            timeBlockPause = 0f;
            // print("block index = " + block_idx);
            
            //print("setting gameSession.hitsNumInBlock to 0");
            gameSession.hitsNumInBlock = 0;

            // wenn die Dauer eines Blocks ueber die Anzahl an Scheiben definiert ist
            if (gameSession.playerData.paradigma.timePerBlock<0){
            for (scheibeIdxInBlock = 0; scheibeIdxInBlock < gameSession.playerData.paradigma.numScheiben; scheibeIdxInBlock++)
            {
                
                // print("Sequence_index = " + scheibeIdxInBlock);
                if (gameSession.playerData.paradigma.InstantNewScheibeAfterHit == 0){
                    timedelay = Random.Range(gameSession.playerData.paradigma.MinimumInterScheibenDelay, gameSession.playerData.paradigma.MaximumInterScheibenDelay);
                    //StartCoroutine(CreateScheibe());
                    StartCoroutine(CreateScheibe());
                    yield return new WaitForSeconds(timedelay);
                }
                if (gameSession.playerData.paradigma.InstantNewScheibeAfterHit == 1){
                    last_scheibe_hit = 0;
                    StartCoroutine(CreateScheibe());
                    while (last_scheibe_hit == 0){
                        yield return new WaitForSeconds(timedelay);
                    }
                }
                
                num_presented_Scheiben_in_block = gameSession.playerData.paradigma.numScheiben;
            }
            }else{
                print("time");
                // wenn die Dauer eines Blocks ueber die ZEit definiert ist
                float start_time = Time.time;
                float time_left_in_block = gameSession.playerData.paradigma.timePerBlock;
                bool is_block_continue = true;
                print("time left in block  vor der schleife = " + time_left_in_block);
                while (is_block_continue)
            {
                time_left_in_block = gameSession.playerData.paradigma.timePerBlock- (Time.time-start_time) + timeBlockPause;
                print("time left in block = " + time_left_in_block);
                print("scheibe num in block " + num_presented_Scheiben_in_block);
                // print("Sequence_index = " + scheibeIdxInBlock);
                if (gameSession.playerData.paradigma.InstantNewScheibeAfterHit == 0){
                    timedelay = Random.Range(gameSession.playerData.paradigma.MinimumInterScheibenDelay, gameSession.playerData.paradigma.MaximumInterScheibenDelay);
                    //StartCoroutine(CreateScheibe());
                    StartCoroutine(CreateScheibe());
                   if (time_left_in_block<timedelay){
                            timedelay = time_left_in_block;
                   }
                   yield return new WaitForSeconds(timedelay);
                }
                if (gameSession.playerData.paradigma.InstantNewScheibeAfterHit == 1){
                    last_scheibe_hit = 0;
                    StartCoroutine(CreateScheibe());
                    while (last_scheibe_hit == 0){

                        yield return new WaitForSeconds(timedelay);
                        time_left_in_block = gameSession.playerData.paradigma.timePerBlock- (Time.time-start_time) + timeBlockPause;
                        if (time_left_in_block<=0){
                            break;
                        }
                    }
                }
                time_left_in_block = gameSession.playerData.paradigma.timePerBlock- (Time.time-start_time) + timeBlockPause;
                if (time_left_in_block<=0){is_block_continue=false;}
                num_presented_Scheiben_in_block+=1;
            }
            }

            // automatic adaptation of the level of difficulty if adaptive is 
            if (gameSession.playerData.paradigma.Adaptive==1){
                // adaptiere die Parameter auf einen Zielwert von 80% Treffer
                float cur_hit_rate = gameSession.hitsNumInBlock/num_presented_Scheiben_in_block*100; //gameSession.playerData.paradigma.numScheiben*100;
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
                string summary = "press space for pause ... Getroffene Scheiben: " + gameSession.hitsNumInBlock + "/" + num_presented_Scheiben_in_block; //gameSession.playerData.paradigma.numScheiben;
                summarytext.SetText(summary);
                waitpanel.SetActive(true);
                isBetweenBlocks = true;
                for (int i=gameSession.playerData.paradigma.initialCountdown; i>0 ; i--){
                    countdowntext.SetText(i.ToString());

                    yield return new WaitForSeconds(1);
                    while (isPause){
                        yield return null;
                    }
                }
                waitpanel.SetActive(false);
                isBetweenBlocks = false;
            }
            else
            {
                yield return new WaitForSeconds(1f);
                string summary = "Getroffene Scheiben: " + gameSession.hitsNumInBlock + "/" + num_presented_Scheiben_in_block ; //gameSession.playerData.paradigma.numScheiben;
                summarytext.SetText(summary);
                waitpanel.SetActive(true);
                starttext.SetText("Congratulations ...");
                countdowntext.SetText("Task finished");
            }
            gameSession.hitsNumInGame = gameSession.hitsNumInGame + gameSession.hitsNumInBlock;
            gameSession.nonHitsNumInGame = gameSession.nonHitsNumInGame + gameSession.nonHitsNumInBlock;
            gameSession.scheibenNumInGame = gameSession.scheibenNumInGame + num_presented_Scheiben_in_block; //gameSession.playerData.paradigma.numScheiben;
            print("saving Data");
            gameSession.playerData.SaveDataAsCSV();
        }
        //print("startPresentation Coroutine am Ende");
        //print("saving Data");
        //gameSession.playerData.SaveDataAsCSV();
        yield return new WaitForSeconds(1);
        countdowntext.SetText("Weiter with SpaceBar");
        while(!Input.GetKey(KeyCode.Space))
        {
            yield return null;
        }
        starttext.SetText("Scheibenzahl = " + gameSession.scheibenNumInGame.ToString());
        summarytext.SetText("getrof. Scheib.: " + gameSession.hitsNumInGame.ToString() + " NonTreffer " + gameSession.nonHitsNumInGame.ToString());
        yield return new WaitForSeconds(1f);
        while(!Input.GetKey(KeyCode.Space))
        {
            yield return null;
        }
        // countdowntext.SetText("the end");
        // while(!Input.GetKey(KeyCode.Escape))
        // {
        //     yield return new WaitForSeconds(1f);
        // }
        Application.Quit();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (isBetweenBlocks){
            if (Input.GetKeyDown(KeyCode.Space)) {
            if (isPause) {
                isPause = false;
                //waitpanel.SetActive(false);
                timeEndPause = Time.time;
                timeBlockPause += timeEndPause - timeStartPause;                
            }
            else {
                isPause = true;
                timeStartPause = Time.time;
                //waitpanel.SetActive(true);
                countdowntext.SetText("pause ... weiter mit space");
            }
        }
        }
    }


    IEnumerator CreateScheibe()
    {
        // print("create Scheibe");
        float eukl_dist = 0;
        float min_dist = gameSession.playerData.paradigma.MinimumScheibenDistance;
        float viewportPosx= -1;
        float viewportPosy= -1;
        float x_dist;
        float y_dist;

        // brute force ... aber mir ist erstmal nix besseres eingefallen        
        while (eukl_dist < min_dist){
            viewportPosx = Random.Range(0.05f, 0.95f);  
            viewportPosy = Random.Range(0.05f, 0.95f);
            x_dist = Mathf.Pow(viewportPosx - specialPosForDistancingX,2);
            y_dist = Mathf.Pow(viewportPosy - specialPosForDistancingY,2);
            eukl_dist = Mathf.Sqrt(x_dist + y_dist);
            // print("eukl_dist = " + eukl_dist);
        }
        specialPosForDistancingX = viewportPosx;
        specialPosForDistancingY = viewportPosy;        
        // float x_0_to = specialPosForDistancingX - min_dist;
        // float x_to_1 = specialPosForDistancingX + min_dist;
        // float y_0_to = specialPosForDistancingY - min_dist;
        // float y_to_1 = specialPosForDistancingY + min_dist;
        // float new_x = Random.Range(0.05f, 0.95f-(min_dist*2));
        // float new_y = Random.Range(0.05f, 0.95f-(min_dist*2));
        // if (new_x>x_0_to){ new_x += min_dist*2;};
        // if (new_y>y_0_to){ new_y += min_dist*2;};
        // print("new_x = " + viewportPosx);
        // print("new_y = " + viewportPosy);


        
//        float viewportPosx = Random.Range(0.05f, 0.95f);
//        float viewportPosy = Random.Range(0.05f, 0.95f);
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
        status.mousePosStartX = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
        status.mousePosStartY = Camera.main.ScreenToViewportPoint(Input.mousePosition).y;
        status.scheibePosStartX = Camera.main.WorldToViewportPoint(newScheibe.transform.position).x;
        status.scheibePosStartY = Camera.main.WorldToViewportPoint(newScheibe.transform.position).y;
        // Add the new Scheibe to the Player Data
           gameSession.playerData.AddData(
                blockIdx,
                (float)(Time.time - timeBlockStart - timeBlockPause),
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
                numScheibenPresent,
                (float) timeToMovementInitiation, 
                (float) cursorPathError,
                (float) finalPosError, 
                (float) maxMovementVelocity
                );

        //circleCollider.radius = 0.5f;
        // starte klein
        transf.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        // wir teilen die duration durch 100 und machen die Scheibe dann in 100
        // Schritten gross und klein
        

        float scaled = 0;
        float fact_to_scale = gameSession.playerData.paradigma.MinimumScheibenDiameter / gameSession.playerData.paradigma.MaximumScheibenDiameter;
        while (Time.time-timeLokaleScheibeInstatiate < durationOfScheibe)
//        for (float f = 0; f < durationOfScheibe; f = f + durationOfScheibe / 200)
        {

            //print("time= " + Time.time);
            
            if (newScheibe != null && !status.wasHit)// GetComponent<Collisions>().GetFreeze())
            {
                
                scaled = Mathf.Abs((Time.time-timeLokaleScheibeInstatiate) - (durationOfScheibe/2)) /(durationOfScheibe/2);
                scaled = ((scaled * -1.0f) + 1.0f); // hier ein Wert zwischen 0.0 und 1
                // wenn MinimumScheibenDiameter gesetzt ist darf es aber nicht 0 sein sondern groesser
                
                // ich moechte aber keine lineare Scalierung sondern eine Saettigung
                scaled = Mathf.Pow(scaled,0.4f);
                float normalizedValue = Mathf.InverseLerp(0, 1, scaled);
                scaled = Mathf.Lerp(gameSession.playerData.paradigma.MinimumScheibenDiameter, gameSession.playerData.paradigma.MaximumScheibenDiameter, normalizedValue);

//                scaled = scaled * gameSession.playerData.paradigma.MaximumScheibenDiameter;
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
            print("save destroy with schiebeIdxInBlock = " + scheibeIdxInBlock);
            gameSession.playerData.AddData(
                blockIdx,
                (float)(Time.time - timeBlockStart - timeBlockPause),
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
                numScheibenPresent,
                (float) timeToMovementInitiation, 
                (float) cursorPathError,
                (float) finalPosError, 
                (float) maxMovementVelocity
                );
           // der Nachteil ist, dass wir die Info ueber die Treffer erst am Ende der Sequenz haben
            Destroy(newScheibe);
            last_scheibe_hit = 1;
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
    public float get_timeBlockPause(){
        return timeBlockPause;
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
        print("get_nearest_Scheiben_data ... numScheibenPresent = " + numScheibenPresent);
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


