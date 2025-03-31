using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

public class Player : MonoBehaviour
{
    public PlayerDisplay playerDisplay;
    public string name;
    public PlayerData playerData;
    private Rigidbody2D _rigidbody;
    private Animator _animator;


    public bool isMoving = false;
    public float inputX, inputY;
    public float facex;
    public bool isFading=false;
    public TileGenerator tileGenerator;
   

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
    public void directlyMove(Board board){
        Debug.Log($"已触发直接移动，目的地为{board.property}");
        StartCoroutine(DirectMoveRoutine(board));
        

    }
    
private IEnumerator DirectMoveRoutine(Board board)
{   isMoving=true;
    yield return StartCoroutine(FadeOut(0.5f));

    yield return new WaitUntil(()=>!isFading);
    _rigidbody.position = GetPosition(board.positionNo);
    playerData.positionNo = board.positionNo;

    yield return StartCoroutine(FadeIn(0.5f, _rigidbody.position));
    isMoving=false;
}
    IEnumerator FadeOut( float duration)
{
    yield return new WaitUntil(()=>!isFading);
    isFading=true;
    SpriteRenderer  renderer = this.GetComponent<SpriteRenderer >();
    Color color = renderer.material.color;

    float startAlpha = color.a;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
        color.a = newAlpha;
        renderer.material.color = color;
        yield return null;
    }

    color.a = 0f;
    renderer.material.color = color;
    isFading=false;
}
    IEnumerator FadeIn( float duration,Vector2 v)
{
    yield return new WaitUntil(()=>!isFading);
    _rigidbody.position=v;
    isFading=true;
    SpriteRenderer  renderer = this.GetComponent<SpriteRenderer >();
    Color color = renderer.material.color;

    float startAlpha = color.a;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float newAlpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
        color.a = newAlpha;
        renderer.material.color = color;
        yield return null;
    }

    color.a = 1f;
    renderer.material.color = color;
    isFading=false;
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
   
    
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
        this.playerData = new PlayerData(); 
        this.name = name;
        this.playerData.name=name;
        this.playerData.money = 1500;
        this.playerData.positionNo = 0;
        this.playerData.freezeTurn = 0;
        this.playerData.isBankrupt = false;
        this.playerData.assetsList = new List<Board>();
        this.playerData.circle = 1;
        this.playerData.assetsWorth = 1500;
        this.positionInitialize(0);

    }
    public void loadPlayer(PlayerData pd){
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
            this.playerData=pd;
            this.name=pd.name;
            this.positionInitialize(pd.positionNo);
    }

    private void positionInitialize(int n)
    {
        _rigidbody.position= GetPosition(n);
        Debug.Log($"将{this.name}初始化指第{n}格");
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
                if(name=="ship")
                {
                    
                    _rigidbody.position = new Vector2(interpolatedPos.x,interpolatedPos.y);
                    elapsedTime += Time.deltaTime;
                yield return null;
                }
                else
                {
                if(isHorizon)
                {
                _rigidbody.position = new Vector2(interpolatedPos.x,interpolatedPos.y+heightFactor);
                if(playerData.positionNo<=15)
                {
                    inputX=-1;
                    inputY=0;

                }else if(
                    playerData.positionNo<=20)
                    {
                    inputX=0;
                    inputY=1;}
                else if(playerData.positionNo<=35)
                {
                    inputX=1;
                    inputY=0;
                }
                else
                {
                    inputX=0;
                    inputY=-1;
                }
                facex=inputX;
                _animator.SetFloat("inputX",inputX);
                _animator.SetFloat("inputY",inputY);
                }
                
                else
                {
                    if(startPos.y-targetPos.y<0)
                    {
                        _rigidbody.position = new Vector2(interpolatedPos.x-heightFactor*1f,interpolatedPos.y+heightFactor*1f);
                    }
                    else
                    {
                        _rigidbody.position = new Vector2(interpolatedPos.x+heightFactor*1f,interpolatedPos.y+heightFactor*1f);
                    }

                }
                                    elapsedTime += Time.deltaTime;
                yield return null;
                
            
        
            }
            }


            _rigidbody.position = targetPos; 
            //播放格子下沉动画
            //将等待改为动画播放完毕
            StartCoroutine(moveBoardAnimation());
            StartCoroutine(tileGenerator.BoardAnimation(playerData.positionNo));
            yield return new WaitUntil(()=>!tileGenerator.isAnimationBoard);

            //yield return new WaitForSeconds(0.2f);
        

            
            
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
    public IEnumerator moveBoardAnimation(){
  
    float total = 0.15f;
    float time=0f;
    Vector2 iniVect= _rigidbody.position;
    Vector2 target= new Vector3(iniVect.x,iniVect.y-0.04f);
    Vector2 velocity = Vector2.zero;
    while(time<total){
         _rigidbody.position = Vector2.SmoothDamp( _rigidbody.position, target, ref velocity, total - time);
        time+=Time.deltaTime;
         yield return  null;
    } 

    time=0f;
    velocity = Vector2.zero;
       while(time<total){
         _rigidbody.position = Vector2.SmoothDamp( _rigidbody.position, iniVect, ref velocity, total - time);
        time+=Time.deltaTime;
         yield return  null;
    }
     _rigidbody.position=iniVect;





}

  

}


public class Bank:IOwner
{
    public string name;
    public int money;
    public Bank()
    {
        this.money=50000;
        this.name="Bank";
    }

public string GetName()
{
    return "Bank";
}

}

