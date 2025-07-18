using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NUnit.Framework;
using System;
using System.Reflection;

namespace AntiBaldaGame.Tests.ViewModels
{
    [TestFixture]
    public class GameWindowViewModelTests
    {
        private GameWindowViewModel _viewModel;
        private PropertyInfo _modeProperty;
        private FieldInfo _previousLetterField;

        [SetUp]
        public void Setup()
        {
            _viewModel = new GameWindowViewModel();

            // Получаем PropertyInfo для свойства Mode
            _modeProperty = typeof(GameWindowViewModel).GetProperty("Mode");

            // Получаем FieldInfo для приватного поля previousLetterOnChosenButton
            _previousLetterField = typeof(GameWindowViewModel)
                .GetField("previousLetterOnChosenButton", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_modeProperty == null || _previousLetterField == null)
            {
                throw new InvalidOperationException("Не удалось получить доступ к необходимым членам класса");
            }
        }

        private void SetMode(GameWindowViewModel.GameMode mode)
        {
            _modeProperty.SetValue(_viewModel, mode);
        }

        private char GetPreviousLetter()
        {
            return (char)(_previousLetterField.GetValue(_viewModel) ?? ' ');
        }

        // ===================== OnGridButtonClick Tests =====================

        [Test]
        [Description("В режиме LetterChoosing с выбранной кнопкой сохраняет предыдущую букву")]
        public void OnGridButtonClick_InLetterChoosingMode_WithChosenButton_SavesPreviousLetter()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            var button = _viewModel.Grid.Get(0, 0);
            button.Letter = 'А';
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());

            // Act
            _viewModel.OnGridButtonClick(button, new RoutedEventArgs());

