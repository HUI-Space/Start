using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Start
{
    public class TALLogicModule
    {
        public const string SceneName = "Assets/Asset/Scenes/TakeALeap.unity";
        public const string GroundName = "Assets/Asset/Model/Scene/TALPlane.prefab";
        public const string Stage_1Name = "Assets/Asset/Model/Scene/TALStage_1.prefab";
        public const string Stage_2Name = "Assets/Asset/Model/Scene/TALStage_2.prefab";
        public const string PlayerName = "Assets/Asset/Model/Char/TALPlayer.prefab";
        
        private AsyncOperationHandle<Scene> _sceneHandle;
        private AsyncOperationHandle<GameObject> _groundHandle;
        private AsyncOperationHandle<GameObject> _stage_1Handle;
        private AsyncOperationHandle<GameObject> _stage_2Handle;
        private AsyncOperationHandle<GameObject> _playerHandle;

        /// <summary>
        /// 相机
        /// </summary>
        private Camera _camera;
        
        /// <summary>
        /// 当前地板
        /// </summary>
        private TAlGround _ground;
        
        /// <summary>
        /// 当前玩家
        /// </summary>
        private TALPlayer _player;

        /// <summary>
        /// 
        /// </summary>
        private TALStage _stage_1;
        
        /// <summary>
        /// 
        /// </summary>
        private TALStage _stage_2;
        
        /// <summary>
        /// 默认舞台
        /// </summary>
        private TALStage _defaultStage;
        
        /// <summary>
        /// 当前player站立的舞台
        /// </summary>
        private TALStage _currentStage;
        
        /// <summary>
        /// 第一个盒子默认位置
        /// </summary>
        private readonly Vector3 _defaultStagePosition = new Vector3(0, -0.25f, 0);
        
        /// <summary>
        /// 第一个盒子默认位置
        /// </summary>
        private readonly Vector3 _defaultPlayerPosition = new Vector3(0, 0.5f, 0);

        /// <summary>
        /// 游戏是否开始
        /// </summary>
        private bool _start;
        
        /// <summary>
        /// 按钮点击开始事件
        /// </summary>
        private float _startTime;
        
        /// <summary>
        /// 方向
        /// </summary>
        private Vector3 _direction = new Vector3(1, 0, 0);
        
        /// <summary>
        /// 玩家是否可以控制
        /// </summary>
        private bool _enableInput;

        private int _generateCount;
        /// <summary>
        /// 当前分数
        /// </summary>
        public int CurrentScore { get; private set; }

        private Vector3 _cameraRelativePosition;

        private List<TALStage> _list = new List<TALStage>();
        
        
        public void Initialize()
        {
            
        }

        public void DeInitialize()
        {
            
        }
        

        public async Task Prepare()
        {
            //1.加载场景
            _sceneHandle = SceneManager.Instance.LoadSceneAsync<Scene>(SceneName,false);
            await _sceneHandle.Task;
            //2.加载地板
            _groundHandle = ResourceManager.Instance.LoadAssetAsync<GameObject>(GroundName);
            await _groundHandle.Task;
            //3.加载格子
            _stage_1Handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(Stage_1Name);
            await _stage_1Handle.Task;
            _stage_2Handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(Stage_2Name);
            await _stage_2Handle.Task;
            //4.加载玩家
            _playerHandle = ResourceManager.Instance.LoadAssetAsync<GameObject>(PlayerName);
            
            _camera = GameObject.Find("MainCamera").GetComponent<Camera>();
            _ground = Object.Instantiate(_groundHandle.Result).GetOrAddComponent<TAlGround>();
            _player = Object.Instantiate(_playerHandle.Result).GetOrAddComponent<TALPlayer>();
            _stage_1 = Object.Instantiate(_stage_1Handle.Result).GetOrAddComponent<TALStage>();
            _stage_2 = Object.Instantiate(_stage_2Handle.Result).GetOrAddComponent<TALStage>();
            _stage_1.gameObject.name = "stage_1";
            _stage_2.gameObject.name = "stage_2";
            _stage_1.gameObject.SetActive(false);
            _stage_2.gameObject.SetActive(false);
            
            _defaultStage = GameObject.Instantiate(_stage_1);
            _defaultStage.gameObject.SetActive(true);
            _defaultStage.name = "defaultStage";
        }
        
        #region 逻辑
        
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            CurrentScore = 0;
            _generateCount = 0;
            _currentStage = _defaultStage;
            _currentStage.transform.position = _defaultStagePosition;
            _player.transform.position = _defaultPlayerPosition;
            _camera.transform.rotation = Quaternion.Euler(60, 35, 0);
            _camera.transform.position = new Vector3(-1, 4.5f, -1.5f);
            _cameraRelativePosition = _camera.transform.position - _player.transform.position;
            GenerateStage();
            _start = true;
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            _player.RestartGame();
            _start = false;
            foreach (var stage in _list)
            {
                GameObject.Destroy(stage.gameObject);
            }
            _list.Clear();
            
            StartGame();
        }

        /// <summary>
        /// 复活继续游戏
        /// </summary>
        public void ResurgenceGame()
        {
            Vector3 currentStagePosition = _currentStage.transform.position;
            Vector3 resurgencePosition = new Vector3(currentStagePosition.x, 0.5f, currentStagePosition.z);
            _player.transform.position =  resurgencePosition;
        }

        public void ConfirmedResurgence()
        {
            // TODO: 确认复活 看广告
            ResurgenceGame();
        }
        
        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameOver()
        {
            _start = false;
        }

        public void Update()
        {
            if (!_start)
            {
                return;
            }
            if (_enableInput)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnMouseButtonDown();
                }
                if (Input.GetMouseButton(0))
                {
                    OnMouseButton();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    OnMouseButtonUp();
                }
            }
        }
        
        public void OnMouseButtonDown()
        {
            _startTime = Time.time;
            _player.ShowParticle();
        }

        public void OnMouseButton()
        {
            if (_currentStage.transform.localScale.y > 0.3)
            {
                _player.PlayAnimation();
                _currentStage.PlayAnimation();
            }
        }
        
        public void OnMouseButtonUp()
        {
            float elapse = Time.time - _startTime;
            _player.OnJump(elapse, TALController.Instance.SettingModule.Factor, _direction);
            _player.ResetAnimation();
            _currentStage.ResetAnimation();
            _enableInput = false;
        }

        public void OnCollisionEnter(Collision other)
        {
            if (!_start)
            {
                return;
            }
            if (other.gameObject.TryGetComponent(out TAlGround ground))
            {
                UIActions.TALGamePanel_ShowResurgenceButton();
            }
            if (other.gameObject.TryGetComponent(out TALStage stage))
            {
                var contacts = other.contacts;
                if (_currentStage != stage)
                {
                    //检查球员的脚是否在舞台上
                    if (contacts.Length == 1 && contacts[0].normal == Vector3.up)
                    {
                        var lastStage = _currentStage;
                        _currentStage = stage;
                        //加分
                        CurrentScore++;
                        //显示加分
                        UIActions.TALGamePanel_GetScore();
                        // 随机方向
                        RandomDirection();
                        //生成格子
                        GenerateStage();
                        //移动相机
                        MoveCamera();
                        //回收格子
                        //RecycleStage(lastStage);
                        _enableInput = true;
                    }
                    else
                    {
                        //  body与box碰撞
                        UIActions.TALGamePanel_ShowResurgenceButton();
                    }
                }
                else
                {
                    //check if player's feet on the stage
                    if (contacts.Length == 1 && contacts[0].normal == Vector3.up)
                    {
                        _enableInput = true;
                    }
                    else // body just collides with this box
                    {
                        UIActions.TALGamePanel_ShowResurgenceButton();
                    }
                }
            }
            
        }
        
        public void OnCollisionExit(Collision other)
        {
            if (!_start)
            {
                return;
            }
            _enableInput = false;
        }
        
        void RandomDirection()
        {
            var seed = Random.Range(0, 2);
            _direction = seed == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
            _player.transform.right = _direction;
        }

        void MoveCamera()
        {
            _camera.transform.DOMove(_player.transform.position + _cameraRelativePosition, 1);
        }
        #endregion
        
        
        #region 格子相关
        
        /// <summary>
        /// 生成格子
        /// </summary>
        private void GenerateStage()
        {
            int randomStage = Random.Range(0, 2);
            TALStage stage;
            if (randomStage == 0)
            {
                stage = GameObject.Instantiate(_stage_1);
            }
            else
            {
                stage = GameObject.Instantiate(_stage_2);
            }
            stage.gameObject.SetActive(true);
            stage.transform.position = _currentStage.transform.position + _direction * Random.Range(1.1f, TALController.Instance.SettingModule.MaxDistance);
            
            var randomScale = Random.Range(0.5f, 1);
            stage.transform.localScale = new Vector3(randomScale, 0.5f, randomScale);
            stage.GetComponent<Renderer>().material.color =
                new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1));
            _list.Add(stage);
            stage.name = $"TALStage_{_generateCount}";
            _generateCount++;
        }

        /// <summary>
        /// 回收格子
        /// </summary>
        private void RecycleStage(TALStage stage)
        {
            
        }

        #endregion
    }
}