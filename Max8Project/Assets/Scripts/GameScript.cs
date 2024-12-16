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
    private GameObject textSayingScore;
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
    private AudioSource BGM;
    public AudioClip[] attackClips;
    public AudioClip[] blockClips;
    public AudioClip[] hitClips;
    public AudioClip healthLost;
    private float clipLength;
    private List<float> pitches = new();

    /*Dictionary creation*/
    Dictionary<string, int> attackDict= new Dictionary<string, int>();
    Dictionary<string, int> pitchDict= new Dictionary<string, int>();
    Dictionary<KeyCode, int> attInpDict= new Dictionary<KeyCode, int>();
    Dictionary<KeyCode, int> pitInpDict= new Dictionary<KeyCode, int>();
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        health = 3;
        score = 0;
        attackType = 0;
        attackPitch = 0;
        currentTime = 10.0f;/*initial timer duration*/

        pitches.Add(1.25f);
        pitches.Add(1f);
        pitches.Add(0.25f);
        soundOnUI.pitch=1f;

        //play and loop background music
        BGM.loop = true;
        BGM.volume = .5f;
        BGM.Play();

        /*adding to the dictonary for the attack dictionary for random (dicts not used but good for reference)*/
        attackDict.Add("Punch",1);
        attackDict.Add("Sword",2);

        /*adding to the dictonary for the pitch dictionary for random*/
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

    private void StartGame(bool isGameOver)
    {
        if (isGameOver)
        {
            score = 0;
            textSayingScore.SetActive(false);
        }
        SetAttackType();
        StartCoroutine(PlayAndWait());
    }

    private IEnumerator PlayAndWait()
    {
        soundOnUI.PlayOneShot(attackClips[attackType]);
        yield return new WaitForSeconds(clipLength);
        StartCoroutine(WaitForPlayerInput(currentTime));
    }

    private IEnumerator WaitForPlayerInput(float timeLeft)
    {
        playerAttackPitch = 0;
        playerAttackType=0;

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
            StartCoroutine(PlayHitAndReset(hitObj,soundOnHit,clipLength,hitClips,attackType,soundOnUI,healthLost,health));

    }

    private IEnumerator PlayBlockedAndReplay(GameObject shower,AudioSource sound, float soundLength, AudioClip[] clips, int attack)
    {
    shower.SetActive(true);
    soundLength = clips[attack].length;
    sound.PlayOneShot(clips[attack]);
    yield return new WaitForSeconds(soundLength);
    //general updates
    score += 100;
    currentTime -= 0.2f;

    shower.SetActive(false);
    StartGame(false);
    }

    private IEnumerator PlayHitAndReset(GameObject shower,AudioSource sound, float soundLength, AudioClip[] clips, int attack, AudioSource healthSound, AudioClip healthClip, int heartCount)
    {
        shower.SetActive(true);
        soundLength = clips[attack].length;
        sound.PlayOneShot(clips[attack]);
        yield return new WaitForSeconds(soundLength);

        healthSound.PlayOneShot(healthClip);
        heartCount--;
        yield return new WaitForSeconds(healthClip.length +1f); //longer pause for dramatic effect
        shower.SetActive(false);
        if(heartCount>= 0)
            StartGame(false);
        else
            GameOver();
    }

    public void SetAttackType()
    {
        /*pick attackType (Sword or Punch)*/
        attackType = Random.Range(1, 3);
        /*pick attackPitch (High, Neutral or Low)*/
        attackPitch = Random.Range(0, 3);
        Debug.Log("$Attack :"+attackType + " Pitch :"+attackPitch);
        
        /*set the sound the the correct attack*/
        soundOnUI.pitch = pitches[attackPitch];
        clipLength = attackClips[attackType].length;
    }

    public void GameOver()
    {
        /*show score*/
        text.text=score.ToString();
        textSayingScore.SetActive(true);
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
            StartGame(true);
        }
    }
}
