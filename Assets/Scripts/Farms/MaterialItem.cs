using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Farms
{
    public class MaterialItem : MonoBehaviour
    {
        public void MoveTo(Transform target, float duration, float height, System.Action onComplete)
        {
            if (target == null) return;
            
            // Xóa tween cũ nếu có
            transform.DOKill();

            // Hiệu ứng nhảy (arc) bằng DOJump
            // DOJump sẽ tự động xử lý cả di chuyển ngang và nhảy dọc theo hình cung
            transform.DOJump(target.position, height, 1, duration)
                .SetEase(Ease.Linear)
                .OnUpdate(() => {
                    // Nếu target di chuyển, chúng ta có thể cần cập nhật đích đến
                    // Tuy nhiên DOJump không hỗ trợ cập nhật đích động một cách dễ dàng giữa chừng
                })
                .OnComplete(() => {
                    if (target != null)
                    {
                        transform.position = target.position;
                        transform.SetParent(target);
                    }
                    onComplete?.Invoke();
                });
        }
    }
}
