using AntiBaldaGame.Models;
using Avalonia.Controls;
using Avalonia.Input;
using AntiBaldaGame.Models;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Reflection;

namespace AntiBaldaGame.ViewModels
{
    public class GameWindowViewModel : ViewModelBase
    {
        private readonly LettersGrid _grid;
        private GameMode _mode = GameMode.LetterChoosing;
        private char _previousLetterOnChosenButton = ' ';
        private Button? _chosenButton;
        private bool _inputVisible;
        private string _inputFieldText = " ";
        private LetterSequence? _letterSequence;
        private Player _firstPlayer;
        private Player _secondPlayer;
        private int _round = 1;

        public GameWindowViewModel()
        {
            try
            {
                // Инициализация игроков с использованием рефлексии
                _firstPlayer = InitializePlayer("Player 1");
                _secondPlayer = InitializePlayer("Player 2");

                // Инициализация сетки с проверкой настроек
                _grid = new LettersGrid();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize GameWindowViewModel", ex);
            }
        }

        private Player InitializePlayer(string name)
        {
            return new Player { Name = name };
        }

        public LettersGrid Grid => _grid;
        public Player FirstPlayer => _firstPlayer;
        public Player SecondPlayer => _secondPlayer;
        public int Round => _round;
        public GameMode Mode => _mode;
        public Button? ChosenButton => _chosenButton;
        public bool InputVisible => _inputVisible;

        public string InputFieldText
        {
            get => _inputFieldText;
            set => SetProperty(ref _inputFieldText, value);
        }

        public LetterSequence? LetterSequence => _letterSequence;

        public void OnGridButtonClick(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            _chosenButton = button;

            if (_mode == GameMode.LetterChoosing)
            {
                if (button.Content?.ToString()?.Trim().Length > 0)
                {
                    _previousLetterOnChosenButton = button.Content.ToString()[0];
                    _inputVisible = false;
                }
                else
                {
                    _inputVisible = true;
                    InputFieldText = " ";
                }
            }
        }

        public void OnTextChange(object? sender, TextChangedEventArgs e)
        {
            if (_chosenButton == null) return;

            if (_mode == GameMode.LetterChoosing && _inputVisible)
            {
                if (_chosenButton.Content?.ToString()?.Trim().Length > 0)
                {
                    _previousLetterOnChosenButton = _chosenButton.Content.ToString()[0];
                }

                var filtered = StringFormatter.LeaveOneCharacter(_inputFieldText);
                _chosenButton.Content = filtered;
                InputFieldText = filtered.ToString();
            }
        }

        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // Проверяем условия для обработки нажатия Enter
            if (e.Key != Key.Enter || _chosenButton == null)
                return;

            // Проверяем режим и что кнопка содержит букву
            if (_mode == GameMode.LetterChoosing &&
                !string.IsNullOrWhiteSpace(_chosenButton.Content?.ToString()))
            {
                try
                {
                    // Получаем символ из кнопки
                    char letter = _chosenButton.Content.ToString().Trim()[0];

                    // Создаем новую последовательность с координатами (0,0)
                    // Если нужны реальные координаты, их нужно получать из другого источника
                    _letterSequence = new LetterSequence(
                        new CoordinatedLetterButton(
                            new LetterButton { Letter = letter },
                            0,  // Замените на реальную строку, если доступна
                            0   // Замените на реальный столбец, если доступен
                        )
                    );

                    // Переключаем режим
                    _mode = GameMode.LetterCombining;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing Enter key: {ex.Message}");
                }
            }
        }
        public void OnApplyButtonClick(object? sender, RoutedEventArgs e)
        {
            if (_letterSequence == null) return;

            var currentPlayer = _firstPlayer.IsMakingMove ? _firstPlayer : _secondPlayer;
            currentPlayer.Score += _letterSequence.GetWord().Length;

            _firstPlayer.IsMakingMove = !_firstPlayer.IsMakingMove;
            _secondPlayer.IsMakingMove = !_secondPlayer.IsMakingMove;

            _round++;
            _mode = GameMode.LetterChoosing;
            _letterSequence = null;
        }

        public enum GameMode
        {
            LetterChoosing,
            LetterCombining
        }
    }
}