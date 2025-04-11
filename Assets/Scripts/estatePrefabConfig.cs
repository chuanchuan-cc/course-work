using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class estatePrefabConfig : TilePrefabConfig
{
    public TextMeshPro property;
    public TextMeshPro price;
    public TextMeshPro rent;
    public TextMeshPro owner;
    public SpriteRenderer building;
    public Sprite[] buildings;

    public float offsetStep = 0.001f;
    private bool zOffsetApplied = false;

    public void updateConfigEstate(estateBoard board)
    {
      
        if (board != null)
        {

            if (property != null)
                property.text = board.property.ToString();
            if (price != null)
                price.text = $"price : {board.price}";
            if (rent != null)
                rent.text = board.rent != 0 ? $"rent : {board.rent}" : "unknown";
            if (owner != null)
                owner.text = $"owner : {board.owner.GetName()}";
            switch(board.improvedLevel){
            case 0:
            building.gameObject.SetActive(false);
            break;
            case 1:
            building.gameObject.SetActive(true);
            building.sprite=buildings[0];
            break;
            case 2:
            building.gameObject.SetActive(true);
            building.sprite=buildings[1];
            break;
            case 3:
            building.gameObject.SetActive(true);
            building.sprite=buildings[2];
            break;
            case 4:
            building.gameObject.SetActive(true);
            building.sprite=buildings[3];
            break;
            case 5:
            building.gameObject.SetActive(true);
            building.sprite=buildings[4];
            break;
            }
            
            SetBuildingPosition(board);

            ApplyZOffsetAndForceRender();
        }
    }
    private void SetBuildingPosition(estateBoard board)
{
    if (building==null) return;

    Vector3 offset=Vector3.zero;

    if (board.positionNo>=0&&board.positionNo<15)
        offset=new Vector3(0f, 0.5f, 0f);
    else if (board.positionNo>=15&&board.positionNo<20)
        offset=new Vector3(-0.85f, -0.25f, 0f);
    else if (board.positionNo>=20&&board.positionNo<35)
        offset=new Vector3(0f, 0.5f, 0f);
    else if (board.positionNo>=35&&board.positionNo<40)
        offset=new Vector3(0.85f, -0.25f, 0f);

    building.transform.localPosition=offset;
    Debug.Log($"Set building position to {offset}, on tile {board.property}, improvedlevel = {board.improvedLevel}");

}

    public void updateConfigBuyable(BuyableBoard board)
    {
        if (board != null)
        {


            if (property != null)
                property.text = board.property.ToString();
            if (price != null)
                price.text = $"price : {board.price}";
            if (rent != null)
                rent.text = board.rent != 0 ? $"rent : {board.rent}" : "unknown";
            if (owner != null)
                owner.text = $"owner : {board.owner.GetName()}";

            ApplyZOffsetAndForceRender();
        }
    }

    private void ApplyZOffsetAndForceRender()
    {
        if (zOffsetApplied || Camera.main == null) return;

        Vector3 camDir = -Camera.main.transform.forward.normalized;

        int order = 0;
        foreach (var tmp in new TextMeshPro[] { property, price, rent, owner})
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
