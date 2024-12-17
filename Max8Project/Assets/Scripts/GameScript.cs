using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    /*create variables*/
    private int health;
    private int numOfHearts;
    public Image[] hearts;
    private int score;
    public Text text;
    public GameObject textSayingScore;
    private int attackType;
    private int attackPitch;
    private int playerAttackType;
    private int playerAttackPitch;
    /*referencing tags in unity*/
    public GameObject hitObj;
    public GameObject blockObj;
    public GameObject UIObj;
    /*referencing audio in unity*/
    private AudioSource soundOnUI;
    private AudioSource soundOnBlock;
    private AudioSource soundOnHit;
    public AudioSource BGM;
    public AudioClip BGMClip;
    public AudioClip[] attackClips;
    public AudioClip[] blockClips;
    public AudioClip[] hitClips;
    public AudioClip healthLost;
    private float clipLength;
    private List<float> pitches = new();

    private float currentTime;
    private bool bGameOver;

    /*Dictionary creation*/
    Dictionary<string, int> attackDict= new Dictionary<string, int>();
    Dictionary<string, int> pitchDict= new Dictionary<string, int>();
    Dictionary<KeyCode, int> attInpDict= new Dictionary<KeyCode, int>();
    Dictionary<KeyCode, int> pitInpDict= new Dictionary<KeyCode, int>();

    // Start is called before the first frame update
    void Start()
    {
        //fetching Audio components on objects
        soundOnUI = UIObj.GetComponent<AudioSource>();
        soundOnBlock = blockObj.GetComponent<AudioSource>();
        soundOnHit = hitObj.GetComponent<AudioSource>();

        //set pitches
        pitches.Add(0.75f);
        pitches.Add(1f);
        pitches.Add(1.25f);

        //play and loop background music
        BGM.clip = BGMClip;
        BGM.loop = true;
        BGM.volume = 0.5f;
        BGM.Play();
        {
        /*adding to the dictonary for the attack dictionary for random (NOTE: dictionaries are not used but good for reference)*/
        attackDict.Add("Punch",0);
        attackDict.Add("Sword",1);

        /*adding to the dictonary for the pitch dictionary for random*/
        pitchDict.Add("High",2);
        pitchDict.Add("Medium",1);
        pitchDict.Add("Low",0);

        /*adding to the dictonary for the attack dictionary for player input*/
        attInpDict.Add(KeyCode.Keypad4,1);//left
        attInpDict.Add(KeyCode.Keypad6,2);//right

        /*adding to the dictonary for the pitch dictionary for player input*/
        pitInpDict.Add(KeyCode.Keypad8,2);//high
        pitInpDict.Add(KeyCode.Keypad5,1);//medium
        pitInpDict.Add(KeyCode.Keypad2,0);//low
        }
        GameOver();
    }

    private void StartGame(bool isGameOver)
    {
        if (isGameOver)//restart game
        {
            bGameOver=false;
            score = 0;
            currentTime = 6.0f;
            health=3;
            textSayingScore.SetActive(false);
        }
        SetAttackType();
        StartCoroutine(PlayAndWait());
    }

    private IEnumerator PlayAndWait()
    {
        //set the sound the the correct attack
        soundOnUI.pitch = pitches[attackPitch];
        clipLength = attackClips[attackType].length;
        //play sound
        soundOnUI.PlayOneShot(attackClips[attackType]);
        //wait for sound to finish playing
        yield return new WaitForSeconds(clipLength);
        //start timer
        StartCoroutine(WaitForPlayerInput(currentTime));
    }

    private IEnumerator WaitForPlayerInput(float timeLeft)
    {
        //reset player input
        playerAttackPitch = 0;
        playerAttackType = 0;
        //wait timer
        while (timeLeft>0f)
        {
            RegisterInput();
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        /*check input and if match attack and pitch*/
        if (playerAttackType==attackType && playerAttackPitch == attackPitch)
            StartCoroutine(PlayBlockedAndReplay(blockObj,soundOnBlock,clipLength,blockClips,attackType));
        else
            StartCoroutine(PlayHitAndReset(hitObj,soundOnHit,clipLength,hitClips,attackType,soundOnUI,healthLost));

    }

    private IEnumerator PlayBlockedAndReplay(GameObject shower,AudioSource sound, float soundLength, AudioClip[] clips, int attack)
    {
        //tell player they blocked
        shower.SetActive(true);
        soundLength = clips[attack].length +0.4f;
        sound.PlayOneShot(clips[attack]);
        yield return new WaitForSeconds(soundLength);
        //general updates
        score += 100;
        currentTime -= 0.2f;
        //reset for next round
        shower.SetActive(false);
        StartGame(false);
    }

    private IEnumerator PlayHitAndReset(GameObject shower,AudioSource sound, float soundLength, AudioClip[] clips, int attack, AudioSource healthSound, AudioClip healthClip)
    {
        //tell player they got hit
        shower.SetActive(true);
        soundLength = clips[attack].length;
        sound.PlayOneShot(clips[attack]);
        yield return new WaitForSeconds(soundLength + 0.4f);
        healthSound.pitch=1f;//resetting pitch
        //general updates
        healthSound.PlayOneShot(healthClip);
        health--;
        yield return new WaitForSeconds(healthClip.length +1f); //longer pause for dramatic effect
        shower.SetActive(false);
        //check if player has lost all lives
        if(health > 0)
            StartGame(false);
        else
            GameOver();
    }

    public void SetAttackType()
    {
        //pick attackType (Sword or Punch)
        attackType = Random.Range(0, 2);
        //pick attackPitch (High, Neutral or Low)
        attackPitch = Random.Range(0, 3);
        Debug.Log("$Attack :"+attackType + " Pitch :"+attackPitch);
        
    }

    public void GameOver()
    {
        //show score
        bGameOver=true;
        text.text=score.ToString();
        textSayingScore.SetActive(true);
    }

    private void RegisterInput()
    {
        //get player inputs
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            playerAttackType = 0;
            Debug.Log(playerAttackType);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            playerAttackType = 1;
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
        //game start input
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            StartGame(true);
        }
    }
void Update()
    {
        for (int i = 0; i < hearts.Length; i++) //display current health
        {
            numOfHearts = health;
            if(i< numOfHearts) hearts[i].enabled=true;
            else hearts[i].enabled=false;
        }
        //allowing player to restart the game
        if (bGameOver)
            RegisterInput();
    }
}