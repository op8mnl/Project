using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScottController : MonoBehaviour
{
    public float speed; //speed
    public float jump; //jump
    private Rigidbody2D _scott; //scott player
    private PolygonCollider2D basicAttack;
    private PolygonCollider2D strike;
    private bool _facingRight = true; //facing direction
    private bool _isOnHill = false;
    private bool _inPortal = false;
    private float _healthPoints = 100f;

    


    Animator scottAnim; //animator
    // Start is called before the first frame update
    public void Start()
    {
        //get components
        _scott = GetComponent<Rigidbody2D>();
        scottAnim = GetComponent<Animator>();
        basicAttack = GameObject.FindGameObjectWithTag("basicAttack").GetComponent<PolygonCollider2D>();
        strike = GameObject.FindGameObjectWithTag("strike").GetComponent<PolygonCollider2D>();
        
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        GetComponent<HealthManager>().setHealth(_healthPoints);

    }
    // Update is called once per frame
    void Update()
    {
        Movement();

        Attack();

        Beam();

        
    }
    
    //flip the direction of the player sprite
    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Movement()
    {

        //flip the character based on which direction ur moving
        var move = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Horizontal") > 0 && _facingRight == false)
        {
            Flip();
        }
        else if (Input.GetAxis("Horizontal") < 0 && _facingRight == true)
        {
            Flip();
        }

        //translate the position of the player
        transform.position += new Vector3(move, 0, 0) * Time.deltaTime * speed;

        //change animation when player is walking
        if (Input.GetAxis("Horizontal") != 0)
        {
            scottAnim.SetBool("IsWalking", true);
        }
        else
        {
            scottAnim.SetBool("IsWalking", false);
        }

        //Crouch animation
        if (Input.GetAxis("Vertical") < 0 && _inPortal == false && scottAnim.GetBool("IsWalking") == false)
        {
            scottAnim.SetBool("IsCrouch", true);
        }
        else
        {
            scottAnim.SetBool("IsCrouch", false);
        }

        //get input for player to jump, add impulse force for jump
        if (Input.GetButtonDown("Jump") && Mathf.Abs(_scott.velocity.y) < 0.001f)
        {
            if (_isOnHill)
            {
                Vector3 targetVelocity = new Vector2(move * 10f, _scott.velocity.y);
                _scott.AddForce(new Vector2(0, jump * _scott.gravityScale / 2), ForceMode2D.Impulse);
            } else
            {
                _scott.AddForce(new Vector2(0, jump), ForceMode2D.Impulse);
            }

        }
    }
    private void Attack()
    {
       
        //Attack 1 Animations
        if (Input.GetButtonDown("Attack1") && !scottAnim.GetCurrentAnimatorStateInfo(0).IsName("Scott_BasicAttack"))
        {
            scottAnim.SetTrigger("BasicAttack");
            basicAttack.enabled = true;
            StartCoroutine(DisableBasicAttackCollider());
        }

        //Attack 2 Animations
        if (Input.GetButtonDown("Attack2") && !scottAnim.GetCurrentAnimatorStateInfo(0).IsName("Scott_Strike"))
        {
            scottAnim.SetTrigger("Strike");
            strike.enabled = true;
            StartCoroutine(DisableStrikeCollider());
        }
    }
    private IEnumerator DisableStrikeCollider()
    {
        yield return new WaitForSeconds(0.03f);
        strike.enabled = false;
        StopCoroutine(DisableStrikeCollider());
    }
    private IEnumerator DisableBasicAttackCollider()
    {
        yield return new WaitForSeconds(0.04f);
        basicAttack.enabled = false;
        StopCoroutine(DisableBasicAttackCollider());
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Hill"))
        {
            _isOnHill = true;
            _scott.gravityScale = 3;
        }
        else
        {
            _isOnHill = false;
            _scott.gravityScale = 1.5f;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("portal1"))
        {
            _inPortal = true;
        }

    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("portal1"))
        {
            _inPortal = false;
        }
    }

    void Beam()
    {
        if (_inPortal == true && Input.GetButtonDown("Down"))
        {
            scottAnim.SetTrigger("Beam");
            Invoke("toggleVisibility", 1.25f);
            StartCoroutine(nextLevel(1.5f, 0, "right"));
        }

    }
    void toggleVisibility()
    {
        GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;

    }

    public void takeDamage(float damage){
        _healthPoints -= damage;
        GetComponent<HealthManager>().healthUpdate(_healthPoints);
    }

    IEnumerator nextLevel(float delayTime,int currentLevel, string direction)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.
        //GetComponent<LevelManager>().getNextLevel(currentLevel, direction);
        StopAllCoroutines();
    }


}
