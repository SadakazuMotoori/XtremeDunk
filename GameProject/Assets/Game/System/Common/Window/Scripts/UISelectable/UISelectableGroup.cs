using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using R3;
using R3.Triggers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

//==================================================================
/// <summary>
/// UISelectableグループ
/// ・UISelectableは、こいつを親にして動作する。
/// ・同グループのUISelectableのみ、遷移できる。
/// </summary>
//==================================================================
public class UISelectableGroup : MonoBehaviour
{
    public static UISelectableGroup s_GlobalGroup { get; set; } = null;

    [Header("初期選択")]
    [SerializeField] UISelectable _firstSelected;

//    [Header("サウンド")]
//    [SerializeField] ISoundManager.UISEs _uiseCursorMove = ISoundManager.UISEs.Select01;
//    [SerializeField] ISoundManager.UISEs _uiseDisable = ISoundManager.UISEs.Error01;

    [Header("連動カーソル")]
    [SerializeField] CursorObjectData _cursorObjectData;

    [Header("グロ―バルグループとして登録")]
    [SerializeField] bool _isGlobalGroup;

    [Header("決定操作の遅延時間")]
    [SerializeField] float _startDecisionDelayTime = 0;

    // 
    List<UISelectable> _members = new();
    public IReadOnlyList<UISelectable> Members => _members;

    UISelectable GetTopSelectable()
    {
        foreach (var s in _members)
        {
            if (s.isActiveAndEnabled == false) continue;
            return s;
        }
        return null;
    }

    List<UISelectable> _reservedAdd = new();

    // 初期化
    UniTaskCompletionSource _initializedTask = new();
    public UniTask WaitForInitialized() => _initializedTask.Task;

    UISelectable _currentSelected;
    float _startTime = -1;

    // 現在選択中のもの
    public UISelectable CurrentSelected
    {
        get => _currentSelected;
        set
        {
            if (_currentSelected == value) return;

            // 現在選択のエフェクトを解除
            if(_currentSelected != null)
            {
                _currentSelected.SetFocusedFlag(false);
                _cursorObjectData.Enable(false);
            }

            _currentSelected = value;
            // 連動カーソル
            _cursorObjectData.SetTarget(value);
            //            OnSelectChanged.OnNext(_currentSelected);

            // 新選択のエフェクトを有効に
            if (_currentSelected != null)
            {
                _currentSelected.SetFocusedFlag(true);

                _cursorObjectData.Enable(true);
            }
        }
    }

    //==================================================
    //
    // イベント
    //
    //==================================================
    // 選択が変更された
    public Subject<UISelectable> OnSelectChanged { get; } = new();


    //==================================================

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitSubsystemRegistration()
    {
        s_GlobalGroup = null;
    }

    void Awake()
    {
        if(_isGlobalGroup)
        {
            s_GlobalGroup = this;
        }

        _cursorObjectData.Owner = this;
        _cursorObjectData.Enable(false);
    }

