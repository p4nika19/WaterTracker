using Microsoft.Maui.Controls;

namespace WaterTrackerApp1;

public partial class MainPage : ContentPage
{
    private double _targetWater = 2000; // Цель по умолчанию
    private double _currentWater = 0;   // Текущий объем воды
    private double _maxDropHeight = 300; // Максимальная высота капли

    public MainPage()
    {
        InitializeComponent();
        UpdateProgressLabel();
    }

    //Установка целевого объема воды
    private void SetTarget_Clicked(object sender, EventArgs e)
    {
        if (double.TryParse(TargetEntry.Text, out double target) && target > 0)
        {
            _targetWater = target;
            _currentWater = 0; //Сбрасываем текущий прогресс
            UpdateWaterDrop();
            UpdateProgressLabel();
        }
        else
        {
            DisplayAlert("Ошибка", "Введите корректное значение цели (положительное число).", "OK");
        }
    }

    //Добавление выпитой воды
    private async void AddWater_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && double.TryParse(button.CommandParameter?.ToString(), out double amount))
        {
            _currentWater += amount;
            if (_currentWater > _targetWater)
                _currentWater = _targetWater; //Не превышаем цель

            await AnimateWaterDrop();
            UpdateProgressLabel();
        }
    }

    //Обновление текстового прогресса
    private void UpdateProgressLabel()
    {
        ProgressLabel.Text = $"{_currentWater}/{_targetWater} мл";
    }

    //Обновление визуального индикатора капли
    private void UpdateWaterDrop()
    {
        double fillPercentage = _currentWater / _targetWater;
        double newHeight = _maxDropHeight * fillPercentage;
        WaterLevelFrame.HeightRequest = newHeight;
    }

    //Анимация заполнения капли
    private async Task AnimateWaterDrop()
    {
        double fillPercentage = _currentWater / _targetWater;
        double targetHeight = _maxDropHeight * fillPercentage;

        await WaterLevelFrame.AnimateAsync("WaterFill", new Animation(v =>
        {
            WaterLevelFrame.HeightRequest = v;
        }, WaterLevelFrame.HeightRequest, targetHeight), length: 500, easing: Easing.SinInOut);
    }
}

// Расширение для анимации
public static class ViewExtensions
{
    public static Task<bool> AnimateAsync(this VisualElement view, string name, Animation animation, uint length = 250, Easing easing = null)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        view.Animate(name, animation, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
        return taskCompletionSource.Task;
    }
}
