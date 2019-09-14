using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
    [SerializeField] GameObject ScheibePrefab;
    [SerializeField] GameObject Dotgray;
    [SerializeField] Rigidbody2D rbScheibePrefab;
    [SerializeField] Camera cam;
    [Header("Scheiben Geschwindigkeit")]
    public float minTimeBetweenScheiben = 0.5f;
    public float maxTimeBetweenScheiben = 2.5f;
    public float durationOfScheibe = 2.5f;
    [Space(20)]
    private GameObject newScheibe;
    private Rigidbody2D rbnewScheibe;
    public int numberOfScheibenPerSequence = 10;
    public Vector3 WPos00;
    public Vector3 WPos11;
    public bool freeze=false;

    // Start is called before the first frame update
    void Start()
    {
        WPos00 = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        WPos11 = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence() { 
        for(int i=0; i<numberOfScheibenPerSequence; i++)
        {
            float timedelay = Random.Range(minTimeBetweenScheiben, maxTimeBetweenScheiben);
            //StartCoroutine(CreateScheibe());
            StartCoroutine(CreateScheibe());
            yield return new WaitForSeconds(timedelay);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CreateScheibe()
    {
        float viewportPosx = Random.Range(0.02f, 0.98f);
        float viewportPosy = Random.Range(0.02f, 0.98f);
        float velInX = Random.Range(0.5f, 3f);
        float velInY = Random.Range(0.5f, 3f);
        Vector3 newPos = V2W(viewportPosx, viewportPosy);
        // print("pos = " + newPos.ToString());
        GameObject newScheibe = Instantiate(ScheibePrefab);
        // print("Scheibe wurde instanziert");
        Rigidbody2D rbnewScheibe = newScheibe.GetComponent<Rigidbody2D>();
        //Instantiate(rbScheibePrefab, newPos, Quaternion.identity);
        rbnewScheibe.transform.position = newPos;
        //rbnewScheibe = Instantiate(rbScheibePrefab, newPos, Quaternion.identity);
        rbnewScheibe.velocity = transform.TransformDirection(velInX, velInY, 0f);
        //CircleCollider2D circleCollider = rbnewScheibe.GetComponent<CircleCollider2D>();
        Transform transf = rbnewScheibe.transform;
        
        //circleCollider.radius = 0.5f;
        // starte klein
        transf.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        // wir teilen die duration durch 100 und machen die Scheibe dann in 100
        // Schritten gross und klein
        int index = 0;
        for (float f = 0; f < durationOfScheibe; f = f + durationOfScheibe/200)
        {
            //print("f= " +f.ToString());
            if (newScheibe != null && !newScheibe.GetComponent<Collisions>().GetFreeze())
            { 
                //print("index " + index.ToString());
                if (f < durationOfScheibe / 2) { index++; } else { index--; }

                float scaled = 0.1f +( (0.9f / 100f) * (float)index);
                transf.localScale = new Vector3(scaled,scaled, 0.1f);
                yield return null; //new WaitForSeconds(durationOfScheibe / 200);
            }
            // yield return null;
        }
        if (newScheibe != null && !newScheibe.GetComponent<Collisions>().GetFreeze())
        {
            Destroy(newScheibe);
        }
    }



    private Vector3 V2W(float x, float y)
    {
        return cam.ViewportToWorldPoint(new Vector3(x, y, 10f));

    }

}
