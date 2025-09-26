using UnityEngine;
using UnityEngine.EventSystems;

namespace Start
{
    public class UIJoystick : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("摇杆组件")]
        [Tooltip("摇杆背景图")]
        public RectTransform Background;
        
        [Tooltip("摇杆中心点")]
        public RectTransform Handle;
        
        [Tooltip("摇杆背景图是否为圆形")]
        public bool BackgroundIsCircle;

        /// <summary>
        /// 摇杆是否激活
        /// </summary>
        public bool IsActive { get; private set;}
        
        /// <summary>
        /// 摇杆方向向量
        /// </summary>
        public Vector2 Direction { get; private set;}

        private RectTransform _uiRectTransform;
    
        /// <summary>
        /// 摇杆背景图的初始本地位置
        /// </summary>
        private Vector2 _backgroundInitialLocalPos;
    
        /// <summary>
        /// 摇杆中心点的初始本地位置
        /// </summary>
        private Vector2 _handleInitialLocalPos;

        /// <summary>
        /// 从空白点击区域计算的背景图最大移动范围
        /// </summary>
        private float _backgroundMoveRange;
        
        /// <summary>
        /// 从摇杆背景图计算的中心点最大移动范围
        /// </summary>
        private float _handleMoveRange;

        /// <summary>
        /// 背景图限制的矩形
        /// </summary>
        private Rect _backgroundRestrictedRect;
        
        private void Awake()
        {
            if (Background == null || Handle == null)
            {
                IsActive = false;
                return;
            }
            IsActive = true;
            _uiRectTransform = GetComponent<RectTransform>();
            _handleInitialLocalPos = Handle.localPosition;
            _backgroundInitialLocalPos = Background.localPosition;
            float backgroundHalfWidth = Background.rect.width / 2f;
            float backgroundHalfHeight = Background.rect.height / 2f;
            if (BackgroundIsCircle)
            {
                float clickAreaHalfWidth = _uiRectTransform.rect.width / 2f;
                float clickAreaHalfHeight = _uiRectTransform.rect.height / 2f;
                // 取宽高中较小的值作为移动范围，确保背景图不会超出点击区域
                _backgroundMoveRange = Mathf.Min(clickAreaHalfWidth - backgroundHalfWidth, clickAreaHalfHeight - backgroundHalfHeight);
            }
            else
            {
                _backgroundRestrictedRect = ClampPositionToRectangle();
            }
            
            float handleHalfWidth = Handle.rect.width / 2f;
            float handleHalfHeight = Handle.rect.height / 2f;
            // 取宽高中较小的值作为移动范围，确保中心点不会超出背景图
            _handleMoveRange = Mathf.Min(backgroundHalfWidth - handleHalfWidth, backgroundHalfHeight - handleHalfHeight);
            SetJoystickActive(false);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsActive)
            {
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_uiRectTransform, eventData.position,
                eventData.enterEventCamera, out var uiPosition);
            // 限制摇杆背景图的位置，使其不会超出点击区域
            var backgroundPosition = BackgroundIsCircle ? ClampPositionToCircle(uiPosition, _backgroundInitialLocalPos, _backgroundMoveRange) : ClampPositionToRect(uiPosition, _backgroundRestrictedRect);
            Background.localPosition = backgroundPosition;
            Handle.localPosition = backgroundPosition + (_handleInitialLocalPos - _backgroundInitialLocalPos);
            SetJoystickActive(true);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsActive)
            {
                return;
            }
            Background.localPosition = _backgroundInitialLocalPos;
            Handle.localPosition = _handleInitialLocalPos;
            SetJoystickActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsActive)
            {
                return;
            }
            // 获取摇杆背景图的位置作为中心
            Vector2 backgroundPos = Background.localPosition;
        
            // 将拖动位置转换为点击区域的本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_uiRectTransform, eventData.position,
                eventData.enterEventCamera, out var dragLocalPos);
            
            // 限制摇杆中心点的位置，使其不会超出背景图范围
            Vector2 handlePos = ClampPositionToCircle(dragLocalPos, backgroundPos, _handleMoveRange);
        
            // 设置摇杆中心点位置
            Handle.localPosition = handlePos;
            
            // 计算方向向量（归一化） 作为摇杆方向向量 帧同步使用
            Direction = (handlePos - backgroundPos).normalized;
            
            print($"方向向量:{Direction} 区域:{GetRegion(Direction, 24)}");
        }

        private Rect ClampPositionToRectangle()
        {
            // 计算点击区域的边界（本地坐标）
            float halfWidth = _uiRectTransform.rect.width / 2f;
            float halfHeight = _uiRectTransform.rect.height / 2f;
        
            Rect rect = new Rect(
                -halfWidth,  // xMin
                -halfHeight, // yMin
                _uiRectTransform.rect.width,  // width
                _uiRectTransform.rect.height  // height
            );
            float backgroundHalfWidth = Background.rect.width / 2f;
            float backgroundHalfHeight = Background.rect.height / 2f;
            
            // 调整点击区域矩形，为背景图留出空间
            Rect backgroundRestrictedRect = new Rect(
                rect.xMin + backgroundHalfWidth,
                rect.yMin + backgroundHalfHeight,
                rect.width - backgroundHalfWidth * 2,
                rect.height - backgroundHalfHeight * 2
            );
            return backgroundRestrictedRect;
        }
        
        /// <summary>
        /// 限制位置在矩形范围内
        /// </summary>
        private Vector2 ClampPositionToRect(Vector2 targetPos, Rect rect)
        {
            return new Vector2(
                Mathf.Clamp(targetPos.x, rect.xMin, rect.xMax), 
                Mathf.Clamp(targetPos.y, rect.yMin, rect.yMax)
            );
        }
        
        /// <summary>
        /// 限制位置在指定圆环内
        /// </summary>
        private Vector2 ClampPositionToCircle(Vector2 targetPos, Vector2 centerPos, float maxDistance)
        {
            Vector2 direction = targetPos - centerPos;
            if (direction.sqrMagnitude > maxDistance * maxDistance)
            {
                direction = direction.normalized * maxDistance;
                return centerPos + direction;
            }
            return targetPos;
        }
        
        private void SetJoystickActive(bool active)
        {
            Background.gameObject.SetActive(active);
            Handle.gameObject.SetActive(active);
            if (!active)
            {
                Direction = Vector2.zero;
            }
        }
        
        /// <summary>
        /// (以正向Y轴为0)计算方向向量所属的区域
        /// </summary>
        /// <param name="direction">方向向量</param>
        /// <param name="divisions">划分份数（默认4份）</param>
        /// <returns>区域索引（1到divisions），0表示无效</returns>
        public static int GetRegion(Vector2 direction, int divisions)
        {
            // 1. 参数有效性校验
            if (divisions < 2)
            {
                Debug.LogError($"划分份数必须≥2，当前值: {divisions}");
                return 0;
            }

            // 2. 处理零向量（无有效方向）
            if (direction.sqrMagnitude > 0)
            {
                // 3. 归一化向量（消除长度影响，确保角度计算准确）
                Vector2 normalizedDir = direction.normalized;
                // 4. 计算向量与Y正轴的夹角（0°~360°）
                // 原理：点积求与Y轴的夹角（0~180°），X<0时补全为180~360°
                float angleRad = Mathf.Acos(Vector2.Dot(normalizedDir, Vector2.up));
                float angle = Mathf.Rad2Deg * angleRad;

                if (normalizedDir.x < 0)
                {
                    angle = 360f - angle;
                }

                int regionIndexZeroBased = 0;
                // 5. 核心优化：用“取模运算”统一计算区域，避免首末区域漏洞
                float stepAngle = 360f / divisions; // 每份角度宽度（24份时为15°）
                float HalfDivAngle = stepAngle / 2;
                if (angle <= HalfDivAngle || angle >= 360f - HalfDivAngle)
                {
                    regionIndexZeroBased = 0;
                }
                else
                {
                    float val = (angle - HalfDivAngle) / stepAngle;
                    val += 1;
                    regionIndexZeroBased = Mathf.FloorToInt(val);
                }
                // 6. 转为1开始的区域索引
                return regionIndexZeroBased + 1;
            }
            
            Debug.LogWarning("传入向量为零向量，无法计算方向区域");
            return 0;
        }
    }
}