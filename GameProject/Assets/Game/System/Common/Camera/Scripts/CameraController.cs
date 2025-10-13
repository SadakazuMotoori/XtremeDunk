//==================================================================
/// <summary>
/// カメラ制御クラス
/// </summary>
//==================================================================
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Overlays;

namespace SGApp.Game.Sys
{
    public partial class CameraController : MonoBehaviour
    {
/*
        [SerializeField]
        private CameraData[] m_CameraTable;

        /// <summary>
        /// 対象のカメラ取得
        /// </summary>
        /// <param name="camid">カメラID</param>
        /// <returns></returns>
        public Camera GetCamera(eCameraID camid)
        {
            return m_CameraTable[(int)camid];
        }
        public Camera bottomCamera { get { return m_cameraTable[(int)CameraID.Bottom]; } }
        public Camera backCamera { get { return m_cameraTable[(int)CameraID.Back]; } }
        public Camera backCutSceneCamera { get { return m_cameraTable[(int)CameraID.BackCutScene]; } }
        public Camera mainCamera { get { return m_cameraTable[(int)CameraID.Main]; } }
        public Camera topCamera { get { return m_cameraTable[(int)CameraID.Top]; } }
        public Camera flatFxCamera { get { return m_cameraTable[(int)CameraID.FlatFx]; } }
        public Camera UIModelCamera { get { return m_cameraTable[(int)CameraID.UIModel]; } }
        public Camera UICamera { get { return m_cameraTable[(int)CameraID.UI]; } }

        /// <summary>
        /// 各カメラのデフォルトCullingMask（バトル用）
        /// </summary>
        [SerializeField]
        private LayerMask[] m_battleLayerMasks;

        /// <summary>
        /// 各カメラのデフォルトCullingMask（ホーム用）
        /// </summary>
        [SerializeField]
        private LayerMask[] m_homeLayerMasks;
        /// <summary>
        /// 各カメラのデフォルトCullingMask（ロビー用）
        /// </summary>
        [SerializeField]
        private LayerMask[] m_lobbyLayerMasks;
        /// <summary>
        /// ランウェイ画面用CullingMask
        /// </summary>
        [SerializeField]
        private LayerMask[] m_runwwayLayerMasks;

        /// <summary>
        /// 各カメラに指定したLayerMaskデータを設定する
        /// </summary>
        /// <param name="masks"></param>
        private void SetupCullingMasks(LayerMask[] masks)
        {
            for (int i = 0; i < m_cameraTable.Length; ++i)
            {
                var cam = m_cameraTable[i];
                cam.cullingMask = masks[i];
            }
        }



        [SerializeField] private ResolutionRenderer m_resolutionRenderer = new ResolutionRenderer();
        [SerializeField] private ResolutionRenderer m_resolutionRenderer2 = new ResolutionRenderer();   //分割画面用
        [SerializeField] private ResolutionRenderer m_resolutionRendererUI = new ResolutionRenderer();
        /// <summary>
        /// ポストエフェクト（ブラー）
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxBlur m_pfxBlur;
        /// <summary>
        /// ポストエフェクト（ブルーム）
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxBloom m_pfxBloom;
        /// <summary>
        /// ポストエフェクト（色反転）
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxInverseColor m_pfxInverseColor;
        /// <summary>
        /// ポストエフェクト（放射ブラー）
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxRadialBlur m_pfxRadialBlur;
        /// <summary>
        /// ポストエフェクト（被写界深度）
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.ScreenOverlayBlur m_pfxDepthOfField;
        [SerializeField] private XeApp.Game.PostEffect.ScreenOverlayBlur m_pfxDepthOfField_Back;

        [SerializeField] private FxScreenOverlay m_fxOverlay;
        [SerializeField] private FxScreenOverlay m_fxOverlayDamage;

        // カメラ初期位置
        private Vector3 mInitPosition = new Vector3(-3.75f, 1.5f, -8.7f);


        [SerializeField] private float initFov;

        // ビルボードの為の逆クォータニオン
        public Quaternion inverseCameraRotation { get; private set; }
#if UNITY_EDITOR
        [SerializeField] bool IsGizmoDebugDisplay = true;
#endif
        /// <summary>
        /// カメラコントローラのインスタンス
        /// </summary>
        public static CameraController mInstance = null;
        public static CameraController Instance
        {
            get { return mInstance; }
        }



        public ResolutionRenderer resolutionRenderer
        {
            get { return m_resolutionRenderer; }
        }

        public ResolutionRenderer resolutionRendererUI
        {
            get { return m_resolutionRendererUI; }
        }


        /// <summary>
        /// ブルームのインスタンス取得
        /// </summary>
        public PostEffect.PfxBloom bloom
        {
            get
            {
                return m_pfxBloom;
            }
        }
        public PostEffect.PfxInverseColor inverseColor
        {
            get
            {
                return m_pfxInverseColor;
            }
        }
        /// <summary>
        /// 放射ブラーのインスタンス取得
        /// </summary>
        public PostEffect.PfxRadialBlur radialBlur { get { return m_pfxRadialBlur; } }

        /// <summary>
        /// カメラの利用モード
        /// </summary>
        public enum Mode
        {
            Battle,
            Menu,
            Lobby,
            Runway,
        }
        public Mode mode { get; private set; }


        /// <summary>
        /// 位置更新の無効有効
        /// </summary>
        public bool isTransformAvailable { get; set; }

        /// <summary>
        /// カメラのレンダリング幅
        /// </summary>
        public float cameraRenderWidth { get; private set; }
        /// <summary>
        /// カメラのレンダリング高さ
        /// </summary>
        public float cameraRenderHeight { get; private set; }

        //ビューポートのワールド座標保持用
        public Vector3 viewportLeftTop { get; private set; }
        public Vector3 viewportRightDown { get; private set; }
        public Vector3 viewportRightTop { get; private set; }
        public Vector3 viewportLeftDown { get; private set; }

        // キャラを表示するセーフエリアのワールド座標保持
        public Vector3 safeLeftTop { get; private set; }
        public Vector3 safeLeftDown { get; private set; }
        public Vector3 safeRightTop { get; private set; }
        public Vector3 safeRightDown { get; private set; }

        private Vector3 mMainCameraForwardPosition;

        // カメラ移動用
        private Vector3 mTargetPosition = Vector3.zero;
        private bool mIsDelay = false;
        private float mDelayCoef = 1.0f;
        private bool mIsBattleDofEnable;

        private Vector3 mTargetUIModelCameraPosition = Vector3.zero;
        private float mUIModelDelayCoef = 0.5f;

        private Quaternion mBaseUIModelCameraRotate = Quaternion.identity;
        private Quaternion mTargetUIModelCameraRotate = Quaternion.identity;

        private int mUIModelRotateFrame = 0;
        private int mUIModelRotateCount = 0;

        //消失点移動用
        private Vector2 mTargetUIModelCameraVanishingPoint = Vector2.zero;
        private float mUIModelCameraVanshingPointDelayCoef = 0.2f;

        // スクリーンの中心位置（スクリーン座標とワールド座標
        public Vector3 screenCenterPosition { get; private set; }
        public Vector3 screenCenterWorldPosition { get; private set; }

        // 更新関数
        System.Action mUpdater;

        /// <summary>
        /// メインカメラのTransformや画角と同期させるためのフラグ
        /// </summary>
        private List<bool> mLinkToMainCameraList;


        [SerializeField] private bool mIsCameraUpdateFlag = true;
        [SerializeField] private bool mIsUIModelCameraUpdateFlag = false;

        [SerializeField] private Canvas mUICanvas;  //UI描画用のキャンバス

        /// <summary>
        /// 各カメラの消失点座標
        /// </summary>
        List<Vector2> mVanishingPointList;

        GameScene.RawImage3DRendererType[] mUIRendererTypes = new GameScene.RawImage3DRendererType[] {
            GameScene.RawImage3DRendererType.ui,
            GameScene.RawImage3DRendererType.front,
        };

        void Awake()
        {
            mInstance = this;

            mLinkToMainCameraList = new List<bool>();
            mVanishingPointList = new List<Vector2>();

            for (int i = 0; i < m_cameraTable.Length; ++i)
            {
                mLinkToMainCameraList.Add(false);

                Camera cam = m_cameraTable[i];
                if (null == cam)
                {
                    continue;
                }
                cam.gameObject.SetActive(false);

                mVanishingPointList.Add(Vector2.zero);
            }
            this.bottomCamera.gameObject.SetActive(true);
            this.mainCamera.gameObject.SetActive(true);
            this.UICamera.gameObject.SetActive(true);

            m_fxOverlay.Initialize();
            m_fxOverlayDamage.Initialize();
            mIsBattleDofEnable = true;

            InitializeSplit();
        }
        void Start()
        {
            //            mUpdater = Update_PositionIdle;
            mUpdater = Update_Position;
        }

        private void LateUpdate()
        {
            //メインカメラとのリンク処理
            var mainCamTform = this.mainCamera.transform;
            var mainCamVanish = GetVanishingPoint(CameraID.Main);
            for (int i = 0; i < mLinkToMainCameraList.Count; ++i)
            {
                if (!mLinkToMainCameraList[i])
                {
                    continue;
                }

                var target = m_cameraTable[i];
                if (null == target)
                {
                    continue;
                }

                target.transform.position = mainCamTform.position;
                target.transform.localRotation = mainCamTform.localRotation;
                target.fieldOfView = this.mainCamera.fieldOfView;
                target.nearClipPlane = this.mainCamera.nearClipPlane;
                target.farClipPlane = this.mainCamera.farClipPlane;
                SetVanishingPoint((CameraID)i, mainCamVanish);
            }
        }

        void DisableAllCamera()
        {
            for (int i = 0; i < m_cameraTable.Length; ++i)
            {
                Camera cam = m_cameraTable[i];
                if (null == cam)
                {
                    continue;
                }
                cam.gameObject.SetActive(false);
                cam.targetTexture = null;
            }
        }

        /// <summary>
        /// 指定カメラのアクティブ設定
        /// </summary>
        /// <param name="camId">対象のカメラID</param>
        /// <param name="activate">trueの時、有効</param>
        public void ActiveCamera(CameraID camId, bool activate)
        {
            var cam = GetCamera(camId);
            cam.gameObject.SetActive(activate);
        }

        /// <summary>
        /// システム解像度切り替えリクエスト時の処理
        /// </summary>
        public static void ChangeSystemResolution()
        {
            if (null == Instance)
            {
                return;
            }
            Instance.Inner_ChangeSystemResolution();
        }
        private void Inner_ChangeSystemResolution()
        {
            this.resolutionRenderer.ApplyScale();
        }

        /// <summary>
        /// 3D解像度切り替えリクエスト時の処理
        /// </summary>
        public static void Change3dResolution()
        {
            if (null == Instance)
            {
                return;
            }
            Instance.Inner_Change3dResolution();
        }

        public void Inner_Change3dResolution()
        {
            this.resolutionRenderer.SetActive(true);

            switch (this.mode)
            {
                case Mode.Battle:
                    SetupRenderer_Battle(true);
                    break;
                case Mode.Lobby:
                    SetupRenderer_Lobby(true);
                    break;
                case Mode.Runway:
                    SetupRenderer_RunWay(true);
                    break;
                case Mode.Menu:
                default:
                    SetupRenderer_Menu(true);
                    break;
            }
        }

        /// <summary>
        /// バトル用レンダラ準備
        /// </summary>
        private void SetupRenderer_Battle(bool force)
        {
            m_resolutionRenderer.SetActive(true);
            m_resolutionRenderer.forceSetup = force;
            m_resolutionRenderer.Setup(GameConst.Layer.RENDERER,
                GameScene.RawImage3DRendererType.normal,
                (int)Mode.Battle,
                GetCamera(CameraID.Main),
                GetCamera(CameraID.Bottom),
                GetCamera(CameraID.Back),
                GetCamera(CameraID.BackCutScene),
                GetCamera(CameraID.FlatFx)
                );

            m_resolutionRendererUI.SetActive(true);
            m_resolutionRendererUI.SetupUnSetActive(GameConst.Layer.RENDERER,
                mUIRendererTypes,
                (int)Mode.Battle,
                GetCamera(CameraID.UIModel)
                );
        }
        /// <summary>
        /// メニュー用レンダラ準備
        /// </summary>
        private void SetupRenderer_Menu(bool force)
        {
            m_resolutionRenderer.SetActive(true);
            m_resolutionRenderer.forceSetup = force;
            m_resolutionRenderer.Setup(GameConst.Layer.RENDERER,
                GameScene.RawImage3DRendererType.normal,
                (int)Mode.Menu,
                GetCamera(CameraID.Top),
                GetCamera(CameraID.Main),
                GetCamera(CameraID.Bottom)
                );

            m_resolutionRendererUI.SetActive(true);
            m_resolutionRendererUI.forceSetup = force;
            m_resolutionRendererUI.SetupUnSetActive(GameConst.Layer.RENDERER,
                mUIRendererTypes,
                (int)Mode.Menu,
                GetCamera(CameraID.UIModel)
                );
        }


        /// <summary>
        /// ロビー用Renderer設定処理
        /// </summary>
        /// <param name="force"></param>
        private void SetupRenderer_Lobby(bool force)
        {
            m_resolutionRenderer.SetActive(true);
            m_resolutionRenderer.forceSetup = force;
            m_resolutionRenderer.Setup(GameConst.Layer.RENDERER,
                GameScene.RawImage3DRendererType.normal,
                (int)Mode.Menu,
                GetCamera(CameraID.Top),
                GetCamera(CameraID.Main),
                GetCamera(CameraID.Bottom)
                );

            m_resolutionRendererUI.SetActive(true);
            m_resolutionRendererUI.forceSetup = force;
            m_resolutionRendererUI.SetupUnSetActive(GameConst.Layer.RENDERER,
                mUIRendererTypes,
                (int)Mode.Menu,
                GetCamera(CameraID.UIModel)
                );
        }

        /// <summary>
        /// ロビー用リセット処理
        /// </summary>
        public void ResetForLobby()
        {
            this.mode = Mode.Lobby;

            DisableSplitRenderer();
            DisableAllCamera();
            GetCamera(CameraID.UI).gameObject.SetActive(true);

            var camMain = GetCamera(CameraID.Main);
            camMain.clearFlags = CameraClearFlags.Color;
            camMain.backgroundColor = Color.black;
            LinkToMainCamera(CameraID.Back, false);

            SetupCullingMasks(m_lobbyLayerMasks);

            SetupRenderer_Lobby(false);

            m_pfxBloom.SetEnable(false);
            m_pfxBlur.SetEnable(false);
            m_pfxBlur.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);
            m_pfxInverseColor.SetEnable(false);
            m_pfxInverseColor.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);
            m_pfxDepthOfField.SetEnable(false);
            m_pfxDepthOfField_Back.SetEnable(false);
            m_fxOverlay.SetEnable(false);
            m_fxOverlayDamage.SetEnable(false);
            m_pfxRadialBlur.SetEnable(false);
            m_pfxRadialBlur.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);
        }

        /// <summary>
        /// メインメニュー用リセット処理
        /// </summary>
        public void ResetForMainMenu()
        {
            this.mode = Mode.Menu;

            DisableSplitRenderer();
            DisableAllCamera();
            GetCamera(CameraID.UI).gameObject.SetActive(true);

            var camMain = GetCamera(CameraID.Main);
            camMain.clearFlags = CameraClearFlags.Color;
            camMain.backgroundColor = Color.black;
            //ActiveCamera( CameraID.Main, true );
            LinkToMainCamera(CameraID.Back, false);

            SetupCullingMasks(m_homeLayerMasks);

            SetupRenderer_Menu(false);

            m_pfxBloom.SetEnable(false);
            m_pfxBlur.SetEnable(false);
            m_pfxBlur.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);

            m_pfxInverseColor.SetEnable(false);
            m_pfxInverseColor.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);

            m_pfxRadialBlur.SetEnable(false);
            m_pfxRadialBlur.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);

            m_pfxDepthOfField.SetEnable(false);
            m_pfxDepthOfField_Back.SetEnable(false);
            m_fxOverlay.SetEnable(false);
            m_fxOverlayDamage.SetEnable(false);
        }
        /// <summary>
        /// ポストエフェクトにゲーム設定を適用
        /// </summary>
        public void ApplyPostEffect()
        {
            m_pfxBlur.Apply();
            m_pfxBloom.Apply();
            m_pfxInverseColor.Apply();
        }

        /// <summary>
        /// バトル用全リセット処理
        /// </summary>
        public void ResetAllForBattle()
        {
            this.mode = Mode.Battle;

            ResetAllVanishingPointValue();
            DisableSplitRenderer();
            DisableAllCamera();
            GetCamera(CameraID.Bottom).gameObject.SetActive(true);
            GetCamera(CameraID.Back).gameObject.SetActive(true);
            GetCamera(CameraID.Main).gameObject.SetActive(true);
            GetCamera(CameraID.FlatFx).gameObject.SetActive(true);
            GetCamera(CameraID.UIModel).gameObject.SetActive(false);
            GetCamera(CameraID.UI).gameObject.SetActive(true);

            LinkToMainCamera(CameraID.Back, true);

            SetupCullingMasks(m_battleLayerMasks);

            //3D空間解像度切替描画
            SetupRenderer_Battle(false);
            //ポストエフェクト
            m_pfxBloom.SetEnable(false);
            m_pfxDepthOfField.SetEnable(false);
            m_pfxDepthOfField_Back.SetEnable(mIsBattleDofEnable);

            m_pfxBlur.SetEnable(false);
            m_pfxBlur.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);

            m_pfxInverseColor.SetEnable(false);
            m_pfxInverseColor.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);

            m_pfxRadialBlur.SetEnable(false);
            m_pfxRadialBlur.SetRenderTexture(m_resolutionRenderer.renderTexture_Color);

            m_fxOverlay.SetEnable(false);
            m_fxOverlay.SetColor(Color.clear);

            m_fxOverlayDamage.SetEnable(false);
            m_fxOverlayDamage.SetColor(Color.white);

            //main
            this.mainCamera.ResetProjectionMatrix();
            SetFov(this.initFov);
            this.mainCamera.clearFlags = CameraClearFlags.Nothing;
            //            this.mainCamera.clearFlags = CameraClearFlags.Depth;// CameraClearFlags.Nothing;

            // 初期位置
            SetTargetPosition(mInitPosition);
            SetDelayCoefficient(1);

            SetPosition(new Vector3(0.0f, 0.7f, -10.0f));
            SetRotation(Quaternion.identity);

            SetNear(0.3f);
            SetFar(1000.0f);
            SetFov(40.0f, true);

            //background
            Camera camBack = GetCamera(CameraID.Bottom);
            camBack.ResetProjectionMatrix();
            camBack.fieldOfView = this.initFov;
            camBack.nearClipPlane = 0.3f;
            camBack.farClipPlane = 2000.0f;

            SetPositionUpdateFlag(true);
        }
        /// <summary>
        /// 被写界深度の有効化
        /// フラグを設定するのみ。
        /// </summary>
        /// <param name="enable"></param>
        public void SetPfxDepthOfFieldEnable(bool enable)
        {
            mIsBattleDofEnable = enable;
        }

        /// <summary>
        /// 指定したカメラをメインカメラの情報とリンクさせる
        /// 
        /// Transform, fieldOfView, nearClipPlane, farClipPlaneがリンクされます
        /// </summary>
        /// <param name="targetID">対象のカメラ</param>
        /// <param name="linked">trueの時、リンクされる</param>
        public void LinkToMainCamera(CameraID targetID, bool linked)
        {
            if (CameraID.Main == targetID)
            {
                //メインとリンクさせる為メインの指定は禁止
                return;
            }
            mLinkToMainCameraList[(int)targetID] = linked;
        }

        /// <summary>
        /// カメラ移動のディレイ効果係数の設定
        /// </summary>
        /// <param name="sw"></param>
        public void SetDelayCoefficient(float coef)
        {
            mDelayCoef = coef;
        }
        public void SetDelayCoefficient_UIModelCamera(float coef)
        {
            mUIModelDelayCoef = coef;
        }
        /// <summary>
        /// カメラの移動目標位置を設定
        /// SetUpdate( true )時に有効
        /// </summary>
        /// <param name="_target"></param>
        public void SetTargetPosition(Vector3 _target)
        {
            mTargetPosition = _target;
        }
        /// <summary>
        /// カメラの移動目標位置を設定
        /// UIModelカメラ用
        /// SetUpdate( true )時に有効
        /// </summary>
        /// <param name="_target"></param>
        public void SetTargetUIModelCameraPosition(Vector3 _target)
        {
            mTargetUIModelCameraPosition = _target;
        }
        public void SetTargetUIModelCameraRotate(Quaternion _target)
        {
            mTargetUIModelCameraRotate = _target;
        }
        public void SetTargetUIModelCameraVanishingPoint(Vector2 _target)
        {
            mTargetUIModelCameraVanishingPoint = _target;
        }
        public Vector3 GetTargetUIModelCameraPosition()
        {
            return mTargetUIModelCameraPosition;
        }
        /// <summary>
        /// カメラ位置設定
        /// </summary>
        /// <param name="_pos"></param>
        public void SetPosition(Vector3 _pos)
        {
#if GAME_DEBUG
            //            DebugLog.Info(_pos);
#endif
            this.mainCamera.transform.position = _pos;
        }
        public void SetPosition(CameraID cameraID, Vector3 _pos)
        {
#if GAME_DEBUG
            //            DebugLog.Info(_pos);
#endif
            this.GetCamera(cameraID).transform.position = _pos;
        }
        /// <summary>
        /// 回転の設定
        /// </summary>
        /// <param name="_quaternion"></param>
        public void SetRotation(Quaternion _quaternion)
        {
            this.mainCamera.transform.localRotation = _quaternion;
        }
        public void SetRotation(CameraID cameraID, Quaternion _quaternion)
        {
            this.GetCamera(cameraID).transform.localRotation = _quaternion;
        }

        /// <summary>
        /// Nearクリップ面設定
        /// </summary>
        /// <param name="_near"></param>
        public void SetNear(float _near)
        {
            this.mainCamera.nearClipPlane = _near;
        }
        /// <summary>
        /// Farクリップ面設定
        /// </summary>
        /// <param name="_far"></param>
        public void SetFar(float _far)
        {
            this.mainCamera.farClipPlane = _far;
        }

        /// <summary>
        /// SetFovにて設定した垂直画角値
        /// </summary>
        public float settingVerticalFov { get; private set; }

        /// <summary>
        /// 垂直画角を設定
        /// </summary>
        /// <param name="sourceVFov">基準画面での垂直画角</param>
        /// <param name="fitHorizontal">trueの時、アスペクト比に関わらず水平画角を一致させる</param>
        public void SetFov(float sourceVFov, bool fitHorizontal = false)
        {
            this.settingVerticalFov = sourceVFov;

            float fov = sourceVFov;
            if (fitHorizontal)
            {
                float sourceHFov = GameRenderUtil.GetHorizontalFov(sourceVFov, XeSys.Gfx.RenderManager.baseSize.x, XeSys.Gfx.RenderManager.baseSize.y);
                fov = GameRenderUtil.GetVerticalFov(sourceHFov, XeSys.Gfx.RenderManager.screenSize.x, XeSys.Gfx.RenderManager.screenSize.y);
            }
            this.mainCamera.fieldOfView = fov;
        }
        /// <summary>
        /// 垂直画角の取得
        /// </summary>
        /// <returns></returns>
        public float GetFov()
        {
            return GetCamera(CameraID.Main).fieldOfView;
        }
        /// <summary>
        /// 両画角の取得
        /// </summary>
        /// <param name="vfov">垂直画角格納先</param>
        /// <param name="hfov">水平画角格納先</param>
        public void GetFov(out float vfov, out float hfov)
        {
            vfov = this.mainCamera.fieldOfView;
            hfov = GameRenderUtil.GetHorizontalFov(vfov, XeSys.Gfx.RenderManager.screenSize.x, XeSys.Gfx.RenderManager.screenSize.y);
        }

        /// <summary>
        /// カメラ位置取得
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            return GetCamera(CameraID.Main).transform.position;
        }
        public Vector3 GetPosition(CameraID cameraID)
        {
            return GetCamera(cameraID).transform.position;
        }

        /// <summary>
        /// カメラ位置設定（X座標）
        /// </summary>
        /// <param name="_x"></param>
        public void SetPositionX(float _x)
        {
            Vector3 pos = GetPosition();
            pos.x = _x;
            SetPosition(pos);
        }
        public void SetPositionX(CameraID cameraID, float _x)
        {
            Vector3 pos = GetPosition(cameraID);
            pos.x = _x;
            SetPosition(cameraID, pos);
        }
        /// <summary>
        /// カメラ位置設定（Y座標）
        /// </summary>
        /// <param name="_y"></param>
        public void SetPositionY(float _y)
        {
            Vector3 pos = GetPosition();
            pos.y = _y;
            SetPosition(pos);
        }
        public void SetPositionY(CameraID cameraID, float _y)
        {
            Vector3 pos = GetPosition(cameraID);
            pos.y = _y;
            SetPosition(cameraID, pos);
        }
        /// <summary>
        /// カメラ位置設定（Z座標）
        /// </summary>
        /// <param name="_z"></param>
        public void SetPositionZ(float _z)
        {
            Vector3 pos = GetPosition();
            pos.z = _z;
            SetPosition(pos);
        }
        public void SetPositionZ(CameraID cameraID, float _z)
        {
            Vector3 pos = GetPosition(cameraID);
            pos.z = _z;
            SetPosition(cameraID, pos);
        }
        /// <summary>
        /// カメラ位置設定（XYZ）
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public void SetPosition(float _x, float _y, float _z)
        {
            Vector3 pos = GetPosition();
            pos.x = _x;
            pos.y = _y;
            pos.z = _z;
            SetPosition(pos);
        }

        public void SetPosition(CameraID cameraID, float _x, float _y, float _z)
        {
            Vector3 pos = GetPosition(cameraID);
            pos.x = _x;
            pos.y = _y;
            pos.z = _z;
            SetPosition(cameraID, pos);
        }

        /// <summary>
        /// カメラ位置の加算
        /// メインカメラ
        /// </summary>
        /// <param name="_pos"></param>
        public void AddPosition_MainCamera(Vector3 _pos)
        {
            this.mainCamera.transform.position += _pos;
        }
        /// <summary>
        /// カメラ位置の加算
        /// 背景カメラ
        /// </summary>
        /// <param name="_pos"></param>
        public void AddPosition_BackgroundCamera(Vector3 _pos)
        {
            this.bottomCamera.transform.position += _pos;
        }
        /// <summary>
        /// カメラ位置の加算
        /// カメラID指定
        /// </summary>
        /// <param name="cameraID"></param>
        /// <param name="_pos"></param>
        public void AddPosition(CameraID cameraID, Vector3 _pos)
        {
            this.GetCamera(cameraID).transform.position += _pos;
        }

        /// <summary>
        /// 注視点を設定
        /// </summary>
        /// <param name="_target"></param>
        public void SetLootAt(Vector3 _target)
        {
            this.mainCamera.transform.LookAt(_target);
        }
        /// <summary>
        /// 注視点を設定
        /// </summary>
        /// <param name="cameraID"></param>
        /// <param name="_target"></param>
        public void SetLookAt(CameraID cameraID, Vector3 _target)
        {
            this.GetCamera(cameraID).transform.LookAt(_target);
        }

        public void StartCameraRotate_UIModelCamera(int frame, Quaternion _target)
        {
            mBaseUIModelCameraRotate = this.GetCamera(CameraID.UIModel).transform.rotation;
            mTargetUIModelCameraRotate = _target;

            mUIModelRotateFrame = frame;
            mUIModelRotateCount = 0;
        }

        /// <summary>
        /// 位置更新をしない更新関数
        /// SetUpdate( false )で設定
        /// </summary>
        private void UpdatePositionIdle()
        {
        }
        /// <summary>
        /// 位置更新を行う更新関数
        /// SetUpdate( true )で設定
        /// </summary>
        private void UpdatePositionMove()
        {
            Vector3 diff = mTargetPosition - this.mainCamera.transform.position;
            // 十分に近づいたらディレイ効果OFF
            if (0.001f >= diff.sqrMagnitude)
            {
                mDelayCoef = 1.0f;
            }

            diff *= mDelayCoef;
            AddPosition(CameraID.Main, diff);
            AddPosition(CameraID.Bottom, diff);
            //            AddPosition_MainCamera( diff );
            //            AddPosition_BackgroundCamera( diff );
        }

        private void UpdatePositionMove_UIModelCamera()
        {
            Vector3 diff = mTargetUIModelCameraPosition - GetPosition(CameraID.UIModel);
            float delay = mUIModelDelayCoef;
            // 十分に近づいたらディレイ効果OFF
            if (0.0001f >= diff.sqrMagnitude)
            {
                delay = 1.0f;
            }
            diff *= delay;
            AddPosition(CameraID.UIModel, diff);
        }

        private void UpdateRotateMove_UIModelCamera()
        {
            float rate = 1.0f;
            mUIModelRotateCount++;
            if (mUIModelRotateFrame != 0 && mUIModelRotateCount < mUIModelRotateFrame)
            {
                rate = (float)mUIModelRotateCount / (float)mUIModelRotateFrame;
                rate = Math.Tween.Evaluate(Math.Tween.EasingFunc.InOutCubic, 0.0f, 1.0f, rate);
            }

            SetRotation(CameraID.UIModel, Quaternion.Lerp(mBaseUIModelCameraRotate, mTargetUIModelCameraRotate, rate));
        }

        private void UpdateVanishingPoint_UIModelCamera()
        {
            Vector2 current = GetVanishingPoint(CameraID.UIModel);
            Vector2 diff = mTargetUIModelCameraVanishingPoint - current;
            // 十分に近づいたらディレイ効果OFF
            if (0.00001f < diff.sqrMagnitude)
            {
                diff *= mUIModelCameraVanshingPointDelayCoef;
            }

            SetVanishingPoint(CameraID.UIModel, current + diff);
        }

        /// <summary>
        /// カメラがターゲットに向かって移動中かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsMove_UIModelCamera()
        {
            bool ret = true;
            Vector3 diff = mTargetUIModelCameraPosition - GetPosition(CameraID.UIModel);
            if (0.001f >= diff.sqrMagnitude)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// カメラの位置更新状態の設定
        /// </summary>
        /// <param name="update">trueでmTargetPositionへ向かう位置加算処理を行う</param>
        public void SetPositionUpdateFlag(bool update)
        {
            if (mIsCameraUpdateFlag != update)
            {
                mIsCameraUpdateFlag = update;
            }
        }
        public void SetUIModelPositionUpdateFlag(bool update)
        {
            if (mIsUIModelCameraUpdateFlag != update)
            {
                mIsUIModelCameraUpdateFlag = update;
            }
        }
        /// <summary>
        /// カメラ更新タイプフラグの取得
        /// </summary>
        /// <returns></returns>
        public bool GetCameraUpdateFlag()
        {
            return mIsCameraUpdateFlag;
        }
        /// <summary>
        /// カメラ位置の更新関数
        /// </summary>
        private void Update_Position()
        {
            if (mIsCameraUpdateFlag)
            {
                UpdatePositionMove();
            }
            else
            {
                UpdatePositionIdle();
            }

            if (mIsUIModelCameraUpdateFlag)
            {
                UpdatePositionMove_UIModelCamera();
                UpdateVanishingPoint_UIModelCamera();
            }
            if (mUIModelRotateCount < mUIModelRotateFrame)
            {
                UpdateRotateMove_UIModelCamera();
            }
        }


        /// <summary>
        /// 更新（毎フレーム）
        /// </summary>
        void Update()
        {
            mUpdater();

            //-------------------------------
            // スクリーンの中心座標(ワールドも)
            //-------------------------------
            int center_w = Screen.width / 2;
            int center_h = Screen.height / 2;
            this.screenCenterPosition = new Vector3(center_w, center_h, -this.mainCamera.transform.localPosition.z);
            /////            this.screenCenterPosition *= CameraController.Instance.resolutionRenderer.scale;
            this.screenCenterPosition *= CameraController.Instance.resolutionRenderer.settingScale;
            this.screenCenterWorldPosition = this.mainCamera.ScreenToWorldPoint(this.screenCenterPosition);


            //-------------------------------
            // ビューポートの座標
            //-------------------------------
            mMainCameraForwardPosition = this.mainCamera.transform.forward * (-this.mainCamera.transform.localPosition.z);
            Update_ViewportPosition(mMainCameraForwardPosition.z);

            //-------------------------------
            // 錐台の高さと幅
            //-------------------------------
            // 錐台の高さと幅。現在未使用だがいずれ使うことがありそうなので算出しておく。
            this.cameraRenderWidth = Vector3.Distance(viewportLeftDown, viewportRightDown);
            this.cameraRenderHeight = Vector3.Distance(viewportLeftDown, viewportLeftTop);

            this.inverseCameraRotation = Quaternion.Inverse(this.mainCamera.transform.localRotation);

#if GAME_DEBUG
            //            DebugText.Draw( new Vector2( 50, 50 ), "CameraRenderWidth:" + this.cameraRenderWidth.ToString("0.0"), Color.cyan );
            //            DebugText.Draw( new Vector2( 50, 75 ), "CameraRenderHeight:" + this.cameraRenderHeight.ToString("0.0"), Color.cyan );
#endif
            UpdateFlatFxCamera();
        }


        /// <summary>
        /// FlatFxカメラの位置をメインカメラの位置に同期させます。
        /// 動的更新されるメインカメラ用オブジェクト用の正射影カメラであるため。
        /// </summary>
        private void UpdateFlatFxCamera()
        {
            Vector3 mainPosi = GetPosition(CameraID.Main);
            Vector3 flatFxPosi = mainPosi;
            flatFxPosi.z = 0;
            SetPosition(CameraID.FlatFx, flatFxPosi);
        }

        Vector3 viewportOverVector = Vector3.zero;
        /// <summary>
        /// ビューポート内に入っているかどうかのチェック
        /// </summary>
        /// <param name="_worldPos"></param>
        /// <returns></returns>
        public bool IsInViewport(Vector3 _worldPos)
        {
            bool isIn = true;
            viewportOverVector = Vector3.zero;
            // 画面左オーバー
            if (viewportLeftTop.x > _worldPos.x)
            {
                viewportOverVector.x = _worldPos.x - viewportLeftTop.x;
                isIn = false;
            }
            // 画面右オーバー
            if (viewportRightTop.x < _worldPos.x)
            {
                viewportOverVector.x = _worldPos.x - viewportRightTop.x;
                isIn = false;
            }
            if (viewportLeftTop.y < _worldPos.y)
            {
                viewportOverVector.y = _worldPos.y - viewportLeftTop.y;
                isIn = false;
            }
            if (viewportLeftDown.y > _worldPos.y)
            {
                viewportOverVector.y = _worldPos.y - viewportLeftDown.y;
                isIn = false;
            }
            return isIn;
        }
        /// <summary>
        /// 左右方向（x軸）のみ画面内チェック
        /// </summary>
        /// <param name="_worldPos"></param>
        /// <returns></returns>
        public bool IsInViewportX(Vector3 _worldPos)
        {
            bool isIn = true;
            viewportOverVector = Vector3.zero;
            // 画面左オーバー
            if (viewportLeftTop.x > _worldPos.x)
            {
                viewportOverVector.x = _worldPos.x - viewportLeftTop.x;
                isIn = false;
            }
            // 画面右オーバー
            if (viewportRightTop.x < _worldPos.x)
            {
                viewportOverVector.x = _worldPos.x - viewportRightTop.x;
                isIn = false;
            }
            return isIn;
        }
        public Vector3 GetViewportOverVector()
        {
            return viewportOverVector;
        }


        /// <summary>
        /// ビューポート座標をワールド座標へ変換
        /// </summary>
        /// <param name="z"></param>
        private void Update_ViewportPosition(float z)
        {
            viewportRightTop = this.mainCamera.ViewportToWorldPoint(new Vector3(1, 1, z));
            viewportRightDown = this.mainCamera.ViewportToWorldPoint(new Vector3(1, 0, z));
            viewportLeftTop = this.mainCamera.ViewportToWorldPoint(new Vector3(0, 1, z));
            viewportLeftDown = this.mainCamera.ViewportToWorldPoint(new Vector3(0, 0, z));

            // ビューポートのワールド変換後にセーフエリアのワールド座標変換も同時に行う
            Update_SafePosition();

        }
        /// <summary>
        /// キャラ表示用のセーフエリア座標をワールド座標へ変換
        /// Update_ViewportPosition()
        /// </summary>
        /// <param name="z"></param>
        private void Update_SafePosition()
        {
            // 下だけ固定
            float down_y = this.viewportLeftDown.y;

            float z = this.viewportLeftTop.z;

            Vector3 leftTop = Vector3.Lerp(this.screenCenterWorldPosition, this.viewportLeftTop, 0.5f);
            leftTop.z = z;
            Vector3 leftDown = leftTop;
            leftDown.y = down_y;

            this.safeLeftTop = leftTop;
            this.safeLeftDown = leftDown;

            Vector3 rightTop = Vector3.Lerp(this.screenCenterWorldPosition, this.viewportRightTop, 0.5f);
            rightTop.z = z;
            Vector3 rightDown = rightTop;
            rightDown.y = down_y;

            this.safeRightTop = rightTop;
            this.safeRightDown = rightDown;
        }
        public enum SafeOutside
        {
            None
            , Top
            , Down
            , Left
            , Right
        }
        /// <summary>
        /// セーフエリア内チェック
        /// </summary>
        /// <param name="posi"></param>
        /// <returns>セーフエリア内ならtrue</returns>
        public SafeOutside IsInSafeArea(Vector3 posi)
        {
            if (this.safeLeftTop.x > posi.x)
            {
                return SafeOutside.Left;
            }
            if (this.safeRightTop.x < posi.x)
            {
                return SafeOutside.Right;
            }
            if (this.safeLeftTop.y < posi.y)
            {
                return SafeOutside.Top;
            }
            if (this.safeLeftDown.y > posi.y)
            {
                return SafeOutside.Down;
            }
            return SafeOutside.None;

        }

        public Vector3 GetViewportLeftTopScreenPosition()
        {
            return GetWorldToScreenPosition(this.viewportLeftTop);
        }
        public Vector3 GetViewportLeftDownScreenPosition()
        {
            return GetWorldToScreenPosition(this.viewportLeftDown);
        }
        public Vector3 GetViewportRightTopScreenPosition()
        {
            return GetWorldToScreenPosition(this.viewportRightTop);
        }
        public Vector3 GetViewportRightDownScreenPosition()
        {
            return GetWorldToScreenPosition(this.viewportRightDown);
        }

        /// <summary>
        /// 指定ワールドzにおける左上ワールド座標を求める
        /// 0指定ならviewportLeftTopと同じ
        /// </summary>
        /// <param name="_worldZ">ワールドz</param>
        /// <returns>ビューポート左上のワールド座標</returns>
        public Vector3 GetViewportLeftTop(float _worldZ)
        {
            float z = _worldZ - this.mainCamera.transform.localPosition.z;
            Vector3 pos = CameraController.Instance.mainCamera.ViewportToWorldPoint(new Vector3(0, 1, z));
            return pos;
        }
        /// <summary>
        /// 指定ワールドzにおける右下ワールド座標を求める
        /// 0指定ならviewportRightDownと同じ
        /// </summary>
        /// <param name="_worldZ">ワールドz</param>
        /// <returns>ビューポート右下のワールド座標</returns>
        public Vector3 GetViewportRightDown(float _worldZ)
        {
            float z = _worldZ - this.mainCamera.transform.localPosition.z;
            Vector3 pos = CameraController.Instance.mainCamera.ViewportToWorldPoint(new Vector3(1, 0, z));
            return pos;
        }


        /// <summary>
        /// ワールド座標をスクリーン座標へ変換した値を得る
        /// z成分は0にした値を返す
        /// </summary>
        /// <param name="worldPos">ワールド座標</param>
        /// <remarks>
        /// 描画先となるRenderTextureの解像度のスケール処理が施されます
        /// </remarks>
        /// <returns></returns>
        public Vector3 GetWorldToScreenPosition(Vector3 worldPos)
        {
            Vector3 scrPosi = this.mainCamera.WorldToScreenPoint(worldPos);
            ////            scrPosi *= this.resolutionRenderer.inverseScale;
            scrPosi *= this.resolutionRenderer.inverseSettingScale;
            scrPosi.z = 0.0f;
            return scrPosi;
        }
        /// <summary>
        /// ワールド座標をUIスクリーン座標へ変換した値を得る
        /// z成分は0にした値を返す
        /// </summary>
        /// <param name="worldPos">ワールド座標</param>
        /// <remarks>
        /// 描画先となるRenderTextureの解像度のスケール処理が施されます
        /// </remarks>
        /// <returns></returns>
        public Vector3 GetWorldToUICameraScreenPosition(Vector3 worldPos)
        {
            Vector3 scrPosi = worldPos;
            if (mUICanvas != null && mUICanvas.renderMode != RenderMode.ScreenSpaceOverlay && mUICanvas.worldCamera != null)
            {
                Camera uiCamera = mUICanvas.worldCamera;
                scrPosi = this.UICamera.WorldToScreenPoint(worldPos);
                ////            scrPosi *= this.resolutionRenderer.inverseScale;
                scrPosi *= this.resolutionRenderer.inverseSettingScale;
                scrPosi.z = 0.0f;
            }
            return scrPosi;
        }
        /// <summary>
        /// スクリーン座標をワールド座標に変換した値を得る
        /// </summary>
        /// <param name="scrPosi">スクリーン座標</param>
        /// <remarks>
        /// 描画先となるRenderTextureの解像度のスケール処理が施されます
        /// </remarks>
        /// <returns></returns>
        public Vector3 GetScreenToWorldPosition(Vector3 scrPosi, float z = 0.0f)
        {
            ////            scrPosi *= this.resolutionRenderer.scale;
            scrPosi *= this.resolutionRenderer.settingScale;
            scrPosi.z = z - this.mainCamera.transform.position.z;
            Vector3 wrdPos = this.mainCamera.ScreenToWorldPoint(scrPosi);
            return wrdPos;
        }

        /// <summary>
        /// ワールド座標をカメラのビューポート座標へ変換した値を得る
        /// </summary>
        /// <param name="worldPos">ビューポート座標(0-1)</param>
        /// <returns></returns>
        public Vector3 GetWorldToViewportPoint(Vector3 worldPos)
        {
            Vector3 viewPosi = CameraController.Instance.mainCamera.WorldToViewportPoint(worldPos);
            viewPosi *= this.resolutionRenderer.settingScale;
            viewPosi.z = 0;
            return viewPosi;
        }

        /// <summary>
        /// ワールド座標をUIカメラ座標に変換した値を得る
        /// zはUIカメラの設定されたキャンバスの距離
        /// </summary>
        /// <param name="worldPos">ワールド座標</param>
        /// <remarks>
        /// 描画先となるRenderTextureの解像度のスケール処理が施されます
        /// </remarks>
        /// <returns></returns>
        public Vector3 GetWorldToUICameraPosition(Vector3 worldPos)
        {
            Vector3 posi = this.mainCamera.WorldToViewportPoint(worldPos);
            ////            scrPosi *= this.resolutionRenderer.inverseScale;
            posi *= this.resolutionRenderer.inverseSettingScale;

            if (mUICanvas != null && mUICanvas.renderMode == RenderMode.ScreenSpaceCamera && mUICanvas.worldCamera != null)
            {
                Camera uiCamera = mUICanvas.worldCamera;
                posi.z = mUICanvas.planeDistance;
                posi = UICamera.ViewportToWorldPoint(posi);
            }
            else
            {
                posi.z = 0;
            }

            return posi;
        }

        /// <summary>
        /// キャンパス上のワールド座標をメインカメラのわーる座標に変換した値を得る
        /// GetScreenToUICameraPosition()で取得したキャンパス上のワールド座標を第一引数に渡してください。
        /// </summary>
        /// <param name="worldPosi"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3 GetUICameraWorldToWorldPosition(Vector3 worldPosi, float z = 0f)
        {
            Vector3 posi = Vector3.zero;
            if (mUICanvas != null && mUICanvas.renderMode == RenderMode.ScreenSpaceCamera && mUICanvas.worldCamera != null)
            {
                Camera uiCamera = mUICanvas.worldCamera;
                posi = uiCamera.WorldToViewportPoint(worldPosi);
                posi.z = z - this.mainCamera.transform.position.z;
                posi = this.mainCamera.ViewportToWorldPoint(posi);
            }
            else
            {
                posi = GetScreenToWorldPosition(worldPosi, z);
            }

            return posi;
        }
        /// <summary>
        /// キャンバス上のスクリーン座標をワールド座標に変換した値を得る
        /// </summary>
        /// <param name="scrPosi">スクリーン座標</param>
        /// <remarks>
        /// 描画先となるRenderTextureの解像度のスケール処理が施されます
        /// </remarks>
        /// <returns></returns>
        public Vector3 GetUICameraScreenToWorldPosition(Vector3 scrPosi, float z = 0.0f)
        {
            Vector3 posi = Vector3.zero;
            if (mUICanvas != null && mUICanvas.renderMode == RenderMode.ScreenSpaceCamera && mUICanvas.worldCamera != null)
            {
                Camera uiCamera = mUICanvas.worldCamera;
                posi = UICamera.ScreenToViewportPoint(scrPosi);
                posi.z = z - this.mainCamera.transform.position.z;
                posi = this.mainCamera.ViewportToWorldPoint(posi);
            }
            else
            {
                posi = GetScreenToWorldPosition(scrPosi, z);
            }

            return posi;
        }

        /// <summary>
        /// スクリーン座標をUIカメラ座標に変換した値を得る
        /// zはUIカメラの設定されたキャンバスの距離
        /// </summary>
        /// <param name="scrPos">スクリーン座標</param>
        /// <returns></returns>
        public Vector3 GetScreenToUICameraPosition(Vector3 scrPos)
        {
            Vector3 posi = scrPos;

            if (mUICanvas != null && mUICanvas.renderMode == RenderMode.ScreenSpaceCamera && mUICanvas.worldCamera != null)
            {
                posi = this.mainCamera.ScreenToViewportPoint(scrPos);
                Camera uiCamera = mUICanvas.worldCamera;
                posi.z = mUICanvas.planeDistance;
                posi = UICamera.ViewportToWorldPoint(posi);
            }
            else
            {
                posi.z = 0;
            }

            return posi;
        }

        /// <summary>
        /// 特定カメラの消失点座標設定
        /// </summary>
        /// <param name="cameraId">設定対象カメラID</param>
        /// <param name="center">消失点の座標
        /// ±0.5で画面端がカメラの視線方向となります
        /// </param>
        public void SetVanishingPoint(CameraID cameraId, Vector2 center)
        {
#if GAME_DEBUG
            if (mDebugDisableVanishingPoint)
            {
                return;
            }
#endif
            var cam = GetCamera(cameraId);
            cam.SetProjectionVanishingPoint(center);
            mVanishingPointList[(int)cameraId] = center;
        }
        /// <summary>
        /// 指定カメラの消失点座標取得
        /// </summary>
        /// <param name="cameraId">対象のカメラID</param>
        /// <returns></returns>
        public Vector2 GetVanishingPoint(CameraID cameraId)
        {
            return mVanishingPointList[(int)cameraId];
        }
        /// <summary>
        /// 全カメラの消失点値のリセット
        /// </summary>
        private void ResetAllVanishingPointValue()
        {
            for (int i = 0; i < mVanishingPointList.Count; ++i)
            {
                mVanishingPointList[i] = Vector2.zero;
            }
        }

        /// <summary>
        /// 画面の色反転リクエスト
        /// </summary>
        /// <param name="inverse">trueの時反転方向へ</param>
        /// <param name="time">変化時間</param>
        public void InverseColor(bool inverse, float time = 0.0f)
        {
            m_pfxInverseColor.Request(inverse, time);
        }

        /// <summary>
        /// ブラーONリクエスト
        /// </summary>
        /// <param name="time">処理時間</param>
        /// <param name="factor">ブラー強度
        /// 0.0でOFF、値が大きい程ボケが強くなりますが大きすぎると簡略化したブラー処理の都合上
        /// エッジが目立つようになり期待するボケ方になりません。
        /// 許容される値はおよそ7.0位までです。
        /// </param>
        public static void BlurOn(float time = 0.5f, float factor = 1.5f)
        {
            mInstance.m_pfxBlur.Request(time, factor);
        }
        /// <summary>
        /// ブラーOFFリクエスト
        /// </summary>
        /// <param name="time">処理時間</param>
        public static void BlurOff(float time = 0.5f)
        {
            mInstance.m_pfxBlur.Request(time, 0.0f);
        }


        public static void SetOverlayTexture(Texture2D texture)
        {
            mInstance.m_fxOverlay.SetTexture(texture);
        }
        public static void SetOverlayColor(Color color)
        {
            mInstance.m_fxOverlay.SetColor(color);
        }
        public static void EnableOverlay(bool flag)
        {
            mInstance.m_fxOverlay.SetEnable(flag);
        }
        public static void ChangeOverlayColor(Color color, float time, System.Action onEnd = null)
        {
            mInstance.m_fxOverlay.ChangeColor(color, time, onEnd);
        }
        /// <summary>
        /// オーバーレイの色変化処理中か判定
        /// </summary>
        /// <returns>trueの時、処理中</returns>
        public static bool IsChangingOverlayColor()
        {
            return mInstance.m_fxOverlay.IsChangeColorRunning();
        }

        /// <summary>
        /// ダメージ用オーバーレイエフェクトのカラー設定
        /// </summary>
        /// <param name="color"></param>
        public static void SetOverlayDamageColor(Color color)
        {
            mInstance.m_fxOverlayDamage.SetColor(color);
        }
        /// <summary>
        /// ダメージ用オーバーレイエフェクトカラー変更
        /// </summary>
        /// <param name="color"></param>
        /// <param name="time"></param>
        /// <param name="onEnd"></param>
        public static void ChangeOverlayDamageColor(Color color, float time, System.Action onEnd = null)
        {
            mInstance.m_fxOverlayDamage.ChangeColor(color, time, onEnd);
        }
        public static void EnableOverlayDamage(bool flag)
        {
            mInstance.m_fxOverlayDamage.SetEnable(flag);
        }
*/
    }
/*
    [System.Serializable]
    public class CameraData : Unity.Cinemachine.AxisState.IInputAxisProvider
    {
        [SerializeField]
        eCameraID                                           m_CameraID;
        public eCameraID                                    CameraID    => m_CameraID;

        [SerializeField]
        protected CinemachineVirtualCamera                  m_VCam;
        public CinemachineVirtualCamera                     VCam        => m_VCam;

        Unity.Cinemachine.CinemachineFramingTransposer      m_Transposer;
        Unity.Cinemachine.CinemachinePOV                    m_CinemachinePOV;

        [SerializeField]
        float                                               m_RotatePowerX = 0;
        [SerializeField]
        float                                               m_RotatePowerY = 0;

        public CameraController                             m_OwnerCtrl { get; private set; }
        float                                               m_OrigAccelTime = 0.1f;
        float                                               m_OrigDecelTime = 0.1f;

        /// <summary>
        /// カメラID
        /// m_CameraTableに登録されているカメラ順
        /// </summary>
        public enum eCameraID
        {
            Normal,
            Demo,
            Free,

            Debug
        }

        public void Initialize(CameraController owner)
        {
            m_OwnerCtrl         = owner;

            m_Transposer        = m_VCam.GetCinemachineComponent<Unity.Cinemachine.CinemachineFramingTransposer>();
            m_CinemachinePOV    = m_VCam.GetCinemachineComponent<Unity.Cinemachine.CinemachinePOV>();

            m_OrigAccelTime     = m_CinemachinePOV.m_HorizontalAxis.m_AccelTime;
            m_OrigDecelTime     = m_CinemachinePOV.m_HorizontalAxis.m_DecelTime;
        }

        public void SetInputAxis()
        {
            m_CinemachinePOV.m_HorizontalAxis.SetInputAxisProvider(0, this);
            m_CinemachinePOV.m_VerticalAxis.SetInputAxisProvider(1, this);
        }

        public void SetCurrent(bool enable)
        {
            m_VCam.Priority = enable ? 10 : -100;
        }

        public float GetAngleY() => m_CinemachinePOV.m_HorizontalAxis.Value;
        public void SetAngleY(float angle) => m_CinemachinePOV.m_HorizontalAxis.Value = angle;

        public void SetTarget(Transform target)
        {
            m_VCam.Follow = target;
            m_VCam.LookAt = target;
        }

        public void SetMouseMode(bool enable)
        {
            if (enable)
            {
                m_CinemachinePOV.m_HorizontalAxis.m_AccelTime = 0.01f;
                m_CinemachinePOV.m_HorizontalAxis.m_DecelTime = 0.01f;
            }
            else
            {
                m_CinemachinePOV.m_HorizontalAxis.m_AccelTime = m_OrigAccelTime;
                m_CinemachinePOV.m_HorizontalAxis.m_DecelTime = m_OrigDecelTime;
            }
        }

        public void ResetRotateAxisToForward(float duration)
        {
            var forward = m_VCam.Follow.forward;
            forward.y = 0;
            forward.Normalize();
            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            if (duration > 0)
            {
                var seq = DOTween.Sequence();

                seq.Join(
                    DOVirtual.Float(_cinemachinePOV.m_HorizontalAxis.Value, angle, duration, value =>
                    {
                        _cinemachinePOV.m_HorizontalAxis.Value = value;
                    }).SetEase(Ease.OutCubic)
                );
            }
            else
            {
                _cinemachinePOV.m_HorizontalAxis.Value = angle;
            }
        }

        public void SetRotateAxisByDir(Vector3 dir)
        {
            var forward = dir;
            forward.y = 0;
            forward.Normalize();
            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            m_CinemachinePOV.m_HorizontalAxis.Value = angle;
        }

        float Unity.Cinemachine.AxisState.IInputAxisProvider.GetAxisValue(int axis)
        {
            if (m_OwnerCtrl.IsLockingOn && m_OwnerCtrl.IsFreeCameraMode == false) return 0;
            if (m_OwnerCtrl._stopRotate) return 0;

            // Y軸
            if (axis == 0)
            {
                return m_OwnerCtrl.m_RotateAxisValue_Y * m_RotatePowerY;
            }
            // X軸
            if (axis == 1)
            {
                return m_OwnerCtrl.m_RotateAxisValue_X * m_RotatePowerX;
            }

            return 0;
        }

        // FreeCmaera専用更新処理
        public void FreeCameraUpdate()
        {
            if (m_OwnerCtrl.m_FreeCameraCharacter == null) return;

            var actions = m_OwnerCtrl.m_FreeCameraCharacter.InputProvider.GetInputActions(true);

            // 通常モードへ
            if (actions != null &&
                actions.Count >= 1)
            {
                if (actions.Contains(GameInputActionTypes.Menu))
                {
                    m_OwnerCtrl.SetFreeCameraMode(false, null);
                    return;
                }
            }

            // カメラ操作
            var opeData = m_OwnerCtrl.m_FreeCameraCharacter.InputProvider.GetFreeCameraModeOperation();

            const float kMaxOffset = 0.4f;

            // X
            _transposer.m_ScreenX -= opeData.axisL.x * 0.01f;
            if (_transposer.m_ScreenX < -kMaxOffset + 0.5f) _transposer.m_ScreenX = -kMaxOffset + 0.5f;
            if (_transposer.m_ScreenX > kMaxOffset + 0.5f) _transposer.m_ScreenX = kMaxOffset + 0.5f;

            // Y
            _transposer.m_ScreenY += opeData.axisL.y * 0.01f;
            if (_transposer.m_ScreenY < -kMaxOffset + 0.5f) _transposer.m_ScreenY = -kMaxOffset + 0.5f;
            if (_transposer.m_ScreenY > kMaxOffset + 0.5f) _transposer.m_ScreenY = kMaxOffset + 0.5f;

            // 回転
            if (SaveData.GameSettings.Instance.CameraRotateMode)
            {
                opeData.axisR.x *= -1;
            }

            OwnerCtrl.SetRotateAxisX(opeData.axisR.y * SaveData.GameSettings.Instance.CameraRotateSpeed);
            OwnerCtrl.SetRotateAxisY(opeData.axisR.x * SaveData.GameSettings.Instance.CameraRotateSpeed);

            // Zoom
            _transposer.m_CameraDistance -= opeData.zoom * 0.05f;
            if (_transposer.m_CameraDistance < 0.5f) _transposer.m_CameraDistance = 0.5f;
            if (_transposer.m_CameraDistance > 3.0f) _transposer.m_CameraDistance = 3.0f;

            // キーボード専用
            if (Keyboard.current != null)
            {
                if (Keyboard.current.digit1Key.isPressed)
                {
                    _vCam.m_Lens.FieldOfView -= 10 * Time.unscaledDeltaTime;
                    if (_vCam.m_Lens.FieldOfView < 10) _vCam.m_Lens.FieldOfView = 10;
                }
                if (Keyboard.current.digit2Key.isPressed)
                {
                    _vCam.m_Lens.FieldOfView += 10 * Time.unscaledDeltaTime;
                    if (_vCam.m_Lens.FieldOfView > 179) _vCam.m_Lens.FieldOfView = 179;
                }
            }

            // 時間進める
            if (actions.Contains(GameInputActionTypes.Guard))
            {
                if (OwnerCtrl._freeCameraModeTimeScaleNode != null)
                {
                    if (OwnerCtrl._freeCameraModeTimeScaleNode._timeScale == 0)
                    {
                        OwnerCtrl._freeCameraModeTimeScaleNode._timeScale = 1.0f;
                    }
                    else
                    {
                        OwnerCtrl._freeCameraModeTimeScaleNode._timeScale = 0.0f;
                    }
                }
            }
        }
    }
*/
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;
//using Cysharp.Threading.Tasks.Triggers;

using Unity.Cinemachine;

//using DG.Tweening;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // 全カメラデータ
    [SerializeField] CameraData[] _playerVirtualCams;

