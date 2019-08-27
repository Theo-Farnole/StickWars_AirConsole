using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class CharacterEntity : Entity
{
    private int _myID = -1;

    protected override void Start()
    {
        base.Start();

        _myID = (int)GetComponent<CharController>().charID;
    }

    protected override void Death(Entity killer)
    {
        // retrieve killer ID
        CharController killerCharController = killer.GetComponent<CharController>();

        int killerID = -1;

        if (killerCharController)
        {
            killerID = (int)killerCharController.charID;
        }

        // report kill to Gamemode
        GameManager.Instance.Gamemode.Kill(killerID, _myID);        

        // respawn player
        transform.position = LevelData.Instance.GetRandomSpawnPoint().position;
        _hp = MaxHp;

        UpdateHealthSlider();

        // report death to CharController
        GetComponent<CharController>().Respawn();

        // feedback
        CameraShake.Instance.Shake();
    }
}
