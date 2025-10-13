//==================================================================
/// <summary>
/// �J��������N���X
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
        /// �Ώۂ̃J�����擾
        /// </summary>
        /// <param name="camid">�J����ID</param>
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
        /// �e�J�����̃f�t�H���gCullingMask�i�o�g���p�j
        /// </summary>
        [SerializeField]
        private LayerMask[] m_battleLayerMasks;

        /// <summary>
        /// �e�J�����̃f�t�H���gCullingMask�i�z�[���p�j
        /// </summary>
        [SerializeField]
        private LayerMask[] m_homeLayerMasks;
        /// <summary>
        /// �e�J�����̃f�t�H���gCullingMask�i���r�[�p�j
        /// </summary>
        [SerializeField]
        private LayerMask[] m_lobbyLayerMasks;
        /// <summary>
        /// �����E�F�C��ʗpCullingMask
        /// </summary>
        [SerializeField]
        private LayerMask[] m_runwwayLayerMasks;

        /// <summary>
        /// �e�J�����Ɏw�肵��LayerMask�f�[�^��ݒ肷��
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
        [SerializeField] private ResolutionRenderer m_resolutionRenderer2 = new ResolutionRenderer();   //������ʗp
        [SerializeField] private ResolutionRenderer m_resolutionRendererUI = new ResolutionRenderer();
        /// <summary>
        /// �|�X�g�G�t�F�N�g�i�u���[�j
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxBlur m_pfxBlur;
        /// <summary>
        /// �|�X�g�G�t�F�N�g�i�u���[���j
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxBloom m_pfxBloom;
        /// <summary>
        /// �|�X�g�G�t�F�N�g�i�F���]�j
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxInverseColor m_pfxInverseColor;
        /// <summary>
        /// �|�X�g�G�t�F�N�g�i���˃u���[�j
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.PfxRadialBlur m_pfxRadialBlur;
        /// <summary>
        /// �|�X�g�G�t�F�N�g�i��ʊE�[�x�j
        /// </summary>
        [SerializeField] private XeApp.Game.PostEffect.ScreenOverlayBlur m_pfxDepthOfField;
        [SerializeField] private XeApp.Game.PostEffect.ScreenOverlayBlur m_pfxDepthOfField_Back;

        [SerializeField] private FxScreenOverlay m_fxOverlay;
        [SerializeField] private FxScreenOverlay m_fxOverlayDamage;

        // �J���������ʒu
        private Vector3 mInitPosition = new Vector3(-3.75f, 1.5f, -8.7f);


        [SerializeField] private float initFov;

        // �r���{�[�h�ׂ̈̋t�N�H�[�^�j�I��
        public Quaternion inverseCameraRotation { get; private set; }
#if UNITY_EDITOR
        [SerializeField] bool IsGizmoDebugDisplay = true;