    // NormalCameraのみのリスト
    List<CameraData> _normalOnlyCams;

    // 
    [Header("※Normal CameraのみのIndex")]
    [SerializeField] int _nowNormalCameraIndex = 2;
    //    public int NowNormalCameraIndex => _nowNormalCameraIndex;

    // NormaCamera専用の現在のカメラ
    public CameraData CurrentNormalCamData => _normalOnlyCams[_nowNormalCameraIndex];

    // 全てのカメラ対象の現在のカメラ
    public ReactiveProperty<CameraData> CurrentCamData { get; set; } = new();

    public CinemachineVirtualCamera CurrentVCam => _playerVirtualCams[_nowNormalCameraIndex].VCam;

    // 
    float _rotateAxisValue_X = 0;
    float _rotateAxisValue_Y = 0;

    bool _stopRotate;

    public bool IsFreeCameraMode => CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera;

    public CameraData GetNormalCameraDataByIndex(int index)
    {
        return _normalOnlyCams[index];
    }


    void Awake()
    {
        DebugLogger.Log("[Test]Awake", DebugLogger.Colors.yellow);
        ICameraManager.Instance.SetBattleCamera(this);

        // 初期設定
        // Normal Cameraのみリストの作成
        _normalOnlyCams = new();
        for (int i = 0; i < _playerVirtualCams.Length; i++)
        {
            _playerVirtualCams[i].Initialize(this);

            if (_playerVirtualCams[i].CamType == CameraData.CameraTypes.NormalCamera)
            {
                _normalOnlyCams.Add(_playerVirtualCams[i]);
            }
        }

    }

