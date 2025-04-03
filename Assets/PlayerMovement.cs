using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D _rigidbody;
    // public Transform[] boardPositions; // 存储所有棋盘格子的位置
    public int currentPositionIndex = 0;
    private float moveSpeed = 3f;
    public float jumpHeight = 0.5f;
    public int boardNo;
    // 初始时规定我的棋子的位置在哪里（为什么用rigidbody来做这个方法）
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();


        // new 是vectoe2需要的关键词，来定义我的棋子的初始位置在哪里，用向量/坐标轴的方法表示
        _rigidbody.position = new Vector2(5.5f, -3.5f);


        Debug.Log("初始位置：" + _rigidbody.position);

        boardNo = 0;
        for (int i = 0; i < 30; i++)
        {
            Debug.Log($"格子: {i}, 坐标为: {getPosition(i)}");
        }





    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 这里加入了随机数来计算骰子的点数是多少
            int dice1 = UnityEngine.Random.Range(1, 7);
            int dice2 = UnityEngine.Random.Range(1, 7);
            // 棋子移动端点数是两个骰子点数之和
            int steps = dice1 + dice2;
            //调用MovePiece的方法来把值赋给steps？ 
            MovePiece(_rigidbody, steps);
        }


    }


    public void MovePiece(Rigidbody2D rigidbody, int steps)
    {
        // 初始化变量
        float totalruntime = 1;
        float runtime = 0;
        Vector2 curposition = rigidbody.position;
        while (runtime < totalruntime)
        {
            // lep这个函数的用法，初始位置，目标位置，时间比例（作用范围是0-1）
            rigidbody.position = Vector2.Lerp(curposition, getPosition(boardNo + steps), runtime / totalruntime);
            runtime += Time.deltaTime;


        }
        boardNo += steps;
    }

    // 棋盘格的位置读取
    Vector2 getPosition(int boardNo)
    {
        // %30 是主要作用于第二轮的棋子的移动，第二圈的1，2，3...
        int newBoardNo = boardNo % 30;
        Vector2 position = Vector2.zero;
        // 看着制作的那个简易的地图来数格子，x&y的不同
        if (newBoardNo == 0)
        {
            position = new Vector2(5.5f, -3.5f);

        }
        // 从初始位置开始
        else if (newBoardNo <= 9)
        {
            position = new Vector2(5.5f - newBoardNo, -3.5f);

        }
        else if (newBoardNo <= 15)
        {
            // -9是前面走过的格数，它在y轴要走多少格
            position = new Vector2(-3.5f, -3.5f + newBoardNo - 9);



        }
        else if (newBoardNo <= 24)
        {
            // -15 是前面走过的格数,它在这个x轴要走多少格
            position = new Vector2(-3.5f + newBoardNo - 15, 2.5f);
        }
        else
        {
            // -24是前面走过的格数，它在这个y轴要走多少格
            position = new Vector2(5.5f, 2.5f - (newBoardNo - 24));


        }
        return position;
    }






    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_rigidbody.position, 0.1f); // 绘制玩家位置

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Camera.main.transform.position, 0.1f); // 绘制摄像机位置
    }








}
// using System.Collections;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     public Rigidbody2D _rigidbody;
//     public int boardNo; // 棋盘位置编号
//     private bool isMoving = false; // 防止重复移动
//     private float moveSpeed = 3f;
//     public float jumpHeight = 0.5f; // 跳跃高度

//     void Start()
//     {
//         _rigidbody = GetComponent<Rigidbody2D>();
//         _rigidbody.position = new Vector2(5.5f, -3.5f);
//         Debug.Log("初始位置：" + _rigidbody.position);
//         boardNo = 0;
//     }

//     void Update()
//     {
//         // 在 Unity 中，Input 相关的函数主要用于获取 玩家输入（键盘、鼠标、手柄等）
//         if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
//         {
//             int dice1 = UnityEngine.Random.Range(1, 7);
//             int dice2 = UnityEngine.Random.Range(1, 7);
//             int steps = dice1 + dice2;
//             Debug.Log($"掷骰子结果：{dice1} 和 {dice2}，前进 {steps} 步");
//             StartCoroutine(MovePiece(steps));
//             // 这一行代码的作用是：
//             // “启动一个 Coroutine（协程）来执行 MovePiece(steps) 这个移动逻辑。”
//             // 为什么要用 Coroutine？
//             // Unity 的 Update() 每一帧都会执行代码，但是 如果用 while 直接让棋子移动，它会瞬间完成所有动作。
//             // Coroutine 可以 逐帧执行代码，让棋子平滑移动，而不是瞬间跳过去
//         }
//     }

//     private IEnumerator MovePiece(int steps)
//     {
//         isMoving = true;
//         for (int i = 0; i < steps; i++)
//         {
//             boardNo++;
//             Vector2 startPos = _rigidbody.position;
//             Vector2 targetPos = getPosition(boardNo);

//             float elapsedTime = 0;
//             float totalTime = 1f / moveSpeed;

//             while (elapsedTime < totalTime)
//             {
//                 float t = elapsedTime / totalTime;
//                 _rigidbody.position = Vector2.Lerp(startPos, targetPos, t);
//                 // +：把水平方向（Lerp）和垂直跳跃（Sin）的位移相加，得到最终位置
//                 // *：用来缩放数值（例如 Sin() 结果 × jumpHeight 来控制跳跃高度）

//                 elapsedTime += Time.FixeddeltaTime;
//                 // Time.deltaTime 代表当前帧的时间间隔（每一帧所花费的时间）。所以运行时间是通过每一帧增加到时间来计算的
//                 // 为什么要用 Time.deltaTime？ 因为 Unity 的 Update() 是每一帧执行一次，而帧率（FPS）是不稳定的，deltaTime 让代码适应不同的帧率，保持动画流畅。
//                 //如果游戏帧率是 60 FPS（每秒 60 帧）
//                 // 每一帧 Time.deltaTime ≈ 1 / 60 = 0.0167s
//                 // 如果游戏帧率是 30 FPS（每秒 30 帧）
//                 // 每一帧 Time.deltaTime ≈ 1 / 30 = 0.0333s
//                 // 这样可以保证：
//                 // 无论 FPS 多少，棋子都能在 duration 秒内移动完成
//                 // 不会因为 FPS 高就移动太快，或 FPS 低就移动太慢
//不同的电脑显示画面一样    
//                 yield return null;
//             }
//当while结束后，确定目标位置  
//             _rigidbody.position = targetPos; // 确保最终位置精确
//             yield return new WaitForSeconds(0.2f); // 短暂停留
//         }

//         isMoving = false;
//     }

//     Vector2 getPosition(int boardNo)
//     {
//         int newBoardNo = boardNo % 30;
//         if (newBoardNo == 0) return new Vector2(5.5f, -3.5f);
//         if (newBoardNo <= 9) return new Vector2(5.5f - newBoardNo, -3.5f);
//         if (newBoardNo <= 15) return new Vector2(-3.5f, -3.5f + newBoardNo - 9);
//         if (newBoardNo <= 24) return new Vector2(-3.5f + newBoardNo - 15, 2.5f);
//         return new Vector2(5.5f, 2.5f - (newBoardNo - 24));
//     }

//     void OnDrawGizmos()
//     {
//         if (_rigidbody != null)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawSphere(_rigidbody.position, 0.1f);
//         }
//     }
// }

