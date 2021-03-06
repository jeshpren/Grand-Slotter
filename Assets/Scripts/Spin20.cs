using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin20 : MonoBehaviour
{
    // * Variables

    [Header("Main")]    //* Settings Main
    public bool randomizeRotation = true;
    public float deceleration = 0.05f;
    public int minSignTurns = 12;  // obrne naj se za najmn 12 znakov.. torej za krog pa pou
    public int maxSignTurns = 24;  // največ 4.5 kroge
    public float initialRotation;
    [HideInInspector]
    public float currentRotation;
    float endRotation;

    [Header("Other Reels")] //* Settings Other Reels
    public Spin20 reelSpin1;
    public Spin20 reelSpin2;
    public Spin20 reelSpin3;
    public Spin20 reelSpin4;

    // [Header("Angles & Coresponding Signs")]
    // public string sign22_5 = "A";
    // public string sign67_5 = "B";
    // public string sign112_5 = "C";
    // public string sign157_5 = "D";
    // public string sign202_5 = "E";
    // public string sign247_5 = "F";
    // public string sign292_5 = "G";
    // public string sign337_5 = "H";

    [Header("Signs")]
    // public GameObject[] signObjects = new GameObject[20];
    [HideInInspector]
    public Material[] signMaterials = new Material[20];
    [HideInInspector]
    public string[] signNames = new string[20];

    [HideInInspector]
    public float angleAdjusted;
    
    [HideInInspector]
    public float spinSpeed;

    int signTurns;
    [HideInInspector]
    public bool calculateSpin = false;
    bool calculateSpinPrev = false;
    [HideInInspector]
    public float timeCounter = 0f;



    [Header("Play Button")]
    public DetectMouseClick detectMouseClick;


    // * v = sqrt(2*a*d).... izračun končne hitrosti ob znanem pospešku in razdlaji *ali za moj primer* izračun začetne hitrosti ob znanem pojemku in številu obratov
    // * končna rotacija = začetna rotacija + signTurns*45°

    // [HideInInspector]
    // public string top;
    // public string middle;
    // public string bot;
    private void Awake() {

        // //* iz vseh sign objectou pober materiale in ih shran v signMaterials
        // for (int i = 0; i < signObjects.Length; i++)
        // {
        //     signMaterials[i] = signObjects[i].GetComponent<MeshRenderer>().material;
        //     // signMaterials[i] = new Material(signMaterials[i]);   // ne rabš delat novih, ker očinu unity poskrbi da se aplicera na vsazga posebej
        //     // if (transform.name == "Reel Numbered 1")
        //     // {
        //     //     signMaterials[i].EnableKeyword("_EMISSION");
        //     // }
        //     //* shran imena znakov
        //     signNames[i] = signObjects[i].name;
        // }

        //* Od vsazga childa (znaka) shran material in ime
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            signMaterials[i] = transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material;
            //* shran imena znakov
            signNames[i] = transform.GetChild(0).GetChild(i).name;
        }

    }

    private void Start()
    {

        if (randomizeRotation)
        {
            float randomSign = Random.Range(1, 20);
            initialRotation = randomSign * 18f;
            transform.rotation = Quaternion.Euler(initialRotation, 0f, 0f);
            currentRotation = initialRotation;
        }
        else
        {
            // if (transform.name == "Reel 1")
            // {
            //     Debug.Log("transform rot: " + transform.localRotation.eulerAngles.x);
            // }
            // ! "Back at it again with the Gimbal lock" - Leonhard Euler
            // initialRotation = transform.rotation.eulerAngles.x;
            currentRotation = initialRotation;
        }
        
    }

    private void Update() 
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        if (calculateSpin)
        {            
            SpinReel();
        }
        calculateSpinPrev = calculateSpin;

    }


    void HandleInput()
    {
        // (že nastavš iz reelManagerja)
        // // *če so se vsi nehal vrtet lahku zaženš
        // if ((Input.GetKeyDown(KeyCode.Space) || detectMouseClick.play) && spinSpeed == 0f && reelSpin1.spinSpeed == 0 && reelSpin2.spinSpeed == 0 && reelSpin3.spinSpeed == 0 && reelSpin4.spinSpeed == 0)
        // {
        //     calculateSpin = true;
        //     timeCounter = 0;
        // }
    }

    void SpinReel()
    {
        // if (transform.gameObject.name == "Reel Numbered 1")
        // {
        //     Debug.Log(Mathf.Round(timeCounter));
        // }

        // zračunej samu taprvič
        if (!calculateSpinPrev)
        {

            signTurns = Random.Range(minSignTurns, maxSignTurns);
            spinSpeed = -Mathf.Sqrt(2 * deceleration * (signTurns * 18f));    // en signTurn je enak 45° (pri oktagonu)
            endRotation = initialRotation - signTurns * 18f;
        }

        // štet morš tut trenutno rotacijo because circle
        currentRotation += spinSpeed;

        transform.Rotate(spinSpeed, 0f, 0f);
        if (spinSpeed < 0f)
        {
            // spinSpeed -= deceleration;
            spinSpeed += deceleration;
        }

        // * če si se zavrtu do konca --> ponastau
        if ((endRotation - currentRotation >= 0f))
        {
            // lock the angle
            transform.localRotation = Quaternion.Euler(endRotation, 0f, 0f);
            // Debug.Log("End rotation reached");
            spinSpeed = 0f;
            currentRotation = endRotation;
            initialRotation = endRotation;

            // * poprau kot
            angleAdjusted = endRotation % 360f - 18f;
            if (angleAdjusted < 0f)
                angleAdjusted += 360f;

            // * shran kote za ReelManagerja ****(ne dela (zamuja za en frame), dela pa če zračunaš znotrej ReelManager.cs)
            // top = signs[(int)(((angleAdjusted) / 45f) % 8)];
            // middle = signs[(int)(((angleAdjusted + 45f) / 45f) % 8)];
            // bot = signs[(int)(((angleAdjusted + 90f) / 45f) % 8)];
            // Debug.Log("1: " + signs[(int)((angleAdjusted / 45f) % 8)] + ", 2: " + signs[(int)(((angleAdjusted + 45f) / 45f) % 8)] + ", 3: " + signs[(int)(((angleAdjusted + 90f) / 45f)) % 8]);

            calculateSpin = false;
        }

        timeCounter += Time.fixedDeltaTime;

    }

}
