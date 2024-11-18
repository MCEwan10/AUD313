using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    /*create variables*/
    public int health;
    public int numOfHearts;
    public Image[] hearts;
    public int score;
    public Text scoreText;
    
    public GameObject textSayingScore;
    public int attackType;
    public int attackPitch;
    public int playerAttakType;
    public int playerAttakPitch;
    /*referencing tags in unity*/
    public GameObject hitObj;
    public GameObject blockObj;
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;
    public GameObject scoreObj;
    /*Dictionary creation*/
    Dictionary<string, int> attackDict= new Dictionary<string, int>();
    Dictionary<string, int> pitchDict= new Dictionary<string, int>();
    Dictionary<KeyCode, int> attInpDict= new Dictionary<KeyCode, int>();
    Dictionary<KeyCode, int> pitInpDict= new Dictionary<KeyCode, int>();
    /*Timer based variables*/
    public float startTime;
    private float currentTime;
    private bool timerStarted = false;
    private bool buttonPressedDuringTimer = false;
    // Start is called before the first frame update
    void Start()
    {
        health = 3;
        score = 0;
        attackType = 0;
        attackPitch = 0;
        startTime = 4.0f;/*initial timer duration*/
        currentTime = startTime;
        hitObj.SetActive(false);
        blockObj.SetActive(false);
        textSayingScore.SetActive(false);
        /*adding to the dictonary for the attack dictionary for random (not used but good for reference)*/
        attackDict.Add("Punch",1);
        attackDict.Add("Sword",2);
        /*adding to the dictonary for the pitch dictionary for random (not used but good for reference)*/
        pitchDict.Add("High",1);
        pitchDict.Add("Medium",2);
        pitchDict.Add("Low",3);
        /*adding to the dictonary for the attack dictionary for player input*/
        attInpDict.Add(KeyCode.Keypad4,1);//left
        attInpDict.Add(KeyCode.Keypad6,2);//right
        /*adding to the dictonary for the pitch dictionary for player input*/
        pitInpDict.Add(KeyCode.Keypad8,1);//up
        pitInpDict.Add(KeyCode.Keypad5,2);//middle
        pitInpDict.Add(KeyCode.Keypad2,3);//down
    }
    public bool ResetTimer(bool wasHealthReduced)
    {
        if (startTime !< 0.4f && wasHealthReduced == false) currentTime = startTime;
        timerStarted = true;
        buttonPressedDuringTimer = false;
    }
    public void GenerateNumbers(bool isTimerRunning)
    {
        if (!isTimerRunning)
        {
        /*pick attackType (Sword or Punch)*/
        attackType = Random.Range(1, 3);
        /*pick attackPitch (High, Neutral or Low)*/
        attackPitch = Random.Range(1, 4);
        Debug.Log("$Attack :"+attackType + " Pitch :"+attackPitch);
        }
    }
    public void RunAttackCycle()
    {
        hitObj.SetActive(false);
        blockObj.SetActive(false);
        GenerateNumbers(timerStarted);
        /*send to Max*/
        /*return sounds*/
        /*play sound*/
        /*wait*/

        if (timerStarted)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0.0f && !buttonPressedDuringTimer)
            {
                
                Debug.Log("Time's up!");
                /*check input and if match attack and pitch*/
                if (playerAttakType==attackType && playerAttakPitch == attackPitch)
                {
                    //if yes;
                    PlayerBlocked();
                }
                //if no;
                else
                {
                    PlayerHit();
                }
                //hitObj.SetActive(false);
                //blockObj.SetActive(false);
                timerStarted = false;
                buttonPressedDuringTimer = false;
            }

        }
        timerStarted = true;
    }
    public void ButtonPressed()
    {
        if (timerStarted) buttonPressedDuringTimer = true;
    }

    public void GameOver()
    {
        /*show score*/
        textSayingScore.SetActive(true);
    }

    public void PlayerHit()
    {
    /*show hit*/
    hitObj.SetActive(true);
    /*reduce health*/
    health -= 1;
    /*check health is above zero*/
    if (health > 0)
    {
        /*if yes;*/
            /*rerun cycle*/
            ResetTimer(true);
        }
        /*if no;*/
        else
        {
            /*stop cycle*/
            GameOver();
        }
        yield return null;//force wait
    }
    public void PlayerBlocked()
    {
        /*if yes;*/
        /*show block*/
        blockObj.SetActive(true);
        //add score
        score += 100;
        /*reduce timer*/
        startTime -= 0.2f;
        yield return null;//force wait
        /*rerun cycle*/
        ResetTimer(false);
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hearts.Length; i++) /*display current health*/
        {
            numOfHearts = health;
            if(i< numOfHearts) hearts[i].enabled=true;
            else hearts[i].enabled=false;
        }
        /*get player input*/
                if (Input.GetKeyDown(KeyCode.Keypad4)) {playerAttakType = 1;
                Debug.Log(playerAttakType);}
                if (Input.GetKeyDown(KeyCode.Keypad6)) {playerAttakType = 2;
                Debug.Log(playerAttakType);}
                if (Input.GetKeyDown(KeyCode.Keypad8)) {playerAttakPitch = 1;
                Debug.Log(playerAttakPitch);}
                if (Input.GetKeyDown(KeyCode.Keypad5)) {playerAttakPitch = 2;
                Debug.Log(playerAttakPitch);}
                if (Input.GetKeyDown(KeyCode.Keypad2)) {playerAttakPitch = 3;
                Debug.Log(playerAttakPitch);}
        if (health > 0) RunAttackCycle();
    }
}
