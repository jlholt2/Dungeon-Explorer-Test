using UnityEngine;

public enum BlockType { Wall = 0, Floor = 1, Invisible = 2, BottomGone = 3 }

[System.Serializable]
public class ColorToTile
{
    public Color color;
    public BlockType blockType;

    public void PrepareBlock(Transform transform)
    {
        if(blockType == BlockType.Floor || blockType == BlockType.Invisible)
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        }
        if(blockType == BlockType.BottomGone || blockType == BlockType.Invisible)
        {
            transform.Find("BotBlock").GetComponent<MeshRenderer>().enabled = false;
        }
        //switch (blockType)
        //{
        //    default:
        //        break;
        //    case BlockType.Floor:
        //        transform.GetComponent<MeshRenderer>().enabled = false;
        //        break;
        //    case BlockType.BottomGone:
        //        transform.Find("BotBlock").GetComponent<MeshRenderer>().enabled = false;
        //        break;
        //    case BlockType.Invisible:
        //        transform.GetComponent<MeshRenderer>().enabled = false;
        //        transform.Find("BotBlock").GetComponent<MeshRenderer>().enabled = false;
        //        break;
        //}
    }
}