    void Start()
    {
        // 1フレームの一回イベントを発生させたいため、ここでやる
        Observable.EveryValueChanged(this, x => CurrentSelected)
            .Where(_ => enabled)
            .Subscribe(selectable =>
            {
                Debug.Log("変更");
                OnSelectChanged.OnNext(selectable);
            });

        // 選択変更時(↑以外でもイベント発生するため)
        OnSelectChanged.Subscribe(_ =>
        {
//            ISoundManager.Instance.PlaySE_2D(
//                ISoundManager.Instance.GetUISE(_uiseCursorMove)
//            );
        });


        // 更新処理
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                // 追加予約がある場合
                UpdateReservedItems();

                // カーソル処理
                _cursorObjectData.Update();
            });

        // 
        _cursorObjectData.FitTargetPos();

        // 初期化終了
        _initializedTask.TrySetResult();
    }

    public void ForceExecOnSelectChanged()
    {
        if (CurrentSelected == null) return;

        OnSelectChanged.OnNext(CurrentSelected);
    }

    public UISelectable GetByID(string strID)
    {
        foreach (var m in _members)
        {
            if (m.IDString == strID)
            {
                return m;
            }
        }
        return null;
    }
    public UISelectable GetByID(int intID)
    {
        foreach (var m in _members)
        {
            if (m.IDInt == intID)
            {
                return m;
            }
        }
        return null;
    }

    // IDで選択
    public void SelectByID(string strID)
    {
        foreach(var m in _members)
        {
            if(m.IDString == strID)
            {
                CurrentSelected = m;
                break;
            }
        }
    }
    // IDで選択
    public void SelectByID(int intID)
    {
        foreach (var m in _members)
        {
            if (m.IDInt == intID)
            {
                CurrentSelected = m;
                break;
            }
        }
    }

    // 子からの通知
    public void Join(UISelectable selectable)
    {
        _reservedAdd.Add(selectable);
    }

    public void UpdateReservedItems()
    {
        if (_reservedAdd.Count >= 1)
        {
            // 本リストに追加
            foreach (var selectable in _reservedAdd)
            {
                if (_members.Contains(selectable)) continue;

                _members.Add(selectable);
            }
            _reservedAdd.Clear();

            // 本リストをソート
            _members.Sort((a, b) => b.Priority - a.Priority);

            // 未選択なら、選択する
            if (CurrentSelected == null)
            {
                CurrentSelected = GetTopSelectable();
            }
        }

    }

    public void Leave(UISelectable selectable)
    {
        if(CurrentSelected == selectable)
        {
            CurrentSelected = null;
        }

//        selectable.SetFocusedFlag(false);
        _members.Remove(selectable);
    }

    private void OnEnable()
    {
        if (_firstSelected != null)
        {
            CurrentSelected = _firstSelected;
        }

    }

    //==================================================================
    //
    // カーソルの処理を実行
    // ・選択変更
    // ・決定で処理を実行
    //
    //==================================================================
    public async UniTask<(UISelectable.Actions action, UISelectable select)> UpdateCursor()
    {
        if(_startTime < 0)
        {
            _startTime = Time.unscaledTime;
        }

        bool enableDecision = false;
        if (_startTime >= 0 && Time.unscaledTime >= _startDecisionDelayTime + _startTime)
        {
            enableDecision = true;
        }

        // 操作不能中
//        if (IAppManager.Instance != null && IAppManager.Instance.IsProcessingCurtain)
//            return (UISelectable.Actions.None, null);

        // 無効化状態
        if(isActiveAndEnabled == false)
        {
            return (UISelectable.Actions.None, null);
        }

        var inputUI = PlayerInputManager.Instance.UIAction;

        try
        {

            if (CurrentSelected != null)
            {
                // 無効物を選択している
                if(CurrentSelected.IsActive() == false)
                {
                    foreach (var m in _members)
                    {
                        if(m.IsActive())
                        {
                            CurrentSelected = m;
                            break;
                        }
                    }
                }

                // 移動
                if (inputUI.AxisLeft)
                {
                    CurrentSelected.SelectLeft();
                }
                if (inputUI.AxisRight)
                {
                    CurrentSelected.SelectRight();
                }
                if (inputUI.AxisUp)
                {
                    CurrentSelected.SelectUp();
                }
                if (inputUI.AxisDown)
                {
                    CurrentSelected.SelectDown();
                }

                // 決定
                if (enableDecision)
                {

                    // ボタン専用
                    if (inputUI.Decide)
                    {
                        CurrentSelected.ClearAction();

                        // 決定時処理
                        if (CurrentSelected.IsInteractable())
                        {
                            await CurrentSelected.ExecDecideProc(true);

                            if(CurrentSelected != null)
                                return (UISelectable.Actions.Decide, CurrentSelected);
                            else// 未選択状態なら、決定と返信しない(上記のExecDecideProc中に選択解除される可能性ありなので)
                                return (UISelectable.Actions.None, CurrentSelected);
                        }
                        else
                        {
                            // SE
//                            ISoundManager.Instance.PlaySE_2D(
//                                ISoundManager.Instance.GetUISE(_uiseDisable)
//                            );

                            await CurrentSelected.NotifyEvent(UISelectable.Actions.CantDecide);

                            return (UISelectable.Actions.None, null);
                        }
                    }
                    if (inputUI.Option1)
                    {
                        if (CurrentSelected.IsInteractable())
                        {
                            CurrentSelected.NowAction = UISelectable.Actions.Menu;
                        }
                        else
                        {
                            // SE
//                            ISoundManager.Instance.PlaySE_2D(
//                                ISoundManager.Instance.GetUISE(_uiseDisable)
//                            );
                            return (UISelectable.Actions.None, null);
                        }
                    }

                    // 
                    if (CurrentSelected.NowAction == UISelectable.Actions.Decide)
                    {
                        CurrentSelected.ClearAction();

                        // 決定時処理
                        if (CurrentSelected.IsInteractable())
                        {
                            await CurrentSelected.ExecDecideProc(true);

                            if (CurrentSelected != null)
                                return (UISelectable.Actions.Decide, CurrentSelected);
                            else// 未選択状態なら、決定と返信しない(上記のExecDecideProc中に選択解除される可能性ありなので)
                                return (UISelectable.Actions.None, CurrentSelected);
                        }
                    }
                    else if (CurrentSelected.NowAction == UISelectable.Actions.CantDecide)
                    {
                        CurrentSelected.ClearAction();

                        if (CurrentSelected.IsInteractable() == false)
                        {
                            // SE
//                            ISoundManager.Instance.PlaySE_2D(
//                                ISoundManager.Instance.GetUISE(_uiseDisable)
//                            );

                            await CurrentSelected.NotifyEvent(UISelectable.Actions.CantDecide);
                        }
                        return (UISelectable.Actions.None, null);
                    }
                    else if (CurrentSelected.NowAction == UISelectable.Actions.Menu)
                    {
                        CurrentSelected.ClearAction();

                        // メニュー時 通知
                        if (CurrentSelected.IsInteractable())
                        {
                            await CurrentSelected.NotifyEvent(UISelectable.Actions.Menu);

                            return (UISelectable.Actions.Menu, CurrentSelected);
                        }
                        else
                        {
                            // SE
//                            ISoundManager.Instance.PlaySE_2D(
//                                ISoundManager.Instance.GetUISE(_uiseDisable)
//                            );

                            return (UISelectable.Actions.None, null);
                        }
                    }
                }
            }
            else
            {
                if (inputUI.IsPressAxis)
                {
                    // 未選択状態になっている場合は、検索
                    if (CurrentSelected == null)
                    {
                        CurrentSelected = GetTopSelectable();
                    }
                }
            }
        }
        catch
        {
            return (UISelectable.Actions.None, null);
        }

        return (UISelectable.Actions.None, null);
    }

    [System.Serializable]
    public class CursorObjectData
    {
        public UISelectableGroup Owner { get; set; }

        [SerializeField] RectTransform _cursorTrans;
        [SerializeField] float _duration = 0.1f;

        UISelectable _target;

        Tween _tween;

        public void Enable(bool enable)
        {
            if (_cursorTrans == null) return;

            // 表示時は座標を合わせる
            if (enable && _cursorTrans.gameObject.activeSelf == false && _target != null)
            {
                _tween.Kill();

                var rectTrans = _target.transform as RectTransform;

                _cursorTrans.position = rectTrans.position;
                _cursorTrans.sizeDelta = rectTrans.sizeDelta;
            }

            _cursorTrans.gameObject.SetActive(enable);
        }

        public async void SetTarget(UISelectable target)
        {
            if (_cursorTrans == null) return;

            _target = target;

            if(_target == null)
            {
                return;
            }

            var rectTrans = _target.transform as RectTransform;

            // 現在の補間を中止
            _tween.Kill();

            if (_duration > 0)
            {
                Sequence seq = DOTween.Sequence().SetUpdate(true).SetLink(_cursorTrans.gameObject);
                _ = seq.Join(
                    _cursorTrans.DOMove(rectTrans.position, _duration)
                );
                _ = seq.Join(
                    _cursorTrans.DOSizeDelta(rectTrans.sizeDelta, _duration)
                );

                _tween = seq;
                await seq;
            }
            else
            {
                _cursorTrans.position = rectTrans.position;
                _cursorTrans.sizeDelta = rectTrans.sizeDelta;
            }
        }

        public void FitTargetPos()
        {
            if (_cursorTrans == null) return;
            if (_target == null) return;

                var rectTrans = _target.transform as RectTransform;

            _cursorTrans.position = rectTrans.position;
            _cursorTrans.sizeDelta = rectTrans.sizeDelta;
        }

        public void Update()
        {
            if (_cursorTrans == null) return;

            if(_target == null)
            {
                Enable(false);
                return;
            }
            else
            {
                // エフェクトオブジェ使用中
                if (_target.UseFocusEffectObject)
                {
                    Enable(false);
                }
                else
                {
                    Enable(true);
                }

                // 補間中でなければ、引っ付ける
                if (_tween.IsActive() == false)
                {
                    var rectTrans = _target.transform as RectTransform;

                    _cursorTrans.position = rectTrans.position;
                    _cursorTrans.sizeDelta = rectTrans.sizeDelta;
                }
            }

        }
    }



    public async UniTask<UISelectable> SimpleExecAsync()
    {
        var cancelToken = this.GetCancellationTokenOnDestroy();

        while (true)
        {
            if (cancelToken.IsCancellationRequested) break;

            var retCursor = await UpdateCursor();
            if (retCursor.action == UISelectable.Actions.Decide)
            {
                return retCursor.select;
            }

            await UniTask.DelayFrame(1);
        }
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("NavgationをExplicitし、探索する")]
    public void Editor_AllExplicit()
    {
        var sels = GetComponentsInChildren<UISelectable>();
        foreach(var sel in sels)
        {
            var nav = sel.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnLeft = sel.FindSelectable(sel.transform.rotation * Vector3.left);
            nav.selectOnRight = sel.FindSelectable(sel.transform.rotation * Vector3.right);
            nav.selectOnUp = sel.FindSelectable(sel.transform.rotation * Vector3.up);
            nav.selectOnDown = sel.FindSelectable(sel.transform.rotation * Vector3.down);

            sel.navigation = nav;

        }
    }
    [ContextMenu("NavgationをAutomaticにする")]
    public void Editor_Automatic()
    {
        var sels = GetComponentsInChildren<UISelectable>();
        foreach (var sel in sels)
        {
            var nav = sel.navigation;
            nav.mode = Navigation.Mode.Automatic;


            sel.navigation = nav;

        }
    }

#endif

}
