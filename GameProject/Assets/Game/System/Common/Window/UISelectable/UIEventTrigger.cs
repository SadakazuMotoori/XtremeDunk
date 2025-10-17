using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;
using R3.Triggers;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

using UnityEngine.EventSystems;

//==================================================================
/// <summary>
/// EventTriggerを継承することで、親への伝播はなくなるよう(なんでやねん)
/// ※結局、EventTriggerが階層になると伝播するのであまり意味ない…
/// </summary>
//==================================================================
public class UIEventTrigger : EventTrigger, IPointerMoveHandler
{
    public virtual void OnPointerMove(PointerEventData eventData)
    {
//        DebugLogger.Log($"OnPointerMove {name}", DebugLogger.Colors.orange);
    }
}
