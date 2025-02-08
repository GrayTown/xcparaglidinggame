using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Animator animator;
    private bool isStarted = false; // Флаг, чтобы не запускать анимацию дважды

    public void SetLifetime(float lifetime)
    {
        if (!isStarted) // Запускаем анимацию только один раз
        {
            animator = GetComponent<Animator>();

            if (animator != null)
            {
                AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];

                // Настраиваем скорость анимации
                animator.speed = clip.length / lifetime;

                // Запускаем анимацию вручную
                animator.Play(clip.name, 0, 0f);
            }

            isStarted = true;
        }
    }
}


