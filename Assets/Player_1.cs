using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
public class Player_1 : MonoBehaviour
{
    public string name;
    public int Position;
    public int lapsCompleted = 0;
    public int money = 1500;
    public bool isInJail = false;
    public bool isBankrupt = false;
    public List<string> assetsList;
    public int jailTurnsRemaing = 0;
    public int circle;
    public int  assetsWorth;
    public void Move(int steps, int totaltiles)
    {
        if (isInJail)
        {
            Debug.Log(name+"在监狱里，不能移动！");
            return;
            // 这里的return是提前退出函数，当这个条件为true时，会输出一条信息
            // 然后return语句会立即结束Move函数的执行，不再执行函数中剩余的代码
        }

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }



}



