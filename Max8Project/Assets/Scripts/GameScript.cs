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
    public bool isGameStarted;
    public GameObject textSayingScore;
    public int attackType;
    public int attackPitch;
    public int playerAttackType;
    public int playerAttackPitch;
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
    private bool _bhasAttacked = false;

    // Start is called before the first frame update
    void Start()
    {
        health = 3;
        score = 0;
        attackType = 0;
        attackPitch = 0;
        startTime = 10.0f;/*initial timer duration*/
        isGameStarted=true;
        currentTime = startTime;
        //hitObj.SetActive(true);
        //blockObj.SetActive(true);
        textSayingScore.SetActive(false);
        pitches.Add(1.25f);
        pitches.Add(1f);
        pitches.Add(0.25f);

        soundOnUI = UIObj.GetComponent<AudioSource>();
        soundOnBlock = blockObj.GetComponent<AudioSource>();
        soundOnHit = hitObj.GetComponent<AudioSource>();
        soundOnUI.pitch=1f;
        //play and loop background music
        BGM.loop = true;
        BGM.volume = .5f;
        BGM.Play();
        /*adding to the dictonary for the attack dictionary for random (not used but good for reference)*/
        attackDict.Add("Punch",1);
        attackDict.Add("Sword",2);

        /*adding to the dictonary for the pitch dictionary for random (not used but good for reference)*/
        pitchDict.Add("High",0);
        pitchDict.Add("Medium",1);
        pitchDict.Add("Low",2);

        /*adding to the dictonary for the attack dictionary for player input*/
        attInpDict.Add(KeyCode.Keypad4,1);//left
        attInpDict.Add(KeyCode.Keypad6,2);//right

        /*adding to the dictonary for the pitch dictionary for player input*/
        pitInpDict.Add(KeyCode.Keypad8,2);//up
        pitInpDict.Add(KeyCode.Keypad5,1);//middle
        pitInpDict.Add(KeyCode.Keypad2,0);//down
    }

    public void SetAttackType(bool isTimerRunning)
    {
        if (!isTimerRunning) //Maybe the loop error has to do with the run statment being a bool and not a proper check
        {
            /*pick attackType (Sword or Punch)*/
            attackType = Random.Range(1, 3);
            /*pick attackPitch (High, Neutral or Low)*/
            attackPitch = Random.Range(0, 3);
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
            soundOnUI.pitch = pitches[attackPitch];
            _bhasAttacked = true;
        }
    }
    public void RunAttackCycle()
    {
        //GenerateNumbers(timerStarted);
        /*send to Max*/
        /*return sounds*/
        /*play sound*/
        soundOnUI.Play();
        /*wait*/
        StartCoroutine(Delay(soundOnUI,clipLength));
        if (timerStarted)
        {
            currentTime -= Time.deltaTime; //start countdown

            if (currentTime <= 0.0f && !buttonPressedDuringTimer) //if valid timer and null input
            {
                soundOnUI.pitch=1f;//reset pitch for attack and heart
                Debug.Log("Time's up to press button!");

                /*check input and if match attack and pitch*/
                if (playerAttackType==attackType && playerAttackPitch == attackPitch)
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

                _bhasAttacked = true;

                //reset assets to init states
                ResetGameLoop();

                //start new round
                if(health > 0 && !_bhasAttacked)
                {
                    //StartCoroutine(Delay(delayAmount));
                    /* NOT SURE WHAT TO PUT HERE*/
                }
            }

        }
        timerStarted = true;
    }

    private void ResetGameLoop()
    {
        //reset UI
        hitObj.SetActive(false);
        blockObj.SetActive(false);

        //flag variable
        _bhasAttacked = false;
        buttonPressedDuringTimer = false;

        //reset timer
        ResetTimer(currentTime);
    }

    public void ButtonPressed()
    {
        if (timerStarted) buttonPressedDuringTimer = true;
    }

    public void GameOver()
    {
        /*show score*/
        textSayingScore.SetActive(true);
        isGameStarted = false;
    }

    public void PlayerHit()
    {
        /*set the sound the the correct hit*/
        if (attackType == 1)
        {
            soundOnHit.clip = punchHit;
            clipLength = punchHit.length + 0.5f;
        }
        else
        {
            soundOnHit.clip = swordHit;
            clipLength = swordHit.length + 0.5f;
        }
        
        //set UI and audio
        hitObj.SetActive(true);


        //set sup-loop to force a slowdown of gameplay loop
        /*while (clipLength>0.0f)
        {
          clipLength -= Time.deltaTime;  
        }*/

        StartCoroutine(Delay(soundOnHit,clipLength));

        //load sound
        //grab length and add a dramatic pause
        //play sound
        //remove life
        soundOnUI.clip = healthLost;
        clipLength = healthLost.length + 1f; //longer pause for dramatic effect
        health--;
        StartCoroutine(Delay(soundOnUI,clipLength));

        //lose conditional
        if (health >= 0)
        {
            _bhasAttacked = true;
            ResetTimer(currentTime);
        }
        else //you suck git gud
        {
            GameOver();
        }

        //delay
        //StartCoroutine(Delay((int)clipLength));
    }

    public void PlayerBlocked()
    {
        /*set the sound the the correct block*/
        if (attackType == 1)
        {
            soundOnBlock.clip = punchBlocked;
            clipLength = punchBlocked.length + 0.5f;
        }
        else
        {
            soundOnBlock.clip = swordBlocked;
            clipLength = swordBlocked.length + 0.5f;
        }

        //set UI and audio
        blockObj.SetActive(true);

        StartCoroutine(Delay(soundOnBlock,clipLength));

        //general pdates
        score += 100;
        startTime -= 0.2f;

        //delay
        //StartCoroutine(Delay((int)clipLength));

        //end and reset
        _bhasAttacked = true;
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

        //gameplay loop
        if(isGameStarted) //gameloop check
        {
            if (health > 0 && !_bhasAttacked) //valid round
            {
                SetAttackType(timerStarted); //generate attack type and audio

                StartCoroutine(Delay(soundOnUI,clipLength));

            }

            currentTime -= Time.deltaTime; //start countdown

            if (currentTime <= 0.0f && !buttonPressedDuringTimer) //if valid timer and null input
            {
                soundOnUI.pitch = 1f;//reset pitch for attack and heart
                Debug.Log("Time's up to press button!");


                //check player input for matches
                if (playerAttackType == attackType && playerAttackPitch == attackPitch)
                {
                    //player wins round
                    PlayerBlocked();
                }
                else
                {
                    //lose round
                    PlayerHit();
                }

                if(health >= 0)
                    ResetGameLoop();
            }
    }
        
}

    public float delayAmount2 = 1.5f;
    IEnumerator Delay(AudioSource audioSource,float delayAmount)
    {
        audioSource.Play();
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
            playerAttackType = 1;
            Debug.Log(playerAttackType);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            playerAttackType = 2;
            Debug.Log(playerAttackType);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            playerAttackPitch = 2;
            Debug.Log(playerAttackPitch);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            playerAttackPitch = 1;
            Debug.Log(playerAttackPitch);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            playerAttackPitch = 0;
            Debug.Log(playerAttackPitch);
        }
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            isGameStarted=true;
        }
    }
}
