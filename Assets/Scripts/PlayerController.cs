using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public Text countText;
    public Text winText;
    public GameObject pickUps;
    public Material green;
    public Text timeText;
    public Material lightRed;

    private Rigidbody rb;
    private int count;
    private int jumpCount;
    private Vector3 origin;
    private bool finished;
    private Material defMat;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defMat = GetComponent<Renderer>().material;
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called 30 times per second
    void FixedUpdate()
    {
        HandleMovement();
        HandleJumping();
        HandleTimer();
        HandleReset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
            StartCoroutine("FlashGreen");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Death Plane")
        {
            ResetPlayer();
        }
        jumpCount = 0;
    }

    // Prevents falling off platform and having two double jumps
    private void OnCollisionExit(Collision collision)
    {
        if (jumpCount == 0)
        {
            jumpCount++;
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            rb.AddForce(Vector3.up * jumpPower);
            jumpCount++;
            StartCoroutine("FlashLightRed");
        }
    }

    private void HandleTimer()
    {
        if (!finished)
        {
            // https://forum.unity.com/threads/convert-float-to-string.3762/#post-27879
            timeText.text = (Time.time - startTime).ToString("#.00") + "s";
        }
    }

    private void HandleReset()
    {
        if (Input.GetKeyDown(KeyCode.R) && finished)
        {
            ResetPlayer();
            for (int i = 0; i < pickUps.transform.childCount; i++)
            {
                Transform Go = pickUps.transform.GetChild(i);
                Go.gameObject.SetActive(true);
            }
            StartSetup();
        }
    }

    private void ResetPlayer()
    {
        // http://answers.unity.com/answers/852083/view.html
        transform.position = origin;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void StartSetup()
    {
        count = 0;
        SetCountText();
        winText.text = "Collect all the cubes!";
        jumpCount = 0;
        origin = transform.position;
        finished = false;
        GetComponent<Renderer>().material = defMat;
        startTime = Time.time;
        StartCoroutine("ClosePromptInThree");
    }

    private void SetCountText()
    {
        countText.text = "Count: " + count.ToString() + " / " + pickUps.transform.childCount.ToString();
        if (count >= pickUps.transform.childCount)
        {
            winText.text = "You Win! (R to reset)";
            finished = true;
        }
    }

    IEnumerator ClosePromptInThree()
    {
        yield return new WaitForSeconds(3f);
        winText.text = "";
    }

    IEnumerator FlashGreen()
    {
        GetComponent<Renderer>().material = green;
        yield return new WaitForSeconds(0.25f);
        GetComponent<Renderer>().material = defMat;
    }

    IEnumerator FlashLightRed()
    {
        GetComponent<Renderer>().material = lightRed;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material = defMat;
    }
}
