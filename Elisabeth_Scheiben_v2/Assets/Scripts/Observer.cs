using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField] GameObject Dot;
    [SerializeField] GameObject Dotgray;
    //private bool mouseover = false;
    public Ray ray;
    public bool mouse_pressed = false;
    private GameSession gameSession;
    private int isHit;
    private ItemSpawner itemSpawner;
    private GameObject nearest_Scheibe;
    
    private Rigidbody2D nearest_rbScheibe;
    
    private float timeToMovementInitiation = -1;
    private float cursorPathError = -1;
    private float finalPosError = -1; 
    private float maxMovementVelocity = -1;

    // private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        itemSpawner = FindObjectOfType<ItemSpawner>();
    }


    void Update()
    {
        isHit = 0;
                // Pause setzen









        //Converting Mouse Pos to 2D (vector2) World Pos
        if (Input.GetMouseButtonDown(0) && !mouse_pressed)
        {
            //isHit = 1;
            //print("hit");
            mouse_pressed = true;
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
            //RaycastHit2D hit=Physics2D.GetRayIntersection(rayPos, Vector2.zero, 0f);
            if (hit)
            {   
                GameObject collidedObject = hit.collider.gameObject;
                if (collidedObject.name == "Scheibe(Clone)"){
                    Status hitted_scheibe_status =  collidedObject.GetComponent<Status>();
                    Rigidbody2D rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                    //hit.collider.gameObject.freezeScheibe = true;
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;

                    GameObject dot = Instantiate(Dot);
                    dot.transform.position = rayPos;
                    // collect necessary information from the Scheibe
                    // before destruction 
                    timeToMovementInitiation = hitted_scheibe_status.get_timeToMovementInitiation();
                    cursorPathError = hitted_scheibe_status.get_cursorPathError();
                    finalPosError = hitted_scheibe_status.get_finalPosError();
                    maxMovementVelocity = hitted_scheibe_status.get_maxMovementVelocity();
                    Destroy(collidedObject, gameSession.playerData.paradigma.timeBetweenHitAndDisappear);
                    Destroy(dot, gameSession.playerData.paradigma.timeBetweenHitAndDisappear+0.1f);
                    isHit = 1;
                    itemSpawner.last_scheibe_hit = 1;
                }                

            }
            else
            {
                isHit = 0;
                GameObject dot = Instantiate(Dotgray);
                dot.transform.position = rayPos;
                Destroy(dot, 1f);
            }
            float velocity = 0f;
            float scale = 0f;
            float duration = 0f;
            float existTime = 0f;
            int numScheibenPresent;
            nearest_Scheibe = itemSpawner.get_nearest_Scheiben_data(Camera.main.ScreenToWorldPoint(Input.mousePosition), out velocity, out numScheibenPresent);
            print("Observer ... numScheibenPresent = " + numScheibenPresent);
       
            if (nearest_Scheibe != null)
            {
                nearest_rbScheibe = nearest_Scheibe.GetComponent<Rigidbody2D>();
                velocity = Mathf.Sqrt(Mathf.Pow(nearest_rbScheibe.velocity.x, 2) + Mathf.Pow(nearest_rbScheibe.velocity.y, 2)); //(float)Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2)+Mathf.Pow(rb.velocity.y,2)),
                                                                                                                                //print("number of Scheiben present = " + numScheibenPresent);
                Status status = nearest_Scheibe.GetComponent<Status>();
                scale = status.scale;
                existTime = Time.time - status.timeLokaleScheibeInstatiate;
                duration = status.durationOfScheibe;
                //print("instantiate time  = " + status.durationOfScheibe);
                if (isHit == 1 && status.wasHit == false)
                {
                    gameSession.hitsNumInBlock += 1;
                    status.wasHit = true;
                }
            }
            // if (isHit == 1)
            // {
            //     gameSession.hitsNumInBlock += 1;                
            //     //print("add to " + gameSession.hitsNumInBlock);
            // }
            if (isHit ==0)
            {
                gameSession.nonHitsNumInBlock += 1;// wenn die scheibe nach Zeit zerstoert wird, dann wurde sie nicht getroffen und wir ziehen von allen Scheiben eine ab
            }
            if (nearest_rbScheibe != null){
            gameSession.playerData.AddData(
                    itemSpawner.get_blockIdx(),
                    (float)(Time.time - itemSpawner.get_timeBlockStart() - itemSpawner.get_timeBlockPause()),
                    "mouse",
                    isHit, // Hit
                    itemSpawner.get_scheibeIdxInBlock(), // Number der Scheibe im aktuellen Block
                    Camera.main.ScreenToViewportPoint(Input.mousePosition).x, // Mouse position
                    Camera.main.ScreenToViewportPoint(Input.mousePosition).y, // Mouse position
                    Camera.main.WorldToViewportPoint(nearest_rbScheibe.transform.position).x, // Mouse position
                    Camera.main.WorldToViewportPoint(nearest_rbScheibe.transform.position).y, // Mouse position
                     velocity, // = Mathf.Sqrt(Mathf.Pow(nearest_rbScheibe.velocity.x,2) + Mathf.Pow(nearest_rbScheibe.velocity.y,2)), //(float)Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2)+Mathf.Pow(rb.velocity.y,2)),
                    (float)(scale), // Scheiben Diameter
                    (float)(existTime), //timeLokaleScheibeInstatiate), //  int existenceTime, 
                    (float)(duration),//durationOfScheibe, // maxExistenceTime
                    numScheibenPresent,
                    (float) timeToMovementInitiation, 
                    (float) cursorPathError,
                    (float) finalPosError, 
                    (float) maxMovementVelocity);

        }
        }
        if (Input.GetMouseButtonUp(0) && mouse_pressed)
        {
            mouse_pressed = false;
        }
    }


}