    void Start()
    {
        DebugLogger.Log("[Test]Start", DebugLogger.Colors.yellow);
        // 初期設定
        for (int i = 0; i < _playerVirtualCams.Length; i++)
        {
            _playerVirtualCams[i].SetInputAxis();
        }

        // カメラ変更時
        CurrentCamData.Subscribe(camData =>
        {
            if (camData == null) return;

            for (int i = 0; i < _playerVirtualCams.Length; i++)
            {
                if (camData == _playerVirtualCams[i])
                {
                    _playerVirtualCams[i].SetCurrent(true);
                }
                else
                {
                    _playerVirtualCams[i].SetCurrent(false);
                }
            }

        });

        ChangePlayerCamIndex(_nowNormalCameraIndex, 0);

        // 更新処理
        this.UpdateAsObservable()
            .Where(_ => enabled)
            .Subscribe(_ =>
            {
                for (int i = 0; i < _playerVirtualCams.Length; i++)
                {
                    _playerVirtualCams[i].SetInputAxis();
                }

                // 角度同期
                {
                    var nowVCamData = _normalOnlyCams[_nowNormalCameraIndex];
                    float angleY = nowVCamData.GetAngleY();

                    foreach (var vcamData in _normalOnlyCams)
                    {
                        if (vcamData.Equals(nowVCamData)) continue;

                        vcamData.SetAngleY(angleY);
                    }
                }

                // ロック生存判定
                if (_lockOnTarget != null &&
                    _lockOnTarget.CharaCore != null &&
                    _lockOnTarget.ObjGroupID.SpStatus_IsDead)
                {
                    _lockOnTarget = null;
                }

                // フリーカメラ
                if (CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera)
                {
                    CurrentCamData.Value.FreeCameraUpdate();
                }
            });

        this.FixedUpdateAsObservable()
            .Where(_ => enabled)
            .Subscribe(_ =>
            {
                // 
                _rotateAxisValue_Y *= 0.8f;
            });

        // 
        IGameInputHandler.Instance.OnChangeDevice.Subscribe(device =>
        {
            if (device == IGameInputHandler.DevideTypes.Keyboard)
            {
                CameraMouseMode(true);
            }
            else
            {
                CameraMouseMode(false);
            }
        }).AddTo(this);
        // 
    }

