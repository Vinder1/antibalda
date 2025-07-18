using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia.Interactivity;
using NUnit.Framework;
using System;
using System.Reflection;

namespace AntiBaldaGame.Tests
{
    [TestFixture]
    public class LettersGridTests
    {
        private LettersGrid _grid;
        private GameWindowViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _grid = new LettersGrid();
            _viewModel = new GameWindowViewModel();

            // Установка режима через рефлексию
            var modeProperty = typeof(GameWindowViewModel).GetProperty("Mode");
            modeProperty?.SetValue(_viewModel, GameWindowViewModel.GameMode.LetterChoosing);
        }

        [Test]
        public void SelectButton_InLetterChoosingMode_SelectsEmptyCell()
        {
            // Arrange
            var button = _grid.Get(0, 0);
            button.Letter = '\0';

            // Act
            var handler = _grid.SelectButton(0, 0, _viewModel);
            handler.Invoke(null, new RoutedEventArgs());

            // Assert
            Assert.That(button.IsSelected, Is.True);
            Assert.That(_grid.SelectedRow, Is.EqualTo(0));
            Assert.That(_grid.SelectedColumn, Is.EqualTo(0));
        }

        [Test]
        public void SelectButton_InLetterChoosingMode_DoesNotSelectNonEmptyCell()
        {
            // Arrange
            var button = _grid.Get(0, 0);
            button.Letter = 'A';

            // Act
            var handler = _grid.SelectButton(0, 0, _viewModel);
            handler.Invoke(null, new RoutedEventArgs());

            // Assert
            Assert.That(button.IsSelected, Is.False);
            Assert.That(_grid.SelectedRow, Is.EqualTo(-1));
            Assert.That(_grid.SelectedColumn, Is.EqualTo(-1));
        }

        [Test]
        public void SelectButton_InLetterChoosingMode_DeselectsPreviousSelection()
        {
            // Arrange
            _grid.Get(0, 0).Letter = '\0';
            _grid.Get(1, 1).Letter = '\0';

            // Select first cell
            var handler1 = _grid.SelectButton(0, 0, _viewModel);
            handler1.Invoke(null, new RoutedEventArgs());

            // Act - select different cell
            var handler2 = _grid.SelectButton(1, 1, _viewModel);
            handler2.Invoke(null, new RoutedEventArgs());

            // Assert
            Assert.That(_grid.Get(0, 0).IsSelected, Is.False);
            Assert.That(_grid.Get(1, 1).IsSelected, Is.True);
            Assert.That(_grid.SelectedRow, Is.EqualTo(1));
            Assert.That(_grid.SelectedColumn, Is.EqualTo(1));
        }


        [Test]
        public void SelectButton_InWordBuildingMode_SelectsLetter()
        {
            // Arrange
            SetMode((GameWindowViewModel.GameMode)Enum.Parse(typeof(GameWindowViewModel.GameMode), "WordBuilding"));
            var button = _grid.Get(0, 0);
            button.Letter = 'A';
            button.IsSelected = false; // Начальное состояние

            // Act
            var handler = _grid.SelectButton(0, 0, _viewModel);
            handler.Invoke(null, new RoutedEventArgs());

            // Assert - проверяем только изменение состояния кнопки
            Assert.That(button.IsSelected, Is.True);
        }

        [Test]
        public void SelectButton_InWordBuildingMode_DeselectsLetterWhenClickedAgain()
        {
            // Arrange
            SetMode((GameWindowViewModel.GameMode)Enum.Parse(typeof(GameWindowViewModel.GameMode), "WordBuilding"));
            var button = _grid.Get(0, 0);
            button.Letter = 'A';
            button.IsSelected = true; // Начальное состояние - уже выбрана

            // Act
            var handler = _grid.SelectButton(0, 0, _viewModel);
            handler.Invoke(null, new RoutedEventArgs());

            // Assert
            Assert.That(button.IsSelected, Is.False);
        }

        [Test]
        public void SelectButton_InWordBuildingMode_DoesNotSelectEmptyCell()
        {
            // Arrange
            SetMode((GameWindowViewModel.GameMode)Enum.Parse(typeof(GameWindowViewModel.GameMode), "WordBuilding"));
            var button = _grid.Get(0, 0);
            button.Letter = ' '; // Пустая клетка
            button.IsSelected = false;

            // Act
            var handler = _grid.SelectButton(0, 0, _viewModel);
            handler.Invoke(null, new RoutedEventArgs());

            // Assert
            Assert.That(button.IsSelected, Is.False);
        }

        [Test]
        public void SelectButton_InWordBuildingMode_DoesNotDeselectWhiteLetters()
        {
            // Arrange
            SetMode((GameWindowViewModel.GameMode)Enum.Parse(typeof(GameWindowViewModel.GameMode), "WordBuilding"));
            var button = _grid.Get(0, 0);
            button.Letter = 'A';
            button.IsSelected = true;
            button.Color = CustomColors.White;

            // Act
            var handler = _grid.SelectButton(0, 0, _viewModel);
            handler.Invoke(null, new RoutedEventArgs());

            // Assert
            Assert.That(button.IsSelected, Is.True);
        }

        private void SetMode(GameWindowViewModel.GameMode mode)
        {
            var modeProperty = typeof(GameWindowViewModel).GetProperty("Mode");
            if (modeProperty != null && modeProperty.CanWrite)
            {
                modeProperty.SetValue(_viewModel, mode);
            }
            else
            {
                Assert.Inconclusive("Невозможно установить режим GameMode");
            }
        }
    }
}