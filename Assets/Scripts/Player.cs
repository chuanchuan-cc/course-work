using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
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
        this.playerData.money = 1500;
        this.playerData.positionNo = 0;
        this.playerData.freezeTurn = 0;
        this.playerData.isBankrupt = false;
        this.playerData.assetsList = new List<string>();
        this.playerData.circle = 0;
        this.playerData.assetsWorth = 1500;

    }
    public void InitializeBank(){
         this.playerData = ScriptableObject.CreateInstance<PlayerData>();


        this.name = "Bank";
        this.playerData.money = 30000;
        this.playerData.positionNo = 0;
        this.playerData.freezeTurn = 0;
        this.playerData.isBankrupt = false;
        this.playerData.assetsList = new List<string>();
        this.playerData.circle = 0;
        this.playerData.assetsWorth = 30000;
    }

    private IEnumerator MovePiece(int steps)
    {
        isMoving=true;
        _animator.SetBool("isMoving", true);
        float jumpHeight = 0.5f;
        float jumpDuration = 0.5f;

        for (int i = 0; i < steps; i++)
        {
            playerData.positionNo = (playerData.positionNo + 1) % 40;             
            Vector2 startPos = _rigidbody.position;
            Vector2 targetPos = GetPosition(playerData.positionNo);

            float elapsedTime = 0;
            float totalTime = 0.2f;

            while (elapsedTime < totalTime)
            {
                float t = elapsedTime / totalTime;
                float heightFactor = Mathf.Sin(t * Mathf.PI) * jumpHeight;
                Vector2 interpolatedPos = Vector2.Lerp(startPos, targetPos, t);
                _rigidbody.position = new Vector2(interpolatedPos.x,interpolatedPos.y+heightFactor);
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