#endif
        /// <summary>
        /// �J�����R���g���[���̃C���X�^���X
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
        /// �u���[���̃C���X�^���X�擾
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
        /// ���˃u���[�̃C���X�^���X�擾
        /// </summary>
        public PostEffect.PfxRadialBlur radialBlur { get { return m_pfxRadialBlur; } }

        /// <summary>
        /// �J�����̗��p���[�h
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
        /// �ʒu�X�V�̖����L��
        /// </summary>
        public bool isTransformAvailable { get; set; }

        /// <summary>
        /// �J�����̃����_�����O��
        /// </summary>
        public float cameraRenderWidth { get; private set; }
        /// <summary>
        /// �J�����̃����_�����O����
        /// </summary>
        public float cameraRenderHeight { get; private set; }

        //�r���[�|�[�g�̃��[���h���W�ێ��p
        public Vector3 viewportLeftTop { get; private set; }
        public Vector3 viewportRightDown { get; private set; }
        public Vector3 viewportRightTop { get; private set; }
        public Vector3 viewportLeftDown { get; private set; }

        // �L������\������Z�[�t�G���A�̃��[���h���W�ێ�
        public Vector3 safeLeftTop { get; private set; }
        public Vector3 safeLeftDown { get; private set; }
        public Vector3 safeRightTop { get; private set; }
        public Vector3 safeRightDown { get; private set; }

        private Vector3 mMainCameraForwardPosition;

        // �J�����ړ��p
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

        //�����_�ړ��p
        private Vector2 mTargetUIModelCameraVanishingPoint = Vector2.zero;
        private float mUIModelCameraVanshingPointDelayCoef = 0.2f;

        // �X�N���[���̒��S�ʒu�i�X�N���[�����W�ƃ��[���h���W
        public Vector3 screenCenterPosition { get; private set; }
        public Vector3 screenCenterWorldPosition { get; private set; }

        // �X�V�֐�
        System.Action mUpdater;

        /// <summary>
        /// ���C���J������Transform���p�Ɠ��������邽�߂̃t���O
        /// </summary>
        private List<bool> mLinkToMainCameraList;


        [SerializeField] private bool mIsCameraUpdateFlag = true;
        [SerializeField] private bool mIsUIModelCameraUpdateFlag = false;

        [SerializeField] private Canvas mUICanvas;  //UI�`��p�̃L�����o�X

        /// <summary>
        /// �e�J�����̏����_���W
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
            //���C���J�����Ƃ̃����N����
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
        /// �w��J�����̃A�N�e�B�u�ݒ�
        /// </summary>
        /// <param name="camId">�Ώۂ̃J����ID</param>
        /// <param name="activate">true�̎��A�L��</param>
        public void ActiveCamera(CameraID camId, bool activate)
        {
            var cam = GetCamera(camId);
            cam.gameObject.SetActive(activate);
        }

        /// <summary>
        /// �V�X�e���𑜓x�؂�ւ����N�G�X�g���̏���
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
        /// 3D�𑜓x�؂�ւ����N�G�X�g���̏���
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
        /// �o�g���p�����_������
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
        /// ���j���[�p�����_������
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
        /// ���r�[�pRenderer�ݒ菈��
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
        /// ���r�[�p���Z�b�g����
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
        /// ���C�����j���[�p���Z�b�g����
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
        /// �|�X�g�G�t�F�N�g�ɃQ�[���ݒ��K�p
        /// </summary>
        public void ApplyPostEffect()
        {
            m_pfxBlur.Apply();
            m_pfxBloom.Apply();
            m_pfxInverseColor.Apply();
        }

        /// <summary>
        /// �o�g���p�S���Z�b�g����
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

            //3D��ԉ𑜓x�ؑ֕`��
            SetupRenderer_Battle(false);
            //�|�X�g�G�t�F�N�g
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

            // �����ʒu
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
        /// ��ʊE�[�x�̗L����
        /// �t���O��ݒ肷��̂݁B
        /// </summary>
        /// <param name="enable"></param>
        public void SetPfxDepthOfFieldEnable(bool enable)
        {
            mIsBattleDofEnable = enable;
        }

        /// <summary>
        /// �w�肵���J���������C���J�����̏��ƃ����N������
        /// 
        /// Transform, fieldOfView, nearClipPlane, farClipPlane�������N����܂�
        /// </summary>
        /// <param name="targetID">�Ώۂ̃J����</param>
        /// <param name="linked">true�̎��A�����N�����</param>
        public void LinkToMainCamera(CameraID targetID, bool linked)
        {
            if (CameraID.Main == targetID)
            {
                //���C���ƃ����N������׃��C���̎w��͋֎~
                return;
            }
            mLinkToMainCameraList[(int)targetID] = linked;
        }

        /// <summary>
        /// �J�����ړ��̃f�B���C���ʌW���̐ݒ�
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
        /// �J�����̈ړ��ڕW�ʒu��ݒ�
        /// SetUpdate( true )���ɗL��
        /// </summary>
        /// <param name="_target"></param>
        public void SetTargetPosition(Vector3 _target)
        {
            mTargetPosition = _target;
        }
        /// <summary>
        /// �J�����̈ړ��ڕW�ʒu��ݒ�
        /// UIModel�J�����p
        /// SetUpdate( true )���ɗL��
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
        /// �J�����ʒu�ݒ�
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
        /// ��]�̐ݒ�
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
        /// Near�N���b�v�ʐݒ�
        /// </summary>
        /// <param name="_near"></param>
        public void SetNear(float _near)
        {
            this.mainCamera.nearClipPlane = _near;
        }
        /// <summary>
        /// Far�N���b�v�ʐݒ�
        /// </summary>
        /// <param name="_far"></param>
        public void SetFar(float _far)
        {
            this.mainCamera.farClipPlane = _far;
        }

        /// <summary>
        /// SetFov�ɂĐݒ肵��������p�l
        /// </summary>
        public float settingVerticalFov { get; private set; }

        /// <summary>
        /// ������p��ݒ�
        /// </summary>
        /// <param name="sourceVFov">���ʂł̐�����p</param>
        /// <param name="fitHorizontal">true�̎��A�A�X�y�N�g��Ɋւ�炸������p����v������</param>
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
        /// ������p�̎擾
        /// </summary>
        /// <returns></returns>
        public float GetFov()
        {
            return GetCamera(CameraID.Main).fieldOfView;
        }
        /// <summary>
        /// ����p�̎擾
        /// </summary>
        /// <param name="vfov">������p�i�[��</param>
        /// <param name="hfov">������p�i�[��</param>
        public void GetFov(out float vfov, out float hfov)
        {
            vfov = this.mainCamera.fieldOfView;
            hfov = GameRenderUtil.GetHorizontalFov(vfov, XeSys.Gfx.RenderManager.screenSize.x, XeSys.Gfx.RenderManager.screenSize.y);
        }

        /// <summary>
        /// �J�����ʒu�擾
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
        /// �J�����ʒu�ݒ�iX���W�j
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
        /// �J�����ʒu�ݒ�iY���W�j
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
        /// �J�����ʒu�ݒ�iZ���W�j
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
        /// �J�����ʒu�ݒ�iXYZ�j
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
        /// �J�����ʒu�̉��Z
        /// ���C���J����
        /// </summary>
        /// <param name="_pos"></param>
        public void AddPosition_MainCamera(Vector3 _pos)
        {
            this.mainCamera.transform.position += _pos;
        }
        /// <summary>
        /// �J�����ʒu�̉��Z
        /// �w�i�J����
        /// </summary>
        /// <param name="_pos"></param>
        public void AddPosition_BackgroundCamera(Vector3 _pos)
        {
            this.bottomCamera.transform.position += _pos;
        }
        /// <summary>
        /// �J�����ʒu�̉��Z
        /// �J����ID�w��
        /// </summary>
        /// <param name="cameraID"></param>
        /// <param name="_pos"></param>
        public void AddPosition(CameraID cameraID, Vector3 _pos)
        {
            this.GetCamera(cameraID).transform.position += _pos;
        }

        /// <summary>
        /// �����_��ݒ�
        /// </summary>
        /// <param name="_target"></param>
        public void SetLootAt(Vector3 _target)
        {
            this.mainCamera.transform.LookAt(_target);
        }
        /// <summary>
        /// �����_��ݒ�
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
        /// �ʒu�X�V�����Ȃ��X�V�֐�
        /// SetUpdate( false )�Őݒ�
        /// </summary>
        private void UpdatePositionIdle()
        {
        }
        /// <summary>
        /// �ʒu�X�V���s���X�V�֐�
        /// SetUpdate( true )�Őݒ�
        /// </summary>
        private void UpdatePositionMove()
        {
            Vector3 diff = mTargetPosition - this.mainCamera.transform.position;
            // �\���ɋ߂Â�����f�B���C����OFF
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
            // �\���ɋ߂Â�����f�B���C����OFF
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
            // �\���ɋ߂Â�����f�B���C����OFF
            if (0.00001f < diff.sqrMagnitude)
            {
                diff *= mUIModelCameraVanshingPointDelayCoef;
            }

            SetVanishingPoint(CameraID.UIModel, current + diff);
        }

        /// <summary>
        /// �J�������^�[�Q�b�g�Ɍ������Ĉړ������ǂ���
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
        /// �J�����̈ʒu�X�V��Ԃ̐ݒ�
        /// </summary>
        /// <param name="update">true��mTargetPosition�֌������ʒu���Z�������s��</param>
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
        /// �J�����X�V�^�C�v�t���O�̎擾
        /// </summary>
        /// <returns></returns>
        public bool GetCameraUpdateFlag()
        {
            return mIsCameraUpdateFlag;
        }
        /// <summary>
        /// �J�����ʒu�̍X�V�֐�
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
        /// �X�V�i���t���[���j
        /// </summary>
        void Update()
        {
            mUpdater();

            //-------------------------------
            // �X�N���[���̒��S���W(���[���h��)
            //-------------------------------
            int center_w = Screen.width / 2;
            int center_h = Screen.height / 2;
            this.screenCenterPosition = new Vector3(center_w, center_h, -this.mainCamera.transform.localPosition.z);
            /////            this.screenCenterPosition *= CameraController.Instance.resolutionRenderer.scale;
            this.screenCenterPosition *= CameraController.Instance.resolutionRenderer.settingScale;
            this.screenCenterWorldPosition = this.mainCamera.ScreenToWorldPoint(this.screenCenterPosition);


            //-------------------------------
            // �r���[�|�[�g�̍��W
            //-------------------------------
            mMainCameraForwardPosition = this.mainCamera.transform.forward * (-this.mainCamera.transform.localPosition.z);
            Update_ViewportPosition(mMainCameraForwardPosition.z);

            //-------------------------------
            // ����̍����ƕ�
            //-------------------------------
            // ����̍����ƕ��B���ݖ��g�p����������g�����Ƃ����肻���Ȃ̂ŎZ�o���Ă����B
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
        /// FlatFx�J�����̈ʒu�����C���J�����̈ʒu�ɓ��������܂��B
        /// ���I�X�V����郁�C���J�����p�I�u�W�F�N�g�p�̐��ˉe�J�����ł��邽�߁B
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
        /// �r���[�|�[�g���ɓ����Ă��邩�ǂ����̃`�F�b�N
        /// </summary>
        /// <param name="_worldPos"></param>
        /// <returns></returns>
        public bool IsInViewport(Vector3 _worldPos)
        {
            bool isIn = true;
            viewportOverVector = Vector3.zero;
            // ��ʍ��I�[�o�[
            if (viewportLeftTop.x > _worldPos.x)
            {
                viewportOverVector.x = _worldPos.x - viewportLeftTop.x;
                isIn = false;
            }
            // ��ʉE�I�[�o�[
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
        /// ���E�����ix���j�̂݉�ʓ��`�F�b�N
        /// </summary>
        /// <param name="_worldPos"></param>
        /// <returns></returns>
        public bool IsInViewportX(Vector3 _worldPos)
        {
            bool isIn = true;
            viewportOverVector = Vector3.zero;
            // ��ʍ��I�[�o�[
            if (viewportLeftTop.x > _worldPos.x)
            {
                viewportOverVector.x = _worldPos.x - viewportLeftTop.x;
                isIn = false;
            }
            // ��ʉE�I�[�o�[
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
        /// �r���[�|�[�g���W�����[���h���W�֕ϊ�
        /// </summary>
        /// <param name="z"></param>
        private void Update_ViewportPosition(float z)
        {
            viewportRightTop = this.mainCamera.ViewportToWorldPoint(new Vector3(1, 1, z));
            viewportRightDown = this.mainCamera.ViewportToWorldPoint(new Vector3(1, 0, z));
            viewportLeftTop = this.mainCamera.ViewportToWorldPoint(new Vector3(0, 1, z));
            viewportLeftDown = this.mainCamera.ViewportToWorldPoint(new Vector3(0, 0, z));

            // �r���[�|�[�g�̃��[���h�ϊ���ɃZ�[�t�G���A�̃��[���h���W�ϊ��������ɍs��
            Update_SafePosition();

        }
        /// <summary>
        /// �L�����\���p�̃Z�[�t�G���A���W�����[���h���W�֕ϊ�
        /// Update_ViewportPosition()
        /// </summary>
        /// <param name="z"></param>
        private void Update_SafePosition()
        {
            // �������Œ�
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
        /// �Z�[�t�G���A���`�F�b�N
        /// </summary>
        /// <param name="posi"></param>
        /// <returns>�Z�[�t�G���A���Ȃ�true</returns>
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
        /// �w�胏�[���hz�ɂ����鍶�ハ�[���h���W�����߂�
        /// 0�w��Ȃ�viewportLeftTop�Ɠ���
        /// </summary>
        /// <param name="_worldZ">���[���hz</param>
        /// <returns>�r���[�|�[�g����̃��[���h���W</returns>
        public Vector3 GetViewportLeftTop(float _worldZ)
        {
            float z = _worldZ - this.mainCamera.transform.localPosition.z;
            Vector3 pos = CameraController.Instance.mainCamera.ViewportToWorldPoint(new Vector3(0, 1, z));
            return pos;
        }
        /// <summary>
        /// �w�胏�[���hz�ɂ�����E�����[���h���W�����߂�
        /// 0�w��Ȃ�viewportRightDown�Ɠ���
        /// </summary>
        /// <param name="_worldZ">���[���hz</param>
        /// <returns>�r���[�|�[�g�E���̃��[���h���W</returns>
        public Vector3 GetViewportRightDown(float _worldZ)
        {
            float z = _worldZ - this.mainCamera.transform.localPosition.z;
            Vector3 pos = CameraController.Instance.mainCamera.ViewportToWorldPoint(new Vector3(1, 0, z));
            return pos;
        }


        /// <summary>
        /// ���[���h���W���X�N���[�����W�֕ϊ������l�𓾂�
        /// z������0�ɂ����l��Ԃ�
        /// </summary>
        /// <param name="worldPos">���[���h���W</param>
        /// <remarks>
        /// �`���ƂȂ�RenderTexture�̉𑜓x�̃X�P�[���������{����܂�
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
        /// ���[���h���W��UI�X�N���[�����W�֕ϊ������l�𓾂�
        /// z������0�ɂ����l��Ԃ�
        /// </summary>
        /// <param name="worldPos">���[���h���W</param>
        /// <remarks>
        /// �`���ƂȂ�RenderTexture�̉𑜓x�̃X�P�[���������{����܂�
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
        /// �X�N���[�����W�����[���h���W�ɕϊ������l�𓾂�
        /// </summary>
        /// <param name="scrPosi">�X�N���[�����W</param>
        /// <remarks>
        /// �`���ƂȂ�RenderTexture�̉𑜓x�̃X�P�[���������{����܂�
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
        /// ���[���h���W���J�����̃r���[�|�[�g���W�֕ϊ������l�𓾂�
        /// </summary>
        /// <param name="worldPos">�r���[�|�[�g���W(0-1)</param>
        /// <returns></returns>
        public Vector3 GetWorldToViewportPoint(Vector3 worldPos)
        {
            Vector3 viewPosi = CameraController.Instance.mainCamera.WorldToViewportPoint(worldPos);
            viewPosi *= this.resolutionRenderer.settingScale;
            viewPosi.z = 0;
            return viewPosi;
        }

        /// <summary>
        /// ���[���h���W��UI�J�������W�ɕϊ������l�𓾂�
        /// z��UI�J�����̐ݒ肳�ꂽ�L�����o�X�̋���
        /// </summary>
        /// <param name="worldPos">���[���h���W</param>
        /// <remarks>
        /// �`���ƂȂ�RenderTexture�̉𑜓x�̃X�P�[���������{����܂�
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
        /// �L�����p�X��̃��[���h���W�����C���J�����̂�[����W�ɕϊ������l�𓾂�
        /// GetScreenToUICameraPosition()�Ŏ擾�����L�����p�X��̃��[���h���W��������ɓn���Ă��������B
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
        /// �L�����o�X��̃X�N���[�����W�����[���h���W�ɕϊ������l�𓾂�
        /// </summary>
        /// <param name="scrPosi">�X�N���[�����W</param>
        /// <remarks>
        /// �`���ƂȂ�RenderTexture�̉𑜓x�̃X�P�[���������{����܂�
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
        /// �X�N���[�����W��UI�J�������W�ɕϊ������l�𓾂�
        /// z��UI�J�����̐ݒ肳�ꂽ�L�����o�X�̋���
        /// </summary>
        /// <param name="scrPos">�X�N���[�����W</param>
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
        /// ����J�����̏����_���W�ݒ�
        /// </summary>
        /// <param name="cameraId">�ݒ�ΏۃJ����ID</param>
        /// <param name="center">�����_�̍��W
        /// �}0.5�ŉ�ʒ[���J�����̎��������ƂȂ�܂�
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
        /// �w��J�����̏����_���W�擾
        /// </summary>
        /// <param name="cameraId">�Ώۂ̃J����ID</param>
        /// <returns></returns>
        public Vector2 GetVanishingPoint(CameraID cameraId)
        {
            return mVanishingPointList[(int)cameraId];
        }
        /// <summary>
        /// �S�J�����̏����_�l�̃��Z�b�g
        /// </summary>
        private void ResetAllVanishingPointValue()
        {
            for (int i = 0; i < mVanishingPointList.Count; ++i)
            {
                mVanishingPointList[i] = Vector2.zero;
            }
        }

        /// <summary>
        /// ��ʂ̐F���]���N�G�X�g
        /// </summary>
        /// <param name="inverse">true�̎����]������</param>
        /// <param name="time">�ω�����</param>
        public void InverseColor(bool inverse, float time = 0.0f)
        {
            m_pfxInverseColor.Request(inverse, time);
        }

        /// <summary>
        /// �u���[ON���N�G�X�g
        /// </summary>
        /// <param name="time">��������</param>
        /// <param name="factor">�u���[���x
        /// 0.0��OFF�A�l���傫�����{�P�������Ȃ�܂����傫������Ɗȗ��������u���[�����̓s����
        /// �G�b�W���ڗ��悤�ɂȂ���҂���{�P���ɂȂ�܂���B
        /// ���e�����l�͂��悻7.0�ʂ܂łł��B
        /// </param>
        public static void BlurOn(float time = 0.5f, float factor = 1.5f)
        {
            mInstance.m_pfxBlur.Request(time, factor);
        }
        /// <summary>
        /// �u���[OFF���N�G�X�g
        /// </summary>
        /// <param name="time">��������</param>
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
        /// �I�[�o�[���C�̐F�ω�������������
        /// </summary>
        /// <returns>true�̎��A������</returns>
        public static bool IsChangingOverlayColor()
        {
            return mInstance.m_fxOverlay.IsChangeColorRunning();
        }

        /// <summary>
        /// �_���[�W�p�I�[�o�[���C�G�t�F�N�g�̃J���[�ݒ�
        /// </summary>
        /// <param name="color"></param>
        public static void SetOverlayDamageColor(Color color)
        {
            mInstance.m_fxOverlayDamage.SetColor(color);
        }
        /// <summary>
        /// �_���[�W�p�I�[�o�[���C�G�t�F�N�g�J���[�ύX
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
        /// �J����ID
        /// m_CameraTable�ɓo�^����Ă���J������
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

            // Y��
            if (axis == 0)
            {
                return m_OwnerCtrl.m_RotateAxisValue_Y * m_RotatePowerY;
            }
            // X��
            if (axis == 1)
            {
                return m_OwnerCtrl.m_RotateAxisValue_X * m_RotatePowerX;
            }

            return 0;
        }

        // FreeCmaera��p�X�V����
        public void FreeCameraUpdate()
        {
            if (m_OwnerCtrl.m_FreeCameraCharacter == null) return;

            var actions = m_OwnerCtrl.m_FreeCameraCharacter.InputProvider.GetInputActions(true);

            // �ʏ탂�[�h��
            if (actions != null &&
                actions.Count >= 1)
            {
                if (actions.Contains(GameInputActionTypes.Menu))
                {
                    m_OwnerCtrl.SetFreeCameraMode(false, null);
                    return;
                }
            }

            // �J��������
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

            // ��]
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

            // �L�[�{�[�h��p
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

            // ���Ԑi�߂�
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
    // �S�J�����f�[�^
    [SerializeField] CameraData[] _playerVirtualCams;

    // NormalCamera�݂̂̃��X�g
    List<CameraData> _normalOnlyCams;

    // 
    [Header("��Normal Camera�݂̂�Index")]
    [SerializeField] int _nowNormalCameraIndex = 2;
    //    public int NowNormalCameraIndex => _nowNormalCameraIndex;

    // NormaCamera��p�̌��݂̃J����
    public CameraData CurrentNormalCamData => _normalOnlyCams[_nowNormalCameraIndex];

    // �S�ẴJ�����Ώۂ̌��݂̃J����
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

        // �����ݒ�
        // Normal Camera�̂݃��X�g�̍쐬
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
        // �����ݒ�
        for (int i = 0; i < _playerVirtualCams.Length; i++)
        {
            _playerVirtualCams[i].SetInputAxis();
        }

        // �J�����ύX��
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

        // �X�V����
        this.UpdateAsObservable()
            .Where(_ => enabled)
            .Subscribe(_ =>
            {
                for (int i = 0; i < _playerVirtualCams.Length; i++)
                {
                    _playerVirtualCams[i].SetInputAxis();
                }

                // �p�x����
                {
                    var nowVCamData = _normalOnlyCams[_nowNormalCameraIndex];
                    float angleY = nowVCamData.GetAngleY();

                    foreach (var vcamData in _normalOnlyCams)
                    {
                        if (vcamData.Equals(nowVCamData)) continue;

                        vcamData.SetAngleY(angleY);
                    }
                }

                // ���b�N��������
                if (_lockOnTarget != null &&
                    _lockOnTarget.CharaCore != null &&
                    _lockOnTarget.ObjGroupID.SpStatus_IsDead)
                {
                    _lockOnTarget = null;
                }

                // �t���[�J����
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
            // ���݃t���[�J�������[�h
            if (CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera) return;

            // FreeCamera���������A�Z�b�g
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

                    // ����ꂽ��
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

            // Normal Camera���Z�b�g
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

        // Cinemachine Blend�̎��Ԃ�ݒ�
        Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
        brain.DefaultBlend.Time = blendDuration;

        // ���݂̃J�����f�[�^���L��
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

        // �Z�b�g
        _normalOnlyCams[_nowNormalCameraIndex].ResetRotateAxisToForward(duration);
    }

    public void SetRotateAxisByDir(Vector3 dir)
    {
        var forward = dir;
        forward.y = 0;
        forward.Normalize();
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        // �Z�b�g
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

    // �qGameObject��VirtualCamera�ɁAtarget��ݒ�
    public void SetCameraTarget(Transform target)
    {
        foreach (var vcamData in _playerVirtualCams)
        {
            vcamData.SetTarget(target);
        }
    }
}
*/