using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class estatePrefabConfig : TilePrefabConfig
{
    public TextMeshPro property;
    public TextMeshPro price;
    public TextMeshPro rent;
    public TextMeshPro owner;
    public TextMeshPro improvedLevel;

    public float offsetStep = 0.001f; 
    private bool zOffsetApplied = false;

    public void updateConfigEstate(estateBoard board)
    {
        if (board != null)
    {
       
    
        property.text = board.property.ToString();
        price.text = $"price : {board.price}";
        rent.text = board.rent != 0 ? $"rent : {board.rent}" : "unknown";
        owner.text = $"owner : {board.owner.GetName()}";
        improvedLevel.text = $"build level : {board.improvedLevel}";

        ApplyZOffsetAndForceRender();}
    }

    public void updateConfigBuyable(BuyableBoard board)
    {
        if (board != null)
    {
   
       
    
        property.text = board.property.ToString();
        price.text = $"price : {board.price}";
        rent.text = board.rent != 0 ? $"rent : {board.rent}" : "unknown";
        owner.text = $"owner : {board.owner.GetName()}";

        ApplyZOffsetAndForceRender();
    }
    }

    private void ApplyZOffsetAndForceRender()
    {
        if (zOffsetApplied || Camera.main == null) return;

        Vector3 camDir = -Camera.main.transform.forward.normalized;

        int order = 0;
        foreach (var tmp in new TextMeshPro[] { property, price, rent, owner, improvedLevel })
        {
            if (tmp != null)
            {
               
                tmp.transform.position += camDir * (order * offsetStep);
                order++;

               
                var renderer = tmp.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.forceRenderingOff = false;
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                    renderer.receiveShadows = false;
                }
            }
        }

        zOffsetApplied = true;
    }
}
