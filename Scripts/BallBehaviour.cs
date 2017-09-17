using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Game rules are defined here...
/// The score is 0-0, ball starts before player1, with the rigidbody asleep,
/// A player is allowed to take 3 chances to hit the ball, before the point is given to the other player
/// If the ball touches the ground, then the other player gets rewarded the point
/// The ball does bounce off the wall and the nets
/// Once the ball dies (falls to the ground or 3 chances are over) then it gets reset above the player that won the point
/// 2 Players race to 15 points, if 15-15, then the player who keeps a distance of 2 points wins
/// 
/// </summary>
public class BallBehaviour : MonoBehaviour {

    Rigidbody2D rigidBody;
    public GameObject playerAScore;
    public GameObject playerBScore;
    public static bool pointWon;
    public AudioSource whistle;
    public GameObject location;
    public static bool ballCollidedWithGround;
    public static Vector2 predictedPoint;
    int hScore;
    public GameObject player;

	
    // Use this for initialization
	
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.gravityScale = 0f;
        hScore = PlayerPrefs.GetInt("HighScore");
    }

    private void Update() {
        //If the y position of the ball is greater than 7.5f, then show the ball indicator for better ball location
        if (transform.localPosition.y >= 7.5f) {
            print("Should show the location sprite of the ball");
            location.SetActive(true);
            location.transform.position = new Vector2(transform.position.x, location.transform.position.y);
        }
        else {
            location.SetActive(false);
        }

    }

    private void FixedUpdate() {
        predictedPoint = new Vector2(transform.position.x, transform.position.y) + rigidBody.velocity * Time.deltaTime;
        predictedPoint.y = -1.5f;
        //print(predictedPoint.x);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(rigidBody.gravityScale == 0)
            rigidBody.gravityScale = 0.4f;
        if(rigidBody.freezeRotation)
            rigidBody.freezeRotation = false;

        if (GameConstants.mode == "Single") {
            if ((collision.collider.name == "GroundA" || (GameConstants.hitCount == 3 && collision.collider.name == "Player")) && !pointWon) {
                pointWon = true;
                ballCollidedWithGround = true;
                initialPosition = new Vector2(GameConstants.playSideBallPosition.x * -1, GameConstants.playSideBallPosition.y);
                rigidBody.velocity = Vector2.zero;
                rigidBody.Sleep();
                GetComponent<AudioSource>().Stop();
                whistle.Play();
                StartCoroutine(ResetPosition());
                //  print("B scored a point");
            }

            if ((collision.collider.name == "GroundB" || (GameConstants.hitCount == 3 && collision.collider.name == "Opponent")) && !pointWon) {
                pointWon = true;
                ballCollidedWithGround = true;
                initialPosition = GameConstants.playSideBallPosition;
                GameObject.Find("Opponent").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                rigidBody.velocity = Vector2.zero;
                rigidBody.Sleep();
                //Play the whistle
                whistle.Play();
                StartCoroutine(ResetPosition());
            }

            if (collision.collider.tag == "PlayerA") {
                GameConstants.hitCount++;
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(predictedPoint, 0.5f);
    }

    IEnumerator ResetPosition() {
        yield return new WaitForSeconds(Random.Range(2, 3));
        transform.position = initialPosition;
        rigidBody.gravityScale = 0f;
        rigidBody.velocity = Vector2.zero;
        ballCollidedWithGround = false;
        rigidBody.WakeUp();
        GameObject.Find("Opponent").GetComponent<Rigidbody2D>().isKinematic = false;
        yield return new WaitForSeconds(0.2f);
        pointWon = false;
        AIBehaviour.waiting = false;
        connected = false;
        if (initialPosition.x == GameConstants.playSideBallPosition.x) {
            //A won the point
            GameConstants.playerAScore++;
            playerAScore.GetComponent<UnityEngine.UI.Text>().text = GameConstants.playerAScore.ToString();
        }
        else {
		//B won the point
            GameConstants.playerBScore++;
            playerBScore.GetComponent<UnityEngine.UI.Text>().text = GameConstants.playerBScore.ToString();
        }
    }

}
