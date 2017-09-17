# VolleyBall-AI
I recently worked on a 2D Volleyball game and developed a fairly decent AI for it. Here's the source code.

There're 3 scripts
1. Player.cs
2. AIBehaviour.cs
3. BallBehaviour.cs

3 Rigidbodies in the scene for the Player, the ball and the AI bot

The sprites setup is shown in the figure below:
![git](https://user-images.githubusercontent.com/22652777/30525123-2a5be2bc-9c1e-11e7-956d-000f1a453188.png)

I've created a simple State Machine for the AI volleyball player.
This state machine has 4 states represented by 4 coroutines...
1. Wait() - When the ball is on the Player's side, then simply wait for the ball to be hit.
2. Serve() - The ball is on the AI's side, and so get ready to jump and volley.
3. Jump() - When the ball is within the range of the Ai bot, then simply jump
4. MoveTowardsPrediction() - predict where the ball will land and simply move towards that position

``
        
        private void FixedUpdate() {
        grounded = Physics2D.OverlapCircle(groundChecker.transform.position, groundCheckRadius, whatIsGround);

        if (GameController.playStarted && !BallBehaviour.connected) {           
            //Wait
            if (ball.transform.position.x < 0 && ballRigidbody.gravityScale == 0) {
                StartCoroutine(Wait());
            }
            //My serve
            else if (ball.transform.position.x > 0 && ballRigidbody.gravityScale == 0) {
                StartCoroutine(Serve());
            }
            //Jump
            else if (Mathf.Abs(ball.transform.position.x - transform.position.x) < 5 &&  Mathf.Abs(ball.transform.position.y - transform.position.y) < Random.Range(4,5) && Time.time > 0.3f + nextChangeTime) {
                //Jump every 0.3 seconds
                nextChangeTime = 0.3f + Time.time;
                StartCoroutine(Jump());
            }
            else if (Mathf.Abs(ball.transform.position.x - transform.position.x) < 5 && Mathf.Abs(ball.transform.position.x - transform.position.x) > 3 && Time.time > 0.3f + nextChangeTime) {
                //Jump every 0.3 seconds
                nextChangeTime = 0.3f + Time.time;
                StartCoroutine(Jump1());
            }

            //Move towards the predicted position
            if (Mathf.Sign(BallBehaviour.predictedPoint.x) == Mathf.Sign(transform.position.x) && BallBehaviour.impactPos.x > 0 && ballRigidbody.velocity.x < 10f && ballRigidbody.gravityScale != 0) {
                StartCoroutine(MoveTowardsPrediction(BallBehaviour.predictedPoint));
            }
            else if (Mathf.Sign(BallBehaviour.predictedPoint.x) == Mathf.Sign(transform.position.x)  && BallBehaviour.impactPos.x > 0 && ballRigidbody.velocity.x > 10f && BallBehaviour.predictedPoint.x > center.transform.position.x && ballRigidbody.gravityScale != 0) {
                BallBehaviour.predictedPoint.x = 8.87f - BallBehaviour.predictedPoint.x;
                StartCoroutine(MoveTowardsPrediction(BallBehaviour.predictedPoint));
            }
        }
    }
``

On Collision With the player, the ball will be hit with a much random direction

``
    private void OnCollisionEnter2D(Collision2D collision) {
        //Apply force to the ball so that it moves in the proper direction when hit by the player Controller
        if (collision.collider.CompareTag("Ball")) {
            ball.GetComponent<AudioSource>().Play();
            ballRigidbody.isKinematic = false;
            valueGiven = false;
            Vector2 diff = (ballRigidbody.transform.position - transform.position).normalized;
            //Ball is on the right
            if (diff.x >= 0) {
                //Find direction
                Vector2 difference = ball.transform.position - transform.position;
                float angle = Vector2.Angle(Vector2.right, difference);
                float randomAngle = Random.Range(angle, 90 - angle);
                Vector3 dir = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector3.right;
                ballRigidbody.velocity = dir * ballForce;
            }
            else if (diff.x < 0) {
                Vector2 difference = ball.transform.position - transform.position;
                float angle = Vector2.Angle(Vector2.right, difference);
                float randomAngle = Random.Range(angle - 15, angle+ 15);
                //  print(randomAngle);
                Vector3 dir = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector3.right;
                //rigidBody.AddForce(dir * ballForce);
                ballRigidbody.velocity = dir * ballForce;
            }
        }
    }
    
    ``