    async void OnEnable()
    {
        await UniTask.DelayFrame(1);
        ChangePlayerCamIndex(_nowNormalCameraIndex, 0);
    }

    public void SetLockOnTarget(SearchTarget target)
    {
        _lockOnTarget = target;
    }

    public void CameraMouseMode(bool enable)
    {
        foreach (var data in _playerVirtualCams)
        {
            data.SetMouseMode(enable);
        }
    }

    public async void SetFreeCameraMode(bool enable, CharacterCore chara)
    {
        if (enable)
        {
            // 現在フリーカメラモード
            if (CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera) return;

            // FreeCameraを検索し、セット
            foreach (var camData in _playerVirtualCams)
            {
                if (camData.CamType == CameraData.CameraTypes.FreeCamera)
                {
                    CurrentCamData.Value = camData;

                    _freeCameraCharacter = chara;
                    if (_freeCameraCharacter != null)
                    {
                        _freeCameraCharacter.FreeCameraMode = true;
                    }

                    _freeCameraModeDisposable.Clear();

                    // 殴られた時
                    chara.OnDamageEvent.Subscribe(param =>
                    {
                        if (param.eveType == Damages.IDamageApplicable.DamageEventTypes.ReceivedDamage)
                        {
                            SetFreeCameraMode(false, null);
                        }
                    }).AddTo(_freeCameraModeDisposable);

                    _freeCameraModeHidePlayerUI = new();
                    _freeCameraModeHidePlayerUI.Enable(null);

                    _freeCameraModeHideWorldUI = new();
                    _freeCameraModeHideWorldUI.Enable(null);

                    _freeCameraModeDisablePlayerOpe = new();
                    _freeCameraModeDisablePlayerOpe.Enable(null);

                    Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
                    brain.IgnoreTimeScale = true;

                    _freeCameraModeTimeScaleNode?.Dispose();
                    _freeCameraModeTimeScaleNode = ITimeScaleManager.Instance.AddTimeScale(0, -1);

                    return;
                }
            }

        }
        else
        {
            await UniTask.DelayFrame(2);

            // Normal Cameraをセット
            CurrentCamData.Value = _normalOnlyCams[_nowNormalCameraIndex];

            if (_freeCameraCharacter != null)
            {
                _freeCameraCharacter.FreeCameraMode = false;
                _freeCameraCharacter.CharaAnimator.SetInteger("DoPerformanceNo", 99);
            }

            _freeCameraModeTimeScaleNode?.Dispose();
            _freeCameraModeTimeScaleNode = null;

            _freeCameraCharacter = null;

            _freeCameraModeHidePlayerUI?.Dispose();
            _freeCameraModeHidePlayerUI = null;

            _freeCameraModeHideWorldUI?.Dispose();
            _freeCameraModeHideWorldUI = null;

            _freeCameraModeDisablePlayerOpe?.Dispose();
            _freeCameraModeDisablePlayerOpe = null;

            Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
            brain.IgnoreTimeScale = false;

            _freeCameraModeTimeScaleNode?.Dispose();
            _freeCameraModeTimeScaleNode = null;
        }
    }

