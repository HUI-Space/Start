using DG.Tweening;
using UnityEngine;

namespace Start
{
    public class TALPlayer : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        // 小人头部
        private Transform _head;
        // 小人身体
        private Transform _body;
        
        private GameObject _particle;
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.centerOfMass = new Vector3(0, 0, 0);
            _body = transform.Find("Body");
            _head = transform.Find("Head");
            _particle = transform.Find("Particle System").gameObject;
        }
        

        private void OnCollisionEnter(Collision other)
        {
            TALController.Instance.LogicModule.OnCollisionEnter(other);
        }
        
        private void OnCollisionExit(Collision other)
        {
            TALController.Instance.LogicModule.OnCollisionExit(other);
        }

        public void RestartGame()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            ResetAnimation();
        }
        
        public void ShowParticle()
        {
            _particle.SetActive(true);
        }
        
        public void PlayAnimation()
        {
            _body.transform.localScale += new Vector3(1, -1, 1) * 0.05f * Time.deltaTime;
            _head.transform.localPosition += new Vector3(0, -1, 0) * 0.1f * Time.deltaTime;
        }

        public void ResetAnimation()
        {
            //还原小人的形状
            _body.transform.DOScale(0.1f, 0.2f);
            _head.transform.DOLocalMoveY(0.29f, 0.2f);
        }

        public void OnJump(float elapse,float factor,Vector3 direction)
        {
            _rigidbody.AddForce(new Vector3(0, 5f, 0) + direction * elapse * factor, ForceMode.Impulse);
            transform.DOLocalRotate(new Vector3(0, 0, -360), 0.6f, RotateMode.LocalAxisAdd);
            _particle.SetActive(false);
        }
    }
}