            // Assert
            Assert.That(GetPreviousLetter(), Is.EqualTo('А'));
        }

        [Test]
        [Description("В режиме LetterChoosing с непустой кнопкой отключает ввод")]
        public void OnGridButtonClick_InLetterChoosingMode_WithNonEmptyButton_DisablesInput()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            var button = new Button { Content = 'А' };

            // Act
            _viewModel.OnGridButtonClick(button, new RoutedEventArgs());

            // Assert
            Assert.That(_viewModel.InputVisible, Is.False);
        }

        [Test]
        [Description("В режиме LetterChoosing с пустой кнопкой включает ввод")]
        public void OnGridButtonClick_InLetterChoosingMode_WithEmptyButton_EnablesInput()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            var button = new Button { Content = ' ' };

            // Act
            _viewModel.OnGridButtonClick(button, new RoutedEventArgs());

            // Assert
            Assert.That(_viewModel.InputVisible, Is.True);
            Assert.That(_viewModel.InputFieldText, Is.EqualTo(" "));
        }

        // ===================== OnTextChange Tests =====================

        [Test]
        [Description("Обновляет текст в выбранной кнопке")]
        public void OnTextChange_WithChosenButton_UpdatesButtonLetter()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.InputFieldText = "Тест";

            // Act
            _viewModel.OnTextChange(null, new TestTextChangedEventArgs());

            // Assert
            Assert.That(_viewModel.ChosenButton?.Letter, Is.EqualTo('т'));
        }

        [Test]
        [Description("Сохраняет предыдущую букву при первом изменении")]
        public void OnTextChange_FirstChange_SavesPreviousLetter()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.ChosenButton!.Letter = 'А';
            _viewModel.InputFieldText = "Б";

            // Act
            _viewModel.OnTextChange(null, new TestTextChangedEventArgs());

            // Assert
            Assert.That(GetPreviousLetter(), Is.EqualTo('А'));
        }

        [Test]
        [Description("Фильтрует ввод, оставляя только русские буквы")]
        public void OnTextChange_FiltersInput_KeepsOnlyRussianLetters()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.InputFieldText = "Hello123!";

            // Act
            _viewModel.OnTextChange(null, new TestTextChangedEventArgs());

            // Assert
            Assert.That(_viewModel.InputFieldText, Is.EqualTo(" "));
        }

        // ===================== OnKeyDown Tests =====================

        [Test]
        [Description("Нажатие Enter в режиме LetterChoosing создает последовательность")]
        public void OnKeyDown_EnterInLetterChoosing_CreatesLetterSequence()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.ChosenButton!.Letter = 'А';

            // Act
            _viewModel.OnKeyDown(null, new KeyEventArgs { Key = Key.Enter });

            // Assert
            Assert.That(_viewModel.LetterSequence, Is.Not.Null);
            Assert.That((GameWindowViewModel.GameMode)_modeProperty.GetValue(_viewModel),
                Is.EqualTo(GameWindowViewModel.GameMode.LetterCombining));
        }

        [Test]
        [Description("Нажатие Enter в режиме LetterCombining ничего не делает")]
        public void OnKeyDown_EnterInLetterCombining_DoesNothing()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterCombining);
            var initialSequence = _viewModel.LetterSequence;

            // Act
            _viewModel.OnKeyDown(null, new KeyEventArgs { Key = Key.Enter });

            // Assert
            Assert.That(_viewModel.LetterSequence, Is.EqualTo(initialSequence));
        }

        [Test]
        [Description("Нажатие не Enter не вызывает изменений")]
        public void OnKeyDown_NonEnterKey_DoesNothing()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterChoosing);

            // Act
            _viewModel.OnKeyDown(null, new KeyEventArgs { Key = Key.A });

            // Assert
            Assert.That(_viewModel.LetterSequence, Is.Null);
        }

        // ===================== OnApplyButtonClick Tests =====================

        [Test]
        [Description("Нажатие кнопки Apply обновляет счет игрока")]
        public void OnApplyButtonClick_UpdatesPlayerScore()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterCombining);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.ChosenButton!.Letter = 'А';
            _viewModel.OnKeyDown(null, new KeyEventArgs { Key = Key.Enter });
            var initialScore = _viewModel.FirstPlayer.Score;

            // Act
            _viewModel.OnApplyButtonClick(null, new RoutedEventArgs());

            // Assert
            Assert.That(_viewModel.FirstPlayer.Score, Is.EqualTo(initialScore + 1));
        }

        [Test]
        [Description("Нажатие кнопки Apply переключает ход")]
        public void OnApplyButtonClick_SwitchesPlayerTurn()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterCombining);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.ChosenButton!.Letter = 'А';
            _viewModel.OnKeyDown(null, new KeyEventArgs { Key = Key.Enter });
            var firstPlayerInitial = _viewModel.FirstPlayer.IsMakingMove;
            var secondPlayerInitial = _viewModel.SecondPlayer.IsMakingMove;

            // Act
            _viewModel.OnApplyButtonClick(null, new RoutedEventArgs());

            // Assert
            Assert.That(_viewModel.FirstPlayer.IsMakingMove, Is.EqualTo(!firstPlayerInitial));
            Assert.That(_viewModel.SecondPlayer.IsMakingMove, Is.EqualTo(!secondPlayerInitial));
        }

        [Test]
        [Description("Нажатие кнопки Apply увеличивает раунд")]
        public void OnApplyButtonClick_IncrementsRound()
        {
            // Arrange
            SetMode(GameWindowViewModel.GameMode.LetterCombining);
            _viewModel.Grid.SelectButton(0, 0, _viewModel).Invoke(null, new RoutedEventArgs());
            _viewModel.ChosenButton!.Letter = 'А';
            _viewModel.OnKeyDown(null, new KeyEventArgs { Key = Key.Enter });
            var initialRound = _viewModel.Round;

            // Act
            _viewModel.OnApplyButtonClick(null, new RoutedEventArgs());

            // Assert
            Assert.That(_viewModel.Round, Is.EqualTo(initialRound + 1));
        }
    }

    // Вспомогательный класс для создания TextChangedEventArgs
    public class TestTextChangedEventArgs : TextChangedEventArgs
    {
        public TestTextChangedEventArgs() : base(null) { }
    }
}