    public async void ChangePlayerCamIndex(int index, float blendDuration)
    {
        if (_stopRotate) return;
        _stopRotate = true;

        _nowNormalCameraIndex = index;

        // Cinemachine Blendの時間を設定
        Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
        brain.DefaultBlend.Time = blendDuration;

        // 現在のカメラデータを記憶
        CurrentCamData.Value = _normalOnlyCams[_nowNormalCameraIndex];

        if (blendDuration > 0)
        {
            await UniTask.Delay((int)(blendDuration * 1000));
        }
        _stopRotate = false;
    }

    public void ChangePlayerCamTypeToNext(bool next, float blendDuration)
    {
        if (CurrentCamData != null && CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera) return;

        if (_normalOnlyCams.Count == 0) return;

        int idx = _nowNormalCameraIndex;

        idx += next ? 1 : -1;
        if (idx < 0) idx = 0;
        if (idx >= _normalOnlyCams.Count) idx = _normalOnlyCams.Count - 1;

        ChangePlayerCamIndex(idx, blendDuration);
    }

    public void ResetRotateAxisToForward(float duration)
    {
        _tweenResetCam.Kill();

        var seq = DOTween.Sequence();
        _tweenResetCam = seq;

        // セット
        _normalOnlyCams[_nowNormalCameraIndex].ResetRotateAxisToForward(duration);
    }

    public void SetRotateAxisByDir(Vector3 dir)
    {
        var forward = dir;
        forward.y = 0;
        forward.Normalize();
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        // セット
        _normalOnlyCams[_nowNormalCameraIndex].SetRotateAxisByDir(dir);
    }

    public void SetRotateAxisY(float value)
    {
        _rotateAxisValue_Y = value;
    }
    public void SetRotateAxisX(float value)
    {
        _rotateAxisValue_X = value;
    }

    // 子GameObjectのVirtualCameraに、targetを設定
    public void SetCameraTarget(Transform target)
    {
        foreach (var vcamData in _playerVirtualCams)
        {
            vcamData.SetTarget(target);
        }
    }
}
*/