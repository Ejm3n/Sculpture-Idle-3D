using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChooseARewardPanel : MonoBehaviour
{
    public Button[] buttons; // Ссылки на кнопки
    public CanvasGroup panelCanvasGroup; // Панель для скрытия
    public float defaultSize = 260f; // Размер кнопок по умолчанию
    public float targetSize = 800f; // Конечный размер для выбранной кнопки
    public float animationDuration = 1f; // Длительность анимации
    public GameObject backgroundPanel; // Фон, который нужно включить

    private bool isAnimating = false; // Флаг для предотвращения повторного запуска

    void OnEnable()
    {
        // Добавляем "дыхание" при включении
        foreach (var btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.localScale = Vector3.one * defaultSize / targetSize;
            rect.DOScale((defaultSize + 20) / targetSize, 0.8f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            // Привязываем действия к кнопкам
            btn.onClick.RemoveAllListeners(); // Убираем старые слушатели, если есть
            btn.onClick.AddListener(() => OnButtonClicked(rect));
        }
    }

    public void OnButtonClicked(RectTransform clickedRect)
    {
        if (isAnimating) return; // Предотвращаем повторную анимацию
        isAnimating = true;

        // Отключаем "дыхание" для всех кнопок
        foreach (var btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.DOKill();
        }

        // Анимация для нажатой кнопки
        clickedRect.DOScale(targetSize / defaultSize, animationDuration).SetEase(Ease.OutQuad);
        clickedRect.DOLocalMove(Vector3.zero, animationDuration).SetEase(Ease.OutQuad);

        // Анимация для остальных кнопок
        foreach (var btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            if (rect != clickedRect)
            {
                var canvasGroup = rect.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = rect.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.DOFade(0, animationDuration).SetEase(Ease.OutQuad);
                rect.DOScale(targetSize / defaultSize, animationDuration).SetEase(Ease.OutQuad);
            }
        }

        // Скрытие панели и анимация фона
        DOVirtual.DelayedCall(animationDuration, () =>
        {
            // Скрываем текущую панель
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
                {
                    panelCanvasGroup.gameObject.SetActive(false);

                    // Включаем фон и анимируем его появление
                    if (backgroundPanel != null)
                    {
                        RectTransform bgRect = backgroundPanel.GetComponent<RectTransform>();
                        bgRect.localScale = Vector3.zero; // Начинаем с нуля
                        backgroundPanel.GetComponent<CanvasGroup>().alpha = 1; // Включаем объект

                        bgRect.DOScale(1, 0.8f).SetEase(Ease.OutBack); // Анимация до полного размера
                    }
                });
            }
        });
    }
}
