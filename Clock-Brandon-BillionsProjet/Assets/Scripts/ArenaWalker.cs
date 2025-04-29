using UnityEngine;

public class ArenaWalker
{

    


    public Vector2 pos;
    public Vector2 dir;
    public float changeChance;

    public ArenaWalker(Vector3 pos, Vector2 dir, float changeChance)
    {
        this.pos = pos;
        this.dir = dir;
        this.changeChance = changeChance;
    }


}
