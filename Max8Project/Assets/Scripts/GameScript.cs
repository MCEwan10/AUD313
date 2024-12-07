using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
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
    public GameObject UIObj;
    /*referencing audio in unity*/
    public AudioSource soundOnUI;
    public AudioSource soundOnBlock;
    public AudioSource soundOnHit;
    public AudioSource BGM;
    public AudioClip swordAttack;
    public AudioClip swordBlocked;
    public AudioClip swordHit;
    public AudioClip punchAttack;
    public AudioClip punchBlocked;
    public AudioClip punchHit;
    public AudioClip healthLost;
    public float clipLength;
    //public float[] pitches;
    public List<float> pitches = new();

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

    //flag for round checking
    private bool isRoundOver = false;

    // Start is called before the first frame update
    void Start()
    {
        health = 3;
        score = 0;
        attackType = 0;
        attackPitch = 0;
        startTime = 10.0f;/*initial timer duration*/
        currentTime = startTime;
        hitObj.SetActive(false);
        blockObj.SetActive(false);
        textSayingScore.SetActive(false);
        pitches.Add(-1f);
        pitches.Add(0f);
        pitches.Add(1f);

        soundOnUI = UIObj.GetComponent<AudioSource>();
        soundOnBlock = blockObj.GetComponent<AudioSource>();
        soundOnHit = hitObj.GetComponent<AudioSource>();
        BGM.Play();
        BGM.loop = true;
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

        return buttonPressedDuringTimer;
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
        /*set the sound the the correct attack*/
        if (attackType == 1)
        {
            soundOnUI.clip = punchAttack;
            clipLength = punchAttack.length;
        }
        else
        {
            soundOnUI.clip = swordAttack;
            clipLength = swordAttack.length;
        }
        soundOnUI.pitch = pitches[attackPitch--];
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
        soundOnUI.Play();
        /*wait*/
        while (clipLength>0.0f)
        {
          clipLength -= Time.deltaTime;  
        }
        
        if (timerStarted)
        {
            currentTime -= Time.deltaTime; //start countdown

            if (currentTime <= 0.0f && !buttonPressedDuringTimer) //if valid timer and null input
            {
                
                Debug.Log("Time's up to press button!");

                /*check input and if match attack and pitch*/
                if (playerAttakType==attackType && playerAttakPitch == attackPitch)
                {
                    //player wins round
                    PlayerBlocked();
                }
                else
                {
                    //lose round
                    PlayerHit();
                }
                //hitObj.SetActive(false);
                //blockObj.SetActive(false);
                timerStarted = false;
                ResetTimer(currentTime);
                buttonPressedDuringTimer = false;

                isRoundOver = true;

                //reset assets to init states
                ResetGameLoop();

                //start new round
                if(health > 0 && !isRoundOver)
                {
                    StartCoroutine(Delay(delayAmount));
                    RunAttackCycle();
                }
            }

        }
        timerStarted = true;
    }

    private void ResetGameLoop()
    {
        hitObj.SetActive(false);
        blockObj.SetActive(false);
        //GenerateNumbers(timerStarted);
        isRoundOver = false;
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
        /*set the sound the the correct hit*/
        if (attackType == 1)
        {
            soundOnHit.clip = punchHit;
        }
        else
        {
            soundOnHit.clip = swordHit;
        }
        soundOnHit.Play();
        hitObj.SetActive(true);
        while (clipLength>0.0f)
        {
          clipLength -= Time.deltaTime;  
        }
        soundOnUI.clip = healthLost;
        soundOnUI.Play();
        health--;
        while (clipLength>0.0f)
        {
          clipLength -= Time.deltaTime;  
        }
        if (health > 0)
        {
            isRoundOver = true;
            ResetTimer(currentTime);
        }
        else
        {
            GameOver();
        }

        StartCoroutine(Delay(delayAmount));
    }

    public void PlayerBlocked()
    {
        /*set the sound the the correct block*/
        if (attackType == 1)
        {
            soundOnBlock.clip = punchBlocked;
        }
        else
        {
            soundOnBlock.clip = swordBlocked;
        }
        soundOnBlock.Play();
        blockObj.SetActive(true);
        while (clipLength>0.0f)
        {
          clipLength -= Time.deltaTime;  
        }
        score += 100;
        startTime -= 0.2f;

        StartCoroutine(Delay(delayAmount));

        isRoundOver = true;
        ResetTimer(currentTime);
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

        RegisterInput();

        if (health > 0 && !isRoundOver)
        {
            StartCoroutine(Delay(delayAmount));

            RunAttackCycle();
        }
        
    }

    public int delayAmount = 0;
    IEnumerator Delay(int delayAmount)
    {
     
        yield return new WaitForSeconds(delayAmount); //2 sec delay
    }

    private void ResetTimer(float _pCurrentTime)
    {
        _pCurrentTime = startTime;
    }

    private void RegisterInput()
    {
        /*get player input*/
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            playerAttakType = 1;
            Debug.Log(playerAttakType);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            playerAttakType = 2;
            Debug.Log(playerAttakType);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            playerAttakPitch = 1;
            Debug.Log(playerAttakPitch);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            playerAttakPitch = 2;
            Debug.Log(playerAttakPitch);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            playerAttakPitch = 3;
            Debug.Log(playerAttakPitch);
        }
    }
}
