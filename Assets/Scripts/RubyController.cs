using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public float slowspeed = 1.0f;

    public float normalspeed = 3.0f;
    public int maxHealth = 5;
    public int cogs = 4;
    public GameObject projectilePrefab;

    public ParticleSystem healthIncrease;
    public ParticleSystem healthDecrease;

    public bool winlose = false;

    public Text scoreText;
    public Text winloseText;
    public Text ammoText;
    public Text timerText;

    public AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip backgroundmusic;
    public AudioClip victorymusic;
    public AudioClip gameovermusic;
    public AudioClip npctalk;
    public AudioClip gearclip;

    public float timeInvincible = 2.0f; 

    public float timeRemaining = 60;

    public bool timerIsRunning = false;

    public static int level=1;

    public int health { get { return currentHealth; }}
    int currentHealth;

    private int scoreValue = 0;
    

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Fixed Robots: " + scoreValue.ToString();
        winloseText.text = "";
        ammoText.text = "Cogs: " + cogs.ToString();
        
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        
        audioSource.clip = backgroundmusic;
        audioSource.Play();
        audioSource.loop = true;
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayTime(timeRemaining);
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                Debug.Log(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
        if (timeRemaining < 0)
        {
            audioSource.clip = gameovermusic;
            audioSource.Play();
            audioSource.loop = false;
            winloseText.text = "Game Over! Press R to Restart";
            speed = 0.0f;
            timeRemaining = 0;
        }
        if (timeRemaining == 0)
        {
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                audioSource.clip = backgroundmusic;
                audioSource.Play();
                audioSource.loop = true;
            }
            
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude); 

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(cogs != 0)
            {
                Launch();
                cogs--;
                ammoText.text = "Cogs: " + cogs.ToString();
            }
            
            
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                
                if (character != null)
                {
                    PlaySound(npctalk);
                    character.DisplayDialog();
                    
                }
                if (scoreValue >= 4)
                {
                    SceneManager.LoadScene("Finalproject"); 
                    level++;
                }
                
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (scoreValue >= 4)
        {
            winloseText.text = "Talk to Jambi to visit stage two!";

        
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

        }
        if (currentHealth <= 0)
        {
            winloseText.text = "Game Over! Press R to Restart";
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                audioSource.clip = backgroundmusic;
                audioSource.Play();
                audioSource.loop = true;
            }
            speed = 0.0f;
        }

        if (level == 2)
        {
            if (scoreValue >= 4)
            {
            winloseText.text = "You win! Game by: Alan Zeng";
            speed = 0.0f;
            }
        }
    }
    void FixedUpdate()
    {    
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;       

        rigidbody2d.MovePosition(position);

    }
    public void ChangeHealth(int amount)
    {
        if (amount <0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            ParticleSystem projectileObject = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }
        if (amount >0)
        {
            ParticleSystem projectileObject = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            audioSource.clip = gameovermusic;
            audioSource.Play();
            audioSource.loop = false;
        }
        if (level == 2)
        {
            if (currentHealth <= 0)
            {
            audioSource.clip = gameovermusic;
            audioSource.Play();
            audioSource.loop = false;
            speed = 0.0f;
            }
        }
    }
    public void ChangeScore(int amount)
    {
        scoreValue = scoreValue + amount;
        scoreText.text = "Fixed Robots: " + scoreValue.ToString();

        if (scoreValue >= 4)
            {
                audioSource.clip = victorymusic;
                audioSource.Play();
                audioSource.loop = false;
                timerIsRunning = false;
            }
    }

   
    

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ammo")
        {
            cogs += 3;
            ammoText.text = "Cogs: " + cogs.ToString();
            Destroy(collision.collider.gameObject);
            PlaySound(gearclip);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}