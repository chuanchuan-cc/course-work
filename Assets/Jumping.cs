using System.Collections;
using UnityEngine;

public class Jumping : MonoBehaviour
{
    public Rigidbody2D _rigidbody;
    public Animator animator;
    public int boardNo; // 棋盘位置编号
    private bool isMoving = false; // 防止重复移动
    private float moveSpeed = 3f;
    public float jumpHeight = 0.5f; // 跳跃高度

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _rigidbody.position = new Vector2(10.5f, -3.5f);
        Debug.Log("初始位置：" + _rigidbody.position);
        boardNo = 0;
    }

    void Update()
    {
        // 在 Unity 中，Input 相关的函数主要用于获取 玩家输入（键盘、鼠标、手柄等）
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            int dice1 = UnityEngine.Random.Range(1, 7);
            int dice2 = UnityEngine.Random.Range(1, 7);
            int steps = dice1 + dice2;
            Debug.Log($"掷骰子结果：{dice1} 和 {dice2}，前进 {steps} 步");
            StartCoroutine(MovePiece(steps));
            // 这一行代码的作用是：
            // “启动一个 Coroutine（协程）来执行 MovePiece(steps) 这个移动逻辑。”
            // 为什么要用 Coroutine？
            // Unity 的 Update() 每一帧都会执行代码，但是 如果用 while 直接让棋子移动，它会瞬间完成所有动作。
            // Coroutine 可以 逐帧执行代码，让棋子平滑移动，而不是瞬间跳过去
        }
    }
    private IEnumerator MovePiece(int steps)
    {
        isMoving = true;
        animator.SetBool("isMoving", true);

        for (int i = 0; i < steps; i++)
        {
            boardNo++;
            Vector2 startPos = _rigidbody.position;
            Vector2 targetPos = getPosition(boardNo);

            float elapsedTime = 0;
            float totalTime = 1f / moveSpeed;

            while (elapsedTime < totalTime)
            {
                float t = elapsedTime / totalTime;
                _rigidbody.position = Vector2.Lerp(startPos, targetPos, t);
                // +：把水平方向（Lerp）和垂直跳跃（Sin）的位移相加，得到最终位置
                // *：用来缩放数值（例如 Sin() 结果 × jumpHeight 来控制跳跃高度）

                elapsedTime += Time.fixedDeltaTime;
                // Time.deltaTime 代表当前帧的时间间隔（每一帧所花费的时间）。所以运行时间是通过每一帧增加到时间来计算的
                // 为什么要用 Time.deltaTime？ 因为 Unity 的 Update() 是每一帧执行一次，而帧率（FPS）是不稳定的，deltaTime 让代码适应不同的帧率，保持动画流畅。
                //如果游戏帧率是 60 FPS（每秒 60 帧）
                // 每一帧 Time.deltaTime ≈ 1 / 60 = 0.0167s
                // 如果游戏帧率是 30 FPS（每秒 30 帧）
                // 每一帧 Time.deltaTime ≈ 1 / 30 = 0.0333s
                // 这样可以保证：
                // 无论 FPS 多少，棋子都能在 duration 秒内移动完成
                // 不会因为 FPS 高就移动太快，或 FPS 低就移动太慢
                //不同的电脑显示画面一样    
                yield return null;
            }
            //当while结束后，确定目标位置  
            _rigidbody.position = targetPos; // 确保最终位置精确
            yield return new WaitForSeconds(0.2f); // 短暂停留
        }
        animator.SetBool("isMoving", false);
        isMoving = false;
    }

    Vector2 getPosition(int boardNo)
    {
        int newBoardNo = boardNo % 40;
        if (newBoardNo == 0) return new Vector2(10.5f, -3.5f);
        if (newBoardNo <= 14) return new Vector2(10.5f - newBoardNo, -3.5f);
        if (newBoardNo <= 20) return new Vector2(-3.5f, -3.5f + newBoardNo - 14);
        if (newBoardNo <= 34) return new Vector2(-3.5f + newBoardNo - 20, 2.5f);
        return new Vector2(10.5f, 2.5f - (newBoardNo - 34));
    }
}

//     void OnDrawGizmos()
//     {
//         if (_rigidbody != null)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawSphere(_rigidbody.position, 0.1f);
//         }
//     }
// }


