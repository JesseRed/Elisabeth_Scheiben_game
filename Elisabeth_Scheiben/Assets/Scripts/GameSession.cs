using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class GameSession : MonoBehaviour
{
    public PlayerData playerData;
    public bool isTutorial = false;
    public bool isInitialized = false;
    

    public string relativeSavePath = "Data";
    public string relativeReadPath = "ExperimentalDesign";
    public string fileDesignName = "Experiment1_Day1.csv";
    public Paradigma paradigma;
    private void Awake()
    {
        SetUpSingleton();
        //Debug.Log("Awake");
    }

    private void SetUpSingleton()
    {
        int numberOfGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numberOfGameSessions > 1)
        {
            print("already initialized");
            //GameObject MM = FindObject


            Destroy(gameObject);

        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayerDataStructure()
    {
        // hole mir die Daten und initialisiere die Classe mit den PlayerData
        //GameObject tmp = FindObjectOfType<VornameText>;
        //GameObject tmp = GameObject.Find("VornameText");
        //TextMeshProUGUI tmp2 = tmp.GetComponent<TextMeshProUGUI>();
        string vorname = GameObject.Find("VornameText").GetComponent<TextMeshProUGUI>().text;
        string nachname = GameObject.Find("NachnameText").GetComponent<TextMeshProUGUI>().text;
        string gebDat = GameObject.Find("GebDatText").GetComponent<TextMeshProUGUI>().text;
        string trainingsDaystring = GameObject.Find("TrainingsDayText").GetComponent<TextMeshProUGUI>().text;
        int trainingsDay;
        int.TryParse(trainingsDaystring, out trainingsDay);
        string vpNummerstring = GameObject.Find("VPNummerText").GetComponent<TextMeshProUGUI>().text;
        int vpNummer;
        int.TryParse(vpNummerstring, out vpNummer);
        string appdatapath = Application.dataPath;
        string relative_path_file = Path.Combine(relativeReadPath,fileDesignName);
        //string jsonString = Path.Combine(appdatapath, relative_path_file);
        char x = Path.DirectorySeparatorChar;
        string jsonFileName = Application.dataPath + x + relativeReadPath + x + fileDesignName; //Path.DirectorySeparatorChar Combine(appdatapath, relative_path_file);
//        string jsonString = "G:\\Unity\\Elisabeth_Scheiben_game\\Elisabeth_Scheiben\\Assets\\ExperimentalDesign\\Paradigm1.json";
        //string jsonString = "G:/Unity/Elisabeth_Scheiben_game/Elisabeth_Scheiben/Assets/ExperimentalDesign/Paradigm2.json";
        string jsonString = File.ReadAllText(jsonFileName);

        playerData = new PlayerData(vorname, nachname, gebDat, trainingsDay, vpNummer, jsonString);
        isInitialized = true;
        playerData.PrintPlayerData();
        //Paradigma paradigma = Paradigma(jsonString);
    }

    [System.Serializable]
    public class PlayerData
    {
        public string vorname;
        public string nachname;
        public string gebDatum;
        public int trainingsDay;
        public int vpNummer;
        public Paradigma paradigma;
        
        private List<PlayerTrackEntry> playerTrackEntries ;
        public char lineSeperater = '\n'; // It defines line seperate character
        public char fieldSeperator = ';'; // It defines field seperate chracter
        public string relativeFilePath = "Assets/Data/";


        // construktor .... ohne die persoenlichen Infos geht nix
        public PlayerData(string vn, string nn, string gd, int td, int vpnum, string jsonString)
        {
            vorname = vn;
            nachname = nn;
            gebDatum = gd;
            trainingsDay = td;
            vpNummer = vpnum;
            //paradigma = Paradigma(jsonString);
            //print(jsonString);
            //paradigma = Paradigma.CreateFromJSON(jsonString);
            paradigma = JsonUtility.FromJson<Paradigma>(jsonString);
            playerTrackEntries = new List<PlayerTrackEntry>();
        }

        public void AddData(int blockIdx, int seqNum, int itemNum, int bp, int bt, int tc, int ta, int timex)
        {
            playerTrackEntries.Add(new PlayerTrackEntry(blockIdx, seqNum,  itemNum, bp, bt, tc, ta, timex));
        }

        public void SaveDataAsCSV()
        {
            string path = relativeFilePath; // Application.persistentDataPath;
            string filename = path + '/' + vpNummer + vorname + nachname + gebDatum + trainingsDay.ToString() + ".csv";
            print("filename = " + filename);
            using (StreamWriter sw = new StreamWriter(filename))
            {

                // heading line for csv File 
                string line = "BlockNumber" + fieldSeperator + "SeqNumber" + fieldSeperator + "ItemNumber" + fieldSeperator + "buttonPressed" + fieldSeperator + "buttonTarget" + fieldSeperator + "CircleTarget" + fieldSeperator + "timeAvailable" + fieldSeperator + "timeToButtonPressed";
                sw.WriteLine(line);
                for (int i=0; i<playerTrackEntries.Count; i++)
                {
                    line = playerTrackEntries[i].getEntryString();
                    sw.WriteLine(line);
                }

            }
        }

        public void PrintPlayerData()
        {   
            print("Ausgabe von Playerdata");
            print("Name = " + vorname + ' ' + nachname);
            print(paradigma.numBlocks);
        }
    }

  

    [System.Serializable]
    public class Paradigma
    {
        public int numBlocks;
        public int numScheiben; 
        public int MinimumInterScheibenDelay;
        public int MaximumInterScheibenDelay;
        public int MinimumVelocity;
        public int MaximumVelocity;
        public int MinimumScheibenExistenceDuration;
        public int MaximumScheibenExistenceDuration;
        public int Adaptive;

        // public static Paradigma CreateFromJSON(string jsonString)
        // {
        //      return JsonUtility.FromJson<Paradigma>(jsonString);
        // }

    }

    public class PlayerTrackEntry
    {
        private int blockIdx;
        private int itemNumber;     
        private int buttonPressed;
        private int buttonTarget;
        private int targetCircle;
        private int timeAvailable;
        private int timeToButtonPress;
        public char fieldSeperator = ';'; // It defines field seperate chracter

        public PlayerTrackEntry(int bidx, int seqNum, int itemNum, int bp, int bt, int tc, int ta, int timex)
        {
            blockIdx = bidx;
            itemNumber = itemNum;
            buttonPressed=bp;
            buttonTarget=bt;
            targetCircle = tc;
            timeAvailable=ta;
            timeToButtonPress=timex;
        }

        public string getEntryString()
        {
            string line = blockIdx.ToString() + fieldSeperator  + itemNumber.ToString() + fieldSeperator +  buttonPressed.ToString() + fieldSeperator + buttonTarget.ToString() +fieldSeperator + targetCircle.ToString() + fieldSeperator + timeAvailable.ToString() + fieldSeperator + timeToButtonPress.ToString();
            return line;
        }
    }
}
