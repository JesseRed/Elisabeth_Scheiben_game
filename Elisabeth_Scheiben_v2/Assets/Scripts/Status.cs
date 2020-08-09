using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float geschw = 0;
    //public float instantiate_time;
    public float durationOfScheibe;
    public float timeLokaleScheibeInstatiate;
    public float scale;
    public bool wasHit = false;
    public float mousePosStartX = -1;
    public float mousePosStartY = -1;
    public float scheibePosStartX = -1;
    public float scheibePosStartY = -1;
    public List<float> mousePosX = new List<float>();
    public List<float> mousePosY = new List<float>();
    public List<float> scheibePosX = new List<float>();
    public List<float> scheibePosY = new List<float>();
    public List<float> timeList = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        // die StartPositionen werden beim intanzieren der Klasse im ItemSpawner gesetzt

        //mousePosStartX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        //mousePosStartY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        //Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        //instantiate_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        timeList.Add(Time.time);
        mousePosX.Add(Camera.main.ScreenToViewportPoint(Input.mousePosition).x);
        mousePosY.Add(Camera.main.ScreenToViewportPoint(Input.mousePosition).y);
        scheibePosX.Add(Camera.main.WorldToViewportPoint(this.transform.position).x);
        scheibePosY.Add(Camera.main.WorldToViewportPoint(this.transform.position).y);
    }

    public float get_timeToMovementInitiation(){
        float timeToMovementInitiation = 0.0f;
        float summedMouseMovements = 0.0f;
        for(int i = 1; i< timeList.Count; i++){
            
            
            summedMouseMovements+= Mathf.Sqrt(Mathf.Pow(mousePosX[i]-mousePosX[i-1],2)+Mathf.Pow(mousePosY[i]-mousePosY[i-1],2));
            // heuristisch ... ca. 30 cm bildschirm
            // 1 % aenderung der Mousposition des Bildschirminhalts
            if (summedMouseMovements>0.01){
                timeToMovementInitiation= timeList[i]-timeList[1];
                break;
            }
        }
        return timeToMovementInitiation;
    }

    public float get_cursorPathError(){
        float cursorPathError = -1;

        float distpowx = Mathf.Pow(mousePosX[0]-scheibePosX[scheibePosX.Count-1],2);
        float distpowy = Mathf.Pow(mousePosY[0]-scheibePosY[scheibePosY.Count-1],2);
        float optimal_path_length = Mathf.Sqrt(distpowx + distpowy);        
        float summedMouseMovements = 0.0f;

        for(int i = 1; i< mousePosX.Count; i++){
            distpowx = Mathf.Pow(mousePosX[i-1]-mousePosX[i],2);
            distpowy = Mathf.Pow(mousePosY[i-1]-mousePosY[i],2);
            summedMouseMovements+= Mathf.Sqrt(distpowx+distpowy);
        }
        cursorPathError = summedMouseMovements-optimal_path_length;
        return cursorPathError;
    }

    public float get_finalPosError(){
        float distpowx = Mathf.Pow(mousePosX[mousePosX.Count-1]-scheibePosX[scheibePosX.Count-1],2);
        float distpowy = Mathf.Pow(mousePosY[mousePosY.Count-1]-scheibePosY[scheibePosY.Count-1],2);
        return Mathf.Sqrt(distpowx + distpowy);        
    }

    // heuristisch ... mittlung ueber 5 frames
    public float get_maxMovementVelocity(){
        float max_velocity = 0;
        for(int i = 5; i< timeList.Count; i=i+5){
            float delta_time = timeList[i]-timeList[i-5];
            float vel = Mathf.Sqrt(Mathf.Pow(mousePosX[i]-mousePosX[i-5],2)+Mathf.Pow(mousePosY[i]-mousePosY[i-5],2));
            float rel_vel = vel/delta_time;
            if (rel_vel>max_velocity){
                max_velocity = rel_vel;
            }
        }
        return max_velocity;
    }

}
