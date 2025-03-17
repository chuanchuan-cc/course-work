using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public PlayerDisplay playerDisplay;
    public string name;
    public PlayerData playerData;
    private Rigidbody2D _rigidbody;
    private Animator _animator;


    public bool isMoving = false;
   

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (playerData != null)
        {
            Debug.Log($"{this.name} successfully loaded data");
        }
      
       
       
        
    }

    public void Move(int steps)
    {
        
        if (!isMoving)
        {
   
            Debug.Log($"{this.name} moves forward {steps} steps");
            isMoving=true;
            StartCoroutine(MovePiece(steps));
            
     
        }
    }
    public void InitializePlayer(string name)
    {
   
    

        this.playerData = ScriptableObject.CreateInstance<PlayerData>();
        this.name = name;
        this.playerData.name=name;
        this.playerData.money = 1500;
        this.playerData.positionNo = 0;
        this.playerData.freezeTurn = 0;
        this.playerData.isBankrupt = false;
        this.playerData.assetsList = new List<Board>();
        this.playerData.circle = 0;
        this.playerData.assetsWorth = 1500;

    }


    private IEnumerator MovePiece(int steps)
    {
        isMoving=true;
        _animator.SetBool("isMoving", true);
        float jumpHeight = 0.5f;
       
        

        for (int i = 0; i < Math.Abs(steps); i++)
        {
            playerData.positionNo = (steps>0)?(playerData.positionNo + 1) % 40:(playerData.positionNo - 1) % 40;             
            Vector2 startPos = _rigidbody.position;
            Vector2 targetPos = GetPosition(playerData.positionNo);

            float elapsedTime = 0;
            float totalTime = 0.2f;

            while (elapsedTime < totalTime)
            {
                float t = elapsedTime / totalTime;
                float heightFactor = (-4 * Mathf.Pow(t - 0.5f, 2) + 1)*jumpHeight;
                bool isHorizon=Math.Abs(startPos.x-targetPos.x)>Math.Abs(startPos.y-targetPos.y);
                Vector2 interpolatedPos = Vector2.Lerp(startPos, targetPos, t);
                if(isHorizon){
                _rigidbody.position = new Vector2(interpolatedPos.x,interpolatedPos.y+heightFactor);

                }else{
                    if(startPos.y-targetPos.y<0){
                        _rigidbody.position = new Vector2(interpolatedPos.x-heightFactor*1f,interpolatedPos.y+heightFactor*1f);
                    }else{
                        _rigidbody.position = new Vector2(interpolatedPos.x+heightFactor*1f,interpolatedPos.y+heightFactor*1f);
                    }
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rigidbody.position = targetPos; 

            yield return new WaitForSeconds(0.1f);
        

            
            
        }

        _animator.SetBool("isMoving", false);
        
        isMoving = false;
    }
   

    private Vector2 GetPosition(int No)
    {
        int newNo = No % 40;
        if (newNo == 0) return new Vector2(7.5f, -2.6f);
        else if (newNo <= 15) return new Vector2(7.5f - (float)newNo, -2.6f);
        else if (newNo <= 20) return new Vector2(-7.5f, (float)newNo - 15f - 2.6f);
        else if (newNo <= 35) return new Vector2(-7.5f + (float)newNo - 20f, 2.4f);
        else return new Vector2(7.5f, 2.4f - ((float)newNo - 35));
    }

  

}


public class Bank:IOwner{
    public string name;
    public int money;
    public Bank(){
        this.money=50000;
        this.name="Bank";
    }

public string GetName(){
    return "Bank";
}
}
