using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimController : MonoBehaviour
{

    public List<AudioClip> clips;

    public GameObject audioPlayer;

    public GameObject player;

    public AudioClip beep1;
    public AudioClip beep2;
    public AudioClip completeRoom;
    public AudioClip finishRooms;

    public List<GameObject> rooms;
    int room = 0;

    public Text room1txt;
    public Text room2txt;
    public Text room3txt;
    public Text room4txt;

    Vector3 playerInitPos;

    AudioSource aSource;

    float room1Time = 0f;
    float room2Time = 0f;
    float room3Time = 0f;
    float room4Time = 0f;

    bool completed = false;
    bool pressedComplete = false;

    bool started = false;
    bool welcome = false;
    bool pressedW = false;
    bool readyForW = true;
    bool pressedA = false;
    bool readyForA = false;
    bool pressedS = false;
    bool readyForS = false;
    bool pressedD = false;
    bool readyForD = false;
    bool roomExplain = false;

    bool runTime = false;

    int clip = 0;

    public VestHapticClip hapClip;

    // Start is called before the first frame update
    void Start()
    {
        welcome = false;
        aSource = audioPlayer.GetComponent<AudioSource>();
        player.GetComponent<SC_FPSController>().enabled = false;
        player.GetComponent<PlayerHapticCast>().enabled = false;
        playerInitPos = player.transform.position;
        StartCoroutine(PlayWelcomeMessage());
    }

    IEnumerator PlayWelcomeMessage()
    {
        while(welcome == false)
        {
            aSource.clip = clips[clip];
            aSource.Play();
            yield return new WaitForSeconds(5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(started == false)
        {
            DoIntro();
        }
        
        if(completed && !pressedComplete && Input.GetMouseButtonDown(0))
        {
            pressedComplete = true;
            aSource.Stop();
            StopAllCoroutines();
            LoadNextRoom();
        }

        if(runTime)
        {
            if(room == 0)
            {
                room1Time += Time.smoothDeltaTime;
                room1txt.text = "" + room1Time;
            } else if(room == 1)
            {
                room2Time += Time.smoothDeltaTime;
                room2txt.text = "" + room2Time;
            }
            else if (room == 2)
            {
                room3Time += Time.smoothDeltaTime;
                room3txt.text = "" + room3Time;
            }
            else if (room == 3)
            {
                room4Time += Time.smoothDeltaTime;
                room4txt.text = "" + room4Time;
            }
        }

    }

    void LoadNextRoom()
    {
        rooms[room].SetActive(false);
        room++;
        rooms[room].SetActive(true);
        player.transform.position = playerInitPos;
        completed = false;
        pressedComplete = false;
        StartCoroutine(CountDown());
    }


    void DoIntro()
    {
        if (Input.GetMouseButtonDown(0) && welcome == false)
        {
            Debug.Log("Clicked");
            welcome = true;
            clip++;
            StopCoroutine(PlayWelcomeMessage());
            aSource.Stop();
            StartCoroutine(PlayInstructions());
        }

        if (readyForW && pressedW == false && Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W pressed");
            pressedW = true;
            readyForW = false;
            StopCoroutine(PlayPressW());
            aSource.Stop();
            clip++;
            StartCoroutine(PlayPressA());
        }

        if (readyForA && pressedA == false && Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A pressed");
            pressedA = true;
            readyForA = false;
            StopCoroutine(PlayPressA());
            aSource.Stop();
            clip++;
            StartCoroutine(PlayPressS());
        }

        if (readyForS && pressedS == false && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S pressed");
            pressedS = true;
            readyForS = false;
            StopCoroutine(PlayPressS());
            aSource.Stop();
            clip++;
            StartCoroutine(PlayPressD());
        }

        if (readyForD && pressedD == false && Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D pressed");
            pressedD = true;
            readyForD = false;
            StopCoroutine(PlayPressD());
            aSource.Stop();
            clip++;
            StartCoroutine(HapticExplaination());
        }

        if (roomExplain && Input.GetMouseButtonDown(0))
        {
            roomExplain = false;
            StopCoroutine(RoomExplaination());
            aSource.Stop();
            StartCoroutine(CountDown());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            StartCoroutine(CountDown());
        }
    }

    public void CompleteRoom()
    {
        runTime = false;
        player.GetComponent<SC_FPSController>().enabled = false;
        player.GetComponent<PlayerHapticCast>().enabled = false;
        aSource.clip = completeRoom;
        completed = true;
        pressedComplete = false;
        if(room + 1 > 3)
        {
            aSource.clip = finishRooms;
            aSource.Play();
            return;
        }
        StartCoroutine(CompleteRoomClick());
    }

    IEnumerator CompleteRoomClick()
    {
        while(!pressedComplete)
        {
            aSource.Play();
            yield return new WaitForSeconds(7);
        }
    }

    IEnumerator CountDown()
    {
        started = true;
        int countDown = 3;

        while(countDown > 0)
        {
            aSource.clip = beep1;
            aSource.Play();
            countDown -= 1;
            yield return new WaitForSecondsRealtime(1);
        }
        aSource.clip = beep2;
        aSource.Play();
        yield return new WaitForSecondsRealtime(1);
        player.GetComponent<SC_FPSController>().enabled = true;
        player.GetComponent<PlayerHapticCast>().enabled = true;
        runTime = true;
        StopAllCoroutines();
    }

    IEnumerator HapticExplaination()
    {
        aSource.clip = clips[clip];
        aSource.Play();
        yield return new WaitForSeconds(10);
        hapClip.Play(1f, 2, 0, 0);
        yield return new WaitForSeconds(1);
        clip++;
        aSource.clip = clips[clip];
        aSource.Play();
        yield return new WaitForSeconds(4);
        hapClip.Play(0.1f, 2, 0, 0);
        yield return new WaitForSeconds(1);
        clip++;
        aSource.clip = clips[clip];
        aSource.Play();
        yield return new WaitForSeconds(3);
        hapClip.Play(5f, 2, 0, 0);
        yield return new WaitForSeconds(3);
        clip++;
        StartCoroutine(RoomExplaination());

    }

    IEnumerator RoomExplaination()
    {
        roomExplain = true;
        while (roomExplain)
        {
            aSource.clip = clips[clip];
            aSource.Play();
            yield return new WaitForSeconds(10);
        }
    }

    IEnumerator PlayPressD()
    {
        readyForD = true;
        while (pressedD == false)
        {
            aSource.clip = clips[clip];
            aSource.Play();
            yield return new WaitForSeconds(5);
        }
    }
    IEnumerator PlayPressS()
    {
        readyForS = true;
        while (pressedS == false)
        {
            aSource.clip = clips[clip];
            aSource.Play();
            yield return new WaitForSeconds(5);
        }
    }
    IEnumerator PlayPressA()
    {
        readyForA = true;
        while (pressedA == false)
        {
            aSource.clip = clips[clip];
            aSource.Play();
            yield return new WaitForSeconds(5);
        }
    }
    IEnumerator PlayPressW()
    {
        readyForW = true;
        while (pressedW == false)
        {
            aSource.clip = clips[clip];
            aSource.Play();
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator PlayInstructions()
    {
        yield return new WaitForSeconds(1);
        aSource.clip = clips[clip];
        clip++;
        aSource.Play();
        yield return new WaitForSeconds(15);
        StartCoroutine(PlayPressW());
    }
}
