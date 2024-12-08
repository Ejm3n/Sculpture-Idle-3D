using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarFragDetachHandler : MonoBehaviour
{
    private PillarObject pillar;
    bool hasPillar;
    public void SetPillar(PillarObject pillar)
    {
        this.pillar = pillar;
        pillar.BindSegmentFragDetach(FragDetach);
        hasPillar = true;
    }

    private void OnDestroy()
    {
        if(hasPillar)
        pillar.UnbindSegmentFragDetach(FragDetach);
    }
    protected virtual void FragDetach(Rigidbody frag)
    {
        frag.gameObject.SetActive(false);
    }
}
