using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetTarget //pra criar e pegar o target do objeto a partir do parent (gerenciar no objeto pai)
{
    GameObject GetTarget();
}

//utilizado em:
//GunControllerEnemyMonkey <-- EnemyControllerMonkey

//todos os inimigos para pegar o target/player