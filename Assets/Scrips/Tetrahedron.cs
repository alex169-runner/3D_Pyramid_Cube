using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron : MonoBehaviour
{
    public int pos;

    public void Relocate()
    {
        Centre.instance.tetra[pos] = this;
    }

    public void ChangePos(int top, bool reversed)
    {
        int code = ((pos & (15 << (top + 1))) >> 1) + (((1 << top) - 1) & pos);
        if (code == 0) return;
        if ((top & 1) != 0) {
            reversed = !reversed;
        }
        if (reversed) {
            code = (code >> 1) + ((code & 1) << 2);
        }
        else {
            code = (code >> 2) + ((code & 3) << 1);
        }
        pos = ((code & (15 << top)) << 1) + (1 << top) + (((1 << top) - 1) & code);
    }
}
